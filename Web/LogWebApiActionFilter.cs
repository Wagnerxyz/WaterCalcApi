using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Serilog;
using Web;
using WengAn;
using SqlParameter = Microsoft.Data.SqlClient.SqlParameter;

/// <summary>
/// ActionFilter级 记录日志
/// </summary>
public class LogWebApiActionFilter : ActionFilterAttribute

{
    private static ILogger _logger = global::Serilog.Log.ForContext<LogWebApiActionFilter>();

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
        if (actionExecutedContext.Response == null)
        {
            _logger.Information(string.Format("执行出错 Response为空. controller:{0} Action Method: {1} executed at {2}. URL: {3}", actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName, actionExecutedContext.ActionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString(), actionExecutedContext.Request.RequestUri), "Web API Logs");

        }
        else if (actionExecutedContext.Response.IsSuccessStatusCode)
        {
            _logger.Information(string.Format("执行成功. controller:{0} Action Method: {1} executed at {2}. URL: {3}", actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName, actionExecutedContext.ActionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString(), actionExecutedContext.Request.RequestUri), "Web API Logs");
            GetMemoryWorkingSet64SizeMB(actionExecutedContext.Request.RequestUri.ToString());

        }
        else
        {
            _logger.Information(string.Format("执行出错. controller:{0} Action Method: {1} executed at {2}. URL: {3}", actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName, actionExecutedContext.ActionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString(), actionExecutedContext.Request.RequestUri), "Web API Logs");
        }
    }

    private void GetMemoryWorkingSet64SizeMB(string uri)
    {
        var proc = Process.GetCurrentProcess();
        var mem = proc.WorkingSet64;
        double memory = mem / 1024.0 / 1024.0;
        string memInfo = $"内存占用 {mem / 1024.0 / 1024.0:n3} MB of working set";
        _logger.Information(memInfo);
        if (Consts.isWriteDatabase)
        {
            string sql = @"insert into MemUsage(DateTime,MachineName,RequestUri,MemWorkingSet)
                        values(@DateTime,@MachineName,@RequestUri,@MemWorkingSet)";
            try
            {
                SqlParameter parameterDate = new SqlParameter("@DateTime", DateTime.Now);
                SqlParameter parameterMachineName = new SqlParameter("@MachineName", Environment.MachineName);
                SqlParameter parameterUri = new SqlParameter("@RequestUri", uri);
                SqlParameter parameterMemWorkingSet = new SqlParameter("@MemWorkingSet", memory);
                int rows = SqlHelper.ExecuteNonQuery(ConfigurationManager.ConnectionStrings["SqlServerConnection"].ToString(),
                    sql, CommandType.Text, parameterDate, parameterMachineName, parameterUri, parameterMemWorkingSet);
                if (rows != 1)
                {
                    throw new Exception("向数据库插入内存统计失败，请检查数据库连接字符串");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                throw;
            }
            // Console.WriteLine("{0} row{1} {2} updated.", rows, rows > 1 ? "s" : null, rows > 1 ? "are" : "is");

        }
    }

}
