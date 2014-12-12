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
        private static float[,] dctFactors = null;
        private static int length = 0;
        
        public static void DiscreteCosineTransform(int len) 
        {
    	    dctFactors = new float[len,len];
    	    length = len;
    	
    	    for(int k = 0; k < length;k++){
    		    for(int i = 0; i < length;i++) {
    			    dctFactors[k,i] = (float)((2 * i + 1) * k * Math.PI);
    		    }
    	    }
        }

        private static float getDctFactor(int k, int i)
        {
            if (k < length && i < length)
            {
                return dctFactors[k,i];
            }
            else
            {
                return (float)((2 * i + 1) * k * Math.PI);
            }
        }

        public static float[] fdct(float[] value, int max)
        {
            int adaptiveSize = value.Length;
            float halfAdaptive = 2 / (float)adaptiveSize;
            float[] output = new float[max];
            double length2 = 2d * adaptiveSize;


            for (int k = 0; k < max && k < adaptiveSize; k++)
            {
                output[k] = halfAdaptive;
                double inner = 0;
                for (int i = 0; i < adaptiveSize; i++)
                {
                    inner += value[i] * Math.Cos(getDctFactor(k, i) / length2);
                }
                output[k] *= (float)inner;

            }
            return output;
        }

        public static float[] fdct(float[] value)
        {
            return fdct(value, value.Length);
        }


        public static float[] idct(float[] value, int max)
        {
            int adaptiveSize = value.Length;
            float halfAdaptive = 2 / (float)adaptiveSize;
            double length2 = 2d * adaptiveSize;

            float[] output = new float[adaptiveSize];
            for (int k = 0; k < max && k < adaptiveSize; k++)
            {
                for (int i = 1; i < adaptiveSize; i++)
                {
                    float bla = (float)(value[i] * Math.Cos(getDctFactor(i, k) / length2));
                    output[k] += (float)(value[i] * Math.Cos(getDctFactor(i, k) / length2));
                }

                output[k] += value[0] / 2f;

            }
            return output;
        }

        public static float[] idct(float[] value)
        {
            return idct(value, value.Length);
        }

        public static float[] slow_fdct(float[] value)
        {
            int adaptiveSize = value.Length;
            float halfAdaptive = 2/(float)adaptiveSize;
            float[] output = new float[adaptiveSize];
            double length2 = 2d * adaptiveSize;

            for (int k = 0; k < adaptiveSize; k++)
            {
                output[k] = halfAdaptive;
                double inner = 0;
                for (int i = 0; i < adaptiveSize; i++)
                {
                    inner += value[i] * Math.Cos(((2 * i + 1) * k * Math.PI) / length2);
                }
                output[k] *= (float)inner;

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
