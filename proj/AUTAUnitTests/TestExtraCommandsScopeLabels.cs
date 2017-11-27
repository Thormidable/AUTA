using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AUTAUnitTests.TestFile;

namespace AUTAUnitTests
{
    internal class TestExtraCommandsScopeLabels
    {
        private static TestFile TestScopeBlockLabelsCreate = new TestFile(@"/*
#AUTA scopeblock EACH_UINT_TYPE
#AUTA labels T
#AUTA label UInt32
#AUTA label UInt64
#AUTA end labels
#AUTA end EACH_UINT_TYPE */");

        private static TestFile TestScopeBlockLabelsCombine = new TestFile(@"
/* #AUTA scopeblock EACH_UINT_TYPE
#AUTA labels T
#AUTA label UInt32
#AUTA label UInt64
#AUTA end labels
#AUTA end EACH_UINT_TYPE */

/* #AUTA scopeblock EACH_INT_TYPE
#AUTA labels T
#AUTA label Int32
#AUTA label Int64
#AUTA end labels
#AUTA end EACH_INT_TYPE*/

/* #AUTA scopeblock EACH_INTEGER_TYPE
#AUTA importscopeblock EACH_INT_TYPE
#AUTA importscopeblock EACH_UINT_TYPE
#AUTA end EACH_INTEGER_TYPE*/
/*");

        private static TestFile TestScopeBlockLabelsUse = new TestFile(@"/*
#AUTA scopeblock EACH_UINT_TYPE
#AUTA labels T
#AUTA label UInt32
#AUTA label UInt64
#AUTA end labels
#AUTA end EACH_UINT_TYPE */
/*
#AUTA export Group1
TypeProxyOf<Object<T>>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1 scope = EACH_UINT_TYPE
//#AUTA end Group1",
@"/*
#AUTA scopeblock EACH_UINT_TYPE
#AUTA labels T
#AUTA label UInt32
#AUTA label UInt64
#AUTA end labels
#AUTA end EACH_UINT_TYPE */
/*
#AUTA export Group1
TypeProxyOf<Object<T>>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1 scope = EACH_UINT_TYPE
TypeProxyOf<Object<UInt32>>::GetTypeProxy();
TypeProxyOf<Object<UInt64>>::GetTypeProxy();
//#AUTA end Group1");

        private static TestFile TestScopeBlockMultiLabelsUse = new TestFile(@"/*
#AUTA scopeblock EACH_UINT_TYPE
#AUTA labels T, OP_NAME
#AUTA label UInt32, ThirtyTwo
#AUTA label UInt64, SixtyFour
#AUTA end labels
#AUTA end EACH_UINT_TYPE */
/*
#AUTA export Group1
TypeProxyOf<Object<T,'OP_NAME'>>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1 scope = EACH_UINT_TYPE
//#AUTA end Group1",
@"/*
#AUTA scopeblock EACH_UINT_TYPE
#AUTA labels T, OP_NAME
#AUTA label UInt32, ThirtyTwo
#AUTA label UInt64, SixtyFour
#AUTA end labels
#AUTA end EACH_UINT_TYPE */
/*
#AUTA export Group1
TypeProxyOf<Object<T,'OP_NAME'>>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1 scope = EACH_UINT_TYPE
TypeProxyOf<Object<UInt32,'ThirtyTwo'>>::GetTypeProxy();
TypeProxyOf<Object<UInt64,'SixtyFour'>>::GetTypeProxy();
//#AUTA end Group1");

        private static TestFile TestScopeBlockInvalidScopeBlock = new TestFile(@"/*
#AUTA scopeblock EACH_UINT_TYPE
#AUTA labels T
#AUTA label UInt32
#AUTA label UInt64
#AUTA end labels
#AUTA end EACH_UINT_TYPE */
/*
#AUTA export Group1
TypeProxyOf<Object<T>>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1 scope = EACH_INT_TYPE
//#AUTA end Group1");

        private static TestFile TestScopeBlockLabelsUseCombined = new TestFile(
@"/*#AUTA scopeblock EACH_UINT_TYPE
#AUTA labels T
#AUTA label UInt32
#AUTA label UInt64
#AUTA end labels
#AUTA end EACH_UINT_TYPE */

/*#AUTA scopeblock EACH_INT_TYPE
#AUTA labels T
#AUTA label Int32
#AUTA label Int64
#AUTA end labels
#AUTA end EACH_INT_TYPE*/

/*#AUTA scopeblock EACH_INTEGER_TYPE
#AUTA importscopeblock EACH_INT_TYPE
#AUTA importscopeblock EACH_UINT_TYPE
#AUTA end EACH_INTEGER_TYPE*/
/*
#AUTA export Group1
TypeProxyOf<Object<T>>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1 scope = EACH_INTEGER_TYPE
//#AUTA end Group1",
@"/*#AUTA scopeblock EACH_UINT_TYPE
#AUTA labels T
#AUTA label UInt32
#AUTA label UInt64
#AUTA end labels
#AUTA end EACH_UINT_TYPE */

/*#AUTA scopeblock EACH_INT_TYPE
#AUTA labels T
#AUTA label Int32
#AUTA label Int64
#AUTA end labels
#AUTA end EACH_INT_TYPE*/

/*#AUTA scopeblock EACH_INTEGER_TYPE
#AUTA importscopeblock EACH_INT_TYPE
#AUTA importscopeblock EACH_UINT_TYPE
#AUTA end EACH_INTEGER_TYPE*/
/*
#AUTA export Group1
TypeProxyOf<Object<T>>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1 scope = EACH_INTEGER_TYPE
TypeProxyOf<Object<Int32>>::GetTypeProxy();
TypeProxyOf<Object<Int64>>::GetTypeProxy();
TypeProxyOf<Object<UInt32>>::GetTypeProxy();
TypeProxyOf<Object<UInt64>>::GetTypeProxy();
//#AUTA end Group1");

        private static TestFile TestScopeBlockLabelsUseCombinedAcrossFiles1 = new TestFile(
@"/*
#AUTA scopeblock EACH_UINT_TYPE
#AUTA labels T
#AUTA label UInt32
#AUTA label UInt64
#AUTA end labels
#AUTA end EACH_UINT_TYPE */

/*#AUTA scopeblock EACH_INT_TYPE
#AUTA labels T
#AUTA label Int32
#AUTA label Int64
#AUTA end labels
#AUTA end EACH_INT_TYPE*/", "", "./TestScopeAcrossFiles.h");

        private static TestFile TestScopeBlockLabelsUseCombinedAcrossFiles2 = new TestFile(
@"/*
#AUTA scopeblock EACH_INTEGER_TYPE
#AUTA importscopeblock EACH_INT_TYPE
#AUTA importscopeblock EACH_UINT_TYPE
#AUTA end EACH_INTEGER_TYPE*/
/*
#AUTA export Group1
TypeProxyOf<Object<T>>::GetTypeProxy();
#AUTA end Group1
*/", "", "./TestScopeAcrossFiles2.h");

        private static TestFile TestScopeBlockLabelsUseCombinedAcrossFiles3 = new TestFile(
        @"
//#AUTA import Group1 scope = EACH_INTEGER_TYPE
//#AUTA end Group1",
        @"
//#AUTA import Group1 scope = EACH_INTEGER_TYPE
TypeProxyOf<Object<Int32>>::GetTypeProxy();
TypeProxyOf<Object<Int64>>::GetTypeProxy();
TypeProxyOf<Object<UInt32>>::GetTypeProxy();
TypeProxyOf<Object<UInt64>>::GetTypeProxy();
//#AUTA end Group1", "./TestScopeAcrossFiles.cpp");

        public static void RunTest()
        {
            TestFile.RunTest("Test Create Scope Block Labels", TestScopeBlockLabelsCreate);
            TestFile.RunTest("Test Create Scope block combining scope blocks Labels", TestScopeBlockLabelsCombine);
            TestFile.RunTest("Test use Scope Block Labels", TestScopeBlockLabelsUse);
            TestFile.RunTest("Test use Scope Block Multi Labels", TestScopeBlockMultiLabelsUse);
            TestFile.RunTest("Test use Combined Scope Blocks Labels", TestScopeBlockLabelsUseCombined);
            TestFile.RunTest("Test use undeclared scope block Labels", TestScopeBlockInvalidScopeBlock, 1);

            List<TestFile> lList = new List<TestFile>() { TestScopeBlockLabelsUseCombinedAcrossFiles1, TestScopeBlockLabelsUseCombinedAcrossFiles2, TestScopeBlockLabelsUseCombinedAcrossFiles3 };
            TestFile.RunTest("Test use Scope Block Labels Across Files", lList);
        }
    }
}