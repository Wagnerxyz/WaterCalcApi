using Bentley.SelectServer.ManagedClient;
using ChinaWaterLib;
using ChinaWaterLib.Models;
using ChinaWaterUtils;
using Haestad.Calculations.Shanghai.QingDaoWT;
using Haestad.Calculations.Shanghai.WaterGEMS;
using Haestad.Domain;
using Haestad.LicensingFacade;
using Haestad.Support.Support;
using Haestad.Support.User;
using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using WengAn.Args;

namespace WengAn
{
    public class WengAnHandler
    {
        /// <summary>
        ///打开数据模型，且 StartDesktop()获取License不报错 
        /// </summary>
        /// <exception cref="LicenseClientException"></exception>
        public static void OpenDataSourceAndGetLicenseSuccess(WaterGEMSModel wm, WengAnBaseArg arg, bool isServerModeLicense, bool workOnCopiedModel)
        {
            //可通过配置文件传入productversion，但测试和里面hardcode的10.00.00.00没区别。
            //wm.ProductVersion = ConfigurationManager.AppSettings["ProductVersion"];
            bool licenseSuccess = wm.OpenDataSource(arg.ModelPath, workOnCopiedModel, isServerModeLicense);
            if (!licenseSuccess)
            {
                Log.Error("OpenDataSource() License返回false!");
                throw new LicenseClientException("OpenDataSource() License返回false!");
            }
        }
        /// <summary>
        /// 运行水力模型
        /// </summary>
        public static WengAnEpsBaseResult RunEPS(RunEPSArg arg, int scenarioId, double pressureEngineSimulationDuration, bool needRealTimeData, bool isServerModeLicense, bool workOnCopiedModel, int demandAdjustmentScenarioId = 0)
        {
            var wm = new WaterGEMSModel();
            var result = new WengAnEpsBaseResult();
            result.StartTime = arg.StartTime;
            try
            {
                OpenDataSourceAndGetLicenseSuccess(wm, arg, isServerModeLicense, workOnCopiedModel);
                if (demandAdjustmentScenarioId != 0)
                {
                    DemandAdjustment(arg, wm, demandAdjustmentScenarioId);
                }
                wm.SetActiveScenario(scenarioId);
                int scenarioIdaa = wm.DomainDataSet.ScenarioManager.ActiveScenarioID;
                //IDomainElementManager pipeManager = wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoPipeElementManager);
                //ModelingElementCollection allPipes = pipeManager.Elements();
                //IDomainElementManager junctionManager = wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoJunctionElementManager);
                //ModelingElementCollection allJunctions = junctionManager.Elements();
                //IDomainElementManager airManager = wm.DomainDataSet.DomainElementManager((int)DomainElementType.AirValveElementManager);
                //ModelingElementCollection allAirValves = airManager.Elements();

                #region set current value
                if (needRealTimeData)
                {
                    SetWengAnOnlineData.SetCurrentPumpValveReservoir(wm, arg);
                }
                #endregion

                //设置压力引擎开始时间
                wm.SetSimulationStartTime(arg.StartTime);
                wm.PressureCalculationOption.SetPressureEngineSimulationDuration(pressureEngineSimulationDuration);
                IUserNotification[] pressureNotifs = wm.RunPressureCalculation();
                if (HasEngineFatalError(pressureNotifs, result))
                {
                    return result;
                }

                #region Get Pipe&Node Result

                if (arg.ResultNodeIds == null || arg.ResultNodeIds.Length < 1)
                {
                    HmIDCollection allJunctionIds =
                        wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoJunctionElementManager).ElementIDs();

                    result.EpsNodeResult = GetNodeEpsTimePointPressureResult(wm, allJunctionIds.ToArray());
                }
                else
                {
                    result.EpsNodeResult = GetNodeEpsTimePointPressureResult(wm, arg.ResultNodeIds);
                }
                if (arg.ResultPipeIds == null || arg.ResultPipeIds.Length < 1)
                {
                    HmIDCollection allPipeIds =
                        wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoPipeElementManager).ElementIDs();

                    result.EpsPipeResult = GetPipeEpsTimePointResult(wm, allPipeIds.ToArray());
                }
                else
                {
                    result.EpsPipeResult = GetPipeEpsTimePointResult(wm, arg.ResultPipeIds);
                }
                #endregion

                return result;
            }
            finally
            {
                wm.CloseDataSource();
            }
        }
        /// <summary>
        /// 消防事件
        /// </summary>
        /// <param name="arg">输入参数</param>
        /// <returns></returns>
        public static WengAnEpsBaseResult FireDemandAtOneNode(FireDemandArg arg, int scenarioId, double pressureEngineSimulationDuration, bool needRealTimeData, bool isServerModeLicense, bool workOnCopiedModel, int demandAdjustmentScenarioId = 0)
        {
            WaterGEMSModel wm = new WaterGEMSModel();
            wm.ProductId = ProductId.Bentley_WaterGEMS;
            var result = new WengAnEpsBaseResult();
            result.StartTime = arg.StartTime;

            try
            {
                OpenDataSourceAndGetLicenseSuccess(wm, arg, isServerModeLicense, workOnCopiedModel);
                if (demandAdjustmentScenarioId != 0)
                {
                    DemandAdjustment(arg, wm, demandAdjustmentScenarioId);
                }

                wm.SetActiveScenario(scenarioId);
                if (needRealTimeData)
                {
                    SetWengAnOnlineData.SetCurrentPumpValveReservoir(wm, arg);
                }
                wm.PressureCalculationOption.SetPressureEngineCalculationType(EpaNetEngine_CalculationTypeEnum.SCADAAnalaysisType);
                wm.PressureCalculationOption.SetSCADACalculationType(SCADACalculationTypeEnum.HydraulicsOnly);

                wm.PressureCalculationOption.ClearSCADAFireDemand();
                wm.PressureCalculationOption.AddSCADAFireDemand(arg.NodeId, arg.DemandInLitersPerSecond, arg.FireStartTime, arg.DurationHours);
                //设置压力引擎开始时间
                wm.SetSimulationStartTime(arg.StartTime);
                wm.PressureCalculationOption.SetPressureEngineSimulationDuration(pressureEngineSimulationDuration);
                IUserNotification[] pressureNotifs = wm.RunPressureCalculation();
                if (HasEngineFatalError(pressureNotifs, result))
                {
                    return result;
                }

                #region Get EPS Result

                if (arg.ResultNodeIds == null || arg.ResultNodeIds.Length < 1)
                {
                    HmIDCollection allJunctionIds =
                        wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoJunctionElementManager).ElementIDs();
                    result.EpsNodeResult = GetNodeEpsTimePointPressureResult(wm, allJunctionIds.ToArray());
                }
                else
                {
                    result.EpsNodeResult = GetNodeEpsTimePointPressureResult(wm, arg.ResultNodeIds);
                }
                if (arg.ResultPipeIds == null || arg.ResultPipeIds.Length < 1)
                {
                    HmIDCollection allPipeIds =
                        wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoPipeElementManager).ElementIDs();

                    result.EpsPipeResult = GetPipeEpsTimePointResult(wm, allPipeIds.ToArray());
                }
                else
                {
                    result.EpsPipeResult = GetPipeEpsTimePointResult(wm, arg.ResultPipeIds);
                }
                #endregion
                return result;
            }
            finally
            {
                wm.CloseDataSource();
            }

        }

        public static WaterQualityResult WaterAge(WaterAgeArg arg, int scenarioId, double pressureEngineSimulationDuration, bool needRealTimeData, bool isServerModeLicense, bool workOnCopiedModel, int demandAdjustmentScenarioId = 0)
        {
            WaterGEMSModel wm = new WaterGEMSModel();
            wm.ProductId = ProductId.Bentley_WaterGEMS;
            var result = new WaterQualityResult();
            result.StartTime = arg.StartTime;
            try
            {
                OpenDataSourceAndGetLicenseSuccess(wm, arg, isServerModeLicense, workOnCopiedModel);
                if (demandAdjustmentScenarioId != 0)
                {
                    DemandAdjustment(arg, wm, demandAdjustmentScenarioId);
                }
                if (needRealTimeData)
                {
                    SetWengAnOnlineData.SetCurrentPumpValveReservoir(wm, arg);
                }
                wm.SetActiveScenario(scenarioId);
                WaterQualityCalculation wqc = new WaterQualityCalculation(wm);
                //设置压力引擎开始时间
                wm.SetSimulationStartTime(arg.StartTime);
                wm.PressureCalculationOption.SetPressureEngineSimulationDuration(pressureEngineSimulationDuration);
                IUserNotification[] pressureNotifs = wm.RunPressureCalculation();
                if (HasEngineFatalError(pressureNotifs, result))
                {
                    return result;
                }

                #region Get Result
                Dictionary<int, double[]> nodeResult = new Dictionary<int, double[]>();

                if (arg.ResultNodeIds == null || arg.ResultNodeIds.Length < 1)
                {
                    HmIDCollection allNodesIds = wm.DomainDataSet
                        .DomainElementManager((int)DomainElementType.IdahoJunctionElementManager).ElementIDs();
                    foreach (var id in allNodesIds)
                    {
                        double[] ages = wqc.GetAgeInHours(id).TakeLast(arg.LastCalculationResultCount).ToArray();
                        nodeResult.Add(id, ages);
                    }
                    result.NodeResult = nodeResult;
                }
                else
                {
                    foreach (var id in arg.ResultNodeIds)
                    {
                        double[] ages = wqc.GetAgeInHours(id).TakeLast(arg.LastCalculationResultCount).ToArray();
                        nodeResult.Add(id, ages);
                    }
                    result.NodeResult = nodeResult;
                }

                Dictionary<int, double[]> pipeResult = new Dictionary<int, double[]>();
                if (arg.ResultPipeIds == null || arg.ResultPipeIds.Length < 1)
                {
                    HmIDCollection allPipeIds =
                        wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoPipeElementManager).ElementIDs();

                    foreach (var id in allPipeIds)
                    {
                        double[] ages = wqc.GetAgeInHours(id).TakeLast(arg.LastCalculationResultCount).ToArray();
                        pipeResult.Add(id, ages);
                    }
                    result.PipeResult = pipeResult;
                }
                else
                {
                    foreach (var id in arg.ResultPipeIds)
                    {
                        double[] ages = wqc.GetAgeInHours(id).TakeLast(arg.LastCalculationResultCount).ToArray();
                        pipeResult.Add(id, ages);
                    }
                    result.PipeResult = pipeResult;
                }
                #endregion
                return result;
            }
            finally
            {
                wm.CloseDataSource();
            }
        }
        /// <summary>
        /// 余氯分析要用户传三个水厂的出厂余氯浓度
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static WaterQualityResult Concentration(WaterConcentrationArg arg, int scenarioId, double pressureEngineSimulationDuration, bool needRealTimeData, bool isServerModeLicense, bool workOnCopiedModel, int demandAdjustmentScenarioId = 0)
        {
            WaterGEMSModel wm = new WaterGEMSModel();
            wm.ProductId = ProductId.Bentley_WaterGEMS;
            var result = new WaterQualityResult();
            result.StartTime = arg.StartTime;
            try
            {
                OpenDataSourceAndGetLicenseSuccess(wm, arg, isServerModeLicense, workOnCopiedModel);
                if (demandAdjustmentScenarioId != 0)
                {
                    DemandAdjustment(arg, wm, demandAdjustmentScenarioId);
                }

                wm.SetActiveScenario(scenarioId);
                if (needRealTimeData)
                {
                    SetWengAnOnlineData.SetCurrentPumpValveReservoir(wm, arg);
                }
                WaterQualityCalculation wqc = new WaterQualityCalculation(wm);
                foreach (var d in arg.CurrentConcentration)
                {
                    wqc.SetInitialConcentration(d.Key, d.Value);
                }
                //设置压力引擎开始时间
                wm.SetSimulationStartTime(arg.StartTime);
                wm.PressureCalculationOption.SetPressureEngineSimulationDuration(pressureEngineSimulationDuration);
                IUserNotification[] pressureNotifs = wm.RunPressureCalculation();
                if (HasEngineFatalError(pressureNotifs, result))
                {
                    return result;
                }
                #region Read Result
                Dictionary<int, double[]> nodeResult = new Dictionary<int, double[]>();

                if (arg.ResultNodeIds == null || arg.ResultNodeIds.Length < 1)
                {
                    HmIDCollection allNodesIds = wm.DomainDataSet
                        .DomainElementManager((int)DomainElementType.IdahoJunctionElementManager).ElementIDs();
                    foreach (var id in allNodesIds)
                    {
                        double[] ages = wqc.GetConcentrationInMGL(id).TakeLast(arg.LastCalculationResultCount).ToArray();
                        nodeResult.Add(id, ages);
                    }
                    result.NodeResult = nodeResult;
                }
                else
                {
                    foreach (var id in arg.ResultNodeIds)
                    {
                        double[] ages = wqc.GetConcentrationInMGL(id).TakeLast(arg.LastCalculationResultCount).ToArray();
                        nodeResult.Add(id, ages);
                    }
                    result.NodeResult = nodeResult;
                }

                Dictionary<int, double[]> pipeResult = new Dictionary<int, double[]>();
                if (arg.ResultPipeIds == null || arg.ResultPipeIds.Length < 1)
                {
                    HmIDCollection allPipeIds =
                        wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoPipeElementManager).ElementIDs();

                    foreach (var id in allPipeIds)
                    {
                        double[] ages = wqc.GetConcentrationInMGL(id).TakeLast(arg.LastCalculationResultCount).ToArray();
                        pipeResult.Add(id, ages);
                    }
                    result.PipeResult = pipeResult;
                }
                else
                {
                    foreach (var id in arg.ResultPipeIds)
                    {
                        double[] ages = wqc.GetConcentrationInMGL(id).TakeLast(arg.LastCalculationResultCount).ToArray();
                        pipeResult.Add(id, ages);
                    }
                    result.PipeResult = pipeResult;
                }
                #endregion
                return result;
            }
            finally
            {
                wm.CloseDataSource();
            }

        }

        /// <summary>
        /// 瓮安爆管影响分析
        /// </summary>
        /// <returns></returns>
        public static BreakPipeResult BreakPipe(BreakPipeArg arg, int scenarioId, double pressureEngineSimulationDuration, bool needRealTimeData, bool isServerModeLicense, bool workOnCopiedModel, int demandAdjustmentScenarioId = 0)
        {

            WaterGEMSModel wm = new WaterGEMSModel();
            var result = new BreakPipeResult();
            result.StartTime = arg.StartTime;
            var valveInitialDict = new Dictionary<int, ValveSettingEnum>();
            var isolationValveInitialDict = new Dictionary<int, IsolationValveInitialSettingEnum>();
            try
            {
                OpenDataSourceAndGetLicenseSuccess(wm, arg, isServerModeLicense, workOnCopiedModel);
                if (demandAdjustmentScenarioId != 0)
                {
                    DemandAdjustment(arg, wm, demandAdjustmentScenarioId);
                }

                wm.SetActiveScenario(scenarioId);
                if (needRealTimeData)
                {
                    SetWengAnOnlineData.SetCurrentPumpValveReservoir(wm, arg);
                }
                wm.PressureCalculationOption.SetPressureEngineCalculationType(EpaNetEngine_CalculationTypeEnum.SCADAAnalaysisType);
                wm.PressureCalculationOption.SetSCADACalculationType(SCADACalculationTypeEnum.HydraulicsOnly);

                //var valvesToClose = new HmIDCollection();
                //var pipesToClose = new HmIDCollection();
                //var isolationValvesToClose = new HmIDCollection();
                //var isolatedPartialPipeIds = new List<HmiPartialPipe>(0);
                //var isolatedPipeIds = new HmIDCollection();
                //var isolatedNodeIds = new HmIDCollection();
                //var isolatedCustomerIds = new HmIDCollection();
                //var outagePartialPipeIds = new List<HmiPartialPipe>(0);
                //var outagePipeIds = new HmIDCollection();
                //var outageNodeIds = new HmIDCollection();
                //var outageCustomerIds = new HmIDCollection();
                new NetworkIsolation(wm.DomainDataSet).GetElementsOfIsolatingPipeBreak(arg.PipeId, arg.BreakPointDistanceToStartNode,
                    new HmIDCollection(arg.ValvesToExclude), out var valvesToClose, out var pipesToClose,
                    out var isolationValvesToClose, out var isolatedPartialPipeIds, out var isolatedPipeIds,
                    out var isolatedNodeIds, out var isolatedCustomerIds, out var outagePartialPipeIds,
                    out var outagePipeIds, out var outageNodeIds, out var outageCustomerIds);


                result.ValvesToClose = valvesToClose.ToArray(); //important
                //baseResult.PipesToClose = pipesToClose.ToArray();
                result.IsolationValvesToClose = isolationValvesToClose.ToArray(); //important
                //baseResult.IsolatedPartialPipeIds = isolatedPartialPipeIds;
                result.IsolatedPipeIds = isolatedPipeIds.ToArray();
                result.IsolatedNodeIds = isolatedNodeIds.ToArray();
                result.IsolatedCustomerIds = isolatedCustomerIds.ToArray();
                //baseResult.OutagePartialPipeIds = outagePartialPipeIds;
                //baseResult.OutagePipeIds = outagePipeIds.ToArray();
                //baseResult.OutageNodeIds = outageNodeIds.ToArray();
                //baseResult.OutageCustomerIds = outageCustomerIds.ToArray();

                #region Save Valve Status

                //GetValveInitialStatus，GetIsolationValveInitialStatus

                foreach (var id in result.ValvesToClose)
                {
                    valveInitialDict.Add(id, wm.InitialSetting.GetValveInitialStatus(id));
                }
                foreach (var id in result.IsolationValvesToClose)
                {
                    isolationValveInitialDict.Add(id, wm.InitialSetting.GetIsolationValveInitialStatus(id));
                }

                #endregion

                #region 关阀

                foreach (var id in result.ValvesToClose)
                {
                    wm.InitialSetting.SetValveInitialStatus(id, ValveSettingEnum.ValveClosedType);
                }
                foreach (var id in result.IsolationValvesToClose)
                {
                    wm.InitialSetting.SetIsolationValveInitialStatus(id, IsolationValveInitialSettingEnum.IsolationValveClosedType);
                }

                #endregion
                //设置压力引擎开始时间
                wm.SetSimulationStartTime(arg.BreakPipeStartTime);
                wm.PressureCalculationOption.SetPressureEngineSimulationDuration(pressureEngineSimulationDuration);
                IUserNotification[] pressureNotifs = wm.RunPressureCalculation();
                if (HasEngineFatalError(pressureNotifs, result))
                {
                    return result;
                }

                #region Get Pipe&Node Result

                if (arg.ResultNodeIds == null || arg.ResultNodeIds.Length < 1)
                {
                    HmIDCollection allJunctionIds =
                        wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoJunctionElementManager).ElementIDs();

                    result.EpsNodeResult = GetNodeEpsTimePointPressureResult(wm, allJunctionIds.ToArray());
                }
                else
                {
                    result.EpsNodeResult = GetNodeEpsTimePointPressureResult(wm, arg.ResultNodeIds);
                }
                if (arg.ResultPipeIds == null || arg.ResultPipeIds.Length < 1)
                {
                    HmIDCollection allPipeIds =
                        wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoPipeElementManager).ElementIDs();

                    result.EpsPipeResult = GetPipeEpsTimePointResult(wm, allPipeIds.ToArray());
                }
                else
                {
                    result.EpsPipeResult = GetPipeEpsTimePointResult(wm, arg.ResultPipeIds);
                }
                #endregion

                return result;
            }
            finally
            {
                //把前面保存的值设回去
                if (valveInitialDict.Any())
                {
                    foreach (var item in valveInitialDict)
                    {
                        wm.InitialSetting.SetValveInitialStatus(item.Key, item.Value);
                    }
                }
                if (isolationValveInitialDict.Any())
                {
                    foreach (var item in isolationValveInitialDict)
                    {
                        wm.InitialSetting.SetIsolationValveInitialStatus(item.Key, item.Value);
                    }
                }
                wm.CloseDataSource();
            }
        }

        /// <summary>
        /// 多水源供水分析 水源追踪
        /// </summary>
        public static WaterHeadTraceResult GetWaterTraceResultsForMultipleElementIds(WaterTraceArg arg, int scenarioId, double pressureEngineSimulationDuration, bool needRealTimeData, bool isServerModeLicense, bool workOnCopiedModel, int demandAdjustmentScenarioId = 0)
        {
            if (arg.TraceSourceElementIds == null || !arg.TraceSourceElementIds.Any())
            {
                throw new ArgumentException("未传入水源id");
            }
            var wm = new WaterGEMSModel();
            var result = new WaterHeadTraceResult();
            result.StartTime = arg.StartTime;
            try
            {
                OpenDataSourceAndGetLicenseSuccess(wm, arg, isServerModeLicense, workOnCopiedModel);
                if (demandAdjustmentScenarioId != 0)
                {
                    DemandAdjustment(arg, wm, demandAdjustmentScenarioId);
                }

                wm.SetActiveScenario(scenarioId);
                if (needRealTimeData)
                {
                    SetWengAnOnlineData.SetCurrentPumpValveReservoir(wm, arg);
                }
                wm.SetSimulationStartTime(arg.StartTime);
                wm.PressureCalculationOption.SetPressureEngineSimulationDuration(pressureEngineSimulationDuration);
                WaterTraceCalculation wt = new WaterTraceCalculation(wm);
                IUserNotification[] notif = wt.RunTraceCalculationForMultipleTraceElements(arg.TraceSourceElementIds);    // reservoir
                if (HasEngineFatalError(notif, result))
                {
                    return result;
                }
                //读结果
                IDomainElementManager junctionManager = wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoJunctionElementManager);
                ModelingElementCollection allJunctions = junctionManager.Elements();
                IList<int> elementIds = new List<int>();
                foreach (var junction in allJunctions)
                {
                    elementIds.Add(junction.Id);
                }

                result.ElementIds = elementIds;
                var list = new List<SingleElementWaterTraceResultInPercentage>();
                double[] timeSteps = null;
                foreach (var id in arg.TraceSourceElementIds)
                {
                    var obj = new SingleElementWaterTraceResultInPercentage
                    {
                        WaterHeadID = id,
                        Values = wt.GetTraceResultsFromOneTraceSourceElementForMultipleElementIdInPercent(id, elementIds, out timeSteps)
                    };
                    list.Add(obj);
                }

                result.TimeStep = timeSteps;
                result.WaterHeadTracePercentageResults = list;
                return result;
            }
            finally
            {
                wm.CloseDataSource();
            }
        }

        /// <summary>
        /// 检查计算错误，且构建ErrorNotifs
        /// </summary>
        /// <returns></returns>
        private static bool HasEngineFatalError(IUserNotification[] notif, WaterEngineBaseResult baseResult)
        {
            List<IUserNotification> error = notif?.Where(x => x.Level == Haestad.Support.User.NotificationLevel.Error).ToList();
            if (error != null && error.Any())
            {
                baseResult.IsCalculationFailure = true;
                baseResult.ErrorNotifs = Consts.Mapper.Map<List<IUserNotification>, List<UserNotification>>(error);
            }
            return baseResult.IsCalculationFailure;
        }
        //抛出IndexOutOfRangeException 可能是License问题，里面代码wm.RunWTmodel(); Catch了没往外抛
        //将引擎计算开始时间设为arg.StartTime.AddDays(-1)
        /// <summary>
        /// 蓄水量调整
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="wm"></param>
        /// <param name="demandAdjustmentScenarioId"></param>
        /// <exception cref="Exception"></exception>
        public static void DemandAdjustment(WengAnBaseArg arg, WaterGEMSModel wm, int demandAdjustmentScenarioId)
        {
            LicenseCheck(wm, Consts.Logger);

            wm.SetActiveScenario(demandAdjustmentScenarioId);
            new WengAnProj().RTDemandAdjustment(wm, arg.StartTime, arg.StartTime.AddDays(-1), arg.FlowSensors);
            wm.CloseResults();
        }
        /// <summary>
        /// 检查LicenseRunStatusEnum和LicenseStatus
        /// </summary>
        /// <param name="wm"></param>
        /// <param name="logger"></param>
        /// <exception cref="Exception"></exception>
        private static void LicenseCheck(WaterGEMSModel wm, ILogger logger)
        {
            var license = wm.License;
            if (license == null)
            {
                throw new Exception("License null不正常");
            }

            if (license.RunStatus == LicenseRunStatusEnum.Unknown || license.RunStatus == LicenseRunStatusEnum.Shutdown)
            {
                logger.Error("License Run Status 不正常 (Unknown||Shutdown)");
                logger.Error("licenseRunStatus: " + license.RunStatus.ToString());
                throw new Exception("License Run Status 不正常(Unknown||Shutdown)");
            }

            Haestad.LicensingFacade.LicenseStatus licenseStatus = license.GetLicenseStatus();
            if (licenseStatus != Haestad.LicensingFacade.LicenseStatus.OK &&
                licenseStatus != Haestad.LicensingFacade.LicenseStatus.Trial)
            {
                logger.Error("License Status 不正常 (!OK&&!Trial)");
                logger.Error("licenseStatus: " + licenseStatus.ToString());
                throw new Exception("License Status 不正常 (!OK&&!Trial)");
            }
        }

        /// <summary>
        /// 返回运行watergems的节点，管道结果
        /// </summary>
        private static List<EpsNodeTimeSeriesResult> GetNodeEpsTimePointPressureResult(WaterGEMSModel wm, int[] nodesIds)
        {
            double[] timeSteps = wm.PressureResult.GetPressureEngineCalculationTimeStepsInSeconds(); //读取EPS报告点动态结果
            var timePointNodeResults = new List<EpsNodeTimeSeriesResult>();

            foreach (var id in nodesIds)
            {
                var epsResult = new EpsNodeTimeSeriesResult();
                epsResult.Id = id;
                epsResult.Label = wm.GetDomainElementLabel(id);
                epsResult.TimeSteps = timeSteps;
                epsResult.Pressures = wm.PressureResult.GetNodePressureInKiloPascals(id);
                //epsResult.HGL = wm.PressureResult.GetNodeHGLInMeters(id);
                timePointNodeResults.Add(epsResult);
            }

            return timePointNodeResults;
        }

        private static List<EpsPipeTimeSeriesResult> GetPipeEpsTimePointResult(WaterGEMSModel wm, int[] pipeIds)
        {
            double[] timeSteps = wm.PressureResult.GetPressureEngineCalculationTimeStepsInSeconds(); //读取EPS报告点动态结果
            var timePointPipeResults = new List<EpsPipeTimeSeriesResult>();
            //HmIDCollection pipeIds = wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoPipeElementManager)
            //    .ElementIDs();
            foreach (var id in pipeIds)
            {
                var epsResult = new EpsPipeTimeSeriesResult();
                epsResult.Id = id;
                epsResult.Label = wm.GetDomainElementLabel(id);
                epsResult.TimeSteps = timeSteps;
                epsResult.Flows = wm.PressureResult.GetPipeFlowInCubicMetersPerSecond(id);
                epsResult.Velocities = wm.PressureResult.GetPipeVelocityInMetersPerSecond(id);
                //epsResult.PipeHeadLoss = wm.PressureResult.GetPipeHeadlossInMeters(id);
                epsResult.HeadlossGradient = wm.PressureResult.GetPipeUnitHeadlossInMeterPerKM(id);
                timePointPipeResults.Add(epsResult);
            }
            return timePointPipeResults;

        }

        #region 废弃
        [Obsolete]
        public static BreakPipeResult BreakPipe2(BreakPipeArg arg)
        {

            WaterGEMSModel wm = new WaterGEMSModel();
            var result = new BreakPipeResult();
            var valveInitialDict = new Dictionary<int, ValveSettingEnum>();
            var isolationValveInitialDict = new Dictionary<int, IsolationValveInitialSettingEnum>();
            try
            {
                wm.OpenDataSource(arg.ModelPath, true, false);
                new WengAnProj().RTDemandAdjustment(wm, arg.StartTime, arg.StartTime.AddDays(-1), arg.FlowSensors);

                IDomainDataSet dataSet = wm.DomainDataSet;
                wm.SetActiveScenario(3973);
                wm.PressureCalculationOption.SetPressureEngineCalculationType(EpaNetEngine_CalculationTypeEnum.SCADAAnalaysisType);
                wm.PressureCalculationOption.SetSCADACalculationType(SCADACalculationTypeEnum.HydraulicsOnly);
                SetWengAnOnlineData.SetCurrentPumpValveReservoir(wm, arg);

                new NetworkIsolation(dataSet).GetElementsOfIsolatingPipeBreak(arg.PipeId, arg.BreakPointDistanceToStartNode,
                    new HmIDCollection(arg.ValvesToExclude), out var valvesToClose, out var pipesToClose,
                    out var isolationValvesToClose, out var isolatedPartialPipeIds, out var isolatedPipeIds,
                    out var isolatedNodeIds, out var isolatedCustomerIds, out var outagePartialPipeIds,
                    out var outagePipeIds, out var outageNodeIds, out var outageCustomerIds);


                result.ValvesToClose = valvesToClose.ToArray(); //important
                //baseResult.PipesToClose = pipesToClose.ToArray();
                result.IsolationValvesToClose = isolationValvesToClose.ToArray(); //important
                //baseResult.IsolatedPartialPipeIds = isolatedPartialPipeIds;
                result.IsolatedPipeIds = isolatedPipeIds.ToArray();
                result.IsolatedNodeIds = isolatedNodeIds.ToArray();
                result.IsolatedCustomerIds = isolatedCustomerIds.ToArray();
                //baseResult.OutagePartialPipeIds = outagePartialPipeIds;
                //baseResult.OutagePipeIds = outagePipeIds.ToArray();
                //baseResult.OutageNodeIds = outageNodeIds.ToArray();
                //baseResult.OutageCustomerIds = outageCustomerIds.ToArray();

                #region Save Valve Status

                //GetValveInitialStatus，GetIsolationValveInitialStatus

                //foreach (var id in baseResult.ValvesToClose)
                //{
                //    valveInitialDict.Add(id, wm.InitialSetting.GetValveInitialStatus(id));
                //}
                //foreach (var id in baseResult.IsolationValvesToClose)
                //{
                //    isolationValveInitialDict.Add(id, wm.InitialSetting.GetIsolationValveInitialStatus(id));
                //}

                //#endregion

                //#region 关阀

                //foreach (var id in baseResult.ValvesToClose)
                //{
                //    wm.InitialSetting.SetValveInitialStatus(id, ValveSettingEnum.ValveClosedType);
                //}
                //foreach (var id in baseResult.IsolationValvesToClose)
                //{
                //    wm.InitialSetting.SetIsolationValveInitialStatus(id, IsolationValveInitialSettingEnum.IsolationValveClosedType);
                //}

                #endregion

                wm.PressureCalculationOption.SetSCADASimulationMode(SCADASimulationModeEnum.BaseInitialCondition);

                wm.PressureCalculationOption.ClearPumpAndOtherControlOverride();

                //foreach (var id in baseResult.ValvesToClose)
                //{
                //    wm.InitialSetting.SetValveInitialStatus(id, ValveSettingEnum.ValveClosedType);
                //}
                //foreach (var id in baseResult.IsolationValvesToClose)
                //{
                //    wm.InitialSetting.SetIsolationValveInitialStatus(id, IsolationValveInitialSettingEnum.IsolationValveClosedType);
                //}



                //设置压力引擎开始时间
                var now = DateTime.Now;
                wm.PressureCalculationOption.SetPressureEngineSimulationStartDate(now);
                wm.PressureCalculationOption.SetPressureEngineSimulationStartTime(now);
                //wm.PressureCalculationOption.
                wm.PressureCalculationOption.AddValveSettingControlOverride(7597, ValveSettingEnum.ValveClosedType, now, 2);

                IUserNotification[] notif = wm.RunPressureCalculation();
                List<IUserNotification> error = notif?.Where(x => x.Level == Haestad.Support.User.NotificationLevel.Error).ToList();
                if (error != null && error.Any())
                {
                    result.IsCalculationFailure = true;
                    result.ErrorNotifs = Consts.Mapper.Map<List<IUserNotification>, List<UserNotification>>(error);
                }
                #region 比较节点水压
                //todo:返回结构已变化
                IDomainElementManager junctionManager =
                    wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoJunctionElementManager);
                ModelingElementCollection allJunctions = junctionManager.Elements();
                var pressureDict = new Dictionary<int, double[]>();
                foreach (var junction in allJunctions)
                {
                    pressureDict.Add(junction.Id, wm.PressureResult.GetNodePressureInKiloPascals(junction.Id));
                }

                //result.NodePressures = pressureDict;
                #endregion

                return result;
            }
            finally
            {
                ////把前面保存的值设回去
                //if (valveInitialDict.Any())
                //{
                //    foreach (var item in valveInitialDict)
                //    {
                //        wm.InitialSetting.SetValveInitialStatus(item.Key, item.Value);
                //    }
                //}
                //if (isolationValveInitialDict.Any())
                //{
                //    foreach (var item in isolationValveInitialDict)
                //    {
                //        wm.InitialSetting.SetIsolationValveInitialStatus(item.Key, item.Value);
                //    }
                //}
                wm.CloseDataSource();
            }
        }

        #endregion


        public static WengAnEpsBaseResult UpdateControl(FireDemandArg arg, int scenarioId, double pressureEngineSimulationDuration, bool needRealTimeData, bool isServerModeLicense, int demandAdjustmentScenarioId = 0)
        {
            WaterGEMSModel wm = new WaterGEMSModel();
            wm.ProductId = ProductId.Bentley_WaterGEMS;
            var result = new WengAnEpsBaseResult();
            result.StartTime = arg.StartTime;

            try
            {
                bool licenseSuccess = wm.OpenDataSource(arg.ModelPath, true, isServerModeLicense);
                if (!licenseSuccess)
                {
                    throw new LicenseClientException("License不正常");
                }
                if (demandAdjustmentScenarioId != 0)
                {
                    DemandAdjustment(arg, wm, demandAdjustmentScenarioId);
                }

                wm.SetActiveScenario(scenarioId);

                wm.DomainDataSet.SupportElementManager(StandardSupportElementTypeName.ControlSet);



                return result;
            }
            finally
            {
                wm.CloseDataSource();
            }

        }

    }
}