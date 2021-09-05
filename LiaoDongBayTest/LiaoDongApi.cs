using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haestad.Calculations.Shanghai.WaterGEMS;
using Haestad.Calculations.Support;
using Haestad.Domain;
using Haestad.LicensingFacade;
using Haestad.Network.Segmentation;
using Haestad.Support.Library;
using Haestad.Support.Support;
using Haestad.Support.Units;
using Haestad.Support.User;

namespace LiaoDongBayTest
{
    public class WaterQualityArgs
    {
        /// <summary>
        ///     模型Sqlite文件路径
        /// </summary>
        public string ModelPath { get; set; }

        /// <summary>
        ///     传入初始Node,Reservior,Tank 水龄
        /// </summary>
        public Dictionary<int, double> InitialValues { get; set; }

        /// <summary>
        ///     要返回水龄的Id
        /// </summary>
        public int[] AgeIds { get; set; }

        /// <summary>
        ///     要返回水质的Id
        /// </summary>
        public int[] ConcentrationIds { get; set; }
    }

    public class WaterQualityResult
    {
        public Dictionary<int, double[]> AgesValues { get; set; } = new Dictionary<int, double[]>();
        public Dictionary<int, double[]> ConcentrationValues { get; set; } = new Dictionary<int, double[]>();
    }

    public class LiaoDongApi
    {
     
        //public void GetAllNodesAndPipesEpsResult(WaterGEMSModel wm)
        //{
        //    double[] epsSteps = wm.ResultDataConnection.TimeStepsInSeconds(wm.DomainDataSet.ScenarioManager.ActiveScenarioID); ;//读取EPS报告点动态结果

        //    var timePointNodeResults = new List<Models.DemoNodeResult>();
        //    HmIDCollection allNodesIds = DomainDataSet
        //        .DomainElementManager((int)DomainElementType.BaseIdahoNodeElementManager).ElementIDs();
        //    foreach (var id in allNodesIds)
        //    {
        //        for (var index = 0; index < epsSteps.Length; index++)
        //        {
        //            var epsResult = new Models.DemoNodeResult();
        //            epsResult.Id = id;
        //            epsResult.Label = GetDomainElementLabel(id);
        //            epsResult.TimeSteps = epsSteps[index];
        //            epsResult.HGL = GetNodeHGLInMeters(id)[index];
        //            timePointNodeResults.Add(epsResult);
        //        }
        //    }

        //    var timePointFlowResults = new List<Models.DemoPipeResult>();
        //    HmIDCollection allPipeIds = DomainDataSet.DomainElementManager((int)DomainElementType.IdahoPipeElementManager)
        //        .ElementIDs();
        //    foreach (var id in allPipeIds)
        //    {
        //        for (var index = 0; index < epsSteps.Length; index++)
        //        {
        //            var epsResult = new Models.DemoPipeResult();
        //            epsResult.Id = id;
        //            epsResult.Label = GetDomainElementLabel(id);
        //            epsResult.TimeSteps = epsSteps[index];
        //            epsResult.Flows = GetPipeFlowInCubicMetersPerSecond(id)[index];
        //            epsResult.Velocities = GetPipeVelocityInMetersPerSecond(id)[index];
        //            epsResult.PipeHeadLoss = GetPipeHeadLossMetersOfH2O(id)[index];
        //            timePointFlowResults.Add(epsResult);
        //        }
        //    }
        //}

        /// <summary>
        ///     瓮安水龄水质
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static WaterQualityResult WaterAgeQuality(WaterQualityArgs arg)
        {
            WaterGEMSModel wm = new WaterGEMSModel();
            wm.ProductId = ProductId.Bentley_WaterGEMS;
            var result = new WaterQualityResult();

            try
            {
                wm.OpenDataSource(arg.ModelPath);
                //IDomainDataSet dataSet = wm.DomainDataSet;
                //License lc = wm.License;

                wm.RunWTmodel(); //run the wtrg model that includes WQ calculations 
                WaterQualityCalculation wqc = new WaterQualityCalculation(wm);
              

                foreach (var id in arg.AgeIds)
                {
                    result.AgesValues.Add(id, wqc.GetAgeInHours(id));
                }

                foreach (var id in arg.ConcentrationIds)
                {
                    result.ConcentrationValues.Add(id, wqc.GetConcentrationInMGL(id));
                }

                //result.Add(95, wqc.GetAgeInHours(95));       //age at J-26
                //result.Add(138, wqc.GetAgeInHours(138));       //age at P-66
                //result.Add(90, wqc.GetConcentrationInMGL(90));        //concentration at J-10
                //double[] cons = wqc.GetConcentrationInMGL(90);       //concentration at J-10
                //double[] ages = wqc.GetAgeInHours(138);       //age at P-66


                //Assert.IsTrue(ages.Length == 280);
                //Assert.AreEqual(0.7337, ages[1], 0.001);
                //Assert.AreEqual(0.8241, ages[2], 0.001);
                //Assert.AreEqual(0.8259, ages[3], 0.001);

                //Assert.AreEqual(0.5170, ages[143], 0.001);
                //Assert.AreEqual(0.6325, ages[144], 0.001);
            }
            finally
            {
                wm.CloseDataSource();
            }

            return result;
        }

 

        public static bool CheckBalance(LiaoDongArg arg)
        {
            var flows = arg.CurrentPipeFlows;
            var sum1 = flows[317] + flows[305] + flows[253] + flows[157] + flows[539] + flows[337] + flows[531];
            var result1 = (sum1 - flows[366]) < (sum1 * 0.05);

            var result2 = (flows[480] - flows[305]) < (flows[480] * 0.05);

            return (result1 && result2);

        }
        public static Dictionary<int, double> SettingObservedDataAndRunWaterLeakCalibration(LiaoDongArg arg)
        {
            //m_fileName = "Leak-DarwinEmptyObservedData.wtg.sqlite";

            //CopyModelFile();

            string fileName = arg.ModelPath;
            WaterGEMSModel wm = new WaterGEMSModel();
            wm.ProductId = ProductId.Bentley_WaterGEMS;

            try
            {
                wm.OpenDataSource(fileName);

                IDomainDataSet dataSet = wm.DomainDataSet;
                License lc = wm.License;

                CalibrationCalculation cc = new CalibrationCalculation(dataSet, lc);

                IList<int> studyIds;
                IList<string> studyLabels;
                cc.GetCalibrationStudyIdsAndLabels(out studyIds, out studyLabels);
                //Assert.IsTrue(studyIds.Count == 1);
                //Assert.IsTrue(studyIds[0] == 94);

                IList<int> snapshotIds;
                IList<string> snapshotLabels;
                cc.GetCalibrationStudySnapShotIdsAndLabels(out snapshotIds, out snapshotLabels);
                //Assert.IsTrue(snapshotIds.Count == 1);
                //Assert.IsTrue(snapshotIds[0] == 96);

                IList<int> runIds;
                IList<string> runLabels;
                cc.GetCalibrationRunIdsAndLabels(out runIds, out runLabels);
                //Assert.IsTrue(runIds.Count == 1);
                //Assert.IsTrue(runIds[0] == 104);

                int studyId = studyIds[0];
                cc.GetCalibrationStudySnapShotIdsAndLabelsForStudyId(studyId, out snapshotIds, out snapshotLabels);
                //Assert.IsTrue(snapshotIds.Count == 1);
                //Assert.IsTrue(snapshotIds[0] == 96);

                int snapshotId = snapshotIds[0];

                IList<ObservedPipeFlowValues> pipeFlowValues = new List<ObservedPipeFlowValues>();
                foreach (var pressure in arg.CurrentPipeFlows)
                {
                    pipeFlowValues.Add(new ObservedPipeFlowValues(pressure.Key, pressure.Value));
                }
                //pipeFlowValues.Add(new ObservedPipeFlowValues(44, 100));   // p7

                IList<ObservedNodePressureValues> nodePressureValues = new List<ObservedNodePressureValues>();
                foreach (var flow in arg.CurrentNodePressures)
                {
                    nodePressureValues.Add(new ObservedNodePressureValues(flow.Key, flow.Value));
                }
                //nodeHGLValues.Add(new ObservedNodeHGLValues(47, 81.01));
                //nodeHGLValues.Add(new ObservedNodeHGLValues(62, 80.89));
                //nodeHGLValues.Add(new ObservedNodeHGLValues(69, 77.42));
                //nodeHGLValues.Add(new ObservedNodeHGLValues(73, 74.07));
                //nodeHGLValues.Add(new ObservedNodeHGLValues(77, 67.84));
                //nodeHGLValues.Add(new ObservedNodeHGLValues(83, 80.94));
                //nodeHGLValues.Add(new ObservedNodeHGLValues(60, 80.89));
                //nodeHGLValues.Add(new ObservedNodeHGLValues(81, 80.43));

                cc.SetCalibrationSnapShotData(snapshotId, pipeFlowValues, null, nodePressureValues);

                int runId = runIds[0];
                cc.RunCalibration(runId);

                int numOfSolutions;
                IList<int[]> nodeIds;
                IList<double[]> emitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O;
                cc.GetNodeEmitterCoefficientResults(runId, out numOfSolutions, out nodeIds,
                    out emitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O);
                if (nodeIds == null)
                {
                    throw new Exception("传入的值不合理，达尔文校正器算不出结果");
                }
                //Assert.IsTrue(numOfSolutions == 1);
                //Assert.IsTrue(nodeIds.Count == 1);
                //Assert.IsTrue(nodeIds[0][7] == 31);
                //Assert.AreEqual(4.4, emitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O[0][7], 0.01);
                wm.RunPressureCalculation();
                var dict = new Dictionary<int, double>();
                for (int i = 0; i < nodeIds[0].Length; i++)
                {
                    int id = nodeIds[0][i];

                    if (emitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O[0][i] == 0)
                    {
                        dict.Add(id, emitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O[0][i]);
                    }
                    else
                    {
                        //todo: check null
                        var pressure = wm.PressureResult.GetNodePressureInKiloPascals(id).First();
                        var value = emitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O[0][i] * Math.Pow(pressure, 0.5);
                        dict.Add(id, value);
                    }

                }

                return dict;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
            finally
            {
                wm.CloseDataSource();
            }
        }

    }
}