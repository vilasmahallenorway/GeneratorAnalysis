using Energy.Utilities;

namespace Energy
{
    public class FolderWatcher
    {
        public static void WatcherSetup()
        {
            // Create a new FileSystemWatcher instance
            FileSystemWatcher watcher = new FileSystemWatcher();

            // Set the path to the folder you want to monitor
            watcher.Path = Common.InputFolderPath;

            // Monitor only XML files
            watcher.Filter = "*.xml";

            // Subscribe to the Created event
            watcher.Created += GenerateFile.OnCreated;

            // Begin watching the folder
            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Monitoring folder {Common.InputFolderPath}. Press any key to exit.");
            Console.ReadKey();

            // If you want to stop watching the folder when a key is pressed, set value to false
            watcher.EnableRaisingEvents = true;
            watcher.Dispose();
        }
    }
}