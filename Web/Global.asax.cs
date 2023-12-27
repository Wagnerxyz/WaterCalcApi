using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using ChinaWaterLib.Models;
using Haestad.Support.User;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Web.App_Start;
using WengAn;

namespace Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            SerilogConfig.Register();
            AreaRegistration.RegisterAllAreas();

            #region Autofac
            var builder = new ContainerBuilder();
            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the filter provider if you have custom filters that need DI.
            //builder.RegisterWebApiFilterProvider(config);

            // OPTIONAL: Register the Autofac model binder provider.
            //builder.RegisterWebApiModelBinderProvider();
            //具体注册类型放这里面
            builder.RegisterModule<MyAutoFacModule>();

            //builder.RegisterInstance(mapper).As<IMapper>(); //not work
            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            Consts.Container = container;
            #endregion
            

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RegisterAutoMapper();
            //var logger = ((ILoggerFactory)config.DependencyResolver.GetService(typeof(ILoggerFactory))).CreateLogger("Main");
            Consts.isWriteDatabase = Convert.ToBoolean(ConfigurationManager.AppSettings["PerformanceLogToDatabase"]);
            Consts.Logger = Log.Logger;
            Consts.Logger.Information("Web Site已启动");

            //    using (var scope = container.BeginLifetimeScope())
            //{
            //    var logger = scope.Resolve<ILoggerFactory>().CreateLogger("Main");
            //    logger.LogInformation("Web Site已启动");
            //}
        }

        protected void Application_End(object sender, EventArgs e)
        {
            var shutdownReason = System.Web.Hosting.HostingEnvironment.ShutdownReason;
            //using (var scope =  Consts.Container.BeginLifetimeScope())
            //{
            //    var logger = scope.Resolve<ILoggerFactory>().CreateLogger("Main");
            //    logger.LogInformation("In Application_End");
            //}
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
        }
    }
}
