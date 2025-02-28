using Azure.Core;
using Azure.Identity;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace FileMonitor
{
    public class Migrator
    {
        public async Task UploadFileToSharePoint(string siteId, string driveId, string fileName, string filePath)
        {
            try
            {

                var versionBeforeUpload = await GetFileVersion(siteId, driveId, fileName);

                var uploadUrl = $"https://graph.microsoft.com/v1.0/sites/{siteId}/drives/{driveId}/items/root:/{fileName}:/content";
                byte[] fileBytes = File.ReadAllBytes(filePath);

                using (var httpClient = new HttpClient())
                {
                    var accessToken = await GetAccessTokenAsync();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, uploadUrl))
                    {
                        httpRequest.Content = new ByteArrayContent(fileBytes);
                        httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        using (var response = await httpClient.SendAsync(httpRequest))
                        {
                            response.EnsureSuccessStatusCode();
                            Console.WriteLine("File uploaded successfully.");
                        }
                    }
                }

                var versionAfterUpload = await GetFileVersion(siteId, driveId, fileName);

                if (Convert.ToDouble(versionAfterUpload) > Convert.ToDouble(versionBeforeUpload))
                {
                    deleteFileInSystem(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
            }
        }

        public void deleteFileInSystem(string filePath)
        {
            File.Delete(filePath);
        }

        public async Task<string> GetItemIdByName(string siteId, string driveId, string fileName)
        {
            var itemsUrl = $"https://graph.microsoft.com/v1.0/sites/{siteId}/drives/{driveId}/root/children?$filter=name eq '{fileName}'&$select=id";


            string fieldId = null;
            using (var httpClient = new HttpClient())
            {
                var accessToken = await GetAccessTokenAsync();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (var response = await httpClient.GetAsync(itemsUrl))
                {
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JObject.Parse(responseContent);

                    fieldId = jsonResponse?["value"]?.FirstOrDefault()?["id"].ToString()!;
                }

            }

            return fieldId;

        }

        public async Task<string> GetFileVersion(string siteId, string driveId, string fileName)
        {
            var fieldId = await GetItemIdByName(siteId, driveId, fileName);

            if (fieldId != null)
            {
                var fileVersionsUrl = $"https://graph.microsoft.com/v1.0/sites/{siteId}/drives/{driveId}/items/{fieldId}/versions?$top=1";

                using (var httpClient = new HttpClient())
                {
                    var accessToken = await GetAccessTokenAsync();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    using (var response = await httpClient.GetAsync(fileVersionsUrl))
                    {
                        response.EnsureSuccessStatusCode();

                        var responseContent = await response.Content.ReadAsStringAsync();
                        var jsonResponse = JObject.Parse(responseContent);

                        return jsonResponse?["value"]?.FirstOrDefault()?["id"].ToString()!;
                    }
                }
            }
            else
            {
                return "0";
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var clientId = Configuration.ClientId;
            var clientSecret = Configuration.clientSecret;
            var tenantId = Configuration.TenantId;

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);
            var tokenRequestContext = new TokenRequestContext(new string[] { "https://graph.microsoft.com/.default" });

            var accessTokenResult = await clientSecretCredential.GetTokenAsync(tokenRequestContext);

            return accessTokenResult.Token;
        }
    }
}


//using Azure.Identity;
//using Microsoft.Graph;
//using Microsoft.Graph.Models;

//namespace FileMonitor
//{
//    public class Migrator
//    {
//        private readonly GraphServiceClient _graphClient;

//        public Migrator()
//        {
//            var clientId = Configuration.ClientId;
//            var clientSecret = Configuration.clientSecret;
//            var tenantId = Configuration.TenantId;

//            var options = new TokenCredentialOptions
//            {
//                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
//            };

//            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

//            _graphClient = new GraphServiceClient(clientSecretCredential);
//        }

//        //public async Task UploadFileToSharePoint(string siteId, string driveId, string fileName, string filePath)
//        //{
//        //    try
//        //    {
//        //        var versionBeforeUpload = await GetFileVersion(siteId, driveId, fileName);

//        //        byte[] fileBytes = File.ReadAllBytes(filePath);

//        //        var uploadPath = $"/{fileName}";
//        //        var drive = await _graphClient.Sites[siteId].Drives[driveId].GetAsync();
//        //        var driveItem1 = drive?.Root. ItemWithPath(uploadPath).Content
//        //            .Request()
//        //            .PutAsync<DriveItem>(fileBytes); ;

//        //        //var driveItem = await _graphClient.Sites[siteId].Drives[driveId].Root.ItemWithPath(uploadPath).Content
//        //        //    .Request()
//        //        //    .PutAsync<DriveItem>(fileBytes);

//        //        Console.WriteLine("File uploaded successfully.");

//        //        var versionAfterUpload = await GetFileVersion(siteId, driveId, fileName);

//        //        if (Convert.ToDouble(versionAfterUpload) > Convert.ToDouble(versionBeforeUpload))
//        //        {
//        //            DeleteFileInSystem(filePath);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Console.WriteLine($"Error uploading file: {ex.Message}");
//        //    }
//        //}

//        public void DeleteFileInSystem(string filePath)
//        {
//            File.Delete(filePath);
//        }

//        public async Task<string> GetItemIdByName(string siteId, string driveId, string fileName)
//        {
//            try
//            {
//                var items = await _graphClient.Sites[siteId].Drives[driveId].Root.Children
//                    .Request()
//                    .Filter($"name eq '{fileName}'")
//                    .Select("id")
//                    .GetAsync();

//                var Drive = await _graphClient.Sites[siteId].Drives[driveId].GetAsync() 

//                    Root.Children
//                            .Request()
//                            .Filter($"name eq '{fileName}'")
//                            .Select("id")
//                            .GetAsync();
//                var item = await Drive.Root.Children.

//                return items.CurrentPage.FirstOrDefault()?.Id ?? string.Empty;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error retrieving item ID: {ex.Message}");
//                return null;
//            }
//        }

//        public async Task<string> GetFileVersion(string siteId, string driveId, string fileName)
//        {
//            try
//            {
//                var itemId = await GetItemIdByName(siteId, driveId, fileName);

//                if (!string.IsNullOrEmpty(itemId))
//                {
//                    var versions = await _graphClient.Sites[siteId].Drives[driveId].Items[itemId].Versions
//                        .Request()
//                        .Top(1)
//                        .GetAsync();

//                    return versions.CurrentPage.FirstOrDefault()?.Id ?? "0";
//                }

//                return "0";
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error retrieving file version: {ex.Message}");
//                return "0";
//            }
//        }
//    }
//}

