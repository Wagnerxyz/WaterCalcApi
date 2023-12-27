using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Serilog;
using WengAn;

namespace Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TestController : ApiController
    {
        private static ILogger _logger = Serilog.Log.ForContext<TestController>();

        [HttpGet]
        public int SerilogStructure()
        {
            var count = 456;
            _logger.Information("Retrieved {Count} records", count);
            _logger.Information("Retrieved {@Count} records", count);
            var sensorInput = new { Latitude = 25, Longitude = 134 };
            _logger.Information(" {SensorInput}", sensorInput);
            _logger.Information(" Preserving Object Structure {@SensorInput}", sensorInput);//Preserving Object Structure {"Latitude":25,"Longitude":134}
            var fruit = new[] { "Apple", "Pear", "Orange" };
            _logger.Information("In my bowl I have {Fruit}", fruit);//In my bowl I have ["Apple","Pear","Orange"]
            return 5;
        }
        [HttpGet]
        public int Log()
        {
            _logger.Information($"{Consts.ProjectName}, {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");

            //ILogger logger = Consts.loggerFactory.CreateLogger("Testttt");
            _logger.Information("Example log message info");
            _logger.Debug("Example log message debug");
            return 5;
        }
        public void GetFiles(string path)
        {
            //string aaa = path.Replace("\\\\", "\\");
            var ww = File.ReadAllText(path);
        }
        public int ThrowException()
        {
            int a = 5;
            int b = 0;
            var qweq = a / b;
            return qweq;
        }
        public int GetException1()
        {
            int a = 5;
            int b = 0;
            int qweq = 0;
            try
            {
                qweq = a / b;
            }
            catch (Exception ex)
            {
                _logger.Information("Example log message info");
                _logger.Information(ex, "Example log message info");
            }
            finally
            {

            }
            return qweq;
        }
    }
}
