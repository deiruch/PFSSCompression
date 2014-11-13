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
            FileInfo fits = new FileInfo(Path.Combine(folder,"standardShort.fits"));
            FileInfo rarFits = new FileInfo(Path.Combine(folder, "standardShort.rar"));
            TestResult result = new TestResult();

            //forward
            //AngleSubsampling.Subsample(data, 5.0);
            Spherical.ForwardToSpherical(data);
            //Discretizer.ToShorts(data);
            //StandardShortWriter.WriteFits(data, fits);
            long size = 0;// RarCompression.DoRar(rarFits, fits);
            result.fileSize = size;
            result.lineCount = data.lines.Count;

            //backward
            Spherical.BackwardToSpherical(data);

            return result;
        }

        public void SelectIntermediateSolution(int i)
        {
            throw new NotImplementedException();
        }
    }
}
