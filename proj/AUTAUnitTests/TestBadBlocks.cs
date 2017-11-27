using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AUTAUnitTests.TestFile;

namespace AUTAUnitTests
{
    internal class TestBadBlocks
    {
        private static TestFile lMissingEndStatement = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group2
*/

//#AUTA import Group1
//#AUTA end Group1
");

        private static TestFile NoMatchingGroup = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group2
//#AUTA end Group2
");

        private static TestFile InvalidAUTACommand = new TestFile(@"/*
#AUTA error Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1
//#AUTA end Group1
");

        private static TestFile InvalidAUTACommandImport = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA importer Group1
//#AUTA end Group1
");

        private static TestFile InvalidAUTACommandScopeMissing = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1 : scope = New
//#AUTA end Group1
");

        private static TestFile InvalidAUTACommandFlagsMissing = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1 : flags = New
//#AUTA end Group1
");

        private static TestFile InvalidAUTACommandFlagsPartial = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1 : flags
//#AUTA end Group1
");

        private static TestFile InvalidAUTAExtraCommand = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1 : InvalidCommand = Group1
//#AUTA end Group1
");

        public static void RunTest()
        {
            TestFile.RunTest("Missing End Statement", lMissingEndStatement, 1);
            TestFile.RunTest("Reading No Matching Group", NoMatchingGroup, 1);
            TestFile.RunTest("Invalid AUTA Command export", InvalidAUTACommand, 1);
            TestFile.RunTest("Invalid AUTA Command import", InvalidAUTACommandImport, 1);
            TestFile.RunTest("Invalid AUTA Command Missing scope", InvalidAUTACommandScopeMissing, 1);
            TestFile.RunTest("Invalid AUTA Command Missing flags", InvalidAUTACommandFlagsMissing, 1);
            TestFile.RunTest("Invalid AUTA Command partial flags", InvalidAUTACommandFlagsPartial, 1);
            TestFile.RunTest("Invalid AUTA Invalid Extra Command", InvalidAUTAExtraCommand, 1);
        }
    }
}