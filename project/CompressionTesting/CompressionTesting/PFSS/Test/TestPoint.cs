using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.PFSS.Test
{
    class TestPoint
    {
        public const double SunRadius = 6.957e8;
        private int index;
        internal float x { get; private set; }
        internal float y { get; private set; }
        internal float z { get; private set; }

        internal PFSSPoint point { get; private set; }

        public TestPoint(int index, float rawR, float rawPhi, float rawTheta, double l0, double b0)
        {
            this.index = index;

            short discR = (short)Math.Round(rawR*8192);
            short discP = (short)Math.Round(rawPhi * 32768.0);
            short discT = (short)Math.Round(rawTheta * 32768.0);
            this.point = new PFSSPoint(index, discR, discP, discT);

            double r = rawR * SunRadius;
            double phi = rawPhi * 2 * Math.PI;
            double theta = rawTheta * 2 * Math.PI;

            //current point
            phi -= l0 / 180.0 * Math.PI;
            theta += b0 / 180.0 * Math.PI;
            z = (float)(r * Math.Sin(theta) * Math.Cos(phi));
            x = (float)(r * Math.Sin(theta) * Math.Sin(phi));
            y = (float)(r * Math.Cos(theta));
        }

        public PFSSPoint GetResettedPoint()
        {
            point.x = this.x;
            point.y = this.y;
            point.z = this.z;

            return this.point;
        }
    }
}
