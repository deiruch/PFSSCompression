using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CompressionTesting.PFSS;

namespace CompressionTesting.DebugOutput
{
    class MedianWriter
    {
    
        public static void AnalyzeDCT(PFSSData data, int offset, FileInfo outFile)
        {
            int maxCount = 0;
            foreach (PFSSLine l in data.lines)
                maxCount = Math.Max(maxCount, l.points.Count);

            maxCount -= offset;
            double[] rMean = new double[maxCount];
            double[] pMean = new double[maxCount];
            double[] tMean = new double[maxCount];

            int[] rCount = new int[maxCount];
            int[] pCount = new int[maxCount];
            int[] tCount = new int[maxCount];

            foreach (PFSSLine l in data.lines)
            {

                //analyze
                for (int i = offset; i < l.points.Count; i++)
                {
                    int j = i - offset;
                    rMean[j] += Math.Abs(l.points[i].x);
                    pMean[j] += Math.Abs(l.points[i].y);
                    tMean[j] += Math.Abs(l.points[i].z);
                    rCount[j]++;
                    pCount[j]++;
                    tCount[j]++;
                }
            }

            StreamWriter w = new StreamWriter(new FileStream(outFile.FullName, FileMode.Create));
            //print analyzation
            w.Write("R;P;T\n");
            for (int i = 0; i < maxCount; i++)
            {
                w.Write(rMean[i] / (double)rCount[i]); w.Write(";");
                w.Write(pMean[i] / (double)pCount[i]); w.Write(";");
                w.Write(tMean[i] / (double)tCount[i]); w.Write("\n");
            }
            w.Close();
        }

        public static void AnalyzeFirstCurveType(PFSSData data, TYPE t, FileInfo outFile)
        {
            int offset = 0;
            int maxCount = 0;
            foreach (PFSSLine l in data.lines)
            {
                if (l.Type == t)
                {
                    maxCount = Math.Max(maxCount, l.points.Count);
                    break;
                }
                    
            }


            maxCount -= offset;
            double[] rMean = new double[maxCount];
            double[] pMean = new double[maxCount];
            double[] tMean = new double[maxCount];

            int[] rMinus = new int[maxCount];
            int[] pMinus = new int[maxCount];
            int[] tMinus = new int[maxCount];

            int[] rCount = new int[maxCount];
            int[] pCount = new int[maxCount];
            int[] tCount = new int[maxCount];

            foreach (PFSSLine l in data.lines)
            {
                if (l.Type == t)
                {
                    //analyze
                    for (int i = offset; i < l.points.Count; i++)
                    {
                        int j = i - offset;
                        rMean[j] += l.points[i].x;
                        pMean[j] += l.points[i].y;
                        tMean[j] += l.points[i].z;
                        rCount[j]++;
                        pCount[j]++;
                        tCount[j]++;
                    }
                    break;
                }
            }

            StreamWriter w = new StreamWriter(new FileStream(outFile.FullName, FileMode.Create));
            //print analyzation
            w.Write("R;P;T;RMinus;PMinus;TMinus");
            w.Write("\n");
            for (int i = 0; i < maxCount; i++)
            {
                w.Write(rMean[i] / (double)rCount[i]); w.Write(";");
                w.Write(pMean[i] / (double)pCount[i]); w.Write(";");
                w.Write(tMean[i] / (double)tCount[i]); w.Write(";");
                w.Write(rMinus[i]); w.Write(";");
                w.Write(pMinus[i]); w.Write(";");
                w.Write(tMinus[i]);
                w.Write("\n");

            }
            w.Close();
        
        }

        public static void AnalyzePerCurveType(PFSSData data,TYPE t, int offset, FileInfo outFile)
        {
            int maxCount = 0;
            foreach (PFSSLine l in data.lines)
            {
                if(l.Type == t)
                    maxCount = Math.Max(maxCount, l.points.Count);
            }
                

            maxCount -= offset;
            double[] rMean = new double[maxCount];
            double[] pMean = new double[maxCount];
            double[] tMean = new double[maxCount];

            int[] rMinus = new int[maxCount];
            int[] pMinus = new int[maxCount];
            int[] tMinus = new int[maxCount];

            int[] rCount = new int[maxCount];
            int[] pCount = new int[maxCount];
            int[] tCount = new int[maxCount];

            foreach (PFSSLine l in data.lines)
            {
                if (l.Type == t)
                {
                    //analyze
                    for (int i = offset; i < l.points.Count; i++)
                    {
                        int j = i - offset;
                        rMean[j] += Math.Abs(l.points[i].x);
                        pMean[j] += Math.Abs(l.points[i].y);
                        tMean[j] += Math.Abs(l.points[i].z);
                        rCount[j]++;
                        pCount[j]++;
                        tCount[j]++;
                        rMinus[j] += Math.Sign(l.points[i].x);
                        pMinus[j] += Math.Sign(l.points[i].y);
                        tMinus[j] += Math.Sign(l.points[i].z);
                    }
                }
            }

            StreamWriter w = new StreamWriter(new FileStream(outFile.FullName, FileMode.Create));
            //print analyzation
            w.Write("R;P;T;RMinus;PMinus;TMinus");
            w.Write("\n");
            for (int i = 0; i < maxCount; i++)
            {
                w.Write(rMean[i] / (double)rCount[i]); w.Write(";");
                w.Write(pMean[i] / (double)pCount[i]); w.Write(";");
                w.Write(tMean[i] / (double)tCount[i]); w.Write(";");
                w.Write(rMinus[i]); w.Write(";");
                w.Write(pMinus[i]); w.Write(";");
                w.Write(tMinus[i]);
                w.Write("\n");

            }
            w.Close();
        
        }

    }
}
