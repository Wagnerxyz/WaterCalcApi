using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Dapper;
using Microsoft.Data.SqlClient;
using Serilog;

namespace WengAnOwin
{
    /// <summary>
    /// ActionFilter级 记录日志
    /// </summary>
    public class LogWebApiActionFilter : ActionFilterAttribute

    {
        private static ILogger _logger = global::Serilog.Log.ForContext<LogWebApiActionFilter>();
        public LogWebApiActionFilter()
        {
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // Get the request lifetime scope so you can resolve services.
            //var requestScope = actionContext.Request.GetDependencyScope();
            //// Resolve the service you want to use.
            //var service = requestScope.GetService(typeof(ICalculation)) as MyCalculation;
            //// Do the rest of the work in the filter.
            //service.DoWork();
            //  cal.Start();

            _logger.Information(string.Format("Start Executing. controller:{0} Action Method: {1} executing at {2}. URL: {3}", actionContext.ActionDescriptor.ControllerDescriptor.ControllerName, actionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString(), actionContext.Request.RequestUri), "Web API Logs");
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            GetMemoryWorkingSet64SizeMB(actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
            if (actionExecutedContext.Response.IsSuccessStatusCode)
            {
                _logger.Information(string.Format("执行成功. controller:{0} Action Method: {1} executed at {2}. URL: {3}", actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName, actionExecutedContext.ActionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString(), actionExecutedContext.Request.RequestUri), "Web API Logs");

            }
            else
            {
                _logger.Information(string.Format("执行出错. controller:{0} Action Method: {1} executed at {2}. URL: {3}", actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName, actionExecutedContext.ActionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString(), actionExecutedContext.Request.RequestUri), "Web API Logs");

            }
        }

        private void GetMemoryWorkingSet64SizeMB(string methodName)
        {
            var proc = Process.GetCurrentProcess();
            var mem = proc.WorkingSet64;
            double memory = mem / 1024.0 / 1024.0;
            string memInfo = $"内存占用 {mem / 1024.0 / 1024.0:n3} MB of working set";
            _logger.Information(memInfo);

            string sql = @"insert into MemUsage(DateTime,MethodName,MemSize)
                        values(@Date,@MethodName,@MemSize)";

            SqlParameter parameterDate = new SqlParameter("@Date", DateTime.Now);
            SqlParameter parameterMethodName = new SqlParameter("@MethodName", methodName);
            SqlParameter parameterMemSize = new SqlParameter("@MemSize", memory);
            Int32 rows = SqlHelper.ExecuteNonQuery(ConfigurationManager.ConnectionStrings["SqlServerConnection"].ToString(),
                sql, CommandType.Text, parameterDate, parameterMethodName, parameterMemSize);

            // Console.WriteLine("{0} row{1} {2} updated.", rows, rows > 1 ? "s" : null, rows > 1 ? "are" : "is");
            if (rows != 1)
            {
                throw new Exception("插入内存统计失败");
            }
        }

    }

    //private void Log(string methodName, RouteData routeData)
    //{
    //    var controllerName = routeData.Values["controller"];
    //    var actionName = routeData.Values["action"];
    //    var message = String.Format("{0} controller:{1} action:{2}", methodName, controllerName, actionName);
    //    Debug.WriteLine(message, "Action Filter Log");
    //}

}

