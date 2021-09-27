using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Haestad.Support.Units;
using Haestad.Support.User;
using LiaoDongBayTest.WengAn.Args;

namespace Models
{
    /// <summary>
    /// Bentley水力计算引擎信息基类
    /// </summary>
    public class WaterEngineResultBase
    {
        /// <summary>
        /// 先检查这个，若true，则读ErrorNotifs详细信息
        /// </summary>
        public bool IsCalculationFailure { get; set; }
        /// <summary>
        /// 引擎计算的错误信息
        /// </summary>
        public IList<UserNotification> ErrorNotifs { get; set; }
    }
    /// <summary>
    /// Haestad.Support.User.IUserNotification的DTO，减少外部程序对Haestad.Support.dll的依赖
    /// </summary>
    public class UserNotification
    {
        public string Label { get; set; }

        /// <summary>
        /// Numeric ID (derived from MessageKey). Intended to be unique across all HMI products. 
        /// This is the public "error number," which can be documented and communicated from
        /// customers to HMI technical support.
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// Associated element ID, or -1 if there if this message does not refer to an individual
        /// element. Query ElementType to know what kind of ID this is: usually it would be an IModelingElement.
        /// </summary>
        public int ElementId { get; set; }

        /// <summary>
        /// Indicates the type of element referred to by this message. Values are considered
        /// to be ModelingElementType's. May need to reserve negative values, or a higher-range of values
        /// for application or product-specific types? 
        /// </summary>
        public int ElementType { get; set; }

        /// <summary>
        /// The help system key for extended help about this notification. Null if none.
        /// </summary>
        public String HelpId { get; set; }

        /// <summary>
        /// The severity or type of notification.
        /// </summary>
        public NotificationLevel Level { get; set; }

        /// <summary>
        /// The string key for retrieving a culture-specific TextManager template to be filled with optional 
        /// positional parameters.
        /// </summary>
        public String MessageKey { get; set; }

        /// <summary>
        /// The positional parameters to be supplied to the TextManager template retrieved with MessageKey.
        /// If there are no parameters, this should be an empty array (not null).
        /// </summary>
        public Object[] Parameters { get; set; }

        /// <summary>
        /// The ID of the scenario this message refers to: -1 if no scenario is specifically indicated.
        /// </summary>
        public int ScenarioId { get; set; }

        /// <summary>
        /// String key for TextManager to retrieve culture-specific description of the source of the message.
        /// This can be used to distinguish different numerical engines, different components, or different
        /// subystems which generate notifications.
        /// </summary>
        public String SourceKey { get; set; }

        /// <summary>
        /// Working units for any unitized values in Parameters list. Guaranteed to be same length as
        /// Parameters. Will hold Unit.None in corresponding position for any non-unitized or non-numeric parameters.
        /// </summary>
        //[SwaggerExclude]
        public Unit[] WorkingUnits { get; set; }

        /// <summary>
        /// NumericFormatters for each of the Parameters.  If a parameter is not unitized, then the
        /// numeric formatter should be null
        /// </summary>
        public String[] NumericFormatterKeys { get; set; }

        /// <summary>
        /// Determines if the shortcut icon should be used and determines if there are other
        /// user notifications related to the current one.
        /// </summary>
        public bool HasSubUserNotifications { get; set; }

        /// <summary>
        /// Mark this UN so that the UN manager will not filter out duplicates of it even if the 
        /// duplicate filtering option is enabled.
        /// </summary>
        public bool ExcludeFromDuplicateProcessing { get; set; }
    }

    /*
     *2949	胜土水厂	1,138.65	<无>	(N/A)	(N/A)
2957	擦耳岩水厂	1,102.00	<无>	(N/A)	(N/A)
2961	西坡水厂	1,104.30	<无>	(N/A)	(N/A)

     *
     */
    /// <summary>
    /// 水源追踪结果
    /// </summary>
    public class WaterTraceResult : WaterEngineResultBase
    {
        public List<WaterTracePercentage> TracePercentageResults { get; set; }
    }

    public class WaterTracePercentage
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

    public class WengAnEpsArg : WengAnBaseArg
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; }

    }
    /// <summary>
    /// EPS计算结果
    /// </summary>
    public class WengAnEpsResult : WaterEngineResultBase
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
        ///// <summary>
        ///// 水头
        ///// </summary>
        //public double[] HGL { get; set; }
        /// <summary>
        /// 压力
        /// </summary>
        public double[] Pressures { get; set; }
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

    #region 消防事件
    /// <summary>
    /// 消防事件接口 输入参数
    /// </summary>
    public class FireDemandArg : WengAnBaseArg
    {

        /// <summary>
        /// 着火节点Id
        /// </summary>
        [Required]
        public int NodeId { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 持续小时
        /// </summary>
        [Required]
        public double DurationHours { get; set; }
        /// <summary>
        /// 消防流量 升/每秒
        /// </summary>
        [Required]
        public double DemandInLitersPerSecond { get; set; }
    }
    /// <summary>
    /// 消防事件接口 输入参数
    /// </summary>
    public class FireDemandResult : WaterEngineResultBase
    {
        /// <summary>
        /// 着火节点Id
        /// </summary>
        [Required]
        public int NodeId { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 持续小时
        /// </summary>
        [Required]
        public double DurationHours { get; set; }
        /// <summary>
        /// 消防流量 升/每秒
        /// </summary>
        [Required]
        public double DemandInLitersPerSecond { get; set; }
    }

    #endregion

    /// <summary>
    /// 水龄或水质结果
    /// </summary>
    public class WaterQualityResult : WaterEngineResultBase
    {
        /// <summary>
        /// 节点水龄或水质结果,按小时
        /// </summary>
        public IDictionary<int, double[]> NodeResult { get; set; }
        /// <summary>
        /// 管道水龄或水质结果,按小时
        /// </summary>
        public IDictionary<int, double[]> PipeResult { get; set; }
    }
    /// <summary>
    /// 水质余氯预测 输入参数
    /// </summary>
    public class WaterConcentrationArg
    {
        /// <summary>
        /// 三个水厂reservior的id及 当前出厂余氯浓度
        /// </summary>
        public IDictionary<int, double> CurrentConcentration { get; set; }
        /// <summary>
        /// 模型Sqlite文件路径
        /// </summary>
        [Required]
        public string ModelPath { get; set; }
    }
}
