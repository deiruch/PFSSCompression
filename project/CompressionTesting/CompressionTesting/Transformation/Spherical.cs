using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

namespace CompressionTesting.Transformation
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

        public static void ForwardMoveSpherical(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
                foreach (PFSSPoint p in l.points)
                    ForwardMoveSpherical(p);
            
        }

        public static void BackwardMoveSpherical(PFSSPoint p)
        {
            p.x = (p.x+ 8192f);
            p.y = (p.y + 16384f);
            p.z = (p.z + 8192f);
        }
        public static void BackwardMoveSpherical(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                    BackwardMoveSpherical(p);
            }
        }

        public static void ForwardToSpherical(PFSSPoint p)
        {
            p.x = p.rawR;
            p.y = p.rawPhi;
            p.z = p.rawTheta;
        }

        public static void ForwardToSpherical(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                    ForwardToSpherical(p);
            }
        }

        public static void BackwardToSpherical(PFSSPoint p, PFSSData data)
        {
            double r = p.x / 8192.0 * PFSSPoint.SunRadius;
            double phi = p.y / 32768.0 * 2 * Math.PI;
            double theta = p.z / 32768.0 * 2 * Math.PI;

            //current point
            phi -= data.l0 / 180.0 * Math.PI;
            theta += data.b0 / 180.0 * Math.PI;
            p.z = (float)(r * Math.Sin(theta) * Math.Cos(phi));
            p.x = (float)(r * Math.Sin(theta) * Math.Sin(phi));
            p.y = (float)(r * Math.Cos(theta));
        }

        public static void BackwardToSpherical(PFSSData data)
        {
            for (int i = 0; i < data.lines.Count;i++)
            {
                PFSSLine l = data.lines[i];
                for (int j = 0; j < l.points.Count;j++ )
                {
                    PFSSPoint p = l.points[j];
                    BackwardToSpherical(p, data);
                }
            }

        }
    }
}
