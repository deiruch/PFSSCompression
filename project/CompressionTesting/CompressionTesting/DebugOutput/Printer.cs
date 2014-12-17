using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;
using System.IO;

namespace CompressionTesting.DebugOutput
{
    class Printer
    {
        public static void PrintLine(PFSSData data, int line, FileInfo outFile)
        {
            PFSSLine l = data.lines[line];

            StreamWriter w = new StreamWriter(new FileStream(outFile.FullName, FileMode.Create));
            //print analyzation
            w.Write("R;P;T\n");
            for (int i = 0; i < l.points.Count; i++)
            {
                PFSSPoint p = l.points[i];
                w.Write(p.x); w.Write(";");
                w.Write(p.y); w.Write(";");
                w.Write(p.z); w.Write("\n");
            }
            w.Close();

        }

        public static void PrintLineExtra(PFSSData data, int line, FileInfo outFile)
        {
            PFSSLine l = data.lines[line];

            StreamWriter w = new StreamWriter(new FileStream(outFile.FullName, FileMode.Create));
            //print analyzation
            w.Write("R;P;T\n");
            for (int i = 0; i < l.extraX.Length; i++)
            {
                PFSSPoint p = l.points[i];
                w.Write(l.extraX[i]); w.Write(";");
                w.Write(l.extraX[i]); w.Write(";");
                w.Write(l.extraX[i]); w.Write("\n");
            }
            w.Close();

        }
    }
}
