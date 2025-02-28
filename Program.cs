using FileMonitor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileMonitor
{
    public class Program
    {
        static void Main(string[] args)
        {

            var host = new HostBuilder()
                        .ConfigureServices((context, services) =>
                        {
                            services.AddHostedService<StartUp>();
                        })
                        .UseConsoleLifetime()
                        .Build();
            host.Run();
        }
    }
}


public class StartUp : BackgroundService
{

    public StartUp()
    {
        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = Configuration.folderToMoniter;

        watcher.Filter = "*.*";

        watcher.Created += OnFileCreatedOrModified;

        watcher.EnableRaisingEvents = true;

    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    private static async void OnFileCreatedOrModified(object sender, FileSystemEventArgs e)
    {
        await (new Migrator()).UploadFileToSharePoint(Configuration.SiteId, Configuration.LibraryId, e.Name!, e.FullPath);
    }
}
