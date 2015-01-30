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
        private static int counter = 0;
        public int GetQualityLevels()
        {
            return 1;
        }

        public string GetName()
        {
            return "Solution2_five";
        }

        public TestResult DoTestRun(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            return Five(data, qualityLevel, folder);
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
            //Subsampling.AngleSubsample(data, 3);
            //PCATransform.Forward(data, 0);
            //PCACoefficient.ForwardQuantization(data,50000);

            foreach (PFSSLine l in data.lines)
            {
                for (int i = 0; i < l.points.Count; i++)
                {
                    Spherical.ForwardToSpherical(l.points[i]);
                    Spherical.ForwardMoveSpherical(l.points[i]);
                }
            }
            //Discretizer.ToShorts(data, 0);
            LinearPredictor.ToShortsHard(data);
            LinearPredictor.Forward(data);
            //LinearPredictor.MovAveragePredictorForwards(data, 1);
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_OUTSIDE, new FileInfo(Path.Combine(folder, this.GetName() + "_sto_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_sts_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve_disk.csv")));
            InterleavedWriter.WriteFits(data, fits, offset);
            //PCAWriter.WriteFits(data, offset, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            LinearPredictor.Backward(data);
            //LinearPredictor.MovAveragePredictorBackwards(data, 1);
            foreach (PFSSLine l in data.lines)
            {
                for (int i = 0; i < l.points.Count; i++)
                {
                    Spherical.BackwardMoveSpherical(l.points[i]);
                    Spherical.BackwardToSpherical(l.points[i], data);
                }
            }
            //PCACoefficient.BackwardQuantization(data, 50000);

            //PCATransform.Backward(data, 0);

            return result;
        }

        public TestResult Two(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();
            int offset = 2;
            Subsampling.Subsample(data, 4);
            //Subsampling.AngleSubsample(data,3);

            PCATransform.Forward(data, 0);
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_OUTSIDE, new FileInfo(Path.Combine(folder, this.GetName() + "_sto_curve.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_sts_curve.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve.csv")));
            PCACoefficient.ForwardQuantization(data, 50000);

            Discretizer.Divide(data, 50000, 0);
            
            switch (qualityLevel)
            {
                case 0:
                    LinearPredictor.Forward(data);
                 
                    offset = 1;
                    break;
                case 1:
                    LinearPredictor.ForwardAdaptive(data);
                    offset = 1;
                    break;
                case 2:
                    LinearPredictor.MovAveragePredictorForwards(data, 1);
                    break;
                case 3:
                    LinearPredictor.MovAveragePredictorForwards(data, 4);
                    break;
                case 4:
                    LinearPredictor.AdaptiveMovAveragePredictorForwards(data, 4);
                    break;
                case 5:
                    break;
            }
            //LinearPredictor.ToShorts(data);
            LinearPredictor.ToShortsHard(data);

            InterleavedWriter.WriteFits(data, fits, offset);
            PCAWriter.WritePureShortFits(data, offset, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;
            switch (qualityLevel)
            {
                case 0:
                    LinearPredictor.Backward(data);
                    break;
                case 1:
                    LinearPredictor.BackwardAdaptive(data);
                    break;
                case 2:
                    LinearPredictor.MovAveragePredictorBackwards(data, 1);
                    break;
                case 3:
                    LinearPredictor.MovAveragePredictorBackwards(data, 4);
                    break;
                case 4:
                    LinearPredictor.AdaptiveMovAveragePredictorBackwards(data, 4);
                    break;
                case 5:
                    break;
            }
            PCACoefficient.BackwardQuantization(data, 50000);
            Discretizer.Multiply(data, 50000, 0);

            PCATransform.Backward(data, 0);

            return result;
        }

        public TestResult Three(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();
            int offset = 2;
            Subsampling.Subsample(data, 4);
            //Subsampling.AngleSubsample(data, 3);

            PCATransform.Forward(data, 0);
            //Residualizer.DoResiduals(data, 1);
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_OUTSIDE, new FileInfo(Path.Combine(folder, this.GetName() + "_sto_curve.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_sts_curve.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve.csv")));
            PCACoefficient.ForwardQuantization(data, 50000);

            Discretizer.DividePoint(data, 50000, 0);
            Discretizer.Divide(data, 50000, 1);
            Discretizer.ToShorts(data, 0);
            switch(qualityLevel)
            {
                case 0:
                    LinearPredictor.Forward(data);
                    offset = 1;
                    break;
                case 1:
                    LinearPredictor.ForwardAdaptive(data);
                    offset = 1;
                    break;
                case 2:
                    LinearPredictor.MovAveragePredictorForwards(data, 1);
                    break;
                case 3:
                    LinearPredictor.MovAveragePredictorForwards(data, 4);
                    break;
                case 4:
                    LinearPredictor.AdaptiveMovAveragePredictorForwards(data, 4);
                    break;
                case 5:
                    break;
            }

            //LinearPredictor.ToShorts(data);
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_OUTSIDE, new FileInfo(Path.Combine(folder, this.GetName() + "_sto_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_sts_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve_disk.csv")));

            //InterleavedWriter.WriteFits(data, fits, offset);
            PCAWriter.WritePureShortFits(data, offset, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            switch (qualityLevel)
            {
                case 0:
                    LinearPredictor.Backward(data);
                    break;
                case 1:
                    LinearPredictor.BackwardAdaptive(data);
                    break;
                case 2:
                    LinearPredictor.MovAveragePredictorBackwards(data, 1);
                    break;
                case 3:
                    LinearPredictor.MovAveragePredictorBackwards(data, 4);
                    break;
                case 4:
                    LinearPredictor.AdaptiveMovAveragePredictorBackwards(data, 4);
                    break;
                case 5:
                    break;
            }
            PCACoefficient.BackwardQuantization(data, 50000);
            Discretizer.Multiply(data, 50000, 1);
            Discretizer.MultiplyPoint(data, 50000, 0);
            //Residualizer.UndoResiduals(data, 1);
            PCATransform.Backward(data, 0);

            return result;
        }

        public TestResult Four(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();
            int offset = 2;
            Subsampling.Subsample(data, 4);
            //Subsampling.AngleSubsample(data, 3);

            PCATransform.Forward(data, 0);

            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_OUTSIDE, new FileInfo(Path.Combine(folder, this.GetName() + "_sto_curve.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_sts_curve.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve.csv")));
            PCACoefficient.ForwardQuantization(data, 50000);

            Discretizer.DividePoint(data, 50000, 0);
            Discretizer.Divide(data, 50000, 1);

            Discretizer.ToShorts(data, 0);
            switch (qualityLevel)
            {
                case 0:
                    LinearPredictor.Forward(data);
                    offset = 1;
                    break;
                case 1:
                    LinearPredictor.ForwardAdaptive(data);
                    offset = 1;
                    break;
                case 2:
                    LinearPredictor.MovAveragePredictorForwards(data, 1);
                    break;
                case 3:
                    LinearPredictor.MovAveragePredictorForwards(data, 4);
                    break;
                case 4:
                    LinearPredictor.AdaptiveMovAveragePredictorForwards(data, 4);
                    break;
                case 5:
                    break;
            }
            //LinearPredictor.ToShorts(data);
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_OUTSIDE, new FileInfo(Path.Combine(folder, this.GetName() + "_sto_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_sts_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve_disk.csv")));

            //InterleavedWriter.WriteFits(data, fits, offset);
            PCAWriter.WritePureShortFits(data, offset, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            //LinearPredictor.Backward(data);
            switch (qualityLevel)
            {
                case 0:
                    LinearPredictor.Backward(data);
                    break;
                case 1:
                    LinearPredictor.BackwardAdaptive(data);
                    break;
                case 2:
                    LinearPredictor.MovAveragePredictorBackwards(data, 1);
                    break;
                case 3:
                    LinearPredictor.MovAveragePredictorBackwards(data, 4);
                    break;
                case 4:
                    LinearPredictor.AdaptiveMovAveragePredictorBackwards(data, 4);
                    break;
                case 5:
                    break;
            }
            PCACoefficient.BackwardQuantization(data, 50000);
            Discretizer.Multiply(data, 50000, 1);
            Discretizer.MultiplyPoint(data, 50000, 0);
            PCATransform.Backward(data, 0);

            return result;
        }


        public TestResult Five(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + counter++ + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();
            int offset = 1;
            //Subsampling.Subsample(data, 4);
            Subsampling.AngleSubsample(data, 3);

            //PCATransform.Forward(data, 0);
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_OUTSIDE, new FileInfo(Path.Combine(folder, this.GetName() + "_sto_curve.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_sts_curve.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve.csv")));
            /*PCACoefficient.ForwardQuantization(data, 50000);
            Discretizer.DividePoint(data, 50000, 0);
            Discretizer.Divide(data, 50000, 1);
            Discretizer.ToShorts(data, 0);*/
            foreach (PFSSLine l in data.lines)
            {
                for (int i = 0; i < l.points.Count; i++)
                {
                    Spherical.ForwardToSpherical(l.points[i]);
                    Spherical.ForwardMoveSpherical(l.points[i]);
                }
            }
            qualityLevel += 5;
            Residuals.factor = qualityLevel+1;
            Residuals.factor2 = qualityLevel + 5;
            Residuals.factor3 = qualityLevel + 11;

            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_OUTSIDE, new FileInfo(Path.Combine(folder, this.GetName() + "_sto_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_sts_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve_disk.csv")));
            Residuals.ForwardPrediction(data);

            //PredictiveResidualWriter.WritePureShortFits(data, offset, fits);
            PredictiveResidualWriter.WritePureShortFits_WithoutPCA(data, offset, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            Residuals.BackwardPrediction(data);
            /*PCACoefficient.BackwardQuantization(data, 50000);
            Discretizer.Multiply(data, 50000, 1);
            Discretizer.MultiplyPoint(data, 50000, 0);
            PCATransform.Backward(data, 0);*/
            foreach (PFSSLine l in data.lines)
            {
                for (int i = 0; i < l.points.Count; i++)
                {
                    Spherical.BackwardMoveSpherical(l.points[i]);
                    Spherical.BackwardToSpherical(l.points[i], data);
                }
            }

            return result;
        }

        public TestResult Six(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();
            int offset = 2;
            //Subsampling.Subsample(data, 4);
            Subsampling.AngleSubsample(data, 3);
            //PCATransform.Forward(data, 0);
            //PCACoefficient.ForwardQuantization(data,50000);

            foreach (PFSSLine l in data.lines)
            {
                for (int i = 0; i < l.points.Count; i++)
                {
                    Spherical.ForwardToSpherical(l.points[i]);
                    Spherical.ForwardMoveSpherical(l.points[i]);
                }
            }
            //Discretizer.ToShorts(data, 0);
            LinearPredictor.ToShortsHard(data);
            switch (qualityLevel)
            {
                case 0:
                    LinearPredictor.Forward(data);
                    break;
                case 1:
                    LinearPredictor.ForwardAdaptive(data);
                    break;
                case 2:
                    LinearPredictor.MovAveragePredictorForwards(data, 1);
                    break;
                case 3:
                    LinearPredictor.MovAveragePredictorForwards(data, 4);
                    break;
                case 4:
                    LinearPredictor.AdaptiveMovAveragePredictorForwards(data, 4);
                    break;
                case 5:
                    break;
            }
            //LinearPredictor.MovAveragePredictorForwards(data, 1);
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_OUTSIDE, new FileInfo(Path.Combine(folder, this.GetName() + "_sto_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_sts_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve_disk.csv")));
            InterleavedWriter.WriteFits(data, fits, 1);
            //PCAWriter.WriteFits(data, offset, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            switch (qualityLevel)
            {
                case 0:
                    LinearPredictor.Backward(data);
                    break;
                case 1:
                    LinearPredictor.BackwardAdaptive(data);
                    break;
                case 2:
                    LinearPredictor.MovAveragePredictorBackwards(data, 1);
                    break;
                case 3:
                    LinearPredictor.MovAveragePredictorBackwards(data, 4);
                    break;
                case 4:
                    LinearPredictor.AdaptiveMovAveragePredictorBackwards(data, 4);
                    break;
                case 5:
                    break;
            }
            //LinearPredictor.MovAveragePredictorBackwards(data, 1);
            foreach (PFSSLine l in data.lines)
            {
                for (int i = 0; i < l.points.Count; i++)
                {
                    Spherical.BackwardMoveSpherical(l.points[i]);
                    Spherical.BackwardToSpherical(l.points[i], data);
                }
            }
            //PCACoefficient.BackwardQuantization(data, 50000);

            //PCATransform.Backward(data, 0);

            return result;
        }


        public TestResult Seven(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, this.GetName() + counter++ + ".fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, this.GetName() + qualityLevel + ".rar"));
            TestResult result = new TestResult();
            int offset = 1;
            //Subsampling.Subsample(data, 4);
            Subsampling.AngleSubsample(data, 3);
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_OUTSIDE, new FileInfo(Path.Combine(folder, this.GetName() + "_sto_curve.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_sts_curve.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve.csv")));
            //PCATransform.Forward(data, 0);
            //PCACoefficient.ForwardQuantization(data, 50000);
            Discretizer.DividePoint(data, 50000, 0);
            Discretizer.Divide(data, 50000, 1);
            Discretizer.ToInt(data, 0);

            //qualityLevel += 3;
            Residuals.factor = 100000;
            Residuals.factor2 = 64;

            Residuals.ForwardPrediction(data);

            //PredictiveResidualWriter.WritePureShortFits(data, offset, fits);
            PredictiveResidualWriter.WritePureShortFits_WithoutPCA(data, offset, fits);
            long size = RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            Residuals.BackwardPrediction(data);
            //PCACoefficient.BackwardQuantization(data, 50000);
            Discretizer.Multiply(data, 50000, 1);
            Discretizer.MultiplyPoint(data, 50000, 0);

            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_OUTSIDE, new FileInfo(Path.Combine(folder, this.GetName() + "_sto_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.SUN_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_sts_curve_disk.csv")));
            DebugOutput.MedianWriter.AnalyzeFirstCurveType(data, TYPE.OUTSIDE_TO_SUN, new FileInfo(Path.Combine(folder, this.GetName() + "_ots_curve_disk.csv")));
            //PCATransform.Backward(data, 0);

            return result;
        }
    }
}
