using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CompressedPFSSManager.PFSS
{
    
    class PFSSData
    {
        internal double b0;
        internal double l0;

        internal List<PFSSLine> lines { get; private set; }

        public PFSSData(double _l0, double _b0, List<PFSSLine> _lines)
        {
            b0 = _b0;
            l0 = _l0;
            lines = _lines;
        }

        public void SubsampleByAngle(double angle)
        {
            var COS_ANGLE_OF_LOD = Math.Cos(angle / 180d * Math.PI);

            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                var newPoints = new List<PFSSPoint>();
                bool lineStarted = false;
                PFSSPoint lastPoint = new PFSSPoint(0, 0, 0);
                for (int j = 0; j < line.points.Count; j++)
                {
                    bool colinear;

                    //last point is always never colinear
                    if (lineStarted && (j + 1) < line.points.Count)
                    {
                        colinear = line.points[j].AngleBetween(line.points[j + 1], lastPoint) > COS_ANGLE_OF_LOD;
                    }
                    else
                    {
                        colinear = false;
                        lineStarted = true;
                    }

                    if (!colinear)
                    {
                        newPoints.Add(line.points[j]);
                        lastPoint = line.points[j];
                    }
                }

                //overwrite
                lines[i] = new PFSSLine(line.Type, newPoints);
            }
        }
    }
}
