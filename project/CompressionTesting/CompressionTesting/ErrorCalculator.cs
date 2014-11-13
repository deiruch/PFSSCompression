using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

namespace CompressionTesting
{
    class ErrorCalculator
    {
        public static Tuple<double, double> CalculateOverallError(PFSSData[] expected, PFSSData[] actual)
        {
            double maxError = 0;
            double quadSumError = 0;
            long pointCount = 0;
            int point = 0;
            int lastPoint = 0;
            int line = 0;
            int file = 0;
            for (int i = 0; i < expected.Length; i++)
            {
                //don't check last line, there is something which eats the last few floats of the last line
                for (int j = 0; j < expected[i].lines.Count-1; j++)
                {
                    CalculateQuadError(expected[i].lines[j], actual[i].lines[j], ref maxError, ref quadSumError,ref pointCount, ref point);
                    if (point != lastPoint)
                    {
                        lastPoint = point;
                        line = j;
                        file = i;
                    }
                }
            }
            quadSumError /= pointCount;
            quadSumError = Math.Sqrt(quadSumError);

            return new Tuple<double,double>(maxError,quadSumError);
        }

        private static void CalculateQuadError(PFSSLine expected, PFSSLine actual, ref double max, ref double quadSum, ref long pointCount, ref int point)
        {
            pointCount += actual.points.Count;
            int startIndex = 0;
            int index = 0;
            foreach (PFSSPoint p in actual.points)
            {
                MinLine min = GetMinimum(expected, p, startIndex);
                //startIndex = min.p1;
                double err = 0;
                if (min.p1 > -1)
                {
                    PFSSPoint a = expected.points[min.p0];
                    PFSSPoint b = expected.points[min.p1];
                    err = calcError(a, b, p);
                    if (23301674660345 < err)
                        System.Console.WriteLine("why");
                } 
                else
                {
                    err = min.d0;
                }

                if (max < err)
                {
                    max = err;
                    point = index; 
                }
                index++;

                max = Math.Max(err, max);
                quadSum += err * err;
            }
        }


        private static MinLine GetMinimum(PFSSLine expected, PFSSPoint p, int startIndex)
        {
            MinLine min = new MinLine();

            double lastDistance = double.MaxValue;
            for (int i = startIndex; i < expected.points.Count; i++)
            {
                double distance = p.getDistanceTo(expected.points[i]);
                if (distance < lastDistance)
                {
                    min.newD(distance, i);
                    lastDistance = distance;
                }
                //shot over minimum
                /*else
                {
                    //check if current distance is smaller than p1;
                    if (min.d1 > distance)
                    {
                        min.newD(distance, i);
                        break;
                    }
                    //break;
                }*/

            }
            return min;
        }


        private static double calcError(PFSSPoint A, PFSSPoint B, PFSSPoint P)
        {
            double t = checkOnLine(A, B, P);

            //if perpendicular line from P to AB does not intersect with AB, then use the distance of A or B.
            if (t >= 1.0)
                return B.getDistanceTo(P);
            if (t <= 0)
                return A.getDistanceTo(P);

            //perpendicular line intersects with AB. Calculate the smallest distance to the line
            PFSSPoint lineVector = PFSSPoint.getVector(B, A);
            PFSSPoint toP = PFSSPoint.getVector(B, P);
            PFSSPoint cross = PFSSPoint.cross(toP, lineVector);

            return cross.magnitude() / lineVector.magnitude();
        }

      
        /// <summary>
        /// line definition: line v = A + (B-A)*t for 0 <= t <= 1
        /// calculates the factor t for the line which starts at p and is Perpendicular to v. 
        /// 
        /// if t > 0 && t < 1, means you can draw a Perpendicular line to v through p and intersects v.
        /// 
        /// if t < 0 && t> 1 means that the intersect point is outside of v.
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private static double checkOnLine(PFSSPoint A, PFSSPoint B, PFSSPoint p)
        {
            double result = 0;
            PFSSPoint vec0 = PFSSPoint.getVector(p, A);
            PFSSPoint vec1 = PFSSPoint.getVector(A, B);
            double mag = vec1.magnitude();
            result = -PFSSPoint.dot(vec0, vec1) / (mag * mag);
            return result;
        }

        /// <summary>
        /// Helper class for finding the minimum
        /// </summary>
        private class MinLine
        {
            internal int p0 = -1;
            internal int p1 = -1;
            internal double d0 = double.MaxValue;
            internal double d1 = double.MaxValue;

            public void newD(double d, int i)
            {
                p1 = p0;
                d1 = d0;
                d0 = d;
                p0 = i;
            }

        }
    }
}
