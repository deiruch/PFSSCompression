using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FITSFormatter.PFSS;

namespace FITSFormatter
{
    class ErrorCalculator
    {
        public static void CalcError(PFSSData data, out double deviationX, out double deviationY, out double deviationZ)
        {
            deviationX = 0;
            deviationY = 0;
            deviationZ = 0;
            int pointCount = 0;

            foreach (PFSSLine l in data.lines)
            {
                pointCount += l.points.Count;
                foreach (PFSSPoint p in l.points)
                {
                    
                    double errX = Math.Abs(p.origX - p.x);
                    double errY = Math.Abs(p.origY - p.y);
                    double errZ = Math.Abs(p.origZ - p.z);
                    deviationX += errX * errX;
                    deviationY += errY * errY;
                    deviationZ += errZ * errZ;
                }
            }

            deviationX /= pointCount;
            deviationY /= pointCount;
            deviationZ /= pointCount;
            deviationX = Math.Sqrt(deviationX);
            deviationY = Math.Sqrt(deviationY);
            deviationZ = Math.Sqrt(deviationZ);
        }
    }
}
