using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.PFSS
{
    class Residuals
    {
        public static int factor = 6;
        public static int factor2 = 8;
        public static int factor3 = 12;
        #region tryout fields
        internal Residuals nextLevel { get; set; }
        internal List<PFSSPoint> points { get; set; }
        internal PFSSPoint extra { get; set; }
        internal PFSSPoint startAverage { get; set; }
        #endregion

        internal PFSSPoint startPoint { get; set; }
        internal PFSSPoint endPoint { get; set; }
        internal List<PFSSPoint> predictionErrors { get; set; }
        internal short moreThanOne { get; set; }

        public Residuals(int count)
        {
            points = new List<PFSSPoint>(count);
            predictionErrors = new List<PFSSPoint>(count);
            
        }

        #region pureWaveletTryout
        public static Residuals Forward(PFSSLine line)
        {
            List<PFSSPoint> currentLevel = new List<PFSSPoint>(line.points);
            Residuals lastLevel = null;
            while (currentLevel.Count != 1)
            {
                Residuals nextLevel;
                List<PFSSPoint> averages;
                EncodeLevel(currentLevel, out nextLevel, out averages);
                currentLevel = averages;
                nextLevel.nextLevel = lastLevel;
                lastLevel = nextLevel;
            }

            lastLevel.startAverage = currentLevel[0];
            
            return lastLevel;
        }

        private static void EncodeLevel(List<PFSSPoint> level, out Residuals res, out List<PFSSPoint> averages)
        {
            int length = level.Count / 2;
            res = new Residuals(length);
            averages = new List<PFSSPoint>(length);
            if(level.Count % 2 == 1 && level.Count != 1)
            {
                PFSSPoint a = level[level.Count-2];
                PFSSPoint b = level[level.Count -1];
                res.extra = new PFSSPoint(-a.x + b.x, -a.y + b.y, -a.z + b.z);
                level.RemoveAt(level.Count - 1);
            }

            for (int i = 0; i < length; i++)
            {
                PFSSPoint p0 = level[i*2];
                PFSSPoint p1 = level[i*2+1];
                PFSSPoint average = new PFSSPoint((int)(p0.x + p1.x) / 2, (int)(p0.y + p1.y) / 2, (int)(p0.z + p1.z) / 2);
                averages.Add(average);
                PFSSPoint residual = new PFSSPoint(average.x - p0.x, average.y - p0.y, average.z - p0.z);
                res.points.Add(residual);
            }
        }

        public static void Backward(PFSSLine line, Residuals res)
        {
            List<PFSSPoint> lastLevelPoints = new List<PFSSPoint>(1);
            lastLevelPoints.Add(res.startAverage);
            Residuals currentLevel = res;
            while (currentLevel != null)
            {
                lastLevelPoints = decodeLevel(lastLevelPoints, currentLevel);
                currentLevel = currentLevel.nextLevel;
            }

            //copy
            for (int i = 0; i < line.points.Count; i++)
            {
                line.points[i].x = lastLevelPoints[i].x;
                line.points[i].y = lastLevelPoints[i].y;
                line.points[i].z = lastLevelPoints[i].z;
            }
        }

        private static List<PFSSPoint> decodeLevel(List<PFSSPoint> lastLevel, Residuals res)
        {
            int length = res.points.Count * 2;
            length = res.extra != null ? length + 1 : length;
            List<PFSSPoint> answer = new List<PFSSPoint>(length);
            for (int i = 0; i < lastLevel.Count; i++)
            {
                PFSSPoint residual = res.points[i];
                PFSSPoint old = lastLevel[i];
                answer.Add(new PFSSPoint(old.x - residual.x, old.y - residual.y, old.z - residual.z));
                answer.Add(new PFSSPoint(old.x + residual.x, old.y + residual.y, old.z + residual.z));
            }

            if (res.extra != null)
            {
                PFSSPoint p = answer[answer.Count - 1];
                answer.Add(new PFSSPoint(p.x + res.extra.x, p.y + res.extra.y, p.z + res.extra.z));
            }

            return answer;
        }

        public static void PredictLineForwared(PFSSLine l)
        {
            Residuals current = l.residuals;
            Residuals before0 = null;
            Residuals before1 = null;
            
            while (current != null)
            {
                for (int i = 0; i < current.points.Count; i++)
                {
                    PFSSPoint levelBefore = before0.points[i/2];
                    PFSSPoint top = before1.points[(i/2)/2];
                    
                    
                }
            }
            //Residuals currentLevel = 
        }
        #endregion

        public static void ForwardPrediction(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                Residuals residuals = new Residuals(l.points.Count - 2);
                residuals.startPoint = l.points[0];
                residuals.endPoint = l.points[l.points.Count - 1];
                l.residuals = residuals;

                Queue<Tuple<int, int, PFSSPoint>> bfs = new Queue<Tuple<int, int, PFSSPoint>>();
                if (l.points.Count > 2)
                {
                    bfs.Enqueue(new Tuple<int, int, PFSSPoint>(0, l.points.Count - 1,new PFSSPoint(0,0,0)));
                    while (bfs.Count >= 1)
                    {
                        Tuple<int, int, PFSSPoint> i = bfs.Dequeue();
                        PredictLinearBF(l, bfs, i.Item1, i.Item2, i.Item3);
                    }
                }
                quantize(l.residuals);
            }
        }

        private static void PredictLinearBF(PFSSLine l, Queue<Tuple<int, int, PFSSPoint>> callQueue, int startIndex, int endIndex, PFSSPoint currentError)
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
            l.residuals.predictionErrors.Add(toPredict);

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

        private static PFSSPoint Predict(PFSSPoint start, PFSSPoint end, PFSSPoint actual,int startIndex,int endIndex, int actualIndex)
        {
            int len = endIndex - startIndex;
            float fac0 = (actualIndex-startIndex) / ((float)len);
            float fac1 = (endIndex-actualIndex) / ((float)len);
            PFSSPoint prediction = new PFSSPoint((int)(fac0 * start.x + fac1 * end.x), (int)(fac0 * start.y + fac1 * end.y), (int)(fac0*start.z + fac1 * end.z) );

            return new PFSSPoint(prediction.x - actual.x, prediction.y - actual.y, prediction.z - actual.z);
        }

        private static void quantize(Residuals res)
        {
            for (int i = 0; i < 5 && i < res.predictionErrors.Count; i++)
            {
                res.predictionErrors[i].x = (int)Math.Truncate(res.predictionErrors[i].x / factor);
                res.predictionErrors[i].y = (int)Math.Truncate(res.predictionErrors[i].y / factor);
                res.predictionErrors[i].z = (int)Math.Truncate(res.predictionErrors[i].z / factor);
            }

            for (int i = 5; i < 16 && i < res.predictionErrors.Count; i++)
            {
                res.predictionErrors[i].x = (int)Math.Truncate(res.predictionErrors[i].x / factor2);
                res.predictionErrors[i].y = (int)Math.Truncate(res.predictionErrors[i].y / factor2);
                res.predictionErrors[i].z = (int)Math.Truncate(res.predictionErrors[i].z / factor2);
            }

            for (int i = 16; i < res.predictionErrors.Count; i++)
            {
                res.predictionErrors[i].x = (int)Math.Truncate(res.predictionErrors[i].x / factor3);
                res.predictionErrors[i].y = (int)Math.Truncate(res.predictionErrors[i].y / factor3);
                res.predictionErrors[i].z = (int)Math.Truncate(res.predictionErrors[i].z / factor3);
            }
        }

        private static void dequantize(Residuals res)
        {
            for (int i = 0; i < 5 && i < res.predictionErrors.Count; i++)
            {
                res.predictionErrors[i].x = res.predictionErrors[i].x * factor;
                res.predictionErrors[i].y = res.predictionErrors[i].y * factor;
                res.predictionErrors[i].z = res.predictionErrors[i].z * factor;
            }

            for (int i = 5; i < 16 && i < res.predictionErrors.Count; i++)
            {
                res.predictionErrors[i].x = res.predictionErrors[i].x * factor2;
                res.predictionErrors[i].y = res.predictionErrors[i].y * factor2;
                res.predictionErrors[i].z = res.predictionErrors[i].z * factor2;
            }

            for (int i = 16; i < res.predictionErrors.Count; i++)
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
                dequantize(l.residuals);
                Queue<PFSSPoint> bfs = new Queue<PFSSPoint>(l.residuals.predictionErrors);
                Queue<Tuple<int, int>> bfsIndices = new Queue<Tuple<int, int>>();
                bfsIndices.Enqueue(new Tuple<int, int>(0, l.points.Count - 1));

                l.points[0].x = l.residuals.startPoint.x;
                l.points[0].y = l.residuals.startPoint.y;
                l.points[0].z = l.residuals.startPoint.z;
                l.points[l.points.Count - 1].x = l.residuals.endPoint.x;
                l.points[l.points.Count - 1].y = l.residuals.endPoint.y;
                l.points[l.points.Count - 1].z = l.residuals.endPoint.z;
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
