using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressedPFSSManager.PFSS;
using nom.tam.fits;
using nom.tam.util;
using System.IO;
using System.Diagnostics;

namespace CompressedPFSSManager.CompressionAlgorithm
{
    class OutputWriter
    {
        /// <summary>
        /// Encode a fits file as rar. This has to be a two step process and 
        /// called via console due to licence restrictions of the RAR API
        /// </summary>
        /// <param name="archivePath"></param>
        /// <param name="fitsPath"></param>
        public static void EncodeRAR(FileInfo archivePath, FileInfo fitsPath)
        {
            Process pProcess = new Process();
            pProcess.StartInfo.FileName = "rar";
            pProcess.StartInfo.Arguments = "a -m5 -ma4 \"" + archivePath.Name + "\" \"" + fitsPath.Name + "\"";

            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.WorkingDirectory = fitsPath.Directory.FullName;
            pProcess.Start();
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            pProcess.WaitForExit();
        }

        /// <summary>
        /// Write an FITS file at output destination
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public static void WriteFits(PFSSData input, FileInfo output)
        {
            int[] ptx;
            int[] pty;
            int[] ptz;
            int[] ptr_nz_len = new int[input.lines.Count];

            List<PFSSLine> lines = input.lines;

            int totalCount = 0;
            for (int i = 0; i < ptr_nz_len.Length; i++)
            {
                int count = lines[i].predictionErrors.Count;

                totalCount += count;
                ptr_nz_len[i] = (int)count;
            }

            ptx = new int[totalCount];
            pty = new int[totalCount];
            ptz = new int[totalCount];

            int index = 0;
            foreach (PFSSLine l in lines)
            {
                for (int i = 0; i < l.predictionErrors.Count; i++)
                {
                    PFSSPoint p = l.predictionErrors[i];
                    ptx[index] = (int)p.X;
                    pty[index] = (int)p.Y;
                    ptz[index] = (int)p.Z;
                    index++;
                }

            }


            var table = new BinaryTable(new Object[][]
            {
                new Object[]
                {
                    ByteEncoder.EncodeAdaptiveUnsigned(ptr_nz_len), 
                    ByteEncoder.EncodeAdaptive(ptx),
                    ByteEncoder.EncodeAdaptive(pty),
                    ByteEncoder.EncodeAdaptive(ptz)
                }
            });

            var hdr = BinaryTableHDU.ManufactureHeader(table);
            hdr.AddValue("B0", (float)input.b0, null);
            hdr.AddValue("L0", (float)input.l0, null);
            hdr.AddValue("Q1", (float)RecursiveLinearPredictor.Q_FACTOR1, null);
            hdr.AddValue("Q2", (float)RecursiveLinearPredictor.Q_FACTOR2, null);
            hdr.AddValue("Q3", (float)RecursiveLinearPredictor.Q_FACTOR3, null);

            var bhdu = new BinaryTableHDU(hdr, table);
            bhdu.SetColumnName(0, "LEN", null);
            bhdu.SetColumnName(1, "X", null);
            bhdu.SetColumnName(2, "Y", null);
            bhdu.SetColumnName(3, "Z", null);

            using (var f = new BufferedDataStream(new FileStream(output.FullName, FileMode.Create)))
            {
                var fits = new Fits();
                fits.AddHDU(bhdu);
                fits.Write(f);
            }
        }
    }
}
