using ChinaWaterLib;
using LiaoDongBay;
using Serilog;
using Swashbuckle.Examples;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Description;
using Web.Models;
using Web.Swagger;

namespace Web.Controllers
{
    public class LDController : ApiController
    {
        object __lockObj = new object();
        private static bool isRunnning = false;
        public readonly string modelPath;
        private static ILogger _logger = Serilog.Log.ForContext<LDController>();
        public LDController()
        {
            modelPath = ConfigurationManager.AppSettings["LiaoDongModel"];
        }

        /// <summary>
        /// 泄漏检测
        /// </summary>
        /// <remarks></remarks>
        /// <param name="arg"></param>
        /// <returns></returns>
        [ResponseType(typeof(LiaoDongResult))]
        [SwaggerRequestExample(typeof(NodeEmitterCoefficientArg), typeof(LD_LeakDetect_Example))]
        public IHttpActionResult LeakDetect(LiaoDongArg arg)
        {
            if (isRunnning)
            {
                return BadRequest("前一个请求正在运行，请稍后再试");
            }
            var innerArg = Consts.Mapper.Map<NodeEmitterCoefficientArg>(arg);
            //override
            innerArg.ModelPath = modelPath;
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunnning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                var result = new LiaoDongResult();
                result.IsBalanced = LiaoDongApi.CheckBalance(innerArg);
                result.NodeEmitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O = LiaoDongApi.SettingObservedDataAndRunWaterLeakCalibration(innerArg);
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
