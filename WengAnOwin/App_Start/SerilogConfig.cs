using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using WengAn;
using WengAnOwin;

namespace Web.App_Start
{
    public class SerilogConfig
    {
        public static void Register()
        {
            string localFilePath = Path.Combine(Directory.GetCurrentDirectory(),
                ConfigurationManager.AppSettings["BentleyApiLogFilePath"], "Log.txt");
            string udpAddress = "172.18.208.1";
            //The {Message:lj} format options cause data embedded in the message to be output in JSON (j) except for string literals, which are output as-is.
            string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}";
            string logDestination = ConfigurationManager.AppSettings["LogFileDestination"];

            //Azure环境 Key Vault, Blob, File Share支持
            if (logDestination == "Azure")
            {
                //write to remote azure share folder 按机器名区分文件夹
                //WriteToAzureFileShare();
                //const string userIdentityclientId = "";
                //var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userIdentityclientId });
                //var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                //var client = new BlobServiceClient(new Uri("https://storagebentley.blob.core.windows.net"), credential);

                //var clients = new SecretClient(new Uri("https://bentleykeyvault.vault.azure.net/"), credential);
                //var asda = clients.GetSecret("ConnectionStrings--storagebentley").Value.Value;

                Log.Logger = new LoggerConfiguration()
                    //.Enrich.WithCorrelationId()
                    .Enrich.WithCorrelationIdHeader()
                    .Enrich.WithClientIp()
                    //.Enrich.WithClientAgent()
                    .Enrich.WithThreadId()

                    .MinimumLevel.Debug() //Logging level要高于或等于sink level才行。否则没效果。
                                          //.WriteTo.Console()//sink 可以设置MinimumLevel  但必须高于logging level
                    .WriteTo.File(localFilePath, outputTemplate: outputTemplate, rollingInterval: RollingInterval.Day)
                    .WriteTo.EventLog(Consts.ProjectName, manageEventSource: true, outputTemplate: outputTemplate)
                    //.WriteTo.Udp(udpAddress, 7071, AddressFamily.InterNetwork, outputTemplate: outputTemplate)
                    .WriteTo.ApplicationInsights(new TelemetryConfiguration(ConfigurationManager.AppSettings["ApplicationInsightsKey"]), TelemetryConverter.Traces)
                    //.WriteTo.Async(x =>
                    //    x.AzureBlobStorage(client,
                    //        Serilog.Events.LogEventLevel.Information, "wenganlog",
                    //        $"{{yyyy}}_{{MM}}/{{dd}}/{Environment.MachineName}_Log.txt"))
                    //因为是appendblob 必须用async包下 不然卡住了。。。
                    // azure data lake Gen2 Preview之前不支持Append Blob 需要新的SDK https://azure.microsoft.com/en-us/updates/append-blob-support-for-azure-data-lake-storage-preview/

                    .CreateLogger();

            }
            else
            {

                Log.Logger = new LoggerConfiguration()
                    //.Enrich.WithCorrelationId()
                    .Enrich.WithCorrelationIdHeader()
                    .Enrich.WithClientIp()
                    //.Enrich.WithClientAgent()
                    .Enrich.WithThreadId()

                    .MinimumLevel.Debug() //Logging level要高于或等于sink level才行。否则没效果。
                    .WriteTo.Console()//sink 可以设置MinimumLevel  但必须高于logging level
                    .WriteTo.File(localFilePath, outputTemplate: outputTemplate, rollingInterval: RollingInterval.Day)
                    .WriteTo.EventLog(Consts.ProjectName, manageEventSource: true, outputTemplate: outputTemplate)
                    //.WriteTo.Udp(udpAddress, 7071, AddressFamily.InterNetwork, outputTemplate: outputTemplate)
                    .CreateLogger();
            }

            Log.Logger.Information("Serilog配置成功，开始记录日志");
        }
    }
}