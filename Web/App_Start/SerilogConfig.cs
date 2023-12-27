using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using WengAn;

namespace Web.App_Start
{
    public class SerilogConfig
    {
        public static void Register()
        {
#if OWIN
            string localFilePath = Path.Combine(Directory.GetCurrentDirectory(),
                       ConfigurationManager.AppSettings["BentleyApiLogFilePath"], "Log.txt");
#else
            string localFilePath =
                System.Web.Hosting.HostingEnvironment.MapPath($"~/{ConfigurationManager.AppSettings["BentleyApiLogFilePath"]}/Log.txt");
#endif

            string udpAddress = ConfigurationManager.AppSettings["SerilogUdpLogReceiverAddress"];
            // { Properties} can include any other enrichment which is applied.
            //The {Message:lj} format options cause data embedded in the message to be output in JSON (j) except for string literals, which are output as-is.
            string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj} {Properties}{NewLine}{Exception}";
            string logDestination = ConfigurationManager.AppSettings["LogFileDestination"];

            //Azure环境 Key Vault, Blob, File Share支持
            if (logDestination == "Azure")
            {
                //write to remote azure share folder 按机器名区分文件夹
                //WriteToAzureFileShare();
                string storageContainerName = "log-nuopai-wengan";

                #region Via UserIdentity
                string userIdentityclientId = ConfigurationManager.AppSettings["AzureUserAssignedClientId"];
                var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userIdentityclientId });

                #endregion

                //var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                //var credential = new DefaultAzureCredential();
                var blobClient = new BlobServiceClient(new Uri("https://storagebentley.blob.core.windows.net"), credential);
                //var client = new SecretClient(new Uri("https://bentley.vault.azure.net/"), credential);

                Log.Logger = new LoggerConfiguration()
                    .Enrich.WithRequestHeader("User-Agent")
                    .Enrich.WithCorrelationId(headerName: "correlation-id", addValueIfHeaderAbsence: false)
                    .Enrich.WithClientIp()
                    .Enrich.WithThreadId()
                    .MinimumLevel.Debug() //Logging level要高于或等于sink level才行。否则没效果。
                                          //.WriteTo.Console()//sink 可以设置MinimumLevel  但必须高于logging level
                    .WriteTo.File(localFilePath, outputTemplate: outputTemplate, rollingInterval: RollingInterval.Day)
                    .WriteTo.EventLog(Consts.ProjectName, manageEventSource: true, outputTemplate: outputTemplate)
                    //.WriteTo.Udp(udpAddress, 7071, AddressFamily.InterNetwork, outputTemplate: outputTemplate)
                    .WriteTo.ApplicationInsights(
                        new TelemetryConfiguration(ConfigurationManager.AppSettings["ApplicationInsightsKey"]),
                        TelemetryConverter.Traces)
                    .WriteTo.AzureBlobStorage(blobClient,
                            Serilog.Events.LogEventLevel.Debug, storageContainerName,
                            $"{{yyyy}}_{{MM}}/{{dd}}/{Environment.MachineName}_Log.txt",
                            writeInBatches: true, period: TimeSpan.FromSeconds(3), batchPostingLimit: 10)
                    //因为appendblob 没有confirm write https://github.com/chriswill/serilog-sinks-azureblobstorage/issues/32 必须用async包住，或者改为批量写 不然卡住不写入
                    // azure data lake Gen2 Preview之前不支持Append Blob 需要新的SDK https://azure.microsoft.com/en-us/updates/append-blob-support-for-azure-data-lake-storage-preview/
                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.WithRequestHeader("User-Agent")
                    .Enrich.WithCorrelationId(headerName: "correlation-id", addValueIfHeaderAbsence: false)
                    .Enrich.WithClientIp()
                    .Enrich.WithThreadId()
                    .MinimumLevel.Debug() //Logging level要高于或等于sink level才行。否则没效果。
                    .WriteTo.Debug()//
                    .WriteTo.Console()//sink 可以设置MinimumLevel, 但必须高于logging level
                    .WriteTo.File(localFilePath, outputTemplate: outputTemplate, rollingInterval: RollingInterval.Day)
                    .WriteTo.EventLog(Consts.ProjectName, manageEventSource: true, outputTemplate: outputTemplate)
                    //.WriteTo.Udp(udpAddress, 7071, AddressFamily.InterNetwork, outputTemplate: outputTemplate)
                    //.WriteTo.ApplicationInsights(
                    //    new TelemetryConfiguration(ConfigurationManager.AppSettings["ApplicationInsightsKey"]),
                    //    TelemetryConverter.Traces)
                    .CreateLogger();
            }
            Log.Logger.Information("Serilog配置成功，开始记录日志");
        }
    }
}