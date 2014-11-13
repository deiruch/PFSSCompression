using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FITSFormatter.PFSS;

namespace FITSFormatter
{
    class MoveData
    {
        public static void ForwardSpherical(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points) 
                {
                    float rCopy = p.x - 8192f;
                    if (rCopy < 0)
                    {
                        rCopy = 0;
                    }

                    p.x = (rCopy - 12288f);
                    p.y = (p.y - 16384f);
                    p.z = (p.z - 8192f);
                }
            }
        }

        public static void BackwardSpherical(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    p.x = (p.x + 12288f + 8192f);
                    p.y = (p.y + 16384f);
                    p.z = (p.z + 8192f);
                }
            }
        }
    }
}
