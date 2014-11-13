using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FITSFormatter.PFSS;
using System.IO;

namespace FITSFormatter
{
    class TestSuite
    {
        public static void DoTests(FileInfo fits,string outFolder)
        {
            //xyz tests
            XyzTest(FitsReader.ReadFits(fits, false), Path.Combine(outFolder, "normal"), 0, false, -1);
            XyzTest(FitsReader.ReadFits(fits, false), Path.Combine(outFolder, "quant100"), 0, false, 100);
            XyzTest(FitsReader.ReadFits(fits, false), Path.Combine(outFolder, "quant100_residuals1"), 1, false, 100);
            XyzTest(FitsReader.ReadFits(fits, false), Path.Combine(outFolder, "quant40_residuals1"), 1 , false, 40);
            XyzTest(FitsReader.ReadFits(fits, false), Path.Combine(outFolder, "final"), 1, true, 40);

            SphericalTest(FitsReader.ReadFits(fits, false), Path.Combine(outFolder, "normal"), 0, false, -1);
            SphericalTest(FitsReader.ReadFits(fits, false), Path.Combine(outFolder, "quant100_residuals1"), 1, false, 100);
            SphericalTest(FitsReader.ReadFits(fits, false), Path.Combine(outFolder, "quant40_residuals1"), 1, false, 40);
            SphericalTest(FitsReader.ReadFits(fits, false), Path.Combine(outFolder, "quant100_residuals2"), 2, false, 100);
            SphericalTest(FitsReader.ReadFits(fits, false), Path.Combine(outFolder, "quant40_residuals2"), 2, false, 40);
        }

        public static void XyzTest(PFSSData data, string outFile, int residuals, bool discretize, int quantizeCount)
        {
            DctPipeline(data, new FileInfo(outFile + "_xyz.csv"), residuals, discretize, quantizeCount);
            ErrorLog(data, new FileInfo(outFile + "_deviaton_xyz.txt"));
        }

        public static void SphericalTest(PFSSData data, string outFile, int residuals, bool discretize, int quantizeCount)
        {
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    p.x = p.rawR;
                    p.y = p.rawPhi;
                    p.z = p.rawTheta;
                }
            }
            MoveData.ForwardSpherical(data);

            DctPipeline(data, new FileInfo(outFile + "_spherical.csv"), residuals, discretize, quantizeCount);

            MoveData.BackwardSpherical(data);
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    p.ConvertSphericalToXYZ();
                }
            }

            ErrorLog(data, new FileInfo(outFile + "_deviaton__spherical.txt"));
        }

        private static void DctPipeline(PFSSData data, FileInfo outFile, int residuals, bool discretize, int quantizeCount)
        {
            
            if(residuals >= 1)
                Residualizer.DoResiduals(data, 1);
            if (residuals >= 2)
            Residualizer.DoResiduals(data, 2);

            DCTransformer.Forward(data, residuals);
            if(discretize)
                DCTransformer.Discretize(data, residuals);
            DCTAnalyzer.AnalyzeDCT(data, residuals, outFile);

            if(quantizeCount >= 0)
             DCTransformer.QuantizeRepeat(data, quantizeCount);

            DCTransformer.Backward(data, residuals);

            if (residuals >= 2)
                Residualizer.UndoResiduals(data, 2);
            if (residuals >= 1)
                Residualizer.UndoResiduals(data, 1);
        }

        private static void ErrorLog(PFSSData data,FileInfo outFile)
        {
            double deviationX;
            double deviationY;
            double deviationZ;
            ErrorCalculator.CalcError(data, out deviationX, out deviationY, out deviationZ);

            StreamWriter w = new StreamWriter(new FileStream(outFile.FullName, FileMode.Create));
            w.WriteLine("deviationx;" + deviationX);
            w.WriteLine("deviationy;" + deviationY);
            w.WriteLine("deviationz;" + deviationZ);
            w.Close();
        }
    }
}
