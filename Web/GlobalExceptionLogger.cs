using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;
using Serilog;

namespace LiaoDongBay
{
    //support registering multiple exception loggers but only a single exception handler.
    //https://docs.microsoft.com/en-us/aspnet/web-api/overview/error-handling/web-api-global-error-handling
    class GlobalExceptionLogger : ExceptionLogger
    {
        private static ILogger _logger = global::Serilog.Log.ForContext<GlobalExceptionLogger>();

        public override void Log(ExceptionLoggerContext context)
        {
            //Trace.TraceError(context.ExceptionContext.Exception.ToString());
            _logger.Error("BentleyAPI ExceptionLogger" + context.ExceptionContext.Exception.ToString());
        }
    }
}