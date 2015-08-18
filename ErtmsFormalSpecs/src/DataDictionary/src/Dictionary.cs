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
using System.IO;
using DataDictionary.Generated;
using DataDictionary.Specification;
using Utils;
using XmlBooster;
using Chapter = DataDictionary.Specification.Chapter;
using ChapterRef = DataDictionary.Specification.ChapterRef;
using Frame = DataDictionary.Tests.Frame;
using FrameRef = DataDictionary.Tests.FrameRef;
using NameSpaceRef = DataDictionary.Types.NameSpaceRef;
using Paragraph = DataDictionary.Generated.Paragraph;
using RequirementSet = DataDictionary.Specification.RequirementSet;
using Rule = DataDictionary.Rules.Rule;
using ShortcutDictionary = DataDictionary.Shortcuts.ShortcutDictionary;
using StateMachine = DataDictionary.Types.StateMachine;
using SubSequence = DataDictionary.Tests.SubSequence;
using TranslationDictionary = DataDictionary.Tests.Translations.TranslationDictionary;
using Type = DataDictionary.Types.Type;

namespace DataDictionary
{
    public class Dictionary : Generated.Dictionary, ISubDeclarator, IFinder, IEnclosesNameSpaces, IHoldsParagraphs,
        IHoldsRequirementSets
    {
        /// <summary>
        ///     The file path associated to the dictionary
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        ///     Used to temporarily store the list of sub-namespaces
        /// </summary>
        private ArrayList _savedNameSpaces;

        /// <summary>
        ///     Used to temporarily store the list of test frames
        /// </summary>
        private ArrayList _savedTests;

        /// <summary>
        ///     Updates the dictionary contents before saving it
        /// </summary>
        private class Updater : Visitor
        {
            /// <summary>
            ///     Indicates if the update operation is performed before saving or after
            /// </summary>
            private readonly bool _beforeSave;

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="beforeSave"></param>
            public Updater(bool beforeSave)
            {
                _beforeSave = beforeSave;
            }

            public override void visit(Generated.Dictionary obj, bool visitSubNodes)
            {
                base.visit(obj, visitSubNodes);

                Dictionary dictionary = (Dictionary) obj;

                if (_beforeSave)
                {
                    dictionary.ClearTempFiles();
                    dictionary.allNameSpaceRefs().Clear();
                    dictionary.allTestRefs().Clear();

                    if (dictionary.allNameSpaces() != null)
                    {
                        foreach (Types.NameSpace subNameSpace in dictionary.allNameSpaces())
                        {
                            dictionary.appendNameSpaceRefs(referenceNameSpace(dictionary, subNameSpace));
                        }
                    }

                    if (dictionary.allTests() != null)
                    {
                        foreach (Frame frame in dictionary.allTests())
                        {
                            dictionary.appendTestRefs(referenceFrame(dictionary, frame));
                        }
                    }
                    dictionary.StoreInfo();
                }
                else
                {
                    dictionary.RestoreInfo();
                }
            }

            public override void visit(NameSpace obj, bool visitSubNodes)
            {
                base.visit(obj, visitSubNodes);

                Types.NameSpace nameSpace = (Types.NameSpace) obj;

                if (_beforeSave)
                {
                    nameSpace.ClearTempFiles();
                    nameSpace.allNameSpaceRefs().Clear();

                    if (nameSpace.allNameSpaces() != null)
                    {
                        foreach (Types.NameSpace subNameSpace in nameSpace.allNameSpaces())
                        {
                            nameSpace.appendNameSpaceRefs(referenceNameSpace(nameSpace, subNameSpace));
                        }
                    }
                    nameSpace.StoreInfo();
                }
                else
                {
                    nameSpace.RestoreInfo();
                }
            }

            public override void visit(Generated.Specification obj, bool visitSubNodes)
            {
                base.visit(obj, visitSubNodes);

                Specification.Specification specification = (Specification.Specification) obj;

                if (_beforeSave)
                {
                    specification.ClearTempFiles();
                    specification.allChapterRefs().Clear();

                    if (specification.allChapters() != null)
                    {
                        foreach (Chapter chapter in specification.allChapters())
                        {
                            specification.appendChapterRefs(referenceChapter(specification, chapter));
                        }
                    }
                    specification.StoreInfo();
                }
                else
                {
                    specification.RestoreInfo();
                }
            }

            private NameSpaceRef referenceNameSpace(ModelElement enclosing, Types.NameSpace nameSpace)
            {
                NameSpaceRef retVal = nameSpace.NameSpaceRef;

                if (retVal == null)
                {
                    retVal = (NameSpaceRef) acceptor.getFactory().createNameSpaceRef();
                }

                retVal.Name = nameSpace.Name;
                retVal.setFather(enclosing);
                retVal.SaveNameSpace(nameSpace);

                return retVal;
            }

            private FrameRef referenceFrame(ModelElement enclosing, Frame test)
            {
                FrameRef retVal = test.FrameRef;

                if (retVal == null)
                {
                    retVal = (FrameRef) acceptor.getFactory().createFrameRef();
                }

                retVal.Name = test.Name;
                retVal.setFather(enclosing);
                retVal.SaveFrame(test);

                return retVal;
            }

            private ChapterRef referenceChapter(ModelElement enclosing, Chapter chapter)
            {
                ChapterRef retVal = chapter.ChapterRef;

                if (retVal == null)
                {
                    retVal = (ChapterRef) acceptor.getFactory().createChapterRef();
                }

                retVal.Name = chapter.Name;
                retVal.setFather(enclosing);
                retVal.SaveChapter(chapter);

                return retVal;
            }

            /// <summary>
            ///     Removes the actions and expectation from translated steps because they may cause conflicts.
            ///     Remove obsolete comments
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Step obj, bool visitSubNodes)
            {
                Tests.Step step = (Tests.Step) obj;

                if (step.getObsoleteComment() == "")
                {
                    step.setObsoleteComment(null);
                }

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            ///     Ensure that empty comments are not stored in the XML file
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(BaseModelElement obj, bool visitSubNodes)
            {
                ICommentable commentable = obj as ICommentable;
                if (commentable != null)
                {
                    if (commentable.Comment == "")
                    {
                        commentable.Comment = null;
                    }
                }

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            ///     Remove obsolete fields from XML file
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(TestCase obj, bool visitSubNodes)
            {
                Tests.TestCase testCase = (Tests.TestCase) obj;

                if (testCase.getObsoleteComment() == "")
                {
                    testCase.setObsoleteComment(null);
                }

                base.visit(obj, visitSubNodes);
            }


            /// <summary>
            ///     Remove obsolete fields from XML file
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Paragraph obj, bool visitSubNodes)
            {
                Specification.Paragraph paragraph = (Specification.Paragraph) obj;

                if (paragraph.getObsoleteFunctionalBlockName() == "")
                {
                    paragraph.setObsoleteFunctionalBlockName(null);
                }

                if (paragraph.getObsoleteGuid() == "")
                {
                    paragraph.setObsoleteGuid(null);
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     The base path for accessing files of this dictionary
        /// </summary>
        public string BasePath
        {
            get
            {
                return Path.GetDirectoryName(FilePath) + Path.DirectorySeparatorChar +
                       Path.GetFileNameWithoutExtension(FilePath);
            }
        }

        /// <summary>
        ///     The dictionary watcher
        /// </summary>
        public DictionaryWatcher Watcher { get; set; }

        /// <summary>
        ///     Saves the dictionary according to its filename
        /// </summary>
        public void Save()
        {
            Util.DontNotify(() =>
            {
                if (Watcher != null)
                {
                    Watcher.StopWatching();
                }

                Updater updater = new Updater(true);
                updater.visit(this);

                VersionedWriter writer = new VersionedWriter(FilePath);
                unParse(writer, false);
                writer.Close();

                updater = new Updater(false);
                updater.visit(this);

                if (Watcher != null)
                {
                    Watcher.StartWatching();
                }
            });
        }

        /// <summary>
        ///     Used to store the list of sub-namespaces and test frames before saving the model
        /// </summary>
        public void StoreInfo()
        {
            _savedNameSpaces = new ArrayList();
            foreach (Types.NameSpace aNameSpace in allNameSpaces())
            {
                _savedNameSpaces.Add(aNameSpace);
            }
            allNameSpaces().Clear();

            _savedTests = new ArrayList();
            foreach (Frame aTest in allTests())
            {
                _savedTests.Add(aTest);
            }
            allTests().Clear();
        }

        /// <summary>
        ///     Used to restore the list of sub-namespaces and test frames, after having saved the model
        /// </summary>
        public void RestoreInfo()
        {
            setAllNameSpaces(new ArrayList());
            foreach (Types.NameSpace aNameSpace in _savedNameSpaces)
            {
                allNameSpaces().Add(aNameSpace);
                aNameSpace.RestoreInfo();
            }
            _savedNameSpaces.Clear();

            setAllTests(new ArrayList());
            foreach (Frame aTest in _savedTests)
            {
                allTests().Add(aTest);
            }
            _savedTests.Clear();
        }

        /// <summary>
        ///     The treenode name of the dictionary
        /// </summary>
        public override string Name
        {
            get
            {
                string retVal = FilePath;

                int index = retVal.LastIndexOf('\\');
                if (index > 0)
                {
                    retVal = retVal.Substring(index + 1);
                }

                index = retVal.LastIndexOf('.');
                if (index > 0)
                {
                    retVal = retVal.Substring(0, index);
                }

                return retVal;
            }
            set { }
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();

            foreach (Types.NameSpace nameSpace in NameSpaces)
            {
                ISubDeclaratorUtils.AppendNamable(this, nameSpace);
            }
        }

        /// <summary>
        ///     Provides the list of declared elements in this Dictionary
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements { get; set; }

        /// <summary>
        ///     Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<INamable> retVal)
        {
            ISubDeclaratorUtils.Find(this, name, retVal);
        }

        /// <summary>
        ///     Finds all namable which match the full name provided
        /// </summary>
        /// <param name="fullname">The full name used to search the namable</param>
        public INamable FindByFullName(string fullname)
        {
            return OverallNamableFinder.INSTANCE.findByName(this, fullname);
        }

        /// <summary>
        ///     The specifications related to this dictionary
        /// </summary>
        public ArrayList Specifications
        {
            get
            {
                ArrayList retVal = allSpecifications();

                if (retVal == null)
                {
                    retVal = new ArrayList();
                }

                return retVal;
            }
        }

        /// <summary>
        ///     The rule disablings related to this rule set
        /// </summary>
        public ArrayList RuleDisablings
        {
            get
            {
                if (allRuleDisablings() == null)
                {
                    setAllRuleDisablings(new ArrayList());
                }
                return allRuleDisablings();
            }
        }

        /// <summary>
        ///     The test frames
        /// </summary>
        public ArrayList Tests
        {
            get
            {
                if (allTests() == null)
                {
                    setAllTests(new ArrayList());
                }
                return allTests();
            }
        }

        /// <summary>
        ///     Associates the types with their full name
        /// </summary>
        private Dictionary<string, Type> _definedTypes;

        public Dictionary<string, Type> DefinedTypes
        {
            get
            {
                if (_definedTypes == null)
                {
                    _definedTypes = new Dictionary<string, Type>();

                    foreach (Type type in EFSSystem.PredefinedTypes.Values)
                    {
                        _definedTypes.Add(type.FullName, type);
                    }

                    foreach (Type type in TypeFinder.INSTANCE.find(this))
                    {
                        // Ignore state machines which have no substate
                        if (type is StateMachine)
                        {
                            StateMachine stateMachine = type as StateMachine;

                            // Ignore state machines which have no substate
                            if (stateMachine.States.Count > 0)
                            {
                                // Ignore state machines which are not root state machines
                                if (stateMachine.EnclosingState == null)
                                {
                                    _definedTypes[type.FullName] = type;
                                }
                            }
                        }
                        else
                        {
                            _definedTypes[type.FullName] = type;
                        }
                    }
                }

                return _definedTypes;
            }
        }

        /// <summary>
        ///     Associates the types with their full name
        /// </summary>
        private Dictionary<Rule, RuleDisabling> _cachedRuleDisablings;

        public Dictionary<Rule, RuleDisabling> CachedRuleDisablings
        {
            get
            {
                if (_cachedRuleDisablings == null)
                {
                    _cachedRuleDisablings = new Dictionary<Rule, RuleDisabling>();

                    foreach (RuleDisabling ruleDisabling in RuleDisablings)
                    {
                        Rule disabledRule = EFSSystem.FindRule(ruleDisabling.getRule());
                        if (disabledRule != null)
                        {
                            _cachedRuleDisablings.Add(disabledRule, ruleDisabling);
                        }
                        else
                        {
                            ruleDisabling.AddError("Cannot find corresponding rule");
                        }
                    }
                }

                return _cachedRuleDisablings;
            }
        }

        /// <summary>
        ///     Clears the caches of this dictionary
        /// </summary>
        public override void ClearCache()
        {
            _definedTypes = null;
            _cachedRuleDisablings = null;
            DeclaredElements = null;
            _cache.Clear();
        }

        /// <summary>
        ///     Removes temporary files created for reference elements
        /// </summary>
        public void ClearTempFiles()
        {
            if (allNameSpaceRefs() != null)
            {
                foreach (NameSpaceRef aReferenceNameSpace in allNameSpaceRefs())
                {
                    aReferenceNameSpace.ClearTempFile();
                }
            }
            if (allTestRefs() != null)
            {
                foreach (FrameRef aReferenceTest in allTestRefs())
                {
                    aReferenceTest.ClearTempFile();
                }
            }
        }

        /// <summary>
        ///     The cache for types / namespace + name
        /// </summary>
        private readonly Dictionary<Types.NameSpace, Dictionary<string, Type>> _cache =
            new Dictionary<Types.NameSpace, Dictionary<string, Type>>();

        /// <summary>
        ///     Provides the type associated to the name
        /// </summary>
        /// <param name="nameSpace">the namespace in which the name should be found</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Type FindType(Types.NameSpace nameSpace, string name)
        {
            Type retVal = null;

            if (name != null)
            {
                if (nameSpace != null)
                {
                    if (!_cache.ContainsKey(nameSpace))
                    {
                        _cache[nameSpace] = new Dictionary<string, Type>();
                    }
                    Dictionary<string, Type> subCache = _cache[nameSpace];

                    if (!subCache.ContainsKey(name))
                    {
                        Type tmp = null;

                        if (!Utils.Util.isEmpty(name))
                        {
                            tmp = nameSpace.findTypeByName(name);

                            if (tmp == null)
                            {
                                if (DefinedTypes.ContainsKey(name))
                                {
                                    tmp = DefinedTypes[name];
                                }
                            }

                            if (tmp == null && DefinedTypes.ContainsKey("Default." + name))
                            {
                                tmp = DefinedTypes["Default." + name];
                            }
                        }

                        if (tmp == null)
                        {
                            Log.Error("Cannot find type named " + name);
                        }

                        subCache[name] = tmp;
                    }

                    retVal = subCache[name];
                }
                else
                {
                    if (DefinedTypes.ContainsKey(name))
                    {
                        retVal = DefinedTypes[name];
                    }
                    else if (DefinedTypes.ContainsKey("Default." + name))
                    {
                        retVal = DefinedTypes["Default." + name];
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the namespace whose name matches the name provided
        /// </summary>
        public Types.NameSpace FindNameSpace(string name)
        {
            Types.NameSpace retVal = (Types.NameSpace) NamableUtils.FindByName(name, NameSpaces);

            return retVal;
        }

        /// <summary>
        ///     Provides the rule according to its fullname
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public Rule FindRule(string fullName)
        {
            Rule retVal = null;

            foreach (Rule rule in AllRules)
            {
                if (rule.FullName.CompareTo(fullName) == 0)
                {
                    retVal = rule;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Should be called when the file system changes
        /// </summary>
        public delegate void HandleFsChange();

        /// <summary>
        ///     The event raised when the file system changed
        /// </summary>
        public event HandleFsChange FileSystemChange;

        /// <summary>
        ///     Handles a file system change
        /// </summary>
        protected virtual void OnFileSystemChange()
        {
            if (FileSystemChange != null)
            {
                FileSystemChange();
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Dictionary()
        {
            FinderRepository.INSTANCE.Register(this);
        }

        /// <summary>
        ///     Provides the namespaces defined in this dictionary
        /// </summary>
        public ArrayList NameSpaces
        {
            get
            {
                if (allNameSpaces() == null)
                {
                    setAllNameSpaces(new ArrayList());
                }
                return allNameSpaces();
            }
        }


        /// <summary>
        ///     Provides the set of paragraphs implemented by this set of rules
        /// </summary>
        /// <returns></returns>
        public HashSet<Specification.Paragraph> ImplementedParagraphs
        {
            get { return ImplementedParagraphsFinder.INSTANCE.find(this); }
        }

        /// <summary>
        ///     Recursively provides the rules stored in this dictionary
        /// </summary>
        /// <returns></returns>
        public HashSet<Rule> AllRules
        {
            get { return RuleFinder.INSTANCE.find(this); }
        }

        /// <summary>
        ///     Recursively provides the req related stored in this dictionary
        /// </summary>
        /// <returns></returns>
        public HashSet<ReqRelated> AllReqRelated
        {
            get { return ReqRelatedFinder.INSTANCE.find(this); }
        }

        /// <summary>
        ///     Recursively provides the req related stored in this dictionary
        /// </summary>
        /// <returns></returns>
        public HashSet<ReqRelated> ImplementedReqRelated
        {
            get
            {
                HashSet<ReqRelated> retVal = new HashSet<ReqRelated>();

                foreach (ReqRelated reqRelated in AllReqRelated)
                {
                    if (reqRelated.getImplemented())
                    {
                        retVal.Add(reqRelated);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Recursively provides the rules stored in this dictionary
        /// </summary>
        /// <returns></returns>
        public HashSet<Rule> ImplementedRules
        {
            get
            {
                HashSet<Rule> retVal = new HashSet<Rule>();

                foreach (Rule rule in AllRules)
                {
                    if (rule.getImplemented())
                    {
                        retVal.Add(rule);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Checks the rules stored in the dictionary
        /// </summary>
        public void CheckRules()
        {
            Util.DontNotify(() =>
            {
                try
                {
                    // Rebuilds everything
                    EFSSystem.Compiler.Compile_Synchronous(EFSSystem.ShouldRebuild);
                    EFSSystem.ShouldRebuild = false;

                    MarkingHistory.PerformMark(() =>
                    {
                        // Check rules
                        RuleCheckerVisitor visitor = new RuleCheckerVisitor(this);
                        visitor.visit(this, true);
                    });
                }
                catch (Exception)
                {
                }
            });
        }

        /// <summary>
        ///     Checks the model for unused element
        /// </summary>
        public void CheckDeadModel()
        {
            Util.DontNotify(() =>
            {
                // Rebuilds everything
                EFSSystem.Compiler.Compile_Synchronous(EFSSystem.ShouldRebuild);
                EFSSystem.ShouldRebuild = false;

                MarkingHistory.PerformMark(() =>
                {
                    // Check dead model
                    UsageChecker visitor = new UsageChecker(this);
                    visitor.visit(this, true);
                });
            });
        }

        private class UnimplementedItemVisitor : Visitor
        {
            public override void visit(Generated.ReqRelated obj, bool visitSubNodes)
            {
                ReqRelated reqRelated = (ReqRelated) obj;

                if (!reqRelated.getImplemented())
                {
                    reqRelated.AddInfo("Implementation not complete");
                }

                base.visit(obj, visitSubNodes);
            }
        }


        /// <summary>
        ///     Marks all unimplemented test cases stored in the dictionary
        /// </summary>
        public void MarkUnimplementedTests()
        {
            UnimplementedTestVisitor visitor = new UnimplementedTestVisitor();
            visitor.visit(this, true);
        }

        private class UnimplementedTestVisitor : Visitor
        {
            public override void visit(TestCase obj, bool visitSubNodes)
            {
                Tests.TestCase testCase = (Tests.TestCase) obj;
                if (!testCase.getImplemented())
                {
                    testCase.AddInfo("Unimplemented test case");
                }
                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Marks all unimplemented test cases stored in the dictionary
        /// </summary>
        public void MarkNotTranslatedTests()
        {
            NotTranslatedTestVisitor visitor = new NotTranslatedTestVisitor();
            visitor.visit(this, true);
        }

        private class NotTranslatedTestVisitor : Visitor
        {
            public override void visit(Step obj, bool visitSubNodes)
            {
                Tests.Step step = (Tests.Step) obj;
                if (step.getTranslationRequired() && !step.getTranslated())
                {
                    step.AddInfo("Not translated step");
                }
                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Marks all unimplemented test cases stored in the dictionary
        /// </summary>
        public void MarkNotImplementedTranslations()
        {
            NotImplementedTranslationVisitor visitor = new NotImplementedTranslationVisitor();
            visitor.visit(this, true);
        }

        private class NotImplementedTranslationVisitor : Visitor
        {
            public override void visit(Translation obj, bool visitSubNodes)
            {
                Tests.Translations.Translation translation = (Tests.Translations.Translation) obj;
                if (!translation.getImplemented())
                {
                    translation.AddInfo("Not implemented translation");
                }
                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Marks all unimplemented rules stored in the dictionary
        /// </summary>
        public void MarkUnimplementedItems()
        {
            UnimplementedItemVisitor visitor = new UnimplementedItemVisitor();
            visitor.visit(this, true);
        }

        private class NotVerifiedRuleVisitor : Visitor
        {
            public override void visit(Generated.Rule obj, bool visitSubNodes)
            {
                Rule rule = (Rule) obj;

                if (!rule.getVerified())
                {
                    rule.AddInfo("Rule not verified");
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Marks all not verified rules stored in the dictionary
        /// </summary>
        public void MarkNotVerifiedRules()
        {
            NotVerifiedRuleVisitor visitor = new NotVerifiedRuleVisitor();
            visitor.visit(this, true);
        }

        /// <summary>
        ///     Creates a dictionary with pairs paragraph - list of its implementations
        /// </summary>
        private class ParagraphReqRefFinder : Visitor
        {
            private Dictionary<Specification.Paragraph, List<ReqRef>> _paragraphsReqRefs;

            public Dictionary<Specification.Paragraph, List<ReqRef>> ParagraphsReqRefs
            {
                get
                {
                    if (_paragraphsReqRefs == null)
                    {
                        _paragraphsReqRefs = new Dictionary<Specification.Paragraph, List<ReqRef>>();
                    }
                    return _paragraphsReqRefs;
                }
            }

            public override void visit(IXmlBBase obj, bool visitSubNodes)
            {
                ReqRef reqRef = obj as ReqRef;
                if (reqRef != null)
                {
                    if (reqRef.Paragraph != null)
                    {
                        if (!ParagraphsReqRefs.ContainsKey(reqRef.Paragraph))
                        {
                            ParagraphsReqRefs.Add(reqRef.Paragraph, new List<ReqRef>());
                        }
                        _paragraphsReqRefs[reqRef.Paragraph].Add(reqRef);
                    }
                }
            }
        }

        public Dictionary<Specification.Paragraph, List<ReqRef>> ParagraphsReqRefs
        {
            get
            {
                ParagraphReqRefFinder visitor = new ParagraphReqRefFinder();
                visitor.visit(this, true);
                return visitor.ParagraphsReqRefs;
            }
        }

        /// <summary>
        ///     Executes the test cases for this test sequence
        /// </summary>
        /// <returns>the number of failed test frames</returns>
        public int ExecuteAllTests()
        {
            int retVal = 0;

            // Compile everything
            EFSSystem.Compiler.Compile_Synchronous(EFSSystem.ShouldRebuild);
            EFSSystem.ShouldRebuild = false;

            foreach (Frame frame in Tests)
            {
                const bool ensureCompilationDone = false;
                int failedFrames = frame.ExecuteAllTests(ensureCompilationDone);
                if (failedFrames > 0)
                {
                    retVal += 1;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The sub sequences of this data dictionary
        /// </summary>
        public List<SubSequence> SubSequences
        {
            get
            {
                List<SubSequence> retVal = new List<SubSequence>();

                foreach (Frame frame in Tests)
                {
                    frame.FillSubSequences(retVal);
                }
                retVal.Sort();

                return retVal;
            }
        }

        /// <summary>
        ///     Finds a test case whose name corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SubSequence FindSubSequence(string name)
        {
            return (SubSequence) NamableUtils.FindByName(name, SubSequences);
        }

        /// <summary>
        ///     Provides the frame which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Frame FindFrame(string name)
        {
            return (Frame) NamableUtils.FindByName(name, Tests);
        }

        /// <summary>
        ///     The translation dictionary.
        /// </summary>
        public TranslationDictionary TranslationDictionary
        {
            get
            {
                if (getTranslationDictionary() == null)
                {
                    setTranslationDictionary(acceptor.getFactory().createTranslationDictionary());
                }

                return (TranslationDictionary) getTranslationDictionary();
            }
        }

        /// <summary>
        ///     The translation dictionary.
        /// </summary>
        public ShortcutDictionary ShortcutsDictionary
        {
            get
            {
                if (getShortcutDictionary() == null)
                {
                    ShortcutDictionary dictionary =
                        (ShortcutDictionary) acceptor.getFactory().createShortcutDictionary();
                    dictionary.Name = Name;
                    setShortcutDictionary(dictionary);
                }

                return (ShortcutDictionary) getShortcutDictionary();
            }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="element"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                Types.NameSpace item = element as Types.NameSpace;
                if (item != null)
                {
                    appendNameSpaces(item);
                }
            }
            {
                RuleDisabling item = element as RuleDisabling;
                if (item != null)
                {
                    appendRuleDisablings(item);
                }
            }
            {
                Frame item = element as Frame;
                if (item != null)
                {
                    appendTests(item);
                }
            }
        }

        public ICollection<Specification.Paragraph> AllParagraphs
        {
            get
            {
                ICollection<Specification.Paragraph> retVal = new HashSet<Specification.Paragraph>();

                foreach (Specification.Specification specification in Specifications)
                {
                    foreach (Specification.Paragraph paragraph in specification.AllParagraphs)
                    {
                        retVal.Add(paragraph);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Gets all paragraphs from a dictionary
        /// </summary>
        /// <param name="paragraphs"></param>
        public void GetParagraphs(List<Specification.Paragraph> paragraphs)
        {
            foreach (Specification.Specification specification in Specifications)
            {
                specification.GetParagraphs(paragraphs);
            }
        }

        public ICollection<Specification.Paragraph> ApplicableParagraphs
        {
            get
            {
                ICollection<Specification.Paragraph> retVal = new HashSet<Specification.Paragraph>();

                foreach (Specification.Specification specification in Specifications)
                {
                    foreach (Specification.Paragraph paragraph in specification.ApplicableParagraphs)
                    {
                        retVal.Add(paragraph);
                    }
                }

                return retVal;
            }
        }

        public ICollection<Specification.Paragraph> MoreInformationNeeded
        {
            get
            {
                ICollection<Specification.Paragraph> retVal = new HashSet<Specification.Paragraph>();

                foreach (Specification.Specification specification in Specifications)
                {
                    foreach (Specification.Paragraph paragraph in specification.MoreInformationNeeded)
                    {
                        retVal.Add(paragraph);
                    }
                }

                return retVal;
            }
        }

        public ICollection<Specification.Paragraph> SpecIssues
        {
            get
            {
                ICollection<Specification.Paragraph> retVal = new HashSet<Specification.Paragraph>();

                foreach (Specification.Specification specification in Specifications)
                {
                    foreach (Specification.Paragraph paragraph in specification.SpecIssues)
                    {
                        retVal.Add(paragraph);
                    }
                }

                return retVal;
            }
        }

        public ICollection<Specification.Paragraph> DesignChoices
        {
            get
            {
                ICollection<Specification.Paragraph> retVal = new HashSet<Specification.Paragraph>();

                foreach (Specification.Specification specification in Specifications)
                {
                    foreach (Specification.Paragraph paragraph in specification.DesignChoices)
                    {
                        retVal.Add(paragraph);
                    }
                }

                return retVal;
            }
        }

        public ICollection<Specification.Paragraph> OnlyComments
        {
            get
            {
                ICollection<Specification.Paragraph> retVal = new HashSet<Specification.Paragraph>();

                foreach (Specification.Specification specification in Specifications)
                {
                    foreach (Specification.Paragraph paragraph in specification.OnlyComments)
                    {
                        retVal.Add(paragraph);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the list of requirement sets in the system
        /// </summary>
        public List<RequirementSet> RequirementSets
        {
            get
            {
                List<RequirementSet> retVal = new List<RequirementSet>();

                if (allRequirementSets() != null)
                {
                    foreach (RequirementSet requirementSet in allRequirementSets())
                    {
                        retVal.Add(requirementSet);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the requirement set whose name corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <param name="create">Indicates that the requirement set should be created if it does not exists</param>
        /// <returns></returns>
        public RequirementSet findRequirementSet(string name, bool create)
        {
            RequirementSet retVal = null;

            foreach (RequirementSet requirementSet in RequirementSets)
            {
                if (requirementSet.Name == name)
                {
                    retVal = requirementSet;
                    break;
                }
            }

            if (retVal == null && create)
            {
                retVal = (RequirementSet) acceptor.getFactory().createRequirementSet();
                retVal.Name = name;
                appendRequirementSets(retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     Adds a new requirement set to this list of requirement sets
        /// </summary>
        /// <param name="requirementSet"></param>
        public void AddRequirementSet(RequirementSet requirementSet)
        {
            appendRequirementSets(requirementSet);
        }

        /// <summary>
        ///     Either provides the requested namespace or creates it if it cannot be found
        ///     This method can create many levels of nested namespaces
        /// </summary>
        /// <param name="levels">The name of the namespace, with the levels separated into separate Strings</param>
        /// <param name="initialDictionary">The dictionary the namespace structure is being copied form</param>
        /// <returns></returns>
        public Types.NameSpace GetNameSpaceUpdate(String[] levels, Dictionary initialDictionary)
        {
            Types.NameSpace retVal = null;

            if (levels.Length > 0)
            {
                retVal = FindNameSpace(levels[0]);
                Types.NameSpace initialNameSpace = initialDictionary.FindNameSpace(levels[0]);

                if (retVal == null)
                {
                    retVal = (Types.NameSpace) acceptor.getFactory().createNameSpace();
                    retVal.setName(levels[0]);
                    appendNameSpaces(retVal);

                    // set the updates link for the new namespace
                    retVal.setUpdates(initialNameSpace.Guid);
                }

                for (int index = 1; index < levels.Length; index++)
                {
                    initialNameSpace = initialNameSpace.findNameSpaceByName((levels[index]));
                    retVal = retVal.FindOrCreateNameSpaceUpdate(levels[index], initialNameSpace);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Checks the entire chain of updates, to see if the dictionary is an update of this one
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public bool IsUpdatedBy(Dictionary dictionary)
        {
            bool retVal = false;

            bool updates = true;
            while (updates)
            {
                if (dictionary.Updates == null)
                {
                    updates = false;
                }
                else if (dictionary.Updates == this)
                {
                    retVal = true;
                    break;
                }

                dictionary = (Dictionary) dictionary.Updates;
            }

            return retVal;
        }

        public void MergeUpdate()
        {
            MergeUpdateVisitor mergeVisitor = new MergeUpdateVisitor();
            foreach (NameSpace nameSpace in NameSpaces)
            {
                mergeVisitor.visit(nameSpace);
            }

            foreach (Specification.Specification specification in Specifications)
            {
                mergeVisitor.visit(specification);
            }
        }

        private class MergeUpdateVisitor : Visitor
        {
            /// <summary>
            ///     Override for visiting a model element. If the model element is not a namespace,
            ///     but its direct enclosing is a namespace, the element is imported and the visitor does
            ///     not go further down the subnodes.
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(BaseModelElement obj, bool visitSubNodes)
            {
                ModelElement element = obj as ModelElement;
                if (element != null && !(element is NameSpace))
                {
                    element.Merge();
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     The name of the requirement set for functional blocs
        /// </summary>
        public const string FunctionalBlockName = "Functional blocs";

        /// <summary>
        ///     The name of the requirement set for scoping information
        /// </summary>
        public const string ScopeName = "Scope";

        /// <summary>
        ///     Provides the set of covered requirements by the tests
        /// </summary>
        /// <returns></returns>
        public HashSet<Specification.Paragraph> CoveredRequirements()
        {
            HashSet<Specification.Paragraph> retVal = new HashSet<Specification.Paragraph>();
            ICollection<Specification.Paragraph> applicableParagraphs = ApplicableParagraphs;
            Dictionary<Specification.Paragraph, List<ReqRef>> paragraphsReqRefDictionary = ParagraphsReqRefs;

            foreach (Specification.Paragraph paragraph in applicableParagraphs)
            {
                bool implemented = paragraph.getImplementationStatus() ==
                                   acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented;
                bool tested = false;
                if (implemented)
                {
                    if (paragraphsReqRefDictionary.ContainsKey(paragraph))
                    {
                        List<ReqRef> implementations = paragraphsReqRefDictionary[paragraph];
                        foreach (ReqRef reqRef in implementations)
                        {
                            ReqRelated reqRelated = reqRef.Enclosing as ReqRelated;
                            if (reqRelated is TestCase && reqRelated.ImplementationCompleted)
                            {
                                tested = true;
                            }
                        }
                    }
                }

                if (implemented && tested)
                {
                    retVal.Add(paragraph);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Creates the status message
        /// </summary>
        /// <returns>the status string for the selected element</returns>
        public override string CreateStatusMessage()
        {
            string retVal = base.CreateStatusMessage();

            List<Specification.Paragraph> paragraphs = new List<Specification.Paragraph>();
            GetParagraphs(paragraphs);

            retVal += Specification.Paragraph.CreateParagraphSetStatus(paragraphs);

            return retVal;
        }
    }
}