namespace FileMonitor
{
    public class Program
    {
        static void Main(string[] args)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Configuration.folderToMoniter;

            watcher.Filter = "*.*";

            watcher.Created += OnFileCreatedOrModified;
            
            watcher.EnableRaisingEvents = true;
            Console.WriteLine("Monitoring folder for changes. Press [Enter] to exit.");


            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter) break;
                }
            }
        }

        private static async void OnFileCreatedOrModified(object sender, FileSystemEventArgs e)
        {
            //await Task.Delay(1000);
            await (new Migrator()).UploadFileToSharePoint(Configuration.SiteId, Configuration.LibraryId, e.Name!, e.FullPath);
        }
    }
}
