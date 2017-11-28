using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AUTAUnitTests.TestFile;

namespace AUTAUnitTests
{
    internal class TestExportImport
    {
        private static TestFile BasicInput = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1
//#AUTA end Group1",
@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1
TypeProxyOf < Object1 >::GetTypeProxy();
//#AUTA end Group1");

        private static TestFile BasicConcatinationTest = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf ## < ## Object1 ##> ##::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1
//#AUTA end Group1",
@"/*
#AUTA export Group1
TypeProxyOf ## < ## Object1 ##> ##::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1
TypeProxyOf<Object1>::GetTypeProxy();
//#AUTA end Group1");

        private static TestFile BasicMultipleGroups = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
#AUTA export Group2
TypeProxyOf < Object2 >::GetTypeProxy();
#AUTA end Group2
*/

//#AUTA import Group2
//#AUTA end Group2
//#AUTA import Group1
//#AUTA end Group1",
@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
#AUTA export Group2
TypeProxyOf < Object2 >::GetTypeProxy();
#AUTA end Group2
*/

//#AUTA import Group2
TypeProxyOf < Object2 >::GetTypeProxy();
//#AUTA end Group2
//#AUTA import Group1
TypeProxyOf < Object1 >::GetTypeProxy();
//#AUTA end Group1");

        private static TestFile BasicRepeatedImport = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1
//#AUTA end Group1
//#AUTA import Group1
//#AUTA end Group1",
@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/

//#AUTA import Group1
TypeProxyOf < Object1 >::GetTypeProxy();
//#AUTA end Group1
//#AUTA import Group1
TypeProxyOf < Object1 >::GetTypeProxy();
//#AUTA end Group1");

        private static TestFile ErrorMultipleExport = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/");

        private static TestFile ErrorMultipleExportAcrossFiles1 = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1", "", "./TestExport1.h");

        private static TestFile ErrorMultipleExportAcrossFiles2 = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1", "", "./TestExport2.h");

        private static TestFile BasicMultiFileImportFile1 = new TestFile(@"/*
#AUTA export Group1
TypeProxyOf < Object1 >::GetTypeProxy();
#AUTA end Group1
*/", "", "./TestExport.h");

        private static TestFile BasicMultiFileImportFile2 = new TestFile(@"//#AUTA import Group1
//#AUTA end Group1",
@"//#AUTA import Group1
TypeProxyOf < Object1 >::GetTypeProxy();
//#AUTA end Group1", "./TestImport.h");

        public static void RunTest()
        {
            TestFile.RunTest("Basic Import / Export", BasicInput);
            TestFile.RunTest("Basic Import / Export Multiple Groups", BasicMultipleGroups);
            TestFile.RunTest("Basic Import / Export Multiple Imports", BasicRepeatedImport);
            TestFile.RunTest("Basic Import / Export Concatenation Test", BasicConcatinationTest);

            List<TestFile> lList = new List<TestFile>() { BasicMultiFileImportFile1, BasicMultiFileImportFile2 };
            TestFile.RunTest("Basic Import / Export Across Files", lList);
            TestFile.RunTest("Error Case : exported multiple times", ErrorMultipleExport, 1);

            List<TestFile> lList2 = new List<TestFile>() { ErrorMultipleExportAcrossFiles1, ErrorMultipleExportAcrossFiles2 };
            TestFile.RunTest("Error Case : exported multiple times across Files", lList2, 1);
        }
    }
}