using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AUTAUnitTests.TestFile;

namespace AUTAUnitTests
{
    internal class TestExtraCommandsFlags
    {
        private static TestFile TestFlagBlockCreate = new TestFile(@"/*
#AUTA flagblock IncludeVisual
true
#AUTA end IncludeVisual*/");

        private static TestFile TestFlagBlockTrue = new TestFile(@"/*
#AUTA flagblock IncludeVisual
true
#AUTA end IncludeVisual*/

/*
#AUTA export Group1
TypeProxyOf<Object1>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1  flags = IncludeVisual
//#AUTA end Group1",
@"/*
#AUTA flagblock IncludeVisual
true
#AUTA end IncludeVisual*/

/*
#AUTA export Group1
TypeProxyOf<Object1>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1  flags = IncludeVisual
TypeProxyOf<Object1>::GetTypeProxy();
//#AUTA end Group1");

        private static TestFile TestFlagBlockFalse = new TestFile(@"/*
#AUTA flagblock IncludeVisual
false
#AUTA end IncludeVisual*/
/*
#AUTA export Group1
TypeProxyOf<Object1>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1  flags = IncludeVisual
Stuff Stuff Stuff
//#AUTA end Group1",
 @"/*
#AUTA flagblock IncludeVisual
false
#AUTA end IncludeVisual*/
/*
#AUTA export Group1
TypeProxyOf<Object1>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1  flags = IncludeVisual
//#AUTA end Group1");

        private static TestFile TestFlagBlockInvalid = new TestFile(@"/*
#AUTA flagblock IncludeVisual
other
#AUTA end IncludeVisual*/");

        private static TestFile TestFlagBlockTrueAcrossFiles1 = new TestFile(@"/*
#AUTA flagblock IncludeVisual
true
#AUTA end IncludeVisual*/
/*
#AUTA export Group1
TypeProxyOf<Object1>::GetTypeProxy();
#AUTA end Group1
*/", "", "./TestFlagBlock.h");

        private static TestFile TestFlagBlockTrueAcrossFiles2 = new TestFile(
@"//#AUTA import Group1  flags = IncludeVisual
//#AUTA end Group1",
@"//#AUTA import Group1  flags = IncludeVisual
TypeProxyOf<Object1>::GetTypeProxy();
//#AUTA end Group1", "./TestFlagBlock2.h");

        public static void RunTest()
        {
            TestFile.RunTest("Test Create Flag Block", TestFlagBlockCreate);
            TestFile.RunTest("Test use Flag Block true", TestFlagBlockTrue);
            TestFile.RunTest("Test use Flag Block false", TestFlagBlockFalse);
            TestFile.RunTest("Error Case : Invalid Flag block", TestFlagBlockInvalid, 1);

            List<TestFile> lList = new List<TestFile>() { TestFlagBlockTrueAcrossFiles1, TestFlagBlockTrueAcrossFiles2 };
            TestFile.RunTest("Test use Flag Block Across Files", lList);
        }
    }
}