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
    internal class TestFileFunctions
    {
        public static void CreateFile(string lPath, string lContents)
        {
            if (File.Exists(lPath)) File.Delete(lPath);
            using (FileStream fs = File.Create(lPath))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(lContents);
                fs.Write(info, 0, info.Length);
            }
        }

        public static bool CheckFile(string lPath, string lExpectedContents)
        {
            using (StreamReader sr = File.OpenText(lPath))
            {
                var result = Regex.Split(lExpectedContents, "\r\n|\r|\n");
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

        public static bool RunTest(string lTestName, string lInput, string lExpectedOutput)
        {
            string lFileName = "./Test.h";
            return RunTest(lTestName, lInput, lExpectedOutput, lFileName);
        }

        public static bool RunTest(string lTestName, string lInput, string lExpectedOutput, string lFileName)
        {
            if (!RunAUTA(lTestName, lInput, lFileName, 0)) return false;
            if (!CheckFile(lFileName, lExpectedOutput))
            {
                Console.Write("AUTA result did not match expected:" + lInput);
                return false;
            }
            return true;
        }

        public static bool RunAUTA(string lTestName, string lInput, int lErrorCodeExpected)
        {
            string lFileName = "./Test.h";
            return RunAUTA(lTestName, lInput, lFileName, lErrorCodeExpected);
        }

        public static bool RunAUTA(string lTestName, string lInput, string lFileName, int lErrorCodeExpected)
        {
            Console.WriteLine("Starting test :" + lTestName);
            CreateFile(lFileName, lInput);
            Process lProcess = Process.Start("AUTA.exe");
            lProcess.WaitForExit();
            if (lProcess.ExitCode != lErrorCodeExpected)
            {
                Console.Write("AUTA returned an error code of " + lProcess.ExitCode.ToString() + " when an error code  of " + lErrorCodeExpected.ToString() + " was expected:" + lInput);
                return false;
            }
            return true;
        }
    }
}