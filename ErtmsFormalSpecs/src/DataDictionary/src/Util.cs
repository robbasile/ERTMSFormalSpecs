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
                    string guid = element.Guid;
                }

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
                RequirementSet scope = paragraph.Dictionary.findRequirementSet(Dictionary.SCOPE_NAME, true);

                if (paragraph.getObsoleteScopeOnBoard())
                {
                    RequirementSet onBoard = scope.findRequirementSet(RequirementSet.ONBOARD_SCOPE_NAME, false);
                    if (onBoard == null)
                    {
                        onBoard = scope.findRequirementSet(RequirementSet.ONBOARD_SCOPE_NAME, true);
                        onBoard.setRecursiveSelection(false);
                        onBoard.setDefault(true);
                    }
                    paragraph.AppendToRequirementSet(onBoard);
                    paragraph.setObsoleteScopeOnBoard(false);
                }

                if (paragraph.getObsoleteScopeTrackside())
                {
                    RequirementSet trackSide = scope.findRequirementSet(RequirementSet.TRACKSIDE_SCOPE_NAME, false);
                    if (trackSide == null)
                    {
                        trackSide = scope.findRequirementSet(RequirementSet.TRACKSIDE_SCOPE_NAME, true);
                        trackSide.setRecursiveSelection(false);
                        trackSide.setDefault(true);
                    }
                    paragraph.AppendToRequirementSet(trackSide);
                    paragraph.setObsoleteScopeTrackside(false);
                }

                if (paragraph.getObsoleteScopeRollingStock())
                {
                    RequirementSet rollingStock = scope.findRequirementSet(RequirementSet.ROLLING_STOCK_SCOPE_NAME,
                        false);
                    if (rollingStock == null)
                    {
                        rollingStock = scope.findRequirementSet(RequirementSet.ROLLING_STOCK_SCOPE_NAME, true);
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
                        paragraph.Dictionary.findRequirementSet(Dictionary.FUNCTIONAL_BLOCK_NAME, true);
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
            ///     The base path used to load files
            /// </summary>
            public string BasePath { get; private set; }

            /// <summary>
            ///     Indicates that the files should be locked
            /// </summary>
            public bool LockFiles { get; private set; }

            /// <summary>
            ///     Indicates that errors can occur during load, for instance, for comparison purposes
            /// </summary>
            public bool AllowErrorsDuringLoad
            {
                get { return ErrorsDuringLoad != null; }
            }

            /// <summary>
            ///     The errors that occured during the load of the file
            /// </summary>
            public List<ElementLog> ErrorsDuringLoad { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="basePath"></param>
            /// <param name="lockFiles"></param>
            /// <param name="errors"></param>
            public LoadDepends(string basePath, bool lockFiles, List<ElementLog> errors)
            {
                BasePath = basePath;
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
            public String FileName { get; private set; }

            /// <summary>
            ///     The stream used to lock the file
            /// </summary>
            public FileStream Stream { get; private set; }

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
        private static List<FileData> openedFiles = new List<FileData>();

        /// <summary>
        ///     Locks a single file
        /// </summary>
        /// <param name="filePath"></param>
        private static void LockFile(String filePath)
        {
            FileData data = new FileData(filePath);
            openedFiles.Add(data);
        }

        /// <summary>
        ///     Unlocks all files locked by the system
        /// </summary>
        public static void UnlockAllFiles()
        {
            foreach (FileData data in openedFiles)
            {
                data.Unlock();
            }
        }

        /// <summary>
        ///     Locks all files loaded in the system
        /// </summary>
        public static void LockAllFiles()
        {
            foreach (FileData data in openedFiles)
            {
                data.Lock();
            }
        }

        /// <summary>
        ///     Loads a document and handles its associated locks
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class DocumentLoader<T>
            where T : class, IXmlBBase
        {
            /// <summary>
            ///     Loads a file and locks it if required
            /// </summary>
            /// <param name="filePath"></param>
            /// <param name="enclosing"></param>
            /// <param name="lockFiles"></param>
            /// <returns></returns>
            public static T loadFile(string filePath, ModelElement enclosing, bool lockFiles)
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

                try
                {
                    ControllersManager.DesactivateAllNotifications();
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
                catch (XmlBException excp)
                {
                    Log.Error(ctxt.errorMessage());
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
                finally
                {
                    ControllersManager.ActivateAllNotifications();
                }

                return retVal;
            }
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
        public static Dictionary load(EFSSystem efsSystem, LoadParams loadParams)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Dictionary retVal = null;

            ObjectFactory factory = (ObjectFactory) acceptor.getFactory();
            try
            {
                factory.AutomaticallyGenerateGuid = false;
                retVal = DocumentLoader<Dictionary>.loadFile(loadParams.FilePath, null, loadParams.LockFiles);
                if (retVal != null)
                {
                    retVal.FilePath = loadParams.FilePath;
                    if (efsSystem != null)
                    {
                        efsSystem.AddDictionary(retVal);
                    }

                    // Loads the dependancies for this .efs file
                    try
                    {
                        ControllersManager.DesactivateAllNotifications();
                        LoadDepends loadDepends = new LoadDepends(retVal.BasePath, loadParams.LockFiles,
                            loadParams.Errors);
                        loadDepends.visit(retVal);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                        retVal = null;
                    }
                    finally
                    {
                        ControllersManager.ActivateAllNotifications();
                    }
                }

                if (retVal != null)
                {
                    // Updates the contents of this .efs file
                    try
                    {
                        ControllersManager.DesactivateAllNotifications();

                        Updater updater = new Updater(loadParams.UpdateGuid, loadParams.ConvertObsolete);
                        updater.visit(retVal);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }
                    finally
                    {
                        ControllersManager.ActivateAllNotifications();
                    }
                }
            }
            finally
            {
                factory.AutomaticallyGenerateGuid = true;
            }

            if (efsSystem != null)
            {
                efsSystem.ShouldRebuild = true;
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
        public static Specification.Specification loadSpecification(String filePath, Dictionary dictionary,
            bool lockFiles)
        {
            Specification.Specification retVal = DocumentLoader<Specification.Specification>.loadFile(filePath,
                dictionary, lockFiles);

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
        public static TranslationDictionary loadTranslationDictionary(string filePath, Dictionary dictionary,
            bool lockFiles)
        {
            TranslationDictionary retVal = DocumentLoader<TranslationDictionary>.loadFile(filePath, dictionary,
                lockFiles);

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
        public static Types.NameSpace loadNameSpace(string filePath, ModelElement enclosing, bool lockFiles,
            bool allowErrors)
        {
            Types.NameSpace retVal = DocumentLoader<Types.NameSpace>.loadFile(filePath, enclosing, lockFiles);

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
        public static Frame loadFrame(string filePath, ModelElement enclosing, bool lockFiles, bool allowErrors)
        {
            Frame retVal = DocumentLoader<Frame>.loadFile(filePath, enclosing, lockFiles);

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
        /// <param name="dictionary"></param>
        /// <param name="lockFiles">Indicates that the files should be locked</param>
        /// <param name="allowErrors"></param>
        /// <returns></returns>
        public static Chapter loadChapter(string filePath, ModelElement enclosing, bool lockFiles, bool allowErrors)
        {
            Chapter retVal = DocumentLoader<Chapter>.loadFile(filePath, enclosing, lockFiles);

            if (retVal == null)
            {
                if (!allowErrors)
                {
                    throw new Exception("Cannot read file " + filePath);
                }
            }

            return retVal;
        }

        private class MessageInfoVisitor : Visitor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public MessageInfoVisitor()
            {
            }

            /// <summary>
            ///     Provides the maximum path info
            /// </summary>
            /// <param name="v1"></param>
            /// <param name="v2"></param>
            /// <returns></returns>
            private MessagePathInfoEnum Max(MessagePathInfoEnum v1, MessagePathInfoEnum v2)
            {
                MessagePathInfoEnum retVal;

                if (v1.CompareTo(v2) > 0)
                {
                    retVal = v1;
                }
                else
                {
                    retVal = v2;
                }

                return retVal;
            }

            public void UpdateMessageInfo(ModelElement element)
            {
                // Compute the local value of the message path info
                if (element.HasMessage(ElementLog.LevelEnum.Error))
                {
                    element.MessagePathInfo = MessagePathInfoEnum.Error;
                }
                else if (element.HasMessage(ElementLog.LevelEnum.Warning))
                {
                    element.MessagePathInfo = MessagePathInfoEnum.Warning;
                }
                else if (element.HasMessage(ElementLog.LevelEnum.Info))
                {
                    element.MessagePathInfo = MessagePathInfoEnum.Info;
                }
                else
                {
                    element.MessagePathInfo = MessagePathInfoEnum.Nothing;
                }

                // Grab the list of sub elements
                ArrayList l = new ArrayList();
                element.subElements(l);

                // Compute the sub element's message info
                foreach (object obj in l)
                {
                    ModelElement subElement = obj as ModelElement;
                    if (subElement != null)
                    {
                        UpdateMessageInfo(subElement);
                    }
                }

                // Combine the message info
                foreach (object obj in l)
                {
                    ModelElement subElement = obj as ModelElement;
                    if (subElement != null)
                    {
                        MessagePathInfoEnum sub = subElement.MessagePathInfo;

                        if (sub == MessagePathInfoEnum.Error)
                        {
                            sub = MessagePathInfoEnum.PathToError;
                        }
                        else if (sub == MessagePathInfoEnum.Warning)
                        {
                            sub = MessagePathInfoEnum.PathToWarning;
                        }
                        else if (sub == MessagePathInfoEnum.Info)
                        {
                            sub = MessagePathInfoEnum.PathToInfo;
                        }

                        element.MessagePathInfo = Max(element.MessagePathInfo, sub);
                    }
                }
            }
        }

        /// <summary>
        ///     Updates the message info according to the model element messages and its sub model elements
        /// </summary>
        /// <param name="modelElement"></param>
        public static void UpdateMessageInfo(ModelElement modelElement)
        {
            MessageInfoVisitor visitor = new MessageInfoVisitor();
            visitor.UpdateMessageInfo(modelElement);
        }

        /// <summary>
        ///     Indicates that the character is a valid character for a file path
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool validChar(char c)
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
        public static string validFilePath(string path)
        {
            string retVal = "";

            foreach (char c in path)
            {
                if (!validChar(c))
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
    }
}