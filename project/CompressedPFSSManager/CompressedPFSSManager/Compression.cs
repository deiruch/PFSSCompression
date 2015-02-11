using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CompressedPFSSManager.PFSS;
using CompressedPFSSManager.CompressionAlgorithm;

namespace CompressedPFSSManager
{
    class Compression
    {
        /// <summary>
        /// Compresses a FITS file
        /// </summary>
        /// <param name="fits"></param>
        /// <returns>FileInfo of compressed RAR file</returns>
        public static FileInfo CompressLossy(FileInfo fits)
        {
            PFSSData data = FitsReader.ReadFloatFits(fits);
            FileInfo rarFits = new FileInfo(fits.FullName + ".rar");
            Subsampling.AdaptiveSubsampling(data, 3);

            SphericalCoordinates.To16BitSpherical(data);
            RecursiveLinearPredictor.ForwardPrediction(data);

            OutputWriter.WriteFits(data, fits);
            OutputWriter.EncodeRAR(rarFits, fits);
            fits.Delete();

            return rarFits;
        }
    }
}
