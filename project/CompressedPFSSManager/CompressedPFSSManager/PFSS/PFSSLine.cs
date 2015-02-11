

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressedPFSSManager.PFSS
{
    enum TYPE
    {
        SUN_TO_OUTSIDE,
        SUN_TO_SUN,
        OUTSIDE_TO_SUN
    }

    class PFSSLine
    {
        internal TYPE Type { get; private set; }
        internal List<PFSSPoint> points { get; private set; }

        internal PFSSPoint startPoint { get; set; }
        internal PFSSPoint endPoint { get; set; }
        internal List<PFSSPoint> predictionErrors { get; set; }

        public PFSSLine(TYPE t, List<PFSSPoint> points)
        {
            this.Type = t;
            this.points = points;
        }
    }
}
