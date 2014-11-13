using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.PFSS
{
    class SimpleLineConstructor
    {
        public static readonly double ANGLE_OF_LOD = Math.Cos(5.0 / 180 * Math.PI);
        protected double l0 { get; private set; }
        protected double b0 { get; private set; }

        protected short[] ptr { get; private set; }
        protected short[] ptr_nz_len { get; private set; }
        protected short[] ptph { get; private set; }
        protected short[] ptth { get; private set; }
        private bool withQuantization = false;

        public SimpleLineConstructor(double l0, double b0, short[] ptr, short[] ptr_nz_len, short[] ptph, short[] ptth,bool quantization)
        {
            this.withQuantization = quantization;
            this.l0 = l0;
            this.b0 = b0;
            this.ptr = ptr;
            this.ptr_nz_len = ptr_nz_len;
            this.ptph = ptph;
            this.ptth = ptth;
        }

        protected TYPE getType(int startLine, int lineEnd)
        {
            if (ptr[startLine] > 8192 * 1.05)
                return TYPE.OUTSIDE_TO_SUN;
            else if (ptr[lineEnd] > 8192 * 1.05)
                return TYPE.SUN_TO_OUTSIDE;
            else
                return TYPE.SUN_TO_SUN;
        }

        public PFSSData ConstructLines()
        {
            List<PFSSLine> lines = new List<PFSSLine>(ptr_nz_len.Length);
            
            int lineCounter = 0;
            int lineEnd = ptr_nz_len[0] - 1;
            int lineStart = 0;
            bool lineStarted = false;
            TYPE type = getType(lineStart, lineEnd);

            PFSSPoint lastPoint = new PFSSPoint(0, 0, 0, 0, 0);
            List<PFSSPoint> line = new List<PFSSPoint>((lineEnd + 1 - lineStart) >> 2);
            
            for (int i = 0; i < ptr.Length; i++)
            {
                if (i > lineEnd && lineCounter < ptr_nz_len.Length)
                {
                    i = lineEnd;
                }

                bool colinear = false;

                PFSSPoint current = new PFSSPoint(ptr[i], ptph[i], ptth[i], l0, b0);
                
                //if there is a next point, see if it 
                if (lineStarted && i + 1 < ptr.Length)
                {
                    PFSSPoint next = new PFSSPoint(ptr[i+1], ptph[i+1], ptth[i+1], l0, b0);
                   
                    if(this.withQuantization)
                        colinear = current.AngleTo(next, lastPoint) > ANGLE_OF_LOD && i != lineEnd;
                }
                else
                {
                    lineStarted = true;
                    //lastPoint = current;
                }

                if (!colinear)
                {
                    if (i != lineEnd)
                    {
                        lastPoint = current;
                    }

                    line.Add(current);
                }

                if (i == lineEnd)
                {
                    lineStarted = false;
                    lines.Add(new PFSSLine(type, line));
                    lineStart = lineEnd + 1;
                    if (lineCounter < this.ptr_nz_len.Length) {
                        lineEnd += this.ptr_nz_len[lineCounter++];
                        line = new List<PFSSPoint>((lineEnd + 1 - lineStart) >> 2);
                    }
                    type = getType(lineStart, lineEnd);
                }
            }


            return new PFSSData(b0,l0,lines);
        }
    }
}
