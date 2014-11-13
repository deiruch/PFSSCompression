using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.PFSS
{
    class PFSSPoint
    {
        public const double SunRadius = 6.957e8;
        private double l0;
        private double b0;

        internal int testPointIndex;
        internal float x { get; set; }
        internal float y { get; set; }
        internal float z { get; set; }

        internal short rawR { get; private set; }
        internal short rawPhi { get; private set; }
        internal short rawTheta { get; private set; }

        public PFSSPoint(short rawR, short rawPhi, short rawTheta, double l0, double b0)
        {
            this.l0 = l0;
            this.b0 = b0;
            this.rawR = rawR;
            this.rawPhi = rawPhi;
            this.rawTheta = rawTheta;

            this.Reset();
        }

        public PFSSPoint(PFSSPoint p)
        {
            this.rawPhi = p.rawPhi;
            this.rawR = p.rawR;
            this.rawTheta = p.rawTheta;
            this.x = p.x;
            this.y = p.y;
            this.z = p.z;
        }

        public PFSSPoint(float rawR, float rawPhi, float rawTheta, double l0, double b0)
        {
            double r = rawR;
            double phi = rawPhi * 2 * Math.PI;
            double theta = rawTheta * 2 * Math.PI;

            //current point
            phi -= l0 / 180.0 * Math.PI;
            theta += b0 / 180.0 * Math.PI;
            z = (float)(r * Math.Sin(theta) * Math.Cos(phi));
            x = (float)(r * Math.Sin(theta) * Math.Sin(phi));
            y = (float)(r * Math.Cos(theta));
        }

        public PFSSPoint(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Reset()
        {
            double r = rawR / 8192.0;
            double phi = rawPhi / 32768.0 * 2 * Math.PI;
            double theta = rawTheta / 32768.0 * 2 * Math.PI;

            //current point
            phi -= l0 / 180.0 * Math.PI;
            theta += b0 / 180.0 * Math.PI;
            z = (float)(r * Math.Sin(theta) * Math.Cos(phi));
            x = (float)(r * Math.Sin(theta) * Math.Sin(phi));
            y = (float)(r * Math.Cos(theta));
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

        public double getDistanceTo(PFSSPoint p)
        {
            PFSSPoint vec = getVector(this, p);
            return vec.magnitude();
        }

        public double magnitude()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public static PFSSPoint getVector(PFSSPoint start, PFSSPoint end)
        {
            return new PFSSPoint(end.x - start.x, end.y - start.y, end.z - start.z);
        }

        public static double dot(PFSSPoint p0, PFSSPoint p1)
        {
            return p0.x * p1.x + p0.y * p1.y + p0.z * p1.z;
        }

        public static PFSSPoint cross(PFSSPoint p0, PFSSPoint p1)
        {
            float x = p0.y * p1.z - p0.z * p1.y;
            float y = p0.z * p1.x - p0.x * p1.z;
            float z = p0.x * p1.y - p0.y * p1.x;
            return new PFSSPoint(x, y, z);
        }
    }
}
