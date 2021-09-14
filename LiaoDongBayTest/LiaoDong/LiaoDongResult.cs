using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiaoDongBayTest
{
    /// <summary>
    /// 辽东湾泄漏检测结果
    /// </summary>
   public class LiaoDongResult
    {
        /// <summary>
        /// 是否流量平衡(由公式算出)
        /// </summary>
        public bool IsBalanced { get; set; }
        /// <summary>
        /// 各个节点的泄漏量
        /// </summary>
        public Dictionary<int,double> NodeEmitterCoefficientsInAscendingOrderInLitersPerSecondPerMetersH2O { get; set; }
    }
}
