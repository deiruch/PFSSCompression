

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressedPFSSManager.PFSS
{
    enum LineType
    {
        SUN_TO_OUTSIDE,
        SUN_TO_SUN,
        OUTSIDE_TO_SUN
    }

    class PFSSLine
    {
        internal LineType Type;
        internal List<PFSSPoint> points;
        internal List<PFSSPoint> predictionErrors;

        public PFSSLine(LineType t, List<PFSSPoint> points)
        {
            this.Type = t;
            this.points = points;
        }
    }
}
