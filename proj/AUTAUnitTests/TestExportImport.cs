using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AUTAUnitTests.TestFileFunctions;

namespace AUTAUnitTests
{
    internal class TestExportImport
    {
        private static string BasicInput = @"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1
//#AUTA end Group1";

        private static string BasicOutput = @"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1
TypeProxyOf < Object1 >::GetTypeProxy();
//#AUTA end Group1";

        public static bool RunTest()
        {
            bool lResult = true;
            lResult = lResult && TestFileFunctions.RunTest("Basic Import / Export", BasicInput, BasicOutput);
            return lResult;
        }
    }
}