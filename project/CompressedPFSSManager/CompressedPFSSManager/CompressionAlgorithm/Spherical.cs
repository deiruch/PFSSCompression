using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressedPFSSManager.PFSS;

namespace CompressedPFSSManager.CompressionAlgorithm
{
    class Spherical
    {
        public static void ForwardMoveSpherical(PFSSPoint p)
        {
            float rCopy = p.x - 8192f;
            if (rCopy < 0)
            {
                rCopy = 0;
            }

            p.x = (rCopy);
            p.y = (p.y - 16384f);
            p.z = (p.z - 8192f);
        }

        /// <summary>
        /// Center discretized and spherical coordinate system.
        /// </summary>
        /// <param name="data"></param>
        public static void ForwardMoveSpherical(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
                foreach (PFSSPoint p in l.points)
                    ForwardMoveSpherical(p);
            
        }

        public static void ForwardToSpherical(PFSSPoint p)
        {
            p.x = p.rawR;
            p.y = p.rawPhi;
            p.z = p.rawTheta;
        }

        /// <summary>
        /// Set data from cartesian to spherical coordinates
        /// </summary>
        /// <param name="data"></param>
        public static void ForwardToSpherical(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                    ForwardToSpherical(p);
            }
        }

    }
}
