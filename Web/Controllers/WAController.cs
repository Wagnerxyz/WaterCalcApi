using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using LiaoDongBay.Swagger;
using LiaoDongBayTest;
using LiaoDongBayTest.WengAn.Args;
using Models;
using Serilog;
using Swashbuckle.Examples;
using WengAn.Args;
using WaterQualityResult = Models.WaterQualityResult;

namespace LiaoDongBay.Controllers
{
    /// <summary>
    /// 并发执行控制，同时只能执行一个
    /// </summary>
    public class WAController : ApiController
    {
        private object __lockObj = new object();
        private static bool isRunnning = false;
        private readonly string modelPath;
        const string runningMsg = "有请求正在运行，请稍后再试";
        const string argMsg = "请求参数错误";
        private static ILogger _logger = Serilog.Log.ForContext<WAController>();

        public WAController()
        {
            modelPath = @"D:\BentleyModels\WengAn\WengAn0916.wtg.sqlite";
            //string path = ConfigurationManager.AppSettings["WengAnModelsFolder"];
            //modelPath = Path.Combine(path, fileName);

        }
        /// <summary>
        /// 运行水力模型返回结果(所有节点压力，所有管道流量，流速，水头损失梯度)
        /// </summary>
        /// <param name="arg">请求</param>
        /// <returns></returns>
        [ResponseType(typeof(WengAnEpsResult))]
        [SwaggerRequestExample(typeof(RunEPSArg), typeof(WA_RunEPS1_Example))]
        public IHttpActionResult RunEps(RunEPSArg arg)
        {
            if (isRunnning)
            {
                return BadRequest(argMsg);
            }
            var result = new WengAnEpsResult();

            //override
            //arg.ModelPath = this.modelPath;
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunnning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                result = WengAnApi.RunEPS(arg);
                LogCalcError(result);
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
        /// <summary>
        /// 爆管影响分析
        /// </summary>
        /// <param name="arg">输入参数</param>
        /// <returns></returns>
        [ResponseType(typeof(BreakPipeResult))]
        [SwaggerRequestExample(typeof(BreakPipeArg), typeof(WA_BreakPipe_Example))]
        public IHttpActionResult BreakPipe(BreakPipeArg arg)
        {
            if (arg == null || !ModelState.IsValid)
            {
                return BadRequest(argMsg);
            }
            if (isRunnning)
            {
                return BadRequest(runningMsg);
            }
            //override
            arg.ModelPath = this.modelPath;
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunnning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                var result = new BreakPipeResult();
                result = WengAnApi.BreakPipe(arg);
                LogCalcError(result);
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
        /// <summary>
        /// 水源追踪(多水源供水分析)
        /// </summary>
        /// <param name="arg">无需在线数据接入，仅需传modelpath一个属性(暂时可任意填写)</param>
        /// <remarks>无需在线数据接入</remarks>
        /// <returns></returns>
        [SwaggerRequestExample(typeof(WengAnBaseArg), typeof(WA_WaterTrace_Example))]
        [ResponseType(typeof(WaterTraceResult))]
        public IHttpActionResult WaterTrace(WengAnBaseArg arg)
        {
            if (isRunnning)
            {
                return BadRequest(runningMsg);
            }
            //override
            arg.ModelPath = this.modelPath;
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunnning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                WaterTraceResult result = WengAnApi.GetWaterTraceResultsForMultipleElementIds(arg.ModelPath);
                LogCalcError(result);
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
        /// <summary>
        /// 消防事件
        /// </summary>
        /// <param name="arg">请求参数</param>
        /// <returns></returns>
        [ResponseType(typeof(WengAnEpsResult))]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [SwaggerRequestExample(typeof(FireDemandArg), typeof(WA_Fire_Example))]
        public IHttpActionResult FireDemand(FireDemandArg arg)
        {
            if (arg == null || !ModelState.IsValid)
            {
                return BadRequest(argMsg);
            }
            if (isRunnning)
            {
                return BadRequest(runningMsg);
            }

            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunnning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                WengAnEpsResult result = WengAnApi.FireDemandAtOneNode(arg);
                LogCalcError(result);
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

        /// <summary>
        /// 水质余氯预测
        /// </summary>
        /// <param name="arg">输入参数</param>
        /// <returns></returns>
        [ResponseType(typeof(WaterQualityResult))]
        [SwaggerRequestExample(typeof(WaterConcentrationArg), typeof(WA_WaterConcentration_Example))]
        public IHttpActionResult Concentration(WaterConcentrationArg arg)
        {
            if (arg == null || !ModelState.IsValid)
            {
                return BadRequest(argMsg);
            }
            if (isRunnning)
            {
                return BadRequest(runningMsg);
            }

            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunnning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                WaterQualityResult result = WengAnApi.Concentration(arg);
                LogCalcError(result);
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
        /// <summary>
        /// 水龄预测
        /// </summary>
        /// <param name="arg">无需在线数据接入，仅需传modelpath一个属性(暂时可任意填写)</param>
        /// <remarks>无需在线数据接入</remarks>
        /// <returns></returns>
        [SwaggerRequestExample(typeof(WengAnBaseArg), typeof(WA_WaterAge_Example))]
        [ResponseType(typeof(WaterQualityResult))]
        public IHttpActionResult WaterAge(WengAnBaseArg arg)
        {
            if (isRunnning)
            {
                return BadRequest(runningMsg);
            }
            //override
            arg.ModelPath = this.modelPath;
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunnning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                WaterQualityResult result = WengAnApi.WaterAge(arg);
                LogCalcError(result);
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
        private void LogCalcError(WaterEngineResultBase result)
        {
            if (result != null && result.IsCalculationFailure && result.ErrorNotifs.Any())
            {
                _logger.Error("{0}: 计算错误总数:{1}  @{2}", new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name, result.ErrorNotifs.Count,
                    result.ErrorNotifs
                        .Select(x => new
                        {
                            Id = x.ElementId,
                            Message = x.MessageKey,
                            SourceKey = x.SourceKey,
                            Label = x.Label
                        }));
            }
        }
    }
}