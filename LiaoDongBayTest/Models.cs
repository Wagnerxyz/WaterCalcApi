using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// <summary>
    /// 水源追踪结果
    /// </summary>
    public class WaterTraceResult
    {
        /// <summary>
        /// Junction Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 胜土水厂水源百分比
        /// </summary>
        public double[] Source1Percentage { get; set; }
        /// <summary>
        /// 擦耳岩水厂水源百分比
        /// </summary>
        public double[] Source2Percentage { get; set; }
        /// <summary>
        /// 西坡水厂水源百分比
        /// </summary>
        public double[] Source3Percentage { get; set; }
        /// <summary>
        /// 时间步长
        /// </summary>
        public double[] TimeStep { get; set; }
    }

    public class WengAnEpsArg
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 模型Sqlite文件路径
        /// </summary>
        [Required]
        public string ModelPath { get; set; }

        /// <summary>
        /// 当前水泵相对速度
        /// </summary>
        public IDictionary<int, double> CurrentPumpSpeed { get; set; }
        /// <summary>
        /// 当前阀门关度 独立TCV阀
        /// </summary>
        public IDictionary<int, double> CurrentPCVValveClosure { get; set; }
        /// <summary>
        /// 当前液位
        /// </summary>
        public IDictionary<int, double> CurrentTankElevations { get; set; }
    }
    /// <summary>
    /// EPS计算结果
    /// </summary>
    public class WengAnEpsResult
    {
        /// <summary>
        /// 所有节点结果
        /// </summary>
        public List<EpsNodeResult> EpsNodeResult { get; set; }
        /// <summary>
        /// 所有管道结果
        /// </summary>
        public List<EpsPipeResult> EpsPipeResult { get; set; }
    }

    public class EpsNodeResult
    {
        public int Id { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 时间步长
        /// </summary>
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
        /// <summary>
        /// 时间步长
        /// </summary>
        public double[] TimeSteps { get; set; }
    }
}
