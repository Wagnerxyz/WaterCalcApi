using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WengAn.Args
{
    public class BreakPipeArg
    {
        /// <summary>
        ///     模型Sqlite文件路径
        /// </summary>
        public string ModelPath { get; set; }

        /// <summary>
        ///     要爆的管道id
        /// </summary>
        public int PipeId { get; set; }

        /// <summary>
        ///     要排除的阀门，不能关的阀门
        /// </summary>
        public int[] ValvesToExclude { get; set; }

        public double BreakPointDistanceToStartNode { get; set; }
        /// <summary>
        ///     节点压力阈值
        /// </summary>
        public double JunctionThreshold { get; set; }
    }
}
