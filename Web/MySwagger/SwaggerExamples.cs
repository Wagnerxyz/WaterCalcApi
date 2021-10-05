using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LiaoDongBay.Controllers;
using LiaoDongBayTest;
using LiaoDongBayTest.WengAn.Args;
using Swashbuckle.Examples;

namespace LiaoDongBay.Swagger
{
    public class LD_LeakDetect_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return new LiaoDongArg()
            {
                ModelPath = LDController.modelPath,
                CurrentNodePressures = LiaoDongDummyData.GetLiaoDongNodePressure(),
                CurrentPipeFlows = LiaoDongDummyData.GetLiaoDongPipeFlow()
            };
        }
    }
    public class WA_RunEPS1_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return   DummyTestData.DummyRunEPSArg();
            
        }
    }
    public class WA_WaterConcentration_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return DummyTestData.DummyWaterConcentrationArg();
        }
    }
    public class WA_Fire_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return DummyTestData.DummyFireArg();
        }
    }
    public class WA_BreakPipe_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return DummyTestData.DummyBreakPipeArg();
        }
    }
    public class WA_WaterAge_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return DummyTestData.DummyWaterAgeArg();
        }
    }
    public class WA_WaterTrace_Example : IExamplesProvider
    {
        public object GetExamples()
        {
            return DummyTestData.DummyWaterTraceArg();
        }
    }
}