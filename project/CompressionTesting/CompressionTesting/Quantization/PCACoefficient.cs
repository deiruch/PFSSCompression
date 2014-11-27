using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

namespace CompressionTesting.Quantization
{
    class PCACoefficient
    {
        static int count = 0;
        public static void Backwards(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                ErrorCalculator.Point pCheck = new ErrorCalculator.Point(l.pcaTransform[0, 2], l.pcaTransform[1, 2], l.pcaTransform[2, 2]);

                ErrorCalculator.Point p0 = new ErrorCalculator.Point(l.pcaTransform[0, 0], l.pcaTransform[1, 0], l.pcaTransform[2, 0]);
                ErrorCalculator.Point p1 = new ErrorCalculator.Point(l.pcaTransform[0, 1], l.pcaTransform[1, 1], l.pcaTransform[2, 1]);
                ErrorCalculator.Point p3 = ErrorCalculator.Point.cross(p1, p0);
                ErrorCalculator.Point p4 = ErrorCalculator.Point.cross(p0, p1);

                //not finished, but used to try to calculate a meaningful error
                l.pcaTransform[0, 2] = p3.x;
                l.pcaTransform[1, 2] = p3.y;
                l.pcaTransform[2, 2] = p3.z;

                if (Math.Sign(p3.x) != Math.Sign(pCheck.x) || Math.Sign(p3.y) != Math.Sign(pCheck.y) || Math.Sign(p3.z) != Math.Sign(pCheck.z))
                {
                    l.pcaTransform[0, 2] = p4.x;
                    l.pcaTransform[1, 2] = p4.y;
                    l.pcaTransform[2, 2] = p4.z;
                }
                count++;
            }
        }

        public static void ForwardQuantization(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                for(int i = 0; i < 2;i++) {
                    for(int j = 0; j < 3;j++) {
                        l.pcaTransform[j,i] = l.pcaTransform[j,i]* 32767;
                    }
                }
                
            }
        }

        public static void BackwardQuantization(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        l.pcaTransform[j, i] = l.pcaTransform[j, i] / 32767;
                    }
                }

            }
        }
    }
}
