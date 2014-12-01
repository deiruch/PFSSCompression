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
            run();
            //testExampleLine();
            //pcatryout2();
            //printLine();
        }
        private static void pcatryout2()
        {
            Matrix<float> bla = Matrix<float>.Build.Dense(11, 3);
            Matrix<float> bla2 = Matrix<float>.Build.Dense(2, 3);
            string[] line = File.ReadAllLines(@"C:\Users\Jonas Schwammberger\Documents\GitHub\PFSSCompression\test\temp\line.csv");
            List<PFSSPoint> points = new List<PFSSPoint>();
            for (int i = 1; i < line.Length; i++)
            {
                string[] stuff = line[i].Split(';');
                PFSSPoint point = new PFSSPoint(float.Parse(stuff[0]),float.Parse(stuff[1]),float.Parse(stuff[2]));
                points.Add(point);
                bla[i - 1, 0] = float.Parse(stuff[0]);
                bla[i - 1, 1] = float.Parse(stuff[1]);
                bla[i - 1, 2] = float.Parse(stuff[2]);
            }
            PFSSLine l = new PFSSLine(TYPE.OUTSIDE_TO_SUN, points);
            List<PFSSLine> lines = new List<PFSSLine>();
            lines.Add(l);
            PFSSData d = new PFSSData(0, 0, lines);
            PCATransform.Forward(d, 0);

            PCA p = new PCA(bla, true);
            Matrix<float> result = p.transform(bla, PCA.TransformationType.ROTATION);

            for (int i = 0; i < bla.RowCount; i++)
            {
                System.Console.Write(result[i, 0] + " ");
                System.Console.Write(result[i, 1]+" ");
                System.Console.Write(result[i, 2] + " ");
                System.Console.WriteLine();
            }
            System.Console.WriteLine();
            for (int i = 0; i < d.lines[0].points.Count; i++)
            {
                System.Console.Write(d.lines[0].points[i].x + " ");
                System.Console.Write(d.lines[0].points[i].y + " ");
                System.Console.Write(d.lines[0].points[i].z + " ");
                System.Console.WriteLine();
                
            }
            System.Console.WriteLine();
            PCACoefficient.Backwards(d);
            PCATransform.Backwards(d, 0);
            for (int i = 0; i < d.lines[0].points.Count; i++)
            {
                System.Console.Write(d.lines[0].points[i].x + " ");
                System.Console.Write(d.lines[0].points[i].y + " ");
                System.Console.Write(d.lines[0].points[i].z + " ");
                System.Console.WriteLine();
            }
            System.Console.WriteLine();

            
            result = p.inverseTransform(result, PCA.TransformationType.ROTATION);
           
            for (int i = 0; i < bla.RowCount; i++)
            {
                for (int j = 0; j < bla.ColumnCount; j++)
                    System.Console.Write(result[i, j] + " ");
                System.Console.WriteLine();
            }
            System.Console.WriteLine();

            System.Console.WriteLine();
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
            StreamWriter w = new StreamWriter(new FileStream(Path.Combine(fitsOutputFolder, solution.GetName() + ".csv"), FileMode.Create));
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
            bool testOneFile = false;

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
    }
}
