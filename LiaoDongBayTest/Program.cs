using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Haestad.Calculations.Shanghai.WaterGEMS;
using Haestad.Domain;
using Haestad.LicensingFacade;
using Haestad.Support.User;
using LiaoDongBay;
using Models;
using Newtonsoft.Json;
using WengAn.Args;

namespace LiaoDongBayTest
{
    class Program
    {
        const string ldModel = @"D:\BentleyModels\LiaoDong\LiaoDongBay_20210716.wtg.sqlite";
        const string wenganModel = @"D:\BentleyModels\WengAn\WengAn0916.wtg.sqlite";

        static void Main(string[] args)
        {
            MapperConfiguration mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IUserNotification, UserNotification>();
                //cfg.AddProfile();
            });
            WengAnApi.mapper = mapConfig.CreateMapper();

            var epsArg = DummyTestData.DummyRunEPSArg();

            WengAnApi.RunEPS(epsArg);

            var aaa = DummyTestData.DummyFireArg();
            var aqweq = WengAnApi.FireDemandAtOneNode(aaa);
            var qwqeq = aqweq.EpsNodeResult.Where(x => x.Id == 1338);
            File.WriteAllText(@"fire baseResult.json", JsonConvert.SerializeObject(qwqeq));
            
            WengAnBreakPipe();

            //string input = File.ReadAllText(@"d:\dhi.json");
            //var client = new HttpClient();
            //var contentData = new StringContent(input, Encoding.UTF8, "application/json");
            //client.Timeout = TimeSpan.FromMinutes(30);
            //var response = client.PostAsync("http://40.117.47.87/BentleyAPI/api/qingdao/FlushWater", contentData).TracePercentageResults;
        
            var fireDemandArg = new FireDemandArg()
            { ModelPath = wenganModel, DemandInLitersPerSecond = 50, DurationHours = 2, NodeId = 1329, StartTime = new DateTime(2021, 7, 5, 0, 0, 0) };
            File.WriteAllText(@"firedemandarg.json", JsonConvert.SerializeObject(fireDemandArg));
            var rrr = WengAnApi.FireDemandAtOneNode(fireDemandArg);
            var result = WengAnApi.GetWaterTraceResultsForMultipleElementIds(wenganModel);

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
            var arg = DummyTestData.DummyBreakPipeArg();
            var result = WengAnApi.BreakPipe(arg);
        }

        private static void WengAnBreakPipe()
        {
            var arg = DummyTestData.DummyBreakPipeArg();
            var xxx = JsonConvert.SerializeObject(arg);
            var result = WengAnApi.BreakPipe(arg);
        }


    }
}
