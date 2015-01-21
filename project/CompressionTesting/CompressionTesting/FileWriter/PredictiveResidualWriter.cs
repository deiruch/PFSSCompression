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
            short[] startPoints = new short[input.lines.Count * offset * 3];
            short[] ptr;
            short[] ptph;
            short[] ptth;
            short[] ptr_nz_len = new short[input.lines.Count];
            short[] extraPoints = new short[input.lines.Count];
            float[] means = new float[input.lines.Count * 3];
            short[] pca = new short[input.lines.Count * 6];

            int totalCount = 0;
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                Residuals res = input.lines[i].residuals;
                int count = 0;
                while (res != null)
                {
                    count += res.points.Count;
                    if (res.extra != null)
                        count++;
                    res = res.nextLevel;
                }
                
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
            int extraPointIndex = 0;

            foreach (PFSSLine l in input.lines)
            {
                //write start points
                for (int i = 0; i < offset; i++)
                {
                    startPoints[startPointIndex++] = (short)l.residuals.startAverage.x;
                    startPoints[startPointIndex++] = (short)l.residuals.startAverage.y;
                    startPoints[startPointIndex++] = (short)l.residuals.startAverage.z;
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
                while (res != null) 
                { 
                    for (int i = 0; i < res.points.Count; i++)
                    {
                        PFSSPoint p = res.points[i];
                        ptr[index] = (short)p.x;
                        ptph[index] = (short)p.y;
                        ptth[index] = (short)p.z;
                        index++;
                        
                    }

                    if(res.extra != null) {
                        PFSSPoint p = res.extra;
                        ptr[index] = (short)p.x;
                        ptph[index] = (short)p.y;
                        ptth[index] = (short)p.z;
                        index++;
                        extraPoint++;
                        
                    }
                    extraPoint <<= 1;
                     res = res.nextLevel;
                }
                extraPoints[extraPointIndex++] = extraPoint;
            }

            Fits fits = new Fits();

            Double[] b0a = new Double[] { input.b0 };
            Double[] l0a = new Double[] { input.l0 };
            Object[][] data = new Object[1][];
            Object[] dataRow = new Object[] { b0a, l0a, means, pca, ptr_nz_len,extraPoints, startPoints, ptr, ptph, ptth };
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
    }
}
