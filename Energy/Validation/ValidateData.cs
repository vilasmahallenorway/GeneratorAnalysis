using Energy.Utilities;
namespace Energy.Validation
{
    public static class ValidateData
    {
        /// <summary>
        ///  Verify If Appsetting keys are properly configured
        /// </summary>
        /// <returns>bool</returns>
        public static bool VerifyAppSetting()
        {
            bool isValid = true;
            try
            {
                // Check if keys value is not null or empty
                if (string.IsNullOrEmpty(Utility.InputFolderPath) || string.IsNullOrEmpty(Utility.OutputFolderPath))
                {
                    isValid = false;
                    Console.WriteLine(Constants.FOLDER_PATH_NOTEXIST);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex.Message}");
            }
            return isValid;
        }
    }
}
