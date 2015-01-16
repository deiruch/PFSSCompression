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
        public static void Forward(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint a = l.points[0];
                PFSSPoint b = l.points[1];
                for (int i = 2; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
                    PFSSPoint pCopy = new PFSSPoint(p);

                    p.x = 2 * b.x - a.x - p.x;
                    p.y = 2 * b.y - a.y - p.y;
                    p.z = 2 * b.z - a.z - p.z;
                    a = b;
                    b = pCopy;
                }
            }
        }

        public static void Backward(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint a = l.points[0];
                PFSSPoint b = l.points[1];
                for (int i = 2; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];

                    p.x = b.x + b.x - a.x - p.x;
                    p.y = b.y + b.y - a.y - p.y;
                    p.z = b.z + b.z - a.z - p.z;
                    a = b;
                    b = p;
                }
            }
        }

        public static void ContinousForward(PFSSData data, int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint last =  new PFSSPoint(l.points[offset]);
                for (int i = offset+1; i < l.points.Count; i++)
                {
                    PFSSPoint current = l.points[i];
                    PFSSPoint pCopy = new PFSSPoint(current);

                    current.z = current.z - last.z;

                    last = pCopy;
                }
            }
        }

        public static void ContinousBackward(PFSSData data, int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint last = new PFSSPoint(l.points[offset]);
                for (int i = offset + 1; i < l.points.Count; i++)
                {
                    PFSSPoint current = l.points[i];

                    current.z = current.z + last.z;

                    last = current;
                }
            }
        }

        public static void BetterLinearPredictorForward(PFSSData data, int channel)
        {
            foreach (PFSSLine l in data.lines)
            {
                float averageRiseX;
                float averageRiseY;
                float averageRiseZ;
                float lastError = 0;

                PFSSPoint a = l.points[0];
                PFSSPoint b = l.points[1];
                averageRiseX = -a.x + b.x;
                averageRiseY = -a.y + b.y;
                averageRiseZ = -a.z + b.z;
                int divider = 1;

                for (int i = 2; i < l.points.Count; i++)
                {
                    float avX = averageRiseX / divider;
                    float avY = averageRiseY / divider;
                    float avZ = averageRiseZ / divider;
                    PFSSPoint p = l.points[i];
                    PFSSPoint pCopy = new PFSSPoint(p);

                    p.x = b.x + avX - p.x ;
                    p.y = b.y + avY - p.y;
                    p.z = b.z + avZ - p.z;
                    //update average
                    averageRiseX += -b.x + pCopy.x;
                    averageRiseY += -b.y + pCopy.y;
                    averageRiseZ += -b.z + pCopy.z;
                    b = pCopy;
                    divider++;
                }
            }
        }


        public static void BetterLinearPredictorBackwards(PFSSData data, int channel)
        {
            foreach (PFSSLine l in data.lines)
            {
                float averageRiseX;
                float averageRiseY;
                float averageRiseZ;
                float lastError = 0;

                PFSSPoint a = l.points[0];
                PFSSPoint b = l.points[1];
                averageRiseX = -a.x + b.x;
                averageRiseY = -a.y + b.y;
                averageRiseZ = -a.z + b.z;
                int divider = 1;

                for (int i = 2; i < l.points.Count; i++)
                {
                    float avX = averageRiseX / divider;
                    float avY = averageRiseY / divider;
                    float avZ = averageRiseZ / divider;
                    PFSSPoint p = l.points[i];

                    float errCopy = p.x;
                    p.x = b.x + avX - p.x-lastError;
                    p.y = b.y + avY - p.y;
                    p.z = b.z + avZ - p.z;
                    //update average
                    averageRiseX += -b.x + p.x;
                    averageRiseY += -b.y + p.y;
                    averageRiseZ += -b.z + p.z;
                    b = p;
                    divider++;
                }
            }
        }

        public static void QuadForward(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {

            }
        }
    }
}
