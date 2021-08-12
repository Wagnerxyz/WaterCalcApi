using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiaoDongBayTest
{
    public class LiaoDongArg
    {
        /// <summary>
        /// 模型Sqlite文件路径
        /// </summary>
        public string ModelPath { get; set; }
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
