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

        public static byte[] EncodeChannel(float[] data)
        {
            short[] copy = new short[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                copy[i] = (short)data[i];
            }
            copy = EncodeRLE(copy);

            return EncodeAdaptive(copy);
        }

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

        #region helper methods
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
            int maxV = continueFlag - 1;

            while (value > maxV)
            {
                value >>= dataBitCount;
                counter++;
            }

            return counter;
        }

        private static int CalcLength(short[] data, int length)
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
        #endregion
    }
}
