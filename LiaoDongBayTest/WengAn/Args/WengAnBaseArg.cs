using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiaoDongBayTest.WengAn.Args
{
    public class WengAnBaseArg
    { 
      

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
        /// 当前水泵开关状态
        /// </summary>
        public IDictionary<int, double> CurrentPumpStatus { get; set; }
        /// <summary>
        /// 当前阀门关度 泵后阀(仅Hammer计算时才传这个，EPS方案里没泵后阀设备，不传)
        /// </summary>
        public IDictionary<int, double> CurrentAfterPumpValveClosure { get; set; }
        /// <summary>
        /// 当前PRV阀压力
        /// </summary>
        public IDictionary<int, double> CurrentPrvPressure { get; set; }
        /// <summary>
        /// 当前液位
        /// </summary>
        public IDictionary<int, double> CurrentTankElevations { get; set; }
    }
}
