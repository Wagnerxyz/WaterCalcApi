using AutoMapper;
using ChinaWaterLib;
using ChinaWaterLib.Models;
using Haestad.Calculations.Shanghai.DataCleaner;
using Haestad.Calculations.Shanghai.WaterGEMS;
using Haestad.Support.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using ChinaWaterModel;
using Haestad.Domain;
using Haestad.Support.Support;
using WengAn;
using WengAn.Args;

namespace ChinaTest
{
    class Program
    {
        const bool isServerModeLicense = false;
        const bool workOnCopiedModel = true;
        static void Main(string[] args)
        {
            ProfileOptimization.SetProfileRoot(@"C:\files\");
            ProfileOptimization.StartProfile("wagner.fff");
            MapperConfiguration mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IUserNotification, UserNotification>();
                //cfg.AddProfile();
            });
            Consts.Mapper = mapConfig.CreateMapper();

            var wm = new WaterGEMSModel();
            //上一步已经复制了，不要复制 用原来的改

            wm.OpenDataSource(@"C:\BentleyModels\tianjin\TianjinModel_Trace_20231222.wtg.sqlite", true);
            //wm.OpenDataSource(@"C:\BentleyModels\WengAn\WengAn20230412.wtg.sqlite", true);
            
            wm.PressureCalculationOption.AddActiveDemandAdjustmentMultiplierForAllNetwork(1.3);
            wm.CloseDataSource();

            wm.OpenDataSource(@"C:\Temp\Qingdao.wtg.sqlite", true);


            var ids1 = wm.DomainDataSet.DomainElementManager((int)DomainElementType.BaseValveElementManager).ElementIDs();

            foreach (var valveId in ids1)
            {
                int valveType = wm.DomainDataSet.DomainElementTypeID(valveId);

                IField relativeClosurePatternField = wm.DomainDataSet.FieldManager.DomainElementField(StandardFieldName.Hammer_OperatingRule, (int)AlternativeType.HammerAlternative, valveType);

                ((IEditField)relativeClosurePatternField).SetValue(valveId, DBNull.Value);
            }
            wm.CloseDataSource();


            WengAnRunEps();
            var result = WengAnHandler.GetWaterTraceResultsForMultipleElementIds(WengAnDummyData.DummyWaterTraceArg(), 4014, 72, true, isServerModeLicense, workOnCopiedModel, 20);

            string aaa = null;

            WengAnDummyData.wenganModel = Consts.WenganDefaultModel;
            string demoModelPath = @"D:\DemoModel\demo\无标题 1.wtg.sqlite";
            wm.OpenDataSource(demoModelPath, true);


            WaterUtils.GetAllDemandPattern(wm.DomainDataSet, out List<PatternHydraulic> patternHydraulicList);



            //  Parallel.For(0, 12, (x) => ForParallelWengAnRunEps());

            DemandForecast();


            WengAnBreakPipe();
            var arg = WengAnDummyData.FillDummyBaseArg(new WengAnBaseArg());

            //string input = File.ReadAllText(@"d:\dhi.json");
            //var client = new HttpClient();
            //var contentData = new StringContent(input, Encoding.UTF8, "application/json");
            //client.Timeout = TimeSpan.FromMinutes(30);
            //var response = client.PostAsync("http://40.117.47.87/BentleyAPI/api/qingdao/FlushWater", contentData).WaterHeadTracePercentageResults;
            //if (response.IsSuccessStatusCode)
            //{
            //    var stringData = response.Content.ReadAsStringAsync().WaterHeadTracePercentageResults;
            //    var baseResult = JsonConvert.DeserializeObject<LiaoDongResult>(stringData);
            //}


            //  WengAnWaterQuality();
            WengAnBreakPipe();

            Debugger.Break();
            //baseResult.Add(95, wqc.GetAgeInHours(95));       //age at J-26
            //baseResult.Add(138, wqc.GetAgeInHours(138));       //age at P-66
            //baseResult.Add(90, wqc.GetConcentrationInMGL(90));        //concentration at J-10
            //double[] cons = wqc.GetConcentrationInMGL(90);       //concentration at J-10
            //double[] ages = wqc.GetAgeInHours(138);       //age at P-66
        }

        private static void WengAnRunEps()
        {
            var epsArg = WengAnDummyData.DummyRunEPSArg();
            var json = JsonConvert.SerializeObject(epsArg);
            WengAnHandler.RunEPS(epsArg, 20, 24, true, isServerModeLicense, workOnCopiedModel, 20);
        }

        //不需要这个接口了
        private static void DemandForecast()
        {
            //每半个小时 CALL 预报更新 API, 这个是 CALL 的日期与时间：
            DateTime runStartDT = new DateTime(2021, 10, 12, 14, 0, 0);

            //这个是 DATA OBJECT
            TimeSeries aData;


            //这个是查尔岩，西坡，剩土三个水厂的流量观察数据，是当前时间前面24小时的数据
            List<TimeSeries> cryData = new List<TimeSeries>();
            List<TimeSeries> xipoData = new List<TimeSeries>();
            List<TimeSeries> shengtuData = new List<TimeSeries>();

            //数据案例

            //     7/19/2021 10:31 419
            //     7/19/2021 10:41 523
            //     7/19/2021 10:52 443
            //     7/19/2021 11:02 445
            //     7/19/2021 11:13 438

            aData = new TimeSeries(new DateTime(2021, 07, 19, 10, 31, 0), 419.0);
            cryData.Add(aData);
            xipoData.Add(aData); // 这个根据西坡的实际数据来加
            shengtuData.Add(aData); // 这个根据剩土实际数据来加
            aData = new TimeSeries(new DateTime(2021, 07, 19, 10, 41, 0), 523.0);
            cryData.Add(aData);
            xipoData.Add(aData);
            shengtuData.Add(aData);
            aData = new TimeSeries(new DateTime(2021, 07, 19, 10, 52, 0), 443.0);
            cryData.Add(aData);
            xipoData.Add(aData);
            shengtuData.Add(aData);
            aData = new TimeSeries(new DateTime(2021, 07, 19, 11, 02, 0), 445.0);
            cryData.Add(aData);
            xipoData.Add(aData);
            shengtuData.Add(aData);
            aData = new TimeSeries(new DateTime(2021, 07, 19, 11, 13, 0), 438.0);
            cryData.Add(aData);
            xipoData.Add(aData);
            shengtuData.Add(aData);

            var arg1 = new ForecastDemandArg()
            {
                ModelPath = Consts.WenganDefaultModel,
                shengtuData = shengtuData,
                cryData = cryData,
                xipoData = xipoData,
                DateTime = runStartDT
            };
            string s = JsonConvert.SerializeObject(arg1);
            //   WengAnDemandForecast.Run(arg1); 参数调整不需要这个接口了

        }


        private static void Fire()
        {
            var fireArg = WengAnDummyData.DummyFireArg();
            var aqweq = WengAnHandler.FireDemandAtOneNode(fireArg, 3972, 24, true, isServerModeLicense, workOnCopiedModel, 20);
            var qwqeq = aqweq.EpsNodeResult.Where(x => x.Id == 1338);
            File.WriteAllText(@"fire baseResult.json", JsonConvert.SerializeObject(qwqeq));
        }

        private static void WengAnBreakPipe()
        {
            var arg = WengAnDummyData.DummyBreakPipeArg();
            var xxx = JsonConvert.SerializeObject(arg);
            var result = WengAnHandler.BreakPipe(arg, 3972, 24, true, isServerModeLicense, workOnCopiedModel, 20);
        }


    }
}
