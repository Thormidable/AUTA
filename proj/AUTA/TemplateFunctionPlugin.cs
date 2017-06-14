using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AUTA
{
    public class TemplateBlock : CommandInterface
    {
        public TemplateBlock()
        {
            templateTypes = new TemplateTypes();
            templateLabels = new TemplateLabels();
            templateFunctions = new List<TemplateFunctions>();
            if (lStripDefaultParams == null) lStripDefaultParams = new Regex(TemplateInstantiationPlugin.DefaultValue());
        }

        public override string GetAcceptedCommands()
        {
            throw new Exception("ERRROR : Template should not call GetAcceptedCommands");
        }

        public override void CacheBlock(CommandBlock lBlock)
        {
            for (int i = 1; i < lBlock.lines.Count; ++i)
            {
                string lLine = lBlock.lines[i];
                if (templateTypes.AcceptedLine(lLine))
                {
                    templateTypes.CacheLine(lLine);
                }
                else
                {
                    if (templateLabels.AcceptedLine(lLine))
                    {
                        var lLabelBlock = templateLabels.ExtractAUTABlock(lBlock.lines.ToList(), i, lBlock.filePath);
                        templateLabels.CacheBlock(lLabelBlock);
                        if (lLabelBlock.endIndex > i) i = lLabelBlock.endIndex;
                    }
                    else
                    {
                        var newFuncBody = new TemplateFunctions();
                        if (newFuncBody.AcceptedLine(lLine))
                        {
                            i = newFuncBody.CacheBlock(lBlock.lines, i, lBlock.filePath);
                            templateFunctions.Add(newFuncBody);
                        }
                    }
                }
            }
        }

        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            return null;
        }

        public string[] GenerateDeclareText(CommandBlock lBlock)
        {
            List<string> lResult = new List<string>();

            foreach (var lFunction in templateFunctions)
            {
                lResult.Add(lFunction.GenerateDeclaration());
            }
            lResult = templateLabels.DuplicateForLabels(lResult);
            return lResult.ToArray();
        }

        public string[] GenerateDefineText(CommandBlock lBlock)
        {
            List<string> lResult = new List<string>();
            foreach (var lFunction in templateFunctions)
            {
                lResult.AddRange(lFunction.GenerateDefinition());
            }
            lResult = templateLabels.DuplicateForLabels(lResult);
            return lResult.ToArray();
        }

        public string[] GenerateSpecialisationText(CommandBlock lBlock)
        {
            List<string> lResult = new List<string>();
            foreach (var lFunction in templateFunctions)
            {
                lResult.AddRange(lFunction.GenerateSpecialisation());
            }
            lResult = templateLabels.DuplicateForLabels(lResult);
            lResult = templateTypes.DuplicateForAllTypes(lResult);
            return lResult.ToArray();
        }

        public string[] GenerateInstanceText(CommandBlock lBlock)
        {
            List<string> lResult = new List<string>();
            foreach (var lFunction in templateFunctions)
            {
                lResult.Add(StripDefaultParams(lFunction.GenerateInstantiation()));
            }
            lResult = templateLabels.DuplicateForLabels(lResult);
            lResult = templateTypes.DuplicateForTypes(lResult);
            return lResult.ToArray();
        }

        public override void CacheForProcessing()
        {
            templateLabels.CacheForProcessing();
            templateTypes.CacheForProcessing();
            foreach (var function in templateFunctions)
            {
                function.CacheForProcessing();
            }
        }

        public override void Clear()
        {
            templateTypes.Clear();
            templateLabels.Clear();
            templateFunctions.Clear();
        }

        public override bool CheckErrors()
        {
            if (templateTypes.CheckErrors()) mError = true;
            else if (templateLabels.CheckErrors()) mError = true;
            else if (mError == false) mError = templateFunctions.Any(x => x.CheckErrors());
            return mError;
        }

        public static List<string> StripDefaultParams(List<string> lInput)
        {
            List<string> lReturn = new List<string>();
            foreach (var lString in lInput)
            {
                lReturn.Add(StripDefaultParams(lString));
            }
            return lReturn;
        }

        public static string StripDefaultParams(string lInput)
        {
            return lStripDefaultParams.Replace(lInput, "");
        }

        private static Regex lStripDefaultParams = null;

        private TemplateTypes templateTypes;
        private TemplateLabels templateLabels;
        private List<TemplateFunctions> templateFunctions;
    }
}