using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiaoDongBayTest.WengAn.Args
{
    /// <summary>
    /// 公共父参数
    /// </summary>
    public class WengAnBaseArg
    {
        /// <summary>
        /// 模型Sqlite文件路径
        /// </summary>
        [Required]
        public string ModelPath { get; set; }

        /// <summary>
        /// 当前水泵速度相对系数(由当前值除以最大值得出。例:当前速度40hz/最大速度50hz = 0.8) ，为0代表关闭  数据类型：Dictionary(设备id, 值)
        /// </summary>
        public IDictionary<int, double> CurrentPumpSpeed { get; set; } = new Dictionary<int, double>();
        ///// <summary>
        ///// 当前水泵开关状态
        ///// </summary>
        //public IDictionary<int, double> CurrentPumpStatus { get; set; } = new Dictionary<int, double>();
        /// <summary>
        /// 当前阀门关度 泵后阀(仅Hammer计算时才传这个，EPS方案里没泵后阀设备，不传)
        /// </summary>
        //public IDictionary<int, double> CurrentAfterPumpValveClosure { get; set; }
        /// <summary>
        /// 当前PRV阀压力 数据类型：Dictionary(设备id, double值)
        /// </summary>
        public IDictionary<int, double> CurrentPrvPressure { get; set; } = new Dictionary<int, double>();
        /// <summary>
        /// 水厂当前液位 模型里5个水厂，目前只用到三个 其他两个水厂的可不传 数据类型：Dictionary(设备id, double值)
        /// </summary>
        public IDictionary<int, double> CurrentReservoirElevation { get; set; } = new Dictionary<int, double>();

        #region 影响返回结果

        /// <summary>
        /// 想要获取结果的节点id  数组
        /// </summary>
        public int[] ResultNodeIds { get; set; }
        /// <summary>
        /// 想要获取结果的节点id  数组
        /// </summary>
        public int[] ResultPipeIds { get; set; }

        #endregion

    }
}
