using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace CompressedPFSSManager
{
    class RarCompression
    {
        public static void DoRar(FileInfo archivePath, FileInfo fitsPath)
        {
            Process pProcess = new Process();
            pProcess.StartInfo.FileName = "rar";
            pProcess.StartInfo.Arguments = "a \""+archivePath.FullName+"\" \""+fitsPath.FullName+"\"";

            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.WorkingDirectory = archivePath.Directory.FullName;
            pProcess.Start();
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            pProcess.WaitForExit();
        }
    }
}
