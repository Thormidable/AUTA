using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AUTA
{
    public class TemplateFunctions : CommandInterface
    {
        public TemplateFunctions()
        {
            lFunctionDeclarationRegex = new Regex(TemplateInstantiationPlugin.FunctionDeclarationRegex() + "(.*)");
            lCleanConcatination = new Regex("\\s*##\\s*");
            functionBody = new List<string>();
            lValidateTemplateBody = new Regex(TemplateInstantiationPlugin.ValidateTemplateBlock());
        }

        public override string GetAcceptedCommands()
        {
            return TemplateInstantiationPlugin.FunctionDeclarationRegex();
        }

        public override void CacheBlock(CommandBlock lBlock)
        {
        }

        public int CacheBlock(List<string> paths, int startIndex, string filePath)
        {
            var lCleanedFirstLine = CleanConcatination(paths[startIndex]);
            var lMatch = lFunctionDeclarationRegex.Match(lCleanedFirstLine);
            if (lMatch.Success)
            {
                //if not template Groups[2].Value is null
                functionTemplateTypes = lMatch.Groups[2].Value;
                if (functionTemplateTypes != null && functionTemplateTypes != "")
                {
                    if (!lValidateTemplateBody.IsMatch(functionTemplateTypes))
                    {
                        Console.WriteLine("ERROR: Template Parameter block is not valid: " + filePath + " : " + paths[startIndex]);
                        mError = true;
                    }
                }

                functionReturnPreQualifiers = lMatch.Groups[3].Value;
                functionReturnTypes = lMatch.Groups[4].Value;
                //if no namespace Groups[6].Value is null
                functionNamespace = lMatch.Groups[6].Value;
                functionName = lMatch.Groups[7].Value;
                functionParameters = lMatch.Groups[8].Value;
                functionPostQualifiers = lMatch.Groups[9].Value;
                functionbodyLineOne = lMatch.Groups[10].Value;
                bool lStarted = false;
                int lCounter = 0;
                lCounter += functionbodyLineOne.Count(x => x == '{');
                if (lCounter > 0)
                {
                    lStarted = true;
                }
                lCounter -= functionbodyLineOne.Count(x => x == '}');
                if (lStarted && lCounter == 0)
                {
                    functionBody = new List<string>();
                    functionBody.Add(functionbodyLineOne);
                    return startIndex;
                }

                for (int i = startIndex + 1; i < paths.Count; ++i)
                {
                    lCounter += paths[i].Count(x => x == '{');
                    lCounter -= paths[i].Count(x => x == '}');
                    if (lCounter > 0)
                    {
                        lStarted = true;
                    }
                    if (lCounter < 1)
                    {
                        if (lCounter < 0)
                        {
                            Console.WriteLine("ERROR : Unmatched brace pairs in block : " + paths[0]);
                            mError = true;
                            return startIndex;
                        }
                        if (lStarted == true)
                        {
                            functionBody = paths.GetRange(startIndex + 1, i - startIndex).ToList();
                            if (functionbodyLineOne != "") functionBody.Insert(0, functionbodyLineOne);

                            return i;
                        }
                    }
                }
                Console.WriteLine("ERROR : Unmatched brace pairs in block : " + paths[0]);
                mError = true;
            }
            return startIndex;
        }

        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            return null;
        }

        public override void Clear()
        {
            functionBody.Clear();
        }

        public string GenerateDeclaration()
        {
            if (functionTemplateTypes != null && functionTemplateTypes != "") return "template " + functionTemplateTypes + functionReturnPreQualifiers + " " + functionReturnTypes + " " + functionName + "(" + functionParameters + ")" + functionPostQualifiers + ";";
            else return functionReturnPreQualifiers + " " + functionReturnTypes + " " + functionName + "(" + functionParameters + ")" + functionPostQualifiers + ";";
        }

        public List<string> GenerateDefinition()
        {
            string lEntry;
            if (functionTemplateTypes != null && functionTemplateTypes != "") lEntry = "template " + TemplateBlock.StripDefaultParams(functionTemplateTypes) + functionReturnPreQualifiers + " " + functionReturnTypes + " " + functionNamespace + functionName + "(" + TemplateBlock.StripDefaultParams(functionParameters) + ")" + functionPostQualifiers;
            else lEntry = functionReturnPreQualifiers + " " + functionReturnTypes + " " + functionNamespace + functionName + "(" + TemplateBlock.StripDefaultParams(functionParameters) + ")" + functionPostQualifiers;

            if (functionbodyLineOne != "")
            {
                List<string> lResult = new List<string>();
                lResult.Add(lEntry + functionbodyLineOne);
                lResult.AddRange(functionBody.GetRange(1, functionBody.Count - 1));
                return lResult;
            }
            else
            {
                List<string> lReturn = new List<string>(functionBody);
                lReturn.Insert(0, lEntry);
                return lReturn;
            }
        }

        public List<string> GenerateSpecialisation()
        {
            List<string> lResult = new List<string>();
            string specialisationEntry;

            if (functionTemplateTypes != null && functionTemplateTypes != "")
            {
                specialisationEntry = "template <>" + functionReturnPreQualifiers + " " + functionReturnTypes + " " + functionNamespace + functionName + instanceTypeblock + "(" + TemplateBlock.StripDefaultParams(functionParameters) + ")" + functionPostQualifiers;
            }
            else
            {
                specialisationEntry = functionReturnPreQualifiers + " " + functionReturnTypes + " " + functionNamespace + functionName + "(" + TemplateBlock.StripDefaultParams(functionParameters) + ")" + functionPostQualifiers;
            }

            if (functionbodyLineOne != "")
            {
                lResult.Add(specialisationEntry + functionbodyLineOne);
                if (functionBody.Count > 1) lResult.AddRange(functionBody.GetRange(1, functionBody.Count - 1));
            }
            else
            {
                lResult.Add(specialisationEntry);
                lResult.AddRange(functionBody);
            }

            return lResult;
        }

        public string GenerateInstantiation()
        {
            if (functionTemplateTypes != null && functionTemplateTypes != "")
            {
                return "template" + functionReturnPreQualifiers + " " + functionReturnTypes + " " + functionNamespace + functionName + instanceTypeblock + "(" + TemplateBlock.StripDefaultParams(functionParameters) + ")" + functionPostQualifiers + ";";
            }
            else
            {
                Console.WriteLine("ERROR: Trying to instantiate an template class is not possible.");
                mError = true;
                return "";
            }
        }

        public override void CacheForProcessing()
        {
            if (functionTemplateTypes != null && functionTemplateTypes != "")
            {
                List<string> lCleanBody = new List<string>();
                foreach (var lLine in functionBody)
                {
                    lCleanBody.Add(CleanConcatination(lLine));
                }
                functionBody = lCleanBody;
                if (functionTemplateTypes != null && functionTemplateTypes != "") functionTemplateTypes = CleanConcatination(functionTemplateTypes);

                if (functionReturnPreQualifiers != null) functionReturnPreQualifiers = CleanConcatination(functionReturnPreQualifiers);
                if (functionReturnTypes != null) functionReturnTypes = CleanConcatination(functionReturnTypes);
                if (functionPostQualifiers != null) functionPostQualifiers = CleanConcatination(functionPostQualifiers);
                if (functionNamespace != null) functionNamespace = CleanConcatination(functionNamespace);
                if (functionName != null) functionName = CleanConcatination(functionName);
                if (functionParameters != null) functionParameters = CleanConcatination(functionParameters);

                Regex lRemoveTemplateTypes = new Regex("(?:<|,)(?:\\s*" + TemplateInstantiationPlugin.NameIdentRegex() + "\\s+)(" + TemplateInstantiationPlugin.TypeIdent() + "\\s*)" + TemplateInstantiationPlugin.DefaultValue() + "?");
                //Regex lRemoveTemplateTypes = new Regex("(?:<|,)(?:\\s*[a-zA-Z0-9_]+\\s+)([a-zA-Z_][a-zA-Z0-9_]*)");
                MatchCollection lMatches = lRemoveTemplateTypes.Matches(functionTemplateTypes);
                instanceTypeblock = "<";
                foreach (Match lMatch in lMatches)
                {
                    instanceTypeblock += lMatch.Groups[1].Value + ",";
                }
                instanceTypeblock = instanceTypeblock.Remove(instanceTypeblock.Length - 1, 1);
                instanceTypeblock += ">";
            }
            else
            {
                instanceTypeblock = null;
            }
        }

        public string CleanConcatination(string lLine)
        {
            return lCleanConcatination.Replace(lLine, "##");
        }

        public List<string> functionBody;
        public string functionTemplateTypes;
        public string instanceTypeblock;
        public string functionReturnPreQualifiers;
        public string functionReturnTypes;
        public string functionPostQualifiers;
        public string functionNamespace;
        public string functionName;
        public string functionParameters;
        public string functionbodyLineOne;
        private Regex lFunctionDeclarationRegex;
        private Regex lCleanConcatination;
        private Regex lValidateTemplateBody;
    }
}