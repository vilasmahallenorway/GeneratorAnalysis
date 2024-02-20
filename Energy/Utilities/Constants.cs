using System.Configuration;

namespace Energy.Utilities
{
    public static class Constants
    {
        public const string INPUT_FOLDER_PATH = "InputFolder";
        public const string OUTPUT_FOLDER_PATH = "OutputFolder";

        public const string ENERGY = "Energy";
        public const string PRICE = "Price";
        public const string NAME = "Name";
        public const string GENERATOR = "Generator";

        public const string COAL_GENERATOR = "CoalGenerator";
        public const string GAS_GENERATOR = "GasGenerator";
        public const string WIND_GENERATOR = "WindGenerator";
        public const string MAXEMISSION_GENERATORS = "MaxEmissionGenerators";
        public const string EMISSIONS_RATING = "EmissionsRating";
        public const string DAY = "Day";
        public const string DATE = "Date";
        public const string OFFSHORE = "Offshore";
        public const string TOTAL = "Total";
        public const string EMISSION = "Emission";
        public const string GENERATION_OUTPUT= "GenerationOutput";
        public const string FOLDER_PATH_NOTEXIST = "Please specify Input and Output folder path in AppSetting file";
    }
    public static class Utility
    {
        public static string OutputFolderPath = ConfigurationManager.AppSettings[Constants.OUTPUT_FOLDER_PATH];
        public static string InputFolderPath = ConfigurationManager.AppSettings[Constants.INPUT_FOLDER_PATH];
    }
}
