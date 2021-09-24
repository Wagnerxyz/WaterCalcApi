using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using LiaoDongBay.Swagger;
using LiaoDongBayTest;
using Serilog;
using Swashbuckle.Examples;

namespace LiaoDongBay.Controllers
{
    public class LDController : ApiController
    {
        object __lockObj = new object();
        private static bool isRunnning = false;
        public const string modelPath= @"D:\BentleyModels\LiaoDong\LiaoDongBay_20210716.wtg.sqlite";
        private static ILogger _logger = Serilog.Log.ForContext<LDController>();
        public LDController()
        {
            //string path = ConfigurationManager.AppSettings["ModelsFolder"];
            //modelPath = Path.Combine(path, fileName);
        }

        /// <summary>
        /// 泄漏检测
        /// </summary>
        /// <remarks></remarks>
        /// <param name="arg"></param>
        /// <returns></returns>
        [ResponseType(typeof(LiaoDongResult))]
        [SwaggerRequestExample(typeof(LiaoDongArg), typeof(LD_LeakDetect_Example))]
        public IHttpActionResult LeakDetect(LiaoDongArg arg)
        {
            if (isRunnning)
            {
                return BadRequest("前一个请求正在运行，请稍后再试");
            }
            //override
            arg.ModelPath = modelPath;
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunnning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                var result = new LiaoDongResult();
                result.IsBalanced = LiaoDongApi.CheckBalance(arg);
                result.NodeEmitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O = LiaoDongApi.SettingObservedDataAndRunWaterLeakCalibration(arg);
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
