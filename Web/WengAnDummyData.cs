using ChinaWaterLib.Models;
using Haestad.Calculations.Shanghai.DataCleaner;
using System;
using System.Collections.Generic;
using ChinaWaterLib.Models.WengAn;
using WengAn.Args;

namespace WengAn
{
    //放这里既可以被 web的swaggerexample引用，也可被console test app引用
    public class WengAnDummyData
    {
        public static string wenganModel = @"C:\BentleyModels\WengAn\WengAn1109.wtg.sqlite";
        public static Dictionary<int, double> GetCurrentReservoirElevation()
        {
            var dictTank = new Dictionary<int, double>();
            dictTank.Add(2949, 1128.65);
            dictTank.Add(2957, 1102);
            dictTank.Add(2961, 1103.3);
            return dictTank;

        }
        public static Dictionary<int, double> GetCurrentPumpRelativeSpeed()
        {
            //pump当前 频率 
            var dict = new Dictionary<int, double>();

            dict.Add(2995, 1);
            dict.Add(3005, 1);
            dict.Add(3344, 0.78);
            dict.Add(3248, 0.78);
            dict.Add(3222, 0.748);
            dict.Add(3226, 0.748);
            dict.Add(3235, 1);
            dict.Add(3238, 1);
            dict.Add(3243, 0);
            dict.Add(3251, 0.882);
            dict.Add(3330, 0.882);
            dict.Add(3333, 0);

            return dict;
        }
        public static Dictionary<int, double> GetCurrentPumpPressure()
        {
            //pump当前 频率 
            var dict = new Dictionary<int, double>();

            dict.Add(2995, 70);
            dict.Add(3005, 70);
            dict.Add(3344, 70);
            dict.Add(3248, 70);
            dict.Add(3222, 70);
            dict.Add(3226, 70);
            dict.Add(3235, 70);
            dict.Add(3238, 70);
            dict.Add(3243, 70);
            dict.Add(3251, 70);
            dict.Add(3330, 70);
            dict.Add(3333, 70);

            return dict;
        }

        /// <summary>
        /// 当前 独立的 非泵后 TCV阀门的假数据
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, double> GetCurrentPRVPressure()
        {
            var dict = new Dictionary<int, double>
            {
                {3119, 30.0},
                {3359, 35.0},
                {3362, 35.0},
                {3365, 25.0},
                {5584, 30.0},
                {3982, 67.0}
            };
            return dict;
        }
        public static WengAnBaseArg FillDummyBaseArg(WengAnBaseArg arg)
        {
            arg.ModelPath = wenganModel;
            arg.CurrentPrvPressure = GetCurrentPRVPressure();
            arg.CurrentPumpPressure = GetCurrentPumpPressure();
            arg.CurrentReservoirElevation = GetCurrentReservoirElevation();
            arg.StartTime = new DateTime(2021, 07, 06, 0, 0, 0);

            //var json = File.ReadAllText(@".\TestData\flowsensors.json");
            //List<SensorData> sensorData = JsonConvert.DeserializeObject<List<SensorData>>(json);
            List<SensorData> flowSensors = new List<SensorData>();
            List<TimeSeries> cryData = new List<TimeSeries>();
            //todo:12个点
            cryData.Add(new TimeSeries(DateTime.Parse("2021-07-05T00:03:57"), 0.121666666666667));
            cryData.Add(new TimeSeries(DateTime.Parse("2021-07-05T00:14:28"), 0.0938888888888889));
            cryData.Add(new TimeSeries(DateTime.Parse("2021-07-05T00:24:59"), 0.090833333333333308));
            List<TimeSeries> xipoData = new List<TimeSeries>();
            xipoData.Add(new TimeSeries(DateTime.Parse("2021-07-05T00:03:57"), 0.191388888888889));
            xipoData.Add(new TimeSeries(DateTime.Parse("2021-07-05T00:14:28"), 0.189444444444444));
            xipoData.Add(new TimeSeries(DateTime.Parse("2021-07-05T00:24:59"), 0.188611111111111));
            List<TimeSeries> shengtuData = new List<TimeSeries>();
            shengtuData.Add(new TimeSeries(DateTime.Parse("2021-07-05T00:03:57"), 0.148055555555556));
            shengtuData.Add(new TimeSeries(DateTime.Parse("2021-07-05T00:14:28"), 0.140277777777778));
            shengtuData.Add(new TimeSeries(DateTime.Parse("2021-07-05T00:24:59"), 0.118611111111111));
            int id1 = 2243;
            SensorData cry = new SensorData(id1, cryData);
            flowSensors.Add(cry);

            int id2 = 2965;
            SensorData xipo = new SensorData(id2, xipoData);
            flowSensors.Add(xipo);

            int id3 = 2953;
            SensorData shengtu = new SensorData(id3, shengtuData);
            flowSensors.Add(shengtu);
            arg.FlowSensors = flowSensors;
            return arg;
            //这个是查尔岩，西坡，剩土三个水厂的流量观察数据，是当前时间前面24小时的数据
            //List<TimeSeries> cryData = new List<TimeSeries>();
            //List<TimeSeries> xipoData = new List<TimeSeries>();
            //List<TimeSeries> shengtuData = new List<TimeSeries>();
            //var aData = new TimeSeries(DateTime.Now, 0.123);

            //数据案例

            //     7/19/2021 10:31 419
            //     7/19/2021 10:41 523
            //     7/19/2021 10:52 443
            //     7/19/2021 11:02 445
            //     7/19/2021 11:13 438

            //aData = new TimeSeries(new DateTime(2021, 07, 19, 10, 31, 0), 419.0);
            //cryData.Add(aData);
            //xipoData.Add(aData);         // 这个根据西坡的实际数据来加
            //shengtuData.Add(aData);      // 这个根据剩土实际数据来加
            //aData = new TimeSeries(new DateTime(2021, 07, 19, 10, 41, 0), 523.0);
            //cryData.Add(aData);
            //xipoData.Add(aData);
            //shengtuData.Add(aData);
            //aData = new TimeSeries(new DateTime(2021, 07, 19, 10, 52, 0), 443.0);
            //cryData.Add(aData);
            //xipoData.Add(aData);
            //shengtuData.Add(aData);
            //aData = new TimeSeries(new DateTime(2021, 07, 19, 11, 02, 0), 445.0);
            //cryData.Add(aData);
            //xipoData.Add(aData);
            //shengtuData.Add(aData);
            //aData = new TimeSeries(new DateTime(2021, 07, 19, 11, 13, 0), 438.0);
            //cryData.Add(aData);
            //xipoData.Add(aData);
            //shengtuData.Add(aData);
            //arg.cryData = cryData;
            //arg.shengtuData = shengtuData;
            //arg.xipoData = xipoData;

        }
        public static RunEPSArg DummyRunEPSArg()
        {
            var arg = new RunEPSArg();
            arg = (RunEPSArg)FillDummyBaseArg(arg);
            return arg;
        }

        public static FireDemandArg DummyFireArg()
        {
            var arg = new FireDemandArg()
            { ModelPath = wenganModel, NodeId = 1339, FireStartTime = DateTime.Parse("2021-07-20 19:00:00Z"), DurationHours = 3, DemandInLitersPerSecond = 50 };
            arg = (FireDemandArg)FillDummyBaseArg(arg);
            return arg;
        }

        public static BreakPipeArg DummyBreakPipeArg()
        {
            var arg = new BreakPipeArg()
            { ModelPath = wenganModel, PipeId = 44, ValvesToExclude = new[] { 48 }, BreakPointDistanceToStartNode = 1 };
            arg.PipeId = 2366;
            arg.ValvesToExclude = new int[] { 3836 };
            arg.BreakPointDistanceToStartNode = 10;
            arg = (BreakPipeArg)FillDummyBaseArg(arg);
            return arg;
        }
        public static MultiWaterSourceTraceArg DummyWaterTraceArg()
        {
            var arg = new MultiWaterSourceTraceArg();
            arg = (MultiWaterSourceTraceArg)FillDummyBaseArg(arg);
            arg.TraceSourceElementIds = new List<int>() { 2243, 2965, 2953 };
            return arg;
        }
        public static WaterAgeArg DummyWaterAgeArg()
        {
            var arg = new WaterAgeArg();
            arg = (WaterAgeArg)FillDummyBaseArg(arg);
            arg.LastCalculationResultCount = 48;
            return arg;
        }
        public static WaterConcentrationArg DummyWaterConcentrationArg()
        {
            var arg = new WaterConcentrationArg();
            arg.CurrentConcentration = new Dictionary<int, double>() { { 2949, 0.57 }, { 2957, 0.79 }, { 2961, 0.52 } };
            arg = (WaterConcentrationArg)FillDummyBaseArg(arg);
            arg.LastCalculationResultCount = 48;
            return arg;
        }
        public static ForecastDemandArg UpdateDemandArg()
        {
            var arg = new ForecastDemandArg();
            arg.ModelPath = wenganModel;
            arg.DateTime = new DateTime(2021, 07, 20, 0, 0, 0);

            //这个是查尔岩，西坡，剩土三个水厂的流量观察数据，是当前时间前面24小时的数据
            List<TimeSeries> cryData = new List<TimeSeries>();
            List<TimeSeries> xipoData = new List<TimeSeries>();
            List<TimeSeries> shengtuData = new List<TimeSeries>();
            var aData = new TimeSeries(DateTime.Now, 0.123);

            //数据案例

            //     7/19/2021 10:31 419
            //     7/19/2021 10:41 523
            //     7/19/2021 10:52 443
            //     7/19/2021 11:02 445
            //     7/19/2021 11:13 438

            aData = new TimeSeries(new DateTime(2021, 07, 19, 10, 31, 0), 419.0);
            cryData.Add(aData);
            xipoData.Add(aData);         // 这个根据西坡的实际数据来加
            shengtuData.Add(aData);      // 这个根据剩土实际数据来加
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
            arg.cryData = cryData;
            arg.shengtuData = shengtuData;
            arg.xipoData = xipoData;

            return arg;
        }
    }
}
