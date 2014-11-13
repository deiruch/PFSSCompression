using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FITSFormatter.PFSS;
using System.IO;
using nom.tam.fits;
using nom.tam.util;

namespace FITSFormatter
{
    class FitsWriter
    {

        public static void WriteNewFits(PFSSData input,FileInfo output)
        {
            short[] ptr;
            short[] ptph;
            short[] ptth;
            short[] ptr_nz_len = new short[input.lines.Count];

            int totalCount = 0;
            for(int i = 0; i < ptr_nz_len.Length;i++)
            {
                int count = input.lines[i].points.Count;
                totalCount += count;
                ptr_nz_len[i] = (short)count;
            }

            ptr = new short[totalCount];
            ptph = new short[totalCount];
            ptth = new short[totalCount];

            int maxR = 0;
            int maxPhi = 0;
            int maxTheta = 0;
            int index = 0;

            bool writeOut = false;
            StreamWriter w = new StreamWriter(new FileStream(@"C:\dev\git\bachelor\tools\FITSFormatter\line_rpt_dct.csv", FileMode.Create));

            foreach(PFSSLine l in input.lines)
            {
                /*PFSSPoint last = l.points[0];
                ptr[index] = last.rawR;
                ptph[index] = last.rawPhi;
                ptth[index] = last.rawTheta;
                index++;
                if(!writeOut)
                    w.WriteLine(last.rawR + ";" + last.rawPhi + ";" + last.rawTheta);

                for (int i = 1; i < l.points.Count; i++)
                {
                    PFSSPoint current = l.points[i];
                    ptr[index] = (short)(last.rawR -current.rawR);
                    ptph[index] = (short)(last.rawPhi- current.rawPhi);
                    ptth[index] = (short)(last.rawTheta -current.rawTheta);

                    if (Math.Abs(ptr[index]) > maxR) maxR = Math.Abs(ptr[index]);
                    if (Math.Abs(ptph[index]) > maxPhi) maxPhi = Math.Abs(ptph[index]);
                    if (Math.Abs(ptth[index]) > maxTheta) maxTheta = Math.Abs(ptth[index]);

                    if(!writeOut)
                        w.WriteLine(ptr[index] + ";" + ptph[index] + ";" + ptth[index]);

                    index++;
                    last = current;
                }
                writeOut = true;
                w.Close();*/
                if (!writeOut)
                {
                    
                   writeOut = true;
                   foreach (PFSSPoint p in l.points) {
                       w.WriteLine(p.rawR+";"+p.rawPhi+";"+p.rawTheta);
                   }
                   w.Close();
                }

                foreach (PFSSPoint p in l.points)
                {
                    ptr[index] = p.rawR;
                    ptph[index] = p.rawPhi;
                    ptth[index] = p.rawTheta;
                    index++;
                }
            }

            System.Console.WriteLine(maxR);
            System.Console.WriteLine(maxPhi);
            System.Console.WriteLine(maxTheta);
            Fits fits = new Fits();

            Double[] b0a = new Double[] { input.b0 };
            Double[] l0a = new Double[] { input.l0 };
            Object[][] data = new Object[1][];
            Object[] dataRow = new Object[] { b0a, l0a, ptr, ptr_nz_len, ptph, ptth };
            data[0] = dataRow;

            BinaryTable table = new BinaryTable(data);
            Header hdr = BinaryTableHDU.ManufactureHeader(table);
            fits.AddHDU(new BinaryTableHDU(hdr, table));
            BinaryTableHDU bhdu = (BinaryTableHDU)fits.GetHDU(1);
            bhdu.SetColumnName(0, "B0", null);
            bhdu.SetColumnName(1, "L0", null);
            bhdu.SetColumnName(2, "PTR", null);
            bhdu.SetColumnName(3, "PTR_NZ_LEN", null);
            bhdu.SetColumnName(4, "PTPH", null);
            bhdu.SetColumnName(5, "PTTH", null);
            

            BufferedDataStream f = new BufferedDataStream(new FileStream(output.FullName, FileMode.Create));
            fits.Write(f);
            f.Close();
        }
    }
}
