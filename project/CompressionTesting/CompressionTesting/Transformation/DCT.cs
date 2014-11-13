using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting
{
    class DCT
    {
        public const int size = 128;
        private static readonly double size2 = 2d*size;
        private static readonly float halfSize = 2/(float)size;
        private static readonly float[] coefficients = {1,1,2,2,8,16,24,32,20,30,40,50,60,80,100,120};

        public static float[] slow_fdct(float[] value)
        {
            int adaptiveSize = value.Length;
            float halfAdaptive = 2/(float)adaptiveSize;
            float[] output = new float[adaptiveSize];
            double adaptive2 = 2d * adaptiveSize;

            for (int k = 0; k < adaptiveSize; k++)
            {
                output[k] = halfAdaptive;
                double inner = 0;
                for (int i = 0; i < adaptiveSize; i++)
                {
                    inner += value[i] * Math.Cos(((2 * i + 1) * k * Math.PI) / adaptive2);
                }
                output[k] *= (float)inner;
                
                //quantization
                /*if (coefficients[k] != 1)
                {
                    output[k] = (float)Math.Floor(output[k]/coefficients[k]);
                }
                else
                {
                    output[k] = (float)Math.Floor(output[k]);
                }*/

            }
            return output;
        }

        public static float[] slow_idct(float[] value)
        {
            //dequantization
            /*for (int i = 0; i < size; i++)
            {
                if (coefficients[i] != 1)
                {
                    value[i] = (float)value[i] * coefficients[i];
                }
            }*/
            int adaptiveSize = value.Length;
            float halfAdaptive = 2 / (float)adaptiveSize;
            double adaptive2 = 2d * adaptiveSize;

            float[] output = new float[adaptiveSize];
            for (int k = 0; k < adaptiveSize; k++)
            {
                for (int i = 1; i < adaptiveSize; i++)
                {
                    output[k] += (float)(value[i] * (Math.Cos((2 * k + 1) * i * Math.PI / adaptive2)));
                }

                output[k] += value[0] / 2f;
                
            }
            return output;
        }


    }
}
