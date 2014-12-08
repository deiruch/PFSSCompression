using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;
using System.IO;

namespace CompressionTesting
{
    class DCTransformer
    {
        public static void Forward(PFSSData data, int pointOffset)
        {
            foreach (PFSSLine l in data.lines)
            {
                float[] x = new float[l.points.Count - pointOffset];
                float[] y = new float[l.points.Count - pointOffset];
                float[] z = new float[l.points.Count - pointOffset];

                for (int i = pointOffset; i < l.points.Count; i++)
                {
                    PFSSPoint current = l.points[i];
                    x[i - pointOffset] = (float)current.x;
                    y[i - pointOffset] = (float)current.y;
                    z[i - pointOffset] = (float)current.z;
                }

                x = DCT.slow_fdct(x);
                y = DCT.slow_fdct(y);
                z = DCT.slow_fdct(z);

                for (int i = pointOffset; i < l.points.Count; i++)
                {
                    PFSSPoint current = l.points[i];
                    current.x = x[i - pointOffset];
                    current.y = y[i - pointOffset];
                    current.z = z[i - pointOffset];
                }
            }
        }

        public static void ForwardExtra(PFSSData data, int pointOffset)
        {
            foreach (PFSSLine l in data.lines)
            {
                float[] x = new float[l.points.Count - pointOffset + l.extra[0].startLength + l.extra[0].endLength];
                float[] y = new float[l.points.Count - pointOffset + l.extra[1].startLength + l.extra[1].endLength];
                float[] z = new float[l.points.Count - pointOffset + l.extra[2].startLength + l.extra[2].endLength];
                int iX= 0, iY = 0,iZ= 0;

                foreach (float f in l.extra[0].start)
                    x[iX++] = f;
                foreach (float f in l.extra[1].start)
                    y[iY++] = f;
                foreach (float f in l.extra[2].start)
                    z[iZ++] = f;
                    
                for (int i = pointOffset; i < l.points.Count; i++)
                {
                    PFSSPoint current = l.points[i];
                    x[iX++] = (float)current.x;
                    y[iY++] = (float)current.y;
                    z[iZ++] = (float)current.z;
                }

                foreach (float f in l.extra[0].end)
                    x[iX++] = f;
                foreach (float f in l.extra[1].end)
                    y[iY++] = f;
                foreach (float f in l.extra[2].end)
                    z[iZ++] = f;

                x = DCT.fdct(x);
                y = DCT.fdct(y);
                z = DCT.fdct(z);

                l.extraX = x;
                l.extraY = y;
                l.extraZ = z;

                /*iX = l.extra[0].startLength; iY = l.extra[1].startLength; iZ = l.extra[2].startLength;
                for (int i = pointOffset; i < l.points.Count; i++)
                {
                    PFSSPoint current = l.points[i];
                    current.x = x[iX++];
                    current.y = y[iY++];
                    current.z = z[iZ++];
                }*/
            }
        }

        public static void Backward(PFSSData data,int pointOffset)
        {
            foreach (PFSSLine l in data.lines)
            {
                float[] x = new float[l.points.Count - pointOffset];
                float[] y = new float[l.points.Count - pointOffset];
                float[] z = new float[l.points.Count - pointOffset];

                for (int i = pointOffset; i < l.points.Count; i++)
                {
                    PFSSPoint current = l.points[i];
                    x[i - pointOffset] = (float)current.x;
                    y[i - pointOffset] = (float)current.y;
                    z[i - pointOffset] = (float)current.z;
                }

                x = DCT.slow_idct(x);
                y = DCT.slow_idct(y);
                z = DCT.slow_idct(z);

                for (int i = pointOffset; i < l.points.Count; i++)
                {
                    PFSSPoint current = l.points[i];
                    current.x = x[i - pointOffset];
                    current.y = y[i - pointOffset];
                    current.z = z[i - pointOffset];
                }
            }
        }


        public static void BackwardExtra(PFSSData data, int pointOffset)
        {
            foreach (PFSSLine l in data.lines)
            {
                l.extraX = DCT.idct(l.extraX);
                l.extraY = DCT.idct(l.extraY);
                l.extraZ = DCT.idct(l.extraZ);
                int iX = l.extra[0].startLength, iY = l.extra[1].startLength, iZ = l.extra[2].startLength;

                iX += pointOffset; iY += pointOffset; iZ += pointOffset;
                for (int i = pointOffset; i < l.points.Count; i++)
                {
                    PFSSPoint current = l.points[i];
                    current.x = l.extraX[iX++];
                    current.y = l.extraY[iY++];
                    current.z = l.extraZ[iZ++];
                }
            }
        }
    }
}
