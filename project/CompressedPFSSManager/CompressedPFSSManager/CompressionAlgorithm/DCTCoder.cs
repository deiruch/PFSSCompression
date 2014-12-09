using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressedPFSSManager.PFSS;

namespace CompressedPFSSManager
{
    class DCTCoder
    {
        public const byte continueFlag = 128;
        public const byte signFlag = 64;
        public const short maxValue = 63;
        public const short minValue = -64;
        public const int dataBitCount = 7;

        public static short[] EncodeRLE(short[] data)
        {
            int length = FindLastValue(data) + 1;
            short[] copy = new short[length + 1];
            Array.Copy(data, 0, copy, 1, length);
            copy[0] = (short)length;
            return copy;
        }

        public static byte[] EncodeAdaptiveUnsigned(short[] data)
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

        public static byte[] EncodeAdaptive(short[] data)
        {
            int length = data.Length;
            int continues = CalcLength(data, length);
            byte[] output = new byte[length+ continues];

            int outIndex = 0;
            for (int i = 0; i < length; i++)
            {
                byte current = 0;
                int extraBytes = CountExtraBytes(data[i]);
                int value = data[i];
                
                current = (byte)((value >> (extraBytes * dataBitCount)) & signFlag-1);
                current += Math.Sign(data[i]) < 0 ? signFlag : (byte)0;
                current += extraBytes > 0 ? continueFlag : (byte)0;
                output[outIndex++] = current;
                
                for (int j = 0; j < extraBytes; j++)
                {
                    current = 0;
                    current = (byte)((value << (extraBytes - j - 1) * dataBitCount) & continueFlag - 1);
                    current += j+1 < extraBytes ? continueFlag : (byte)0;
                    output[outIndex++] = current;
                }
            }

            return output;
        }

        private static int FindLastValue(short[] data)
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
                value >>= dataBitCount;
                counter++;
            }

            return counter;
        }

        private static int CountExtraBytesUnsigned(int value)
        {
            int counter = 0;
            int maxV = continueFlag-1;

            while (value > maxValue)
            {
                value >>= dataBitCount;
                counter++;
            }

            return counter;
        }

        private static int CalcLength(short[] data,int length)
        {
            int counter = 0;
            for (int i = 0; i < length; i++)
                counter += CountExtraBytes(data[i]);
            
            return counter;
        }

        private static int CalcLengthUnsigned(short[] data, int length)
        {
            int counter = 0;
            for (int i = 0; i < length; i++)
                counter += CountExtraBytesUnsigned(data[i]);

            return counter;
        }

        public static short[] Decode(byte[] data,int decLen)
        {
            //count length
            short[] output = new short[decLen];
            int outIndex = 0;
            for (int i = 0; i < data.Length; i++)
            {
                byte current = data[i];
                int value = (short)(current & (signFlag-1));
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
                output[outIndex++] = (short)(value + minus);
            }

            return output;
        }

        public static List<byte[]> Encode(PFSSLine line, int offset)
        {
            List<byte[]> output = new List<byte[]>(3);

            short[] x = new short[line.points.Count];
            short[] y = new short[line.points.Count];
            short[] z = new short[line.points.Count];
            for (int i = offset; i < line.points.Count; i++) 
            {
                x[i] = (short)line.points[i].x;
                y[i] = (short)line.points[i].y;
                z[i] = (short)line.points[i].z;
            }
            x = EncodeRLE(x);
            y = EncodeRLE(y);
            z = EncodeRLE(z);
            output.Add(EncodeAdaptive(x));
            output.Add(EncodeAdaptive(y));
            output.Add(EncodeAdaptive(z));
            return output;
        }

        public static byte[] Encode(float[] data)
        {
            short[] copy = new short[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                copy[i] = (short)data[i];
            }
            copy = EncodeRLE(copy);

            return EncodeAdaptive(copy);
        }

        public static byte[] Encode(short[] data)
        {
            short[] copy = EncodeRLE(data);

            return EncodeAdaptive(copy);
        }
    }
}
