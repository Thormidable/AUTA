using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AUTA
{
    public class TemplateTypes : CommandInterface
    {
        public TemplateTypes()
        {
            templateType = new Dictionary<string, List<String>>();
            blackList = new BlacklistPlugin();
        }

        public override string GetAcceptedCommands()
        {
            return TypeSingleStringRegex();
        }

        public override void CacheBlock(CommandBlock lBlock)
        {
            foreach (var lLine in lBlock.lines)
            {
                CacheLine(lLine);
            }
        }

        protected void ExtractTypeLine(Match lMatch)
        {
            string lTypeName = lMatch.Groups[2].Value;
            if (!templateType.ContainsKey(lTypeName) && lMatch.Groups.Count > 1)
            {
                string lTypesString = lMatch.Groups[3].Value;
                var lMatches = ExtractTypes.Matches(lTypesString);
                if (lMatches.Count > 0)
                {
                    List<string> lLabels = new List<string>();
                    for (int i = 0; i < lMatches.Count; ++i)
                    {
                        string lNewType = lMatches[i].Groups[0].Value;
                        lNewType = lNewType.Trim();
                        lLabels.Add(lNewType);
                    }
                    templateType[lTypeName] = lLabels;
                }
                else
                {
                    mError = true;
                    Console.WriteLine("ERROR: Unable to extract types from type command  :" + lMatch.Groups[0]);
                }
            }
        }
        public void CacheLine(string lLine)
        {
            Match lMatch = TypeSingleLine.Match(lLine);
            if (lMatch.Success)
            {
                string lCommand = lMatch.Groups[1].Value.ToLower();
                switch (lCommand)
                {
                    case "type":
                        ExtractTypeLine(lMatch);
                        break;
                    case "blacklisttypes":
                        blackList.CacheLine(lLine);
                        break;
                    default:
                        mError = true;
                        Console.WriteLine("ERROR: Unrecognised Command passed to Types : " + lCommand);
                        break;
                }

            }
        }

        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            return null;
        }

        public static string TypeSingleStringRegex()
        {
            return DefaultAUTABegin() + "(type|blacklisttypes)\\s+([a-zA-Z_][a-zA-Z0-9_]*)\\s+(.+)";
        }

        public static string ExtractTypeRegex()
        {
            return "((?:[a-zA-Z0-9_:]+)(?:\\s*" + TemplateInstantiationPlugin.TemplateBodyRegex() + "?\\s*" + TemplateInstantiationPlugin.ParameterBodyRegex() + "?))+";
        }

        public void GenerateBespokeIterator(string lLine)
        {
            int keyCount = 0;
            foreach (var lSet in templateType)
            {
                Regex keyRegex = lKeyRegexes[keyCount];
                if (keyRegex.IsMatch(lLine)) { lBespokeIterationMaxValue[keyCount] = lSet.Value.Count; }
                else lBespokeIterationMaxValue[keyCount] = 0;
                keyCount++;
            }
        }


        public List<string> DuplicateForTypes(List<string> lInput)
        {
            if (templateType.Count > 0)
            {                
                List<string> lResult = new List<string>();
                foreach (var lLine in lInput)
                {
                    for (int i = 0; i < templateType.Count; ++i) lTypeValue[i] = 0;

                    GenerateBespokeIterator(lLine);

                    bool lContinue = true;
                    while (lContinue)
                    {
                        string lProcessed = lLine;
                        int keyCount = 0;

                        if (!CheckIfBlackListed(lTypeValue))
                        {
                            foreach (var lSet in templateType)
                            {
                                Regex keyRegex = lKeyRegexes[keyCount];
                                lProcessed = keyRegex.Replace(lProcessed, "$1 " + lSet.Value[lTypeValue[keyCount]] + " $2");
                                keyCount++;
                            }
                            lResult.Add(lProcessed);
                        }

                        lContinue = IncrementIterator(lBespokeIterationMaxValue);
                        
                    }
                }
                return lResult;
            }
            else
            {
                return new List<string>(lInput);
            }
        }

        protected bool CheckIfBlackListed(int[] lLabelCombo)
        {
            Dictionary<string, string> lPairs = new Dictionary<string, string>();
            int keyCount = 0;
            foreach (var lSet in templateType)
            {
                lPairs[lSet.Key] = lSet.Value[lLabelCombo[keyCount]];
                keyCount++;
            }
            return blackList.IsBlacklisted(lPairs);
        }

        public bool IncrementIterator(int[] lIterationMaxValues)
        {
            bool lContinue = true;
            int lTempType = 0;
            while (++lTypeValue[lTempType] >= lIterationMaxValues[lTempType])
            {
                lTypeValue[lTempType] = 0;
                ++lTempType;
                if (lTempType == templateType.Count)
                {
                    lContinue = false;
                    break;
                }
            }
            return lContinue;
        }

        public List<string> DuplicateForAllTypes(List<string> lInput)
        {
            if (templateType.Count > 0)
            {
                for (int i = 0; i < templateType.Count; ++i) lTypeValue[i] = 0;

                List<string> lResult = new List<string>();

                bool lContinue = true;
                while (lContinue)
                {
                    foreach (var lLine in lInput)
                    {
                        int keyCount = 0;
                        string lProcessed = lLine;

                        if (!CheckIfBlackListed(lTypeValue))
                        {
                            foreach (var lSet in templateType)
                            {
                                Regex keyRegex = lKeyRegexes[keyCount];
                                lProcessed = keyRegex.Replace(lProcessed, "$1 " + lSet.Value[lTypeValue[keyCount]] + " $2");
                                keyCount++;
                            }
                            lResult.Add(lProcessed);
                        }
                    }

                    lContinue = IncrementIterator(lIterationMaxValue);

                }
                return lResult;
            }
            else
            {
                return new List<string>(lInput);
            }
        }

        protected void GenerateTypeKeys()
        {
            lTypekeys = new List<string>();
            foreach (var lSet in templateType)
            {
                lTypekeys.Add(lSet.Key);
            }
        }

        protected void GenerateKeyRegexes()
        {
            lKeyRegexes = new List<Regex>();
            foreach (var lSet in templateType)
            {
                lKeyRegexes.Add(new Regex("(^|[^a-zA-Z0-9_])" + lSet.Key + "([^a-zA-Z0-9_])"));
            }

        }

        protected void GenerateIteratorArray()
        {
            lIterationMaxValue = new int[templateType.Count];
            int keyCount = 0;
            foreach (var lSet in templateType)
            {
                Regex keyRegex = lKeyRegexes[keyCount];
                lIterationMaxValue[keyCount] = lSet.Value.Count;
                keyCount++;
            }
        }

        public override void CacheForProcessing()
        {
            GenerateKeyRegexes();
            GenerateTypeKeys();

            lTypeValue = new int[templateType.Count];
            lBespokeIterationMaxValue = new int[templateType.Count];

            GenerateIteratorArray();

        }

        public override void Clear()
        {
            base.Clear();
            templateType.Clear();
            blackList.Clear();
        }

        protected BlacklistPlugin blackList;
        public IDictionary<string, List<string>> templateType;

        static Regex TypeSingleLine = new Regex(TypeSingleStringRegex());
        static Regex ExtractTypes = new Regex(ExtractTypeRegex());

        protected List<string> lTypekeys;
        protected List<string> lResult;
        protected int[] lIterationMaxValue;
        protected int[] lBespokeIterationMaxValue;
        protected int[] lTypeValue;

        List<Regex> lKeyRegexes;
    }

}
