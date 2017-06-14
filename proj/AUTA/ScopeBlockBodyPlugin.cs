using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AUTA
{
    public class ScopeBlockBody : CommandInterface
    {
        public ScopeBlockBody(CommandBlock lBlock)
        {
            mLabels = new TemplateLabels();
            mTypes = new TemplateTypes();
            mScopeBodies = new List<ScopeBlockBody>();
            mScopes = new List<string>();
            LoopDetection = false;
            CacheBlock(lBlock);
        }

        public override string GetAcceptedCommands()
        {
            throw new Exception("ERRROR : ScopeBlockBody should not call GetAcceptedCommands");
        }

        public override void CacheBlock(CommandBlock lBlock)
        {
            for (int i = 1; i < lBlock.lines.Count; ++i)
            {
                string lLine = lBlock.lines[i];
                if (mTypes.AcceptedLine(lLine))
                {
                    mTypes.CacheLine(lLine);
                }
                else
                {
                    if (mLabels.AcceptedLine(lLine))
                    {
                        var lLabelBlock = mLabels.ExtractAUTABlock(lBlock.lines.ToList(), i, lBlock.filePath);
                        mLabels.CacheBlock(lLabelBlock);
                        if (lLabelBlock.endIndex > i) i = lLabelBlock.endIndex;
                    }
                    else
                    {
                        var lMatch = lImportScopeBlock.Match(lLine);
                        if (lMatch.Success)
                        {
                            mScopes.Add(lMatch.Groups[1].Value);
                        }
                    }
                }
            }
        }

        public override string[] ProcessBlock(CommandBlock lBlock)
        {
            return null;
        }

        public string[] ProcessBlock(List<string> lResult)
        {
            List<string> lTempCopy;
            if (mLabels.templateLabelSets.Count > 0 || mTypes.templateType.Count > 0 || mScopeBodies.Count == 0)
            {
                lTempCopy = mLabels.DuplicateForLabels(lResult);
                lTempCopy = mTypes.DuplicateForAllTypes(lTempCopy);
                //lTempCopy = RemoveConcatination(lTempCopy);
            }
            else
            {
                lTempCopy = new List<string>();
            }
            foreach (var lScope in mScopeBodies)
            {
                lTempCopy.AddRange(lScope.ProcessBlock(lResult).ToList());
            }

            return lTempCopy.ToArray();
        }

        public void ImportSubScopes()
        {
            ScopeBlock lAllScopes = ScopeBlock.GetInstance();
            foreach (string lSubScope in mScopes)
            {
                lAllScopes.GetScopeFromTag(lSubScope).ImportSubScopes();
                var currScope = lAllScopes.GetScopeFromTag(lSubScope);
                foreach (var ltemptype in currScope.mTypes.templateType)
                {
                    if (!mTypes.templateType.Contains(ltemptype)) mTypes.templateType.Add(ltemptype);
                }
                if (mLabels.templateLabels == null) mLabels.templateLabels = new List<string>();
                if (currScope.mLabels.templateLabels != null)
                {
                    foreach (var lTempLabel in currScope.mLabels.templateLabels)
                    {
                        if (!mLabels.templateLabels.Contains(lTempLabel)) mLabels.templateLabels.Add(lTempLabel);
                        foreach (var lLabelsets in currScope.mLabels.templateLabelSets)
                        {
                            if (!mLabels.templateLabelSets.Contains(lLabelsets)) mLabels.templateLabelSets.Add(lLabelsets);
                        }
                    }
                }
            }
        }

        public override void CacheForProcessing()
        {
            if (LoopDetection == true)
            {
                mError = true;
                Console.WriteLine("ERROR: Circular Reference detected within ScopeBlocks");
                return;
            }
            ImportSubScopes();
            mLabels.CacheForProcessing();
            mTypes.CacheForProcessing();
            LoopDetection = true;
            LoopDetection = false;
        }

        public override void Clear()
        {
            mLabels.Clear();
            mTypes.Clear();
        }

        public override bool CheckErrors()
        {
            mError = mError || mLabels.CheckErrors() || mTypes.CheckErrors();
            return mError;
        }

        public TemplateLabels GetLabels()
        {
            return mLabels;
        }

        public TemplateTypes GetTypes()
        {
            return mTypes;
        }

        private bool LoopDetection;

        private TemplateLabels mLabels;
        private TemplateTypes mTypes;
        private List<string> mScopes;
        private List<ScopeBlockBody> mScopeBodies;
        private static Regex lImportScopeBlock = new Regex(DefaultAUTABegin() + "importscopeblock\\s+([a-zA-Z0-9_]+)");
    }
}