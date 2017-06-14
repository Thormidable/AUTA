using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AUTA
{
    public class TemplateLabels : CommandInterface
    {
        public TemplateLabels()
        {
            templateLabelSets = new List<List<string>>();
            LabelList = new Regex(GetAcceptedCommands());
        }

        public override string GetAcceptedCommands()
        {
            return DefaultAUTABegin() + "labels\\s*(.*)";
        }

        protected List<string> ExtractLabelSymbols(CommandBlock lBlock)
        {
            var ltemplateLabels = new List<string>();
            Match lMatch = LabelList.Match(lBlock.lines[0]);
            if (lMatch.Success)
            {
                if (templateLabels != null)
                {
                    mError = true;
                    Console.WriteLine("ERROR: Template Block Already has template labels : " + lBlock.filePath + " on line : ", lBlock.lines[0]);
                    return null;
                }
                else
                {
                    var lMatches = ExtractLabels.Matches(lMatch.Groups[1].Value);
                    for (int i = 0; i < lMatches.Count; ++i)
                    {
                        ltemplateLabels.Add(lMatches[i].Groups[0].Value);
                    }
                }
            }
            else
            {
                mError = true;
                Console.WriteLine("ERROR: Failed to pass block : " + lBlock.lines[0]);
                return null;
            }
            return ltemplateLabels;
        }

        public void ExtractLabelSets(CommandBlock lBlock)
        {
            for (int l = 0; l < lBlock.lines.Count; ++l)
            {
                var lLine = lBlock.lines[l];
                if (lEndMatch.IsMatch(lLine))
                {
                    lBlock.endIndex = l;
                    return;
                }

                var lLabelMatch = lLabelTags.Match(lLine);
                if (lLabelMatch.Success)
                {
                    string lLabelSetString = lLabelMatch.Groups[1].Value;
                    var lMatches = ExtractLabelContents.Matches(lLabelSetString);
                    if (lMatches.Count > 0)
                    {
                        List<string> lLabels = new List<string>();
                        for (int i = 0; i < lMatches.Count; ++i)
                        {
                            string lTemp = lMatches[i].Groups[1].Value;
                            lTemp = lTemp.Trim();
                            lTemp = lRemoveBackslashes.Replace(lTemp, "${1}");
                            lLabels.Add(lTemp);
                        }
                        if (templateLabelSets.Count > 0 && lLabels.Count != templateLabelSets[0].Count)
                        {
                            Console.WriteLine("ERROR: Label set size do not match previous label sets sizes : " + lBlock.filePath + " on line : " + lLine);
                            mError = true;
                        }
                        templateLabelSets.Add(lLabels);
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Unable to extract labels : " + lBlock.filePath + " on line : " + lLine);
                        mError = true;
                    }
                }
            }
        }

        public override void CacheBlock(CommandBlock lBlock)
        {
            lBlock.endIndex = -1;

            templateLabels = ExtractLabelSymbols(lBlock);
            if (templateLabels == null) return;

            ExtractLabelSets(lBlock);
        }

        public string LabelListRegex()
        {
            return "([a-zA-Z_][a-zA-Z0-9_]*)";
        }

        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            return null;
        }

        public override bool ParseAUTAEnd(string lLine, string lTag)
        {
            Regex lRegex = new Regex(DefaultAUTABegin() + "end");
            return lRegex.IsMatch(lLine);
        }

        public override string[] ParseAUTABlockStart(string lLine)
        {
            Regex lRegex = new Regex(DefaultAUTABegin() + "(\\S+)\\s+(\\S+)");
            Match lMatch = lRegex.Match(lLine);
            if (lMatch.Success)
            {
                string[] lResult = new string[lMatch.Groups.Count - 1];
                for (int i = 0; i < lMatch.Groups.Count - 1; ++i)
                {
                    lResult[i] = lMatch.Groups[i + 1].Value;
                }
                return lResult;
            }
            return null;
        }

        public override void Clear()
        {
            templateLabels.Clear();
            templateLabelSets.Clear();
        }

        public List<string> DuplicateForLabels(List<string> lInput)
        {
            if (templateLabels != null && templateLabelSets != null && templateLabelSets.Count > 0)
            {
                List<string> lResult = new List<string>();
                foreach (var lSet in templateLabelSets)
                {
                    foreach (var lLine in lInput)
                    {
                        string lProcessed = lLine;
                        for (int i = 0; i < templateLabels.Count; ++i)
                        {
                            //if (lDebugRegexes[i].IsMatch(lProcessed))
                            //{
                            //    Console.WriteLine("Debug");
                            //}
                            lProcessed = lLabelRegexes[i].Replace(lProcessed, "${1}" + lSet[i] + "${2}");
                            lProcessed = lLabelRegexes[i].Replace(lProcessed, "${1}" + lSet[i] + "${2}");
                        }
                        lResult.Add(lProcessed);
                    }
                }
                return lResult;
            }
            else
            {
                return lInput;
            }
        }

        public override void CacheForProcessing()
        {
            if (templateLabels != null)
            {
                lLabelRegexes = new List<Regex>(templateLabels.Count);
                lDebugRegexes = new List<Regex>();
                foreach (var lLabel in templateLabels)
                {
                    //   lDebugRegexes.Add(new Regex(lLabel+"\\(" + lLabel));
                    lLabelRegexes.Add(new Regex("(^|[^a-zA-Z0-9_])" + lLabel + "([^a-zA-Z0-9_]|$)"));
                }
            }
        }

        public List<string> templateLabels;
        public List<List<string>> templateLabelSets;
        private Regex LabelList;

        private static Regex ExtractLabels = new Regex("[a-zA-Z_][a-zA-Z0-9_]*");
        private static Regex ExtractLabelContents = new Regex(@"((?:[^,]|\\,)*[^\\])(?:,|$)");

        private static Regex lLabelTags = new Regex(DefaultAUTABegin() + "label\\s+(.*)");
        private static Regex lEndMatch = new Regex(DefaultAUTABegin() + "end");
        private static Regex lRemoveBackslashes = new Regex(@"\\(.)");

        private List<Regex> lLabelRegexes;
        private List<Regex> lDebugRegexes;
    }
}