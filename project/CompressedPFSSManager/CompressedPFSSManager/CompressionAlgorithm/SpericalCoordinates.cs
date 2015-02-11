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
        /// Convert 32Bit floating point data to 16 Bit.
        /// </summary>
        /// <param name="data"></param>
        public static void To16BitSpherical(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    double r = p.Radius;
                    double phi = p.Phi ;
                    double theta = p.Theta;

                    p.Radius = (short)Math.Round(r * 8192);
                    p.Phi = (short)Math.Round(phi * 32768.0);
                    p.Theta = (short)Math.Round(theta * 32768.0);

                    ShiftToCenter(p);
                }
            }
        }
    }
}
