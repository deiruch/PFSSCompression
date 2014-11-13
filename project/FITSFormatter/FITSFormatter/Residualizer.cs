using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FITSFormatter.PFSS;

namespace FITSFormatter
{
    class Residualizer
    {
        public static void DoResiduals(PFSSData data, int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint last = l.points[offset-1];
                for (int i = offset; i < l.points.Count; i++)
                {
                    PFSSPoint current = l.points[i];
                    PFSSPoint copy = new PFSSPoint(current);
                    current.x = (-last.x + current.x);
                    current.y = (-last.y + current.y);
                    current.z = (-last.z + current.z);

                    last = copy;
                }
            }
        }

        public static void UndoResiduals(PFSSData data,int offset)
        {
            foreach (PFSSLine l in data.lines)
            {
                PFSSPoint before = l.points[offset -1];
                for (int i = offset; i < l.points.Count; i++)
                {
                    l.points[i].x = (l.points[i].x) + before.x;
                    l.points[i].y = (l.points[i].y) + before.y;
                    l.points[i].z = (l.points[i].z) + before.z;

                    before = l.points[i];
                }
            }
            
        }
    }
}
