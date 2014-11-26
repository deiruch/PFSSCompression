using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.PFSS.Test
{
    class TestSuiteConstructor
    {
        public static readonly double ANGLE_OF_LOD = Math.Cos(5.0 / 180 * Math.PI);
        protected double l0 { get; private set; }
        protected double b0 { get; private set; }
        protected float[] ptr { get; private set; }
        protected short[] ptr_nz_len { get; private set; }
        protected float[] ptph { get; private set; }
        protected float[] ptth { get; private set; }

        public TestSuiteConstructor(double l0, double b0, float[] ptr, short[] ptr_nz_len, float[] ptph, float[] ptth)
        {
            this.l0 = l0;
            this.b0 = b0;
            this.ptr = ptr;
            this.ptr_nz_len = ptr_nz_len;
            this.ptph = ptph;
            this.ptth = ptth;
        }

        protected TYPE getType(int startLine, int lineEnd)
        {
            //error: somewhere between IDL and Fits there is something which eats the last few floats, but only floats!
            if (lineEnd > ptr.Length)
            {
                lineEnd = ptr.Length - 1;
            }

            if (ptr[startLine] > 1 * 1.05)
                return TYPE.OUTSIDE_TO_SUN;
            else if (ptr[lineEnd] > 1 * 1.05)
                return TYPE.SUN_TO_OUTSIDE;
            else
                return TYPE.SUN_TO_SUN;
        }

        public TestSuite ConstructSuite()
        {
            List<TestLine> lines = new List<TestLine>(ptr_nz_len.Length);

            int lineCounter = 0;
            int lineEnd = ptr_nz_len[0] - 1;
            int lineStart = 0;
            bool lineStarted = false;
            TYPE type = getType(lineStart, lineEnd);

            int vertexIndex = 0;
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                int lineSize = ptr_nz_len[i];
                List<TestPoint> line = new List<TestPoint>(lineSize);
                TYPE t = getType(vertexIndex, vertexIndex + lineSize - 1);

                int maxSize = vertexIndex + lineSize;
                int index = 0;
                for (; vertexIndex < maxSize; vertexIndex++)
                {
                    TestPoint current = new TestPoint(index, ptr[vertexIndex], ptph[vertexIndex],ptth[vertexIndex], l0, b0);
                    line.Add(current);
                    index++;
                }

                TestLine l = new TestLine(t, line);
                lines.Add(l);
            }

            return new TestSuite(lines, l0, b0);
        }
    }
}
