using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CompressionTesting.Quantization;
using CompressionTesting.Transformation;
using CompressionTesting.FileWriter;
using CompressionTesting.Compression;
using CompressionTesting.PFSS;

namespace CompressionTesting.Solutions
{
    class Solution1 : ISolution
    {
        public int GetQualityLevels()
        {
            return 1;//5, 6
        }

        public string GetName()
        {
            return "DCTXYZ";
        }

        public TestResult DoTestRun(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName()+qualityLevel+".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.Subsample(data,4);
            //Discretizer.Divide(data, 1000, 0);
            Residualizer.DoResiduals(data, 1);
            //Residualizer.DoResiduals(data, 2);
            DCTransformer.Forward(data, 1);

            int zeroCount = GetZeroCount(data, qualityLevel+5);
            CompressionTesting.DebugOutput.MedianWriter.AnalyzeDCT(data,1, new FileInfo(Path.Combine(folder, this.GetName()+".csv")));
            DCTQuantization.SetToZero(data, zeroCount);
            Discretizer.Divide(data, 1000, 1);
            Discretizer.ToShorts(data, 1);

            InterleavedWriter.WriteFits(data, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            Discretizer.Multiply(data, 1000, 1);
            DCTransformer.Backward(data, 1);
            Residualizer.UndoResiduals(data, 1);
            Discretizer.Multiply(data, 1000, 1);
            return result;
        }

        private int GetZeroCount(PFSS.PFSSData data, int qualityLevel)
        {
            int maxCount = 0;
            foreach (PFSSLine l in data.lines)
            {
                maxCount = Math.Max(l.points.Count, maxCount);
            }


            return (int)Math.Round(maxCount*(1-0.15*qualityLevel));
        }

        public void SelectIntermediateSolution(int i)
        {
            throw new NotImplementedException();
        }

        #region intermediate solutions
        public TestResult FirstTry(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.Subsample(data, 4);
            Residualizer.DoResiduals(data, 1);
            DCTransformer.Forward(data, 1);

            int zeroCount = GetZeroCount(data, qualityLevel);
            DCTQuantization.SetToZero(data, zeroCount);
            Discretizer.Divide(data, 1000, 1);
            Discretizer.ToShorts(data, 1);

            InterleavedWriter.WriteFits(data, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            Discretizer.Multiply(data, 1000, 1);
            DCTransformer.Backward(data, 1);
            Residualizer.UndoResiduals(data, 1);

            return result;
        }
        #endregion
    }
}
