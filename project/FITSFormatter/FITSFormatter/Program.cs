using nom.tam.fits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using nom.tam.util;
using FITSFormatter.PFSS;
namespace FITSFormatter
{
    class Program
    {

        static void Main(string[] args)
        {
            /*float[] dcttest = { 8182, 8385, 8489, 8534, 8607, 8632, 8665, 8714};//, 8723, 30916, 22726, 10430, 16624, 29168, -2979, 23397 };
            float[] result = DCT.slow_fdct(dcttest);
            for (int i = 0; i < result.Length;i++ )
                System.Console.Write(result[i].ToString()+" ");
            System.Console.WriteLine();

            float[] back = DCT.slow_idct(result);
            for (int i = 0; i < result.Length; i++)
                System.Console.Write(back[i].ToString() + " ");
            System.Console.WriteLine();*/

            Tests();
        }

        public static void tryout()
        {
            FileInfo input;

            input = new FileInfo(@"C:\dev\git\bachelor\tools\FITSFormatter\2014-07-01_00-04-00.000_pfss_field_data.fits");
  
            if (input.Exists)
            {
                FileInfo output = new FileInfo(@"C:\dev\git\bachelor\tools\FITSFormatter\fitsOut_delta.fits");
            }
            else
                System.Console.WriteLine("No File found!");

            System.Console.WriteLine((new FileInfo(".")).FullName);
        }


        public static void Tests()
        {
            FileInfo f = new FileInfo(@"C:\dev\git\bachelor\testdata\subsampled\2014-06-10_12-04-00.000_pfss_field_data.fits");
            TestSuite.DoTests(f, @"C:\dev\git\bachelor\suite");
        }

        public static void XYZDctTest()
        {
            FileInfo f = new FileInfo(@"C:\dev\git\bachelor\testdata\subsampled\2014-06-10_12-04-00.000_pfss_field_data.fits");
            PFSSData data = FitsReader.ReadFits(f, false);

            /*foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    p.x = p.rawR;
                    p.y = p.rawPhi;
                    p.z = p.rawTheta;
                }
            }
            MoveData.ForwardSpherical(data);*/

            Residualizer.DoResiduals(data,1);
            //Residualizer.DoResiduals(data, 2);

            DCTransformer.Forward(data, 1);
            //DCTransformer.Discretize(data, 1);
            DCTAnalyzer.AnalyzeDCT(data, 1, new FileInfo(@"C:\dev\git\bachelor\tools\FITSFormatter\testresult_xyz.csv"));
           
            DCTransformer.QuantizeRepeat(data, 40);
            DCTransformer.Backward(data, 1);

            //Residualizer.UndoResiduals(data, 2);
            Residualizer.UndoResiduals(data,1);

            /*MoveData.BackwardSpherical(data);
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    p.ConvertSphericalToXYZ();
                }
            }*/

            double deviationX;
            double deviationY;
            double deviationZ;
            ErrorCalculator.CalcError(data,out deviationX,out deviationY,out deviationZ);


            System.Console.WriteLine(deviationX);
        }

    }
}
