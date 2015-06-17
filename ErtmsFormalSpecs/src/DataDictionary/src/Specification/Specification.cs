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

namespace DataDictionary.Specification
{
    public class Specification : Generated.Specification, IFinder, IHoldsParagraphs
    {
        /// <summary>
        ///     Used to temporarily store the list of chapters
        /// </summary>
        private ArrayList savedChapters;

        /// <summary>
        ///     Constructor
        /// </summary>
        public Specification()
            : base()
        {
            FinderRepository.INSTANCE.Register(this);
        }

        /// <summary>
        ///     The version of the specification
        /// </summary>
        public string Version
        {
            get { return getVersion(); }
            set { setVersion(value); }
        }

        /// <summary>
        ///     The chapters
        /// </summary>
        public ArrayList Chapters
        {
            get
            {
                if (allChapters() == null)
                {
                    setAllChapters(new ArrayList());
                }
                return allChapters();
            }
        }

        /// <summary>
        ///     The cache
        /// </summary>
        public Dictionary<String, Paragraph> TheCache = new Dictionary<string, Paragraph>();

        /// <summary>
        ///     Clears the caches
        /// </summary>
        public void ClearCache()
        {
            TheCache.Clear();
        }

        /// <summary>
        ///     Removes temporary files created for reference chapters
        /// </summary>
        public void ClearTempFiles()
        {
            if (allChapterRefs() != null)
            {
                foreach (ChapterRef aReferenceChapter in allChapterRefs())
                {
                    aReferenceChapter.ClearTempFile();
                }
            }
        }

        /// <summary>
        ///     Used to store the list of chapters before saving the specification
        /// </summary>
        public void StoreInfo()
        {
            savedChapters = new ArrayList();
            foreach (Chapter aChapter in allChapters())
            {
                savedChapters.Add(aChapter);
            }
            allChapters().Clear();
        }

        /// <summary>
        ///     Used to restore the list of chapters, after having saved the specification
        /// </summary>
        public void RestoreInfo()
        {
            setAllChapters(new ArrayList());
            foreach (Chapter aChapter in savedChapters)
            {
                allChapters().Add(aChapter);
            }
            savedChapters.Clear();
        }

        /// <summary>
        ///     The Guid cache
        /// </summary>
        private Dictionary<string, Paragraph> GuidCache = new Dictionary<string, Paragraph>();

        private class GuidParagraphVisitor : Visitor
        {
            /// <summary>
            ///     The cache to update
            /// </summary>
            private Dictionary<string, Paragraph> GuidCache { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="guidCache">The cache to update</param>
            public GuidParagraphVisitor(Dictionary<string, Paragraph> guidCache)
            {
                GuidCache = guidCache;
            }

            /// <summary>
            ///     Updates the cache
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                GuidCache[paragraph.getGuid()] = paragraph;

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Looks for the specific paragraph in the specification using its Guid for identification
        /// </summary>
        /// <param name="guid">The guid of the paragraph to find</param>
        /// <returns></returns>
        public Paragraph FindParagraphByGuid(String guid)
        {
            Paragraph retVal = null;

            if (guid != null)
            {
                if (!GuidCache.TryGetValue(guid, out retVal))
                {
                    GuidParagraphVisitor cacheUpdater = new GuidParagraphVisitor(GuidCache);
                    cacheUpdater.visit(this);
                }

                GuidCache.TryGetValue(guid, out retVal);
                GuidCache[guid] = retVal;
            }

            return retVal;
        }

        /// <summary>
        ///     Looks for the specific paragraph in the specification
        /// </summary>
        /// <param name="id">The id of the paragraph to find</param>
        /// <param name="create">If true, creates the paragraph tree if needed</param>
        /// <returns></returns>
        public Paragraph FindParagraphByNumber(String id, bool create = false)
        {
            Paragraph retVal = null;

            if (id != null)
            {
                if (!TheCache.ContainsKey(id))
                {
                    Paragraph tmp = null;

                    foreach (Chapter chapter in Chapters)
                    {
                        if (id.StartsWith(chapter.getId()))
                        {
                            tmp = chapter.FindParagraph(id, create);
                            if (tmp != null)
                            {
                                break;
                            }
                        }
                    }

                    if (tmp != null)
                    {
                        TheCache[id] = tmp;
                    }
                    else
                    {
                        return null;
                    }
                }

                retVal = TheCache[id];
            }

            return retVal;
        }


        /// <summary>
        ///     Looks for the specific chapter in this specification
        /// </summary>
        /// <param name="id">Id of the chapter to find</param>
        /// <returns></returns>
        public Chapter FindChapter(String id)
        {
            Chapter retVal = null;

            foreach (Chapter chapter in Chapters)
            {
                if (chapter.getId() == id)
                {
                    retVal = chapter;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Looks for specific paragraphs in the specification, whose number begins with the Id provided
        /// </summary>
        /// <param name="id"></param>
        /// <param name="retVal">the list to fill with the corresponding paragraphs</param>
        public void SubParagraphs(String id, List<Paragraph> retVal)
        {
            foreach (Chapter chapter in Chapters)
            {
                chapter.SubParagraphs(id, retVal);
            }
        }

        /// <summary>
        ///     Provides the list of all paragraphs
        /// </summary>
        /// <returns></returns>
        public List<string> ParagraphList()
        {
            List<string> retVal = new List<string>();

            foreach (Chapter chapter in Chapters)
            {
                foreach (Paragraph paragraph in chapter.Paragraphs)
                {
                    retVal.Add(paragraph.getId());
                }
            }

            return retVal;
        }


        /// <summary>
        ///     Provides the paragraphs that require an implementation
        /// </summary>
        public ICollection<Paragraph> ApplicableParagraphs
        {
            get
            {
                ICollection<Paragraph> retVal = new HashSet<Paragraph>();

                foreach (Chapter c in Chapters)
                {
                    foreach (Paragraph p in c.applicableParagraphs())
                    {
                        retVal.Add(p);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the paragraphs that are marked as needing more information
        /// </summary>
        public ICollection<Paragraph> MoreInformationNeeded
        {
            get
            {
                ICollection<Paragraph> retVal = new HashSet<Paragraph>();

                foreach (Chapter c in Chapters)
                {
                    foreach (Paragraph p in c.applicableParagraphs())
                    {
                        if (p.getMoreInfoRequired() == true)
                        {
                            retVal.Add(p);
                        }
                    }
                }

                return retVal;
            }
        }


        /// <summary>
        ///     Provides the paragraphs that are marked as specification issues
        /// </summary>
        public ICollection<Paragraph> SpecIssues
        {
            get
            {
                ICollection<Paragraph> retVal = new HashSet<Paragraph>();

                foreach (Chapter c in Chapters)
                {
                    foreach (Paragraph p in c.applicableParagraphs())
                    {
                        if (p.getSpecIssue() == true)
                        {
                            retVal.Add(p);
                        }
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the paragraphs from the chapter Design Choices
        /// </summary>
        public ICollection<Paragraph> DesignChoices
        {
            get
            {
                ICollection<Paragraph> retVal = new HashSet<Paragraph>();

                foreach (Chapter c in Chapters)
                {
                    if (c.Name.Equals("Chapter A1"))
                    {
                        foreach (Paragraph p in c.applicableParagraphs())
                        {
                            retVal.Add(p);
                        }
                        break;
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the paragraphs from the chapter Comments
        /// </summary>
        public ICollection<Paragraph> OnlyComments
        {
            get
            {
                ICollection<Paragraph> retVal = new HashSet<Paragraph>();

                foreach (Chapter c in Chapters)
                {
                    foreach (Paragraph p in c.applicableParagraphs())
                    {
                        if (!string.IsNullOrEmpty(p.Comment))
                        {
                            if (!p.getSpecIssue() && !p.getMoreInfoRequired())
                            {
                                retVal.Add(p);
                            }
                        }
                    }
                }

                return retVal;
            }
        }

        public List<Paragraph> AllParagraphs
        {
            get
            {
                List<Paragraph> retVal = new List<Paragraph>();

                foreach (Chapter chapter in Chapters)
                {
                    chapter.GetParagraphs(retVal);
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Gets all paragraphs from a specification
        /// </summary>
        /// <param name="paragraphs"></param>
        public void GetParagraphs(List<Paragraph> paragraphs)
        {
            foreach (Chapter chapter in Chapters)
            {
                chapter.GetParagraphs(paragraphs);
            }
        }

        private class NotImplementedVisitor : Visitor
        {
            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                if (paragraph.getImplementationStatus() == acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NA)
                {
                    paragraph.AddInfo("Not implemented");
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Indicates which requirement has been not implemented
        /// </summary>
        public void CheckImplementation()
        {
            NotImplementedVisitor visitor = new NotImplementedVisitor();
            visitor.visit(this);
        }

        private class NotReviewedVisitor : Visitor
        {
            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                if (!paragraph.getReviewed())
                {
                    paragraph.AddInfo("Not reviewed");
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Indicates which requirement has been not reviewed
        /// </summary>
        public void CheckReview()
        {
            NotReviewedVisitor visitor = new NotReviewedVisitor();
            visitor.visit(this);
        }

        private class NiewRevisionVisitor : Visitor
        {
            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                if (paragraph.getImplementationStatus() == acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NewRevisionAvailable)
                {
                    paragraph.AddInfo("New revision");
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Indicates which requirement has been not reviewed
        /// </summary>
        public void CheckNewRevision()
        {
            NiewRevisionVisitor visitor = new NiewRevisionVisitor();
            visitor.visit(this);
        }

        private class ApplicableParagraphsVisitor : Visitor
        {
            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                if (paragraph.isApplicable())
                {
                    paragraph.AddInfo("Applicable paragraph");
                }

                base.visit(obj, visitSubNodes);
            }
        }

        private class NonApplicableParagraphsVisitor : Visitor
        {
            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                if (!paragraph.isApplicable())
                {
                    paragraph.AddInfo("Non applicable paragraph");
                }

                base.visit(obj, visitSubNodes);
            }
        }

        private class SpecIssuesParagraphsVisitor : Visitor
        {
            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                if (paragraph.getSpecIssue())
                {
                    paragraph.AddInfo("This paragraph has an issue");
                }

                base.visit(obj, visitSubNodes);
            }
        }

        public void CheckApplicable()
        {
            ApplicableParagraphsVisitor visitor = new ApplicableParagraphsVisitor();
            visitor.visit(this);
        }

        public void CheckNonApplicable()
        {
            NonApplicableParagraphsVisitor visitor = new NonApplicableParagraphsVisitor();
            visitor.visit(this);
        }

        public void CheckSpecIssues()
        {
            SpecIssuesParagraphsVisitor visitor = new SpecIssuesParagraphsVisitor();
            visitor.visit(this);
        }


        private class MoreInfoParagraphsVisitor : Visitor
        {
            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                if (paragraph.getMoreInfoRequired())
                {
                    paragraph.AddInfo("More info is required");
                }

                base.visit(obj, visitSubNodes);
            }
        }

        public void CheckMoreInfo()
        {
            MoreInfoParagraphsVisitor visitor = new MoreInfoParagraphsVisitor();
            visitor.visit(this);
        }

        /// <summary>
        ///     Provides all ReqReferences
        /// </summary>
        private class AllReferences : Visitor
        {
            /// <summary>
            ///     Provides the list of references found
            /// </summary>
            public List<ReqRef> References { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            public AllReferences()
            {
                References = new List<ReqRef>();
            }

            public override void visit(Generated.ReqRef obj, bool visitSubNodes)
            {
                References.Add((ReqRef) obj);

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            ///     The set of paragraph which have a functional test defined
            /// </summary>
            public HashSet<Paragraph> TestedParagraphs { get; private set; }

            /// <summary>
            ///     Set up the TestedParagraphs cache
            /// </summary>
            /// <param name="dictionary">Initialises this class according to the dictionary provided</param>
            public void Initialize(Dictionary dictionary)
            {
                foreach (Tests.Frame frame in dictionary.Tests)
                {
                    visit(frame);
                }

                TestedParagraphs = new HashSet<Paragraph>();
                foreach (ReqRef reqRef in References)
                {
                    Paragraph paragraph = reqRef.Paragraph;
                    if (paragraph != null)
                    {
                        TestedParagraphs.Add(paragraph);
                    }
                }
            }
        }

        /// <summary>
        ///     Provides the paragraph which are implemented but where no functional test is present
        /// </summary>
        private class ImplementedWithNoFunctionalTestVisitor : Visitor
        {
            /// <summary>
            ///     Provides references to all functional tests
            /// </summary>
            private AllReferences FunctionalTests { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            public ImplementedWithNoFunctionalTestVisitor(Specification specification)
            {
                FunctionalTests = new AllReferences();
                foreach (Dictionary dictionary in EFSSystem.INSTANCE.Dictionaries)
                {
                    FunctionalTests.Initialize(dictionary);
                }
            }

            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                if (paragraph.getImplementationStatus() == acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented)
                {
                    if (!FunctionalTests.TestedParagraphs.Contains(paragraph))
                    {
                        paragraph.AddInfo("Paragraph is implemented but has no associated functional test");
                    }
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Mark paragraphs that are implemented, but where there is no test to validate this implementation
        /// </summary>
        public void CheckImplementedWithNoFunctionalTest()
        {
            ImplementedWithNoFunctionalTestVisitor visitor = new ImplementedWithNoFunctionalTestVisitor(this);
            visitor.visit(this);
        }


        /// <summary>
        ///     Provides the paragraph which are not set as tested but where a functional test is present
        /// </summary>
        private class NotTestedWithFunctionalTestVisitor : Visitor
        {
            /// <summary>
            ///     Provides references to all functional tests
            /// </summary>
            private AllReferences FunctionalTests { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            public NotTestedWithFunctionalTestVisitor(Specification specification)
            {
                FunctionalTests = new AllReferences();
                foreach (Dictionary dictionary in EFSSystem.INSTANCE.Dictionaries)
                {
                    FunctionalTests.Initialize(dictionary);
                }
            }

            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                if (!paragraph.getTested())
                {
                    if (FunctionalTests.TestedParagraphs.Contains(paragraph))
                    {
                        paragraph.AddInfo(
                            "Paragraph is not marked as tested, but is already associated functional test(s)");
                    }
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Mark paragraphs that are implemented, but where there is no test to validate this implementation
        /// </summary>
        public void CheckNotTestedWithFunctionalTests()
        {
            NotTestedWithFunctionalTestVisitor visitor = new NotTestedWithFunctionalTestVisitor(this);
            visitor.visit(this);
        }

        /// <summary>
        ///     Provides the paragraph which are not marked as implemented but where implementation exists
        /// </summary>
        private class NotImplementedButImplementationExistsVisitor : Visitor
        {
            /// <summary>
            ///     Provides all ReqReferences
            /// </summary>
            private class AllReferences : Visitor
            {
                /// <summary>
                ///     Provides the list of references found
                /// </summary>
                public List<ReqRef> References { get; private set; }

                /// <summary>
                ///     Constructor
                /// </summary>
                public AllReferences()
                {
                    References = new List<ReqRef>();
                }

                public override void visit(Generated.ReqRef obj, bool visitSubNodes)
                {
                    References.Add((ReqRef) obj);

                    base.visit(obj, visitSubNodes);
                }

                /// <summary>
                ///     Do not visit test frames
                /// </summary>
                /// <param name="obj"></param>
                /// <param name="visitSubNodes"></param>
                public override void visit(Frame obj, bool visitSubNodes)
                {
                }

                /// <summary>
                ///     The set of paragraph which have are implemented
                /// </summary>
                public HashSet<Paragraph> ImplementedParagraphs { get; private set; }

                /// <summary>
                ///     Set up the TestedParagraphs cache
                /// </summary>
                /// <param name="dictionary">Initialises this class according to the dictionary provided</param>
                public void Initialize(Dictionary dictionary)
                {
                    visit(dictionary);

                    ImplementedParagraphs = new HashSet<Paragraph>();
                    foreach (ReqRef reqRef in References)
                    {
                        Paragraph paragraph = reqRef.Paragraph;
                        if (paragraph != null)
                        {
                            ImplementedParagraphs.Add(paragraph);
                        }
                    }
                }
            }

            /// <summary>
            ///     Provides references to all implementations
            /// </summary>
            private AllReferences Implementations { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            public NotImplementedButImplementationExistsVisitor(Specification specification)
            {
                Implementations = new AllReferences();
                foreach (Dictionary dictionary in EFSSystem.INSTANCE.Dictionaries)
                {
                    Implementations.Initialize(dictionary);
                }
            }

            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                if (paragraph.getImplementationStatus() != acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented)
                {
                    if (Implementations.ImplementedParagraphs.Contains(paragraph))
                    {
                        paragraph.AddInfo("Paragraph is not marked as implemented but has implementations related to it");
                    }
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Mark paragraphs where implementation exists, but that are not marked as implemented
        /// </summary>
        public void CheckNotImplementedButImplementationExists()
        {
            NotImplementedButImplementationExistsVisitor visitor = new NotImplementedButImplementationExistsVisitor(this);
            visitor.visit(this);
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                Chapter item = element as Chapter;
                if (item != null)
                {
                    appendChapters(item);
                }
            }
        }
    }
}