using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressionTesting.PFSS;

namespace CompressionTesting.Solutions
{
    interface ISolution
    {

        int GetQualityLevels();

        string GetName();

        TestResult DoTestRun(PFSSData data, int qualityLevel, string folder);

        void SelectIntermediateSolution(int i);

    }
}
