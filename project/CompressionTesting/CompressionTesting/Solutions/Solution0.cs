using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.Quantization;
using CompressionTesting.FileWriter;
using CompressionTesting.Compression;
using CompressionTesting.Transformation;
using System.IO;

namespace CompressionTesting.Solutions
{
    class Solution0 : ISolution
    {
        public int GetQualityLevels()
        {
            return 1;
        }

        public string GetName()
        {
            return "ServerSubsampling";
        }

        public TestResult DoTestRun(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            return Zero(data, qualityLevel, folder);
        }

        public void SelectIntermediateSolution(int i)
        {
            throw new NotImplementedException();
        }

        public TestResult Zero(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, "standardShort.fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, "standardShort.rar"));
            TestResult result = new TestResult();

            //forward
            Subsampling.AngleSubsample(data, 5.0);
            //DebugOutput.Printer.PrintLine(data,0,new FileInfo(Path.Combine(folder,"line.csv")));
            Spherical.ForwardToSpherical(data);
            Discretizer.ToShorts(data, 0);
            StandardWriter.WriteShortFits(data, fits);
            long size = RarCompression.DoRar(rarFits, fits);

            result.pointCount = 0;
            for (int i = 0; i < data.lines.Count; i++)
            {
                result.pointCount += data.lines[i].points.Count;
            }
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            //backward
            Spherical.BackwardToSpherical(data);

            return result;
        }

        public TestResult One(PFSS.PFSSData data, int qualityLevel, string folder)
        {
            FileInfo fits = new FileInfo(Path.Combine(folder, "standardShort.fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, "standardShort.rar"));
            TestResult result = new TestResult();

            //forward
            Subsampling.AngleSubsample(data, 5.0);
            //Residualizer.DoResiduals(data, 1);
            PCATransform.Forward(data, 0);
            Discretizer.Divide(data, 50000, 0);
            //Discretizer.ToShorts(data, 0);
            //StandardWriter.WriteShortFits(data, fits);
            PCAWriter.WriteFits(data, 0, fits);
            long size = RarCompression.DoRar(rarFits, fits);

            result.pointCount = 0;
            for (int i = 0; i < data.lines.Count; i++)
            {
                result.pointCount += data.lines[i].points.Count;
            }
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            //backward
            Discretizer.Multiply(data, 50000, 0);
            PCATransform.Backward(data,0);
            //Residualizer.UndoResiduals(data, 1);

            return result;
        }
    }
}
