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

        public float[] CopyChannel(int channel, PFSSLine line)
        {
            float[] answer = new float[line.points.Count];
            for (int i = 0; i < answer.Length; i++)
            {
                TestPoint p = points[line.points[i].testPointIndex];
                if (channel == 0)
                    answer[i] = p.x;
                if (channel == 1)
                    answer[i] = p.y;
                if (channel == 2)
                    answer[i] = p.z;
            }

            return answer;
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
