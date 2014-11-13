using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FITSFormatter.PFSS;

namespace FITSFormatter
{
    class DCTAnalyzer
    {

        public static void AnalyzeWithWholeRange(FileInfo inFile, FileInfo outFile,FileInfo fitsOut) 
        { 
            //do dct
            PFSSData data = FitsReader.ReadFits(inFile, true);

            int maxCount = 0;
            foreach (PFSSLine l in data.lines)
                maxCount = Math.Max(maxCount, l.points.Count);

            maxCount--;
            double[] rMean = new double[maxCount];
            double[] pMean = new double[maxCount];
            double[] tMean = new double[maxCount];

            double varR = 0;
            double varP = 0;
            double varT = 0;
            int pointCount = 0;

            int[] rCount = new int[maxCount];
            int[] pCount = new int[maxCount];
            int[] tCount = new int[maxCount];
            int maxMinus = 0;

            foreach (PFSSLine l in data.lines)
            {
                float[] r = new float[l.points.Count-1]; //-2^13 -12288
                float[] p = new float[l.points.Count-1]; //-2^14
                float[] t = new float[l.points.Count-1]; //-2^13
                pointCount += l.points.Count;

                transformPoint(l.points[0]);
                PFSSPoint last = l.points[0];
                //copy
                for(int i = 1; i < l.points.Count;i++)
                {
                    PFSSPoint current = l.points[i];
                    transformPoint(current);
                    r[i-1] = (last.rawR -current.rawR);
                    p[i-1] = (last.rawPhi- current.rawPhi);
                    t[i-1] = (last.rawTheta - current.rawTheta);
                    last = current;
                    /*PFSSPoint point = l.points[i];
                    float rCopy = point.rawR-8192;
                    if (rCopy < 0) {
                        maxMinus = Math.Max(maxMinus,(int)Math.Abs(rCopy));
                        rCopy = 0;
                    }

                    r[i] = rCopy-12288;
                    p[i] = point.rawPhi - 16384;
                    t[i] = point.rawTheta - 8192;*/
                    /*r[i] = current.rawR;
                    p[i] = current.rawPhi;
                    t[i] = current.rawTheta; */
                }


                l.rCos = DCT.slow_fdct(r);
                l.pCos = DCT.slow_fdct(p);
                l.tCos = DCT.slow_fdct(t);

                //quantize
                //DCTransformer.QuantizeRepeat(l, l.points.Count - 1);
               
                //error
                l.riCos = DCT.slow_idct(l.rCos);
                l.piCos = DCT.slow_idct(l.pCos);
                l.tiCos = DCT.slow_idct(l.tCos);

                //calcError
                PFSSPoint before = l.points[0];
                for (int i = 0; i < l.points.Count-1;i++ )
                {
                    double newR = -(Math.Round(l.riCos[i])) + before.rawR;
                    double newP = -(Math.Round(l.piCos[i])) + before.rawPhi;
                    double newT = -(Math.Round(l.tiCos[i])) + before.rawTheta;
                    /*double newR = (Math.Round(l.riCos[i]));
                    double newP = (Math.Round(l.piCos[i]));
                    double newT = (Math.Round(l.tiCos[i]));*/
                    double errR = Math.Abs(newR- l.points[i+1].rawR);
                    double errP = Math.Abs(newP - l.points[i+1].rawPhi);
                    double errT = Math.Abs(newT - l.points[i+1].rawTheta);

                    varR += errR * errR;
                    varP += errP * errP;
                    varT += errT * errT;
                    before = new PFSSPoint((short)newR,(short)newP,(short)newT);
                }

                //analyze
                for(int i = 0; i < l.points.Count-1;i++)
                {
                    rMean[i] += Math.Abs(l.rCos[i]);
                    pMean[i] += Math.Abs(l.pCos[i]);
                    tMean[i] += Math.Abs(l.tCos[i]);
                    rCount[i]++;
                    pCount[i]++;
                    tCount[i]++;
                }
            }

            varR /= pointCount;
            varP /= pointCount;
            varT /= pointCount;
            varR = Math.Sqrt(varR);
            varP = Math.Sqrt(varP);
            varT = Math.Sqrt(varT);

            System.Console.WriteLine(varR);
            System.Console.WriteLine(varP);
            System.Console.WriteLine(varT);

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

            DCTWriter.Write(data, fitsOut);
        }

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

        

        public static void transformPoint(PFSSPoint p)
        {
            
            int rCopy = p.rawR - 8192;
            if (rCopy < 0)
            {
                rCopy = 0;
            }

            p.rawR = (short)(rCopy - 12288);
            p.rawPhi = (short)(p.rawPhi - 16384);
            p.rawTheta = (short)(p.rawTheta - 8192);
        }

    }
}
