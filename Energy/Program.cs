using Energy.Utilities;
using Energy.Validation;
namespace Energy
{
    public class Program
    {
        static void Main(string[] args)
        {
            bool folderPathIsExist = ValidateData.VerifyAppSetting();
            if (!folderPathIsExist)
                return;

            // Create an instance of FolderWatcher and call WatcherSetup
            FolderWatcher folderWatcher = new();
            folderWatcher.WatcherSetup();
        }
    }
}
