﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using nom.tam.fits;

namespace CompressedPFSSManager.PFSS
{
    class FitsReader
    {
        /// <summary>
        /// Reads a fits file and puts them in the PFSSData structure for compression
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static PFSSData ReadFloatFits(FileInfo input)
        {
            bool compressed = input.Extension == ".gz";

            Fits fits = new Fits(input, compressed);
            BasicHDU[] hdus = fits.Read();
            BinaryTableHDU bhdu = (BinaryTableHDU)hdus[1];
            //bhdu1.
            string type = string.Join(" ", ((string[])bhdu.GetColumn("TYPE")).Select(v => v.ToString()));
            string date = string.Join(" ", ((string[])bhdu.GetColumn("DATE_TIME")).Select(v => v.ToString()));

            double b0 = ((double[])bhdu.GetColumn("B0"))[0];
            double l0 = ((double[])bhdu.GetColumn("L0"))[0];
            float[] ptr = ((float[])((Array[])bhdu.GetColumn("PTR"))[0]);
            short[] ptr_nz_len = ((short[])((Array[])bhdu.GetColumn("PTR_NZ_LEN"))[0]);
            float[] ptph = ((float[])((Array[])bhdu.GetColumn("PTPH"))[0]);
            float[] ptth = ((float[])((Array[])bhdu.GetColumn("PTTH"))[0]);

            var data = new SphericalDataCreator(l0, b0, ptr, ptr_nz_len, ptph, ptth).Create();
            fits.Close();
            return data;
        }
    }
}
