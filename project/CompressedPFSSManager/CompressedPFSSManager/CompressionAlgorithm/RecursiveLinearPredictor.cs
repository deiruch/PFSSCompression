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
        public const float Q_FACTOR1 = 1e6f;
        public const float Q_FACTOR2 = 1e6f;
        public const float Q_FACTOR3 = 1e6f;

        public static void ForwardPrediction(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                l.predictionErrors = new List<PFSSPoint>(l.points.Count - 2);

                l.predictionErrors.Add(l.points[0]);
                l.predictionErrors.Add(l.points[l.points.Count - 1]-l.points[0]);

                var bfs = new Queue<Tuple<int, int>>();
                if (l.points.Count > 2)
                {
                    bfs.Enqueue(Tuple.Create(0, l.points.Count - 1));
                    while (bfs.Any())
                    {
                        var i = bfs.Dequeue();
                        PredictLinearBF(l, bfs, i.Item1, i.Item2);
                    }
                }
                Quantize(l);
            }
        }

        private static void PredictLinearBF(PFSSLine l, Queue<Tuple<int, int>> callQueue, int startIndex, int endIndex)
        {
            var start = l.points[startIndex];
            var end = l.points[endIndex];

            var toPredictIndex = (startIndex + endIndex) / 2;
            var toPredict = l.points[toPredictIndex];

            l.predictionErrors.Add(Predict(start, end, toPredict,startIndex,endIndex,toPredictIndex));

            if (startIndex + 1 != toPredictIndex)
                callQueue.Enqueue(new Tuple<int, int>(startIndex, toPredictIndex));

            if (endIndex - 1 != toPredictIndex)
                callQueue.Enqueue(new Tuple<int, int>(toPredictIndex, endIndex));
        }

        private static PFSSPoint Predict(PFSSPoint start, PFSSPoint end, PFSSPoint actual,int startIndex,int endIndex, int actualIndex)
        {
            var fac0 = (actualIndex-startIndex) / (double)(endIndex - startIndex);
            var prediction = (1-fac0) * start + fac0 * end;
            return prediction - actual;
        }

        private static void Quantize(PFSSLine line)
        {
            int i = 0;
            for (; i < 5 && i < line.predictionErrors.Count; i++)
            {
                line.predictionErrors[i].X = (int)Math.Truncate(line.predictionErrors[i].X / Q_FACTOR1);
                line.predictionErrors[i].Y = (int)Math.Truncate(line.predictionErrors[i].Y / Q_FACTOR1);
                line.predictionErrors[i].Z = (int)Math.Truncate(line.predictionErrors[i].Z / Q_FACTOR1);
            }

            for (; i < 16 && i < line.predictionErrors.Count; i++)
            {
                line.predictionErrors[i].X = (int)Math.Truncate(line.predictionErrors[i].X / Q_FACTOR2);
                line.predictionErrors[i].Y = (int)Math.Truncate(line.predictionErrors[i].Y / Q_FACTOR2);
                line.predictionErrors[i].Z = (int)Math.Truncate(line.predictionErrors[i].Z / Q_FACTOR2);
            }

            for (; i < line.predictionErrors.Count; i++)
            {
                line.predictionErrors[i].X = (int)Math.Truncate(line.predictionErrors[i].X / Q_FACTOR3);
                line.predictionErrors[i].Y = (int)Math.Truncate(line.predictionErrors[i].Y / Q_FACTOR3);
                line.predictionErrors[i].Z = (int)Math.Truncate(line.predictionErrors[i].Z / Q_FACTOR3);
            }
        }
    }
}
