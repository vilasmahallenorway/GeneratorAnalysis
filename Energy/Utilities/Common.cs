using System.Configuration;

namespace Energy.Utilities
{
    public static class Common
    {
        public static string OutputFolderPath = ConfigurationManager.AppSettings[Constants.OUTPUT_FOLDER_PATH];
        public static string InputFolderPath = ConfigurationManager.AppSettings[Constants.INPUT_FOLDER_PATH];

        /// <summary>
        /// Remove all files from the specified path
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void RemoveXmlFiles(string directoryPath)
        {
            try
            {
                string[] xmlFiles = Directory.GetFiles(directoryPath, "*.xml");

                foreach (string file in xmlFiles)
                {
                    File.Delete(file);
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Removing files from {directoryPath} failed");
            }
        }

        /// <summary>
        /// Generate unique file name to avoid conflicts
        /// </summary>
        /// <returns></returns>
        public static string GenerateUniqueFileName()
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            // Create a file name using the current date and time along with milliseconds
            string fileName = string.Format("{0}-{1:D2}-{2:D2}_{3:D2}-{4:D2}-{5:D2}_{6:D3}.xml",
                now.Year, now.Month, now.Day,
                now.Hour, now.Minute, now.Second, now.Millisecond);

            return fileName;
        }
    }
}