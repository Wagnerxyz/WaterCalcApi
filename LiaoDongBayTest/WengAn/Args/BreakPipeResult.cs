using Haestad.Network.Segmentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WengAn.Args
{
    public class BreakPipeResult
    {
        //重要参数
        public int[] ValvesToClose { get; set; }
        public int[] PipesToClose { get; set; }
        //重要参数
        public int[] IsolationValvesToClose { get; set; }
        public IList<HmiPartialPipe> IsolatedPartialPipeIds { get; set; }
        public int[] IsolatedPipeIds { get; set; }
        public int[] IsolatedNodeIds { get; set; }
        public int[] IsolatedCustomerIds { get; set; }
        public IList<HmiPartialPipe> OutagePartialPipeIds { get; set; }
        public int[] OutagePipeIds { get; set; }
        public int[] OutageNodeIds { get; set; }
        public int[] OutageCustomerIds { get; set; }
        public Dictionary<int, double[]> NodePressures { get; set; }
        //public bool Result { get; set; }

    }
}
