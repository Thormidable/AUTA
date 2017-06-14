using System;
using System.Collections.Generic;

namespace AUTA
{
    public class ImportExportPlugin : CommandInterface
    {
        public IDictionary<string, List<string>> mExports;

        public ImportExportPlugin()
        {
            mExports = new Dictionary<string, List<string>>();
            mError = false;
            importBlocks = 0;
            exportBlocks = 0;
        }

        public override string GetAcceptedCommands()
        {
            return DefaultAUTABegin() + "(import|export)\\s+(?:[a-zA-Z-0-9_]+)";
        }

        public override void Clear()
        {
            importBlocks = 0;
            exportBlocks = 0;
            mError = false;
            mExports.Clear();
            mProcessPaths.Clear();
        }

        public override void CacheBlock(CommandBlock lBlock)
        {
            string lCommand = lBlock.elements[0].ToLower();
            if (lCommand == "export")
            {
                ++exportBlocks;
                string lExportIdent = lBlock.elements[1];
                if (Program.IsValidIdent(lExportIdent))
                {
                    if (!mExports.ContainsKey(lExportIdent))
                    {
                        mExports[lExportIdent] = lBlock.lines;
                    }
                    else
                    {
                        Console.WriteLine("ERROR: AUTA ident already exists: " + lExportIdent);
                        mError = true;
                    }
                }
            }
            else if (lCommand == "import")
            {
                mProcessPaths.Add(lBlock.filePath);
            }
        }

        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            if (lBlock.elements[0].ToLower() == "import")
            {
                ++importBlocks;
                string lExportIdent = lBlock.elements[1];
                if (mExports.ContainsKey(lExportIdent))
                {
                    var lContents = mExports[lExportIdent].GetRange(1, mExports[lExportIdent].Count - 2);
                    return Program.ProcessExtraCommands(lBlock.lines[0], lContents);
                }
                else
                {
                    mError = true;
                    Console.WriteLine("ERROR: AUTA ident doesn't exist: " + lExportIdent);
                    return null;
                }
            }
            return null;
        }

        public override void OutputResults()
        {
            Console.WriteLine("Imported " + importBlocks + " import blocks");
            Console.WriteLine("Exported " + exportBlocks + " export blocks");
        }

        private int importBlocks;
        private int exportBlocks;
    }
}