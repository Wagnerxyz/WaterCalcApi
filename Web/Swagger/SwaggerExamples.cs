using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LiaoDongBay.Controllers;
using LiaoDongBayTest;
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
}