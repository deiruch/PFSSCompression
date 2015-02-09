using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressedPFSSManager.PFSS
{
    class PFSSPoint
    {
        public const double SunRadius = 6.957e8;

        internal float x { get; set; }
        internal float y { get; set; }
        internal float z { get; set; }


        public PFSSPoint(float rawR, float rawPhi, float rawTheta, double l0, double b0)
        {
            double r = rawR * SunRadius;
            double phi = rawPhi * 2 * Math.PI;
            double theta = rawTheta * 2 * Math.PI;

            short discR = (short)Math.Round(rawR * 8192);
            short discP = (short)Math.Round(rawPhi * 32768.0);
            short discT = (short)Math.Round(rawTheta * 32768.0);


            this.x = discR;
            this.y = discP;
            this.z = discT;
        }


        public PFSSPoint(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        public double AngleTo(PFSSPoint next,PFSSPoint before)
        {
            return calculateAngleBetween2Vecotrs(next.x - x,
                                                    next.y - y, next.z - z, x - before.x, y - before.y, z
                                                                                 - before.z);
        }


        private static double calculateAngleBetween2Vecotrs(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            return (x1 * x2 + y1 * y2 + z1 * z2)
                                         / (Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1) * Math.Sqrt(x2 * x2
                                                                     + y2 * y2 + z2 * z2));
        }

        
    }
}
