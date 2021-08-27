using System.Configuration;
using System.IO;
using System.Web.Http;
using System.Web.Mvc;
using LiaoDongBayTest;

namespace LiaoDongBay.Controllers
{
    /// <summary>
    /// 瓮安
    /// </summary>
    public class WAController : ApiController
    {
        object __lockObj = new object();
        private static bool isRunnning = false;
        private readonly string modelPath;
        private const string fileName = "WengAn20210813.wtg.sqlite";
        public WAController()
        {
            modelPath = @"C:\Data\WengAn\WengAn20210813\WengAn20210813.wtg.sqlite";
            //string path = ConfigurationManager.AppSettings["WengAnModelsFolder"];
            //modelPath = Path.Combine(path, fileName);

        }
        public IHttpActionResult RunEps(string modelPath)
        {
            if (isRunnning)
            {
                return BadRequest("前一个请求正在运行，请稍后再试");
            }
            //override
            modelPath = this.modelPath;
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunnning);
                var result = new WengAnEpsResult();
                result = WengAnApi.RunEPS(modelPath);
                return Ok(result);

            }
            finally
            {
                if (isRunnning)
                {
                    System.Threading.Monitor.Exit(__lockObj);
                    isRunnning = false;
                }
            }

        }
    }
}