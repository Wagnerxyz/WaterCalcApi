using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LiaoDongBayTest;

namespace LiaoDongBay.Controllers
{
    public class WaterGemsController : ApiController
    {
        object __lockObj = new object();
        private readonly string modelPath;
        private const string fileName = "LiaoDongBay_20210716.wtg.sqlite";
        public WaterGemsController()
        {
            string path = ConfigurationManager.AppSettings["ModelsFolder"];
            modelPath = Path.Combine(path, fileName);
        }
        public LiaoDongResult LeakDetect(LiaoDongArg arg)
        {
            //override
            arg.ModelPath = modelPath;
            lock (__lockObj)
            {
                var result = new LiaoDongResult();
                var asda = Environment.MachineName;
                //try
                //{
                result.IsBalanced = WaterGemsApi.CheckBalance(arg);
                result.NodeEmitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O = WaterGemsApi.SettingObservedDataAndRunWaterLeakCalibration(arg);
                //}
                //catch (Exception e)
                //{
                //    BadRequest(e.ToString());
                //}

                return result;
            }

        }

    }
}