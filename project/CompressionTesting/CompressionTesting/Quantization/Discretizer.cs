using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

namespace CompressionTesting.Quantization
{
    class Discretizer
    {
        public static void ToShorts(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    p.x = (short)p.x;
                    p.y = (short)p.y;
                    p.z = (short)p.z;
                }
            }
        }
    }
}
