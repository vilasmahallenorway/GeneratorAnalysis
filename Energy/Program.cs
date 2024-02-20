using Energy.Validation;
namespace Energy
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Ensure keys exist in App setting with mentioned physical path
                bool folderPathIsExist = ValidateData.VerifyAppSetting();
                if (!folderPathIsExist)
                    return;

                // Call FileWatcher if xml file is exist
                FolderWatcher.WatcherSetup();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while reading or writing file", ex.Message);
            }
        }
    }
}
