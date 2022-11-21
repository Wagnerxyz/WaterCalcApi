using AutoMapper;
using ChinaWaterLib;
using ChinaWaterLib.Models;
using Haestad.Support.User;
using LiaoDongBay;
using Serilog;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Web.App_Start;

namespace Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            SerilogConfig.RegisterComponents();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RegisterAutoMapper();
        }
        protected void Application_End(object sender, EventArgs e)
        {
            Log.Debug("In Application_End");
            ApplicationShutdownReason shutdownReason = System.Web.Hosting.HostingEnvironment.ShutdownReason;
            Log.Information("App is shutting down (reason = {@shutdownReason})", shutdownReason);
            // Finally, once just before the application exits...
            Log.CloseAndFlush();
            // WARNING : Some code runs AFTER Application_End ... see AppPostShutDown
        }
        private void RegisterAutoMapper()
        {
            MapperConfiguration mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IUserNotification, UserNotification>();
                //cfg.AddProfile();
            });
            var mapper = mapConfig.CreateMapper();
            Consts.Mapper = mapper;
            WengAnHandler.mapper = mapper;
        }
    }
}
