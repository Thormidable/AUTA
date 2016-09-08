using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

                for (int index = startIndex + 1; index < lines.Count; ++index)
                {
                    var lLine = lines[index];
                    lNewBlock.lines.Add(lLine);
                    if (ParseAUTAEnd(lLine, lNewBlock.elements[1]))
                    {
                        lNewBlock.endIndex = index;
                        return lNewBlock;
                    }
                }
                Console.WriteLine("ERROR: AUTA block never terminated: " + filePath + ", line: " + startIndex.ToString() + " : " + lines[startIndex]);
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
            Regex lRegEx = new Regex(DefaultAUTABegin()+"end\\s+" + lTag);
            return lRegEx.IsMatch(lLine);
        }

        public virtual string[] ParseAUTABlockStart(string lLine)
        {
            Regex lRegEx = new Regex(DefaultAUTABegin()+ "([a-zA-Z]+)\\s+([a-zA-Z0-9_]+)");
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
                lInput[i] = lConcatanateRegex.Replace(lInput[i], "$1$2");
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
