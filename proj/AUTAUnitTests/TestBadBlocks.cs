using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AUTAUnitTests.TestFileFunctions;

namespace AUTAUnitTests
{
    internal class TestBadBlocks
    {
        private static string MissingEndStatementInput = @"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group2
*/

//#AUTA import Group1
//#AUTA end Group1
";

        private static string NoMatchingGroup = @"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group2
//#AUTA end Group2
";

        public static bool RunTest()
        {
            bool lResult = true;
            lResult = lResult && TestFileFunctions.RunAUTA("Missing End Statement", MissingEndStatementInput, 1);
            lResult = lResult && TestFileFunctions.RunAUTA("Reading No Matching Group", NoMatchingGroup, 1);

            return lResult;
        }
    }
}