using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System.Web;

namespace Smagribot.Function
{
    public static class PatchTwinInfo
    {
        [FunctionName("PatchTwinInfo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string deviceId = HttpUtility.UrlDecode(req.Query["deviceid"]);
            string etag = HttpUtility.UrlDecode(req.Query["etag"]);
            string patchData = HttpUtility.UrlDecode(req.Query["patch"]);

            if(string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(patchData) || string.IsNullOrEmpty(etag))
                new BadRequestObjectResult("Malformed request!");

            log.LogInformation($"PatchTwinInfo called: Device Id:{deviceId} - etag: {etag}");

            var connectionString = Environment.GetEnvironmentVariable("IoTHubConnectionString");
            var client = RegistryManager.CreateFromConnectionString(connectionString);

            var updatedTwin = await client.UpdateTwinAsync(deviceId, patchData, etag);
            return (ActionResult)new OkObjectResult(updatedTwin.ToJson());
        }
    }
}
