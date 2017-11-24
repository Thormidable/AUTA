using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AUTA
{
    public class ScopeBlock : CommandInterface
    {
        protected ScopeBlock()
        {
            mScopes = new Dictionary<string, ScopeBlockBody>();
            lExtraCommands = new Regex("\\s+scope\\s*=\\s*([a-zA-Z0-9_]+)");
            scopeBlocks = 0;
        }

        ~ScopeBlock()
        {
            instance = null;
        }

        public static ScopeBlock GetInstance()
        {
            if (instance == null) instance = new ScopeBlock();
            return instance;
        }

        public override string GetAcceptedCommands()
        {
            return DefaultAUTABegin() + "scopeblock\\s+([a-zA-Z0-9_]+)";
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
            if (lCommand == "scopeblock")
            {
                scopeBlocks++;
                string lExportIdent = lBlock.elements[1];
                if (Program.IsValidIdent(lExportIdent))
                {
                    if (!mScopes.ContainsKey(lExportIdent))
                    {
                        mScopes[lExportIdent] = new ScopeBlockBody(lBlock);
                    }
                    else
                    {
                        Console.WriteLine("ERROR: AUTA ident already exists: " + lExportIdent + ". File Path: " + lBlock.filePath.ToString());
                        mError = true;
                    }
                }
            }
        }

        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            return null;
        }

        public ScopeBlockBody GetScope(string lScopeCommand)
        {
            var lMatch = lExtraCommands.Match(lScopeCommand);
            return GetScopeFromTag(lMatch.Groups[1].Value);
        }

        public ScopeBlockBody GetScopeFromTag(string lKey)
        {
            if (mScopes.ContainsKey(lKey)) return mScopes[lKey];
            else
            {
                mError = true;
                Console.WriteLine("ERROR: Scope not Found :" + lKey);
                return null;
            }
        }

        public TemplateLabels GetGlobalLabel(string lScopeCommand)
        {
            return GetScope(lScopeCommand)?.GetLabels();
        }

        public TemplateTypes GetGlobalType(string lScopeCommand)
        {
            return GetScope(lScopeCommand)?.GetTypes();
        }

        public override void CacheForProcessing()
        {
            foreach (var function in mScopes)
            {
                function.Value.CacheForProcessing();
            }
        }

        public override void Clear()
        {
            mScopes.Clear();
            scopeBlocks = 0;
        }

        public override bool CheckErrors()
        {
            if (mError == false) mError = mScopes.Any(x => x.Value.CheckErrors());
            return mError;
        }

        public override void OutputResults()
        {
            Console.WriteLine("Imported " + scopeBlocks + " scope blocks");
        }

        private Dictionary<string, ScopeBlockBody> mScopes;

        private int scopeBlocks;

        private Regex lExtraCommands;
        private static ScopeBlock instance = null;
    }
}