using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AUTA
{
    public abstract class CommandInterface
    {
        public CommandInterface()
        {
            mProcessPaths = new HashSet<string>();
        }

        public bool AcceptedBlock(CommandBlock lBlock)
        {
            return AcceptedLine(lBlock.lines[0]);
        }

        public bool AcceptedLine(string lLine)
        {
            Regex regEx = new Regex(GetAcceptedCommands());
            return regEx.IsMatch(lLine);
        }

        public static bool IsEmptyExtraCommand(string lLine)
        {
            Regex lRegEx = new Regex(DefaultAUTABegin() + "([a-zA-Z]+)\\s+([a-zA-Z0-9_]+)\\s+(.+)");
            Match lMatch = lRegEx.Match(lLine);
            return !lMatch.Success;
        }

        public virtual bool IsAcceptedExtraCommands(string lCommandLine)
        {
            return false;
        }

        public virtual string[] ProcessExtraCommands(string lCommandLine, List<string> lOutput)
        {
            return lOutput.ToArray();
        }

        public virtual void CacheForProcessing()
        {
        }

        public virtual CommandBlock ExtractAUTABlock(List<string> lines, int startIndex, string filePath)
        {
            string[] elements = ParseAUTABlockStart(lines[startIndex]);
            if (elements == null || (elements.Length > 2 && Program.IsValidIdent(elements[1])))
            {
                Console.WriteLine("ERROR: Bad AUTA block Started : " + filePath + ", line: " + startIndex.ToString() + " : " + lines[startIndex]);
                return null;
            }
            else
            {
                var lNewBlock = new CommandBlock();
                lNewBlock.lines = new List<string>();
                lNewBlock.lines.Add(lines[startIndex]);
                lNewBlock.elements = elements;
                lNewBlock.filePath = filePath;

                int lRecursionCounter = 0;

                for (int index = startIndex + 1; index < lines.Count; ++index)
                {
                    var lLine = lines[index];
                    lNewBlock.lines.Add(lLine);
                    if (ParseAUTABlockStart(lines[index]) != null) lRecursionCounter++;
                    if (ParseAUTAEnd(lLine, lNewBlock.elements[1]))
                    {
                        lRecursionCounter--;
                        if (lRecursionCounter == 0)
                        {
                            lNewBlock.endIndex = index;
                            return lNewBlock;
                        }
                    }
                }
                if (lRecursionCounter == 0) Console.WriteLine("ERROR: AUTA block never terminated: " + filePath + ", line: " + startIndex.ToString() + " : " + lines[startIndex]);
                if (lRecursionCounter > 0) Console.WriteLine("ERROR: AUTA block contained un terminated AUTA blocks: " + filePath + ", line: " + startIndex.ToString() + " : " + lines[startIndex]);
                if (lRecursionCounter < 0) Console.WriteLine("ERROR: AUTA block contained too many terminating AUTA commands: " + filePath + ", line: " + startIndex.ToString() + " : " + lines[startIndex]);
            }
            return null;
        }

        public static bool ParseAUTALine(string lLine)
        {
            Regex lRegEx = new Regex(DefaultAUTABegin() + "([a-zA-Z]+)\\s+([a-zA-Z0-9_]+)");
            Match lMatch = lRegEx.Match(lLine);
            if (lMatch.Success)
            {
                string[] lResult = new string[lMatch.Groups.Count - 1];
                for (int i = 0; i < lMatch.Groups.Count - 1; ++i)
                {
                    lResult[i] = lMatch.Groups[i + 1].Value;
                }
                return true;
            }
            return false;
        }

        public virtual bool ParseAUTAEnd(string lLine, string lTag)
        {
            Regex lRegEx = new Regex(DefaultAUTABegin() + "end\\s+" + lTag);
            return lRegEx.IsMatch(lLine);
        }

        public virtual string[] ParseAUTABlockStart(string lLine)
        {
            Regex lRegEx = new Regex(DefaultAUTABegin() + "([a-zA-Z]+)\\s+([a-zA-Z0-9_]+)");
            Match lMatch = lRegEx.Match(lLine);
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

        static public List<string> RemoveConcatination(List<string> lInput)
        {
            for (int i = 0; i < lInput.Count; ++i)
            {
                if (lConcatanateRegex.IsMatch(lInput[i]))
                {
                    lInput[i] = lConcatanateRegex.Replace(lInput[i], "${1}${2}");
                }
            }
            return lInput;
        }

        public abstract string GetAcceptedCommands();

        public abstract void CacheBlock(CommandBlock lBlock);

        public abstract string[] ProcessBlock(CommandBlock lBlock);

        public virtual void OutputResults()
        {
        }

        public virtual void Clear()
        {
            mError = false;
        }

        public virtual bool CheckErrors()
        {
            return mError;
        }

        public static string DefaultAUTABegin()
        {
            return "^\\s*(?://|/\\*)?\\s*#AUTA\\s+";
        }

        public static Regex lConcatanateRegex = new Regex("(?:([^\\\\])\\s*##\\s*)|(?:\\s*\\\\(##)\\s*)");
        public HashSet<string> mProcessPaths;
        public bool mError;
    }
}