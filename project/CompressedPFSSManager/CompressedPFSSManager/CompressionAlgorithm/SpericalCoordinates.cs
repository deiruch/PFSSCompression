using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressedPFSSManager.PFSS;

namespace CompressedPFSSManager.CompressionAlgorithm
{
    class SphericalCoordinates
    {
        private static void ShiftToCenter(PFSSPoint p)
        {
            float rCopy = p.Radius - 8192f;
            if (rCopy < 0)
            {
                rCopy = 0;
            }

            p.Radius = (rCopy);
            p.Phi = (p.Phi - 16384f);
            p.Theta = (p.Theta - 8192f);
        }


        /// <summary>
        /// Shifts discretized data to center
        /// </summary>
        /// <param name="data"></param>
        public static void ShiftToCenter(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    ShiftToCenter(p);
                }
            }
        }
    }
}
