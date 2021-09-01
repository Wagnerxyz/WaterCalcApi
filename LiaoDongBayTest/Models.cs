using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{

    /*
     *2949	胜土水厂	1,138.65	<无>	(N/A)	(N/A)
2957	擦耳岩水厂	1,102.00	<无>	(N/A)	(N/A)
2961	西坡水厂	1,104.30	<无>	(N/A)	(N/A)

     *
     */
    public class WaterTraceResult
    {
        /// <summary>
        /// Junction Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 胜土水厂
        /// </summary>
        public double[] Source1Percentage { get; set; }
        /// <summary>
        /// 擦耳岩水厂
        /// </summary>
        public double[] Source2Percentage { get; set; }
        /// <summary>
        /// 西坡水厂
        /// </summary>
        public double[] Source3Percentage { get; set; }

        public double[] TimeStep { get; set; }
    }
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
        /// 管道水头损失
        /// </summary>
        //public double[] PipeHeadLoss { get; set; }
        /// <summary>
        /// 管道水头损失梯度
        /// </summary>
        public double[] PipeHeadlossGradient { get; set; }
      

        public double[] TimeSteps { get; set; }
    }
}
