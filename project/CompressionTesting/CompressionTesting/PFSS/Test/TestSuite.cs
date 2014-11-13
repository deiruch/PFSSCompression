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

        public TestSuite(List<TestLine> lines)
        {
            this.lines = lines;
        }

        public void SetData(PFSSData data)
        {

        }

        public PFSSData GetData()
        {
            return null;
        }

        public PFSSData GetSubsampledData(int factor)
        {
            return null;
        }

    }
}
