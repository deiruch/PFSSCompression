using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

namespace CompressionTesting.Encoding
{
    class DCTCoder
    {
        public const byte continueFlag = 128;
        public const byte signFlag = 64;
        public const short maxValue = 63;
        public const short minValue = -64;
        public const int dataBitCount = 7;

        public static int[] EncodeRLE(int[] data)
        {
            int length = FindLastValue(data) + 1;
            int[] copy = new int[length + 1];
            Array.Copy(data, 0, copy, 1, length);
            copy[0] = (int)length;
            return copy;
        }

        public static byte[] EncodeAdaptiveUnsigned(int[] data)
        {
            int length = data.Length;
            int continues = CalcLengthUnsigned(data, length);
            byte[] output = new byte[length + continues];

            int outIndex = 0;
            for (int i = 0; i < length; i++)
            {
                byte current = 0;
                int extraBytes = CountExtraBytesUnsigned(data[i]);
                int value = data[i];

                current = (byte)((value >> (extraBytes * dataBitCount)) & continueFlag - 1);
                current += extraBytes > 0 ? continueFlag : (byte)0;
                output[outIndex++] = current;

                for (int j = 0; j < extraBytes; j++)
                {
                    current = 0;
                    current = (byte)((value << (extraBytes - j - 1) * dataBitCount) & continueFlag - 1);
                    current += j + 1 < extraBytes ? continueFlag : (byte)0;
                    output[outIndex++] = current;
                }
            }

            return output;
        }

        public static byte[] EncodeAdaptive(int[] data)
        {
            int length = data.Length;
            int continues = CalcLength(data, length);
            byte[] output = new byte[length+ continues];

            int outIndex = 0;
            for (int i = 0; i < length; i++)
            {
                byte current = 0;
                int extraBytes = CountExtraBytes(data[i]);
                int value = (int)data[i];
                
                current = (byte)((value >> (extraBytes * dataBitCount)) & signFlag-1);
                current += Math.Sign(data[i]) < 0 ? signFlag : (byte)0;
                current += extraBytes > 0 ? continueFlag : (byte)0;
                output[outIndex++] = current;
                
                for (int j = 0; j < extraBytes; j++)
                {
                    current = 0;
                    current = (byte)((value >> (extraBytes - j - 1) * dataBitCount) & continueFlag - 1);
                    current += j+1 < extraBytes ? continueFlag : (byte)0;
                    output[outIndex++] = current;
                }
            }

            return output;
        }

        private static int FindLastValue(int[] data)
        {
            int lastIndex = data.Length - 1;

            for (int i = lastIndex; i >= 0; i--)
            {
                if (data[i] != 0)
                {
                    lastIndex = i;
                    break;
                }
                    
            }

            return lastIndex;
        }

        private static int CountExtraBytes(int value)
        {
            int counter = 0;
            int maxV = maxValue;
            int minV = minValue;
            while (value > maxValue || value < minValue)
            {
                value = value >> dataBitCount;
                counter++;
            }

            return counter;
        }

        private static int CountExtraBytesUnsigned(int value)
        {
            int counter = 0;
            int maxV = continueFlag-1;

            while (value > maxV)
            {
                value >>= dataBitCount;
                counter++;
            }

            return counter;
        }

        private static int CalcLength(int[] data,int length)
        {
            int counter = 0;
            for (int i = 0; i < length; i++)
                counter += CountExtraBytes(data[i]);
            
            return counter;
        }

        private static int CalcLengthUnsigned(int[] data, int length)
        {
            int counter = 0;
            for (int i = 0; i < length; i++)
                counter += CountExtraBytesUnsigned(data[i]);

            return counter;
        }

        public static int[] Decode(byte[] data,int decLen)
        {
            //count length
            int[] output = new int[decLen];
            int outIndex = 0;
            for (int i = 0; i < data.Length; i++)
            {
                byte current = data[i];
                int value = (int)(current & (signFlag-1));
                int minus = -(current & signFlag);
                bool run = current >= continueFlag;
                while (run)
                {
                    if (run) 
                    { 
                        i++;
                        current = data[i];
                    }
                    run = current >= continueFlag;
                    minus <<= dataBitCount;
                    value <<= dataBitCount;
                    value += current & (continueFlag - 1);
                }
                output[outIndex++] = (int)(value + minus);
            }

            return output;
        }

        public static List<byte[]> EncodeChannels(PFSSLine line, int offset)
        {
            List<byte[]> output = new List<byte[]>(3);

            int[] x = new int[line.points.Count -offset];
            int[] y = new int[line.points.Count - offset];
            int[] z = new int[line.points.Count - offset];
            for (int i = offset; i < line.points.Count; i++) 
            {
                x[i - offset] = (int)line.points[i].x;
                y[i - offset] = (int)line.points[i].y;
                z[i - offset] = (int)line.points[i].z;
            }
            x = EncodeRLE(x);
            y = EncodeRLE(y);
            z = EncodeRLE(z);
            output.Add(EncodeAdaptive(x));
            output.Add(EncodeAdaptive(y));
            output.Add(EncodeAdaptive(z));
            return output;
        }

        public static byte[] EncodeChannel(float[] data)
        {
            int[] copy = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                copy[i] = (int)data[i];
            }
            copy = EncodeRLE(copy);

            return EncodeAdaptive(copy);
        }

        public static byte[] EncodeStartPointChannel(int[] data)
        {
            int[] copy = EncodeRLE(data);
            //remove size, not needed in this situation
            int[] copy2 = new int[copy.Length - 1];
            Array.Copy(copy, 1, copy2, 0, copy2.Length);

            return EncodeAdaptive(copy2);
        }
    }
}
