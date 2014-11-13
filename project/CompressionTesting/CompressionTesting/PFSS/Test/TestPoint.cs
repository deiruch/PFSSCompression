using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.PFSS.Test
{
    class TestPoint
    {
        private int index;
        internal float x { get; private set; }
        internal float y { get; private set; }
        internal float z { get; private set; }

        internal PFSSPoint point { get; private set; }

        public TestPoint(int index, float rawR, float rawPhi, float rawTheta, double l0, double b0)
        {
            this.index = index;
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

        public void SetPoint(PFSSPoint p)
        {
            this.point = p;
            p.testPointIndex = index;
        }
    }
}
