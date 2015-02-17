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
                Quantize(l);
            }
        }

        private static void PredictLinearBF(PFSSLine l, Queue<Tuple<int, int>> callQueue, int startIndex, int endIndex)
        {
            PFSSPoint start = l.points[startIndex];
            PFSSPoint end = l.points[endIndex];

            int toPredictIndex = (endIndex - startIndex) / 2+startIndex;
            PFSSPoint toPredict = l.points[toPredictIndex];

            PFSSPoint error = Predict(start, end, toPredict,startIndex,endIndex,toPredictIndex);
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

        private static PFSSPoint Predict(PFSSPoint start, PFSSPoint end, PFSSPoint actual,int startIndex,int endIndex, int actualIndex)
        {
            int len = endIndex - startIndex;
            float fac0 = (actualIndex-startIndex) / ((float)len);
            float fac1 = (endIndex-actualIndex) / ((float)len);
            PFSSPoint prediction = new PFSSPoint((int)(fac0 * start.Radius + fac1 * end.Radius), (int)(fac0 * start.Phi + fac1 * end.Phi), (int)(fac0*start.Theta + fac1 * end.Theta) );

            return new PFSSPoint(prediction.Radius - actual.Radius, prediction.Phi - actual.Phi, prediction.Theta - actual.Theta);
        }

        private static void Quantize(PFSSLine line)
        {
            for (int i = 0; i < 5 && i < line.predictionErrors.Count; i++)
            {
                line.predictionErrors[i].Radius = (int)Math.Truncate(line.predictionErrors[i].Radius / factor);
                line.predictionErrors[i].Phi = (int)Math.Truncate(line.predictionErrors[i].Phi / factor);
                line.predictionErrors[i].Theta = (int)Math.Truncate(line.predictionErrors[i].Theta / factor);
            }

            for (int i = 5; i < 16 && i < line.predictionErrors.Count; i++)
            {
                line.predictionErrors[i].Radius = (int)Math.Truncate(line.predictionErrors[i].Radius / factor2);
                line.predictionErrors[i].Phi = (int)Math.Truncate(line.predictionErrors[i].Phi / factor2);
                line.predictionErrors[i].Theta = (int)Math.Truncate(line.predictionErrors[i].Theta / factor2);
            }

            for (int i = 16; i < line.predictionErrors.Count; i++)
            {
                line.predictionErrors[i].Radius = (int)Math.Truncate(line.predictionErrors[i].Radius / factor3);
                line.predictionErrors[i].Phi = (int)Math.Truncate(line.predictionErrors[i].Phi / factor3);
                line.predictionErrors[i].Theta = (int)Math.Truncate(line.predictionErrors[i].Theta / factor3);
            }
        }
    }
}
