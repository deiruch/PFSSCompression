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

        public void SetData(PFSSLine line)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].SetPoint(line.points[i]);
            }
        }
    }
}
