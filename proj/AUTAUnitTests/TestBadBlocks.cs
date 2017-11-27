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

        private static string InvalidAUTACommand = @"/*
#AUTA error Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1
//#AUTA end Group1
";

        private static string InvalidAUTACommandImport = @"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA importer Group1
//#AUTA end Group1
";

        private static string InvalidAUTACommandScopeMissing = @"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1 : scope = New
//#AUTA end Group1
";

        private static string InvalidAUTACommandFlagsMissing = @"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1 : flags = New
//#AUTA end Group1
";

        private static string InvalidAUTACommandFlagsPartial = @"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1 : flags
//#AUTA end Group1
";

        public static bool RunTest()
        {
            bool lResult = true;
            lResult = TestFileFunctions.RunAUTA("Missing End Statement", MissingEndStatementInput, 1) && lResult;
            lResult = TestFileFunctions.RunAUTA("Reading No Matching Group", NoMatchingGroup, 1) && lResult;
            lResult = TestFileFunctions.RunAUTA("Invalid AUTA Command export", InvalidAUTACommand, 1) && lResult;
            lResult = TestFileFunctions.RunAUTA("Invalid AUTA Command import", InvalidAUTACommandImport, 1) && lResult;
            lResult = TestFileFunctions.RunAUTA("Invalid AUTA Command Missing scope", InvalidAUTACommandScopeMissing, 1) && lResult;
            lResult = TestFileFunctions.RunAUTA("Invalid AUTA Command Missing flags", InvalidAUTACommandFlagsMissing, 1) && lResult;
            lResult = TestFileFunctions.RunAUTA("Invalid AUTA Command partial flags", InvalidAUTACommandFlagsPartial, 1) && lResult;

            return lResult;
        }
    }
}