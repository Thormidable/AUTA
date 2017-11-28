using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AUTAUnitTests.TestFile;

namespace AUTAUnitTests
{
    internal class TestExtraCommandsScopeTypes
    {
        private static TestFile TestScopeBlockTypesCreate = new TestFile(@"/*
#AUTA scopeblock TypePairs
#AUTA type R Float64, Int64
#AUTA type TA Float64 , Int64
#AUTA type TB Float64 , Int64
#AUTA end TypePairs */");

        private static TestFile TestScopeBlockTypesCombine = new TestFile(@"
/* #AUTA scopeblock TypePairs
#AUTA type R Float64, Int64
#AUTA type TA Float64 , Int64
#AUTA type TB Float64 , Int64
#AUTA end TypePairs */

/* #AUTA scopeblock EACH_INT_TYPE
#AUTA labels T
#AUTA label Int32
#AUTA label Int64
#AUTA end labels
#AUTA end EACH_INT_TYPE*/

/* #AUTA scopeblock CombinedBlock
#AUTA importscopeblock EACH_INT_TYPE
#AUTA importscopeblock TypePairs
#AUTA end CombinedBlock*/
/*");

        private static TestFile TestScopeBlockTypesUse = new TestFile(@"/*
#AUTA scopeblock TypePairs
#AUTA type R Float64, Int64
#AUTA type TA Float64 , Int64
#AUTA type TB Float64 , Int64
#AUTA end TypePairs*/
/*
#AUTA export Group1
TypeProxyOf<Object<R,TA,TB>>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1 scope = TypePairs
//#AUTA end Group1",
@"/*
#AUTA scopeblock TypePairs
#AUTA type R Float64, Int64
#AUTA type TA Float64 , Int64
#AUTA type TB Float64 , Int64
#AUTA end TypePairs*/
/*
#AUTA export Group1
TypeProxyOf<Object<R,TA,TB>>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1 scope = TypePairs
TypeProxyOf<Object<Float64,Float64,Float64>>::GetTypeProxy();
TypeProxyOf<Object<Int64,Float64,Float64>>::GetTypeProxy();
TypeProxyOf<Object<Float64,Int64,Float64>>::GetTypeProxy();
TypeProxyOf<Object<Int64,Int64,Float64>>::GetTypeProxy();
TypeProxyOf<Object<Float64,Float64,Int64>>::GetTypeProxy();
TypeProxyOf<Object<Int64,Float64,Int64>>::GetTypeProxy();
TypeProxyOf<Object<Float64,Int64,Int64>>::GetTypeProxy();
TypeProxyOf<Object<Int64,Int64,Int64>>::GetTypeProxy();
//#AUTA end Group1");

        private static TestFile TestScopeBlockTypesUseWithConcatenation = new TestFile(@"/*
#AUTA scopeblock TypePairs
#AUTA type R Float64, Int64
#AUTA type TA Float64 , Int64
#AUTA type TB Float64 , Int64
#AUTA end TypePairs*/
/*
#AUTA export Group1
TypeProxyOf<Object ## R,TA,TB ##>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1 scope = TypePairs
//#AUTA end Group1",
@"/*
#AUTA scopeblock TypePairs
#AUTA type R Float64, Int64
#AUTA type TA Float64 , Int64
#AUTA type TB Float64 , Int64
#AUTA end TypePairs*/
/*
#AUTA export Group1
TypeProxyOf<Object ## R,TA,TB ##>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1 scope = TypePairs
TypeProxyOf<ObjectFloat64,Float64,Float64>::GetTypeProxy();
TypeProxyOf<ObjectInt64,Float64,Float64>::GetTypeProxy();
TypeProxyOf<ObjectFloat64,Int64,Float64>::GetTypeProxy();
TypeProxyOf<ObjectInt64,Int64,Float64>::GetTypeProxy();
TypeProxyOf<ObjectFloat64,Float64,Int64>::GetTypeProxy();
TypeProxyOf<ObjectInt64,Float64,Int64>::GetTypeProxy();
TypeProxyOf<ObjectFloat64,Int64,Int64>::GetTypeProxy();
TypeProxyOf<ObjectInt64,Int64,Int64>::GetTypeProxy();
//#AUTA end Group1");

        private static TestFile TestScopeBlockTypesUseCombined = new TestFile(
@"/*#AUTA scopeblock TypePairs1
#AUTA type R Float64, Int64
#AUTA type TA Float64 , Int64
#AUTA end TypePairs1 */

/*#AUTA scopeblock TypePairs2
#AUTA type TB Float64, Int64
#AUTA end TypePairs2 */

/*#AUTA scopeblock AllTypePairs
#AUTA importscopeblock TypePairs1
#AUTA importscopeblock TypePairs2
#AUTA end AllTypePairs*/
/*
#AUTA export Group1
TypeProxyOf<Object<R,TA,TB>>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1 scope = AllTypePairs
//#AUTA end Group1",
@"/*#AUTA scopeblock TypePairs1
#AUTA type R Float64, Int64
#AUTA type TA Float64 , Int64
#AUTA end TypePairs1 */

/*#AUTA scopeblock TypePairs2
#AUTA type TB Float64, Int64
#AUTA end TypePairs2 */

/*#AUTA scopeblock AllTypePairs
#AUTA importscopeblock TypePairs1
#AUTA importscopeblock TypePairs2
#AUTA end AllTypePairs*/
/*
#AUTA export Group1
TypeProxyOf<Object<R,TA,TB>>::GetTypeProxy();
#AUTA end Group1
*/
//#AUTA import Group1 scope = AllTypePairs
TypeProxyOf<Object<Float64,Float64,Float64>>::GetTypeProxy();
TypeProxyOf<Object<Int64,Float64,Float64>>::GetTypeProxy();
TypeProxyOf<Object<Float64,Int64,Float64>>::GetTypeProxy();
TypeProxyOf<Object<Int64,Int64,Float64>>::GetTypeProxy();
TypeProxyOf<Object<Float64,Float64,Int64>>::GetTypeProxy();
TypeProxyOf<Object<Int64,Float64,Int64>>::GetTypeProxy();
TypeProxyOf<Object<Float64,Int64,Int64>>::GetTypeProxy();
TypeProxyOf<Object<Int64,Int64,Int64>>::GetTypeProxy();
//#AUTA end Group1");

        private static TestFile TestScopeBlockTypesUseCombinedAcrossFiles1 = new TestFile(
@"/*#AUTA scopeblock TypePairs1
#AUTA type R Float64, Int64
#AUTA type TA Float64 , Int64
#AUTA end TypePairs1 */", "", "./TestScopeAcrossFiles.h");

        private static TestFile TestScopeBlockTypesUseCombinedAcrossFiles2 = new TestFile(
@"/*
#AUTA export Group1
TypeProxyOf<Object<T>>::GetTypeProxy();
#AUTA end Group1
*/", "", "./TestScopeAcrossFiles2.h");

        private static TestFile TestScopeBlockTypesUseCombinedAcrossFiles3 = new TestFile(
        @"
//#AUTA import Group1 scope = TypePairs1
//#AUTA end Group1",
        @"
//#AUTA import Group1 scope = TypePairs1
TypeProxyOf<Object<T>>::GetTypeProxy();
TypeProxyOf<Object<T>>::GetTypeProxy();
TypeProxyOf<Object<T>>::GetTypeProxy();
TypeProxyOf<Object<T>>::GetTypeProxy();
//#AUTA end Group1", "./TestScopeAcrossFiles.cpp");

        public static void RunTest()
        {
            TestFile.RunTest("Test Create Scope Block Labels", TestScopeBlockTypesCreate);
            TestFile.RunTest("Test Create Scope block combining scope blocks Labels", TestScopeBlockTypesCombine);
            TestFile.RunTest("Test use Scope Block Labels", TestScopeBlockTypesUse);
            TestFile.RunTest("Test use Combined Scope Blocks Labels", TestScopeBlockTypesUseCombined);
            TestFile.RunTest("Test use concatenation seperate symbols", TestScopeBlockTypesUseWithConcatenation);

            List<TestFile> lList = new List<TestFile>() { TestScopeBlockTypesUseCombinedAcrossFiles1, TestScopeBlockTypesUseCombinedAcrossFiles2, TestScopeBlockTypesUseCombinedAcrossFiles3 };
            TestFile.RunTest("Test use Scope Block Labels Across Files", lList);
        }
    }
}