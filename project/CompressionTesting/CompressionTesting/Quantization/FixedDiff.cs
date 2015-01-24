using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

namespace CompressionTesting.Quantization
{
    class FixedDiff
    {
        internal PFSSPoint difference;
        internal PFSSPoint minimum;
        internal List<short> xPoints;
        internal List<short> yPoints;
        internal List<short> zPoints;
        public static void Forward(PFSSData data,int FixPoints) {
            foreach (PFSSLine l in data.lines)
            {
                
                PFSSPoint mins = GetMin(l);
                PFSSPoint max = GetMax(l);
                PFSSPoint diff = null;
                //diff = max-min/fixpoints

                PFSSPoint currentMin= null;
                PFSSPoint currentMax = null;

                int xIndex = 0;
                int yIndex = 0;
                int zIndex = 0;
                for (int i = 0; i < l.points.Count; i++)
                {
                    PFSSPoint current = l.points[i];

                    if (current.x >= diff.x)
                        ;//add xPoint plus
                    //update min max
                    if (current.x <= currentMin.x)
                        ;//add xPoint minus
                }
                
            }
        }

        private static PFSSPoint GetMin(PFSSLine l)
        {
            return null;
        }

        private static PFSSPoint GetMax(PFSSLine l)
        {
            return null;
        }



    }
}
