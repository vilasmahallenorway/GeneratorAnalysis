using Energy.Validation;
namespace Energy
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Ensure keys exist in App setting with physical path 
                bool folderPathIsExist = ValidateData.VerifyAppSetting();
                if (!folderPathIsExist)
                    return;

                // FileWatcher trigger event when xml file arrives in input folder
                FolderWatcher.WatcherSetup();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while reading or writing file", ex.Message);
            }
        }
    }
}
