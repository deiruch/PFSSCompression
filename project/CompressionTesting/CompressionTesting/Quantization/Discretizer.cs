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
        public static void HandleR(PFSSData data, int index, double factor) 
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint p = l.points[index];
                if (p.x < factor)
                {
                    p.x = 0;
                }
            }
        }

        public static void ToShorts(PFSSData data,int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                for(int i = offset; i < l.points.Count;i++)
                {
                    PFSSPoint p = l.points[i];
                    p.x = (short)Math.Truncate(p.x);
                    p.y = (short)Math.Truncate(p.y);
                    p.z = (short)Math.Truncate(p.z);
                }
            }
        }


        public static void ToInt(PFSSData data, int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                for (int i = offset; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
                    p.x = (int)Math.Truncate(p.x);
                    p.y = (int)Math.Truncate(p.y);
                    p.z = (int)Math.Truncate(p.z);
                }
            }
        }

        public static void DividePoint(PFSSData data, double factor, int index)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint p = l.points[index];
                p.x = (float)(p.x / factor);
                p.y = (float)(p.y / factor);
                p.z = (float)(p.z / factor);
            }
        }
        public static void MultiplyPoint(PFSSData data, double factor, int index)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint p = l.points[index];
                p.x = (float)(p.x * factor);
                p.y = (float)(p.y * factor);
                p.z = (float)(p.z * factor);
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

        public static void DivideLinear(PFSSData data, double start,double increase, int offset,int length)
        {
            foreach (PFSSLine l in data.lines)
            {
                double div = start;
                for (int i = offset; i < offset+length && i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
                    p.x = (float)(p.x / div);
                    p.y = (float)(p.y / div);
                    p.z = (float)(p.z / div);
                    div += increase;
                }
            }
        }


        public static void MultiplyLinear(PFSSData data, double start, double increase, int offset, int length)
        {
            foreach (PFSSLine l in data.lines)
            {
                double div = start;
                for (int i = offset; i < offset + length && i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
                    p.x = (float)(p.x * div);
                    p.y = (float)(p.y * div);
                    p.z = (float)(p.z * div);
                    div += increase;
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

        public static void Cut(PFSSData data, int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                for (int i = offset; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
                    p.x = 0;
                    p.y = 0;
                    p.z = 0;
                }
            }

        }


    }
}
