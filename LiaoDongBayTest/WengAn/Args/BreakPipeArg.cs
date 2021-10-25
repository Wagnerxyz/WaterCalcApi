using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LiaoDongBayTest.WengAn.Args;

namespace WengAn.Args
{
    /// <summary>
    /// 爆管接口输入参数
    /// </summary>
    public class BreakPipeArg : WengAnBaseArg
    {

        /// <summary>
        /// 要爆的管道id
        /// </summary>
        [Required]
        public int PipeId { get; set; }
        [Required]
        /// <summary>
        /// 要排除的阀门，不能关的阀门
        /// </summary>
        public int[] ValvesToExclude { get; set; }
        /// <summary>
        /// 爆管点距离开始节点距离
        /// </summary>
        [Required]
        public double BreakPointDistanceToStartNode { get; set; }
        /// <summary>
        /// 想要获取结果的节点id  数组
        /// </summary>
        public int[] ResultNodeIds { get; set; }
    }
}
