using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using Haestad.Support.User;
using LiaoDongBay.App_Start;
using LiaoDongBay.Models;
using ChinaWaterLib;
using Models;
using NSwag.AspNet.Owin;
using Serilog;

namespace LiaoDongBay
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //RouteTable.Routes.MapOwinPath("swagger", app =>
            //{
            //    app.UseSwaggerUi3(typeof(WebApiApplication).Assembly, settings =>
            //    {
            //        settings.MiddlewareBasePath = "/swagger";
            //        //settings.GeneratorSettings.DefaultUrlTemplate = "api/{controller}/{id}";  //this is the default one
            //        settings.GeneratorSettings.DefaultUrlTemplate = "api/{controller}/{action}/{id}";
            //        settings.DocumentTitle = "Bentley API";
            //    });
            //});
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
                cfg.CreateMap<LiaoDongArg, NodeEmitterCoefficientArg>();
                //cfg.AddProfile();
            });
            var mapper = mapConfig.CreateMapper();
            Consts.Mapper = mapper;
            WengAnApi.mapper = mapper;
        }
    }
}
