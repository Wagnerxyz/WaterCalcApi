using Haestad.Domain;
using Haestad.Support.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Haestad.Calculations.Shanghai.WaterGEMS;
using Haestad.Calculations.Support;
using Haestad.LicensingFacade;
using Haestad.Support.User;
using Models;
using WengAn.Args;

namespace LiaoDongBayTest
{
    public class WengAnApi
    {


        public static IMapper mapper;
        /// <summary>
        /// 运行水力模型
        /// </summary>
        /// <remarks>
        /// POST /ApiTest/api/WA/WaterTrace?modelpath=555
        /// </remarks>
        /// <param name="modelPath">模型路径</param>
        /// <returns></returns>
        public static WengAnEpsResult RunEPS(string modelpath)
        {
            var wm = new WaterGEMSModel();
            var result = new WengAnEpsResult();

            try
            {
                wm.OpenDataSource(modelpath);
                wm.SetActiveScenario(1);
                //IDomainElementManager pipeManager = wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoPipeElementManager);
                //ModelingElementCollection allPipes = pipeManager.Elements();
                //IDomainElementManager junctionManager = wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoJunctionElementManager);
                //ModelingElementCollection allJunctions = junctionManager.Elements();
                //IDomainElementManager airManager = wm.DomainDataSet.DomainElementManager((int)DomainElementType.AirValveElementManager);
                //ModelingElementCollection allAirValves = airManager.Elements();

                #region set current value
                //Utils.SetCurrentPumpValveTank(wm, args);
                #endregion

                //设置压力引擎开始时间
                var now = DateTime.Now;
                wm.PressureCalculationOption.SetPressureEngineSimulationStartDate(now);
                wm.PressureCalculationOption.SetPressureEngineSimulationStartTime(now);

                IUserNotification[] pressureNotifs = wm.RunPressureCalculation();
                List<IUserNotification> error = pressureNotifs?.Where(x => x.Level == Haestad.Support.User.NotificationLevel.Error).ToList();
                if (error != null && error.Any())
                {
                    result.IsCalculationFailure = true;
                    result.ErrorNotifs = mapper.Map<List<IUserNotification>, List<UserNotification>>(error);
                }

                result.EpsNodeResult = GetEpsTimeNodePointResult(wm);
                result.EpsPipeResult = GetEpsTimePointPipeResult(wm);
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
        public static WengAnEpsResult FireDemandAtOneNode(FireDemandArg arg)
        {
            WaterGEMSModel wm = new WaterGEMSModel();
            wm.ProductId = ProductId.Bentley_WaterGEMS;
            var result = new WengAnEpsResult();

            try
            {
                wm.OpenDataSource(arg.ModelPath);
                wm.SetActiveScenario(3973);

                wm.PressureCalculationOption.SetPressureEngineCalculationType(EpaNetEngine_CalculationTypeEnum.SCADAAnalaysisType);
                wm.PressureCalculationOption.SetSCADACalculationType(SCADACalculationTypeEnum.HydraulicsOnly);

                wm.PressureCalculationOption.ClearSCADAFireDemand();
                wm.PressureCalculationOption.AddSCADAFireDemand(arg.NodeId, arg.DemandInLitersPerSecond, arg.StartTime, arg.DurationHours);

                IUserNotification[] pressureNotifs = wm.RunPressureCalculation();
                List<IUserNotification> error = pressureNotifs?.Where(x => x.Level == Haestad.Support.User.NotificationLevel.Error).ToList();
                if (error != null && error.Any())
                {
                    result.IsCalculationFailure = true;
                    result.ErrorNotifs = mapper.Map<List<IUserNotification>, List<UserNotification>>(error);
                }

                result.EpsNodeResult = GetEpsTimeNodePointResult(wm);
                result.EpsPipeResult = GetEpsTimePointPipeResult(wm);
                return result;
            }
            finally
            {
                wm.CloseDataSource();
            }

        }
        //水龄不用传参数
        public static WaterQualityResult WaterAge(string modelPath)
        {
            WaterGEMSModel wm = new WaterGEMSModel();
            wm.ProductId = ProductId.Bentley_WaterGEMS;
            var result = new Models.WaterQualityResult();
            try
            {
                wm.OpenDataSource(modelPath);
                wm.SetActiveScenario(4015);
                wm.RunWTmodel();     //run the wtrg model that includes WQ calculations 
                WaterQualityCalculation wqc = new WaterQualityCalculation(wm);

                IUserNotification[] pressureNotifs = wm.RunPressureCalculation();
                List<IUserNotification> error = pressureNotifs?.Where(x => x.Level == Haestad.Support.User.NotificationLevel.Error).ToList();
                if (error != null && error.Any())
                {
                    result.IsCalculationFailure = true;
                    result.ErrorNotifs = mapper.Map<List<IUserNotification>, List<UserNotification>>(error);
                }

                HmIDCollection allNodesIds = wm.DomainDataSet
                    .DomainElementManager((int)DomainElementType.BaseIdahoNodeElementManager).ElementIDs();
                Dictionary<int, double[]> nodeResult = new Dictionary<int, double[]>();
                foreach (var id in allNodesIds)
                {
                    double[] ages = wqc.GetAgeInHours(id);
                    nodeResult.Add(id, ages);
                }

                result.NodeResult = nodeResult;

                Dictionary<int, double[]> pipeResult = new Dictionary<int, double[]>();

                HmIDCollection allPipeIds = wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoPipeElementManager)
                    .ElementIDs();
                foreach (var id in allPipeIds)
                {
                    double[] ages = wqc.GetAgeInHours(id);
                    pipeResult.Add(id, ages);
                }
                result.PipeResult = pipeResult;
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
        public static Models.WaterQualityResult Concentration(WaterConcentrationArg arg)
        {
            WaterGEMSModel wm = new WaterGEMSModel();
            wm.ProductId = ProductId.Bentley_WaterGEMS;
            var result = new Models.WaterQualityResult();
            try
            {
                wm.OpenDataSource(arg.ModelPath);
                wm.SetActiveScenario(4016);
                wm.RunWTmodel();     //run the wtrg model that includes WQ calculations 
                WaterQualityCalculation wqc = new WaterQualityCalculation(wm);

                IUserNotification[] pressureNotifs = wm.RunPressureCalculation();
                List<IUserNotification> error = pressureNotifs?.Where(x => x.Level == Haestad.Support.User.NotificationLevel.Error).ToList();
                if (error != null && error.Any())
                {
                    result.IsCalculationFailure = true;
                    result.ErrorNotifs = mapper.Map<List<IUserNotification>, List<UserNotification>>(error);
                }

                HmIDCollection allNodesIds = wm.DomainDataSet
                    .DomainElementManager((int)DomainElementType.BaseIdahoNodeElementManager).ElementIDs();
                Dictionary<int, double[]> nodeResult = new Dictionary<int, double[]>();
                foreach (var id in allNodesIds)
                {
                    double[] ages = wqc.GetConcentrationInMGL(id);
                    nodeResult.Add(id, ages);
                }

                result.NodeResult = nodeResult;

                Dictionary<int, double[]> pipeResult = new Dictionary<int, double[]>();

                HmIDCollection allPipeIds = wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoPipeElementManager)
                    .ElementIDs();
                foreach (var id in allPipeIds)
                {
                    double[] ages = wqc.GetConcentrationInMGL(id);
                    pipeResult.Add(id, ages);
                }
                result.PipeResult = pipeResult;
                return result;
            }
            finally
            {
                wm.CloseDataSource();
            }

        }

        /// <summary>
        ///     瓮安爆管影响分析
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static BreakPipeResult BreakPipe(BreakPipeArg arg)
        {
            WaterGEMSModel wm = new WaterGEMSModel();
            var result = new BreakPipeResult();
            var valveInitialDict = new Dictionary<int, ValveSettingEnum>();
            var isolationValveInitialDict = new Dictionary<int, IsolationValveInitialSettingEnum>();
            try
            {
                wm.OpenDataSource(arg.ModelPath);
                IDomainDataSet dataSet = wm.DomainDataSet;
                wm.SetActiveScenario(3973);
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
                new NetworkIsolation(dataSet).GetElementsOfIsolatingPipeBreak(arg.PipeId, arg.BreakPointDistanceToStartNode,
                    new HmIDCollection(arg.ValvesToExclude), out var valvesToClose, out var pipesToClose,
                    out var isolationValvesToClose, out var isolatedPartialPipeIds, out var isolatedPipeIds,
                    out var isolatedNodeIds, out var isolatedCustomerIds, out var outagePartialPipeIds,
                    out var outagePipeIds, out var outageNodeIds, out var outageCustomerIds);


                result.ValvesToClose = valvesToClose.ToArray(); //important
                //result.PipesToClose = pipesToClose.ToArray();
                result.IsolationValvesToClose = isolationValvesToClose.ToArray(); //important
                //result.IsolatedPartialPipeIds = isolatedPartialPipeIds;
                result.IsolatedPipeIds = isolatedPipeIds.ToArray();
                result.IsolatedNodeIds = isolatedNodeIds.ToArray();
                result.IsolatedCustomerIds = isolatedCustomerIds.ToArray();
                //result.OutagePartialPipeIds = outagePartialPipeIds;
                //result.OutagePipeIds = outagePipeIds.ToArray();
                //result.OutageNodeIds = outageNodeIds.ToArray();
                //result.OutageCustomerIds = outageCustomerIds.ToArray();

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
                var now = DateTime.Now;
                wm.PressureCalculationOption.SetPressureEngineSimulationStartDate(now);
                wm.PressureCalculationOption.SetPressureEngineSimulationStartTime(now);
                IUserNotification[] notif = wm.RunPressureCalculation();
                List<IUserNotification> error = notif?.Where(x => x.Level == Haestad.Support.User.NotificationLevel.Error).ToList();
                if (error != null && error.Any())
                {
                    result.IsCalculationFailure = true;
                    result.ErrorNotifs = mapper.Map<List<IUserNotification>, List<UserNotification>>(error);
                }
                #region 比较节点水压

                IDomainElementManager junctionManager =
                    wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoJunctionElementManager);
                ModelingElementCollection allJunctions = junctionManager.Elements();
                var pressureDict = new Dictionary<int, double[]>();
                foreach (var junction in allJunctions)
                {
                    pressureDict.Add(junction.Id, wm.PressureResult.GetNodePressureInKiloPascals(junction.Id));
                }

                result.NodePressures = pressureDict;
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
        /// <param name="modelpath">模型路径</param>
        /// <returns></returns>
        public static WaterTraceResult GetWaterTraceResultsForMultipleElementIds(string modelpath)
        {
            var wm = new WaterGEMSModel();
            var result = new WaterTraceResult();
            try
            {
                wm.OpenDataSource(modelpath, false);
                wm.SetActiveScenario(4017);

                IDomainDataSet dataSet = wm.DomainDataSet;
                License lc = wm.License;

                WaterTraceCalculation wt = new WaterTraceCalculation(wm);
                IList<int> traceElementIds = new List<int>(3);
                traceElementIds.Add(2949);
                traceElementIds.Add(2957);
                traceElementIds.Add(2961);
                IUserNotification[] notif = wt.RunTraceCalculationForMultipleTraceElements(traceElementIds);    // reservoir
                GetEngineFatalErrorException(notif, result);
                IDomainElementManager junctionManager = wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoJunctionElementManager);
                ModelingElementCollection allJunctions = junctionManager.Elements();
                IList<int> elementIds = new List<int>();
                foreach (var junction in allJunctions)
                {
                    elementIds.Add(junction.Id);
                }
                double[] timeSteps;
                IList<double[]> traceResults1 = wt.GetTraceResultsFromOneTraceSourceElementForMultipleElementIdInPercent(2949, elementIds, out timeSteps);
                IList<double[]> traceResults2 = wt.GetTraceResultsFromOneTraceSourceElementForMultipleElementIdInPercent(2957, elementIds, out timeSteps);
                IList<double[]> traceResults3 = wt.GetTraceResultsFromOneTraceSourceElementForMultipleElementIdInPercent(2961, elementIds, out timeSteps);
                var list = new List<WaterTracePercentage>();
                for (int i = 0; i < elementIds.Count; i++)
                {
                    var r = new WaterTracePercentage();
                    r.Id = elementIds[i];
                    r.Source1Percentage = traceResults1[i];
                    r.Source2Percentage = traceResults2[i];
                    r.Source3Percentage = traceResults3[i];
                    r.TimeStep = timeSteps;
                    list.Add(r);
                }

                result.TracePercentageResults = list;
                return result;

            }
            finally
            {
                wm.CloseDataSource();
            }
        }

        private static void GetEngineFatalErrorException(IUserNotification[] notif, WaterEngineResultBase result)
        {
            List<IUserNotification> error = notif?.Where(x => x.Level == Haestad.Support.User.NotificationLevel.Error).ToList();
            if (error != null && error.Any())
            {
                result.IsCalculationFailure = true;
                result.ErrorNotifs = mapper.Map<List<IUserNotification>, List<UserNotification>>(error);
            }
        }

        /// <summary>
        /// 返回运行watergems的节点，管道结果
        /// </summary>
        /// <param name="wm"></param>
        /// <returns></returns>
        private static List<EpsNodeResult> GetEpsTimeNodePointResult(WaterGEMSModel wm)
        {
            double[] timeSteps = wm.PressureResult.GetPressureEngineCalculationTimeStepsInSeconds(); //读取EPS报告点动态结果

            var timePointNodeResults = new List<EpsNodeResult>();
            HmIDCollection allNodesIds = wm.DomainDataSet
                .DomainElementManager((int)DomainElementType.BaseIdahoNodeElementManager).ElementIDs();
            foreach (var id in allNodesIds)
            {
                var epsResult = new EpsNodeResult();
                epsResult.Id = id;
                epsResult.Label = wm.GetDomainElementLabel(id);
                epsResult.TimeSteps = timeSteps;
                epsResult.Pressures = wm.PressureResult.GetNodePressureInKiloPascals(id);
                //epsResult.HGL = wm.PressureResult.GetNodeHGLInMeters(id);
                timePointNodeResults.Add(epsResult);
            }

            return timePointNodeResults;
        }

        private static List<EpsPipeResult> GetEpsTimePointPipeResult(WaterGEMSModel wm)
        {
            double[] timeSteps = wm.PressureResult.GetPressureEngineCalculationTimeStepsInSeconds(); //读取EPS报告点动态结果
            var timePointPipeResults = new List<EpsPipeResult>();
            HmIDCollection allPipeIds = wm.DomainDataSet.DomainElementManager((int)DomainElementType.IdahoPipeElementManager)
                .ElementIDs();
            foreach (var id in allPipeIds)
            {
                var epsResult = new EpsPipeResult();
                epsResult.Id = id;
                epsResult.Label = wm.GetDomainElementLabel(id);
                epsResult.TimeSteps = timeSteps;
                epsResult.Flows = wm.PressureResult.GetPipeFlowInCubicMetersPerSecond(id);
                epsResult.Velocities = wm.PressureResult.GetPipeVelocityInMetersPerSecond(id);
                //epsResult.PipeHeadLoss = wm.PressureResult.GetPipeHeadlossInMeters(id);
                epsResult.PipeHeadlossGradient = wm.PressureResult.GetPipeUnitHeadlossInMeterPerKM(id);
                timePointPipeResults.Add(epsResult);
            }
            return timePointPipeResults;

        }
    }


}
