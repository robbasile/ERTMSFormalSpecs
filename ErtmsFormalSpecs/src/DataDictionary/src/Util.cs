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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using DataDictionary.Generated;
using log4net;
using Utils;
using XmlBooster;
using Chapter = DataDictionary.Specification.Chapter;
using ChapterRef = DataDictionary.Specification.ChapterRef;
using Frame = DataDictionary.Tests.Frame;
using FrameRef = DataDictionary.Tests.FrameRef;
using NameSpaceRef = DataDictionary.Types.NameSpaceRef;
using RequirementSet = DataDictionary.Specification.RequirementSet;
using TranslationDictionary = DataDictionary.Tests.Translations.TranslationDictionary;

namespace DataDictionary
{
    public class Util
    {
        /// <summary>
        ///     The Logger
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     Indicates that the files should be locked for edition when opened
        /// </summary>
        public static bool PleaseLockFiles = true;

        /// <summary>
        ///     Updates the dictionary contents
        /// </summary>
        public class Updater : Cleaner
        {
            /// <summary>
            ///     Indicates that GUID should be updated
            /// </summary>
            private bool UpdateGuid { get; set; }

            /// <summary>
            ///     Indicates obsolete versions of model files should be updated
            /// </summary>
            private bool ConvertObsoleteFile { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="updateGuid"></param>
            /// <param name="convertObsoleteFile"></param>
            public Updater(bool updateGuid, bool convertObsoleteFile)
            {
                UpdateGuid = updateGuid;
                ConvertObsoleteFile = convertObsoleteFile;
            }

            /// <summary>
            ///     Ensures that all elements have a Guid
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(BaseModelElement obj, bool visitSubNodes)
            {
                ModelElement element = (ModelElement) obj;

                if (UpdateGuid)
                {
                    // Side effect : creates a new Guid if it is empty
                    // ReSharper disable once UnusedVariable
                    string guid = element.Guid;
                }

                // Make sure that the update information is consistent
                if (!String.IsNullOrEmpty(element.Guid))
                {
                    if (element.Updates != null)
                    {
                        element.SetUpdateInformation(element.Updates);
                    }
                }

                IExpressionable expressionable = obj as IExpressionable;
                if (expressionable != null && ConvertObsoleteFile)
                {
                    UpdateExpressionable(expressionable);
                }

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            ///     Indicates that a character may belong to an identifier
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            private bool belongsToIdentifier(char c)
            {
                bool retVal = Char.IsLetterOrDigit(c) || c == '.' || c == '_';

                return retVal;
            }

            /// <summary>
            ///     Indicates that a character is a white space
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            private bool isWhiteSpace(char c)
            {
                bool retVal = c == ' ' || c == '\t' || c == '\n';

                return retVal;
            }

            /// <summary>
            ///     Provides the identifier, if any at the position in the expression
            /// </summary>
            /// <param name="expression"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            private string Identifier(string expression, int index)
            {
                string retVal = "";

                while (index < expression.Length && isWhiteSpace(expression[index]))
                {
                    index = index + 1;
                }

                while (index < expression.Length && belongsToIdentifier(expression[index]))
                {
                    retVal = retVal + expression[index];
                    index = index + 1;
                }

                return retVal;
            }

            /// <summary>
            ///     Updates the expressionable, according to the grammar changes
            /// </summary>
            /// <param name="expressionable"></param>
            private void UpdateExpressionable(IExpressionable expressionable)
            {
                string expression = expressionable.ExpressionText;

                expression = Replace(expression, "USING", "USING X IN");
                expression = Replace(expression, "THERE_IS_IN", "THERE_IS X IN");
                expression = Replace(expression, "LAST_IN", "LAST X IN");
                expression = Replace(expression, "FIRST_IN", "FIRST X IN");
                expression = Replace(expression, "FORALL_IN", "FORALL X IN");
                expression = Replace(expression, "COUNT", "COUNT X IN");

                expressionable.ExpressionText = expression;
            }

            /// <summary>
            ///     Replaces an initial expression from 'expression' by the 'replacementValue'
            ///     if the exclusiong pattern is not found after the 'initial expression'
            /// </summary>
            /// <param name="expression"></param>
            /// <param name="initialExpression"></param>
            /// <param name="replacementValue"></param>
            /// <returns></returns>
            private string Replace(string expression, string initialExpression, string replacementValue)
            {
                string retVal = expression;

                int i = 0;
                while (i >= 0)
                {
                    i = retVal.IndexOf(initialExpression, i);
                    if (i >= 0)
                    {
                        if ((i == 0) ||
                            (i > 0 && i < retVal.Length - 1 && !belongsToIdentifier(retVal[i - 1]) &&
                             !belongsToIdentifier(retVal[i + initialExpression.Length])))
                        {
                            bool replace = false;
                            string identifier = Identifier(retVal, i + initialExpression.Length);
                            if (string.IsNullOrEmpty(identifier) || identifier == "IN")
                            {
                                replace = true;
                            }
                            else
                            {
                                int j = expression.IndexOf(identifier, i + initialExpression.Length);
                                string inKeyword = Identifier(retVal, j + identifier.Length);
                                if (string.IsNullOrEmpty(inKeyword) || inKeyword != "IN")
                                {
                                    replace = true;
                                }
                            }

                            if (replace)
                            {
                                retVal = retVal.Substring(0, i) + replacementValue +
                                         retVal.Substring(i + initialExpression.Length);
                            }
                            i = i + 1;
                        }
                        else
                        {
                            i = i + 1;
                        }
                    }
                }

                return retVal;
            }

            /// <summary>
            ///     Update references to paragraphs
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Generated.ReqRef obj, bool visitSubNodes)
            {
                ReqRef reqRef = (ReqRef) obj;

                if (UpdateGuid)
                {
                    Specification.Paragraph paragraph = reqRef.Paragraph;
                    if (paragraph != null)
                    {
                        // Updates the paragraph Guid
                        if (paragraph.Guid != reqRef.getId())
                        {
                            reqRef.setId(paragraph.getGuid());
                        }

                        // Updates the specification Guid
                        Specification.Specification specification =
                            EnclosingFinder<Specification.Specification>.find(paragraph);
                        if (specification.Guid != reqRef.getSpecId())
                        {
                            reqRef.setSpecId(specification.Guid);
                        }
                    }
                }

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            ///     Replaces the paragraph scope by the corresponding flags
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Paragraph obj, bool visitSubNodes)
            {
                Specification.Paragraph paragraph = (Specification.Paragraph) obj;

                // WARNING : This phase is completed by the next phase to place all requirement in requirement sets
                // Ensures the scope is located in the flags
                switch (paragraph.getObsoleteScope())
                {
                    case acceptor.Paragraph_scope.aOBU:
                        paragraph.setObsoleteScopeOnBoard(true);
                        break;

                    case acceptor.Paragraph_scope.aTRACK:
                        paragraph.setObsoleteScopeTrackside(true);
                        break;

                    case acceptor.Paragraph_scope.aOBU_AND_TRACK:
                    case acceptor.Paragraph_scope.defaultParagraph_scope:
                        paragraph.setObsoleteScopeOnBoard(true);
                        paragraph.setObsoleteScopeTrackside(true);
                        break;

                    case acceptor.Paragraph_scope.aROLLING_STOCK:
                        paragraph.setObsoleteScopeRollingStock(true);
                        break;
                }
                paragraph.setObsoleteScope(acceptor.Paragraph_scope.aFLAGS);

                // WARNING : do not remove the preceding phase since it still required for previous versions of EFS files
                // Based on the flag information, place the requirements in their corresponding requirement set
                // STM was never used, this information is discarded
                RequirementSet scope = paragraph.Dictionary.findRequirementSet(Dictionary.ScopeName, true);

                if (paragraph.getObsoleteScopeOnBoard())
                {
                    RequirementSet onBoard = scope.findRequirementSet(RequirementSet.OnboardScopeName, false);
                    if (onBoard == null)
                    {
                        onBoard = scope.findRequirementSet(RequirementSet.OnboardScopeName, true);
                        onBoard.setRecursiveSelection(false);
                        onBoard.setDefault(true);
                    }
                    paragraph.AppendToRequirementSet(onBoard);
                    paragraph.setObsoleteScopeOnBoard(false);
                }

                if (paragraph.getObsoleteScopeTrackside())
                {
                    RequirementSet trackSide = scope.findRequirementSet(RequirementSet.TracksideScopeName, false);
                    if (trackSide == null)
                    {
                        trackSide = scope.findRequirementSet(RequirementSet.TracksideScopeName, true);
                        trackSide.setRecursiveSelection(false);
                        trackSide.setDefault(true);
                    }
                    paragraph.AppendToRequirementSet(trackSide);
                    paragraph.setObsoleteScopeTrackside(false);
                }

                if (paragraph.getObsoleteScopeRollingStock())
                {
                    RequirementSet rollingStock = scope.findRequirementSet(RequirementSet.RollingStockScopeName,
                        false);
                    if (rollingStock == null)
                    {
                        rollingStock = scope.findRequirementSet(RequirementSet.RollingStockScopeName, true);
                        rollingStock.setRecursiveSelection(false);
                        rollingStock.setDefault(false);
                    }
                    paragraph.AppendToRequirementSet(rollingStock);
                    paragraph.setObsoleteScopeRollingStock(false);
                }

                // Updates the functional block information based on the FunctionalBlockName field
                if (!string.IsNullOrEmpty(paragraph.getObsoleteFunctionalBlockName()))
                {
                    RequirementSet allFunctionalBlocks =
                        paragraph.Dictionary.findRequirementSet(Dictionary.FunctionalBlockName, true);
                    RequirementSet functionalBlock =
                        allFunctionalBlocks.findRequirementSet(paragraph.getObsoleteFunctionalBlockName(), true);
                    functionalBlock.setRecursiveSelection(true);
                    functionalBlock.setDefault(false);
                    paragraph.AppendToRequirementSet(functionalBlock);
                    paragraph.setObsoleteFunctionalBlockName(null);
                }

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            ///     Updates the state machine : initial state has been moved to the default value
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(StateMachine obj, bool visitSubNodes)
            {
                Types.StateMachine stateMachine = (Types.StateMachine) obj;

                if (string.IsNullOrEmpty(stateMachine.getDefault()))
                {
                    stateMachine.setDefault(stateMachine.getInitialState());
                }
                stateMachine.setInitialState(null);

                base.visit(obj, visitSubNodes);
            }


            /// <summary>
            ///     Updates the step : comment has been moved
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Step obj, bool visitSubNodes)
            {
                Tests.Step step = (Tests.Step) obj;

                if (!string.IsNullOrEmpty(step.getObsoleteComment()))
                {
                    if (string.IsNullOrEmpty(step.getComment()))
                    {
                        step.setComment(step.getObsoleteComment());
                    }
                    else
                    {
                        step.setComment(step.getComment() + "\n" + step.getObsoleteComment());
                    }
                    step.setObsoleteComment(null);
                }

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            ///     Updates the step : comment has been moved
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Translation obj, bool visitSubNodes)
            {
                Tests.Translations.Translation translation = (Tests.Translations.Translation) obj;

                if (!string.IsNullOrEmpty(translation.getObsoleteComment()))
                {
                    if (string.IsNullOrEmpty(translation.getComment()))
                    {
                        translation.setComment(translation.getObsoleteComment());
                    }
                    else
                    {
                        translation.setComment(translation.getComment() + "\n" + translation.getObsoleteComment());
                    }
                    translation.setObsoleteComment(null);
                }

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            ///     Remove the obsolete comments
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(TestCase obj, bool visitSubNodes)
            {
                if (!string.IsNullOrEmpty(obj.getObsoleteComment()))
                {
                    if (string.IsNullOrEmpty(obj.getComment()))
                    {
                        obj.setComment(obj.getObsoleteComment());
                        obj.setObsoleteComment(null);
                    }
                    else
                    {
                        if (obj.getComment() == obj.getObsoleteComment())
                        {
                            obj.setObsoleteComment(null);
                        }
                        else
                        {
                            throw new Exception("Cannot mix both comments...");
                        }
                    }
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Updates the dictionary contents
        /// </summary>
        private class LoadDepends : Visitor
        {
            /// <summary>
            ///     Indicates that the files should be locked
            /// </summary>
            private bool LockFiles { get; set; }

            /// <summary>
            ///     Indicates that errors can occur during load, for instance, for comparison purposes
            /// </summary>
            private bool AllowErrorsDuringLoad
            {
                get { return ErrorsDuringLoad != null; }
            }

            /// <summary>
            ///     The errors that occured during the load of the file
            /// </summary>
            private List<ElementLog> ErrorsDuringLoad { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="lockFiles"></param>
            /// <param name="errors"></param>
            public LoadDepends(bool lockFiles, List<ElementLog> errors)
            {
                LockFiles = lockFiles;
                ErrorsDuringLoad = errors;
            }

            public override void visit(Generated.Dictionary obj, bool visitSubNodes)
            {
                Dictionary dictionary = (Dictionary) obj;

                if (dictionary.allNameSpaceRefs() != null)
                {
                    foreach (NameSpaceRef nameSpaceRef in dictionary.allNameSpaceRefs())
                    {
                        Types.NameSpace nameSpace = nameSpaceRef.LoadNameSpace(LockFiles, AllowErrorsDuringLoad);
                        if (nameSpace != null)
                        {
                            dictionary.appendNameSpaces(nameSpace);
                            nameSpace.NameSpaceRef = nameSpaceRef;
                        }
                        else
                        {
                            ErrorsDuringLoad.Add(new ElementLog(ElementLog.LevelEnum.Error,
                                "Cannot load file " + nameSpaceRef.FileName));
                        }
                    }
                    dictionary.allNameSpaceRefs().Clear();
                }
                if (dictionary.allTestRefs() != null)
                {
                    foreach (FrameRef testRef in dictionary.allTestRefs())
                    {
                        Frame frame = testRef.LoadFrame(LockFiles, AllowErrorsDuringLoad);
                        if (frame != null)
                        {
                            dictionary.appendTests(frame);
                            frame.FrameRef = testRef;
                        }
                        else
                        {
                            ErrorsDuringLoad.Add(new ElementLog(ElementLog.LevelEnum.Error,
                                "Cannot load file " + testRef.FileName));
                        }
                    }
                    dictionary.allTestRefs().Clear();
                }

                base.visit(obj, visitSubNodes);
            }

            public override void visit(NameSpace obj, bool visitSubNodes)
            {
                Types.NameSpace nameSpace = (Types.NameSpace) obj;

                if (nameSpace.allNameSpaceRefs() != null)
                {
                    foreach (NameSpaceRef nameSpaceRef in nameSpace.allNameSpaceRefs())
                    {
                        Types.NameSpace subNameSpace = nameSpaceRef.LoadNameSpace(LockFiles, AllowErrorsDuringLoad);
                        if (subNameSpace != null)
                        {
                            nameSpace.appendNameSpaces(subNameSpace);
                            subNameSpace.NameSpaceRef = nameSpaceRef;
                        }
                        else
                        {
                            ErrorsDuringLoad.Add(new ElementLog(ElementLog.LevelEnum.Error,
                                "Cannot load file " + nameSpaceRef.FileName));
                        }
                    }
                    nameSpace.allNameSpaceRefs().Clear();
                }

                base.visit(obj, visitSubNodes);
            }

            public override void visit(Generated.Specification obj, bool visitSubNodes)
            {
                Specification.Specification specification = (Specification.Specification) obj;

                if (specification.allChapterRefs() != null)
                {
                    foreach (ChapterRef chapterRef in specification.allChapterRefs())
                    {
                        Chapter chapter = chapterRef.LoadChapter(LockFiles, AllowErrorsDuringLoad);
                        if (chapter != null)
                        {
                            specification.appendChapters(chapter);
                            chapter.ChapterRef = chapterRef;
                        }
                        else
                        {
                            ErrorsDuringLoad.Add(new ElementLog(ElementLog.LevelEnum.Error,
                                "Cannot load file " + chapterRef.FileName));
                        }
                    }
                    specification.allChapterRefs().Clear();
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Holds information about opened files in the system
        /// </summary>
        private class FileData
        {
            /// <summary>
            ///     The name of the corresponding file
            /// </summary>
            private String FileName { get; set; }

            /// <summary>
            ///     The stream used to lock the file
            /// </summary>
            private FileStream Stream { get; set; }

            /// <summary>
            ///     The length of the lock section
            /// </summary>
            private long LockLength { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="fileName"></param>
            public FileData(String fileName)
            {
                FileName = fileName;
                Lock();
            }

            /// <summary>
            ///     Locks the corresponding file
            /// </summary>
            public void Lock()
            {
                if (Stream == null && PleaseLockFiles)
                {
                    Stream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    LockLength = Stream.Length;
                    Stream.Lock(0, LockLength);
                }
            }

            /// <summary>
            ///     Unlocks the corresponding file
            /// </summary>
            public void Unlock()
            {
                if (Stream != null && PleaseLockFiles)
                {
                    Stream.Unlock(0, LockLength);
                    Stream.Close();
                    Stream = null;
                }
            }
        }

        /// <summary>
        ///     Lock all opened files
        /// </summary>
        private static readonly List<FileData> OpenedFiles = new List<FileData>();

        /// <summary>
        ///     Locks a single file
        /// </summary>
        /// <param name="filePath"></param>
        private static void LockFile(String filePath)
        {
            FileData data = new FileData(filePath);
            OpenedFiles.Add(data);
        }

        /// <summary>
        ///     Unlocks all files locked by the system
        /// </summary>
        public static void UnlockAllFiles()
        {
            foreach (FileData data in OpenedFiles)
            {
                data.Unlock();
            }
        }

        /// <summary>
        ///     Locks all files loaded in the system
        /// </summary>
        public static void LockAllFiles()
        {
            foreach (FileData data in OpenedFiles)
            {
                data.Lock();
            }
        }

        /// <summary>
        ///     Loads a file and locks it if required
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="enclosing"></param>
        /// <param name="lockFiles"></param>
        /// <returns></returns>
        public static T LoadFile<T>(string filePath, ModelElement enclosing, bool lockFiles)
            where T : class, IXmlBBase
        {
            T retVal = null;

            // Do not rely on XmlBFileContext since it does not care about encoding. 
            // File encoding is UTF-8
            XmlBStringContext ctxt;
            using (StreamReader file = new StreamReader(filePath))
            {
                ctxt = new XmlBStringContext(file.ReadToEnd());
                file.Close();
            }

            DontNotify(() =>
            {
                try
                {
                    retVal = acceptor.accept(ctxt) as T;
                    if (retVal != null)
                    {
                        retVal.setFather(enclosing);
                        if (lockFiles)
                        {
                            LockFile(filePath);
                        }
                    }
                }
                catch (XmlBException)
                {
                    Log.Error(ctxt.errorMessage());
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            });

            return retVal;
        }

        /// <summary>
        ///     Parameters to be used when loading a file
        /// </summary>
        public class LoadParams
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="filePath"></param>
            public LoadParams(string filePath)
            {
                FilePath = filePath;
            }

            /// <summary>
            ///     The file path to load
            /// </summary>
            public String FilePath { get; private set; }

            /// <summary>
            ///     Indicates that the files should be locked
            /// </summary>
            public bool LockFiles { get; set; }

            /// <summary>
            ///     The location where errors should be stored
            /// </summary>
            public List<ElementLog> Errors { get; set; }

            /// <summary>
            ///     Indicates that errors can be raised when loading the file
            /// </summary>
            public bool AllowErrors
            {
                get { return Errors != null; }
            }

            /// <summary>
            ///     Indicates that the empty GUID of the elements should be setup to a real GUID
            /// </summary>
            public bool UpdateGuid { get; set; }

            /// <summary>
            ///     Indicates that obsolete files should be updated
            /// </summary>
            public bool ConvertObsolete { get; set; }
        }

        /// <summary>
        ///     Loads a dictionary and lock the file
        /// </summary>
        /// <param name="efsSystem">The system for which this dictionary is loaded</param>
        /// <param name="loadParams">The parameters used to load the file</param>
        /// <returns></returns>
        public static Dictionary Load(EFSSystem efsSystem, LoadParams loadParams)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Dictionary retVal;

            ObjectFactory factory = (ObjectFactory) acceptor.getFactory();
            try
            {
                factory.AutomaticallyGenerateGuid = false;
                retVal = LoadFile<Dictionary>(loadParams.FilePath, null, loadParams.LockFiles);
                if (retVal != null)
                {
                    retVal.FilePath = loadParams.FilePath;
                    if (efsSystem != null)
                    {
                        efsSystem.AddDictionary(retVal);
                    }

                    // Loads the dependancies for this .efs file
                    DontNotify(() =>
                    {
                        try
                        {
                            LoadDepends loadDepends = new LoadDepends(loadParams.LockFiles, loadParams.Errors);
                            loadDepends.visit(retVal);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                            retVal = null;
                        }
                    });
                }

                if (retVal != null)
                {
                    // Updates the contents of this .efs file
                    DontNotify(() =>
                    {
                        try
                        {
                            Updater updater = new Updater(loadParams.UpdateGuid, loadParams.ConvertObsolete);
                            updater.visit(retVal);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }
                    });
                }
            }
            finally
            {
                factory.AutomaticallyGenerateGuid = true;
            }

            if (efsSystem != null)
            {
                efsSystem.Context.HandleChangeEvent(null, Context.ChangeKind.Load);
            }

            return retVal;
        }

        /// <summary>
        ///     Loads a specification and lock the file
        /// </summary>
        /// <param name="filePath">The name of the file which holds the dictionary data</param>
        /// <param name="dictionary">The dictionary for which the specification is loaded</param>
        /// <param name="lockFiles">Indicates that the files should be locked</param>
        /// <returns></returns>
        public static Specification.Specification LoadSpecification(String filePath, Dictionary dictionary,
            bool lockFiles)
        {
            Specification.Specification retVal = LoadFile<Specification.Specification>(filePath, dictionary, lockFiles);

            if (retVal == null)
            {
                throw new Exception("Cannot read file " + filePath);
            }

            return retVal;
        }

        /// <summary>
        ///     Loads a translation dictionary and lock the file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dictionary"></param>
        /// <param name="lockFiles">Indicates that the files should be locked</param>
        /// <returns></returns>
        public static TranslationDictionary LoadTranslationDictionary(string filePath, Dictionary dictionary,
            bool lockFiles)
        {
            TranslationDictionary retVal = LoadFile<TranslationDictionary>(filePath, dictionary, lockFiles);

            if (retVal == null)
            {
                throw new Exception("Cannot read file " + filePath);
            }

            return retVal;
        }

        /// <summary>
        ///     Loads a namespace and locks the file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="enclosing"></param>
        /// <param name="lockFiles"></param>
        /// <param name="allowErrors"></param>
        /// <returns></returns>
        public static Types.NameSpace LoadNameSpace(string filePath, ModelElement enclosing, bool lockFiles,
            bool allowErrors)
        {
            Types.NameSpace retVal = LoadFile<Types.NameSpace>(filePath, enclosing, lockFiles);

            if (retVal == null)
            {
                if (!allowErrors)
                {
                    throw new Exception("Cannot read file " + filePath);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Loads a frame and locks the file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="enclosing"></param>
        /// <param name="lockFiles"></param>
        /// <param name="allowErrors"></param>
        /// <returns></returns>
        public static Frame LoadFrame(string filePath, ModelElement enclosing, bool lockFiles, bool allowErrors)
        {
            Frame retVal = LoadFile<Frame>(filePath, enclosing, lockFiles);

            if (retVal == null)
            {
                if (!allowErrors)
                {
                    throw new Exception("Cannot read file " + filePath);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Loads a chapter and locks the file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="enclosing"></param>
        /// <param name="lockFiles">Indicates that the files should be locked</param>
        /// <param name="allowErrors"></param>
        /// <returns></returns>
        public static Chapter LoadChapter(string filePath, ModelElement enclosing, bool lockFiles, bool allowErrors)
        {
            Chapter retVal = LoadFile<Chapter>(filePath, enclosing, lockFiles);

            if (retVal == null)
            {
                if (!allowErrors)
                {
                    throw new Exception("Cannot read file " + filePath);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates that the character is a valid character for a file path
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool ValidChar(char c)
        {
            bool retVal = true;

            foreach (char other in Path.GetInvalidPathChars())
            {
                if (other == c)
                {
                    retVal = false;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Creates a valid file path for the path provided
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ValidFilePath(string path)
        {
            string retVal = "";

            foreach (char c in path)
            {
                if (!ValidChar(c))
                {
                    retVal += "_";
                }
                else
                {
                    retVal += c;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     An action that do not notify the controllers
        /// </summary>
        public delegate void NonNotifiedAction();

        /// <summary>
        ///     Indicates the number of times a DontNotify has been recursively called
        /// </summary>
        private static int _dontNotifyCount;

        private static readonly Mutex NotificationMutex = new Mutex(false, "Nofitication excusive region");

        /// <summary>
        ///     Indicates that notification should not occur for this action
        /// </summary>
        /// <param name="action"></param>
        public static void DontNotify(NonNotifiedAction action)
        {
            try
            {
                NotificationMutex.WaitOne();
                _dontNotifyCount += 1;
                if (_dontNotifyCount == 1)
                {
                    ControllersManager.DesactivateAllNotifications();
                }
                NotificationMutex.ReleaseMutex();

                action();
            }
            finally
            {
                NotificationMutex.WaitOne();
                _dontNotifyCount -= 1;
                if (_dontNotifyCount == 0)
                {
                    ControllersManager.ActivateAllNotifications();
                }
                NotificationMutex.ReleaseMutex();
            }
        }

        /// <summary>
        ///     Checks if there is any error in the model
        /// </summary>
        public class IsThereAnyError : Visitor
        {
            /// <summary>
            ///     The list of errors
            /// </summary>
            public List<ElementLog> ErrorsFound { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            public IsThereAnyError()
            {
                ErrorsFound = new List<ElementLog>();

                foreach (Dictionary dictionary in EFSSystem.INSTANCE.Dictionaries)
                {
                    visit(dictionary);
                }
            }

            public override void visit(BaseModelElement obj, bool visitSubNodes)
            {
                foreach (ElementLog log in obj.Messages)
                {
                    if (log.Level == ElementLog.LevelEnum.Error)
                    {
                        ErrorsFound.Add(log);
                    }
                }

                base.visit(obj, visitSubNodes);
            }
        }

    }
}