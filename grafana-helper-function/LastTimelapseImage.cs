using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace Smagribot.Function
{
    public static class LastTimelapseImage
    {
        [FunctionName("LastTimelapseImage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("BlobConnectionString");
            var blobContainerName = Environment.GetEnvironmentVariable("BlobContainerName");

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            var newestItemBlob = containerClient.GetBlobs().OrderByDescending(b => b.Properties.LastModified).FirstOrDefault();

            if(newestItemBlob == null)
                return new NotFoundResult();

            log.LogInformation($"Returning image: {newestItemBlob.Name}");

            var blobClient = containerClient.GetBlobClient(newestItemBlob.Name);
            var memStream = new MemoryStream();
            await blobClient.DownloadToAsync(memStream);
            memStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memStream, "image/jpeg");
        }
    }
}
