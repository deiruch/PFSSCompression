using CompressedPFSSManager.CompressionAlgorithm;
using CompressedPFSSManager.PFSS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace CompressedPFSSManager
{
    class Program
    {
        static DirectoryInfo DestDir;
        static void Main(string[] args)
        {
            //var output2 = CompressLossy(new FileInfo(@"C:\Users\de\Documents\Visual Studio 2013\Projects\PfssManagerCompressed\project\CompressedPFSSManager\CompressedPFSSManager\bin\Debug\1996-07-02_18-04-00.000_pfss_field_data.fits"));
            //return;

            if (args.Length != 2)
            {
                Console.WriteLine("PfssManager.exe [DestinationDirectory] [SswStartupScript]");
                return;
            }
            DestDir = Directory.CreateDirectory(args[0]);
            //first: ?Bfield_1996-07-01T06:04:00.h5
            for (; ; )
            {
                var requestDate = GetLastFileDate().AddMinutes(1);
                var tmpDir = GetTemporaryDirectory();
                Console.WriteLine("Requesting " + requestDate.ToString("yyyy-MM-dd HH:mm:ss"));
                Environment.SetEnvironmentVariable("PFSS_DATE", requestDate.ToString("yyyy-MM-dd HH:mm:ss"));
                Environment.SetEnvironmentVariable("PFSS_OUTPUT", tmpDir.FullName);
                try
                {
                    File.Delete("idllog.txt");
                }
                catch
                {
                }

                var proc = Process.Start(args[1], Path.Combine(Environment.CurrentDirectory, "pfss_run_batch.pro") + " > idllog.txt");
                if (!proc.WaitForExit(5 * 60 * 1000))
                {
                    Console.WriteLine("IDL is not responding... :(");
                    try
                    {
                        proc.Kill();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    tmpDir.Delete(true);
                    Thread.Sleep(30000);
                    continue;
                }

                try
                {
                    proc.Kill();
                }
                catch (InvalidOperationException)
                {
                }

                if (tmpDir.EnumerateFiles("*.fits").Count() == 0)
                {
                    Console.WriteLine("Could not find any FITS files in output directory " + tmpDir.FullName);
                    tmpDir.Delete(true);
                    Thread.Sleep(30000);
                    continue;
                }


                if (tmpDir.EnumerateFiles("*.fits").Count() != 1)
                {
                    Console.WriteLine("Did not find exactly one FITS file in output directory " + tmpDir.FullName);
                    return;
                }
                var fits = tmpDir.EnumerateFiles("*.fits").SingleOrDefault();
                var output = CompressLossy(fits);

                //var output = tmpDir.EnumerateFiles("*.fits.rar").SingleOrDefault();
                //---------------------------Modification by Jonas Schwammberger---------------------------------------------------


                if (output == null)
                {
                    Console.WriteLine("Did not find exactly one compressed file in output directory " + tmpDir.FullName);
                    return;
                }
                var outputDate = GetDate(output.Name);
                var monthDir = DestDir.CreateSubdirectory(Path.Combine(outputDate.ToString("yyyy"), outputDate.ToString("MM")));
                var destFn = outputDate.ToString("yyyy-MM-dd_HH-mm") + "_pfss_field_data.fits.rar";
                output.CopyTo(Path.Combine(monthDir.FullName, destFn));
                tmpDir.Delete(true);
                using (var l = File.AppendText(Path.Combine(monthDir.FullName, "list.txt")))
                {
                    l.WriteLine(outputDate.ToString("yyyy-MM-ddTHH:mm:ss") + " " + destFn);
                }
                Thread.Sleep(2000);
            }
        }
        static DirectoryInfo GetTemporaryDirectory()
        {
            return Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
        }
        static DateTime GetLastFileDate()
        {
            int tmp;
            var maxYear = DestDir.EnumerateDirectories().Where(dir => dir.Name.Length == 4 && int.TryParse(dir.Name, out tmp) && int.Parse(dir.Name) > 1900).Max(dir => dir.Name);
            if (maxYear == null)
                return new DateTime(1900, 1, 1);
            var maxMonth = DestDir.GetDirectories(maxYear).Single().EnumerateDirectories().Where(dir => dir.Name.Length == 2 && int.TryParse(dir.Name, out tmp) && int.Parse(dir.Name) <= 12).Max(dir => dir.Name);
            if (maxMonth == null)
                return new DateTime(int.Parse(maxYear), 1, 1);
            var maxFile = DestDir.GetDirectories(maxYear).Single().GetDirectories(maxMonth).Single().EnumerateFiles("*_pfss_field_data.fits.rar").Max(dir => dir.Name);
            if (maxFile == null)
                return new DateTime(int.Parse(maxYear), int.Parse(maxMonth), 1);
            return GetDate(maxFile);
        }
        static DateTime GetDate(string _path)
        {
            var fn = Path.GetFileNameWithoutExtension(_path);
            try
            {
                return DateTime.ParseExact(fn.Substring(0, 16), "yyyy-MM-dd_HH-mm", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                return DateTime.ParseExact(fn.Substring(0, 23), "yyyy-MM-dd_HH-mm-SS.fff", CultureInfo.InvariantCulture);
            }
        }
        static DirectoryInfo GetDirectory(DateTime _dt)
        {
            return DestDir.CreateSubdirectory(_dt.ToString(@"YYYY\MM\"));
        }

        public static FileInfo CompressLossy(FileInfo fits)
        {
            var data = FitsReader.ReadFloatFits(fits);

            data.lines.Sort((a,b) => a.points[0].X.CompareTo(b.points[0].X));

            data.SubsampleByAngle(3.0);

            RecursiveLinearPredictor.ForwardPrediction(data);

            var fitsCompressed = new FileInfo(fits.Directory.FullName + "\\v1.fits");
            OutputWriter.WriteFits(data, fitsCompressed);

            var rarFits = new FileInfo(fits.FullName + ".rar");
            OutputWriter.EncodeRAR(rarFits, fitsCompressed);
            return rarFits;
        }
    }
}
