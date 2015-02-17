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

        internal float Radius { get; set; }
        internal float Phi { get; set; }
        internal float Theta { get; set; }

        public PFSSPoint(float x, float y, float z)
        {
            this.Radius = x;
            this.Phi = y;
            this.Theta = z;
        }


        public double AngleTo(PFSSPoint next,PFSSPoint before)
        {
            return calculateAngleBetween2Vecotrs(next.Radius - Radius,
                                                    next.Phi - Phi, next.Theta - Theta, Radius - before.Radius, Phi - before.Phi, Theta
                                                                                 - before.Theta);
        }


        private static double calculateAngleBetween2Vecotrs(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            return (x1 * x2 + y1 * y2 + z1 * z2)
                                         / (Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1) * Math.Sqrt(x2 * x2
                                                                     + y2 * y2 + z2 * z2));
        }

        
    }
}
