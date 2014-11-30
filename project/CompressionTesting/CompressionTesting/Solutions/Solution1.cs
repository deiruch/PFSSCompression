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
            return 1;
        }

        public string GetName()
        {
            return "Solution1_Six";
        }

        public TestResult DoTestRun(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            
            return One(data, qualityLevel, folder);
        }

        private int GetZeroCount(PFSS.PFSSData data, int qualityLevel)
        {
            int maxCount = 0;
            foreach (PFSSLine l in data.lines)
            {
                maxCount = Math.Max(l.points.Count, maxCount);
            }


            return (int)Math.Round(maxCount*(1-0.05*qualityLevel));
        }

        public void SelectIntermediateSolution(int i)
        {
            throw new NotImplementedException();
        }

        public TestResult Solution(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.Subsample(data, 4);
            Discretizer.Divide(data, 1000, 0);
            Residualizer.DoResiduals(data, 1);
            //Residualizer.DoResiduals(data, 2);
            DCTransformer.Forward(data, 1);

            int zeroCount = GetZeroCount(data, (qualityLevel + 1) * 2 + 1);
            //CompressionTesting.DebugOutput.MedianWriter.AnalyzeDCT(data,1, new FileInfo(Path.Combine(folder, this.GetName()+".csv")));
            DCTQuantization.SetToZero(data, zeroCount);
            //Discretizer.Divide(data, 1000, 1);
            Discretizer.ToShorts(data, 1);

            InterleavedWriter.WriteFits(data, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            ///Discretizer.Multiply(data, 1000, 1);
            DCTransformer.Backward(data, 1);
            Residualizer.UndoResiduals(data, 1);
            Discretizer.Multiply(data, 1000, 1);
            return result;
        }

        #region intermediate solutions


        public TestResult Zero(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.Subsample(data, 4);
            //Residualizer.DoResiduals(data, 1);
            DCTransformer.Forward(data, 0);

            //int zeroCount = GetZeroCount(data, qualityLevel + 16);
            //DCTQuantization.SetToZero(data, zeroCount);
            Discretizer.Divide(data, 1000, 0);
            Discretizer.DivideLinear(data, 2 * (qualityLevel +90), 0);
            Discretizer.ToInt(data, 0);

            StandardWriter.WriteIntFits(data, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            Discretizer.MultiplyLinear(data, 2 * (qualityLevel + 90),0);
            Discretizer.Multiply(data, 1000, 0);
            DCTransformer.Backward(data, 0);
            //Residualizer.UndoResiduals(data, 1);

            return result;
        }

        public TestResult One(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.Subsample(data, 4);
            Residualizer.DoResiduals(data, 1);
            DCTransformer.Forward(data, 1);

            //int zeroCount = GetZeroCount(data, qualityLevel + 16);
            //DCTQuantization.SetToZero(data, zeroCount);
            DebugOutput.MedianWriter.AnalyzeDCT(data, 0, new FileInfo(Path.Combine(folder, this.GetName() + ".csv")));
            Discretizer.DividePoint(data, 50000, 0);
            Discretizer.Divide(data, 1000, 1);
            Discretizer.DivideLinear(data, 2*(qualityLevel+11), 1);
            Discretizer.ToShorts(data, 1);


            InterleavedWriter.WriteShortFits(data, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            Discretizer.MultiplyLinear(data, 2 * (qualityLevel + 11), 1);
            Discretizer.Multiply(data, 1000, 1);
            Discretizer.MultiplyPoint(data, 50000, 0);
            DCTransformer.Backward(data, 1);
            Residualizer.UndoResiduals(data, 1);

            return result;
        }

        public TestResult Two(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.Subsample(data, 4);
            //Residualizer.DoResiduals(data, 1);
            PCATransform.Forward(data, 0);
            //YCbCr.ForwardFull(data, 0);
            DCTransformer.Forward(data, 0);

            //Residualizer.DoResiduals(data, 3);
            Discretizer.Divide(data, 1000, 0);
            Discretizer.DivideLinear(data, 2 * (qualityLevel + 70), 0);
            Discretizer.ToInt(data, 0);

            PCAWriter.WriteIntFits(data, 0, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;


            Discretizer.MultiplyLinear(data, 2 * (qualityLevel + 70), 0);
            Discretizer.Multiply(data, 1000, 0);
            //Residualizer.UndoResiduals(data, 3);
            DCTransformer.Backward(data, 0);
            //YCbCr.BackwardsFull(data, 0);
            //PCACoefficient.Backwards(data);
            PCATransform.Backwards(data, 0);
            //Residualizer.UndoResiduals(data, 1);

            return result;
        }

        public TestResult Three(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.AngleSubsample(data, 4);
            //Residualizer.DoResiduals(data, 1);
            DCTransformer.Forward(data, 0);

            //int zeroCount = GetZeroCount(data, qualityLevel + 16);
            //DCTQuantization.SetToZero(data, zeroCount);
            Discretizer.Divide(data, 1000, 0);
            Discretizer.DivideLinear(data, 2 * (qualityLevel + 10), 0);
            Discretizer.ToInt(data, 0);

            StandardWriter.WriteIntFits(data, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            Discretizer.MultiplyLinear(data, 2 * ((qualityLevel +1)* 10), 0);
            Discretizer.Multiply(data, 1000, 0);
            DCTransformer.Backward(data, 0);
            //Residualizer.UndoResiduals(data, 1);

            return result;
        }

        public TestResult Four(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.Subsample(data, 4);
            Residualizer.DoResiduals(data, 1);
            PCATransform.Forward(data, 1);
            //YCbCr.ForwardFull(data, 0);
            DCTransformer.Forward(data, 1);

            //Residualizer.DoResiduals(data, 3);
            
            Discretizer.DividePoint(data, 70000, 0);
            Discretizer.Divide(data, 1000, 1);
            Discretizer.DivideLinear(data, 2 * (qualityLevel + 7), 1);
            PCACoefficient.ForwardQuantization(data);
            Discretizer.ToShorts(data, 1);
            /*DebugOutput.MedianWriter.AnalyzePerCurveType(data,TYPE.OUTSIDE_TO_SUN,0,new FileInfo(Path.Combine(folder, this.GetName()+"_ots.csv")));
            DebugOutput.MedianWriter.AnalyzePerCurveType(data, TYPE.SUN_TO_OUTSIDE, 0, new FileInfo(Path.Combine(folder, this.GetName() + "_sto.csv")));
            DebugOutput.MedianWriter.AnalyzePerCurveType(data, TYPE.SUN_TO_SUN, 0, new FileInfo(Path.Combine(folder, this.GetName() + "_sts.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve.csv")));*/
            PCAWriter.WritePureShortFits(data,1, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            PCACoefficient.BackwardQuantization(data);
            Discretizer.MultiplyLinear(data, 2 * (qualityLevel + 7), 1);
            Discretizer.Multiply(data, 1000, 1);
            //Residualizer.UndoResiduals(data, 3);
            Discretizer.MultiplyPoint(data, 70000, 0);
            DCTransformer.Backward(data, 1);
            //YCbCr.BackwardsFull(data, 0);
            //PCACoefficient.Backwards(data);
            PCATransform.Backwards(data, 1);
            Residualizer.UndoResiduals(data, 1);

            return result;
        }

        public TestResult Five(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.Subsample(data, 4);
            
            PCATransform.Forward(data, 0);
            Residualizer.DoResiduals(data, 1);
            //YCbCr.ForwardFull(data, 0);
            DCTransformer.Forward(data, 1);

            Discretizer.DividePoint(data, 70000, 0);
            Discretizer.Divide(data, 1000, 1);
            Discretizer.DivideLinear(data, 2 * (qualityLevel + 6), 1);
            Discretizer.ToShorts(data, 1);

            PCAWriter.WriteFits(data, 1, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            Discretizer.MultiplyLinear(data, 2 * (qualityLevel + 6), 1);
            Discretizer.Multiply(data, 1000, 1);
            //Residualizer.UndoResiduals(data, 3);
            Discretizer.MultiplyPoint(data, 70000, 0);
            DCTransformer.Backward(data, 1);
            //PCACoefficient.Backwards(data);
            Residualizer.UndoResiduals(data, 1);
            PCATransform.Backwards(data, 0);

            return result;
        }


        public TestResult Six(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.Subsample(data, 4);
            Residualizer.DoResiduals(data, 1);
            DCTransformer.Forward(data, 1);

            //int zeroCount = GetZeroCount(data, qualityLevel + 16);
            //DCTQuantization.SetToZero(data, zeroCount);
            Discretizer.DividePoint(data, 50000, 0);
            Discretizer.Divide(data, 1000, 1);
            Discretizer.DivideLinear(data, 2 * (qualityLevel + 7), 1);
            Discretizer.ToShorts(data, 1);


            InterleavedWriter.WriteDCTByteFits(data, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            Discretizer.MultiplyLinear(data, 2 * (qualityLevel + 7), 1);
            Discretizer.Multiply(data, 1000, 1);
            Discretizer.MultiplyPoint(data, 50000, 0);
            DCTransformer.Backward(data, 1);
            Residualizer.UndoResiduals(data, 1);

            return result;
        }
        #endregion
    }
}
