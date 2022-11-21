using ChinaWaterLib;
using LiaoDongBay;
using Swashbuckle.Examples;
using System.Configuration;

namespace Web.Swagger
{
    public class WA_RunEPS1_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return WengAnDummyData.DummyRunEPSArg();

        }
    }
    public class WA_WaterConcentration_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return WengAnDummyData.DummyWaterConcentrationArg();
        }
    }
    public class WA_Fire_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return WengAnDummyData.DummyFireArg();
        }
    }
    public class WA_BreakPipe_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return WengAnDummyData.DummyBreakPipeArg();
        }
    }
    public class WA_WaterAge_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return WengAnDummyData.DummyWaterAgeArg();
        }
    }
    public class WA_WaterTrace_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return WengAnDummyData.DummyWaterTraceArg();
        }
    }
    public class WA_UpDateDemand_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return WengAnDummyData.UpdateDemandArg();
        }
    }
}