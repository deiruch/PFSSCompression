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
        public static byte[] Encode(short[] data)
        {
            int length = FindLastValue(data) + 1;
            int continues = CountContinues(data, length);
            byte[] output = new byte[length + continues + 1];

            int outIndex = 0;
            for (int i = 1; i < length; i++)
            {
                byte current = 0;

                //use 2 bytes for saving value
                if (data[i] > maxValue || data[i] < minValue)
                {
                    short copy = data[i];
                    byte next = (byte)(255 & copy);
                    copy = (short)(copy >> 8);
                    current += (byte)(copy & maxValue);
                    current += continueFlag;
                    current += Math.Sign(data[i]) < 0 ? signFlag : (byte)0;

                    output[outIndex++] = current;
                    output[outIndex++] = next;
                }
                else
                {
                    //use one byte
                    current += Math.Sign(data[i]) < 0 ? signFlag : (byte)0;
                    current += (byte)(data[i] & maxValue);
                    output[outIndex++] = current;
                }
            }

            output[0] = (byte)length;
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

        private static int CountContinues(short[] data,int length)
        {
            int counter = 0;
            for (int i = 0; i < length; i++)
            {
                if (data[i] > maxValue || data[i] < minValue)
                    counter++;
            }


            return counter;
        }

        public static short[] Decode(byte[] data,int decLen)
        {
            short[] output = new short[decLen];
            int outIndex = 0;
            for (int i = 1; i < data.Length; i++)
            {
                byte current = data[i];
                short value = (short)(current & (signFlag-1));
                
                if (current >= continueFlag)
                {
                    //2 byte value
                    value <<= 8;
                    value += data[i + 1];
                    value -= (signFlag & current) != 0 ? (short)16384:(short)0;
                    i++;
                }
                else
                {
                    value -= (signFlag & current) != 0 ? signFlag : (short)0;
                }
                
                output[outIndex++] = value;
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
            output.Add(Encode(x));
            output.Add(Encode(y));
            output.Add(Encode(z));
            return output;
        }
    }
}
