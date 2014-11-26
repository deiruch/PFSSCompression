using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

namespace CompressionTesting.Transformation
{
    class YCbCr
    {
        public static void ForwardFull(PFSSData data, int pointOffset)
        {
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    float x = p.x;
                    float y = p.y;
                    float z = p.z;
                    p.x = -x + y + z;
                    p.y = y - p.x;
                    p.z = z - p.x;
                }
            }
        }

        public static void BackwardsFull(PFSSData data, int pointOffset)
        {
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    float x = p.x;
                    float y = p.y;
                    float z = p.z;
                    p.x = x + y + z;
                    p.y = x + y;
                    p.z = x + z;
                }
            }
        }

        public void forwardHalf(PFSSData data, int pointOffset)
        {
            throw new NotImplementedException();
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    
                    float x = p.x;
                    float y = p.y;
                    p.x = x + y;
                    p.y = y- p.x;
                }
            }
        }

        public void backwardsHalf(PFSSData data, int pointOffset)
        {
            foreach (PFSSLine l in data.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    float x = p.x;
                    float y = p.y;
                    p.x = x - y;
                    p.y = y;
                }
            }
        }
    }
}
