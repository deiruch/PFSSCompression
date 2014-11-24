using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

namespace CompressionTesting.Quantization
{
    class Discretizer
    {
        public static void ToShorts(PFSSData data,int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                for(int i = offset; i < l.points.Count;i++)
                {
                    PFSSPoint p = l.points[i];
                    p.x = (short)p.x;
                    p.y = (short)p.y;
                    p.z = (short)p.z;
                }
            }
        }

        public static void Divide(PFSSData data, double factor, int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                for(int i = offset; i < l.points.Count;i++)
                {
                    PFSSPoint p = l.points[i];
                    p.x = (float)(p.x / factor);
                    p.y = (float)(p.y / factor);
                    p.z = (float)(p.z / factor);
                }
            }
        }

        public static void Multiply(PFSSData data, double factor, int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                for (int i = offset; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
                    p.x = (float)(p.x * factor);
                    p.y = (float)(p.y * factor);
                    p.z = (float)(p.z * factor);
                }
            }
        }

        public static void DivideLinear(PFSSData data,double factor, int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                double div = factor;
                for (int i = offset; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
                    p.x = (float)(p.x / div);
                    p.y = (float)(p.y / div);
                    p.z = (float)(p.z / div);
                    div += factor;
                }
            }
        }

        public static void MultiplyLinear(PFSSData data, double factor, int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                double div = factor;
                for (int i = offset; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
                    p.x = (float)(p.x * div);
                    p.y = (float)(p.y * div);
                    p.z = (float)(p.z * div);
                    div += factor;
                }
            }
        }
    }
}
