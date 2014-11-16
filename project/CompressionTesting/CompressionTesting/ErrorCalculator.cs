using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;
using CompressionTesting.PFSS.Test;

namespace CompressionTesting
{
    class ErrorCalculator
    {
        public static Tuple<double, double> CalculateOverallError(TestSuite[] expected, PFSSData[] actual)
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
                    CalculateLineQuadError(expected[i].lines[j], actual[i].lines[j], ref maxError, ref quadSumError,ref pointCount, ref point);
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

        private static void CalculateLineQuadError(TestLine expected, PFSSLine actual, ref double max, ref double quadSum, ref long pointCount, ref int point)
        {
            pointCount += actual.points.Count;
            int startIndex = 0;
            int index = 0;

            //randbehandlung
            Point actualPoint = new Point(actual.points[0]);
            for (int i = 0; i < actual.points[0].testPointIndex; i++)
            {
                Point expectedPoint = new Point(expected.points[i]);
                double err = actualPoint.CalculateDistanceTo(expectedPoint);
                
                max = Math.Max(err, max);
                quadSum += err * err;
            }

            for (int i = 0; i < actual.points.Count - 1; i++)
            {
                PFSSPoint start = actual.points[i];
                PFSSPoint end = actual.points[i + 1];

                Point startPoint = new Point(start);
                Point endPoint = new Point(end);
                for (int j = start.testPointIndex; j < end.testPointIndex; j++)
                {
                    Point expectedPoint = new Point(expected.points[j]);
                    double err = calcError(startPoint, endPoint, expectedPoint);
                    if (err == double.NaN)
                        System.Console.WriteLine("baadf");
                    max = Math.Max(err, max);
                    quadSum += err * err;
                }
            }

            //randbehandlung 2
            PFSSPoint actualPFSSPoint = actual.points[actual.points.Count - 1];
            actualPoint = new Point(actualPFSSPoint);
            for (int i = actualPFSSPoint.testPointIndex; i < expected.points.Count; i++)
            {
                Point expectedPoint = new Point(expected.points[i]);
                double err = actualPoint.CalculateDistanceTo(expectedPoint);
                max = Math.Max(err, max);
                quadSum += err * err;
            }
          
        }

        private class Point
        {
            float x;
            float y;
            float z;

            public Point(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public Point(TestPoint p)
            {
                this.x = p.x;
                this.y = p.y;
                this.z = p.z;
            }

            public Point(PFSSPoint p)
            {
                this.x = p.x;
                this.y = p.y;
                this.z = p.z;
            }

            public double CalculateDistanceTo(Point p)
            {
                Point vec = getVector(this, p);
                return vec.magnitude();
            }

            public double magnitude()
            {
                return Math.Sqrt(x * x + y * y + z * z);
            }

            public static Point getVector(Point start, Point end)
            {
                return new Point(end.x - start.x, end.y - start.y, end.z - start.z);
            }

            public static double dot(Point p0, Point p1)
            {
                return p0.x * p1.x + p0.y * p1.y + p0.z * p1.z;
            }

            public static Point cross(Point p0, Point p1)
            {
                float x = p0.y * p1.z - p0.z * p1.y;
                float y = p0.z * p1.x - p0.x * p1.z;
                float z = p0.x * p1.y - p0.y * p1.x;
                return new Point(x, y, z);
            }
        }

        private static double calcError(Point A, Point B, Point P)
        {
            double t = checkOnLine(A, B, P);

            //if perpendicular line from P to AB does not intersect with AB, then use the distance of A or B.
            if (t >= 1.0)
                return B.CalculateDistanceTo(P);
            if (t <= 0)
                return A.CalculateDistanceTo(P);

            //perpendicular line intersects with AB. Calculate the smallest distance to the line
            Point lineVector = Point.getVector(B, A);
            Point toP = Point.getVector(B, P);
            Point cross = Point.cross(toP, lineVector);

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
        private static double checkOnLine(Point A, Point B, Point p)
        {
            double result = 0;
            Point vec0 = Point.getVector(p, A);
            Point vec1 = Point.getVector(A, B);
            double mag = vec1.magnitude();
            result = -Point.dot(vec0, vec1) / (mag * mag);
            return result;
        }
    }
}
