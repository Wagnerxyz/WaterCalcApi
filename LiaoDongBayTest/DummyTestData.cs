using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haestad.Calculations.Shanghai.QingDaoWT;
using LiaoDongBayTest.WengAn.Args;
using Models;
using WengAn.Args;

namespace LiaoDongBayTest
{
    public class DummyTestData
    {
        public const string wenganModel = @"D:\BentleyModels\WengAn\WengAn0916.wtg.sqlite";
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
                {3974, 30.0},
                {3982, 67.0}
            };
            return dict;
        }
        public static WengAnBaseArg FillDummyBaseArg(WengAnBaseArg arg)
        {
            arg.ModelPath = wenganModel;
            arg.CurrentPrvPressure = GetCurrentPRVPressure();
            arg.CurrentPumpSpeed = GetCurrentPumpRelativeSpeed();
            arg.CurrentReservoirElevation = GetCurrentReservoirElevation();
            return arg;
        }
        public static RunEPSArg DummyRunEPSArg()
        {
            var arg = new RunEPSArg();
            arg = (RunEPSArg)FillDummyBaseArg(arg);
            return arg;
        }
        public static WaterConcentrationArg DummyWaterConcentrationArg()
        {
            var arg = new WaterConcentrationArg();
            arg.CurrentConcentration = new Dictionary<int, double>() { { 2949, 0.57 }, { 2957, 0.79 }, { 2961, 0.52 } };
            arg = (WaterConcentrationArg)FillDummyBaseArg(arg);
            return arg;
        }
        public static FireDemandArg DummyFireArg()
        {
            var arg = new FireDemandArg()
            { ModelPath = wenganModel, NodeId = 1339, StartTime = DateTime.Parse("2021-07-04 17:00:00Z"), DurationHours = 3, DemandInLitersPerSecond = 50 };
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
        public static WengAnBaseArg DummyWaterTraceArg()
        {
            var arg = new WengAnBaseArg();
            arg.ModelPath = wenganModel;
            return arg;
        }
        public static WengAnBaseArg DummyWaterAgeArg()
        {
            var arg = new WengAnBaseArg();
            arg.ModelPath = wenganModel;
            return arg;
        }
        public static ForecastDemandArg UpdateDemandArg()
        {
            var arg = new ForecastDemandArg();
            arg.ModelPath = wenganModel;
            arg.DateTime = new DateTime(2021, 07, 20, 0, 0, 0);

            //这个是查尔岩，西坡，剩土三个水厂的流量观察数据，是当前时间前面24小时的数据
            List<ObsDate> cryData = new List<ObsDate>();
            List<ObsDate> xipoData = new List<ObsDate>();
            List<ObsDate> shengtuData = new List<ObsDate>();
            var aData = new ObsDate(DateTime.Now, 0.123);

            //数据案例

            //     7/19/2021 10:31 419
            //     7/19/2021 10:41 523
            //     7/19/2021 10:52 443
            //     7/19/2021 11:02 445
            //     7/19/2021 11:13 438

            aData = new ObsDate(new DateTime(2021, 07, 19, 10, 31, 0), 419.0);
            cryData.Add(aData);
            xipoData.Add(aData);         // 这个根据西坡的实际数据来加
            shengtuData.Add(aData);      // 这个根据剩土实际数据来加
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
            arg.cryData = cryData;
            arg.shengtuData = shengtuData;
            arg.xipoData = xipoData;

            return arg;
        }
    }
}
