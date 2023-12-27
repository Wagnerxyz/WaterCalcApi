using Autofac;
using Microsoft.Extensions.Logging;
using Serilog;
using Web.Controllers;

namespace Web.App_Start
{
    public class MyAutoFacModule : Module
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
                    .AddFilter(nameof(WAController), LogLevel.Debug)
                    .AddSerilog(Log.Logger); // 引入Serilog作为Provider，主要为了文件输出
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

            });

            //向DI注册ILoggerFactory比注册ILogger更灵活，传入Controller后CreateLogger("loggername")时方便自定义logger name
            builder.RegisterInstance(loggerFactory).As<ILoggerFactory>();
        }
    }
}