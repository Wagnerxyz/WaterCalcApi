using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haestad.Calculations.Shanghai.QingDaoWT;
using Models;

namespace LiaoDongBayTest.WengAn
{
  public  class WengAnDemandForecast
    {
        public static void Run(ForecastDemandArg arg)
        {
            //CALL 预报更新 API 
            WengAnProj project = new WengAnProj(arg.DateTime, arg.ModelPath,arg.cryData,arg.xipoData,arg.shengtuData);
            project.Forecast();

            ////check some results w/o adjustment
            //double[] times;
            //double[] results;
            //double diff;
            //times = project.wm.PressureResult.GetPressureEngineCalculationTimeStepsInSeconds();

            ////discharge
            //int pipeID = 2624;
            //results = project.wm.PressureResult.GetPipeFlowInCubicMetersPerSecond(pipeID);

            //textOut("Expected Results ");
            //diff = 0.136464 - results[12];
            //string line = "12: The Difference between Computed and Expected is  " + diff.ToString();
            //textOut(line);

            //diff = 0.128045 - results[68];
            //line = "68: The Difference between Computed and Expected is  " + diff.ToString();
            //textOut(line);


            //textOut(" ");

            //5 Close database
            project.wm.CloseDataSource();
            
        }
    }
}
