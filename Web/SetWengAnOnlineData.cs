using ChinaWaterLib.WengAn.Args;
using Haestad.Calculations.Shanghai.WaterGEMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web
{
    public static class SetWengAnOnlineData
    {
        public static void SetCurrentPumpValveReservoir(WaterGEMSModel wm, WengAnCalculationBaseArg args)
        {
            //foreach (var item in args.CurrentPumpSpeed)
            //{

            //    if (item.Value != 0)
            //    {
            //        wm.InitialSetting.SetInitialPumpStatus(item.Key, PumpInitialSettingEnum.PumpOn);
            //        wm.InitialSetting.SetInitialPumpSpeed(item.Key, item.Value);

            //    }
            //    else
            //    {
            //        wm.InitialSetting.SetInitialPumpStatus(item.Key, PumpInitialSettingEnum.PumpOff);

            //    }
            //} 
            //不管开关泵信息

            foreach (var item in args.CurrentPumpPressure)
            {
                wm.SetVariableSpeedPumpTargetPressure(item.Key, item.Value);
            }
            foreach (var item in args.CurrentPrvPressure)
            {
                wm.SetPRVPressure(item.Key, item.Value);
            }


            foreach (var item in args.CurrentReservoirElevation)
            {
                wm.SetReservoirFixedElevation(item.Key, item.Value);
            }
        }
    }
}