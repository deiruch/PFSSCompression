﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressedPFSSManager.PFSS;

namespace CompressedPFSSManager
{
    internal class Subsampling
    {
        /// <summary>
        /// Use the adaptive subsampling algorithm.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="angle"> </param>
        public static void AdaptiveSubsampling(PFSSData data, double angle)
        {
            double ANGLE_OF_LOD = Math.Cos(angle / 180d * Math.PI);

            for (int i = 0; i < data.lines.Count; i++)
            {
                PFSSLine line = data.lines[i];

                List<PFSSPoint> newPoints = new List<PFSSPoint>();
                bool lineStarted = false;
                PFSSPoint lastPoint = new PFSSPoint(0, 0, 0);
                for (int j = 0; j < line.points.Count; j++)
                {
                    bool colinear = false;
                    //last point is always never colinear
                    if (lineStarted && (j + 1) < line.points.Count)
                    {
                        colinear = line.points[j].AngleTo(line.points[j + 1], lastPoint) > ANGLE_OF_LOD;
                    }
                    else
                    {
                        lineStarted = true;
                    }

                    if (!colinear)
                    {
                        newPoints.Add(line.points[j]);
                        lastPoint = line.points[j];
                    }
                }

                //overwrite
                PFSSLine newLine = new PFSSLine(line.Type, newPoints);
                data.lines[i] = newLine;
            }
        }
    }
}
