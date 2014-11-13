using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FITSFormatter.PFSS;

namespace FITSFormatter
{
    class SimpleCompression
    {
        internal static readonly double ANGLE_OF_LOD = Math.Cos(5.0 / 180 * Math.PI);


        public static void Compress(FileInfo input, FileInfo output) 
        {
            BinaryReader reader = new BinaryReader(new FileStream(input.FullName, FileMode.Open));
            
            int length = reader.ReadInt32();
            double l0 = reader.ReadDouble();
            double b0 = reader.ReadDouble();
            short r = reader.ReadInt16();
            short phi = reader.ReadInt16();
            short theta = reader.ReadInt16();
            
            PFSSPoint p = new PFSSPoint(r, phi, theta, l0, b0);
            PFSSPoint last = p;

            List<PFSSPoint> points = new List<PFSSPoint>(length/2);
            points.Add(p);

            int maxDR = 0;
            int maxDp = 0;
            int maxDt = 0;

            for (int i = 1; i < length; i++)
            {
                //System.Console.WriteLine(i);
                short r1 = reader.ReadInt16();
                short phi1 = reader.ReadInt16();
                short theta1 = reader.ReadInt16();
                PFSSPoint p1 = new PFSSPoint(r1, phi1, theta1, l0, b0);

                if (!(p.AngleTo(p1,last) > ANGLE_OF_LOD && i + 1 != length))
                {
                    points.Add(p);
                    if (Math.Abs(r1 - r) > maxDR)
                        maxDR = Math.Abs(r1 - r);
                    if (Math.Abs(phi1 - phi) > maxDp)
                        maxDp = Math.Abs(phi1 - phi);
                    if (Math.Abs(theta1 - theta) > maxDt)
                        maxDt = Math.Abs(theta1 - theta);

                    r = r1;
                    phi = phi1;
                    theta = theta1;

                    last = p;
                }
                p = p1;
            }

            System.Console.WriteLine("dR " + maxDR);
            System.Console.WriteLine("dPhi " + maxDp);
            System.Console.WriteLine("dTheta " + maxDt);

            System.Console.WriteLine("size: " + points.Count+ " original: " + length);
            WriteNew(l0, b0, points, new FileInfo(input.FullName+".jon"));
        }

        private static void WriteNew(double l0, double b0, List<PFSSPoint> points, FileInfo output)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream(output.FullName, FileMode.Create));
            writer.Write(points.Count - 1);
            writer.Write(l0);
            writer.Write(b0);
            PFSSPoint p = points[0];

            writer.Write(p.rawR);
            writer.Write(p.rawPhi);
            writer.Write(p.rawTheta);

            for (int i = 1; i < points.Count; i++)
            {
                PFSSPoint p1 = points[i];
                short dR = (short) (p1.rawR - p.rawR);
                short dPhi = (short)(p1.rawPhi - p.rawPhi);
                short dTheta = (short)(p1.rawTheta - p.rawTheta);

                writer.Write(dR);
                writer.Write(dPhi);
                writer.Write(dTheta);

                p = p1;
            }

            writer.Close();

        }
    }
}
