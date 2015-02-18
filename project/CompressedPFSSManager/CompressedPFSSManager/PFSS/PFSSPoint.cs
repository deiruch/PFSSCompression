using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressedPFSSManager.PFSS
{
    class PFSSPoint
    {
        public const double SUN_RADIUS = 6.957e8;

        internal float X;
        internal float Y;
        internal float Z;

        public static PFSSPoint operator-(PFSSPoint _lhs,PFSSPoint _rhs)
        {
            return new PFSSPoint(_lhs.X-_rhs.X,_lhs.Y-_rhs.Y,_lhs.Z-_rhs.Z);
        }

        public static PFSSPoint operator +(PFSSPoint _lhs, PFSSPoint _rhs)
        {
            return new PFSSPoint(_lhs.X + _rhs.X, _lhs.Y + _rhs.Y, _lhs.Z + _rhs.Z);
        }

        public override string ToString()
        {
            return string.Format("x={0:F2} y={1:F2} z={2:F2}", X, Y, Z);
        }

        public static PFSSPoint operator *(PFSSPoint _lhs, double _factor)
        {
            return new PFSSPoint((float)(_lhs.X * _factor), (float)(_lhs.Y * _factor), (float)(_lhs.Z * _factor));
        }

        public static PFSSPoint operator *(double _factor, PFSSPoint _lhs)
        {
            return new PFSSPoint((float)(_lhs.X * _factor), (float)(_lhs.Y * _factor), (float)(_lhs.Z * _factor));
        }

        public PFSSPoint(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }


        public double AngleBetween(PFSSPoint next,PFSSPoint before)
        {
            return calculateAngleBetween2Vecotrs(next.X - X,
                                                    next.Y - Y, next.Z - Z, X - before.X, Y - before.Y, Z
                                                                                 - before.Z);
        }


        private static double calculateAngleBetween2Vecotrs(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            return (x1 * x2 + y1 * y2 + z1 * z2)
                                         / (Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1) * Math.Sqrt(x2 * x2
                                                                     + y2 * y2 + z2 * z2));
        }

        
    }
}
