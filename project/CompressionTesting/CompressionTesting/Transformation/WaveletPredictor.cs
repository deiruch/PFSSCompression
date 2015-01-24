using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

namespace CompressionTesting.Transformation
{
    class WaveletPredictor
    {
        public static void Forward(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint start = l.points[0];
                PFSSPoint end = l.points[l.points.Count - 1];

                if (l.points.Count % 2 == 0)
                {

                }
                else
                {
                    
                }
            }
        }


    }
}
