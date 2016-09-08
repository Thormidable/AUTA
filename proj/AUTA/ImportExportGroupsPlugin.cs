using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AUTA
{
    public class ImportExportGroupsPlugin : CommandInterface
    {
        public IDictionary<string, List<string>> mExports;

        public ImportExportGroupsPlugin()
        {
            mExports = new Dictionary<string, List<string>>();
            mError = false;
            importBlocks = 0;
            exportBlocks = 0;
        }

        public override string GetAcceptedCommands()
        {
            return DefaultAUTABegin() + "(importgroup|exportgroup)\\s+(?:[a-zA-Z-0-9_]+)";
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
            if (lCommand == "exportgroup")
            {
                ++exportBlocks;
                string lExportIdent = lBlock.elements[1];
                if (Program.IsValidIdent(lExportIdent))
                {
                    if (lBlock.lines.Count > 2)
                    {
                        if (!mExports.ContainsKey(lExportIdent))
                        {
                            mExports[lExportIdent] = lBlock.lines.GetRange(1, lBlock.lines.Count - 2);
                        }
                        else
                        {
                            mExports[lExportIdent].AddRange(lBlock.lines.GetRange(1, lBlock.lines.Count - 2));
                        }
                    }
                    else
                    {
                        mError = true;
                        Console.WriteLine("ERROR: AUTA Block doesn't contain sufficient lines: " + lExportIdent);
                    }
                }
            }
            else if(lCommand == "importgroup")
            {
                mProcessPaths.Add(lBlock.filePath);
            }
        }
        
        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            if (lBlock.elements[0].ToLower() == "importgroup")
            {
                ++importBlocks;
                string lExportIdent = lBlock.elements[1];
                if (mExports.ContainsKey(lExportIdent))
                {
                    return Program.ProcessExtraCommands(lBlock.lines[0], mExports[lExportIdent]);
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
            Console.WriteLine("Imported "+importBlocks+ " importgroup blocks");
            Console.WriteLine("Exported "+exportBlocks+ " exportgroup blocks");
        }

        int importBlocks;
        int exportBlocks;
    }
}
