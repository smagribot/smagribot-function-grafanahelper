using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Text;

namespace Samgribot.Function
{
    public class LogEntry
    {
        public string DeviceId {get;set;}
        public string Activity {get;set;}
        public string Log {get;set;}
    }

    public static class AddLog
    {
        private const string AddLogSql = "INSERT INTO dbo.Log (DeviceId, Time, Activity, Log) VALUES (@deviceId, GETDATE(), @activiy, @log);";

        [FunctionName("AddLog")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var logEntry = JsonConvert.DeserializeObject<LogEntry>(requestBody);

            log.LogInformation($"AddLog called - DeviceId: {logEntry.DeviceId} Activity: {logEntry.Activity} Log: {logEntry.Log}");
           
            var str = Environment.GetEnvironmentVariable("DBConnection");

            using (var conn = new SqlConnection(str))
            {
                conn.Open();

                using (var cmd = new SqlCommand(AddLogSql, conn))
                {
                    cmd.Parameters.Add("@deviceId", System.Data.SqlDbType.NVarChar).Value = logEntry.DeviceId;
                    cmd.Parameters.Add("@activiy", System.Data.SqlDbType.NVarChar).Value = logEntry.Activity;
                    cmd.Parameters.Add("@log", System.Data.SqlDbType.NVarChar).Value = logEntry.Log;

                    var rows = await cmd.ExecuteNonQueryAsync();
                    log.LogInformation($"{rows} rows were updated");
                }
            }

            return new OkResult();
        }
    }
}
