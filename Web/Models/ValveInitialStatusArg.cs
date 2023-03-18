using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    /// <summary>
    /// 更改阀门状态接口输入参数
    /// </summary>
    public class ValveInitialStatusArg
    {
        /// <summary>
        /// 模型文件 .sqlite路径
        /// </summary>
        public string ModelPath { get; set; }
        /// <summary>
        /// 阀门id
        /// </summary>
        public int ValveId { get; set; }
        /// <summary>
        /// 阀门状态(0-激活，1-非激活，2-关闭)
        /// </summary>
        public ValveSettingEnum ValveStatus { get; set; }
    }
}