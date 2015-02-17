using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CompressedPFSSManager.PFSS
{
    
    class PFSSData
    {
        internal double b0 { get; private set; }
        internal double l0 { get; private set; }

        internal List<PFSSLine> lines { get; private set; }

        public PFSSData(double l0, double b0, List<PFSSLine> lines)
        {
            this.b0 = b0;
            this.l0 = l0;
            this.lines = lines;
        }
    }
}
