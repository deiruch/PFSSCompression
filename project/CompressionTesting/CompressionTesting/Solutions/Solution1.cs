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
    class Solution1 : ISolution
    {
        public int GetQualityLevels()
        {
            return 11;
        }

        public string GetName()
        {
            return "Solution1_Seven";
        }

        public TestResult DoTestRun(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            
            return Seven(data, qualityLevel, folder);
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
            Random r = new Random();
            int rand = r.Next();
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + rand + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + rand + ".rar"));
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
            PCATransform.Backward(data, 0);
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
            Discretizer.DivideLinear(data, 2 * (qualityLevel + 9), 1);
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
            Discretizer.MultiplyLinear(data, 2 * (qualityLevel + 9), 1);
            Discretizer.Multiply(data, 1000, 1);
            //Residualizer.UndoResiduals(data, 3);
            Discretizer.MultiplyPoint(data, 70000, 0);
            DCTransformer.Backward(data, 1);
            //YCbCr.BackwardsFull(data, 0);
            //PCACoefficient.Backwards(data);
            PCATransform.Backward(data, 1);
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

            Discretizer.DividePoint(data, 60000, 0);
            Discretizer.Divide(data, 1000, 1);

            //Optimum: qualitylevel 8
            PCACoefficient.DivideLinear(data, 0, (qualityLevel + 12), (qualityLevel + 12), 1, 800);
            PCACoefficient.DivideLinear(data, 1, (qualityLevel + 13), (qualityLevel + 13), 1, 800);
            PCACoefficient.DivideLinear(data, 2, (qualityLevel + 14), (qualityLevel + 14), 1, 800);
            Discretizer.ToShorts(data, 1);

            PCAWriter.WritePureShortFits(data, 1, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            PCACoefficient.MultiplyLinear(data, 0, (qualityLevel + 12), (qualityLevel + 12), 1, 800);
            PCACoefficient.MultiplyLinear(data, 1, (qualityLevel + 13), (qualityLevel + 13), 1, 800);
            PCACoefficient.MultiplyLinear(data, 2, (qualityLevel + 14), (qualityLevel + 14), 1, 800);
            Discretizer.Multiply(data, 1000, 1);
            //Residualizer.UndoResiduals(data, 3);
            Discretizer.MultiplyPoint(data, 60000, 0);
            DCTransformer.Backward(data, 1);
            Residualizer.UndoResiduals(data, 1);
            PCATransform.Backward(data, 0);

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

            foreach (PFSSLine l in data.lines)
            {
                Spherical.ForwardToSpherical(l.points[0]);
                Spherical.ForwardMoveSpherical(l.points[0]);
            }
            Discretizer.HandleR(data, 0, 8192 * 0.05);
            Discretizer.Divide(data, 1000, 1);
            Discretizer.DivideLinear(data, 2 * (qualityLevel + 2), 1);
            Discretizer.ToShorts(data, 1);

            InterleavedWriter.WriteDCTByteFits(data, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            Discretizer.MultiplyLinear(data, 2 * (qualityLevel + 2), 1);
            Discretizer.Multiply(data, 1000, 1);
            //Discretizer.MultiplyPoint(data, 50000, 0);
            foreach (PFSSLine l in data.lines)
            {
                Spherical.BackwardMoveSpherical(l.points[0]);
                Spherical.BackwardToSpherical(l.points[0],data);
            }
            DCTransformer.Backward(data, 1);
            Residualizer.UndoResiduals(data, 1);

            return result;
        }

        public TestResult Seven(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.Subsample(data, 4);
            DCTImprover.AddExtraPoints(data, 100000);
            DCTransformer.ForwardExtra(data, 0);

            ExtraPointDiscretizer.DivideExtra(data,1000);
            ExtraPointDiscretizer.DivideLinearExtra(data, 0, 2 * (qualityLevel * 2 + 18), 0, 1);
            ExtraPointDiscretizer.DivideLinearExtra(data, 1, 2 * (qualityLevel * 2 + 18), 0, 1);
            ExtraPointDiscretizer.DivideLinearExtra(data, 2, 2 * (qualityLevel * 2 + 18), 0, 1);
            ExtraPointDiscretizer.ToIntsExtra(data);
            //ExtraPointDiscretizer.ToShortsExtra(data);

            StandardWriter.WriteDCTByteFits(data, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            //140,180
            ExtraPointDiscretizer.MultiplyLinearExtra(data, 0, 2 * (qualityLevel * 2 + 18), 0, 1);
            ExtraPointDiscretizer.MultiplyLinearExtra(data, 1, 2 * (qualityLevel * 2 + 18), 0, 1);
            ExtraPointDiscretizer.MultiplyLinearExtra(data, 2, 2 * (qualityLevel * 2 + 18), 0, 1);
            ExtraPointDiscretizer.MultiplyExtra(data, 1000);

            DCTransformer.BackwardExtra(data, 0);

            return result;
        }

        public TestResult Eight(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.Subsample(data, 4);
            Residualizer.DoResiduals(data, 1);
            DCTransformer.Forward(data, 1);

            foreach (PFSSLine l in data.lines)
            {
                Spherical.ForwardToSpherical(l.points[0]);
                Spherical.ForwardMoveSpherical(l.points[0]);
            }

            Discretizer.HandleR(data, 0, 8192 * 0.05);
            Discretizer.Divide(data, 500, 1);
            Discretizer.DivideLinear(data, 5+(5*qualityLevel), 5, 1, 10);
            Discretizer.DivideLinear(data, 55+(5 * qualityLevel), 0, 11, 8);
            Discretizer.DivideLinear(data, 60+(5 * qualityLevel), 0, 19, 7);
            Discretizer.DivideLinear(data, 30+(5 * qualityLevel), 0, 26, 30);

            //Discretizer.DivideLinear(data, 400, 20, 21, 15);
            Discretizer.Cut(data, 46);
            Discretizer.ToShorts(data, 1);

            InterleavedWriter.WriteDCTByteFits(data, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;
            
            Discretizer.MultiplyLinear(data, 5+(5 * qualityLevel), 5, 1, 10);
            Discretizer.MultiplyLinear(data, 55+(5 * qualityLevel), 0, 11, 8);
            Discretizer.MultiplyLinear(data, 60+(5 * qualityLevel), 0, 19, 7);
            Discretizer.MultiplyLinear(data, 30+(5 * qualityLevel), 0, 26, 30);
            Discretizer.Multiply(data, 500, 1);
            //Discretizer.MultiplyPoint(data, 50000, 0);
            foreach (PFSSLine l in data.lines)
            {
                Spherical.BackwardMoveSpherical(l.points[0]);
                Spherical.BackwardToSpherical(l.points[0], data);
            }
            DCTransformer.Backward(data, 1);
            Residualizer.UndoResiduals(data, 1);

            return result;
        }

        public TestResult Nine(PFSS.PFSSData data, int qualityLevel, string folder)
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

            foreach (PFSSLine l in data.lines)
            {
                Spherical.ForwardToSpherical(l.points[0]);
                Spherical.ForwardMoveSpherical(l.points[0]);
            }
            Discretizer.Divide(data, 1000, 1);
            Discretizer.DivideLinear(data, 20, 5, 1, 10);
            Discretizer.DivideLinear(data, 60, 0, 11, 8);
            Discretizer.DivideLinear(data, 50, 0, 19, 7);
            Discretizer.DivideLinear(data, 10, 0, 26, 45);

            Discretizer.Cut(data, 71);
            PCACoefficient.ForwardQuantization(data);
            Discretizer.ToShorts(data, 1);

            PCAWriter.WritePureByteFits(data, 1, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            PCACoefficient.BackwardQuantization(data);
            Discretizer.MultiplyLinear(data, 20, 5, 1, 10);
            Discretizer.MultiplyLinear(data, 60, 0, 11, 8);
            Discretizer.MultiplyLinear(data, 50, 0, 19, 7);
            Discretizer.MultiplyLinear(data, 10, 0, 26, 45);
            Discretizer.Multiply(data, 1000, 1);
            //Residualizer.UndoResiduals(data, 3);
            foreach (PFSSLine l in data.lines)
            {
                Spherical.BackwardMoveSpherical(l.points[0]);
                Spherical.BackwardToSpherical(l.points[0], data);
            }
            DCTransformer.Backward(data, 1);
            //YCbCr.BackwardsFull(data, 0);
            //PCACoefficient.Backwards(data);
            PCATransform.Backward(data, 1);
            Residualizer.UndoResiduals(data, 1);

            return result;
        }

        public TestResult Ten(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.Subsample(data, 4);
            PCATransform.Forward(data, 0);
            LinearPredictor.Forward(data);
            //DCTransformer.Forward(data, 2);
            DebugOutput.MedianWriter.AnalyzePerCurveType(data,TYPE.OUTSIDE_TO_SUN,0,new FileInfo(Path.Combine(folder, this.GetName()+"_ots.csv")));
            DebugOutput.MedianWriter.AnalyzePerCurveType(data, TYPE.SUN_TO_OUTSIDE, 0, new FileInfo(Path.Combine(folder, this.GetName() + "_sto.csv")));
            DebugOutput.MedianWriter.AnalyzePerCurveType(data, TYPE.SUN_TO_SUN, 0, new FileInfo(Path.Combine(folder, this.GetName() + "_sts.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve.csv")));

            //DCTransformer.Backward(data, 2);
            LinearPredictor.Backward(data);
            PCATransform.Backward(data, 1);

            return result;
        }

        public TestResult Eleven(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();

            Subsampling.Subsample(data, 4);
            PCATransform.Forward(data, 0);
            DCTImprover.AddExtraPoints(data, 100000);
            DCTransformer.ForwardExtra(data, 0);

            //ExtraPointDiscretizer.DividePoint(data, 10, 0);
            ExtraPointDiscretizer.DivideExtra(data, 1000);
            ExtraPointDiscretizer.DivideLinearExtra(data, 0, 2 * (qualityLevel * 2 + 20), 0, 1);
            ExtraPointDiscretizer.DivideLinearExtra(data, 1, 2 * (qualityLevel * 2 + 35), 0, 1);
            ExtraPointDiscretizer.DivideLinearExtra(data, 2, 2 * (qualityLevel * 2 + 40), 0, 2);
            ExtraPointDiscretizer.ToShortsExtra(data);
            PCACoefficient.ForwardQuantization(data, 50000);
            //ExtraPointDiscretizer.ToShortsExtra(data);

            StandardWriter.WritePCADCTByteFits(data, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            //140,180
            PCACoefficient.BackwardQuantization(data, 50000);
            ExtraPointDiscretizer.MultiplyLinearExtra(data, 0, 2 * (qualityLevel * 2 + 20), 0, 1);
            ExtraPointDiscretizer.MultiplyLinearExtra(data, 1, 2 * (qualityLevel * 2 + 35), 0, 1);
            ExtraPointDiscretizer.MultiplyLinearExtra(data, 2, 2 * (qualityLevel * 2 + 40), 0, 2);
            ExtraPointDiscretizer.MultiplyExtra(data, 1000);
            //ExtraPointDiscretizer.MultiplyPoint(data, 10, 0);

            DCTransformer.BackwardExtra(data, 0);
            PCATransform.Backward(data, 0);

            return result;
        }
        #endregion
    }
}
