using System.IO;
namespace Energy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Read input folder location from configuration file
            string inputFolder = ReadInputFolderFromConfig();

            // Create a new FileSystemWatcher instance
            FileSystemWatcher watcher = new FileSystemWatcher();

            // Set the path to the folder you want to monitor
            watcher.Path = inputFolder;

            // Monitor only XML files
            watcher.Filter = "*.xml";

            // Subscribe to the Created event
            watcher.Created += OnCreated;

            // Begin watching the folder
            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Monitoring folder {inputFolder}. Press any key to exit.");
            Console.ReadKey();

            // Stop watching the folder when a key is pressed
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }

        static void OnCreated(object sender, FileSystemEventArgs e)
        {
            // Process the newly created XML file
            Console.WriteLine($"Detected new XML file: {e.Name}");

            // Add your XML processing logic here
            // For example, you can read the contents of the XML file
            // using XmlDocument or XDocument classes
            try
            {
                string xmlContent = File.ReadAllText(e.FullPath);
                Console.WriteLine($"XML Content: {xmlContent}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing XML file: {ex.Message}");
            }
        }

        static string ReadInputFolderFromConfig()
        {
            // Read input folder location from configuration file
            // You can implement your own logic to read from a configuration file
            // For simplicity, I'm hardcoding the folder path here
            return @"C:\Path\To\Input\Folder";
        }
    }


}
