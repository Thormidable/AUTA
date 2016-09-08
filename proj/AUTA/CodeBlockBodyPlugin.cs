using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AUTA
{

    public class CodeBlockBodyPlugin : CommandInterface
    {
        public CodeBlockBodyPlugin()
        {
            lRegex = new Regex(TemplateInstantiationPlugin.FunctionDeclarationRegex());
            lCleanConcatination = new Regex("\\s*##\\s*");
            codeBlockBody = new List<string>();
        }

        public override string GetAcceptedCommands()
        {
            return "(body)\\s*(?:[a-zA-Z-0-9_]+)";
        }

        public override void CacheBlock(CommandBlock lBlock)
        {
            codeBlockBody = lBlock.lines.GetRange(1,lBlock.lines.Count-2);
        }
        
        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            return codeBlockBody.ToArray();
        }

        public override void Clear()
        {
            base.Clear();
            codeBlockBody.Clear();
        }
        
        public override void CacheForProcessing()
        {

        }

        public List<string> codeBlockBody;
        Regex lRegex;
        Regex lCleanConcatination;        
    }
}
