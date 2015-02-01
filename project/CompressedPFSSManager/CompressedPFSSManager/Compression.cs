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
        public static FileInfo Compress(FileInfo fits)
        {
            PFSSData data = FitsReader.ReadFloatFits(fits);
            FileInfo rarFits = new FileInfo(fits.FullName + ".rar");
            Subsampling.AngleSubsample(data, 3);

            Spherical.ForwardToSpherical(data);
            Spherical.ForwardMoveSpherical(data);

            RecursiveLinearPredictor.ForwardPrediction(data);

            PredictionFitsWriter.WriteFits(data, fits);
            RarCompression.DoRar(rarFits, fits);
            fits.Delete();

            return rarFits;
        }
    }
}
