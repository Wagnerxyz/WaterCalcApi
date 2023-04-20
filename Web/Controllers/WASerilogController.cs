using Bentley.SelectServer.ManagedClient;
using ChinaWaterLib.Models;
using ChinaWaterLib.WengAn.Args;
using Haestad.Calculations.Shanghai.WaterGEMS;
using Haestad.Domain;
using Haestad.LicensingFacade;
using Haestad.ManagedLicensing;
using Haestad.Support.Library;
using Haestad.Support.Support;
using Serilog;
using Swashbuckle.Examples;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using Swashbuckle.Swagger.Annotations;
using Web.Models;
using Web.Swagger;
using WengAn.Args;
using ChinaWebUtils;

namespace Web.Controllers
{
    /// <summary>
    /// 不允许并发，只能同时执行一个
    /// </summary>
    [ModelExistCheckAttribute(ArgumentName ="arg")]
    public class WAController : ApiController
    {
        private static readonly object __lockObj = new object();
        private static bool isRunning = false;
        private readonly string demoModelPath;
        private readonly int demandAdjustmentScenarioId;
        private readonly string runDirectory = @"C:\BentleyModels\WengAn\RunDirectory";
        private readonly string modelName = @"WengAn1109.wtg.sqlite";
        private const string conflictMsg = "水力引擎不支持多个计算，有计算正在运行，请稍后再试";
        private const string argMsg = "请求参数错误";
        private static ILogger _logger = Serilog.Log.ForContext<WAController>();
        //private static bool isLicenseOk = false;
        private bool isServerModeLicense;
        private bool workOnCopiedModel;

        public WAController()
        {
            demandAdjustmentScenarioId = Convert.ToInt32(ConfigurationManager.AppSettings["DemandAdjustmentScenarioId"]);
            isServerModeLicense = Convert.ToBoolean(ConfigurationManager.AppSettings["ServerModeLicense"]);
            workOnCopiedModel = Convert.ToBoolean(ConfigurationManager.AppSettings["WorkOnCopiedModel"]);
            //demoModelPath = Path.Combine(path, fileName);
        }

        /// <summary>
        /// 运行水力模型
        /// </summary>
        /// <remarks>返回结果(节点压力，管道流量，流速，水头损失梯度)</remarks>
        /// <param name="arg">输入参数</param>
        /// <returns></returns>
        ///<response code="200">正常结果</response>
        ///<response code="400">错误</response>
        ///<response code="409">有请求正在运行，不支持同时计算，请稍后再试</response>
        [ResponseType(typeof(WengAnEpsBaseResult))]
        [SwaggerRequestExample(typeof(RunEPSArg), typeof(WA_RunEPS1_Example))]
        public IHttpActionResult RunEps(RunEPSArg arg)
        {
            if (arg == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!System.Threading.Monitor.TryEnter(__lockObj))
            {
                LogContentionError();
                return Content(HttpStatusCode.Conflict, conflictMsg);
            }
            //CheckLicense();
            try
            {
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                //WengAnHandler.WengAnDemandForecast(arg);
                var result = WengAnHandler.RunEPS(arg, Convert.ToInt32(ConfigurationManager.AppSettings["RunEPSScenarioId"]), Convert.ToDouble(ConfigurationManager.AppSettings["RunEPSPressureEngineSimulationDuration"]), true, isServerModeLicense, workOnCopiedModel, demandAdjustmentScenarioId);
                CheckAndLogWaterEngineError(result);
                return Ok(result);
            }
            finally
            {
                //if (isRunning)
                //{
                Monitor.Exit(__lockObj);
                //}
            }
        }


        /// <summary>
        /// 爆管影响分析
        /// </summary>
        /// <param name="arg">输入参数</param>
        /// <returns></returns>
        ///<response code="200">正常结果</response>
        ///<response code="400">错误</response>
        ///<response code="409">有请求正在运行，不支持同时计算，请稍后再试</response>
        [ResponseType(typeof(BreakPipeResult))]
        [SwaggerRequestExample(typeof(BreakPipeArg), typeof(WA_BreakPipe_Example))]
        public IHttpActionResult BreakPipe(BreakPipeArg arg)
        {
            if (arg == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!System.Threading.Monitor.TryEnter(__lockObj))
            {
                LogContentionError();
                return Content(HttpStatusCode.Conflict, conflictMsg);
            }
            //CheckLicense();
            try
            {
                // arg.ModelPath = CopyNewModel();
                //  try
                //  {
                //      System.Threading.Monitor.Enter(__lockObj, ref isRunning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new StackTrace().GetFrame(0).GetMethod().Name}");
                var result = WengAnHandler.BreakPipe(arg, Convert.ToInt32(ConfigurationManager.AppSettings["BreakPipeScenarioId"]), Convert.ToDouble(ConfigurationManager.AppSettings["BreakPipePressureEngineSimulationDuration"]), true, isServerModeLicense, workOnCopiedModel, demandAdjustmentScenarioId);
                CheckAndLogWaterEngineError(result);
                return Ok(result);
            }
            finally
            {
                Monitor.Exit(__lockObj);
            }
        }

        /// <summary>
        /// 水源追踪(多水源供水分析)
        /// </summary>
        /// <returns></returns>
        ///<response code="200">正常结果</response>
        ///<response code="400">错误</response>
        ///<response code="409">有请求正在运行，不支持同时计算，请稍后再试</response>
        [SwaggerRequestExample(typeof(WaterTraceArg), typeof(WA_WaterTrace_Example))]
        [ResponseType(typeof(WaterHeadTraceResult))]
        public IHttpActionResult WaterTrace(WaterTraceArg arg)
        {
            if (arg == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!System.Threading.Monitor.TryEnter(__lockObj))
            {
                LogContentionError();
                return Content(HttpStatusCode.Conflict, conflictMsg);
            }
            //  CheckLicense();
            try
            {
                //  arg.ModelPath = CopyNewModel();
                //try
                //{
                //    System.Threading.Monitor.Enter(__lockObj, ref isRunning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new StackTrace().GetFrame(0).GetMethod().Name}");
                WaterHeadTraceResult result = WengAnHandler.GetWaterTraceResultsForMultipleElementIds(arg, Convert.ToInt32(ConfigurationManager.AppSettings["WaterTraceScenarioId"]), Convert.ToDouble(ConfigurationManager.AppSettings["WaterTracePressureEngineSimulationDuration"]), true, isServerModeLicense, workOnCopiedModel, demandAdjustmentScenarioId);
                CheckAndLogWaterEngineError(result);
                return Ok(result);
            }
            finally
            {
                System.Threading.Monitor.Exit(__lockObj);
            }
        }

        /// <summary>
        /// 消防事件
        /// </summary>
        /// <param name="arg">请求参数</param>
        /// <returns></returns>
        ///<response code="200">正常结果</response>
        ///<response code="400">错误</response>
        ///<response code="409">有请求正在运行，不支持同时计算，请稍后再试</response>
        [ResponseType(typeof(WengAnEpsBaseResult))]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [SwaggerRequestExample(typeof(FireDemandArg), typeof(WA_Fire_Example))]
        public IHttpActionResult FireDemand(FireDemandArg arg)
        {
            if (arg == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!System.Threading.Monitor.TryEnter(__lockObj))
            {
                LogContentionError();
                return Content(HttpStatusCode.Conflict, conflictMsg);
            }
            try
            {
                // arg.ModelPath = CopyNewModel();
                //try
                //{
                //    System.Threading.Monitor.Enter(__lockObj, ref isRunning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                WengAnEpsBaseResult result = WengAnHandler.FireDemandAtOneNode(arg, Convert.ToInt32(ConfigurationManager.AppSettings["FireDemandAtOneNodeScenarioId"]), Convert.ToDouble(ConfigurationManager.AppSettings["FireDemandAtOneNodeEngineSimulationDuration"]), true, isServerModeLicense, workOnCopiedModel, demandAdjustmentScenarioId);
                CheckAndLogWaterEngineError(result);
                return Ok(result);
            }
            finally
            {
                Monitor.Exit(__lockObj);
            }
        }

        /// <summary>
        /// 水质余氯预测
        /// </summary>
        /// <param name="arg">输入参数</param>
        /// <returns></returns>
        ///<response code="200">正常结果</response>
        ///<response code="400">错误</response>
        ///<response code="409">有请求正在运行，不支持同时计算，请稍后再试</response>
        [ResponseType(typeof(WaterQualityResult))]
        [SwaggerRequestExample(typeof(WaterConcentrationArg), typeof(WA_WaterConcentration_Example))]
        public IHttpActionResult Concentration(WaterConcentrationArg arg)
        {
            if (arg == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!System.Threading.Monitor.TryEnter(__lockObj))
            {
                LogContentionError();
                return Content(HttpStatusCode.Conflict, conflictMsg);
            }
            try
            {
                //   arg.ModelPath = CopyNewModel();
                //try
                //{
                //    System.Threading.Monitor.Enter(__lockObj, ref isRunning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                WaterQualityResult result = WengAnHandler.Concentration(arg, Convert.ToInt32(ConfigurationManager.AppSettings["WaterConcentrationScenarioId"]), Convert.ToDouble(ConfigurationManager.AppSettings["WaterConcentrationPressureEngineSimulationDuration"]), true, isServerModeLicense, workOnCopiedModel, demandAdjustmentScenarioId);
                CheckAndLogWaterEngineError(result);
                return Ok(result);
            }
            finally
            {
                System.Threading.Monitor.Exit(__lockObj);
            }
        }

        /// <summary>
        /// 水龄预测
        /// </summary>
        /// <returns></returns>
        /// <response code="200">正常结果</response>
        ///<response code="400">错误</response>
        ///<response code="409">有请求正在运行，不支持同时计算，请稍后再试</response>
        [SwaggerRequestExample(typeof(WaterAgeArg), typeof(WA_WaterAge_Example))]
        [ResponseType(typeof(WaterQualityResult))]
        public IHttpActionResult WaterAge(WaterAgeArg arg)
        {
            if (arg == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!System.Threading.Monitor.TryEnter(__lockObj))
            {
                LogContentionError();
                return Content(HttpStatusCode.Conflict, conflictMsg);
            }
            //   CheckLicense();
            try
            {
                // arg.ModelPath = CopyNewModel();
                //try
                //{
                //    System.Threading.Monitor.Enter(__lockObj, ref isRunning);
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                WaterQualityResult result = WengAnHandler.WaterAge(arg, Convert.ToInt32(ConfigurationManager.AppSettings["WaterAgeScenarioId"]), Convert.ToDouble(ConfigurationManager.AppSettings["WaterAgePressureEngineSimulationDuration"]), true, isServerModeLicense, workOnCopiedModel, demandAdjustmentScenarioId);
                CheckAndLogWaterEngineError(result);
                return Ok(result);
            }
            finally
            {
                System.Threading.Monitor.Exit(__lockObj);
            }
        }

        #region 更改管道，阀门状态

        /// <summary>
        /// 更改管道开闭状态(0-打开，1-关闭)
        /// </summary>
        /// <param name="pipeId">管道id</param>
        /// <param name="pipeStatus">管道状态(0-打开，1-关闭)</param>
        /// <returns></returns>
        /// <response code="200">正常结果</response>
        ///<response code="400">错误</response>
        ///<response code="409">有请求正在运行，不支持同时计算，请稍后再试</response>
        public IHttpActionResult SetPipeStatus(PipeStatusArg arg)
        {
            if (arg == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!System.Threading.Monitor.TryEnter(__lockObj))
            {
                LogContentionError();
                return Content(HttpStatusCode.Conflict, conflictMsg);
            }

            try
            {
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                WaterGEMSModel wm = new WaterGEMSModel();
                wm.ProductId = ProductId.Bentley_WaterGEMS;
                bool licenseSuccess = wm.OpenDataSource(arg.ModelPath, workOnCopiedModel, isServerModeLicense);
                if (!licenseSuccess)
                {
                    throw new LicenseClientException("License不正常");
                }
                IField pipeStatusField = wm.DomainDataSet.FieldManager.DomainElementField(StandardFieldName.PipeStatus, (int)AlternativeType.InitialSettingsAlternative, (int)DomainElementType.IdahoPipeElementManager);
                ((IEditField)pipeStatusField).SetValue(arg.PipeId, (int)arg.PipeStatus);
                wm.CloseDataSource();
                return Ok("修改管道状态成功");
            }
            finally
            {
                Monitor.Exit(__lockObj);
            }
        }

        /// <summary>
        /// 更改阀门状态(0-激活，1-非激活，2-关闭)
        /// </summary>
        /// <param name="ValveId">阀门id</param>
        /// <param name="ValveStatus">阀门状态(0-激活，1-非激活，2-关闭)</param>
        /// <returns></returns>
        /// <response code="200">正常结果</response>
        ///<response code="400">错误</response>
        ///<response code="409">有请求正在运行，不支持同时计算，请稍后再试</response>
        public IHttpActionResult SetValveInitialStatus(ValveInitialStatusArg arg)
        {
            if (arg == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!System.Threading.Monitor.TryEnter(__lockObj))
            {
                LogContentionError();
                return Content(HttpStatusCode.Conflict, conflictMsg);
            }

            try
            {
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                WaterGEMSModel wm = new WaterGEMSModel();
                wm.ProductId = ProductId.Bentley_WaterGEMS;
                bool licenseSuccess = wm.OpenDataSource(arg.ModelPath, workOnCopiedModel, isServerModeLicense);
                if (!licenseSuccess)
                {
                    throw new LicenseClientException("License不正常");
                }
                IField valveInitialStatusField = wm.DomainDataSet.FieldManager.DomainElementField(StandardFieldName.ValveSetting, StandardAlternativeName.InitialSettings,
                    StandardDomainElementTypeName.BaseValve);
                ((IEditField)valveInitialStatusField).SetValue(arg.ValveId, (int)arg.ValveStatus);
                wm.CloseDataSource();
                return Ok("修改阀门状态成功");
            }
            finally
            {
                Monitor.Exit(__lockObj);
            }
        }
        /// <summary>
        /// 更改隔离阀状态(0-打开，1-关闭) 
        /// </summary>
        /// <param name="ValveId">阀门id</param>
        /// <param name="ValveStatus">阀门状态(0-打开，1-关闭)</param>
        /// <returns></returns>
        /// <response code="200">正常结果</response>
        ///<response code="400">错误</response>
        ///<response code="409">有请求正在运行，不支持同时计算，请稍后再试</response>
        public IHttpActionResult SetIsolationValveInitialStatus(IsolationValveInitialStatusArg arg)
        {
            if (arg == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!System.Threading.Monitor.TryEnter(__lockObj))
            {
                LogContentionError();
                return Content(HttpStatusCode.Conflict, conflictMsg);
            }

            try
            {
                _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
                WaterGEMSModel wm = new WaterGEMSModel();
                wm.ProductId = ProductId.Bentley_WaterGEMS;
                bool licenseSuccess = wm.OpenDataSource(arg.ModelPath, workOnCopiedModel, isServerModeLicense);
                if (!licenseSuccess)
                {
                    throw new LicenseClientException("License不正常");
                }
                IField isoInitialStatusField = wm.DomainDataSet.FieldManager.DomainElementField(StandardFieldName.IsolationValveInitialSetting, StandardAlternativeName.InitialSettings,
                    StandardDomainElementTypeName.PressureIsolationValve);
                ((IEditField)isoInitialStatusField).SetValue(arg.ValveId, arg.ValveStatus);
                wm.CloseDataSource();
                return Ok("修改隔离阀状态成功");
            }
            finally
            {
                Monitor.Exit(__lockObj);
            }
        }
        #endregion

        private void CheckAndLogWaterEngineError(WaterEngineBaseResult baseResult)
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

        private void LogContentionError()
        {
            _logger.Error("{0}: {1}", conflictMsg, new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name);
        }

        //执行前将模型拷贝到日期命名目录
        private string CopyNewModel()
        {
            if (isRunning)
            {
                LogContentionError();
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
                File.Copy(demoModelPath, newModelPath);
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

        private bool CheckLicense()
        {
            ObjectCache cache = MemoryCache.Default;
            bool isLicenseOk = Convert.ToBoolean(cache.Get("isLicenseOk"));
            if (isLicenseOk)
            {
                return true;
            }

            _logger.Information("Start check license");
            ProductRelease pr = new ProductRelease(ProductId.Bentley_WaterGEMS, "10.00.00.00");
            if (isServerModeLicense)
            {
                var m_managedLicense = new ManagedLicense((int)ProductId.Bentley_WaterGEMS, "WaterGEMS", "10.00.00.00");   // to be changed: Use OpenFlowAnalysisAPI product
                try
                {
                    #region Check Server Mode SES Licensing 
                    if (m_managedLicense.IsServerModeEnabled())
                    {
                        _logger.Information("Check Server Mode SES Licensing");
                        LicenseRunStatus managedStatus = m_managedLicense.StartServerLicense();
                        if (managedStatus == LicenseRunStatus.Ok)
                        {
                            CheckHaestadLicenseFacadeLicense(pr);
                        }
                        if (m_managedLicense != null)
                        {
                            m_managedLicense.StopLicense();
                        }
                        #endregion
                        return true;
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Trim() == "The Client was not successfully initialized.")
                    {
                        _logger.Error("The server is not configured in ServerMode");
                        throw new Exception("The server is not configured in ServerMode");
                    }
                    else
                        throw;
                }
            }
            else
            {
                _logger.Information("in Appsetting license");
                CheckHaestadLicenseFacadeLicense(pr);
                return true;
            }
            _logger.Information("end license");
            //CacheItemPolicy policy = new CacheItemPolicy
            //{
            //    SlidingExpiration = TimeSpan.FromHours(6)
            //};
            return false;
        }
        /// <summary>
        /// Check whether Haestad.LicenseFacade.License is OK, hydraulic engine only accepts this license
        /// </summary>
        /// <param name="pr"></param>
        /// <exception cref="LicenseClientException"></exception>
        private void CheckHaestadLicenseFacadeLicense(ProductRelease pr)
        {
            LicenseRunStatusEnum status;
            License m_license = License.Default(pr, IntPtr.Zero, null);
            if (m_license.Initialize())
            {
                status = m_license.StartDesktop();
                if (status == LicenseRunStatusEnum.OK)
                {
                    _logger.Information("LicenseRunStatusEnum is OK");
                }
                else
                {
                    throw new LicenseClientException("Can't get Haestad.LicensingFacade.License");
                }
            }
            if (m_license != null)
                m_license.Dispose();
        }
        /// <summary>
        /// 更新需水量
        /// </summary>
        /// <param name="arg"></param>
        /// <remarks>每半个小时， 平台 CALL 我们这个 API, 计算未来 24 小时的预报</remarks>
        /// <returns></returns>
        /// <response code="200">正常结果</response>
        ///<response code="400">错误</response>
        ///<response code="409">有请求正在运行，不支持同时计算，请稍后再试</response>
        //[SwaggerRequestExample(typeof(ForecastDemandArg), typeof(WA_UpDateDemand_Example))]
        //[ResponseType(typeof(IHttpActionResult))]
        //public IHttpActionResult UpdateDemand(ForecastDemandArg arg)
        //{
        //    if (isRunning)
        //    {
        //        LogContentionError();
        //        return BadRequest(conflictMsg);
        //    }
        //    //  arg.ModelPath = CopyNewModel();
        //    try
        //    {
        //        System.Threading.Monitor.Enter(__lockObj, ref isRunning);
        //        _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");
        //        WengAnDemandForecast.Run(arg);
        //        //string archiveFileName = $"NewModel_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.zip";
        //        string archiveFileName = HostingEnvironment.MapPath($"~/NewModel.zip"); ;
        //        if (File.Exists(archiveFileName))
        //        {
        //            File.Delete(archiveFileName);
        //        }
        //        string newModel = Path.Combine(Path.GetDirectoryName(arg.ModelPath), "working", "workingmodel.sqlite");
        //        using (ZipArchive zip = ZipFile.Open(archiveFileName, ZipArchiveMode.Create))
        //        {
        //            zip.CreateEntryFromFile(newModel, Path.GetFileName(arg.ModelPath));
        //        }

        //        return Ok("更新需水量成功");
        //        //CheckAndLogWaterEngineError(result);
        //        //return Ok(result);
        //    }
        //    finally
        //    {
        //        if (isRunning)
        //        {
        //            System.Threading.Monitor.Exit(__lockObj);
        //            isRunning = false;
        //        }
        //    }
        //}
        /// <summary>
        /// 下载最新模型文件（定时更新完需水量后的模型）供后续使用
        /// </summary>
        /// <remarks></remarks>
        /// <returns></returns>
        ///<response code="400">错误</response>
        ///<response code="409">有请求正在运行，不支持同时计算，请稍后再试</response>
        //[SwaggerRequestExample(typeof(WengAnCalculationBaseArg), typeof(WA_WaterAge_Example))]
        //[ResponseType(typeof(WaterQualityResult))]
        //public HttpResponseMessage GetLatestModel()
        //{
        //    if (isRunning)
        //    {
        //        LogContentionError();
        //        var response = new HttpResponseMessage(HttpStatusCode.Conflict);
        //        response.Content = new StringContent(conflictMsg);
        //        return response;
        //    }
        //    try
        //    {
        //        System.Threading.Monitor.Enter(__lockObj, ref isRunning);
        //        _logger.Information($"项目名：{Consts.ProjectName},开始执行 {new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}");

        //        string archiveFileName = HostingEnvironment.MapPath($"~/NewModel.zip");
        //        if (!File.Exists(archiveFileName))
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotFound, "未找到最新模型文件");
        //        }
        //        //  ZipFile.CreateFromDirectory("aa","ff",compress);
        //        var time = File.GetLastWriteTime(archiveFileName).ToString("yyyyMMdd_HHmmss");
        //        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        //        var stream = new FileStream(archiveFileName, FileMode.Open, FileAccess.Read);
        //        response.Content = new StreamContent(stream);
        //        response.Content.Headers.ContentType =
        //            new MediaTypeHeaderValue("application/zip");
        //        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = $"NewModel_{time}.zip"
        //        };
        //        return response;
        //    }
        //    finally
        //    {
        //        if (isRunning)
        //        {
        //            System.Threading.Monitor.Exit(__lockObj);
        //            isRunning = false;
        //        }
        //    }
        //}

    }
}