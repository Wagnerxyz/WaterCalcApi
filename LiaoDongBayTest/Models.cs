using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiaoDongBayTest
{

    public class WengAnEpsResult
    {
        public List<EpsNodeResult> EpsNodeResult { get; set; }
        public List<EpsPipeResult> EpsPipeResult { get; set; }
    }

    public class EpsNodeResult
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public double[] TimeSteps { get; set; }
        /// <summary>
        /// 水头
        /// </summary>
        public double[] HGL { get; set; }
    }
    public class EpsPipeResult
    {
        public int Id { get; set; }
        public string Label { get; set; }
        /// <summary>
        /// 管道流量
        /// </summary>
        public double[] Flows { get; set; }
        /// <summary>
        /// 管道流速
        /// </summary>
        public double[] Velocities { get; set; }
        /// <summary>
        /// 水头损失
        /// </summary>
        public double[] HeadLoss { get; set; }
        public double[] TimeSteps { get; set; }
    }
}
