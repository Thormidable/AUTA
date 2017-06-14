using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AUTA
{
    public class BlacklistPlugin : CommandInterface
    {
        public BlacklistPlugin()
        {
            blackList = new List<Dictionary<string, string>>();
            ExtractBlackListPairsRegex = new Regex("([a-zA-Z0-9_]+)\\s*==\\s*([^,\\s]+)\\s*");
        }

        public override string GetAcceptedCommands()
        {
            return DefaultAUTABegin() + "blacklist[^=]==[^,],";
        }

        public override void CacheBlock(CommandBlock lBlock)
        {
            foreach (var lLine in lBlock.lines)
            {
                CacheLine(lLine);
            }
        }

        public void CacheLine(string lBlackListString)
        {
            var lMatches = ExtractBlackListPairsRegex.Matches(lBlackListString);
            if (lMatches.Count > 0)
            {
                var lDictionary = new Dictionary<string, string>();
                foreach (Match lMatch in lMatches)
                {
                    if (!lDictionary.ContainsKey(lMatch.Groups[1].Value))
                    {
                        lDictionary[lMatch.Groups[1].Value.Trim()] = lMatch.Groups[2].Value.Trim();
                    }
                    else
                    {
                        mError = true;
                        return;
                    }
                    blackList.Add(lDictionary);
                }
            }
        }

        public bool IsBlacklisted(Dictionary<string, string> lPairs)
        {
            foreach (var blackListItem in blackList)
            {
                bool lFound = true;
                foreach (var lPair in lPairs)
                {
                    if (blackListItem.ContainsKey(lPair.Key))
                    {
                        if (blackListItem[lPair.Key] != lPair.Value)
                        {
                            lFound = false;
                            break;
                        }
                    }
                }
                if (lFound == true) return true;
            }
            return false;
        }

        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            return null;
        }

        public override void CacheForProcessing()
        {
        }

        public override void Clear()
        {
            base.Clear();
            blackList.Clear();
        }

        protected List<Dictionary<string, string>> blackList;
        private Regex ExtractBlackListPairsRegex;
    }
}