using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiaoDongBayTest.WengAn.Args;
using Models;
using WengAn.Args;

namespace LiaoDongBayTest
{
    public class DummyTestData
    {
        const string wenganModel = @"D:\BentleyModels\WengAn\WengAn0916.wtg.sqlite";
        public static Dictionary<int, double> GetCurrentTankElevation()
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
        internal static Dictionary<int, double> GetCurrentPRVPressure()
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
            arg.CurrentTankElevations = GetCurrentTankElevation();
            return arg;
        }
        public static WengAnBaseArg DummyBaseArg()
        {
            var arg = new WengAnBaseArg();
            arg = FillDummyBaseArg(arg);
            return arg;
        }
        public static WaterConcentrationArg DummyWaterConcentrationArg()
        {
            var arg = new WaterConcentrationArg();
            arg.ModelPath = wenganModel;
            arg.CurrentConcentration = new Dictionary<int, double>() { { 2949, 0.57 }, { 2957, 0.79 }, { 2961, 0.52 } };
            arg = (WaterConcentrationArg)FillDummyBaseArg(arg);
            return arg;
        }
        public static FireDemandArg DummyFireArg()
        {
            var arg = new FireDemandArg()
            { ModelPath = wenganModel, NodeId = 1339, StartTime = DateTime.Parse("2021-09-26 17:00:00Z"), DurationHours = 3, DemandInLitersPerSecond = 50 };

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

    }
}
