using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CompressionTesting.Solutions;
using CompressionTesting.PFSS;
using CompressionTesting.PFSS.Test;

namespace CompressionTesting
{
    class Program
    {
         

        static void Main(string[] args)
        {
            ISolution solution = new Solution1();
            bool testOneFile = false;

            string fitsOutputFolder = @"C:\Users\Jonas Schwammberger\Documents\GitHub\PFSSCompression\test\temp";
            string outputFolder = @"C:\Users\Jonas Schwammberger\Documents\GitHub\PFSSCompression\test\testresult";
            string[] expectedFiles = Directory.GetFiles(@"C:\dev\git\bachelor\test\testdata\raw");
            TestSuite[] testData = testOneFile ? new TestSuite[1] : new TestSuite[expectedFiles.Length];
        

            StreamWriter w = new StreamWriter(new FileStream(Path.Combine(outputFolder,solution.GetName()+".csv"), FileMode.Create));
            w.Write("Average Line Size (Bytes);Max Error(Meters);standard deviation (Meters)");

            //load data
            for (int i = 0; i < testData.Length; i++)
            {
                testData[i] = FitsReader.ReadFloatFits(new FileInfo(expectedFiles[i]));
            }
           
            //do tests
            int qualityLevels = solution.GetQualityLevels();
            for (int i = 0; i < qualityLevels; i++)
            {
                TestResult[] result = new TestResult[testData.Length];
                PFSSData[] data = new PFSSData[testData.Length];
                for (int j = 0; j < data.Length; j++)
                {
                    data[j] = testData[j].GetData();
                    result[j] = solution.DoTestRun(data[j], i, fitsOutputFolder);
                }

                Tuple<double, double> overall = ErrorCalculator.CalculateOverallError(testData, data);
                long lineCount = 0;
                long fileSize = 0;
                foreach (TestResult res in result)
                {
                    fileSize += res.fileSize;
                    lineCount += res.lineCount;
                }

                //calculate overall line size 
                double averageLineSize = fileSize / (double)lineCount;

                w.Write("\n");
                w.Write(averageLineSize + ";" + overall.Item1 + ";" + overall.Item2);
            }

            w.Close();
        }


        private static PFSSData[] Create()
        {
            PFSSPoint p = new PFSSPoint(1, 1, 0);
            PFSSPoint p1 = new PFSSPoint(2, 2, 0);
            PFSSPoint p2 = new PFSSPoint(3, 3, 0);

            PFSSPoint p3 = new PFSSPoint(1.9f, 1.1f, 0);
            PFSSPoint p4 = new PFSSPoint(2.5f, 3, 0);

            List<PFSSPoint> points0 = new List<PFSSPoint> { p, p1, p2 };
            List<PFSSPoint> points1= new List<PFSSPoint> { p3, p4 };

            List<PFSSLine> line0 = new List<PFSSLine> { new PFSSLine(TYPE.OUTSIDE_TO_SUN, points0) };
            List<PFSSLine> line1 = new List<PFSSLine> { new PFSSLine(TYPE.OUTSIDE_TO_SUN, points1) };

            return new PFSSData[] { new PFSSData(0, 0, line0), new PFSSData(0, 0, line1) };
        }
    }
}
