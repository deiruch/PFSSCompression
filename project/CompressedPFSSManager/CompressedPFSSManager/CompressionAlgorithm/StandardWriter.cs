using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CompressedPFSSManager.PFSS;
using nom.tam.fits;
using nom.tam.util;

namespace CompressedPFSSManager
{
    class StandardWriter
    {
        public static void WriteDCTByteFits(PFSSData input, FileInfo output)
        {
            short[] ptr_nz_len = new short[input.lines.Count];
            short[] startEnd = new short[input.lines.Count * 6];

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
                ptr_nz_len[i] = (short)count;

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
    }
}
