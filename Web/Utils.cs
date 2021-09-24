using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Haestad.Calculations.Shanghai.WaterGEMS;
using LiaoDongBayTest.WengAn.Args;

namespace LiaoDongBay
{
    //todo: todo
    public class Utils
    {
        /// <summary>
        ///设置在线数据接入
        /// </summary>
        public static void SetCurrentPumpValveTank(WaterGEMSModel wm, WengAnBaseArg args)
        {
            foreach (var item in args.CurrentPumpSpeed)
            {
                wm.InitialSetting.SetInitialPumpSpeed(item.Key, item.Value);
            }
            foreach (var item in args.CurrentPumpStatus)
            {
                wm.InitialSetting.SetInitialPumpSpeed(item.Key, item.Value);
            }

            foreach (var item in args.CurrentPrvPressure)
            {
                wm.SetPRVPressure(item.Key, item.Value);
            }

            foreach (var item in args.CurrentTankElevations)
            {
                wm.InitialSetting.SetInitialTankElevation(item.Key, item.Value);
            }
        }
    }
}