using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Devices;

namespace SmagriBot.Function
{
    public static class GetTwinInfo
    {
        [FunctionName("GetTwinInfo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string deviceId = req.Query["deviceid"];

            if(string.IsNullOrEmpty(deviceId))
                new BadRequestObjectResult("Malformed request!");

            log.LogInformation($"GetTwinInfo called: Device Id:{deviceId}");

            var connectionString = Environment.GetEnvironmentVariable("IoTHubConnectionString");
            var client = RegistryManager.CreateFromConnectionString(connectionString);

            var twin = await client.GetTwinAsync(deviceId);
            return (ActionResult)new OkObjectResult(twin.ToJson());
        }
    }
}
