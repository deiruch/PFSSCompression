using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTesting.PFSS.Test
{
    class TestSuite
    {
        internal List<TestLine> lines { get; private set; }
        internal double b0 { get; private set; }
        internal double l0 { get; private set; }

        internal string file { get; private set; }

        public TestSuite(List<TestLine> lines, double l0, double b0)
        {
            this.lines = lines;
            this.l0 = l0;
            this.b0 = b0;
        }

        public PFSSData GetData()
        {
            List<PFSSLine> newLines = new List<PFSSLine>(lines.Count);

            foreach (TestLine l in lines)
                newLines.Add(l.GetPFSSLine());
            
            return new PFSSData(b0,l0,newLines);
        }


    }
}
