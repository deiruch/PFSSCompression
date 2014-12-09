using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CompressionTesting.Solutions;
using CompressionTesting.PFSS;
using CompressionTesting.PFSS.Test;

using MathNet.Numerics.LinearAlgebra;
using CompressionTesting.Transformation;
using CompressionTesting.Quantization;
using CompressionTesting.Encoding;

namespace CompressionTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] data = new int[] { 362,-127001, 6105,8827 };
            byte[] result = DCTCoder.EncodeAdaptive(data);
            int[] res = DCTCoder.Decode(result, 4);
            run();
            //Testing();
            //testExampleLine();
            //printLine();
        }
        private static void testExampleLine()
        {
            ISolution solution = new Solution1();
            string fitsOutputFolder = @"C:\Users\Jonas Schwammberger\Documents\GitHub\PFSSCompression\test\temp";
            string lineFile = @"C:\Users\Jonas Schwammberger\Documents\GitHub\PFSSCompression\test\solutions\exampleLine.csv";
            string[] content = File.ReadAllLines(lineFile);
            List<PFSSPoint> points = new List<PFSSPoint>();
            for (int i = 1; i < content.Length; i++)
            {
                string[] split = content[i].Split(';');
                PFSSPoint p = new PFSSPoint(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
                points.Add(p);
            }
            PFSSLine l = new PFSSLine(TYPE.OUTSIDE_TO_SUN, points);
            List<PFSSLine> lines = new List<PFSSLine>();
            lines.Add(l);
            PFSSData data = new PFSSData(0, 0, lines);


            solution.DoTestRun(data, 1, fitsOutputFolder);
            StreamWriter w = new StreamWriter(new FileStream(Path.Combine(fitsOutputFolder, solution.GetName() + "_Line.csv"), FileMode.Create));
            l = data.lines[0];
            w.Write("X;Y;Z\n");
            foreach (PFSSPoint p in l.points)
            {
                w.Write(p.x + ";");
                w.Write(p.y + ";");
                w.Write(p.z + "\n");
            }
            w.Close();
            //load line
            //construct PFSSData
        }

        private static void printLine()
        {
            bool testOneFile = true;

            string fitsOutputFolder = @"C:\Users\Jonas Schwammberger\Documents\GitHub\PFSSCompression\test\temp";
            string outputFolder = @"C:\Users\Jonas Schwammberger\Documents\GitHub\PFSSCompression\test\testresult";
            string[] expectedFiles = Directory.GetFiles(@"C:\dev\git\bachelor\test\testdata\raw");
            TestSuite testData = FitsReader.ReadFloatFits(new FileInfo(expectedFiles[0]));

            PFSSData data = testData.GetData();
            PCATransform.Forward(data, 0);
            StreamWriter w = new StreamWriter(new FileStream(Path.Combine(fitsOutputFolder, "exampleLine" + ".csv"), FileMode.Create));
            w.Write("X;Y;Z\n");
            PFSSLine l = data.lines[0];
            for (int i = 0; i < l.points.Count; i++)
            {
                PFSSPoint p = l.points[i];
                w.Write(p.x + ";");
                w.Write(p.y + ";");
                w.Write(p.z + "\n");
            }
            w.Close();
        }

        private static void run()
        {
            ISolution solution = new Solution1();
            bool testOneFile = true;
            //DCT.DiscreteCosineTransform(400);

            string fitsOutputFolder = @"C:\Users\Jonas Schwammberger\Documents\GitHub\PFSSCompression\test\temp";
            string outputFolder = @"C:\Users\Jonas Schwammberger\Documents\GitHub\PFSSCompression\test\testresult";
            string[] expectedFiles = Directory.GetFiles(@"C:\dev\git\bachelor\test\testdata\raw");
            TestSuite[] testData = testOneFile ? new TestSuite[1] : new TestSuite[expectedFiles.Length];


            StreamWriter w = new StreamWriter(new FileStream(Path.Combine(outputFolder, solution.GetName() + ".csv"), FileMode.Create));
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

        private static void Testing()
        {
            DCT.DiscreteCosineTransform(400);
            ISolution solution = new Solution1();
            bool testOneFile = true;

            string fitsOutputFolder = @"C:\Users\Jonas Schwammberger\Documents\GitHub\PFSSCompression\test\temp";
            string outputFolder = @"C:\Users\Jonas Schwammberger\Documents\GitHub\PFSSCompression\test\testresult";
            string[] expectedFiles = Directory.GetFiles(@"C:\dev\git\bachelor\test\testdata\raw");
            TestSuite[] testData = testOneFile ? new TestSuite[1] : new TestSuite[expectedFiles.Length];

            //load data
            for (int i = 0; i < testData.Length; i++)
            {
                testData[i] = FitsReader.ReadFloatFits(new FileInfo(expectedFiles[i]));
            }

            Tuple<double, double> max = new Tuple<double, double>(double.MaxValue, double.MaxValue);
            int bla0 = 0;
            int bla1 = 0;
            int bla2 = 0;
            int bla3 = 0;
            int bla4 = 0;
            int bla5 = 0;
            for (int percentSearch = 0; percentSearch < 5; percentSearch++)
            {
                for (int maxSearch = 15; maxSearch < 50; maxSearch += 10)
                {
                    for (int maxPercentLength = 2; maxPercentLength < 25; maxPercentLength += 5)
                    {
                        for (int plusLength = 0; plusLength < 10; plusLength += 5)
                        {
                            for (int lenFactor = 0; lenFactor < 25; lenFactor += 10)
                            {
                                DCTImprover.maxSearch = maxSearch;
                                DCTImprover.percentSearch = 5 + percentSearch * 6;
                                DCTImprover.maxPercentLength = maxPercentLength;
                                DCTImprover.plusLength = plusLength;
                                DCTImprover.factorStatic = (lenFactor + 1) * 20000;

                                TestResult[] result = new TestResult[testData.Length];
                                PFSSData[] data = new PFSSData[testData.Length];
                                for (int j = 0; j < data.Length; j++)
                                {
                                    data[j] = testData[j].GetData();
                                    result[j] = solution.DoTestRun(data[j], 1, fitsOutputFolder);
                                }
                                Tuple<double, double> overall = ErrorCalculator.CalculateOverallError(testData, data);
                                long lineCount = 0;
                                long fileSize = 0;
                                foreach (TestResult res in result)
                                {
                                    fileSize += res.fileSize;
                                    lineCount += res.lineCount;
                                }
                                double averageLineSize = fileSize / (double)lineCount;
                                if (averageLineSize <= max.Item1 && overall.Item2 <= 6000000)
                                {
                                    Tuple<double, double> newMax = new Tuple<double, double>(averageLineSize, overall.Item2);
                                    max = newMax;
                                    System.Console.Write("maxSearch: " + maxSearch);
                                    System.Console.Write("maxPercentLength: " + maxPercentLength);
                                    System.Console.Write("plusLength: " + plusLength);
                                    System.Console.Write("factorStatic: " + (lenFactor * 2 + 1) * 20000);
                                    System.Console.WriteLine("--------------------------------");

                                    bla0 = percentSearch;
                                    bla1 = maxSearch;
                                    bla2 = maxPercentLength;
                                    bla3 = plusLength;
                                    bla4 = lenFactor;
                                }




                            }
                        }
                    }
                }
            }

            System.Console.ReadLine();
        }
    }
}
