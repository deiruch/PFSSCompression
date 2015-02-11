using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.PFSS
{
    class Residuals
    {
        public const int factor = 6;
        public const int factor2 = 8;
        public const int factor3 = 12;

        public static void ForwardPrediction(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                l.predictionErrors = new List<PFSSPoint>(l.points.Count - 2);
                l.startPoint = l.points[0];
                l.endPoint = l.points[l.points.Count - 1];

                Queue<Tuple<int, int>> bfs = new Queue<Tuple<int, int>>();
                if (l.points.Count > 2)
                {
                    bfs.Enqueue(new Tuple<int, int>(0, l.points.Count - 1));
                    while (bfs.Count >= 1)
                    {
                        Tuple<int, int> i = bfs.Dequeue();
                        PredictLinearBF(l, bfs, i.Item1, i.Item2);
                    }
                }
                quantize(l);
            }
        }

        private static void PredictLinearBF(PFSSLine l, Queue<Tuple<int, int>> callQueue, int startIndex, int endIndex)
        {
            PFSSPoint start = l.points[startIndex];
            PFSSPoint end = l.points[endIndex];

            int toPredictIndex = (endIndex - startIndex) / 2 + startIndex;
            PFSSPoint toPredict = l.points[toPredictIndex];

            PFSSPoint error = Predict(start, end, toPredict, startIndex, endIndex, toPredictIndex);
            l.predictionErrors.Add(error);

            if (startIndex + 1 != toPredictIndex)
            {
                Tuple<int, int> t0 = new Tuple<int, int>(startIndex, toPredictIndex);
                callQueue.Enqueue(t0);
            }
            if (endIndex - 1 != toPredictIndex)
            {
                Tuple<int, int> t1 = new Tuple<int, int>(toPredictIndex, endIndex);
                callQueue.Enqueue(t1);
            }

        }

        private static PFSSPoint Predict(PFSSPoint start, PFSSPoint end, PFSSPoint actual, int startIndex, int endIndex, int actualIndex)
        {
            int len = endIndex - startIndex;
            float fac0 = (actualIndex - startIndex) / ((float)len);
            float fac1 = (endIndex - actualIndex) / ((float)len);
            PFSSPoint prediction = new PFSSPoint((int)(fac0 * start.x + fac1 * end.x), (int)(fac0 * start.y + fac1 * end.y), (int)(fac0 * start.z + fac1 * end.z));

            return new PFSSPoint(prediction.x - actual.x, prediction.y - actual.y, prediction.z - actual.z);
        }

        private static void quantize(PFSSLine line)
        {
            for (int i = 0; i < 8 && i < line.predictionErrors.Count; i++)
            {
                line.predictionErrors[i].x = (int)Math.Truncate(line.predictionErrors[i].x / factor);
                line.predictionErrors[i].y = (int)Math.Truncate(line.predictionErrors[i].y / factor);
                line.predictionErrors[i].z = (int)Math.Truncate(line.predictionErrors[i].z / factor);
            }

            for (int i = 8; i < 24 && i < line.predictionErrors.Count; i++)
            {
                line.predictionErrors[i].x = (int)Math.Truncate(line.predictionErrors[i].x / factor2);
                line.predictionErrors[i].y = (int)Math.Truncate(line.predictionErrors[i].y / factor2);
                line.predictionErrors[i].z = (int)Math.Truncate(line.predictionErrors[i].z / factor2);
            }

            for (int i = 24; i < line.predictionErrors.Count; i++)
            {
                line.predictionErrors[i].x = (int)Math.Truncate(line.predictionErrors[i].x / factor3);
                line.predictionErrors[i].y = (int)Math.Truncate(line.predictionErrors[i].y / factor3);
                line.predictionErrors[i].z = (int)Math.Truncate(line.predictionErrors[i].z / factor3);
            }
        }





        internal PFSSPoint startPoint { get; set; }
        internal PFSSPoint endPoint { get; set; }
        internal List<PFSSPoint> predictionErrors { get; set; }
        internal short moreThanOne { get; set; }

        public Residuals(int count)
        {
            predictionErrors = new List<PFSSPoint>(count);
            
        }

        public static void ForwardPrediction_OLD(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                Residuals residuals = new Residuals(l.points.Count - 2);
                residuals.startPoint = l.points[0];
                residuals.endPoint = l.points[l.points.Count - 1];
                l.residuals_old = residuals;

                Queue<Tuple<int, int, PFSSPoint>> bfs = new Queue<Tuple<int, int, PFSSPoint>>();
                if (l.points.Count > 2)
                {
                    bfs.Enqueue(new Tuple<int, int, PFSSPoint>(0, l.points.Count - 1,new PFSSPoint(0,0,0)));
                    while (bfs.Count >= 1)
                    {
                        Tuple<int, int, PFSSPoint> i = bfs.Dequeue();
                        PredictLinearBF_OLD(l, bfs, i.Item1, i.Item2, i.Item3);
                    }
                }
                quantize(l.residuals_old);
            }
        }

        private static void PredictLinearBF_OLD(PFSSLine l, Queue<Tuple<int, int, PFSSPoint>> callQueue, int startIndex, int endIndex, PFSSPoint currentError)
        {
            PFSSPoint start = l.points[startIndex];
            PFSSPoint end = l.points[endIndex];

            int toPredictIndex = (endIndex - startIndex) / 2+startIndex;
            PFSSPoint toPredict = l.points[toPredictIndex];
            /*toPredict.x -= currentError.x;
            toPredict.y -= currentError.y;
            toPredict.z -= currentError.z;*/
            PFSSPoint error = Predict(start, end, toPredict,startIndex,endIndex,toPredictIndex);
            toPredict = new PFSSPoint(toPredict);
            toPredict.x = error.x;
            toPredict.y = error.y;
            toPredict.z = error.z;
            //Strip(toPredict);
            l.residuals_old.predictionErrors.Add(toPredict);

            toPredict = new PFSSPoint(toPredict);
            //Push(toPredict);
            PFSSPoint actual = Predict(start, end, toPredict, startIndex, endIndex, toPredictIndex);
            PFSSPoint nextError = new PFSSPoint(-l.points[toPredictIndex].x + actual.x, -l.points[toPredictIndex].y + actual.y, -l.points[toPredictIndex].z + actual.z);
            /*l.points[toPredictIndex].x = actual.x;
            l.points[toPredictIndex].y = actual.y;
            l.points[toPredictIndex].z = actual.z;*/
            if (startIndex + 1 != toPredictIndex)
            {
                Tuple<int, int, PFSSPoint> t0 = new Tuple<int, int, PFSSPoint>(startIndex, toPredictIndex,nextError);
                callQueue.Enqueue(t0);
            }
            if (endIndex - 1 != toPredictIndex)
            {
                Tuple<int, int, PFSSPoint> t1 = new Tuple<int, int, PFSSPoint>(toPredictIndex, endIndex, nextError);
                callQueue.Enqueue(t1);
            }
            
        }


        private static void quantize(Residuals res)
        {
            for (int i = 0; i < 8 && i < res.predictionErrors.Count; i++)
            {
                res.predictionErrors[i].x = (int)Math.Truncate(res.predictionErrors[i].x / factor);
                res.predictionErrors[i].y = (int)Math.Truncate(res.predictionErrors[i].y / factor);
                res.predictionErrors[i].z = (int)Math.Truncate(res.predictionErrors[i].z / factor);
            }

            for (int i = 8; i < 24 && i < res.predictionErrors.Count; i++)
            {
                res.predictionErrors[i].x = (int)Math.Truncate(res.predictionErrors[i].x / factor2);
                res.predictionErrors[i].y = (int)Math.Truncate(res.predictionErrors[i].y / factor2);
                res.predictionErrors[i].z = (int)Math.Truncate(res.predictionErrors[i].z / factor2);
            }

            for (int i = 24; i < res.predictionErrors.Count; i++)
            {
                res.predictionErrors[i].x = (int)Math.Truncate(res.predictionErrors[i].x / factor3);
                res.predictionErrors[i].y = (int)Math.Truncate(res.predictionErrors[i].y / factor3);
                res.predictionErrors[i].z = (int)Math.Truncate(res.predictionErrors[i].z / factor3);
            }
        }

        private static void dequantize(PFSSLine res)
        {
            for (int i = 0; i < 8 && i < res.predictionErrors.Count; i++)
            {
                res.predictionErrors[i].x = res.predictionErrors[i].x * factor;
                res.predictionErrors[i].y = res.predictionErrors[i].y * factor;
                res.predictionErrors[i].z = res.predictionErrors[i].z * factor;
            }

            for (int i = 8; i < 24 && i < res.predictionErrors.Count; i++)
            {
                res.predictionErrors[i].x = res.predictionErrors[i].x * factor2;
                res.predictionErrors[i].y = res.predictionErrors[i].y * factor2;
                res.predictionErrors[i].z = res.predictionErrors[i].z * factor2;
            }

            for (int i = 24; i < res.predictionErrors.Count; i++)
            {
                res.predictionErrors[i].x = res.predictionErrors[i].x * factor3;
                res.predictionErrors[i].y = res.predictionErrors[i].y * factor3;
                res.predictionErrors[i].z = res.predictionErrors[i].z * factor3;
            }
        }

        private static void Strip(PFSSPoint p)
        {
            p.x = (int)Math.Truncate(p.x / factor2);
            p.y = (int)Math.Truncate(p.y / factor2);
            p.z = (int)Math.Truncate(p.z / factor2);
        }

        private static void Push(PFSSPoint p)
        {

            p.x = (int)(p.x * factor2);
            p.y = (int)(p.y * factor2);
            p.z = (int)(p.z * factor2);
        }
        public static void BackwardPrediction(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                dequantize(l);
                Queue<PFSSPoint> bfs = new Queue<PFSSPoint>(l.predictionErrors);
                Queue<Tuple<int, int>> bfsIndices = new Queue<Tuple<int, int>>();
                bfsIndices.Enqueue(new Tuple<int, int>(0, l.points.Count - 1));

                l.points[0].x = l.startPoint.x;
                l.points[0].y = l.startPoint.y;
                l.points[0].z = l.startPoint.z;
                l.points[l.points.Count - 1].x = l.endPoint.x;
                l.points[l.points.Count - 1].y = l.endPoint.y;
                l.points[l.points.Count - 1].z = l.endPoint.z;
                if (l.points.Count > 2)
                {
                    while (bfs.Count >= 1)
                    {
                        Tuple<int, int> i = bfsIndices.Dequeue();
                        PredictLinearBFBackwards(l, bfs, bfsIndices, i.Item1, i.Item2);
                    }
                }
            }
        }

        private static void PredictLinearBFBackwards(PFSSLine l, Queue<PFSSPoint> pointQueue, Queue<Tuple<int,int>> callQueue,int startIndex, int endIndex)
        {
            PFSSPoint start = l.points[startIndex];
            PFSSPoint end = l.points[endIndex];

            int toPredictIndex = (endIndex - startIndex) / 2 + startIndex;
            PFSSPoint toPredict = pointQueue.Dequeue();
            //Push(toPredict);
            PFSSPoint actual = Predict(start, end, toPredict, startIndex, endIndex, toPredictIndex);
            if (l.points[(endIndex - startIndex) / 2+startIndex].x != actual.x)
                System.Console.Write("");
            l.points[toPredictIndex].x = actual.x;
            l.points[toPredictIndex].y = actual.y;
            l.points[toPredictIndex].z = actual.z;
            if (startIndex + 1 != toPredictIndex)
            {
                Tuple<int, int> t0 = new Tuple<int, int>(startIndex, toPredictIndex);
                callQueue.Enqueue(t0);
            }
            if (endIndex - 1 != toPredictIndex)
            {
                Tuple<int, int> t1 = new Tuple<int, int>(toPredictIndex, endIndex);
                callQueue.Enqueue(t1);
            }
            

        }
    }
}
