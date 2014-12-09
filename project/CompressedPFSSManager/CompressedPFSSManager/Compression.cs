using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CompressedPFSSManager.PFSS;

namespace CompressedPFSSManager
{
    class Compression
    {
        public static FileInfo Compress(FileInfo fits, int qualityLevel)
        {
            System.Console.WriteLine(fits.FullName);
            PFSSData data = FitsReader.ReadFloatFits(fits);
            FileInfo rarFits = new FileInfo(fits.FullName + ".rar");

            Subsampling.Subsample(data, 4);
            DCTImprover.AddExtraPoints(data, 100000);
            DCTransformer.ForwardExtra(data, 0);

            ExtraPointDiscretizer.DividePoint(data, 800, 0);
            ExtraPointDiscretizer.DivideExtra(data, 1000);
            ExtraPointDiscretizer.DivideLinearExtra(data, 2 * (qualityLevel * 10 + 180), 1, 1);
            ExtraPointDiscretizer.ToShortsExtra(data);

            System.Console.WriteLine("DC Transform");
            StandardWriter.WriteDCTByteFits(data, fits);
            System.Console.WriteLine("Done DC");
            RarCompression.DoRar(rarFits, fits);
            fits.Delete();

            return rarFits;
        }
    }
}
