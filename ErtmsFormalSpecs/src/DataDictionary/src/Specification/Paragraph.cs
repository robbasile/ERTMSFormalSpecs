// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using DataDictionary.Generated;
using Utils;
using Frame = DataDictionary.Tests.Frame;

namespace DataDictionary.Specification
{
    public class Paragraph : Generated.Paragraph, IHoldsParagraphs, IEditable
    {
        private static readonly int A = Char.ConvertToUtf32("a", 0);

        private int[] _id;

        public int[] Id
        {
            get
            {
                if (_id == null)
                {
                    string[] levels = getId().Split('.');
                    _id = new int[levels.Length];
                    for (int i = 0; i < levels.Length; i++)
                    {
                        try
                        {
                            _id[i] = Int16.Parse(levels[i]);
                        }
                        catch (FormatException)
                        {
                            _id[i] = 0;
                            for (int j = 0; j < levels[i].Length; j++)
                            {
                                if (Char.IsLetterOrDigit(levels[i][j]))
                                {
                                    if (Char.IsDigit(levels[i][j]))
                                    {
                                        _id[i] = _id[i]*10 + Char.Parse(levels[i][j] + "");
                                    }
                                    else
                                    {
                                        int v = (Char.ConvertToUtf32(Char.ToLower(levels[i][j]) + "", 0) - A);
                                        _id[i] = _id[i]*100 + v;
                                    }
                                }
                            }
                        }
                    }
                }
                return _id;
            }
            set
            {
                string tmp = "";

                bool first = true;
                foreach (int i in value)
                {
                    if (!first)
                    {
                        tmp += ".";
                    }
                    tmp += i;
                    first = false;
                }

                setId(tmp);
            }
        }

        /// <summary>
        ///     Provides the Guid of the paragraph and creates one if it is not yet set
        /// </summary>
        public override string Guid
        {
            get
            {
                // Remove the obsolete guid
                if (!string.IsNullOrEmpty(getObsoleteGuid()))
                {
                    setGuid(getObsoleteGuid());
                    setObsoleteGuid(null);
                }

                return base.Guid;
            }
        }

        /// <summary>
        ///     Provides the requirement set references for this paragraph
        /// </summary>
        public ArrayList RequirementSetReferences
        {
            get
            {
                if (allRequirementSets() == null)
                {
                    setAllRequirementSets(new ArrayList());
                }

                return allRequirementSets();
            }
        }

        public string FullId
        {
            get { return getId(); }
            set { setId(value); }
        }

        /// <summary>
        ///     The maximum size of the text to be displayed
        /// </summary>
        private const int MaxTextLength = 50;

        private const bool StripLongText = false;

        /// <summary>
        ///     The paragraph name
        /// </summary>
        public override string Name
        {
            get
            {
                string retVal = FullId;

                if (acceptor.Paragraph_type.aTITLE == getType())
                {
                    retVal = retVal + " " + getText();
                }
                else
                {
                    string textStart = getText();
                    if (StripLongText && textStart.Length > MaxTextLength)
                    {
                        textStart = textStart.Substring(0, MaxTextLength) + "...";
                    }
                    retVal = retVal + " " + textStart;
                }

                return retVal;
            }
            set { }
        }

        /// <summary>
        ///     Allow to edit the paragraph text in the ExpressionText richttextbox
        /// </summary>
        public override string ExpressionText
        {
            get { return Text; }
            set { Text = value; }
        }

        /// <summary>
        ///     The paragraph text
        /// </summary>
        public string Text
        {
            get
            {
                string retVal = getText();

                if (string.IsNullOrEmpty(retVal))
                {
                    if (getMessage() != null)
                    {
                        Message msg = getMessage() as Message;
                        if (msg != null)
                        {
                            retVal += msg.Text;
                        }
                    }
                    if (allTypeSpecs() != null)
                    {
                        foreach (TypeSpec aTypeSpec in allTypeSpecs())
                        {
                            if (aTypeSpec.getShort_description() == null && getName() != null)
                            {
                                aTypeSpec.setShort_description(getName());
                            }
                            retVal += aTypeSpec.Text;
                        }
                    }
                }

                return retVal;
            }
            set { setText(value); }
        }

        /// <summary>
        /// No syntax highlighting for this
        /// </summary>
        public bool SyntaxHightlight { get { return false; } }

        /// <summary>
        /// Editable window title
        /// </summary>
        public string Title { get { return "Requirement"; } }

        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = null;
                if (EnclosingParagraph != null)
                {
                    retVal = EnclosingParagraph.SubParagraphs;
                }
                else
                {
                    Chapter chapter = EnclosingChapter;
                    if (chapter != null)
                    {
                        retVal = chapter.Paragraphs;
                    }
                }
                return retVal;
            }
        }

        public Paragraph EnclosingParagraph
        {
            get { return getFather() as Paragraph; }
        }

        public Chapter EnclosingChapter
        {
            get { return getFather() as Chapter; }
        }

        public bool SubParagraphBelongsToRequirementSet(RequirementSet requirementSet)
        {
            bool retVal = false;

            foreach (Paragraph p in SubParagraphs)
            {
                retVal = p.BelongsToRequirementSet(requirementSet) ||
                         p.SubParagraphBelongsToRequirementSet(requirementSet);
                if (retVal)
                {
                    break;
                }
            }

            return retVal;
        }

        public void SetType(acceptor.Paragraph_type type)
        {
            setType(type);
            switch (type)
            {
                case acceptor.Paragraph_type.aREQUIREMENT:
                    break;

                default:
                    setImplementationStatus(acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NotImplementable);
                    break;
            }
        }

        /// <summary>
        ///     Tells if the paragraph is of the selected type
        /// </summary>
        /// <returns></returns>
        public bool IsTitle
        {
            get { return (getType() == acceptor.Paragraph_type.aTITLE); }
        }


        /// <summary>
        ///     Looks for a specific paragraph
        /// </summary>
        /// <param name="id">The id of the paragraph to find</param>
        /// <param name="create">If true, creates the paragraph tree if needed</param>
        /// <returns></returns>
        public Paragraph FindParagraph(String id, bool create)
        {
            Paragraph retVal = null;

            if (String.Compare(getId(), id, StringComparison.Ordinal) == 0)
            {
                retVal = this;
            }
            else
            {
                if ((id.StartsWith(FullId) && id[FullId.Length] == '.') || !create)
                {
                    foreach (Paragraph sub in SubParagraphs)
                    {
                        retVal = sub.FindParagraph(id, create);
                        if (retVal != null)
                        {
                            break;
                        }
                    }

                    if (retVal == null && create)
                    {
                        retVal = (Paragraph) acceptor.getFactory().createParagraph();
                        string subId = id.Substring(FullId.Length + 1);
                        string[] items = subId.Split('.');
                        if (items.Length > 0)
                        {
                            retVal.setId(FullId + "." + items[0]);
                            appendParagraphs(retVal);

                            if (retVal.getId().Length < id.Length)
                            {
                                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                                retVal = retVal.FindParagraph(id, create);
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The sub paragraphs of this paragraph
        /// </summary>
        /// <param name="letter">Indicates that the paragraph id should be terminated by a letter</param>
        public string GetNewSubParagraphId(bool letter)
        {
            string retVal;

            if (letter)
            {
                retVal = FullId + ".a";
            }
            else
            {
                retVal = FullId + ".1";
            }

            if (SubParagraphs.Count > 0)
            {
                Paragraph lastParagraph = SubParagraphs[SubParagraphs.Count - 1] as Paragraph;
                if (lastParagraph != null)
                {
                    int[] ids = lastParagraph.Id;
                    int lastId = ids[ids.Length - 1];
                    if (letter)
                    {
                        retVal = FullId + "." + (char) ('a' + (lastId + 1));
                    }
                    else
                    {
                        retVal = FullId + "." + (lastId + 1);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The sub paragraphs of this paragraph
        /// </summary>
        public ArrayList SubParagraphs
        {
            get
            {
                if (allParagraphs() == null)
                {
                    setAllParagraphs(new ArrayList());
                }
                return allParagraphs();
            }
            set { setAllParagraphs(value); }
        }

        /// <summary>
        ///     The type specs of this paragraph
        /// </summary>
        public ArrayList TypeSpecs
        {
            get
            {
                if (allTypeSpecs() == null)
                {
                    setAllTypeSpecs(new ArrayList());
                }
                return allTypeSpecs();
            }
            set { setAllTypeSpecs(value); }
        }

        /// <summary>
        ///     Adds a type spec to a paragraph
        /// </summary>
        /// <param name="aTypeSpec">The type spec to add</param>
        public void AddTypeSpec(TypeSpec aTypeSpec)
        {
            TypeSpecs.Add(aTypeSpec);
        }

        public override int CompareTo(IModelElement other)
        {
            int retVal = 0;

            if (other is Paragraph)
            {
                Paragraph otherParagraph = other as Paragraph;

                int[] levels = Id;
                int[] otherLevels = otherParagraph.Id;

                int i = 0;
                while (i < levels.Length && i < otherLevels.Length && retVal == 0)
                {
                    if (levels[i] < otherLevels[i])
                    {
                        retVal = -1;
                    }
                    else
                    {
                        if (levels[i] > otherLevels[i])
                        {
                            retVal = 1;
                        }
                    }
                    i = i + 1;
                }

                if (retVal == 0)
                {
                    if (i < levels.Length)
                    {
                        retVal = -1;
                    }
                    else if (i < otherLevels.Length)
                    {
                        retVal = 1;
                    }
                }
            }
            else
            {
                retVal = base.CompareTo(other);
            }

            return retVal;
        }

        /// <summary>
        ///     The paragraph level.
        ///     1.1 is level 2, ...
        /// </summary>
        public int Level
        {
            get { return getId().Split('.').Length; }
        }

        /**
         * Indicates that the paragraph need an implementation
         */

        public bool IsApplicable()
        {
            bool retVal = false;

            if (getType() == acceptor.Paragraph_type.aREQUIREMENT)
            {
                retVal = getImplementationStatus() != acceptor.SPEC_IMPLEMENTED_ENUM.defaultSPEC_IMPLEMENTED_ENUM
                         && getImplementationStatus() != acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NotImplementable;
            }

            return retVal;
        }

        /// <summary>
        ///     Restructures the name of this paragraph
        /// </summary>
        public void RestructureName()
        {
            if (EnclosingParagraph != null)
            {
                setId(getId().Substring(EnclosingParagraph.FullId.Length + 1));
            }
            foreach (Paragraph paragraph in SubParagraphs)
            {
                paragraph.RestructureName();
            }
        }

        /// <summary>
        ///     Finds all req ref to this paragraph
        /// </summary>
        private class ReqRefFinder : Visitor
        {
            /// <summary>
            ///     Provides the paragraph currently looked for
            /// </summary>
            private Paragraph Paragraph { get; set; }

            /// <summary>
            ///     Provides the req refs which implement this paragraph
            /// </summary>
            public List<ReqRef> Implementations { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="paragraph"></param>
            public ReqRefFinder(Paragraph paragraph)
            {
                Paragraph = paragraph;
                Implementations = new List<ReqRef>();
            }

            public override void visit(Generated.ReqRef obj, bool visitSubNodes)
            {
                ReqRef reqRef = (ReqRef) obj;

                if (reqRef.Paragraph == Paragraph)
                {
                    Implementations.Add(reqRef);
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Provides the list of references to this paragraph
        /// </summary>
        public List<ReqRef> Implementations
        {
            get
            {
                ReqRefFinder finder = new ReqRefFinder(this);
                finder.visit(Dictionary);
                return finder.Implementations;
            }
        }

        /// <summary>
        ///     Fills the collection of paragraphs with this paragraph, and the sub paragraphs
        /// </summary>
        /// <param name="retVal"></param>
        public void FillCollection(List<Paragraph> retVal)
        {
            retVal.Add(this);
            foreach (Paragraph subParagraph in SubParagraphs)
            {
                subParagraph.FillCollection(retVal);
            }
        }

        /// <summary>
        ///     Changes the type of the paragraph if the paragraph type is the original type
        /// </summary>
        /// <param name="originalType">The type of the paragraph which should be matched</param>
        /// <param name="targetType">When the originalType is matched, the new type to set</param>
        public void ChangeType(acceptor.Paragraph_type originalType, acceptor.Paragraph_type targetType)
        {
            // If the type is matched, change the type
            if (getType() == originalType)
            {
                setType(targetType);
            }

            // Recursively apply this procedure on sub paragraphs
            foreach (Paragraph paragraph in SubParagraphs)
            {
                paragraph.ChangeType(originalType, targetType);
            }
        }

        /// <summary>
        ///     Worker for get sub paragraphs
        /// </summary>
        /// <param name="retVal"></param>
        public void GetParagraphs(List<Paragraph> retVal)
        {
            foreach (Paragraph p in SubParagraphs)
            {
                retVal.Add(p);
                p.GetParagraphs(retVal);
            }
        }

        /// <summary>
        ///     Provides all sub paragraphs of this paragraph
        /// </summary>
        /// <returns></returns>
        public List<Paragraph> GetSubParagraphs()
        {
            List<Paragraph> retVal = new List<Paragraph>();

            GetParagraphs(retVal);

            return retVal;
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="element"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                Paragraph item = element as Paragraph;
                if (item != null)
                {
                    appendParagraphs(item);
                }
            }
        }

        private class RemoveReqRef : Visitor
        {
            /// <summary>
            ///     The paragraph for which no req ref should exist
            /// </summary>
            private Paragraph Paragraph { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="paragraph"></param>
            public RemoveReqRef(Paragraph paragraph)
            {
                Paragraph = paragraph;
            }

            public override void visit(Generated.ReqRef obj, bool visitSubNodes)
            {
                ReqRef reqRef = (ReqRef) obj;

                if (reqRef.Paragraph == Paragraph)
                {
                    reqRef.Delete();
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Also removes the req refs to that paragraph
        /// </summary>
        public override void Delete()
        {
            RemoveReqRef remover = new RemoveReqRef(this);
            foreach (Dictionary dictionary in EFSSystem.Dictionaries)
            {
                remover.visit(dictionary, true);
            }

            base.Delete();
        }

        /// <summary>
        ///     Indicates whether this paragraphs belongs to the functionam block whose name is provided as parameter
        /// </summary>
        /// <param name="requirementSet"></param>
        public bool BelongsToRequirementSet(RequirementSet requirementSet)
        {
            bool retVal = false;

            if (requirementSet != null)
            {
                // Try to find a reference to this requirement set
                foreach (RequirementSetReference reference in RequirementSetReferences)
                {
                    if (reference.Ref == requirementSet)
                    {
                        retVal = true;
                        break;
                    }
                }

                // Maybe a parent paragraph references this requirement set 
                // (only if the requirement set specifies that selection is recursive)
                if (!retVal && requirementSet.getRecursiveSelection())
                {
                    Paragraph enclosing = EnclosingParagraph;
                    if (enclosing != null)
                    {
                        retVal = enclosing.BelongsToRequirementSet(requirementSet);
                    }
                }

                // Try if the requirement belong to a sub requirement set
                if (!retVal)
                {
                    foreach (RequirementSet subSet in requirementSet.SubSets)
                    {
                        if (BelongsToRequirementSet(subSet))
                        {
                            retVal = true;
                            break;
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Appends this paragraph to the requirement set if it does not belong to it already
        /// </summary>
        /// <param name="requirementSet"></param>
        public bool AppendToRequirementSet(RequirementSet requirementSet)
        {
            bool retVal = false;

            if (!BelongsToRequirementSet(requirementSet))
            {
                retVal = true;
                RequirementSetReference reference =
                    (RequirementSetReference) acceptor.getFactory().createRequirementSetReference();
                reference.setTarget(requirementSet.Guid);
                appendRequirementSets(reference);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the list of applicable requirement sets
        /// </summary>
        /// <returns></returns>
        public HashSet<RequirementSet> ApplicableRequirementSets
        {
            get
            {
                HashSet<RequirementSet> retVal = new HashSet<RequirementSet>();

                FillApplicableRequirementSets(retVal);

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the list of applicable requirement sets
        /// </summary>
        /// <param name="applicableRequirementSets"></param>
        private void FillApplicableRequirementSets(HashSet<RequirementSet> applicableRequirementSets)
        {
            foreach (RequirementSetReference reference in RequirementSetReferences)
            {
                RequirementSet requirementSet = reference.Ref;
                if (requirementSet != null)
                {
                    applicableRequirementSets.Add(reference.Ref);
                }
            }

            Paragraph enclosing = EnclosingParagraph;
            if (enclosing != null)
            {
                enclosing.FillApplicableRequirementSets(applicableRequirementSets);
            }
        }

        /// <summary>
        ///     Indicates if all implementations of the paragraph have been verified
        ///     If there are none, returns false
        /// </summary>
        public bool Verified
        {
            get
            {
                bool retVal = true;

                bool reqRelatedFound = false;
                foreach (ReqRef reqRef in Implementations)
                {
                    ReqRelated reqRelated = reqRef.Model as ReqRelated;

                    if (reqRelated != null)
                    {
                        retVal = retVal && reqRelated.getVerified();
                        reqRelatedFound = true;
                    }
                }

                if (!reqRelatedFound)
                {
                    retVal = false;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Finds the chapter enclosing this paragraph
        /// </summary>
        /// <returns></returns>
        public Chapter FindEnclosingChapter()
        {
            Chapter retVal = null;

            Paragraph current = this;
            bool found = false;
            while (!found)
            {
                if (current.EnclosingChapter != null)
                {
                    retVal = current.EnclosingChapter;
                    found = true;
                }
                else if (current.EnclosingParagraph != null)
                {
                    current = current.EnclosingParagraph;
                }
                else
                {
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Finds the specification enclosing this paragraph
        /// </summary>
        /// <returns></returns>
        public Specification FindEnclosingSpecification()
        {
            Specification retVal = null;

            Chapter enclosingChapter = FindEnclosingChapter();
            if (enclosingChapter != null)
            {
                retVal = FindEnclosingChapter().EnclosingSpecification;
            }

            return retVal;
        }

        /// <summary>
        ///     Creates an update for this paragraph in the provided dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to put the updated paragraph in</param>
        /// <returns>The updated paragraph</returns>
        public Paragraph CreateParagraphUpdate(Dictionary dictionary)
        {
            Paragraph retVal = (Paragraph) acceptor.getFactory().createParagraph();
            retVal.FullId = FullId;
            retVal.Text = Text;
            retVal.setUpdates(Guid);

            Specification specUpdate = FindEnclosingSpecification().FindOrCreateSpecificationUpdate(dictionary);

            // If the enclosing is a paragraph, find or create the enclosing paragraph update
            // If it is a chapter, find or create the chapter update
            if (EnclosingParagraph != null)
            {
                string parentId = FullId.Substring(0, FullId.LastIndexOf('.'));
                Paragraph parent = specUpdate.FindParagraphByNumber(parentId);
                if (parent == null)
                {
                    parent = EnclosingParagraph.CreateParagraphUpdate(dictionary);
                }
                parent.appendParagraphs(retVal);
                parent.SubParagraphs.Sort();
            }
            else if (EnclosingChapter != null)
            {
                Chapter parent = EnclosingChapter.FindChapterUpdate(dictionary);
                if (parent == null)
                {
                    parent = EnclosingChapter.CreateChapterUpdate(specUpdate);
                }
                parent.appendParagraphs(retVal);
                parent.Paragraphs.Sort();
            }

            UpdatedBy.Add(retVal);

            return retVal;
        }

        /// <summary>
        ///     Creates the status message
        /// </summary>
        /// <returns>the status string for the selected element</returns>
        public override string CreateStatusMessage()
        {
            string retVal = base.CreateStatusMessage();

            List<Paragraph> paragraphs = new List<Paragraph> {this};
            GetParagraphs(paragraphs);

            retVal += CreateParagraphSetStatus(paragraphs);

            return retVal;
        }

        /// <summary>
        ///     Creates the status string for a set of paragraphs
        /// </summary>
        /// <param name="paragraphs"></param>
        /// <returns></returns>
        public static string CreateParagraphSetStatus(List<Paragraph> paragraphs)
        {
            String retVal = "Statistics : ";

            ParagraphSetMetrics metrics = CreateParagraphSetMetrics(paragraphs);
            if (metrics.SubParagraphCount > 0 && metrics.ImplementableCount > 0)
            {
                retVal += metrics.SubParagraphCount + " requirements, ";
                retVal += +metrics.ImplementableCount + " implementable (" +
                          Math.Round(((float) metrics.ImplementableCount/metrics.SubParagraphCount*100), 2) + "%), ";
                retVal += metrics.ImplementedCount + " implemented (" +
                          Math.Round(((float) metrics.ImplementedCount/metrics.ImplementableCount*100), 2) + "%), ";
                retVal += +metrics.UnImplementedCount + " not implemented (" +
                          Math.Round(((float) metrics.UnImplementedCount/metrics.ImplementableCount*100), 2) + "%), ";
                retVal += metrics.NewRevisionAvailable + " with new revision (" +
                          Math.Round(((float) metrics.NewRevisionAvailable/metrics.ImplementableCount*100), 2) + "%), ";
                retVal += metrics.TestedCount + " tested (" +
                          Math.Round(((float) metrics.TestedCount/metrics.ImplementableCount*100), 2) + "%)";
            }
            else
            {
                retVal += "No implementable requirement selected";
            }

            return retVal;
        }

        /// <summary>
        ///     Holds metrics computed on a set of paragraphs
        /// </summary>
        public struct ParagraphSetMetrics
        {
            public int SubParagraphCount;
            public int ImplementableCount;
            public int ImplementedCount;
            public int UnImplementedCount;
            public int NotImplementable;
            public int NewRevisionAvailable;
            public int TestedCount;
        }

        /// <summary>
        /// Indicates that the paragraph should be considered while computing metrics
        /// that is, if the paragraph is related to a requirements set with "Applicable" flag set to true
        /// </summary>
        /// <returns></returns>
        private bool ConsiderInMetrics()
        {
            bool retVal = false;

            foreach (RequirementSet requirementSet in ApplicableRequirementSets)
            {
                if (requirementSet.getApplicable())
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Creates the stat message according to the list of paragraphs provided
        /// </summary>
        /// <param name="paragraphs"></param>
        /// <returns></returns>
        public static ParagraphSetMetrics CreateParagraphSetMetrics(List<Paragraph> paragraphs)
        {
            ParagraphSetMetrics retVal = new ParagraphSetMetrics {SubParagraphCount = paragraphs.Count};

            Dictionary<Paragraph, List<ReqRef>> paragraphsReqRefDictionary = null;
            foreach (Paragraph p in paragraphs)
            {
                if (paragraphsReqRefDictionary == null)
                {
                    paragraphsReqRefDictionary = p.Dictionary.ParagraphsReqRefs;
                }

                if (p.ConsiderInMetrics())
                {
                    switch (p.getImplementationStatus())
                    {
                        case acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented:
                            retVal.ImplementableCount += 1;

                            bool implemented = true;
                            if (paragraphsReqRefDictionary.ContainsKey(p))
                            {
                                List<ReqRef> implementations = paragraphsReqRefDictionary[p];
                                foreach (ReqRef implementation in implementations)
                                {
                                    // the implementation may be also a ReqRef
                                    ReqRelated reqRelated = implementation.Enclosing as ReqRelated;
                                    if (reqRelated != null)
                                    {
                                        // Do not consider tests
                                        if (EnclosingFinder<Frame>.find(reqRelated) == null)
                                        {
                                            implemented = implemented && reqRelated.ImplementationCompleted;
                                        }
                                    }
                                }
                            }
                            if (implemented)
                            {
                                retVal.ImplementedCount += 1;
                            }
                            else
                            {
                                retVal.UnImplementedCount += 1;
                            }
                            break;

                        case acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NA:
                        case acceptor.SPEC_IMPLEMENTED_ENUM.defaultSPEC_IMPLEMENTED_ENUM:
                            retVal.ImplementableCount += 1;
                            retVal.UnImplementedCount += 1;
                            break;

                        case acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NotImplementable:
                            retVal.NotImplementable += 1;
                            break;

                        case acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NewRevisionAvailable:
                            retVal.ImplementableCount += 1;
                            retVal.NewRevisionAvailable += 1;
                            break;
                    }
                }
                else
                {
                    retVal.NotImplementable += 1;
                }
            }

            // Count the tested paragraphs
            HashSet<Paragraph> testedParagraphs = new HashSet<Paragraph>();
            foreach (Dictionary dictionary in EfsSystem.Instance.Dictionaries)
            {
                foreach (Paragraph p in dictionary.CoveredRequirements())
                {
                    testedParagraphs.Add(p);
                }
            }

            foreach (Paragraph p in paragraphs)
            {
                if (testedParagraphs.Contains(p))
                {
                    retVal.TestedCount += 1;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <param name="enclosingId"></param>
        /// <returns></returns>
        public static Paragraph CreateDefault(ICollection enclosingCollection, string enclosingId)
        {
            Paragraph retVal = (Paragraph) acceptor.getFactory().createParagraph();

            Util.DontNotify(() =>
            {
                retVal.FullId = enclosingId + "." + GetElementNumber(enclosingCollection);
                retVal.Text = "";
                retVal.setType(acceptor.Paragraph_type.aREQUIREMENT);

                foreach (RequirementSet requirementSet in EfsSystem.Instance.RequirementSets)
                {
                    requirementSet.SetDefaultRequirementSets(retVal);
                }
            });

            return retVal;
        }
    }
}