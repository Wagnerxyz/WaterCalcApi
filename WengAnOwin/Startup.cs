using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using WengAn;

namespace WengAnOwin
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.UseErrorPage();
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Services.Add(typeof(IExceptionLogger), new GlobalExceptionLogger());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            SwaggerConfig.Register(config);
            appBuilder.UseWebApi(config);
        }
    }
}
