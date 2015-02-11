using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressedPFSSManager.PFSS
{
    /// <summary>
    /// Creates the PFSSData structure containing points in the spherical coordinate system
    /// </summary>
    class SphericalDataCreator
    {
        public static readonly double ANGLE_OF_LOD = Math.Cos(5.0 / 180 * Math.PI);
        protected double l0 { get; private set; }
        protected double b0 { get; private set; }
        protected short[] ptr { get; private set; }
        protected short[] ptr_nz_len { get; private set; }
        protected short[] ptph { get; private set; }
        protected short[] ptth { get; private set; }

        public SphericalDataCreator(double l0, double b0, short[] ptr, short[] ptr_nz_len, short[] ptph, short[] ptth)
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

        public PFSSData Create()
        {
            List<PFSSLine> lines = new List<PFSSLine>(ptr_nz_len.Length);

            int lineEnd = ptr_nz_len[0] - 1;
            int lineStart = 0;

            TYPE type = getType(lineStart, lineEnd);

            int vertexIndex = 0;
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                int lineSize = ptr_nz_len[i];
                List<PFSSPoint> line = new List<PFSSPoint>(lineSize);
                TYPE t = getType(vertexIndex, vertexIndex + lineSize - 1);

                int maxSize = vertexIndex + lineSize;
                int index = 0;
                for (; vertexIndex < maxSize; vertexIndex++)
                {
                    PFSSPoint current = new PFSSPoint(ptr[vertexIndex], ptph[vertexIndex], ptth[vertexIndex], l0, b0);
                    line.Add(current);
                    index++;
                }

                PFSSLine l = new PFSSLine(t, line);
                lines.Add(l);
            }

            return new PFSSData(l0, b0,lines);
        }
    }
}
