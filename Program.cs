using FileMonitor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileMonitor
{
    public class Program
    {
        static void Main(string[] args)
        {

            var host = new HostBuilder()
                        .ConfigureAppConfiguration((context, config) =>
                        {
                           config.SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        })
                        .ConfigureServices((context, services) =>
                        {
                            services.AddSingleton<IConfiguration>(context.Configuration);
                            services.AddSingleton<Configuration>();
                            services.AddSingleton<Migrator>();
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

    private readonly Configuration _configuration;
    private readonly Migrator _migrator;

    public StartUp(Configuration configuration, Migrator migrator)
    {
        _configuration = configuration;
        _migrator = migrator;


        FileSystemWatcher watcher = new FileSystemWatcher();
        
        watcher.Path = _configuration.FolderToMonitor;
        
        watcher.Filter = "*.*"; 

        watcher.Created += OnFileCreatedOrModified;

        watcher.EnableRaisingEvents = true;

    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    private async void OnFileCreatedOrModified(object sender, FileSystemEventArgs e)
    {

        if ( e.Name == _configuration.TargetFileName || _configuration.TargetFileName == String.Empty)
        {
            await _migrator.UploadFileToSharePoint(_configuration.SiteId, _configuration.LibraryId, e.Name!, e.FullPath);
        }
    }
}
