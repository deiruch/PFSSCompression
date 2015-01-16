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
using CompressionTesting.DebugOutput;

namespace CompressionTesting.Solutions
{
    class Solution2 : ISolution
    {
        public int GetQualityLevels()
        {
            return 1;
        }

        public string GetName()
        {
            return "Solution2_two";
        }

        public TestResult DoTestRun(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            return Two(data, qualityLevel, folder);
        }

        public void SelectIntermediateSolution(int i)
        {
            throw new NotImplementedException();
        }

        public TestResult One(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();
            int offset = 2;
            Subsampling.Subsample(data, 4);
            //PCATransform.Forward(data, 0);
            //PCACoefficient.ForwardQuantization(data,50000);
            
            Discretizer.Divide(data, 50000, 0);
            Discretizer.ToShorts(data, 0);
            LinearPredictor.Forward(data);

            //DCTransformer.Forward(data, 2);
            /*DebugOutput.MedianWriter.AnalyzePerCurveType(data, TYPE.OUTSIDE_TO_SUN, 0, new FileInfo(Path.Combine(folder, this.GetName() + "_ots.csv")));
            DebugOutput.MedianWriter.AnalyzePerCurveType(data, TYPE.SUN_TO_OUTSIDE, 0, new FileInfo(Path.Combine(folder, this.GetName() + "_sto.csv")));
            DebugOutput.MedianWriter.AnalyzePerCurveType(data, TYPE.SUN_TO_SUN, 0, new FileInfo(Path.Combine(folder, this.GetName() + "_sts.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve.csv")));*/
            InterleavedWriter.WriteFits(data, fits, offset);
            //PCAWriter.WriteFits(data, offset, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            LinearPredictor.Backward(data);
            Discretizer.Multiply(data, 50000, 0);

            
            //PCATransform.Backward(data, 0);

            return result;
        }

        public TestResult Two(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();
            int offset = 2;
            Subsampling.AngleSubsample(data,3);

            PCATransform.Forward(data, 0);
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_OUTSIDE, new FileInfo(Path.Combine(folder, this.GetName() + "_sto_curve.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_sts_curve.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve.csv")));
            PCACoefficient.ForwardQuantization(data,50000);

            
            Discretizer.Divide(data, 50000, 0);
            Discretizer.ToShorts(data, 0);
            LinearPredictor.BetterLinearPredictorForward(data, 0);

            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_OUTSIDE, new FileInfo(Path.Combine(folder, this.GetName() + "_sto_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_sts_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve_disk.csv")));

            InterleavedWriter.WriteFits(data, fits, offset);
            PCAWriter.WritePureShortFits(data, offset, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            LinearPredictor.BetterLinearPredictorBackwards(data, 0);
            PCACoefficient.BackwardQuantization(data, 50000);
            Discretizer.Multiply(data, 50000, 0);
            
            PCATransform.Backward(data, 0);

            return result;
        }
    }
}
