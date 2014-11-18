using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

namespace CompressionTesting.Quantization
{
    class DCTQuantization
    {
        public static void SetToZero(PFSSData data, int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                for (int i = offset; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
                    p.x = 0;
                    p.y = 0;
                    p.z = 0;
                }
            }
        }
    }
}
