using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace AUTAUnitTests
{
    internal class TestFile
    {
        public TestFile(string lStartingContents, string lExpectedContents = "", string lFiles = "./Test.h")
        {
            mStartingContents = lStartingContents;
            mExpectedContents = lExpectedContents;
            mFilePath = lFiles;
        }

        ~TestFile()
        {
        }

        public void CreateFile()
        {
            if (File.Exists(mFilePath)) File.Delete(mFilePath);
            using (FileStream fs = File.Create(mFilePath))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(mStartingContents);
                fs.Write(info, 0, info.Length);
            }
        }

        public bool CheckFile()
        {
            using (StreamReader sr = File.OpenText(mFilePath))
            {
                var result = Regex.Split(mExpectedContents, "\r\n|\r|\n");
                string lCurrentLine = "";
                int lLine = 0;
                while ((lCurrentLine = sr.ReadLine()) != null)
                {
                    if (result[lLine++] != lCurrentLine)
                    {
                        return false;
                    }
                }

                return lLine == result.Length;
            }
        }

        public string GetFileName()
        {
            return mFilePath;
        }

        public void CleanupFile()
        {
            if (File.Exists(mFilePath)) File.Delete(mFilePath);
        }

        public static void RunTest(string lTestName, TestFile lFiles, int lErrorCodeExpected = 0)
        {
            List<TestFile> lFileList = new List<TestFile>();
            lFileList.Add(lFiles);
            RunTest(lTestName, lFileList, lErrorCodeExpected);
        }

        public static void RunTest(string lTestName, List<TestFile> lFiles, int lErrorCodeExpected = 0)
        {
            TestsRun++;
            foreach (var lFile in lFiles) lFile.CreateFile();

            Console.WriteLine("Starting test :" + lTestName);
            Process lProcess = Process.Start("AUTA.exe");
            lProcess.WaitForExit();
            if (lProcess.ExitCode != lErrorCodeExpected)
            {
                Console.WriteLine("FAILED: AUTA returned an error code of " + lProcess.ExitCode.ToString() + " when an error code  of " + lErrorCodeExpected.ToString() + " was expected.");
                TestsFailed++;
            }
            else
            {
                if (lErrorCodeExpected == 0)
                {
                    bool lAnyFailed = false;
                    foreach (var lFile in lFiles)
                    {
                        if (!lFile.CheckFile())
                        {
                            Console.WriteLine("FAILED: AUTA output failed for file: " + lFile.GetFileName());
                            lAnyFailed = true;
                        }
                    }
                    if (lAnyFailed) TestsFailed++;
                }
            }
            foreach (var lFile in lFiles) lFile.CleanupFile();
        }

        public static int OutputResults()
        {
            Console.WriteLine(TestsRun.ToString() + " tests Run");
            Console.WriteLine(TestsFailed.ToString() + " tests Failed");
            if (TestsFailed > 0) Console.WriteLine("FAILED");
            else Console.WriteLine("SUCCESS");
            var lResult = Console.ReadKey();
            return TestsFailed;
        }

        private string mStartingContents;
        private string mExpectedContents;
        private string mFilePath;

        private static int TestsRun = 0;
        private static int TestsFailed = 0;
    };
}