using AutoMapper;
using Haestad.Support.Library;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ChinaWaterLib.Models;
using ChinaWaterModel;
using Haestad.Support.User;
using Serilog;
using Topshelf;
using Web.App_Start;
using WengAn;

namespace WengAnOwin
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SerilogConfig.Register();
            RegisterAutoMapper();
            //SetHaestadAppRuntimePath();
            Consts.isWriteDatabase = Convert.ToBoolean(ConfigurationManager.AppSettings["PerformanceLogToDatabase"]);
            Consts.Logger = Log.Logger;

            //  UseOwinBoot();
            UseTopShelfBoot();
        }
        private static void UseTopShelfBoot()
        {
            TopshelfExitCode rc = HostFactory.Run(x => //1
            {
                x.Service<TopShelfService>(s => //2
                {
                    s.ConstructUsing(name => new TopShelfService()); //3
                    s.WhenStarted(tc => tc.StartOWIN()); //4
                    s.WhenStopped(tc => tc.StopServer()); //5
                });
                x.RunAsLocalSystem(); //6 https://topshelf.readthedocs.io/en/latest/configuration/config_api.html#service-identity
                x.StartAutomatically(); // Automatic (Delayed) -- only available on .NET 4.0 or later
                x.UseSerilog();

                Log.Logger.Information("Start StartAutomatically");
                x.SetDescription("瓮安Bentley Service"); //7
                //x.SetDisplayName("瓮安Bentley Service"); //8
                x.SetServiceName("瓮安Bentley Service"); //9
                x.OnException(ex =>
                {
                    Log.Logger.Error(ex.ToString());
                    // Do something with the exception
                });
            }); //10

            int exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode()); //11
            Environment.ExitCode = exitCode;
        }
        /// <summary>
        /// Start OWIN host 
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="testUrl"></param>
        internal static void StartOwinService(string port)
        {
            var prefix = "http://*:";
            var baseAddress = prefix + port + "/";
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                // Create HttpClient and make a request to api/values 
                HttpClient client = new HttpClient();
                var testUrl = "http://localhost:" + port + Consts.HealthCheckApiPath;
                var response = client.GetAsync(testUrl).Result;

                Console.WriteLine(response);
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.ReadLine();
            }
        }
        private static void UseOwinBoot()
        {
            string port = ConfigurationManager.AppSettings["Port"];
            StartOwinService(port);
        }
        //private static void UseWinServiceBoot()
        //{
        //    ServiceBase[] ServicesToRun;
        //    ServicesToRun = new ServiceBase[]
        //    {
        //            new SewerGemsService()
        //    };
        //    ServiceBase.Run(ServicesToRun);
        //}
        private static void SetHaestadAppRuntimePath()
        {
            string folder = AppDomain.CurrentDomain.RelativeSearchPath;
            ApplicationLibrary.ApplicationRuntimePath = folder;
        }

        private static void RegisterAutoMapper()
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
