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
    class PredictiveResidualWriter
    {
        public static void WritePureShortFits(PFSSData input, int offset, FileInfo output)
        {
            offset = 1;
            short[] startPoints = new short[input.lines.Count * offset * 3*2];
            int[] ptr;
            int[] ptph;
            int[] ptth;
            short[] ptr_nz_len = new short[input.lines.Count];
            float[] means = new float[input.lines.Count * 3];
            short[] pca = new short[input.lines.Count * 6];

            int totalCount = 0;
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                Residuals res = input.lines[i].residuals;
                int count = res.predictionErrors.Count;

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
            int extraPointIndex = 0;

            foreach (PFSSLine l in input.lines)
            {
                //write start points
                for (int i = 0; i < offset; i++)
                {
                    startPoints[startPointIndex++] = (short)l.residuals.startPoint.x;
                    startPoints[startPointIndex++] = (short)l.residuals.startPoint.y;
                    startPoints[startPointIndex++] = (short)l.residuals.startPoint.z;
                    startPoints[startPointIndex++] = (short)l.residuals.endPoint.x;
                    startPoints[startPointIndex++] = (short)l.residuals.endPoint.y;
                    startPoints[startPointIndex++] = (short)l.residuals.endPoint.z;
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

                Residuals res = l.residuals;
                short extraPoint = 0;
   
                for (int i = 0; i < res.predictionErrors.Count; i++)
                {
                    PFSSPoint p = res.predictionErrors[i];
                    ptr[index] = (short)p.x;
                    ptph[index] = (short)p.y;
                    ptth[index] = (short)p.z;
                    System.Console.WriteLine(p.x);
                    index++;
                 }
                
            }

            Fits fits = new Fits();

            Double[] b0a = new Double[] { input.b0 };
            Double[] l0a = new Double[] { input.l0 };
            Object[][] data = new Object[1][];
            Object[] dataRow = new Object[] { b0a, l0a, means, pca, ptr_nz_len, startPoints, DCTCoder.EncodeAdaptive(ptr), DCTCoder.EncodeAdaptive(ptph), DCTCoder.EncodeAdaptive(ptth) };
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

        public static void WritePureShortFits_WithoutPCA(PFSSData input, int offset, FileInfo output)
        {
            offset = 1;
            int[] startPointsR = new int[input.lines.Count * offset];
            int[] startPointsPhi = new int[input.lines.Count * offset];
            int[] startPointsTheta = new int[input.lines.Count * offset];
            int[] endpointsR = new int[input.lines.Count * offset];
            int[] endpointsPhi = new int[input.lines.Count * offset];
            int[] endpointsTheta = new int[input.lines.Count * offset];
            int[] ptr;
            int[] ptph;
            int[] ptth;
            int[] ptr_nz_len = new int[input.lines.Count];

            List<PFSSLine> lines = new List<PFSSLine>(input.lines);
            //lines.Sort();

            int totalCount = 0;
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                Residuals res = lines[i].residuals;
                int count = res.predictionErrors.Count;

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
                //write start points
                for (int i = 0; i < offset; i++)
                {
                    startPointsR[startPointIndex] = (int)l.residuals.startPoint.x;
                    startPointsPhi[startPointIndex] = (int)l.residuals.startPoint.y;
                    startPointsTheta[startPointIndex] = (int)l.residuals.startPoint.z;
                    endpointsR[startPointIndex] = (int)l.residuals.endPoint.x;
                    endpointsPhi[startPointIndex] = (int)l.residuals.endPoint.y;
                    endpointsTheta[startPointIndex] = (int)l.residuals.endPoint.z;
                    startPointIndex++;
                }

                Residuals res = l.residuals;

                for (int i = 0; i < res.predictionErrors.Count; i++)
                {
                    PFSSPoint p = res.predictionErrors[i];
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
                DCTCoder.EncodeAdaptiveUnsigned(ptr_nz_len), 
                DCTCoder.EncodeStartPointChannel(startPointsR),
                DCTCoder.EncodeStartPointChannel(endpointsR),
                DCTCoder.EncodeStartPointChannel(startPointsPhi),
                DCTCoder.EncodeStartPointChannel(endpointsPhi),
                DCTCoder.EncodeStartPointChannel(startPointsTheta),
                DCTCoder.EncodeStartPointChannel(endpointsTheta), 
                DCTCoder.EncodeAdaptive(ptr), DCTCoder.EncodeAdaptive(ptph), DCTCoder.EncodeAdaptive(ptth) };
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
            bhdu.SetColumnName(3, "StartPointsR", null);
            bhdu.SetColumnName(4, "EndpointsR", null);
            bhdu.SetColumnName(5, "StartPointsPhi", null);
            bhdu.SetColumnName(6, "EndpointsPhi", null);
            bhdu.SetColumnName(7, "StartPointsTheta", null);
            bhdu.SetColumnName(8, "EndpointsTheta", null);
            bhdu.SetColumnName(9, "R", null);
            bhdu.SetColumnName(10, "PHI", null);
            bhdu.SetColumnName(11, "THETA", null);

            BufferedDataStream f = new BufferedDataStream(new FileStream(output.FullName, FileMode.Create));
            fits.Write(f);
            f.Close();

        }
    }
}
