using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.PFSS
{
    class Residuals
    {
        internal Residuals nextLevel { get; set; }
        internal List<PFSSPoint> points { get; set; }
        internal PFSSPoint extra { get; set; }
        internal PFSSPoint startAverage { get; set; }

        public Residuals(int count)
        {
            points = new List<PFSSPoint>(count);
        }

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
    }
}
