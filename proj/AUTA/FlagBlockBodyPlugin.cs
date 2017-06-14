using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AUTA
{
    public class FlagBlockBody : CommandInterface
    {
        public FlagBlockBody(CommandBlock lBlock)
        {
            mFlagValue = false;
            CacheBlock(lBlock);
        }

        public override string GetAcceptedCommands()
        {
            throw new Exception("ERRROR : FlagBlockBody should not call GetAcceptedCommands");
        }

        public override void CacheBlock(CommandBlock lBlock)
        {
            int Count = 0;
            for (int i = 1; i < lBlock.lines.Count; ++i)
            {
                string lLine = lBlock.lines[i];
                var lMatch = lFlagBlockRegex.Match(lLine.ToLower());
                if (lMatch.Success)
                {
                    if (lMatch.Captures[0].ToString() == "true")
                    {
                        Count++;
                        mFlagValue = true;
                    }
                    else
                    {
                        if (lMatch.Captures[0].ToString() == "false")
                        {
                            Count++;
                            mFlagValue = false;
                        }
                    }
                }
            }
            if (Count == 0)
            {
                mError = true;
                Console.WriteLine("ERROR: No true/false statements in FlagBlock : " + lBlock.lines[0]);
            }
            if (Count > 1)
            {
                mError = true;
                Console.WriteLine("ERROR: multiple true/false statements in FlagBlock : " + lBlock.lines[0]);
            }
        }

        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            return null;
        }

        public string[] ProcessBlock(List<string> lResult)
        {
            if (mFlagValue == true) return lResult.ToArray();
            else
            {
                return new List<string>().ToArray();
            }
        }

        public override void CacheForProcessing()
        {
        }

        public override void Clear()
        {
        }

        public override bool CheckErrors()
        {
            return mError;
        }

        private bool mFlagValue;
        private static Regex lFlagBlockRegex = new Regex("^\\s*(true|false)\\s*$");
    }
}