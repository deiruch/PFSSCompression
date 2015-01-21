using CompressionTesting.PFSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.Transformation
{
    class LinearPredictor
    {
        public static void Forward(PFSSData data, int offset = 1)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint a = l.points[offset-1];
                for (int i = offset; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
                    PFSSPoint pCopy = new PFSSPoint(p);

                    p.x = a.x - p.x;
                    p.y = a.y - p.y;
                    p.z =  a.z - p.z;
                    a = pCopy;
                }
            }
        }
        public static void Backward(PFSSData data, int offset = 1)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint a = l.points[0];
                for (int i = offset; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];

                    p.x = a.x - p.x;
                    p.y = a.y - p.y;
                    p.z = a.z - p.z;
                    a = p;
                }
            }
        }

        public static void ForwardAdaptive(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                float lastErrorX = 0;
                float lastErrorY = 0;
                float lastErrorZ = 0;

                PFSSPoint a = l.points[0];
                for (int i = 1; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
                    PFSSPoint pCopy = new PFSSPoint(p);

                    p.x = a.x - p.x+lastErrorX;
                    lastErrorX = p.x;

                    p.y = a.y - p.y+lastErrorY;
                    lastErrorY = p.y;

                    p.z = a.z - p.z+lastErrorZ;
                    lastErrorZ = p.z;

                    a = pCopy;
                }
            }
        }

        public static void BackwardAdaptive(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                float lastErrorX = 0;
                float lastErrorY = 0;
                float lastErrorZ = 0;

                PFSSPoint a = l.points[0];
                for (int i = 1; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
                    float errCopy = 0;

                    errCopy = p.x;
                    p.x = a.x - p.x+lastErrorX;
                    lastErrorX = errCopy;

                    errCopy = p.y;
                    p.y = a.y - p.y + lastErrorY;
                    lastErrorY = errCopy;

                    errCopy = p.z;
                    p.z = a.z - p.z + lastErrorZ;
                    lastErrorZ = errCopy;
                    a = p;
                }
            }
        }

        public static void AdaptiveMovAveragePredictorForwards(PFSSData data, int numberOfAv)
        {
            foreach (PFSSLine l in data.lines)
            {
                MovingAverage averageRiseX = new MovingAverage(numberOfAv);
                MovingAverage averageRiseY = new MovingAverage(numberOfAv);
                MovingAverage averageRiseZ = new MovingAverage(numberOfAv);
                float lastErrorX = 0;
                float lastErrorY = 0;
                float lastErrorZ = 0;

                PFSSPoint a = l.points[0];
                PFSSPoint b = l.points[1];
                averageRiseX.Add(-a.x + b.x);
                averageRiseY.Add(-a.y + b.y);
                averageRiseZ.Add(-a.z + b.z);
                int divider = 1;

                for (int i = 2; i < l.points.Count; i++)
                {
                    float avX = (short)averageRiseX.GetAverage();
                    float avY = (short)averageRiseY.GetAverage();
                    float avZ = (short)averageRiseZ.GetAverage();
                    PFSSPoint p = l.points[i];
                    PFSSPoint pCopy = new PFSSPoint(p);

                    p.x = b.x + avX - p.x +lastErrorX;
                    lastErrorX -= p.x;

                    p.y = b.y + avY - p.y+ lastErrorY;
                    lastErrorY -= p.y;

                    p.z = b.z + avZ - p.z+ lastErrorZ;
                    lastErrorZ -= p.z;
                    //update average
                    averageRiseX.Add(-b.x + pCopy.x);
                    averageRiseY.Add(-b.y + pCopy.y);
                    averageRiseZ.Add(-b.z + pCopy.z);
                    b = pCopy;
                    divider++;
                }
            }
        }

        public static void AdaptiveMovAveragePredictorBackwards(PFSSData data, int numberOfAv)
        {
            foreach (PFSSLine l in data.lines)
            {
                MovingAverage averageRiseX = new MovingAverage(numberOfAv);
                MovingAverage averageRiseY = new MovingAverage(numberOfAv);
                MovingAverage averageRiseZ = new MovingAverage(numberOfAv);
                float lastErrorX = 0;
                float lastErrorY = 0;
                float lastErrorZ = 0;

                PFSSPoint a = l.points[0];
                PFSSPoint b = l.points[1];
                averageRiseX.Add(-a.x + b.x);
                averageRiseY.Add(-a.y + b.y);
                averageRiseZ.Add(-a.z + b.z);
                int divider = 1;

                for (int i = 2; i < l.points.Count; i++)
                {
                    float avX = (short)averageRiseX.GetAverage();
                    float avY = (short)averageRiseY.GetAverage();
                    float avZ = (short)averageRiseZ.GetAverage();
                    PFSSPoint p = l.points[i];

                    float errCopy = p.x;
                    p.x = b.x + avX - p.x+lastErrorX;
                    lastErrorX -= errCopy;

                    errCopy = p.y;
                    p.y = b.y + avY - p.y+lastErrorY;
                    lastErrorY -= errCopy;

                    errCopy = p.z;
                    p.z = b.z + avZ - p.z+lastErrorZ;
                    lastErrorZ -= errCopy;
                    //update average
                    averageRiseX.Add(-b.x + p.x);
                    averageRiseY.Add(-b.y + p.y);
                    averageRiseZ.Add(-b.z + p.z);
                    b = p;
                    divider++;
                }
            }
        }

        public static void MovAveragePredictorForwards(PFSSData data, int numberOfAv)
        {
            foreach (PFSSLine l in data.lines)
            {
                MovingAverage averageRiseX = new MovingAverage(numberOfAv);
                MovingAverage averageRiseY = new MovingAverage(numberOfAv);
                MovingAverage averageRiseZ = new MovingAverage(numberOfAv);
                float lastErrorX = 0;
                float lastErrorY = 0;
                float lastErrorZ = 0;

                PFSSPoint a = l.points[0];
                PFSSPoint b = l.points[1];
                averageRiseX.Add(-a.x + b.x);
                averageRiseY.Add(-a.y + b.y);
                averageRiseZ.Add(-a.z + b.z);

                for (int i = 2; i < l.points.Count; i++)
                {
                    float avX = (short)averageRiseX.GetAverage();
                    float avY = (short)averageRiseY.GetAverage();
                    float avZ = (short)averageRiseZ.GetAverage();

                    PFSSPoint p = l.points[i];
                    PFSSPoint pCopy = new PFSSPoint(p);

                    p.x = b.x + avX - p.x;
                    lastErrorX -= p.x;

                    p.y = b.y + avY - p.y;
                    lastErrorY -= p.y;

                    p.z = b.z + avZ - p.z;
                    lastErrorZ -= p.z;
                    //update average
                    averageRiseX.Add(-b.x + pCopy.x);
                    averageRiseY.Add(-b.y + pCopy.y);
                    averageRiseZ.Add(-b.z + pCopy.z);
                    b = pCopy;
                }
            }
        }

        public static void MovAveragePredictorBackwards(PFSSData data, int numberOfAv)
        {
            foreach (PFSSLine l in data.lines)
            {
                MovingAverage averageRiseX = new MovingAverage(numberOfAv);
                MovingAverage averageRiseY = new MovingAverage(numberOfAv);
                MovingAverage averageRiseZ = new MovingAverage(numberOfAv);
                float lastErrorX = 0;
                float lastErrorY = 0;
                float lastErrorZ = 0;

                PFSSPoint a = l.points[0];
                PFSSPoint b = l.points[1];
                averageRiseX.Add(-a.x + b.x);
                averageRiseY.Add(-a.y + b.y);
                averageRiseZ.Add(-a.z + b.z);
                int divider = 1;

                for (int i = 2; i < l.points.Count; i++)
                {
                    float avX = (short)averageRiseX.GetAverage();
                    float avY = (short)averageRiseY.GetAverage();
                    float avZ = (short)averageRiseZ.GetAverage();
                    PFSSPoint p = l.points[i];

                    float errCopy = p.x;
                    p.x = b.x + avX - p.x;
                    lastErrorX -= errCopy;

                    errCopy = p.y;
                    p.y = b.y + avY - p.y;
                    lastErrorY -= errCopy;

                    errCopy = p.z;
                    p.z = b.z + avZ - p.z;
                    lastErrorZ -= errCopy;
                    //update average
                    averageRiseX.Add(-b.x + p.x);
                    averageRiseY.Add(-b.y + p.y);
                    averageRiseZ.Add(-b.z + p.z);
                    b = p;
                    divider++;
                }
            }
        }


        public static void QuadForward(PFSSData data, int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint last = null;
                PFSSPoint last1 = null;
                for (int i = 1; i < l.points.Count; i++)
                {
                    PFSSPoint current = l.points[i];
                    PFSSPoint currentCopy = new PFSSPoint(current);
                    if (last != null && last1 != null)
                    {
                        float newX = last.x + last.x - last1.x;
                        float newY = last.y + last.y - last1.y;
                        float newZ = last.z + last.z - last1.z;
                        if (Math.Sign(newX) != Math.Sign(last.x))
                            current.x = last.x - current.x;
                        else 
                            current.x = last.x + last.x - last1.x - current.x;

                        if (Math.Sign(newY) != Math.Sign(last.y))
                            current.y = last.y - current.y;
                        else 
                            current.y = last.y + last.y - last1.y - current.y;

                        if (Math.Sign(newZ) != Math.Sign(last.z))
                            current.z = last.z - current.z;
                        else 
                            current.z = last.z + last.z - last1.z - current.z;
                    }
                    last1 = last;
                    last = currentCopy;
                }
            }
        }

        public static void QuadBackward(PFSSData data)
        {

            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint last = null;
                PFSSPoint last1 = null;
                for (int i = 1; i < l.points.Count; i++)
                {
                    if (i == 10)
                    {
                        System.Console.Write("");
                    }
                    PFSSPoint current = l.points[i];
                    if (last != null && last1 != null)
                    {
                        float newX = last.x + last.x - last1.x;
                        float newY = last.y + last.y - last1.y;
                        float newZ = last.z + last.z - last1.z;
                        if (Math.Sign(newX) != Math.Sign(last.x))
                            current.x = last.x - current.x;
                        else
                            current.x = last.x + last.x - last1.x - current.x;

                        if (Math.Sign(newY) != Math.Sign(last.y))
                            current.y = last.y - current.y;
                        else
                            current.y = last.y + last.y - last1.y - current.y;

                        if (Math.Sign(newZ) != Math.Sign(last.z))
                            current.z = last.z - current.z;
                        else
                            current.z = last.z + last.z - last1.z - current.z;
                    }
                    last1 = last;
                    last = current;
                }
            }
        }

        public static void ToShorts(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                float errorX = 0;
                float errorY = 0;
                float errorZ = 0;
                for (int i = 0; i < l.points.Count; i++)
                {
                    l.points[i].x += errorX;
                    errorX = l.points[i].x - (short)Math.Round(l.points[i].x);
                    l.points[i].x = (short)Math.Round(l.points[i].x);

                    l.points[i].y += errorY;
                    errorY = l.points[i].y - (short)Math.Round(l.points[i].y);
                    l.points[i].y = (short)Math.Round(l.points[i].y);

                    l.points[i].z += errorZ;
                    errorZ = l.points[i].z - (short)Math.Round(l.points[i].z);
                    l.points[i].z = (short)Math.Round(l.points[i].z);
                }
            }
        }

        public static void ToShortsHard(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                for (int i = 0; i < l.points.Count; i++)
                {
                    l.points[i].x = (short)(l.points[i].x);
                    l.points[i].y = (short)(l.points[i].y);
                    l.points[i].z = (short)(l.points[i].z);
                }
            }
        }
    }
}
