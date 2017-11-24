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
            bool lResult = true;
            lResult = lResult && TestExportImport.RunTest();
            lResult = lResult && TestBadBlocks.RunTest();

            if (lResult)
            {
                Console.Write("Success");
                Console.ReadKey();
                return 0;
            }
            else
            {
                Console.Write("Failed");
                Console.ReadKey();
                return 1;
            }
        }
    }
}