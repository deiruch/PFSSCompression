using CompressionTesting.PFSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.Transformation
{
    class LinearPredictor
    {
        public static void Forward(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint a = l.points[0];
                PFSSPoint b = l.points[1];
                for (int i = 2; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
                    PFSSPoint pCopy = new PFSSPoint(p);

                    p.x = 2 * b.x - a.x - p.x;
                    p.y = 2 * b.y - a.y - p.y;
                    p.z = 2 * b.z - a.z - p.z;
                    b = pCopy;
                    a = b;
                }
            }
        }

        public static void Backward(PFSSData data)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint a = l.points[0];
                PFSSPoint b = l.points[1];
                for (int i = 2; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];

                    p.x = b.x + b.x - a.x - p.x;
                    p.y = b.y + b.y - a.y - p.y;
                    p.z = b.z + b.z - a.z - p.z;
                    b = p;
                    a = b;
                }
            }
        }
    }
}
