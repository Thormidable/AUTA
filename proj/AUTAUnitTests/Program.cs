using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AUTAUnitTests.TestExportImport;

namespace AUTAUnitTests
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            TestFile.ClearSourceFiles();

            TestExtraCommandsScopeTypes.RunTest();
            TestExportImport.RunTest();
            TestBadBlocks.RunTest();
            TestExtraCommandsFlags.RunTest();
            TestExtraCommandsScopeLabels.RunTest();

            return TestFile.OutputResults();
        }
    }
}