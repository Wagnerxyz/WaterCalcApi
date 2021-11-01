using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiaoDongBay.Models
{
    /// <summary>
    /// 请求参数，无需传ModelPath，接口里自动设置
    /// </summary>
    public class LiaoDongArg
    {
        /// <summary>
        /// 实测节点的压力值集合
        /// </summary>
        public IDictionary<int, double> CurrentNodePressures { get; set; }

        /// <summary>
        /// 实测管道的流量值集合
        /// </summary>
        public IDictionary<int, double> CurrentPipeFlows { get; set; }
    }
}