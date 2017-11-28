using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AUTA
{
    internal class Program
    {
        private static void AddPlugins()
        {
            mPlugins.Clear();
            mPlugins.Add(new ImportExportPlugin());
            mPlugins.Add(new TemplateInstantiationPlugin());
            mPlugins.Add(new CodeReplicationPlugin());
            mPlugins.Add(new ImportExportGroupsPlugin());
            mPlugins.Add(ScopeBlock.GetInstance());
            mPlugins.Add(FlagBlock.GetInstance());
        }

        private static void Clear()
        {
            totalFiles = 0;
            filesWithAUTABlocks = 0;
            changedFiles = 0;

            foreach (var plugin in mPlugins)
            {
                plugin.Clear();
            }
        }

        public static string[] ProcessExtraCommands(string lLine, List<string> lOutput)
        {
            if (!CommandInterface.IsEmptyExtraCommand(lLine))
            {
                bool FoundCommand = false;
                foreach (var lPlugin in mPlugins)
                {
                    if (lPlugin.IsAcceptedExtraCommands(lLine))
                    {
                        lOutput = lPlugin.ProcessExtraCommands(lLine, lOutput).ToList();
                        FoundCommand = true;
                    }
                }
                if (FoundCommand == false)
                {
                    Console.WriteLine("ERROR: Invalid Extra Command Text : " + lLine);
                    mError = true;
                }
            }
            return lOutput.ToArray();
        }

        private static string[] GetSourceFiles(string lTargetDir)
        {
            var lHeaderPaths = Directory.GetFiles(lTargetDir, "*.h", SearchOption.AllDirectories);
            var lSourcePaths = Directory.GetFiles(lTargetDir, "*.cpp", SearchOption.AllDirectories);
            var lCudaSourcePaths = Directory.GetFiles(lTargetDir, "*.cu", SearchOption.AllDirectories);

            var lPaths = new string[lHeaderPaths.Length + lSourcePaths.Length + lCudaSourcePaths.Length];
            Array.Copy(lHeaderPaths, lPaths, lHeaderPaths.Length);
            Array.Copy(lSourcePaths, 0, lPaths, lHeaderPaths.Length, lSourcePaths.Length);
            Array.Copy(lCudaSourcePaths, 0, lPaths, lHeaderPaths.Length + lSourcePaths.Length, lCudaSourcePaths.Length);

            return lPaths;
        }

        private static void CacheParse(string[] lPaths)
        {
            foreach (var lPath in lPaths)
            {
                bool foundAUTABlock = false;
                var lLines = File.ReadAllLines(lPath).ToList();
                for (int index = 0; index < lLines.Count; ++index)
                {
                    var lLine = lLines[index];
                    //Check if line is a valid AUTA command
                    if (CommandInterface.ParseAUTALine(lLine))
                    {
                        //Find and use plugin which can handle the detected line
                        var plugin = mPlugins.FirstOrDefault(x => x.AcceptedLine(lLine));
                        var lBlock = plugin?.ExtractAUTABlock(lLines, index, lPath);
                        if (lBlock == null)
                        {
                            if (plugin != null) Console.WriteLine("ERROR: No Plugin found to support Command Block : " + lPath + " : " + lLine);
                            mError = true;
                        }
                        else
                        {
                            foundAUTABlock = true;
                            index = lBlock.endIndex;
                            plugin.CacheBlock(lBlock);
                        }
                    }
                }
                if (foundAUTABlock == true) ++filesWithAUTABlocks;
            }
        }

        private static void ProcessParse()
        {
            HashSet<string> lImportPaths = new HashSet<string>();
            foreach (var plugin in mPlugins)
            {
                foreach (var lPath in plugin.mProcessPaths)
                {
                    lImportPaths.Add(lPath);
                }
            }

            foreach (var lPath in lImportPaths)
            {
                List<string> lResult = new List<string>();

                var lLines = File.ReadAllLines(lPath).ToList();
                for (int index = 0; index < lLines.Count; ++index)
                {
                    var lLine = lLines[index];
                    if (CommandInterface.ParseAUTALine(lLine))
                    {
                        //Find plugin to handle AUTA command
                        var plugin = mPlugins.FirstOrDefault(x => x.AcceptedLine(lLine));
                        var lBlock = plugin?.ExtractAUTABlock(lLines, index, lPath);
                        if (lBlock == null)
                        {
                            if (plugin != null) Console.WriteLine("ERROR: No Plugin found to support Command Block : " + lPath + " : " + lLine);
                            mError = true;
                        }
                        else
                        {
                            var lInsert = plugin.ProcessBlock(lBlock);
                            if (lInsert != null)
                            {
                                //Strip ## commands
                                lInsert = CommandInterface.RemoveConcatination(lInsert.ToList()).ToArray();
                                lResult.Add(lLines[index]);
                                lResult.AddRange(lInsert);
                                lResult.Add(lLines[lBlock.endIndex]);
                            }
                            else
                            {
                                lResult.AddRange(lBlock.lines);
                            }
                            index = lBlock.endIndex;
                        }
                    }
                    else
                    {
                        lResult.Add(lLines[index]);
                    }
                }

                CheckErrors();

                if (mError == false)
                {
                    for (int index = 0; index < lLines.Count; ++index)
                    {
                        if (lResult[index] != lLines[index])
                        {
                            Console.WriteLine("Writing to source file: " + lPath);
                            File.WriteAllLines(lPath, lResult.ToArray());
                            ++changedFiles;
                            break;
                        }
                    }
                }
            }
        }

        static public void CheckErrors()
        {
            if (mError == false)
            {
                mError = mPlugins.Any(x => x.CheckErrors());
            }
        }

        public static void CacheForProcessing()
        {
            foreach (var lPlugin in mPlugins)
            {
                lPlugin.CacheForProcessing();
            }
        }

        public static bool IsValidIdent(string lIdent)
        {
            const string lStart = @"(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl})";
            const string lExtend = @"(\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\p{Cf})";
            Regex lExp = new Regex(string.Format("{0}({0}|{1})*", lStart, lExtend));
            return lExp.IsMatch(lIdent.Normalize());
        }

        static public void OutputResults()
        {
            Console.WriteLine("Found " + totalFiles + " source files");
            Console.WriteLine("Found " + filesWithAUTABlocks + " files with AUTA blocks");
            foreach (var lPlugin in mPlugins)
            {
                lPlugin.OutputResults();
            }
            Console.WriteLine("Changed " + changedFiles + " files");
        }

        private static int Main(string[] args)
        {
            Console.WriteLine("===========================AUTA START===========================");
            string lTargetDir = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
            if (!Directory.Exists(lTargetDir))
            {
                Console.WriteLine("ERROR: Target directory does not exist: " + lTargetDir);
                return 1;
            }

            Console.WriteLine("Working in source directory: " + lTargetDir);

            var lPaths = GetSourceFiles(lTargetDir);
            totalFiles = lPaths.Length;

            AddPlugins();
            CacheParse(lPaths);
            CheckErrors();

            if (!mError)
            {
                //Process received data now to ensure all input blocks commands and objects have been inputted
                CacheForProcessing();
                ProcessParse();
                CheckErrors();
            }

            OutputResults();

            if (mError)
            {
                Console.WriteLine("One or more errors occurred whilst applying AUTA exports");
            }
            else
            {
                Console.WriteLine("DONE");
            }
            Console.WriteLine("===========================AUTA END=============================");
            return mError ? 1 : 0;
        }

        public static List<CommandInterface> GetPluginsList()
        {
            return mPlugins;
        }

        private static int totalFiles = 0;
        private static int filesWithAUTABlocks = 0;
        private static int changedFiles = 0;

        private static bool mError = false;

        private static List<CommandInterface> mPlugins = new List<CommandInterface>();
    }
}