using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Serilog.Extensions.Logging;

namespace Web.App_Start
{
    public class MyAutofacModule : Module
    {
        public bool UseMELLogger { get; set; }
        protected override void Load(ContainerBuilder builder)
        {
            if (UseMELLogger)
            {

            }
            var loggerFactory = LoggerFactory.Create(builder1 =>
            {

                builder1
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("WAController", LogLevel.Debug);
                //.AddConsole()
                //.AddDebug()
                //.AddEventLog(eventLogSettings =>
                //{
                //    eventLogSettings.SourceName = Consts.ProjectName;
                //    eventLogSettings.Filter = (str, level) =>
                //    {
                //        //if ((str == "Default" || str == "LogWebApiActionFilter") && level == LogLevel.Trace)
                //        //    if (level == LogLevel.Trace)
                //        return false;
                //        //     else
                //        //  return true;
                //    };
                //}
                //)
                //.AddFilter<EventLogLoggerProvider>("", LogLevel.Trace);
                // 引入Serilog作为Provider，主要为了文件输出
                SerilogLoggingBuilderExtensions.AddSerilog(builder1, Log.Logger);
            }); 
            //注册ILoggerFactory比注册ILogger更灵活，传入Controller后CreateLogger可自行传名字
            builder.RegisterInstance(loggerFactory).As<ILoggerFactory>();

            #region 注册ILogger
            //ILogger logger = loggerFactory.CreateLogger("MyLogger");
            //builder.RegisterInstance(logger).As<ILogger>().SingleInstance();
            #endregion


            //builder.RegisterType<SerilogConfig>().PropertiesAutowired().AsSelf();

            //Consts.loggerFactory = loggerFactory;
            //builder.RegisterInstance(loggerFactory).As<ILoggerFactory>();
            //builder.RegisterType<LogWebApiActionFilter>().PropertiesAutowired();
            //builder.RegisterLogger();
        }
    }
}