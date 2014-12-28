using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CompressionTesting.PFSS;
using nom.tam.fits;
using nom.tam.util;
using CompressionTesting.Encoding;

namespace CompressionTesting.FileWriter
{
    class StandardWriter
    {
        public static void WriteIntFits(PFSSData input, FileInfo output)
        {
            int[] ptr;
            int[] ptph;
            int[] ptth;
            short[] ptr_nz_len = new short[input.lines.Count];

            int totalCount = 0;
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                int count = input.lines[i].points.Count;
                totalCount += count;
                ptr_nz_len[i] = (short)count;
            }

            ptr = new int[totalCount];
            ptph = new int[totalCount];
            ptth = new int[totalCount];

            int index = 0;

            foreach (PFSSLine l in input.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    ptr[index] = (int)p.x;
                    ptph[index] = (int)p.y;
                    ptth[index] = (int)p.z;
                    index++;
                }
            }

            Fits fits = new Fits();

            Double[] b0a = new Double[] { input.b0 };
            Double[] l0a = new Double[] { input.l0 };
            Object[][] data = new Object[1][];
            Object[] dataRow = new Object[] { b0a, l0a, ptr_nz_len, ptr, ptph, ptth };
            data[0] = dataRow;

            BinaryTable table = new BinaryTable(data);
            Header hdr = BinaryTableHDU.ManufactureHeader(table);
            fits.AddHDU(new BinaryTableHDU(hdr, table));
            BinaryTableHDU bhdu = (BinaryTableHDU)fits.GetHDU(1);
            bhdu.SetColumnName(0, "B0", null);
            bhdu.SetColumnName(1, "L0", null);
            bhdu.SetColumnName(2, "PTR_NZ_LEN", null);
            bhdu.SetColumnName(3, "PTR", null);
            bhdu.SetColumnName(4, "PTPH", null);
            bhdu.SetColumnName(5, "PTTH", null);


            BufferedDataStream f = new BufferedDataStream(new FileStream(output.FullName, FileMode.Create));
            fits.Write(f);
            f.Close();
        }

        public static void WriteShortFits(PFSSData input, FileInfo output)
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

            int index = 0;

            foreach(PFSSLine l in input.lines)
            {
                foreach (PFSSPoint p in l.points)
                {
                    ptr[index] = (short)p.x;
                    ptph[index] = (short)p.y;
                    ptth[index] = (short)p.z;
                    index++;
                }
            }

            Fits fits = new Fits();

            Double[] b0a = new Double[] { input.b0 };
            Double[] l0a = new Double[] { input.l0 };
            Object[][] data = new Object[1][];
            Object[] dataRow = new Object[] { b0a, l0a, ptr_nz_len, ptr, ptph, ptth };
            data[0] = dataRow;

            BinaryTable table = new BinaryTable(data);
            Header hdr = BinaryTableHDU.ManufactureHeader(table);
            fits.AddHDU(new BinaryTableHDU(hdr, table));
            BinaryTableHDU bhdu = (BinaryTableHDU)fits.GetHDU(1);
            bhdu.SetColumnName(0, "B0", null);
            bhdu.SetColumnName(1, "L0", null);
            bhdu.SetColumnName(2, "PTR_NZ_LEN", null);
            bhdu.SetColumnName(3, "PTR", null); 
            bhdu.SetColumnName(4, "PTPH", null);
            bhdu.SetColumnName(5, "PTTH", null);
            

            BufferedDataStream f = new BufferedDataStream(new FileStream(output.FullName, FileMode.Create));
            fits.Write(f);
            f.Close();
        
        }

        public static void WriteDCTByteFits(PFSSData input, FileInfo output)
        {
            int[] ptr_nz_len = new int[input.lines.Count];
            int[] startEnd = new int[input.lines.Count * 6];

            int totalCount = 0;
            int byteCountX = 0;
            int byteCountY = 0;
            int byteCountZ = 0;
            List<List<byte[]>> temp = new List<List<byte[]>>(input.lines.Count);
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                PFSSLine l = input.lines[i];
                int count = input.lines[i].points.Count;
                totalCount += count;
                ptr_nz_len[i] = (int)count;

                List<byte[]> line = new List<byte[]>(3);
                line.Add(DCTCoder.EncodeChannel(l.extraX));
                line.Add(DCTCoder.EncodeChannel(l.extraY));
                line.Add(DCTCoder.EncodeChannel(l.extraZ));
                byteCountX += line[0].Length;
                byteCountY += line[1].Length;
                byteCountZ += line[2].Length;
                temp.Add(line);
            }

            byte[] ptr_nz_len_byte = DCTCoder.EncodeAdaptiveUnsigned(ptr_nz_len);
            byte[][] channels = new byte[3][];
            channels[0] = new byte[byteCountX];
            channels[1] = new byte[byteCountY];
            channels[2] = new byte[byteCountZ];

            int[] channelIndices = new int[3];
            int startEndIndex = 0;
            for (int i = 0; i < input.lines.Count; i++)
            {
                PFSSLine l = input.lines[i];

                List<byte[]> bytePoints = temp[i];
                for (int j = 0; j < channels.Length; j++)
                {
                    int index = channelIndices[j];
                    byte[] channel = channels[j];
                    byte[] toCopy = bytePoints[j];
                    startEnd[startEndIndex++] = (byte)input.lines[i].extra[j].startLength;
                    startEnd[startEndIndex++] = (byte)input.lines[i].extra[j].endLength;

                    for (int k = 0; k < toCopy.Length; k++)
                        channel[index++] = toCopy[k];

                    channelIndices[j] = index;
                }

            }

            Fits fits = new Fits();

            Object[][] data = new Object[1][];
            Object[] dataRow = new Object[] { DCTCoder.EncodeAdaptiveUnsigned(startEnd), ptr_nz_len_byte, channels[0], channels[1], channels[2] };
            data[0] = dataRow;

            BinaryTable table = new BinaryTable(data);
            Header hdr = BinaryTableHDU.ManufactureHeader(table);
            hdr.AddValue("Version", "1.0", null);
            fits.AddHDU(new BinaryTableHDU(hdr, table));
            BinaryTableHDU bhdu = (BinaryTableHDU)fits.GetHDU(1);
            bhdu.SetColumnName(0, "START_END", null);
            bhdu.SetColumnName(1, "LINE_LENGTH", null);
            bhdu.SetColumnName(2, "X", null);
            bhdu.SetColumnName(3, "Y", null);
            bhdu.SetColumnName(4, "Z", null);

            BufferedDataStream f = new BufferedDataStream(new FileStream(output.FullName, FileMode.Create));
            fits.Write(f);
            f.Close();
        }

        public static void WritePCADCTByteFits(PFSSData input, FileInfo output)
        {
            int[] ptr_nz_len = new int[input.lines.Count];
            int[] startEnd = new int[input.lines.Count * 6];
            short[] means = new short[input.lines.Count * 3];
            short[] pca = new short[input.lines.Count * 7];

            int totalCount = 0;
            int byteCountX = 0;
            int byteCountY = 0;
            int byteCountZ = 0;
            List<List<byte[]>> temp = new List<List<byte[]>>(input.lines.Count);
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                PFSSLine l = input.lines[i];
                int count = input.lines[i].points.Count;
                totalCount += count;
                ptr_nz_len[i] = (int)count;

                List<byte[]> line = new List<byte[]>(3);
                line.Add(DCTCoder.EncodeChannel(l.extraX));
                line.Add(DCTCoder.EncodeChannel(l.extraY));
                line.Add(DCTCoder.EncodeChannel(l.extraZ));
                byteCountX += line[0].Length;
                byteCountY += line[1].Length;
                byteCountZ += line[2].Length;
                temp.Add(line);
            }

            byte[] ptr_nz_len_byte = DCTCoder.EncodeAdaptiveUnsigned(ptr_nz_len);
            byte[][] channels = new byte[3][];
            channels[0] = new byte[byteCountX];
            channels[1] = new byte[byteCountY];
            channels[2] = new byte[byteCountZ];

            int[] channelIndices = new int[3];
            int startEndIndex = 0;
            int pcaIndex = 0;
            int meansIndex = 0;
            for (int i = 0; i < input.lines.Count; i++)
            {
                PFSSLine l = input.lines[i];
                //write coefficients
                for (int j = 0; j < 2; j++)
                {
                    pca[pcaIndex++] = (short)l.pcaTransform[0, j];
                    pca[pcaIndex++] = (short)l.pcaTransform[1, j];
                    pca[pcaIndex++] = (short)l.pcaTransform[2, j];
                }
                pca[pcaIndex++] = (short)l.minus;
                //write medians
                for (int j = 0; j < 3; j++)
                {
                    means[meansIndex++] = (short)l.means[j];
                }

                List<byte[]> bytePoints = temp[i];
                for (int j = 0; j < channels.Length; j++)
                {
                    int index = channelIndices[j];
                    byte[] channel = channels[j];
                    byte[] toCopy = bytePoints[j];
                    startEnd[startEndIndex++] = (byte)input.lines[i].extra[j].startLength;
                    startEnd[startEndIndex++] = (byte)input.lines[i].extra[j].endLength;

                    for (int k = 0; k < toCopy.Length; k++)
                        channel[index++] = toCopy[k];

                    channelIndices[j] = index;
                }

            }

            Fits fits = new Fits();

            Object[][] data = new Object[1][];
            Object[] dataRow = new Object[] {DCTCoder.EncodeAdaptiveUnsigned(startEnd),means,pca, ptr_nz_len_byte, channels[0], channels[1], channels[2] };
            data[0] = dataRow;

            BinaryTable table = new BinaryTable(data);
            Header hdr = BinaryTableHDU.ManufactureHeader(table);
            hdr.AddValue("Version", "1.0", null);
            fits.AddHDU(new BinaryTableHDU(hdr, table));
            BinaryTableHDU bhdu = (BinaryTableHDU)fits.GetHDU(1);
            bhdu.SetColumnName(0, "START_END", null);
            bhdu.SetColumnName(1, "LINE_LENGTH", null);
            bhdu.SetColumnName(2, "Means", null);
            bhdu.SetColumnName(3, "PCA", null);
            bhdu.SetColumnName(4, "X", null);
            bhdu.SetColumnName(5, "Y", null);
            bhdu.SetColumnName(6, "Z", null);

            BufferedDataStream f = new BufferedDataStream(new FileStream(output.FullName, FileMode.Create));
            fits.Write(f);
            f.Close();
        }
    }
}
