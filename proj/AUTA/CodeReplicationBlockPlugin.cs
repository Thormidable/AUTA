using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AUTA
{
    public class CodeReplicationBlockPlugin : CommandInterface
    {

        public CodeReplicationBlockPlugin(CommandBlock lBlock)
        {
            codeTypes = new TemplateTypes();
            codeLabels = new TemplateLabels();
            codeBlocks = new List<CodeBlockBodyPlugin>();            
            CacheBlock(lBlock);
        }

        public override string GetAcceptedCommands()
        {
            throw new Exception("ERRROR : CodeReplicationBlock should not call GetAcceptedCommands");
        }

        public override void CacheBlock(CommandBlock lBlock)
        {
            for (int i = 1; i < lBlock.lines.Count; ++i)
            {
                string lLine = lBlock.lines[i];
                if (codeTypes.AcceptedLine(lLine))
                {
                    codeTypes.CacheLine(lLine);
                }
                else
                {
                    if (codeLabels.AcceptedLine(lLine))
                    {
                        var lLabelBlock = codeLabels.ExtractAUTABlock(lBlock.lines.ToList(), i, lBlock.filePath);
                        codeLabels.CacheBlock(lLabelBlock);
                        if (lLabelBlock.endIndex > i) i = lLabelBlock.endIndex;
                    }
                    else
                    {
                        var newFuncBody = new CodeBlockBodyPlugin();
                        if (newFuncBody.AcceptedLine(lLine))
                        {
                            var codeBlockBlock = newFuncBody.ExtractAUTABlock(lBlock.lines.ToList(), i, lBlock.filePath);
                            if (codeBlockBlock != null)
                            {
                                newFuncBody.CacheBlock(codeBlockBlock);
                                codeBlocks.Add(newFuncBody);
                            }
                            else
                            {
                                mError = true;
                            }
                        }
                    }
                }
            }
        }

        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            List<string> lResult = new List<string>();
            foreach (var lFunction in codeBlocks)
            {
                var lTemp = lFunction.ProcessBlock(lBlock).ToList();
                lTemp = codeLabels.DuplicateForLabels(lTemp);
                lResult.AddRange(codeTypes.DuplicateForAllTypes(lTemp));
            }
            return lResult.ToArray();
        }

        public override void CacheForProcessing()
        {
            codeLabels.CacheForProcessing();
            codeTypes.CacheForProcessing();
            foreach (var function in codeBlocks)
            {
                function.CacheForProcessing();
            }
        }

        public override void Clear()
        {
            codeLabels.Clear();
            codeBlocks.Clear();
        }

        public override bool CheckErrors()
        {
            if (mError == false) mError = codeBlocks.Any(x => x.CheckErrors());
            return mError;   
        }
        
        TemplateLabels codeLabels;
        TemplateTypes codeTypes;
        List<CodeBlockBodyPlugin> codeBlocks;
    }
}
