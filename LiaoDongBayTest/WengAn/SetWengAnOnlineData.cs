﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haestad.Calculations.Shanghai.WaterGEMS;
using LiaoDongBayTest.WengAn.Args;


public class Utils
{
    public static void SetCurrentPumpValveReservoir(WaterGEMSModel wm, WengAnBaseArg args)
    {
        foreach (var item in args.CurrentPumpSpeed)
        {
            if (item.Value != 0)
            {
                wm.InitialSetting.SetInitialPumpStatus(item.Key, PumpInitialSettingEnum.PumpOn);
                wm.InitialSetting.SetInitialPumpSpeed(item.Key, item.Value);

            }
            else
            {
                wm.InitialSetting.SetInitialPumpStatus(item.Key, PumpInitialSettingEnum.PumpOff);

            }
        }

        foreach (var item in args.CurrentPrvPressure)
        {
            wm.SetPRVPressure(item.Key, item.Value);
        }
        //todo:有问题 报错 ！！

        foreach (var item in args.CurrentReservoirElevation)
        {
            wm.SetReservoirFixedElevation(item.Key, item.Value);
        }
    }
}
