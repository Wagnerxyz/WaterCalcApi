using Haestad.Network.Segmentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace WengAn.Args
{
    /// <summary>
    /// 爆管影响结果对象
    /// </summary>
    public class BreakPipeResult : WaterEngineResultBase
    {
        /// <summary>
        /// 各节点压力 数据类型： Dictionary<设备id, double[]>
        /// </summary>
        public Dictionary<int, double[]> NodePressures { get; set; }
        /// <summary>
        /// 需要关闭的阀门id
        /// </summary>
        public int[] ValvesToClose { get; set; }
        /// <summary>
        /// 需要关闭的隔离阀门id
        /// </summary>
        public int[] IsolationValvesToClose { get; set; }
        /// <summary>
        /// 受影响管道Id
        /// </summary>
        public int[] IsolatedPipeIds { get; set; }
        /// <summary>
        /// 受影响节点Id
        /// </summary>
        public int[] IsolatedNodeIds { get; set; }
        /// <summary>
        /// 受影响水表Id
        /// </summary>
        public int[] IsolatedCustomerIds { get; set; }
        //public IList<HmiPartialPipe> IsolatedPartialPipeIds { get; set; }
        //public IList<HmiPartialPipe> OutagePartialPipeIds { get; set; }
        //public int[] OutagePipeIds { get; set; }
        //public int[] OutageNodeIds { get; set; }
        //public int[] OutageCustomerIds { get; set; }

        //public int[] PipesToClose { get; set; }
        //public bool TracePercentageResults { get; set; }

    }
}
