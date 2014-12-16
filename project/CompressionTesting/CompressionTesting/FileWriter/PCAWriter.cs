using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;
using nom.tam.fits;
using nom.tam.util;
using System.IO;
using CompressionTesting.Encoding;

namespace CompressionTesting.FileWriter
{
    class PCAWriter
    {
        public static void WriteFits(PFSSData input, int offset, FileInfo output)
        {
            short[] startPoints = new short[input.lines.Count * offset*3];
            short[] ptr;
            short[] ptph;
            short[] ptth;
            short[] ptr_nz_len = new short[input.lines.Count];
            //float[] startPoints = new float[input.lines.Count * 3 * offset];
            float[] means = new float[input.lines.Count * 3];
            float[] pca = new float[input.lines.Count*6];

            int totalCount = 0;
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                int count = input.lines[i].points.Count - offset;
                totalCount += count;
                ptr_nz_len[i] = (short)count;
            }

            ptr = new short[totalCount];
            ptph = new short[totalCount];
            ptth = new short[totalCount];

            int index = 0;
            int startPointIndex = 0;
            int meansIndex = 0;
            int pcaIndex = 0;

            foreach (PFSSLine l in input.lines)
            {
                //write start points
                for (int i = 0; i < offset; i++) 
                { 
                    startPoints[startPointIndex++] = (short)l.points[i].x;
                    startPoints[startPointIndex++] = (short)l.points[i].y;
                    startPoints[startPointIndex++] = (short)l.points[i].z;
                }

                //write coefficients
                for (int i = 0; i < 2; i++)
                {
                    pca[pcaIndex++] = l.pcaTransform[0, i];
                    pca[pcaIndex++] = l.pcaTransform[1, i];
                    pca[pcaIndex++] = l.pcaTransform[2, i];
                }

                //write medians
                for (int i = 0; i < 3; i++)
                {
                    means[meansIndex++] = l.means[i];
                }


                for (int i = 1; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
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
            Object[] dataRow = new Object[] { b0a,l0a, means, pca, ptr_nz_len, startPoints, ptr, ptph, ptth };
            data[0] = dataRow;
            //means, pca, startPoints,
            BinaryTable table = new BinaryTable(data);
            Header hdr = BinaryTableHDU.ManufactureHeader(table);
            fits.AddHDU(new BinaryTableHDU(hdr, table));
            BinaryTableHDU bhdu = (BinaryTableHDU)fits.GetHDU(1);
            bhdu.SetColumnName(0, "B0", null);
            bhdu.SetColumnName(1, "L0", null);
            bhdu.SetColumnName(2, "means", null);
            bhdu.SetColumnName(3, "pcaCoefficients", null);
            bhdu.SetColumnName(4, "PTR_NZ_LEN", null);
            bhdu.SetColumnName(5, "StartPoints", null);
            bhdu.SetColumnName(6, "PTR", null);
            bhdu.SetColumnName(7, "PTPH", null);
            bhdu.SetColumnName(8, "PTTH", null);


            BufferedDataStream f = new BufferedDataStream(new FileStream(output.FullName, FileMode.Create));
            fits.Write(f);
            f.Close();

        }



        public static void WriteIntFits(PFSSData input, int offset, FileInfo output)
        {
            int[] ptr;
            int[] ptph;
            int[] ptth;
            short[] ptr_nz_len = new short[input.lines.Count];
            //float[] startPoints = new float[input.lines.Count * 3 * offset];
            float[] means = new float[input.lines.Count * 3];
            float[] pca = new float[input.lines.Count * 6];

            int totalCount = 0;
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                int count = input.lines[i].points.Count - offset;
                totalCount += count;
                ptr_nz_len[i] = (short)count;
            }

            ptr = new int[totalCount];
            ptph = new int[totalCount];
            ptth = new int[totalCount];

            int index = 0;
            int startPointIndex = 0;
            int meansIndex = 0;
            int pcaIndex = 0;

            foreach (PFSSLine l in input.lines)
            {
                //write start points
                /*for (int i = 0; i < offset; i++) 
                { 
                    startPoints[startPointIndex++] = l.points[i].x;
                    startPoints[startPointIndex++] = l.points[i].y;
                    startPoints[startPointIndex++] = l.points[i].z;
                }*/

                //write coefficients
                for (int i = 0; i < 2; i++)
                {
                    pca[pcaIndex++] = l.pcaTransform[0, i];
                    pca[pcaIndex++] = l.pcaTransform[1, i];
                    pca[pcaIndex++] = l.pcaTransform[2, i];
                }

                //write medians
                for (int i = 0; i < 3; i++)
                {
                    means[meansIndex++] = l.means[i];
                }


                for (int i = 1; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
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
            Object[] dataRow = new Object[] { b0a, l0a, means, pca, ptr_nz_len, ptr, ptph, ptth };
            data[0] = dataRow;
            //means, pca, startPoints,
            BinaryTable table = new BinaryTable(data);
            Header hdr = BinaryTableHDU.ManufactureHeader(table);
            fits.AddHDU(new BinaryTableHDU(hdr, table));
            BinaryTableHDU bhdu = (BinaryTableHDU)fits.GetHDU(1);
            bhdu.SetColumnName(0, "B0", null);
            bhdu.SetColumnName(1, "L0", null);
            bhdu.SetColumnName(2, "means", null);
            bhdu.SetColumnName(3, "pcaCoefficients", null);
            bhdu.SetColumnName(4, "PTR_NZ_LEN", null);
            bhdu.SetColumnName(5, "PTR", null);
            bhdu.SetColumnName(6, "PTPH", null);
            bhdu.SetColumnName(7, "PTTH", null);


            BufferedDataStream f = new BufferedDataStream(new FileStream(output.FullName, FileMode.Create));
            fits.Write(f);
            f.Close();

        }




        public static void WritePureShortFits(PFSSData input, int offset, FileInfo output)
        {
            short[] startPoints = new short[input.lines.Count * offset * 3];
            short[] ptr;
            short[] ptph;
            short[] ptth;
            short[] ptr_nz_len = new short[input.lines.Count];
            //float[] startPoints = new float[input.lines.Count * 3 * offset];
            float[] means = new float[input.lines.Count * 3];
            short[] pca = new short[input.lines.Count * 6];

            int totalCount = 0;
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                int count = input.lines[i].points.Count - offset;
                totalCount += count;
                ptr_nz_len[i] = (short)count;
            }

            ptr = new short[totalCount];
            ptph = new short[totalCount];
            ptth = new short[totalCount];

            int index = 0;
            int startPointIndex = 0;
            int meansIndex = 0;
            int pcaIndex = 0;

            foreach (PFSSLine l in input.lines)
            {
                //write start points
                for (int i = 0; i < offset; i++)
                {
                    startPoints[startPointIndex++] = (short)l.points[i].x;
                    startPoints[startPointIndex++] = (short)l.points[i].y;
                    startPoints[startPointIndex++] = (short)l.points[i].z;
                }

                //write coefficients
                for (int i = 0; i < 2; i++)
                {
                    pca[pcaIndex++] = (short)l.pcaTransform[0, i];
                    pca[pcaIndex++] = (short)l.pcaTransform[1, i];
                    pca[pcaIndex++] = (short)l.pcaTransform[2, i];
                }

                //write medians
                for (int i = 0; i < 3; i++)
                {
                    means[meansIndex++] = l.means[i];
                }


                for (int i = 1; i < l.points.Count; i++)
                {
                    PFSSPoint p = l.points[i];
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
            Object[] dataRow = new Object[] { b0a, l0a, means, pca, ptr_nz_len, startPoints, ptr, ptph, ptth };
            data[0] = dataRow;
            //means, pca, startPoints,
            BinaryTable table = new BinaryTable(data);
            Header hdr = BinaryTableHDU.ManufactureHeader(table);
            fits.AddHDU(new BinaryTableHDU(hdr, table));
            BinaryTableHDU bhdu = (BinaryTableHDU)fits.GetHDU(1);
            bhdu.SetColumnName(0, "B0", null);
            bhdu.SetColumnName(1, "L0", null);
            bhdu.SetColumnName(2, "means", null);
            bhdu.SetColumnName(3, "pcaCoefficients", null);
            bhdu.SetColumnName(4, "PTR_NZ_LEN", null);
            bhdu.SetColumnName(5, "StartPoints", null);
            bhdu.SetColumnName(6, "PTR", null);
            bhdu.SetColumnName(7, "PTPH", null);
            bhdu.SetColumnName(8, "PTTH", null);


            BufferedDataStream f = new BufferedDataStream(new FileStream(output.FullName, FileMode.Create));
            fits.Write(f);
            f.Close();

        }



        public static void WritePureByteFits(PFSSData input, int offset, FileInfo output)
        {
            int[] ptr_nz_len = new int[input.lines.Count];
            int[] ptr = new int[input.lines.Count];
            int[] ptph = new int[input.lines.Count];
            int[] ptth = new int[input.lines.Count];
            short[] means = new short[input.lines.Count * 3];
            short[] pca = new short[input.lines.Count * 7];

            List<PFSSLine> lines = new List<PFSSLine>(input.lines);
            lines.Sort();

            int totalCount = 0;
            int byteCountX = 0;
            int byteCountY = 0;
            int byteCountZ = 0;

            List<List<byte[]>> temp = new List<List<byte[]>>(lines.Count);
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                int count = lines[i].points.Count - 1;
                totalCount += count;
                ptr_nz_len[i] = (short)count;

                List<byte[]> line = DCTCoder.EncodeChannels(lines[i], 1);
                byteCountX += line[0].Length;
                byteCountY += line[1].Length;
                byteCountZ += line[2].Length;
                temp.Add(line);
            }

            byte[] ptr_nz_len_byte = DCTCoder.EncodeAdaptive(ptr_nz_len);
            byte[][] channels = new byte[3][];
            channels[0] = new byte[byteCountX];
            channels[1] = new byte[byteCountY];
            channels[2] = new byte[byteCountZ];

            //do startpoints;

            //run over all lines
            int[] channelIndices = new int[3];
            int startPointIndex = 0;
            int pcaIndex = 0;
            int meansIndex = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                PFSSLine l = lines[i];
                ptr[startPointIndex] = (short)l.points[0].x;
                ptph[startPointIndex] = (short)l.points[0].y;
                ptth[startPointIndex] = (short)l.points[0].z;
                startPointIndex++;

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

                    for (int k = 0; k < toCopy.Length; k++)
                        channel[index++] = toCopy[k];

                    channelIndices[j] = index;
                }

            }

            Fits fits = new Fits();

            Double[] b0a = new Double[] { input.b0 };
            Double[] l0a = new Double[] { input.l0 };
            Object[][] data = new Object[1][];
            Object[] dataRow = new Object[] {b0a,l0a,means,pca,DCTCoder.EncodeStartPointChannel(ptr), DCTCoder.EncodeStartPointChannel(ptph), DCTCoder.EncodeStartPointChannel(ptth), ptr_nz_len_byte, channels[0], channels[1], channels[2] };
            data[0] = dataRow;
            //means, pca, startPoints,
            BinaryTable table = new BinaryTable(data);
            Header hdr = BinaryTableHDU.ManufactureHeader(table);
            hdr.AddValue("Version", "1.0", null);
            fits.AddHDU(new BinaryTableHDU(hdr, table));
            BinaryTableHDU bhdu = (BinaryTableHDU)fits.GetHDU(1);
            bhdu.SetColumnName(0, "B0", null);
            bhdu.SetColumnName(1, "L0", null);
            bhdu.SetColumnName(2, "means", null);
            bhdu.SetColumnName(3, "pca", null);
            bhdu.SetColumnName(4, "StartPointR", null);
            bhdu.SetColumnName(5, "StartPointPhi", null);
            bhdu.SetColumnName(6, "StartPointTheta", null);
            bhdu.SetColumnName(7, "LINE_LENGTH", null);
            bhdu.SetColumnName(8, "X", null);
            bhdu.SetColumnName(9, "Y", null);
            bhdu.SetColumnName(10, "Z", null);


            BufferedDataStream f = new BufferedDataStream(new FileStream(output.FullName, FileMode.Create));
            fits.Write(f);
            f.Close();

        }
    }
}
