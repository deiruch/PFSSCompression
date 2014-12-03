using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

namespace CompressionTesting.Transformation
{
    class DCTImprover
    {
        public static void AddExtraPoints(PFSSData data, double factor)
        {
            foreach (PFSSLine l in data.lines)
            {
                l.extra = new ExtraPoints[3];
                int length = l.points.Count;
                double rise = GetRiseX(l, 0, 20);
                float[] start = HandleCurve(l.points[0].x,rise,factor,length);
                rise = -GetRiseX(l, l.points.Count - 21, l.points.Count-1);
                float[] end = HandleCurve(l.points[length - 1].x, rise, factor, length);
                Array.Reverse(start);
                ExtraPoints e = new ExtraPoints();
                e.start = start;
                e.startLength = start.Length;
                e.end = end;
                e.endLength = end.Length;
                l.extra[0] =e;


                rise = GetRiseY(l, 0, 20);
                start = HandleCurve(l.points[0].y, rise, factor, length);
                rise = -GetRiseY(l, l.points.Count - 21, l.points.Count - 1);
                end = HandleCurve(l.points[length - 1].y, rise, factor, length);
                Array.Reverse(start);
                e = new ExtraPoints();
                e.start = start;
                e.startLength = start.Length;
                e.end = end;
                e.endLength = end.Length;
                l.extra[1] = e;

                rise = GetRiseZ(l, 0, 20);
                start = HandleCurve(l.points[0].z, rise, factor, length);
                rise = -GetRiseZ(l, l.points.Count - 21, l.points.Count - 1);
                end = HandleCurve(l.points[length - 1].z, rise, factor, length);
                Array.Reverse(start);
                e = new ExtraPoints();
                e.start = start;
                e.startLength = start.Length;
                e.end = end;
                e.endLength = end.Length;
                l.extra[2] = e;
            }
        }

        public static float[] HandleCurve(float p0, double rise, double factor, int length)
        {
            
            //double factor = Math.Abs(rise / divider);

            //factor = Math.Sign(rise) < 0 ? factor : -factor;
            //(int)Math.Ceiling(Math.Abs(rise / factor))
            int proposedLen = (int)(Math.Abs(rise / factor)) ;
            int len = proposedLen > length / 5 ? length / 5 : proposedLen;
            float[] output = new float[len];
            float oldPoint = p0;
            //float diff = (float)factor;

            for (int i = 0; i < output.Length; i++)
            {
                //rise = (float)(rise + diff);
                //diff += (float)factor;
                var f = 1-i / (float)(output.Length);
                float cos = (float)Math.Cos(Math.PI * f);
                float r = (float)(rise * (1 - cos * 0.5-0.5));
                output[i] = oldPoint - r;
                oldPoint = output[i];
            }

            return output;
        }

        public static double GetRiseX(PFSSLine l,int startIndex,int endIndex) {
            if(l.points.Count/5 <= (endIndex-startIndex)) {
                if(startIndex == 0) 
                {
                    return -l.points[startIndex].x + l.points[startIndex + 1].x;
                }
                else
                {
                    return -l.points[endIndex-1].x + l.points[endIndex].x;
                }
                    
            }

            float[] r = new float[endIndex-startIndex];

            for (int i = startIndex; i < endIndex; i++)
            {
                r[i - startIndex] = -l.points[i].x + l.points[i + 1].x;
            }

            return r[r.Length/2];
        }

        public static double GetRiseY(PFSSLine l, int startIndex, int endIndex)
        {
            if (l.points.Count / 5 <= (endIndex - startIndex))
            {
                if (startIndex == 0)
                {
                    return -l.points[startIndex].x + l.points[startIndex + 1].x;
                }
                else
                {
                    return -l.points[endIndex - 1].x + l.points[endIndex].x;
                }

            }
            float[] r = new float[endIndex - startIndex];

            for (int i = startIndex; i < endIndex; i++)
            {
                r[i - startIndex] = -l.points[i].y + l.points[i + 1].y;
            }

            return r[r.Length / 2];
        }

        public static double GetRiseZ(PFSSLine l, int startIndex, int endIndex)
        {
            if(l.points.Count/5 <= (endIndex-startIndex)) {
                if(startIndex == 0) 
                {
                    return -l.points[startIndex].x + l.points[startIndex + 1].x;
                }
                else
                {
                    return -l.points[endIndex-1].x + l.points[endIndex].x;
                }
                    
            }
            float[] r = new float[endIndex - startIndex];

            for (int i = startIndex; i < endIndex; i++)
            {
                r[i - startIndex] = -l.points[i].z + l.points[i + 1].z;
            }

            return r[r.Length / 2];
        }

    }
    internal class ExtraPoints
    {
        internal int startLength;
        internal int endLength;
        internal float[] start;
        internal float[] end;
    }
}
