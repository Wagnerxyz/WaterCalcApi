using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ChinaWaterLib;
using ChinaWaterLib.WengAn;
using Haestad.Calculations.Shanghai.QingDaoWT;
using Haestad.Calculations.Shanghai.WaterGEMS;
using Haestad.Domain;
using Haestad.LicensingFacade;
using Haestad.Support.User;
using LiaoDongBay;
using Models;
using Newtonsoft.Json;
using WengAn.Args;

namespace ChinaTest
{
    class Program
    {
        const string ldModel = @"D:\BentleyModels\LiaoDong\LiaoDongBay_20210716.wtg.sqlite";
        const string wenganModel = @"D:\BentleyModels\WengAn\WengAn1109.wtg.sqlite";

        static void Main(string[] args)
        {

            string demoModelPath = @"D:\DemoModel\demo\无标题 1.wtg.sqlite";
            var wm = new WaterGEMSModel();
            wm.OpenDataSource(demoModelPath, true);


            WaterUtils.GetAllDemandPattern(wm.DomainDataSet, out List<PatternHydraulic> patternHydraulicList);

            MapperConfiguration mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IUserNotification, UserNotification>();
                //cfg.AddProfile();
            });
            WengAnApi.mapper = mapConfig.CreateMapper();

            WengAnRunEps();
            //  Parallel.For(0, 12, (x) => ForParallelWengAnRunEps());

            DemandForecast();



            var fireArg = WengAnDummyData.DummyFireArg();
            var aqweq = WengAnApi.FireDemandAtOneNode(fireArg);
            var qwqeq = aqweq.EpsNodeResult.Where(x => x.Id == 1338);
            File.WriteAllText(@"fire baseResult.json", JsonConvert.SerializeObject(qwqeq));

            WengAnBreakPipe();
            var result = WengAnApi.GetWaterTraceResultsForMultipleElementIds(wenganModel);

            //string input = File.ReadAllText(@"d:\dhi.json");
            //var client = new HttpClient();
            //var contentData = new StringContent(input, Encoding.UTF8, "application/json");
            //client.Timeout = TimeSpan.FromMinutes(30);
            //var response = client.PostAsync("http://40.117.47.87/BentleyAPI/api/qingdao/FlushWater", contentData).TracePercentageResults;
            //if (response.IsSuccessStatusCode)
            //{
            //    var stringData = response.Content.ReadAsStringAsync().TracePercentageResults;
            //    var baseResult = JsonConvert.DeserializeObject<LiaoDongResult>(stringData);
            //}

            TestLiaoDong();

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
            WengAnApi.RunEPS(epsArg);
        }
        private static void ForParallelWengAnRunEps()
        {
            var epsArg = WengAnDummyData.DummyRunEPSArg();
            WengAnApi.RunEPSP(epsArg);
        }
        private static void DemandForecast()
        {
            //每半个小时 CALL 预报更新 API, 这个是 CALL 的日期与时间：
            DateTime runStartDT = new DateTime(2021, 10, 12, 14, 0, 0);

            //这个是 DATA OBJECT
            ObsDate aData;


            //这个是查尔岩，西坡，剩土三个水厂的流量观察数据，是当前时间前面24小时的数据
            List<ObsDate> cryData = new List<ObsDate>();
            List<ObsDate> xipoData = new List<ObsDate>();
            List<ObsDate> shengtuData = new List<ObsDate>();

            //数据案例

            //     7/19/2021 10:31 419
            //     7/19/2021 10:41 523
            //     7/19/2021 10:52 443
            //     7/19/2021 11:02 445
            //     7/19/2021 11:13 438

            aData = new ObsDate(new DateTime(2021, 07, 19, 10, 31, 0), 419.0);
            cryData.Add(aData);
            xipoData.Add(aData); // 这个根据西坡的实际数据来加
            shengtuData.Add(aData); // 这个根据剩土实际数据来加
            aData = new ObsDate(new DateTime(2021, 07, 19, 10, 41, 0), 523.0);
            cryData.Add(aData);
            xipoData.Add(aData);
            shengtuData.Add(aData);
            aData = new ObsDate(new DateTime(2021, 07, 19, 10, 52, 0), 443.0);
            cryData.Add(aData);
            xipoData.Add(aData);
            shengtuData.Add(aData);
            aData = new ObsDate(new DateTime(2021, 07, 19, 11, 02, 0), 445.0);
            cryData.Add(aData);
            xipoData.Add(aData);
            shengtuData.Add(aData);
            aData = new ObsDate(new DateTime(2021, 07, 19, 11, 13, 0), 438.0);
            cryData.Add(aData);
            xipoData.Add(aData);
            shengtuData.Add(aData);

            var arg1 = new ForecastDemandArg()
            {
                ModelPath = wenganModel,
                shengtuData = shengtuData,
                cryData = cryData,
                xipoData = xipoData,
                DateTime = runStartDT
            };
            string s = JsonConvert.SerializeObject(arg1);
            WengAnDemandForecast.Run(arg1);

        }

        private static void TestLiaoDong()
        {
            var arg = new NodeEmitterCoefficientArg()
            {
                ModelPath = ldModel,
                CurrentNodePressures = LiaoDongDummyData.GetLiaoDongNodePressure(),
                CurrentPipeFlows = LiaoDongDummyData.GetLiaoDongPipeFlow()
            };
            File.WriteAllText(@"liaodonginput.json", JsonConvert.SerializeObject(arg));
            //TestWaterLeakByFindingEmitterCoefficient();
            // TestSettingObservedDataAndRunWaterLeakCalibration();

            LiaoDongApi.SettingObservedDataAndRunWaterLeakCalibration(arg);
        }

        private static void Fire()
        {
            var arg = WengAnDummyData.DummyBreakPipeArg();
            var result = WengAnApi.BreakPipe(arg);
        }

        private static void WengAnBreakPipe()
        {
            var arg = WengAnDummyData.DummyBreakPipeArg();
            var xxx = JsonConvert.SerializeObject(arg);
            var result = WengAnApi.BreakPipe(arg);
        }


    }
}
