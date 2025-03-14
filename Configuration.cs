using Microsoft.Extensions.Configuration;

namespace FileMonitor
{
    public class Configuration
    {
        private readonly IConfiguration _configuration;

        public Configuration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string FolderToMonitor
        {
            get => _configuration["FolderToMonitor"]!;
        }

        public string ClientId
        {
            get => _configuration["ClientId"]!;
        }

        public string TenantId
        {
            get => _configuration["TenantId"]!;
        }

        public string ClientSecret
        {
            get => _configuration["ClientSecret"]!;
        }

        public string SiteId
        {
            get => _configuration["SiteId"]!;
        }

        public string LibraryId
        {
            get => _configuration["LibraryId"]!;
        }

        public bool AutoDeleteSrc
        {
            get => Convert.ToBoolean(_configuration["AutoDeleteSrc"]!) || false;
        }

        public bool OnlyMigrateTarget
        {
            get => Convert.ToBoolean(_configuration["OnlyMigrateTarget"]!) || false;
        }

        public string TargetFileName
        {
            get => _configuration["TargetFileName"]!;
        }

    }
}
