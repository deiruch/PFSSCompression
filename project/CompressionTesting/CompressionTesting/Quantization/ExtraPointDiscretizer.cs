using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

namespace CompressionTesting.Quantization
{
    class ExtraPointDiscretizer
    {

        public static void ToShortsExtra(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                for (int i = 0; i < l.extraX.Length; i++)
                    l.extraX[i] = (short)Math.Truncate(l.extraX[i]);
                for (int i = 0; i < l.extraY.Length; i++)
                    l.extraY[i] = (short)Math.Truncate(l.extraY[i]);
                for (int i = 0; i < l.extraZ.Length; i++)
                    l.extraZ[i] = (short)Math.Truncate(l.extraZ[i]);

            }
        }

        public static void ToIntsExtra(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                for (int i = 0; i < l.extraX.Length; i++)
                    l.extraX[i] = (int)Math.Truncate(l.extraX[i]);
                for (int i = 0; i < l.extraY.Length; i++)
                    l.extraY[i] = (int)Math.Truncate(l.extraY[i]);
                for (int i = 0; i < l.extraZ.Length; i++)
                    l.extraZ[i] = (int)Math.Truncate(l.extraZ[i]);

            }
        }

        public static void DivideLinearExtra(PFSSData data, int channel, double factor, int offset, int start)
        {
            foreach (PFSSLine l in data.lines)
            {
                double div = factor*start;
                if (channel == 0)
                {
                    for (int i = offset; i < l.extraX.Length; i++)
                    {
                        l.extraX[i] = (float)(l.extraX[i] / div);
                        div += factor;
                    }
                }

                if (channel == 1)
                {
                    for (int i = offset; i < l.extraY.Length; i++)
                    {
                        l.extraY[i] = (float)(l.extraY[i] / div);
                        div += factor;
                    }
                }

                if (channel == 2)
                {
                    for (int i = offset; i < l.extraZ.Length; i++)
                    {
                        l.extraZ[i] = (float)(l.extraZ[i] / div);
                        div += factor;
                    }
                }

            }
        }

        public static void MultiplyLinearExtra(PFSSData data, int channel, double factor, int offset, int start)
        {
            foreach (PFSSLine l in data.lines)
            {
                double div = factor*start;
                if (channel == 0)
                {
                    for (int i = offset; i < l.extraX.Length; i++)
                    {
                        l.extraX[i] = (float)(l.extraX[i] * div);
                        div += factor;
                    }
                }
                if (channel == 1)
                {
                    for (int i = offset; i < l.extraY.Length; i++)
                    {
                        l.extraY[i] = (float)(l.extraY[i] * div);
                        div += factor;
                    }
                }
                if (channel == 2)
                {
                    div = factor * start;
                    for (int i = offset; i < l.extraZ.Length; i++)
                    {
                        l.extraZ[i] = (float)(l.extraZ[i] * div);
                        div += factor;
                    }
                }

            }
        }

        public static void MultiplyExtra(PFSSData data, double factor)
        {
            foreach (PFSSLine l in data.lines)
            {
                for (int i = 0; i < l.extraX.Length; i++)
                {

                    l.extraX[i] = (float)(l.extraX[i] * factor);
                }
                for (int i = 0; i < l.extraY.Length; i++)
                {

                    l.extraY[i] = (float)(l.extraY[i] * factor);
                }

                for (int i = 0; i < l.extraZ.Length; i++)
                {

                    l.extraZ[i] = (float)(l.extraZ[i] * factor);
                }
            }
        }

        public static void DivideExtra(PFSSData data, double factor)
        {
            foreach (PFSSLine l in data.lines)
            {
                for (int i = 0; i < l.extraX.Length; i++)
                {

                    l.extraX[i] = (float)(l.extraX[i] / factor);
                }
                for (int i = 0; i < l.extraY.Length; i++)
                {

                    l.extraY[i] = (float)(l.extraY[i] / factor);
                }

                for (int i = 0; i < l.extraZ.Length; i++)
                {

                    l.extraZ[i] = (float)(l.extraZ[i] / factor);
                }
            }
        }


        public static void DividePoint(PFSSData data, double factor, int index)
        {
            foreach (PFSSLine l in data.lines)
            {
                if (index < l.points.Count)
                {
                    l.extraX[index] = (float)(l.extraX[index] / factor);
                    l.extraY[index] = (float)(l.extraY[index] / factor);
                    l.extraZ[index] = (float)(l.extraZ[index] / factor);
                }

            }
        }
        public static void MultiplyPoint(PFSSData data, double factor, int index)
        {
            foreach (PFSSLine l in data.lines)
            {
                if (index < l.points.Count)
                {
                    l.extraX[index] = (float)(l.extraX[index] * factor);
                    l.extraY[index] = (float)(l.extraY[index] * factor);
                    l.extraZ[index] = (float)(l.extraZ[index] * factor);
                }
            }
        }

        public static void Cut(PFSSData data, int offset,int channel)
        {
            foreach (PFSSLine l in data.lines)
            {
                if (channel == 0)
                {
                    for (int i = offset; i < l.extraX.Length; i++)
                    {

                        l.extraX[i] = 0;
                    }
                }
                if (channel == 1)
                {
                    for (int i = offset; i < l.extraY.Length; i++)
                    {

                        l.extraY[i] = 0;
                    }
                }

                if (channel == 2)
                {
                    for (int i = offset; i < l.extraZ.Length; i++)
                    {

                        l.extraZ[i] = 0;
                    }
                }
            }

        }
    }
}
