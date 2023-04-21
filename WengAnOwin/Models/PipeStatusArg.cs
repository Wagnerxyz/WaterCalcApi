using ChinaWaterLib.WengAn.Args;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WengAnOwin.Models
{
    /// <summary>
    /// 更改管道状态接口输入参数
    /// </summary>
    public class PipeStatusArg
    {
        /// <summary>
        /// 模型文件 .sqlite路径
        /// </summary>
        public string ModelPath { get; set; }
        /// <summary>
        /// 管道id
        /// </summary>
        public int PipeId { get; set; }
        /// <summary>
        /// 管道状态(0-打开，1-关闭)
        /// </summary>
        public PipeStatusEnum PipeStatus { get; set; }
    }
}