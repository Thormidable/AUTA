using System;
using System.Collections.Generic;
using System.Linq;

namespace AUTA
{
    public class TemplateInstantiationPlugin : CommandInterface
    {
        public TemplateInstantiationPlugin()
        {
            mTemplates = new Dictionary<string, TemplateBlock>();
            mError = false;
            functionExportBlocks = 0;
            functionImportBlocks = 0;
        }

        public override string GetAcceptedCommands()
        {
            return DefaultAUTABegin() + "(function|declare|define|instance|specialise)\\s*(?:[a-zA-Z-0-9_]+)";
        }

        public override void Clear()
        {
            functionExportBlocks = 0;
            functionImportBlocks = 0;
            mError = false;
            mTemplates.Clear();
            mProcessPaths.Clear();
        }

        public override void CacheBlock(CommandBlock lBlock)
        {
            string lCommand = lBlock.elements[0].ToLower();
            if (lCommand == "function")
            {
                ++functionExportBlocks;
                string lExportIdent = lBlock.elements[1];
                if (Program.IsValidIdent(lExportIdent))
                {
                    if (!mTemplates.ContainsKey(lExportIdent))
                    {
                        mTemplates[lExportIdent] = new TemplateBlock();
                        mTemplates[lExportIdent].CacheBlock(lBlock);
                    }
                    else
                    {
                        Console.WriteLine("ERROR: AUTA ident already exists: " + lExportIdent + ". File Path: " + lBlock.filePath.ToString());
                        mError = true;
                    }
                }
            }
            if (lCommand == "declare" || lCommand == "define" || lCommand == "instance" || lCommand == "specialise")
            {
                mProcessPaths.Add(lBlock.filePath);
            }
        }

        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            string lCommand = lBlock.elements[0].ToLower();
            if (lCommand == "declare" || lCommand == "define" || lCommand == "instance" || lCommand == "specialise")
            {
                ++functionImportBlocks;
                string lExportIdent = lBlock.elements[1];
                if (Program.IsValidIdent(lExportIdent))
                {
                    if (mTemplates.ContainsKey(lExportIdent))
                    {
                        string[] lContents = null;
                        switch (lCommand)
                        {
                            case "declare":
                                lContents = mTemplates[lExportIdent].GenerateDeclareText(lBlock);
                                break;

                            case "define":
                                lContents = mTemplates[lExportIdent].GenerateDefineText(lBlock);
                                break;

                            case "instance":
                                lContents = mTemplates[lExportIdent].GenerateInstanceText(lBlock);
                                break;

                            case "specialise":
                                lContents = mTemplates[lExportIdent].GenerateSpecialisationText(lBlock);
                                break;

                            default:
                                return null;
                        }
                        return Program.ProcessExtraCommands(lBlock.lines[0], lContents.ToList());
                    }
                    else
                    {
                        mError = true;
                        Console.WriteLine("ERROR: AUTA ident doesn't exist: " + lExportIdent);
                    }
                }
                else
                {
                    mError = true;
                    Console.WriteLine("ERROR: AUTA ident isn't valid: " + lExportIdent);
                }
            }
            return null;
        }

        public override void CacheForProcessing()
        {
            foreach (var template in mTemplates)
            {
                template.Value.CacheForProcessing();
            }
        }

        public override bool CheckErrors()
        {
            if (mError == false) mError = mTemplates.Any(x => x.Value.CheckErrors());
            return mError;
        }

        public override void OutputResults()
        {
            Console.WriteLine("Exported " + functionExportBlocks + " function blocks");
            Console.WriteLine("Imported " + functionImportBlocks + " function blocks");
        }

        public static string FunctionDeclarationRegex()
        {
            string lDeclaration = "^" + TemplateInstantiationPlugin.TemplateDeclaration() + "?" + PreQualifiersRegex() + "?\\s*(" + TemplateInstantiationPlugin.TypeIdentReturn() + ")(" + TemplateInstantiationPlugin.NamespaceDeclaration() + ")(" + TemplateInstantiationPlugin.NameIdentRegex() + ")\\(([^)]*)\\)" + PostQualifiersRegex() + "?";
            return lDeclaration;
        }

        public static string NamespaceDeclaration()
        {
            return "(?:(?:" + NameIdentRegex() + "::)+)?";
        }

        public static string TemplateDeclaration()
        {
            return "\\s*(template\\s*(" + TemplateBodyRegex() + "))";
        }

        public static string TypeIdentReturn()
        {
            return "(?:" + TypeIdent() + "(?:(?:[&]|[*])*\\s+(?:[&]|[*])*))";
        }

        public static string TypeIdent()
        {
            return "(?:" + NamespaceDeclaration() + NameIdentRegex() + "(?:\\s*(" + TemplateBodyRegex() + "))?)";
        }

        public static string HashConcatenate()
        {
            return "\\s*##\\s*";
        }

        public static string PreQualifiersRegex()
        {
            return "((?:\\s+(?:inline|__global__|static|const|volatile|virtual|restrict|_Atomic|constexpr))+)";
        }

        public static string PostQualifiersRegex()
        {
            return "((?:\\s+(?:const|override|final))+)";
        }

        public static string NameIdentRegex()
        {
            string FirstCharacter = "[a-zA-Z]";
            return "(?:" + FirstCharacter + "(?:[a-zA-Z0-9_]|" + HashConcatenate() + "[a-zA-Z0-9_]" + ")*)";
        }

        public static string TemplateBodyRegex()
        {
            return "(?:<(?:[^<>]|(?<counter><)|(?<-counter>>))+(?(counter)(?!))>)";
        }

        public static string ParameterBodyRegex()
        {
            return "(?:\\((?:[^()]|(?<counter>\\()|(?<-counter>\\)))*(?(counter)(?!))\\))";
        }

        public static string DefaultValue()
        {
            return "(?:\\s*=[^,<(]+" + TemplateBodyRegex() + "?" + ParameterBodyRegex() + "?)";
        }

        public static string TemplateDefaultValue()
        {
            return "(?:\\s*=[^,<(]+" + TemplateBodyRegex() + "?)";
        }

        public static string ValidateTemplateBlock()
        {
            return "<(\\s*" + TypeIdent() + "\\s+" + NameIdentRegex() + TemplateDefaultValue() + "?\\s*,?)+>";
        }

        public IDictionary<string, TemplateBlock> mTemplates;
        private int functionExportBlocks;
        private int functionImportBlocks;
    }
}