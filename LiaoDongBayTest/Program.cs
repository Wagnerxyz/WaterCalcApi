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
            //string input = File.ReadAllText(@"d:\dhi.json");
            //var client = new HttpClient();
            //var contentData = new StringContent(input, Encoding.UTF8, "application/json");
            //client.Timeout = TimeSpan.FromMinutes(30);
            //var response = client.PostAsync("http://40.117.47.87/BentleyAPI/api/qingdao/FlushWater", contentData).Result;
            //var result = WengAnApi.RunEPS(wenganModel);
            //var arg = new BreakPipeArg(){ModelPath = wenganModel,PipeId = 2887,};
            //WengAnApi.BreakPipe();

            var fireDemandArg = new FireDemandArg()
            { ModelPath = wenganModel, DemandInLitersPerSecond = 50, DurationHours = 2, NodeId = 1329, StartTime = new DateTime(2021, 7, 5, 0, 0, 0) };
            File.WriteAllText(@"firedemandarg.json", JsonConvert.SerializeObject(fireDemandArg));
            var rrr = WengAnApi.FireDemandAtOneNode(fireDemandArg);
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
                ModelPath = ldModel,
                CurrentNodePressures = LiaoDongDummyData.GetLiaoDongNodePressure(),
                CurrentPipeFlows = LiaoDongDummyData.GetLiaoDongPipeFlow()
            };
            File.WriteAllText(@"liaodonginput.json", JsonConvert.SerializeObject(arg));
            //TestWaterLeakByFindingEmitterCoefficient();
            // TestSettingObservedDataAndRunWaterLeakCalibration();

            LiaoDongApi.SettingObservedDataAndRunWaterLeakCalibration(arg);
        }



        private static void WengAnBreakPipe()
        {
            var modelPath = @"D:\P4V\Aspen\Components\Haestad.Calculations.Shanghai\Development\Haestad.Calculations.Shanghai.WaterGEMS.Test\MDBs\PipeBreakIsolationModel.wtg.sqlite";
            var arg = new BreakPipeArg() { ModelPath = modelPath, PipeId = 44, ValvesToExclude = new[] { 48 }, BreakPointDistanceToStartNode = 1 };

            var result = WengAnApi.BreakPipe(arg);
        }

    }
}
