using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using LiaoDongBayTest;

namespace LiaoDongBay.Controllers
{
    public class LDController : ApiController
    {
        object __lockObj = new object();
        private static bool isRunnning = false;
        private readonly string modelPath;
        private const string fileName = "LiaoDongBay_20210716.wtg.sqlite";
        public LDController()
        {
            string path = ConfigurationManager.AppSettings["ModelsFolder"];
            modelPath = Path.Combine(path, fileName);
        }
        /// <summary>
        /// 泄漏检测
        /// </summary>
        /// <remarks></remarks>
        /// <param name="arg"></param>
        /// <returns></returns>
        [ResponseType(typeof(LiaoDongResult))]

        public IHttpActionResult LeakDetect(LiaoDongArg arg)
        {
            if (isRunnning)
            {
                return BadRequest("前一个请求正在运行，请稍后再试");
            }
            //override
            arg.ModelPath = @"D:\BentleyModels\LD\Model_20210730\LiaoDongBay_20210716.wtg.sqlite";
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunnning);
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
