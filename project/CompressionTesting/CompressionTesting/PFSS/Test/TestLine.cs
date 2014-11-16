using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.PFSS.Test
{
    class TestLine
    {
        internal List<TestPoint> points { get; private set; }
        internal TYPE Type { get; private set; }

        public TestLine(TYPE t, List<TestPoint> points)
        {
            this.points = points;
            this.Type = t;
        }

        public PFSSLine GetPFSSLine()
        {
            List<PFSSPoint> newPoints = new List<PFSSPoint>(points.Count);
            foreach (TestPoint p in points)
            {
                newPoints.Add(p.GetResettedPoint());
            }

            return new PFSSLine(Type, newPoints);
        }
    }
}
