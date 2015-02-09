using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressedPFSSManager.PFSS;
using nom.tam.fits;
using nom.tam.util;
using System.IO;

namespace CompressedPFSSManager.CompressionAlgorithm
{
    class PredictionFitsWriter
    {
        public static void WriteFits(PFSSData input, FileInfo output)
        {
            int[] startPointsR = new int[input.lines.Count];
            int[] startPointsPhi = new int[input.lines.Count];
            int[] startPointsTheta = new int[input.lines.Count ];
            int[] endpointsR = new int[input.lines.Count];
            int[] endpointsPhi = new int[input.lines.Count];
            int[] endpointsTheta = new int[input.lines.Count];
            int[] ptr;
            int[] ptph;
            int[] ptth;
            int[] ptr_nz_len = new int[input.lines.Count];

            List<PFSSLine> lines = new List<PFSSLine>(input.lines);

            int totalCount = 0;
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                int count = lines[i].predictionErrors.Count;

                totalCount += count;
                ptr_nz_len[i] = (int)count;
            }

            ptr = new int[totalCount];
            ptph = new int[totalCount];
            ptth = new int[totalCount];

            int index = 0;
            int startPointIndex = 0;

            foreach (PFSSLine l in lines)
            {

                startPointsR[startPointIndex] = (int)l.startPoint.x;
                startPointsPhi[startPointIndex] = (int)l.startPoint.y;
                startPointsTheta[startPointIndex] = (int)l.startPoint.z;
                endpointsR[startPointIndex] = (int)l.endPoint.x;
                endpointsPhi[startPointIndex] = (int)l.endPoint.y;
                endpointsTheta[startPointIndex] = (int)l.endPoint.z;
                startPointIndex++;

                for (int i = 0; i < l.predictionErrors.Count; i++)
                {
                    PFSSPoint p = l.predictionErrors[i];
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
            Object[] dataRow = new Object[] { b0a, l0a, 
                ByteEncoder.EncodeAdaptiveUnsigned(ptr_nz_len), 
                ByteEncoder.EncodeAdaptive(startPointsR),
                ByteEncoder.EncodeAdaptive(endpointsR),
                ByteEncoder.EncodeAdaptive(startPointsPhi),
                ByteEncoder.EncodeAdaptive(endpointsPhi),
                ByteEncoder.EncodeAdaptive(startPointsTheta),
                ByteEncoder.EncodeAdaptive(endpointsTheta), 
                ByteEncoder.EncodeAdaptive(ptr), ByteEncoder.EncodeAdaptive(ptph), ByteEncoder.EncodeAdaptive(ptth) };
            data[0] = dataRow;
            //means, pca, startPoints,
            BinaryTable table = new BinaryTable(data);
            Header hdr = BinaryTableHDU.ManufactureHeader(table);
            hdr.AddValue("Version", "1.0", null);
            fits.AddHDU(new BinaryTableHDU(hdr, table));
            BinaryTableHDU bhdu = (BinaryTableHDU)fits.GetHDU(1);
            bhdu.SetColumnName(0, "B0", null);
            bhdu.SetColumnName(1, "L0", null);
            bhdu.SetColumnName(2, "LINE_LENGTH", null);
            bhdu.SetColumnName(3, "START_R", null);
            bhdu.SetColumnName(4, "END_R", null);
            bhdu.SetColumnName(5, "START_PHI", null);
            bhdu.SetColumnName(6, "END_PHI", null);
            bhdu.SetColumnName(7, "START_THETA", null);
            bhdu.SetColumnName(8, "END_THETA", null);
            bhdu.SetColumnName(9, "CHANNEL_R", null);
            bhdu.SetColumnName(10, "CHANNEL_PHI", null);
            bhdu.SetColumnName(11, "CHANNEL_THETA", null);

            BufferedDataStream f = new BufferedDataStream(new FileStream(output.FullName, FileMode.Create));
            fits.Write(f);
            f.Close();
        }
    }
}
