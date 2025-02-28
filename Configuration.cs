//using Microsoft.Extensions.Configuration;

//namespace FileMonitor
//{
//    public class Configuration
//    {
//        //public static string folderToMoniter = @"C:\PROJ\FileChecker\FileMonitor\";

//        //public const string ClientId = "c85d544f-2612-4fa4-a4a0-23a2ad96f1ed";

//        //public const string TenantId = "e4b96893-f561-4636-a52f-b29896fe1cc1";

//        //public const string clientSecret = "YJ68Q~lNSOWOLRMirjBfgR76JJdSyULZvF7FQcrW";

//        //public const string SiteId = "zkny4.sharepoint.com,f1ff0bb4-3cfc-4636-9b66-861033d49886,a2e468e0-84fa-4205-8fa8-74c8f83e7668";

//        //public const string LibraryId = "b!tAv_8fw8NkabZoYQM9SYhuBo5KL6hAVCj6h0yPg-dmi3S-vU3PKyRLFAIUfDwNRV";
//    }
//}



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
