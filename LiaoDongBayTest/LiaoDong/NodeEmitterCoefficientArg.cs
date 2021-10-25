using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Configuration.Annotations;

namespace LiaoDongBay
{
    /// <summary>
    /// 辽东湾输入参数
    /// </summary>
    public class NodeEmitterCoefficientArg
    {
        /// <summary>
        /// 模型Sqlite文件路径 不确定暂时可任意填写 
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
