using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WengAn;

namespace WengAnOwin
{
    internal class TopShelfService
    {
        private static Serilog.ILogger _logger = Serilog.Log.ForContext<TopShelfService>();

        private IDisposable _server = null;
        public TopShelfService()
        {

        }

        public void StartOWIN()
        {
            string port = ConfigurationManager.AppSettings["Port"];
            // Start OWIN host 
            Program.StartOwinService(port);
            _logger.Information("服务启动aa");
        }

      

        public void StopServer()
        {
            if (_server != null)
            {
                _server.Dispose();
            }
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            _logger.Information("服务停止");

        }
    }
}
