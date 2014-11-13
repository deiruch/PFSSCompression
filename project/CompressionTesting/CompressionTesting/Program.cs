using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CompressionTesting.Solutions;
using CompressionTesting.PFSS;

namespace CompressionTesting
{
    class Program
    {
         

        static void Main(string[] args)
        {
            PFSSData[] d1 = {FitsReader.ReadFits(new FileInfo(@"C:\dev\2014-06-10_06-04-32.000_pfss_field_data.fits"), false)};
            PFSSData[] d0 = { FitsReader.ReadFloatFits(new FileInfo(@"C:\dev\2014-06-10_06-04-32.000_pfss_field_data_flt.fits"), false) };
            /*PFSSData[] d = Create();
            d1[0] = d[1];
            d0[0] = d[0];*/

            //Tuple<double,double> err = ErrorCalculator.CalculateOverallError(d0, d1);

            ISolution solution = new Solution0();
            string fitsOutputFolder = @"C:\dev\git\bachelor\test\temp";
            string outputFolder = @"C:\dev\git\bachelor\test\testresult";
            string[] expectedFiles = Directory.GetFiles(@"C:\dev\git\bachelor\test\testdata\raw");
            string[] testFiles = Directory.GetFiles(@"C:\dev\git\bachelor\test\testdata\without_subsampling");
            PFSSData[] testData = new PFSSData[1];
            PFSSData[] expected = new PFSSData[1];

            StreamWriter w = new StreamWriter(new FileStream(Path.Combine(outputFolder,solution.GetName()), FileMode.Create));
            w.Write("Average Line Size (Bytes);Max Error(Meters); standard deviation (Meters)");

            //load data
            for (int i = 0; i < 1; i++)
            {
                testData[i] = FitsReader.ReadFits(new FileInfo(testFiles[i]), false);
                expected[i] = FitsReader.ReadFloatFits(new FileInfo(expectedFiles[i]), false);
            }

            //do tests
            int qualityLevels = solution.GetQualityLevels();
            for (int i = 0; i < qualityLevels; i++)
            {
                TestResult[] data = new TestResult[testData.Length];
                for (int j = 0; j < 1; j++)
                {
                    data[j] = solution.DoTestRun(testData[j], 0, fitsOutputFolder);
                }

                Tuple<double, double> overall = ErrorCalculator.CalculateOverallError(expected, testData);

                long lineCount = 0;
                long fileSize = 0;
                foreach (TestResult res in data)
                {
                    fileSize += res.fileSize;
                    lineCount += res.lineCount;
                }
                //calculate overall line size 
                double averageLineSize = fileSize / (double)lineCount;

                w.Write(averageLineSize + ";" + overall.Item1 + ";" + overall.Item2);

                //rearm
                for (int j = 0; j < testData.Length; j++)
                {
                    testData[j] = FitsReader.ReadFits(new FileInfo(testFiles[j]), false);
                }
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
