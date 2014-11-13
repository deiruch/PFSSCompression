using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

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

        public static void QuantizeRepeat(PFSSData data, int start)
        {
            foreach (PFSSLine l in data.lines)
            {
                for (int i = start; i < l.points.Count; i++)
                {
                    l.points[i].x = Math.Sign(l.points[i].x) * Math.Abs(l.points[start - 1].x);
                    l.points[i].y = Math.Sign(l.points[i].y) * Math.Abs(l.points[start - 1].y);
                    l.points[i].z = Math.Sign(l.points[i].z) * Math.Abs(l.points[start - 1].z);

                }
            }
        }

        public static void Discretize(PFSSData data, int start)
        {
            foreach (PFSSLine l in data.lines)
            {
                for (int i = start; i < l.points.Count; i++)
                {
                    l.points[i].x = (float)(Math.Round(l.points[i].x / 1000) * 1000);
                    l.points[i].y = (float)(Math.Round(l.points[i].y / 1000) * 1000);
                    l.points[i].z = (float)(Math.Round(l.points[i].z / 1000) * 1000);
                }
            }
        }

        public static void Quantize(PFSSData data, int start)
        {
            foreach (PFSSLine l in data.lines)
            {
                for (int i = start; i < l.points.Count; i++)
                {
                    l.points[i].x = 0;
                    l.points[i].y = 0;
                    l.points[i].z = 0;

                }
            }
        }
    }
}
