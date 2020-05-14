using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using System.IO;

namespace Smagribot.Function
{
    public static class SendDirectMethod
    {
        private class MethodResult {
            public int Status {get;set;}
            public object ResultPayload {get;set;}
        }

        private class DirectMethodCommand {
            public string DeviceId { get; set; }
            public string Method { get; set; }
            public string Payload { get; set; }
        }

        [FunctionName("SendDirectMethod")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<DirectMethodCommand>(requestBody);

            if(string.IsNullOrEmpty(request.DeviceId) || string.IsNullOrEmpty(request.Method) || string.IsNullOrEmpty(request.Payload))
                new BadRequestObjectResult("Malformed request!");

            log.LogInformation($"SendDirectMethod called: Device Id:{request.DeviceId} Method:{request.Method} Payload:{request.Payload}");

            var connectionString = Environment.GetEnvironmentVariable("IoTHubConnectionString");
            var client = ServiceClient.CreateFromConnectionString(connectionString);

            var c2dMethod = new CloudToDeviceMethod(request.Method, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
            if(!string.IsNullOrEmpty(request.Payload))
                c2dMethod.SetPayloadJson(request.Payload);

            var result = await client.InvokeDeviceMethodAsync(request.DeviceId, c2dMethod);
            var resultPayload = result.GetPayloadAsJson();
            dynamic data = JsonConvert.DeserializeObject(resultPayload);

            log.LogInformation($"Result: Status:{result?.Status} Payload:{resultPayload}");

            return (ActionResult)new OkObjectResult(new MethodResult(){ 
                Status = result.Status,
                ResultPayload = data
            });
        }
    }
}
