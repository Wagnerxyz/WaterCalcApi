using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WengAn.Args
{
    public class BreakPipeArg
    {
        /// <summary>
        ///     模型Sqlite文件路径
        /// </summary>
        [Required]
        public string ModelPath { get; set; }

        /// <summary>
        ///     要爆的管道id
        /// </summary>
        [Required]
        public int PipeId { get; set; }

        /// <summary>
        /// 要排除的阀门，不能关的阀门
        /// </summary>
        public int[] ValvesToExclude { get; set; }
        /// <summary>
        /// 爆管点距离开始节点距离
        /// </summary>
        [Required]
        public double BreakPointDistanceToStartNode { get; set; }
    }
}
