using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using Haestad.Support.Library;
using LiaoDongBay.Swagger;
using ChinaWaterLib;
using ChinaWaterLib.WengAn;
using ChinaWaterLib.WengAn.Args;
using Models;
using Serilog;
using Swashbuckle.Examples;
using WengAn.Args;

namespace LiaoDongBay.Controllers
{
    /// <summary>
    /// 并发执行控制，同时只能执行一个
    /// </summary>
    public class WAController : ApiController
    {
        private object __lockObj = new object();
        private static bool isRunning = false;
        private readonly string originalModelPath;
        private readonly string runDirectory = @"D:\BentleyModels\WengAn\RunDirectory";
        private readonly string modelName = @"WengAn0916.wtg.sqlite";
        const string runningMsg = "有请求正在运行，不支持并发计算，请稍后再试";
        const string argMsg = "请求参数错误";
        private static ILogger _logger = Serilog.Log.ForContext<WAController>();

        public WAController()
        {
            originalModelPath = @"D:\BentleyModels\WengAn\WengAn1109.wtg.sqlite"; ;
            //string path = ConfigurationManager.AppSettings["WengAnModelsFolder"];
            //originalModelPath = Path.Combine(path, fileName);

        }
        /// <summary>
        /// 运行水力模型返回结果(所有节点压力，所有管道流量，流速，水头损失梯度)
        /// </summary>
        /// <param name="arg">请求</param>
        /// <returns></returns>
        [ResponseType(typeof(WengAnEpsBaseResult))]
        [SwaggerRequestExample(typeof(RunEPSArg), typeof(WA_RunEPS1_Example))]
        public IHttpActionResult RunEps(RunEPSArg arg)
        {
            if (isRunning)
            {
                LogRacingError();
                return BadRequest(runningMsg);
            }

            //override
            //arg.ModelPath = this.originalModelPath;
            arg.ModelPath = CopyNewModel();
            //try
            //{
            //   System.Threading.Monitor.Enter(__lockObj, ref isRunning);
            _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
            var result = WengAnApi.RunEPS(arg);
            LogCalcError(result);
            return Ok(result);
            //}
            //finally
            //{
            //    if (isRunning)
            //    {
            //        System.Threading.Monitor.Exit(__lockObj);
            //        isRunning = false;
            //    }
            //}
        }
        public IHttpActionResult RunEPSP(RunEPSArg arg)
        {
            if (isRunning)
            {
                LogRacingError();
                return BadRequest(runningMsg);
            }

            //override
            //arg.ModelPath = this.originalModelPath;
           // arg.ModelPath = CopyNewModel();
            //try
            //{
            //   System.Threading.Monitor.Enter(__lockObj, ref isRunning);
            _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
            var result = WengAnApi.RunEPSP(arg);
            LogCalcError(result);
            return Ok(result);
            //}
            //finally
            //{
            //    if (isRunning)
            //    {
            //        System.Threading.Monitor.Exit(__lockObj);
            //        isRunning = false;
            //    }
            //}
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
            if (isRunning)
            {
                LogRacingError();
                return BadRequest(runningMsg);
            }
            //override
            //arg.ModelPath = this.originalModelPath;
            arg.ModelPath = CopyNewModel();
            //  try
            //  {
            //      System.Threading.Monitor.Enter(__lockObj, ref isRunning);
            _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
            var result = new BreakPipeResult();
            result = WengAnApi.BreakPipe(arg);
            LogCalcError(result);
            return Ok(result);
            //   }
            //finally
            //{
            //    if (isRunning)
            //    {
            //        System.Threading.Monitor.Exit(__lockObj);
            //        isRunning = false;
            //    }
            //}
        }
        /// <summary>
        /// 水源追踪(多水源供水分析)
        /// </summary>
        /// <param name="arg">无需在线数据接入，仅需传modelpath一个属性(暂时可任意填写)</param>
        /// <remarks>无需在线数据接入</remarks>
        /// <returns></returns>
        [SwaggerRequestExample(typeof(WengAnBaseArg), typeof(WA_WaterTrace_Example))]
        [ResponseType(typeof(WaterTraceBaseResult))]
        public IHttpActionResult WaterTrace(WengAnBaseArg arg)
        {
            if (isRunning)
            {
                LogRacingError();
                return BadRequest(runningMsg);
            }
            //override
            //arg.ModelPath = this.originalModelPath;
            arg.ModelPath = CopyNewModel();
            //try
            //{
            //    System.Threading.Monitor.Enter(__lockObj, ref isRunning);
            _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
            WaterTraceBaseResult baseResult = WengAnApi.GetWaterTraceResultsForMultipleElementIds(arg.ModelPath);
            LogCalcError(baseResult);
            return Ok(baseResult);
            //}
            //finally
            //{
            //    if (isRunning)
            //    {
            //        System.Threading.Monitor.Exit(__lockObj);
            //        isRunning = false;
            //    }
            //}
        }
        /// <summary>
        /// 消防事件
        /// </summary>
        /// <param name="arg">请求参数</param>
        /// <returns></returns>
        [ResponseType(typeof(WengAnEpsBaseResult))]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [SwaggerRequestExample(typeof(FireDemandArg), typeof(WA_Fire_Example))]
        public IHttpActionResult FireDemand(FireDemandArg arg)
        {
            if (arg == null || !ModelState.IsValid)
            {
                return BadRequest(argMsg);
            }
            if (isRunning)
            {
                LogRacingError();
                return BadRequest(runningMsg);
            }
            //override
            //arg.ModelPath = this.originalModelPath;
            arg.ModelPath = CopyNewModel();
            //try
            //{
            //    System.Threading.Monitor.Enter(__lockObj, ref isRunning);
            _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
            WengAnEpsBaseResult baseResult = WengAnApi.FireDemandAtOneNode(arg);
            LogCalcError(baseResult);
            return Ok(baseResult);
            //}
            //finally
            //{
            //    if (isRunning)
            //    {
            //        System.Threading.Monitor.Exit(__lockObj);
            //        isRunning = false;
            //    }
            //}
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
            if (isRunning)
            {
                LogRacingError();
                return BadRequest(runningMsg);
            }
            //override
            //arg.ModelPath = this.originalModelPath;
            arg.ModelPath = CopyNewModel();
            //try
            //{
            //    System.Threading.Monitor.Enter(__lockObj, ref isRunning);
            _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
            WaterQualityResult result = WengAnApi.Concentration(arg);
            LogCalcError(result);
            return Ok(result);
            //}
            //finally
            //{
            //    if (isRunning)
            //    {
            //        System.Threading.Monitor.Exit(__lockObj);
            //        isRunning = false;
            //    }
            //}
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
            if (isRunning)
            {
                LogRacingError();
                return BadRequest(runningMsg);
            }
            //override
            //arg.ModelPath = this.originalModelPath;
            arg.ModelPath = CopyNewModel();
            //try
            //{
            //    System.Threading.Monitor.Enter(__lockObj, ref isRunning);
            _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
            WaterQualityResult result = WengAnApi.WaterAge(arg);
            LogCalcError(result);
            return Ok(result);
            //}
            //finally
            //{
            //    if (isRunning)
            //    {
            //        System.Threading.Monitor.Exit(__lockObj);
            //        isRunning = false;
            //    }
            //}
        }
        /// <summary>
        /// 更新需水量
        /// </summary>
        /// <param name="arg"></param>
        /// <remarks>每半个小时， 平台 CALL 我们这个 API, 计算未来 24 小时的预报</remarks>
        /// <returns></returns>
        [SwaggerRequestExample(typeof(ForecastDemandArg), typeof(WA_UpDateDemand_Example))]
        [ResponseType(typeof(IHttpActionResult))]
        public IHttpActionResult UpdateDemand(ForecastDemandArg arg)
        {
            if (isRunning)
            {
                LogRacingError();
                return BadRequest(runningMsg);
            }
            //override
            arg.ModelPath = this.originalModelPath;
            //  arg.ModelPath = CopyNewModel();
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                WengAnDemandForecast.Run(arg);
                //string archiveFileName = $"NewModel_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.zip";
                string archiveFileName = HostingEnvironment.MapPath($"~/NewModel.zip"); ;
                if (File.Exists(archiveFileName))
                {
                    File.Delete(archiveFileName);
                }
                string newModel = Path.Combine(Path.GetDirectoryName(arg.ModelPath), "working", "workingmodel.sqlite");
                using (ZipArchive zip = ZipFile.Open(archiveFileName, ZipArchiveMode.Create))
                {
                    zip.CreateEntryFromFile(newModel, Path.GetFileName(arg.ModelPath));
                }

                return Ok("更新需水量成功");
                //LogCalcError(result);
                //return Ok(result);
            }
            finally
            {
                if (isRunning)
                {
                    System.Threading.Monitor.Exit(__lockObj);
                    isRunning = false;
                }
            }
        }
        /// <summary>
        /// 下载最新模型文件（定时更新完需水量后的模型）供后续使用
        /// </summary>
        /// <remarks></remarks>
        /// <returns></returns>
        //[SwaggerRequestExample(typeof(WengAnBaseArg), typeof(WA_WaterAge_Example))]
        //[ResponseType(typeof(WaterQualityResult))]
        public HttpResponseMessage GetLatestModel()
        {
            if (isRunning)
            {
                LogRacingError();
                var response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent(runningMsg);
                return response;
            }
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");

                string archiveFileName = HostingEnvironment.MapPath($"~/NewModel.zip");
                if (!File.Exists(archiveFileName))
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "未找到最新模型文件");
                }
                //  ZipFile.CreateFromDirectory("aa","ff",compress);
                var time = File.GetLastWriteTime(archiveFileName).ToString("yyyyMMdd_HHmmss");
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(archiveFileName, FileMode.Open, FileAccess.Read);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/zip");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"NewModel_{time}.zip"
                };
                return response;
            }
            finally
            {
                if (isRunning)
                {
                    System.Threading.Monitor.Exit(__lockObj);
                    isRunning = false;
                }
            }
        }
        private void LogCalcError(WaterEngineBaseResult baseResult)
        {
            if (baseResult != null && baseResult.IsCalculationFailure && baseResult.ErrorNotifs.Any())
            {
                _logger.Error("{0}: 计算错误总数:{1}  @{2}", new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name, baseResult.ErrorNotifs.Count,
                    baseResult.ErrorNotifs
                        .Select(x => new
                        {
                            Id = x.ElementId,
                            Message = x.MessageKey,
                            SourceKey = x.SourceKey,
                            Label = x.Label
                        }));
            }
        }
        private void LogRacingError()
        {
            _logger.Error("发生竟态: {0}", new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name);
        }

        private string CopyNewModel()
        {
            if (isRunning)
            {
                LogRacingError();
                var response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("计算接口之间模型拷贝冲突");
                throw new HttpResponseException(response);
            }
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunning);
                string newFolderName = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                string newDir = Path.Combine(runDirectory, newFolderName);
                if (!Directory.Exists(newDir))
                    Directory.CreateDirectory(newDir);

                string newModelPath = Path.Combine(newDir, modelName);
                File.Copy(originalModelPath, newModelPath);
                FileLibrary.SetReadWriteSafely(newModelPath);
                return newModelPath;
            }
            finally
            {
                if (isRunning)
                {
                    System.Threading.Monitor.Exit(__lockObj);
                    isRunning = false;
                }
            }
        }
    }
}