using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FITSFormatter.PFSS;
using nom.tam.fits;
using nom.tam.util;


namespace FITSFormatter
{
    class DCTWriter
    {
        public static void Write(PFSSData input, FileInfo output)
        {
            float[] ptr;
            float[] ptph;
            float[] ptth;
            short[] ptr_nz_len = new short[input.lines.Count];

            int totalCount = 0;
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                int count = input.lines[i].points.Count;
                totalCount += count;
                ptr_nz_len[i] = (short)count;
            }

            ptr = new float[totalCount];
            ptph = new float[totalCount];
            ptth = new float[totalCount];

            int index = 0;
            foreach (PFSSLine l in input.lines)
            {
                for (int i = 0; i < l.points.Count-1;i++)
                {
                    ptr[index] = l.rCos[i];
                    ptph[index] = l.pCos[i];
                    ptth[index] = l.tCos[i];
                    index++;
                }
            }

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
