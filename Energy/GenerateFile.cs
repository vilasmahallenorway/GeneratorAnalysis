﻿using Energy.Utilities;
using System.Xml.Linq;
namespace Energy
{
    public static class GenerateFile
    {
        /// <summary>
        /// FileWatcher event triggers when file is arrive in specified input folder   
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnCreated(object sender, FileSystemEventArgs e)
        {
            // Process the newly created XML file
            Console.WriteLine($"Detected new XML file: {e.Name}");

            // XML processing logic
            try
            {
                string xmlContent = File.ReadAllText(e.FullPath);
                if (string.IsNullOrEmpty(xmlContent)) return;

                XDocument doc = XDocument.Parse(xmlContent);
                XElement totals = Generator(doc);

             
                // Create the XML document with the root element and namespaces
                XDocument docs = CreateDocument(totals);

                // Save file to output folder specified in appsetting
                string outputFilePath = Path.Combine(Common.OutputFolderPath, Common.GenerateUniqueFileName());
                docs.Save(outputFilePath);

                // after creating file in output folder , remove all xml files from input folder 
                Common.RemoveXmlFiles(Common.InputFolderPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing XML file: {ex.Message}");
            }
        }

        /// <summary>
        /// Create final xml document
        /// </summary>
        /// <param name="totals"></param>
        /// <param name="xsi"></param>
        /// <param name="xsd"></param>
        /// <returns></returns>
        private static XDocument CreateDocument(XElement totals)
        {
            // Define the namespaces
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace xsd = "http://www.w3.org/2001/XMLSchema";

            return new(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(Constants.GENERATION_OUTPUT,
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(XNamespace.Xmlns + "xsd", xsd), totals
                )
            );
        }

        /// <summary>
        /// Processing input file perform calculations 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns>xElement</returns>
        private static XElement Generator(XDocument doc)
        {
            XElement totals = CalculateDailyGenerationsForOilCoilGas(doc);

            XElement maxEmissionGenerators = GenerateMaxEmissionGenerators(doc);
            totals.Add(maxEmissionGenerators);

            XElement maxHeatGenerators = HeatDetailsGenerators(doc);
            totals.Add(maxHeatGenerators);

            return totals;
        }

        static XElement CalculateDailyGenerationsForOilCoilGas(XDocument doc)
        {
            var windGenerators = GetGenerators(doc, Constants.WIND_GENERATOR, CalculateWindGenerationValue);
            var gasGenerators = GetGenerators(doc, Constants.GAS_GENERATOR, CalculateGasEmissions);
            var coalGenerators = GetGenerators(doc, Constants.COAL_GENERATOR, CalculateCoalEmissions);

            var totals = new XElement(Constants.TOTAL);
            AddGeneratorsToElement(windGenerators, totals);
            AddGeneratorsToElement(gasGenerators, totals);
            AddGeneratorsToElement(coalGenerators, totals);

            return totals;
        }
        static XElement GenerateMaxEmissionGenerators(XDocument doc)
        {
            var coalDays = GetDays(doc, Constants.COAL_GENERATOR, CalculateCoalDayEmissions);
            var gasDays = GetDays(doc, Constants.GAS_GENERATOR, CalculateGasDayEmissions);

            var maxEmissionGenerators = new XElement(Constants.MAXEMISSION_GENERATORS);
            AddDaysToElement(coalDays, maxEmissionGenerators);
            AddDaysToElement(gasDays, maxEmissionGenerators);

            return maxEmissionGenerators;
        }
        static XElement HeatDetailsGenerators(XDocument doc)
        {
            var heatElements = doc.Descendants(Constants.COAL_GENERATOR)
                  .Select(generator => new XElement(Constants.ACTUALHEAT_RATE,
                      new XElement(Constants.NAME, generator.Element(Constants.NAME).Value),
                      new XElement(Constants.HEAT_RATE, CalculateActualCoalHeatRate(double.Parse(generator.Element(Constants.TOTALHEATINPUT).Value), double.Parse(generator.Element(Constants.ACTUALNETGENERATION).Value)))
                     ));

            var heatGenerators = new XElement(Constants.ACTUALHEAT_RATES);
            AddDaysToElement(heatElements, heatGenerators);
            return heatGenerators;
        }

        static IEnumerable<XElement> GetGenerators(XDocument doc, string generatorType, Func<XElement, double> calculationFunction)
        {
            return doc.Descendants(generatorType)
                      .Select(generator => new XElement(Constants.GENERATOR,
                          new XElement(Constants.NAME, generator.Element(Constants.NAME).Value),
                          new XElement(Constants.TOTAL, calculationFunction(generator))));

        }

        static IEnumerable<XElement> GetDays(XDocument doc, string generatorType, Func<XElement, double, double> calculationFunction)
        {

            double dayEmissions = generatorType == Constants.COAL_GENERATOR ? double.Parse((string)doc.Descendants(Constants.COAL_GENERATOR).First().Element(Constants.EMISSIONS_RATING)) : double.Parse((string)doc.Descendants(Constants.GAS_GENERATOR).First().Element(Constants.EMISSIONS_RATING));

            return doc.Descendants(generatorType)
                      .Descendants(Constants.DAY)
                      .Select(day => new
                      {
                          Name = (string)doc.Descendants(generatorType).First().Element(Constants.NAME),
                          Date = (string)day.Element(Constants.DATE),
                          Emission = calculationFunction(day, dayEmissions)
                      })
                      .Where(day => day.Emission > 0)
                      .Select(day => new XElement(Constants.DAY,
                          new XElement(Constants.NAME, day.Name),
                          new XElement(Constants.DATE, day.Date),
                          new XElement(Constants.EMISSION, day.Emission)));
        }

        static void AddGeneratorsToElement(IEnumerable<XElement> generators, XElement element)
        {
            foreach (var generator in generators)
            {
                element.Add(generator);
            }
        }

        static void AddDaysToElement(IEnumerable<XElement> days, XElement element)
        {
            foreach (var day in days)
            {
                element.Add(day);
            }
        }

        static double CalculateWindGenerationValue(XElement generator)
        {
            string generatorName = generator.Element(Constants.NAME).Value;
            var days = generator.Descendants(Constants.DAY);

            double multiplier = generatorName.Contains(Constants.OFFSHORE) ? 0.265 : 0.946;
            return days.Sum(day =>
            {
                double energy = double.Parse(day.Element(Constants.ENERGY).Value);
                double price = double.Parse(day.Element(Constants.PRICE).Value);
                return energy * price * multiplier;
            });
        }

        static double CalculateGasEmissions(XElement generator)
        {
            var days = generator.Descendants(Constants.DAY);
            return days.Sum(day =>
            {
                double energy = double.Parse(day.Element(Constants.ENERGY).Value);
                double price = double.Parse(day.Element(Constants.PRICE).Value);
                return energy * price * 0.696;
            });
        }

        static double CalculateCoalEmissions(XElement generator)
        {
            var days = generator.Descendants(Constants.DAY);
            return days.Sum(day =>
            {
                double energy = double.Parse(day.Element(Constants.ENERGY).Value);
                double price = double.Parse(day.Element(Constants.PRICE).Value);
                return energy * price * 0.696;
            });
        }

        static double CalculateActualCoalHeatRate(double totalHeatInput, double actualNetGeneration)
        {
            return totalHeatInput / actualNetGeneration;
        }

        static double CalculateCoalDayEmissions(XElement day, double coalDayEmissions)
        {
            return double.Parse((string)day.Element(Constants.ENERGY)) * coalDayEmissions * 0.812;
        }

        static double CalculateGasDayEmissions(XElement day, double calculateGasDayEmissions)
        {
            return double.Parse((string)day.Element(Constants.ENERGY)) * calculateGasDayEmissions * 0.562;
        }
    }
}
