using System.Xml.Linq;

namespace Energy.Utilities
{
    public class FolderWatcher
    {
        public void WatcherSetup()
        {
            // Create a new FileSystemWatcher instance
            FileSystemWatcher watcher = new FileSystemWatcher();

            // Set the path to the folder you want to monitor
            watcher.Path = Utility.InputFolderPath;

            // Monitor only XML files
            watcher.Filter = "*.xml";

            // Subscribe to the Created event
            watcher.Created += OnCreated;

            // Begin watching the folder
            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Monitoring folder {Utility.InputFolderPath}. Press any key to exit.");
            Console.ReadKey();

            // Stop watching the folder when a key is pressed
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }

        static void OnCreated(object sender, FileSystemEventArgs e)
        {
            // Process the newly created XML file
            Console.WriteLine($"Detected new XML file: {e.Name}");

            // XML processing logic here
            try
            {
                string xmlContent = File.ReadAllText(e.FullPath);
                if (string.IsNullOrEmpty(xmlContent)) return;

                XDocument doc = XDocument.Parse(xmlContent);
                XElement totals = Generator(doc);  //WindGenerator(doc);

                // Define the namespaces
                XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                XNamespace xsd = "http://www.w3.org/2001/XMLSchema";

                // Create the XML document with the root element and namespaces
                XDocument docs = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement(Constants.GENERATION_OUTPUT,
                        new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                        new XAttribute(XNamespace.Xmlns + "xsd", xsd), totals
                    )
                );

                docs.Save(Utility.OutputFolderPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing XML file: {ex.Message}");
            }
        }

        /// <summary>
        /// Processing input file perform calculations 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns>xElement</returns>
        private static XElement Generator(XDocument doc)
        {
            XElement totals = GenerateTotals(doc);
            XElement maxEmissionGenerators = GenerateMaxEmissionGenerators(doc);

            totals.Add(maxEmissionGenerators);

            return totals;
        }

        static XElement GenerateTotals(XDocument doc)
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

        static IEnumerable<XElement> GetGenerators(XDocument doc, string generatorType, Func<XElement, double> calculationFunction)
        {
            return doc.Descendants(generatorType)
                      .Select(generator => new XElement(Constants.GENERATOR,
                          new XElement(Constants.NAME, generator.Element(Constants.NAME).Value),
                          new XElement(Constants.TOTAL, calculationFunction(generator))));
        }

        static IEnumerable<XElement> GetDays(XDocument doc, string generatorType, Func<XElement, double, double> calculationFunction)
        {

            double calculateDayEmissions = generatorType == Constants.COAL_GENERATOR ? double.Parse((string)doc.Descendants(Constants.COAL_GENERATOR).First().Element(Constants.EMISSIONS_RATING)) : double.Parse((string)doc.Descendants(Constants.GAS_GENERATOR).First().Element(Constants.EMISSIONS_RATING));

            return doc.Descendants(generatorType)
                      .Descendants(Constants.DAY)
                      .Select(day => new
                      {
                          Name = (string)doc.Descendants(generatorType).First().Element(Constants.NAME),
                          Date = (string)day.Element(Constants.DATE),
                          Emission = calculationFunction(day, calculateDayEmissions)
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

        static double CalculateCoalDayEmissions(XElement day, double calculateCoalDayEmissions)
        {
            return double.Parse((string)day.Element(Constants.ENERGY)) * calculateCoalDayEmissions * 0.812;
        }

        static double CalculateGasDayEmissions(XElement day, double calculateGasDayEmissions)
        {
            return double.Parse((string)day.Element(Constants.ENERGY)) * calculateGasDayEmissions * 0.562;
        }
    }
}