using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FITSFormatter.PFSS
{
    public enum TYPE
    {
        SUN_TO_OUTSIDE,
        SUN_TO_SUN,
        OUTSIDE_TO_SUN
    }

    class PFSSLine
    {
        internal TYPE Type { get; private set; }
        internal List<PFSSPoint> points { get; private set; }
        internal List<PFSSCosinus> cosinus { get; private set; }

        internal float[] rCos;
        internal float[] pCos;
        internal float[] tCos;

        internal float[] riCos;
        internal float[] piCos;
        internal float[] tiCos;

        public PFSSLine(TYPE t, List<PFSSPoint> points)
        {
            this.Type = t;
            this.points = points;
        }

        public PFSSPoint getPoint(int index)
        {
            if (index >= points.Count)
                return points[points.Count - (index % points.Count+1)];
            return points[index];
        }
    }
}
