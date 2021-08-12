using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiaoDongBayTest
{
    class DummyData
    {
        internal static IDictionary<int, double> GetLiaoDongNodePressure()
        {
            var dict = new Dictionary<int, double>();
            dict.Add(81, 30);
            dict.Add(94, 30.89);
            dict.Add(150, 27.42);
            dict.Add(175, 34.07);
            dict.Add(182, 27.84);
            dict.Add(240, 30.94);
            dict.Add(278, 30.89);
            dict.Add(294, 30.43);
            return dict;
        }
        internal static IDictionary<int, double> GetLiaoDongPipeFlow()
        {
            var dict = new Dictionary<int, double>();
            dict.Add(317, 100);
            dict.Add(305, 93);
            dict.Add(253, 93);
            dict.Add(353, 93);
            dict.Add(157, 110);
            dict.Add(539, 102);
            dict.Add(337, 102);
            dict.Add(531, 102);
            dict.Add(366, 102);
            dict.Add(480, 102);
            return dict;
        }
    }
}
