using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Serilog;

namespace LiaoDongBay.App_Start
{
    public class SerilogConfig
    {
        public static void RegisterComponents()
        {
            string filePath;
            //Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["BentleyApiLogFilePath"]));
            string logFullPath = ConfigurationManager.AppSettings["BentleyApiLogFilePath"];
            if (HostingEnvironment.IsDevelopmentEnvironment)
            {
                const string localLogFolder = "Logs";
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, localLogFolder, "Log.txt");
            }
            else
            {//write to remote azure share folder 按机器名区分文件夹
                //filePath = Path.Combine(logFullPath, Environment.MachineName, "Log.txt");
                var asda = HostingEnvironment.MapPath("~/Logs");
                filePath = Path.Combine(asda, "Log.txt");
            }
            
            //The {Message:lj} format options cause data embedded in the message to be output in JSON (j) except for string literals, which are output as-is.
            string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                    //.Enrich.WithCorrelationId()
                    .Enrich.WithCorrelationIdHeader()
                    .Enrich.WithClientIp()
                  //.Enrich.WithClientAgent()
                  .Enrich.WithThreadId()

                .MinimumLevel.Debug() //Logging level要高于或等于sink level才行。否则没效果。
                                      //.WriteTo.Console()//sink 可以设置MinimumLevel  但必须高于logging level
                .WriteTo.File(filePath, outputTemplate: outputTemplate, rollingInterval: RollingInterval.Day)
                //.WriteTo.EventLog(Consts.ProjectName, manageEventSource: true, outputTemplate: outputTemplate)
                //.WriteTo.Udp("172.18.208.1", 7071, AddressFamily.InterNetwork, outputTemplate: outputTemplate)

                .CreateLogger();

        }
    }
}