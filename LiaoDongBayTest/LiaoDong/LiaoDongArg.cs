using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LiaoDongBayTest
{
    /// <summary>
    /// 输入参数
    /// </summary>
    public class LiaoDongArg
    {
        /// <summary>
        /// 模型Sqlite文件路径
        /// </summary>
        [Required]
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
