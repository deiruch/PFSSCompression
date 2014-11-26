using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CompressionTesting.PFSS;

namespace CompressionTesting.FileWriter
{
    class StandardBufferWriter
    {
        public static void WriteIntFits(PFSSData input, FileInfo output)
        {
            short[] ptr;
            short[] ptph;
            short[] ptth;
            short[] ptr_nz_len = new short[input.lines.Count];

            int totalCount = 0;
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                int count = input.lines[i].points.Count;
                totalCount += count;
                ptr_nz_len[i] = (short)count;
            }

            ptr = new short[totalCount];
            ptph = new short[totalCount];
            ptth = new short[totalCount];

            int index = 0;
            BinaryWriter w = new BinaryWriter(new FileStream(output.FullName,FileMode.Create));
            w.Write(input.b0);
            w.Write(input.l0);
            w.Write(ptr_nz_len.Length);
            foreach (short s in ptr_nz_len)
            {
                w.Write(s);
            }
            foreach (PFSSLine l in input.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    w.Write((short)p.x);
                }
            }
            foreach (PFSSLine l in input.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    w.Write((short)p.y);
                }
            }
            foreach (PFSSLine l in input.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    w.Write((short)p.z);
                }
            }


            w.Close();
        }

       
    }
}
