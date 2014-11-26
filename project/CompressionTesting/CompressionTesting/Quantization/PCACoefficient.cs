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

                l.pcaTransform[0, 2] = p3.x;
                l.pcaTransform[1, 2] = p3.y;
                l.pcaTransform[2, 2] = p3.z;

                if (Math.Sign(p3.x) != Math.Sign(pCheck.x) || Math.Sign(p3.y) != Math.Sign(pCheck.y) || Math.Sign(p3.z) != Math.Sign(pCheck.z))
                {
                    System.Console.WriteLine("F");
                }
                count++;
            }
        }
    }
}
