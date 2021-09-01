using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Haestad.Calculations.Shanghai.WaterGEMS;
using Haestad.Domain;
using Haestad.LicensingFacade;
using Newtonsoft.Json;
using WengAn.Args;

namespace LiaoDongBayTest
{
    class Program
    {
        const string ldModel = @"D:\BentleyModels\WQ-1.wtg.sqlite";
        const string wenganModel = @"D:\BentleyModels\WengAn\WengAn20210813\WengAn20210813.wtg.sqlite";


        static void Main(string[] args)
        {

            //string input = File.ReadAllText(@"d:\dhi.json");
            //var client = new HttpClient();
            //var contentData = new StringContent(input, Encoding.UTF8, "application/json");
            //client.Timeout = TimeSpan.FromMinutes(30);
            //var response = client.PostAsync("http://40.117.47.87/BentleyAPI/api/qingdao/FlushWater", contentData).Result;
            //var result = WengAnApi.RunEPS(wenganModel);
            var result = WengAnApi.GetWaterTraceResultsForMultipleElementIds(wenganModel);
            //if (response.IsSuccessStatusCode)
            //{
            //    var stringData = response.Content.ReadAsStringAsync().Result;
            //    var result = JsonConvert.DeserializeObject<LiaoDongResult>(stringData);
            //}

            TestLiaoDong();


            //  WengAnWaterQuality();
            WengAnBreakPipe();

            Debugger.Break();
            //result.Add(95, wqc.GetAgeInHours(95));       //age at J-26
            //result.Add(138, wqc.GetAgeInHours(138));       //age at P-66
            //result.Add(90, wqc.GetConcentrationInMGL(90));        //concentration at J-10
            //double[] cons = wqc.GetConcentrationInMGL(90);       //concentration at J-10
            //double[] ages = wqc.GetAgeInHours(138);       //age at P-66
        }

        private static void TestLiaoDong()
        {
            var arg = new LiaoDongArg()
            {
                ModelPath = @"D:\BentleyModels\LiaoDong\LiaoDongBay_20210813.wtg.sqlite",
                CurrentNodePressures = DummyData.GetLiaoDongNodePressure(),
                CurrentPipeFlows = DummyData.GetLiaoDongPipeFlow()
            };
            File.WriteAllText(@"liaodonginput.json", JsonConvert.SerializeObject(arg));
            //TestWaterLeakByFindingEmitterCoefficient();
            // TestSettingObservedDataAndRunWaterLeakCalibration();

            LiaoDongApi.SettingObservedDataAndRunWaterLeakCalibration(arg);
        }

        public static void TestSettingObservedDataAndRunWaterLeakCalibration()
        {
            //m_fileName = "Leak-DarwinEmptyObservedData.wtg.sqlite";

            //CopyModelFile();

            string fileName = @"D:\BentleyModels\LiaodongBay_20210730\LiaoDongBay_20210716.wtg.sqlite";
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
                pipeFlowValues.Add(new ObservedPipeFlowValues(44, 100));   // p7

                IList<ObservedNodeHGLValues> nodeHGLValues = new List<ObservedNodeHGLValues>();
                nodeHGLValues.Add(new ObservedNodeHGLValues(47, 81.01));
                nodeHGLValues.Add(new ObservedNodeHGLValues(62, 80.89));
                nodeHGLValues.Add(new ObservedNodeHGLValues(69, 77.42));
                nodeHGLValues.Add(new ObservedNodeHGLValues(73, 74.07));
                nodeHGLValues.Add(new ObservedNodeHGLValues(77, 67.84));
                nodeHGLValues.Add(new ObservedNodeHGLValues(83, 80.94));
                nodeHGLValues.Add(new ObservedNodeHGLValues(60, 80.89));
                nodeHGLValues.Add(new ObservedNodeHGLValues(81, 80.43));

                cc.SetCalibrationSnapShotData(snapshotId, pipeFlowValues, nodeHGLValues, null);

                int runId = runIds[0];
                cc.RunCalibration(runId);

                int numOfSolutions;
                IList<int[]> nodeIds;
                IList<double[]> emitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O;
                cc.GetNodeEmitterCoefficientResults(runId, out numOfSolutions, out nodeIds, out emitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O);
                //Assert.IsTrue(numOfSolutions == 1);
                //Assert.IsTrue(nodeIds.Count == 1);
                //Assert.IsTrue(nodeIds[0][7] == 31);
                //Assert.AreEqual(4.4, emitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O[0][7], 0.01);


            }
            finally
            {
                wm.CloseDataSource();
            }
        }

        public static void TestWaterLeakByFindingEmitterCoefficient()
        {


            string fileName = @"D:\BentleyModels\LiaodongBay_20210730\LiaoDongBay_20210716.wtg.sqlite";
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

                int runId = runIds[0];
                cc.RunCalibration(runId);

                int numOfSolutions;
                IList<int[]> nodeIds;
                IList<double[]> emitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O;
                cc.GetNodeEmitterCoefficientResults(runId, out numOfSolutions, out nodeIds, out emitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O);
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
                var result = new LiaoDongResult();
                result.IsBalanced = false;
                result.NodeEmitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O = dict;
                var asda = JsonConvert.SerializeObject(result);
            }
            finally
            {
                wm.CloseDataSource();
            }
        }

        private static void WengAnBreakPipe()
        {

            var modelPath = @"D:\P4V\Aspen\Components\Haestad.Calculations.Shanghai\Development\Haestad.Calculations.Shanghai.WaterGEMS.Test\MDBs\PipeBreakIsolationModel.wtg.sqlite";
            var arg = new BreakPipeArg() { ModelPath = modelPath, PipeId = 44, ValvesToExclude = new[] { 48 }, BreakPointDistanceToStartNode = 1 };


            var result = WengAnApi.BreakPipe(arg);
        }

        private static void WengAnWaterQuality()
        {
            var arg = new WaterQualityArgs() { ModelPath = ldModel };
            var dict = new Dictionary<int, double>();
            dict.Add(123, 2.222); //T-1
            dict.Add(120, 1.111); //R-1
            dict.Add(105, 1.111); //J-6
            arg.InitialValues = dict;

            arg.AgeIds = new int[] { 95, 138, 130 };
            arg.ConcentrationIds = new[] { 90, 137 };


            var result = LiaoDongApi.WaterAgeQuality(arg);
        }
    }
}
