using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.PFSS
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

        internal Matrix<float> pcaTransform;
        internal float[] means;

        public PFSSLine(TYPE t, List<PFSSPoint> points)
        {
            this.Type = t;
            this.points = points;
        }


    }
}
