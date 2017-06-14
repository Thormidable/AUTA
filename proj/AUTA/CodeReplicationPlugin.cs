using System;
using System.Collections.Generic;
using System.Linq;

namespace AUTA
{
    public class CodeReplicationPlugin : CommandInterface
    {
        public CodeReplicationPlugin()
        {
            mTemplates = new Dictionary<string, CodeReplicationBlockPlugin>();
            mError = false;
            replicationExportBlocks = 0;
            replicationImportBlocks = 0;
        }

        public override string GetAcceptedCommands()
        {
            return DefaultAUTABegin() + "(codeblock|replicate)\\s*(?:[a-zA-Z-0-9_]+)";
        }

        public override void Clear()
        {
            replicationExportBlocks = 0;
            replicationImportBlocks = 0;
            mError = false;
            mTemplates.Clear();
            mProcessPaths.Clear();
        }

        public override void CacheBlock(CommandBlock lBlock)
        {
            string lCommand = lBlock.elements[0].ToLower();
            if (lCommand == "codeblock")
            {
                replicationExportBlocks++;
                string lExportIdent = lBlock.elements[1];
                if (Program.IsValidIdent(lExportIdent))
                {
                    if (!mTemplates.ContainsKey(lExportIdent))
                    {
                        mTemplates[lExportIdent] = new CodeReplicationBlockPlugin(lBlock);
                    }
                    else
                    {
                        Console.WriteLine("ERROR: AUTA ident already exists: " + lExportIdent);
                        mError = true;
                    }
                }
            }
            else if (lCommand == "replicate")
            {
                mProcessPaths.Add(lBlock.filePath);
            }
        }

        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            if (lBlock.elements[0].ToLower() == "replicate")
            {
                ++replicationImportBlocks;
                string lExportIdent = lBlock.elements[1];
                if (mTemplates.ContainsKey(lExportIdent))
                {
                    return Program.ProcessExtraCommands(lBlock.lines[0], mTemplates[lExportIdent].ProcessBlock(lBlock).ToList());
                }
                else
                {
                    mError = true;
                    Console.WriteLine("ERROR: AUTA ident doesn't exist: " + lExportIdent);
                }
            }
            return null;
        }

        public override void CacheForProcessing()
        {
            foreach (var template in mTemplates)
            {
                template.Value.CacheForProcessing();
            }
        }

        public override bool CheckErrors()
        {
            if (mError == false) mError = mTemplates.Any(x => x.Value.CheckErrors());
            return mError;
        }

        public override void OutputResults()
        {
            Console.WriteLine("Exported " + replicationExportBlocks + " replication blocks");
            Console.WriteLine("Imported " + replicationImportBlocks + " replication blocks");
        }

        private int replicationExportBlocks;
        private int replicationImportBlocks;

        public IDictionary<string, CodeReplicationBlockPlugin> mTemplates;
    }
}