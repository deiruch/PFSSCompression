using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressedPFSSManager.PFSS;

namespace CompressedPFSSManager.CompressionAlgorithm
{
    class RecursiveLinearPredictor
    {
        public const int factor = 6;
        public const int factor2 = 10;
        public const int factor3 = 16;

        public static void ForwardPrediction(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                l.predictionErrors = new List<PFSSPoint>(l.points.Count - 2);
                l.startPoint = l.points[0];
                l.endPoint = l.points[l.points.Count - 1];

                Queue<Tuple<int, int, int>> bfs = new Queue<Tuple<int, int, int>>();
                if (l.points.Count > 2)
                {
                    bfs.Enqueue(new Tuple<int, int, int>(0, l.points.Count - 1,0));
                    while (bfs.Count >= 1)
                    {
                        Tuple<int, int,int> i = bfs.Dequeue();
                        PredictLinearBF(l, bfs, i.Item1, i.Item2,i.Item3);
                    }
                }
            }
        }

        private static void PredictLinearBF(PFSSLine l, Queue<Tuple<int, int, int>> callQueue, int startIndex, int endIndex, int level)
        {
            PFSSPoint start = l.points[startIndex];
            PFSSPoint end = l.points[endIndex];

            int toPredictIndex = (endIndex - startIndex) / 2+startIndex;
            PFSSPoint toPredict = l.points[toPredictIndex];

            PFSSPoint error = Predict(start, end, toPredict,startIndex,endIndex,toPredictIndex);
            Quantization(error, level);
            l.predictionErrors.Add(error);

            if (startIndex + 1 != toPredictIndex)
            {
                Tuple<int, int, int> t0 = new Tuple<int, int, int>(startIndex, toPredictIndex, level + 1);
                callQueue.Enqueue(t0);
            }
            if (endIndex - 1 != toPredictIndex)
            {
                Tuple<int, int, int> t1 = new Tuple<int, int, int>(toPredictIndex, endIndex,level+1);
                callQueue.Enqueue(t1);
            }
            
        }

        private static PFSSPoint Predict(PFSSPoint start, PFSSPoint end, PFSSPoint actual,int startIndex,int endIndex, int actualIndex)
        {
            int len = endIndex - startIndex;
            float fac0 = (actualIndex-startIndex) / ((float)len);
            float fac1 = (endIndex-actualIndex) / ((float)len);
            PFSSPoint prediction = new PFSSPoint((int)(fac0 * start.Radius + fac1 * end.Radius), (int)(fac0 * start.Phi + fac1 * end.Phi), (int)(fac0*start.Theta + fac1 * end.Theta) );

            return new PFSSPoint(prediction.Radius - actual.Radius, prediction.Phi - actual.Phi, prediction.Theta - actual.Theta);
        }

        private static void Quantization(PFSSPoint point, int i)
        {
            if (i < 3)
            {
                point.Radius = (int)Math.Truncate(point.Radius / factor);
                point.Phi = (int)Math.Truncate(point.Phi / factor);
                point.Theta = (int)Math.Truncate(point.Theta / factor);
            }

            if (i >= 3 && i < 5)
            {
                point.Radius = (int)Math.Truncate(point.Radius / factor2);
                point.Phi = (int)Math.Truncate(point.Phi / factor2);
                point.Theta = (int)Math.Truncate(point.Theta / factor2);
            }

            if (i >= 5)
            {
                point.Radius = (int)Math.Truncate(point.Radius / factor3);
                point.Phi = (int)Math.Truncate(point.Phi / factor3);
                point.Theta = (int)Math.Truncate(point.Theta / factor3);
            }
        }
    }
}
