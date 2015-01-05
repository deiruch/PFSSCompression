using CompressionTesting.PFSS.Test;
using CompressionTesting.PFSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.Transformation;
namespace CompressionTesting
{
    class psnr_hvs
    {
        private static double[] factors = null;
        private static int max = 100; 
        private static readonly double maxI = 20* Math.Log10(4 * PFSSPoint.SunRadius);
        private const double eNorm = 32*1000;
        private const double maskNorm = 100;

        private static void calcFactors()
        {
            int index = 0;
            factors = new double[max];
            factors[0] = 10 / 140d * 10 / 140d;
            index++;
            double factor = 20;
            for (int i = 1; i < 10;i++) 
            {
                factors[index] = 10/factor;
                factors[index] *= factors[index];
                index++;
                factor += 5;
            }
            

            factor = 10d/25d;
            factor *= factor;
            for (int i = 0; i < 8;i++) 
            {
                factors[index++] = factor;
            }

            factor = 10d / 15d;
            factor *= factor;
            for(int i = 0; i < 22;i++) {
                factors[index++] = factor;
            }

            factor = 10d / 8d;
            factor *= factor;
            while (index < factors.Length)
            {
                factors[index++] = factor;
            }
        }

        public static double Calculate(TestSuite[] expected, PFSSData[] actual)
        {
            double psnr = Double.MaxValue;
            int index = 0;
            for (int i = 0; i < expected.Length; i++)
            {
                double oldPSNR = psnr;
                psnr = Math.Min(psnr, Calculate(expected[i], actual[i]));
                if (psnr < oldPSNR)
                    index = i;
            }
            System.Console.WriteLine(index);
            return psnr;
        }

        public static double Calculate(TestSuite expected, PFSSData actual)
        {
            if (factors == null)
                calcFactors();

            int pointCount = 0;
            double mse = 0;
            for (int i = 0; i < expected.lines.Count; i++)
            {
                int points;
                mse +=CalcSE(expected.lines[i], actual.lines[i], out points);
                pointCount += points;
            }

            mse = mse / pointCount / 3;
            return maxI-10*Math.Log10(mse);
        }

        private static double CalcSE(TestLine expected, PFSSLine actual, out int points)
        {
            points = 0;
            double squaredError =0;
          
                for (int i = 0; i < 3; i++)
                {
                    //copy channel
                    float[] ex = expected.CopyChannel(i, actual);
                    float[] ac = actual.CopyChannel(i);
                    ex = DCT.fdct(ex, max);
                    ac = DCT.fdct(ac, max);
                    float[] diff = new float[ex.Length];
                    for (int j = 0; j < diff.Length; j++)
                    {
                        diff[j] = ex[j] - ac[j];
                    }
                    double mask = Math.Max(CalcMask(ex), CalcMask(ac));
                    mask = Math.Sqrt(mask / maskNorm);

                    points += ex.Length;
                    for (int j = 0; j < ex.Length; j++)
                    {
                        double error = 0;
                        double maskFactor = mask / factors[j];
                        if (Math.Abs(diff[j]) > maskFactor)
                        {
                            if (diff[j] > maskFactor)
                            {
                                error = diff[j] - maskFactor;
                            }
                            else
                            {
                                error = diff[j] + maskFactor;
                            }
                        }
                        //else, mask. Error = 0

                        squaredError += error * error;
                    }

                
            }
            return squaredError;
        }

        private static double CalcMask(float[]  channel) 
        {
            double energy = 0;
            for (int i = 0; i < channel.Length; i++)
            {
                energy += channel[i] * channel[i] * factors[i];
            }

            return energy/eNorm;
        }
    }
}
