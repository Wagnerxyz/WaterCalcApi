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
using LiaoDongBayTest;
using Models;

namespace LiaoDongBay
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

        private void RegisterAutoMapper()
        {
            MapperConfiguration mapConfig = new MapperConfiguration(cfg =>
            {
             
                cfg.CreateMap<IUserNotification, UserNotification>();
                //cfg.AddProfile();
            });
            var mapper = mapConfig.CreateMapper();
            Consts.Mapper = mapper;
            WengAnApi.mapper = mapper;
        }
    }
}
