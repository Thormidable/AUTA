using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AUTA
{
    public class FlagBlock : CommandInterface
    {
        protected FlagBlock()
        {
            mFlags = new Dictionary<string, FlagBlockBody>();
            lExtraCommands = new Regex("\\s+flags\\s*=\\s*([a-zA-Z0-9_]+)");
            flagBlocks = 0;
        }

        ~FlagBlock()
        {
            instance = null;
        }

        public static FlagBlock GetInstance()
        {
            if (instance == null) instance = new FlagBlock();
            return instance;
        }

        public override string GetAcceptedCommands()
        {
            return DefaultAUTABegin() + "flagblock\\s+([a-zA-Z0-9_]+)";
        }

        public override bool IsAcceptedExtraCommands(string lLine)
        {
            return lExtraCommands.IsMatch(lLine);
        }

        public override string[] ProcessExtraCommands(string lCommandLine, List<string> lOutput)
        {
            var lScope = GetScope(lCommandLine);
            if (lScope != null) return lScope.ProcessBlock(lOutput);
            return lOutput.ToArray();
        }

        public override void CacheBlock(CommandBlock lBlock)
        {
            string lCommand = lBlock.elements[0].ToLower();
            if (lCommand == "flagblock")
            {
                flagBlocks++;
                string lExportIdent = lBlock.elements[1];
                if (Program.IsValidIdent(lExportIdent))
                {
                    if (!mFlags.ContainsKey(lExportIdent))
                    {
                        mFlags[lExportIdent] = new FlagBlockBody(lBlock);
                    }
                    else
                    {
                        Console.WriteLine("ERROR: AUTA ident already exists: " + lExportIdent);
                        mError = true;
                    }
                }
            }
        }

        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            return null;
        }

        public FlagBlockBody GetScope(string lScopeCommand)
        {
            var lMatch = lExtraCommands.Match(lScopeCommand);
            return GetScopeFromTag(lMatch.Groups[1].Value);
        }

        public FlagBlockBody GetScopeFromTag(string lKey)
        {
            if (mFlags.ContainsKey(lKey)) return mFlags[lKey];
            else
            {
                mError = true;
                Console.WriteLine("ERROR: Flag not Found :" + lKey);
                return null;
            }
        }

        public override void CacheForProcessing()
        {
            foreach (var function in mFlags)
            {
                function.Value.CacheForProcessing();
            }
        }

        public override void Clear()
        {
            mFlags.Clear();
            flagBlocks = 0;
        }

        public override bool CheckErrors()
        {
            if (mError == false) mError = mFlags.Any(x => x.Value.CheckErrors());
            return mError;
        }

        public override void OutputResults()
        {
            Console.WriteLine("Imported " + flagBlocks + " scope blocks");
        }

        private Dictionary<string, FlagBlockBody> mFlags;

        private int flagBlocks;

        private Regex lExtraCommands;
        private static FlagBlock instance = null;
    }
}