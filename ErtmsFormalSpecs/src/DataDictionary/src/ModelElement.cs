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
using System.Linq;
using System.Text;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;
using Utils;
using XmlBooster;
using Comparer = DataDictionary.Compare.Comparer;
using NameSpace = DataDictionary.Types.NameSpace;
using Paragraph = DataDictionary.Specification.Paragraph;
using StateMachine = DataDictionary.Types.StateMachine;
using Structure = DataDictionary.Types.Structure;
using Visitor = DataDictionary.Generated.Visitor;

namespace DataDictionary
{
    public abstract class ModelElement : BaseModelElement
    {
        /// <summary>
        ///     Provides the EFS System in which this element belongs
        /// </summary>
        public EFSSystem EFSSystem
        {
            get { return EFSSystem.INSTANCE; }
        }

        /// <summary>
        ///     Provides the Dictionary in which this element belongs
        /// </summary>
        public Dictionary Dictionary
        {
            get { return EnclosingFinder<Dictionary>.find(this); }
        }

        /// <summary>
        ///     Adds a new element log attached to this model element
        /// </summary>
        /// <param name="log"></param>
        public override void AddElementLog(ElementLog log)
        {
            if (!BeSilent)
            {
                Parameter enclosingParameter = EnclosingFinder<Parameter>.find(this);
                if (enclosingParameter != null)
                {
                    log.Log = "In " + FullName + ":" + log.Log;
                    enclosingParameter.AddElementLog(log);
                }
                else
                {
                    base.AddElementLog(log);
                    EFSSystem.Context.HandleInfoMessageChangeEvent(this);
                }
            }
        }

        /// <summary>
        ///     Adds an error and explains it
        /// </summary>
        /// <param name="message"></param>
        /// <param name="explanation"></param>
        public void AddErrorAndExplain(string message, ExplanationPart explanation)
        {
            AddError(message);
            Explain = explanation;
        }

        /// <summary>
        ///     The last explanation part for this model element
        /// </summary>
        public ExplanationPart Explain { get; set; }

        /// <summary>
        ///     Indicates that no logging should occur
        /// </summary>
        private static bool BeSilent { get; set; }

        /// <summary>
        ///     An action which should not raise errors
        /// </summary>
        public delegate void SilentAction();

        /// <summary>
        ///     The number of times DontRaiseError has been recursively called
        /// </summary>
        private static int SilentCount { get; set; }

        /// <summary>
        ///     Do not raise errors while execution the action
        /// </summary>
        /// <param name="action"></param>
        public static void DontRaiseError(SilentAction action)
        {
            DontRaiseError(true, action);
        }

        /// <summary>
        ///     Do not raise errors while execution the action
        /// </summary>
        /// <param name="silent">Indicates that the action should be silent</param>
        /// <param name="action"></param>
        public static void DontRaiseError(bool silent, SilentAction action)
        {
            // Heuristic : 
            // Do not notify changes in the model when we are not interested 
            // in the errors raised while performing the action
            Util.DontNotify(() =>
            {
                if (silent)
                {
                    try
                    {
                        SilentCount += 1;
                        if (SilentCount == 1)
                        {
                            BeSilent = true;
                            action();
                        }
                        else
                        {
                            action();
                        }
                    }
                    finally
                    {
                        SilentCount -= 1;
                        if (SilentCount == 0)
                        {
                            BeSilent = false;
                        }
                    }
                }
                else
                {
                    action();
                }
            });
        }

        /// <summary>
        ///     Ensures that the GUID is set for this model element
        /// </summary>
        public void EnsureGuid()
        {
            if (string.IsNullOrEmpty(getGuid()))
            {
                setGuid(System.Guid.NewGuid().ToString());
            }
        }

        /// <summary>
        ///     Provides the Guid of the paragraph and creates one if it is not yet set
        /// </summary>
        public virtual string Guid
        {
            get
            {
                if (string.IsNullOrEmpty(getGuid()))
                {
                    EnsureGuid();
                }

                return getGuid();
            }
        }

        /// <summary>
        ///     Provides the name of this model element when accessing it from the other model element (provided as parameter)
        /// </summary>
        /// <param name="user">The user of this element</param>
        /// <returns></returns>
        public virtual string ReferenceName(ModelElement user)
        {
            string retVal = Name;

            ModelElement current = Enclosing as ModelElement;
            bool validName = CheckNewName(user, retVal);
            while (current != null && !(current is Dictionary) && current != user && !validName)
            {
                StateMachine enclosingStateMachine = current as StateMachine;
                if (enclosingStateMachine != null && enclosingStateMachine.EnclosingStateMachine != null)
                {
                    // Ignore state machines because they have the same name as their enclosing state
                    // This is not true for the first state machine in the chain (no enclosing state)
                }
                else
                {
                    retVal = current.Name + "." + retVal;
                    validName = CheckNewName(user, retVal);
                }

                current = current.Enclosing as ModelElement;
            }

            return retVal;
        }

        /// <summary>
        ///     Check if the new name references the right element in this context
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckNewName(ModelElement modelElement, string name)
        {
            bool retVal = false;

            DontRaiseError(() =>
            {
                Expression expression = EFSSystem.INSTANCE.Parser.Expression(modelElement, name,
                    AllMatches.INSTANCE, true, null, true);

                foreach (
                    ReturnValueElement target in
                        expression.GetReferences(null, AllMatches.INSTANCE, true).Values)
                {
                    if (target.Value == this)
                    {
                        retVal = true;
                        break;
                    }
                }
            });

            return retVal;
        }

        /// <summary>
        ///     Provides the description of the requirements related to this model element
        /// </summary>
        /// <returns></returns>
        public virtual string RequirementDescription()
        {
            string retVal = "";

            ReqRelated reqRelated = EnclosingFinder<ReqRelated>.find(this, true);
            if (reqRelated != null)
            {
                retVal = reqRelated.RequirementDescription();
            }

            return retVal;
        }


        /// <summary>
        ///     Generates new GUID for the element
        /// </summary>
        private class RegererateGuidVisitor : Visitor
        {
            /// <summary>
            ///     Ensures that all elements have a new Guid
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(BaseModelElement obj, bool visitSubNodes)
            {
                ModelElement element = (ModelElement) obj;

                // Side effect : creates a new Guid if it is empty
                element.setGuid(null);
                // ReSharper disable once UnusedVariable
                string guid = element.Guid;

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Duplicates the model element and avoid duplicated GUID
        /// </summary>
        /// <returns></returns>
        public ModelElement Duplicate()
        {
            ModelElement retVal = null;

            Util.DontNotify(() =>
            {
                XmlBStringContext ctxt = new XmlBStringContext(ToXMLString());
                try
                {
                    retVal = acceptor.accept(ctxt) as ModelElement;
                    RegererateGuidVisitor visitor = new RegererateGuidVisitor();
                    visitor.visit(retVal, true);
                }
                catch (Exception)
                {
                }
            });

            return retVal;
        }

        /// <summary>
        ///     Returns the path from the parent to the object as a list of element names.
        ///     The list includes the parent's name (not FullName) and, if the object is not
        ///     a direct descendant of the parent, an empty list.
        /// </summary>
        /// <param name="objName"></param>
        /// <param name="parentName"></param>
        /// <returns></returns>
        public List<string> GetRelativePathFrom(string objName, string parentName)
        {
            List<string> retVal = new List<string>();

            if (objName.StartsWith(parentName))
            {
                string[] fullNames = objName.Split('.');
                string[] topNames = parentName.Split('.');

                retVal.Add(topNames[topNames.Count() - 1]);
                foreach (string name in fullNames)
                {
                    if (!topNames.Contains(name))
                    {
                        retVal.Add(name);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the source of the update chain
        /// </summary>
        public ModelElement SourceOfUpdateChain
        {
            get
            {
                ModelElement retVal = this;

                while (retVal.Updates != null)
                {
                    retVal = retVal.Updates;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the model element that has been updated by this one (if any)
        /// </summary>
        public ModelElement Updates
        {
            get
            {
                ModelElement retVal = null;

                if (!string.IsNullOrEmpty(getUpdates()))
                {
                    retVal = GuidCache.Instance.GetModel(getUpdates());
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the list of the model elements which update this model element
        /// </summary>
        public List<ModelElement> UpdatedBy = new List<ModelElement>();

        /// <summary>
        ///     Sets the update information for this model element (this model element updates source)
        /// </summary>
        /// <param name="source"></param>
        public virtual void SetUpdateInformation(ModelElement source)
        {
            if (string.IsNullOrEmpty(getUpdates()) || getUpdates() != source.Guid)
            {
                setUpdates(source.Guid);
            }
        }

        /// <summary>
        ///     Brings the model element from an update dictionary to the updated dictionary
        /// </summary>
        public virtual void Merge()
        {
            ModelElement target = Updates;
            if (target == null)
            {
                // Copy element to parent 
                ModelElement parent = Enclosing as ModelElement;
                if (parent != null)
                {
                    NameSpace parentNameSpace = parent.Updates as NameSpace;
                    if (parentNameSpace != null)
                    {
                        parentNameSpace.AddModelElement(Duplicate());
                    }

                    Paragraph parentParagraph = parent.Updates as Paragraph;
                    if (parentParagraph != null)
                    {
                        parentParagraph.AddModelElement(Duplicate());
                    }

                    Structure parentStructure = parent.Updates as Structure;
                    if (parentStructure != null)
                    {
                        parentStructure.AddModelElement(Duplicate());
                    }

                    StateMachine parentStateMachine = parent.Updates as StateMachine;
                    if (parentStateMachine != null)
                    {
                        parentStateMachine.AddModelElement(Duplicate());
                    }
                }
            }
            else
            {
                if (!IsRemoved)
                {
                    target.UpdateModelElementAccordingToSource(this);
                }
                else
                {
                    target.Delete();
                }
            }
        }

        /// <summary>
        ///     Gets all values from source to target
        /// </summary>
        /// <param name="source"></param>
        protected virtual void UpdateModelElementAccordingToSource(ModelElement source)
        {
            IGraphicalDisplay sourceGraphicalDisplay = source as IGraphicalDisplay;
            IGraphicalDisplay targetGraphicalDisplay = this as IGraphicalDisplay;
            if (sourceGraphicalDisplay != null && targetGraphicalDisplay != null)
            {
                int x = targetGraphicalDisplay.X;
                int y = targetGraphicalDisplay.Y;
                int width = targetGraphicalDisplay.Width;
                int height = targetGraphicalDisplay.Height;
                bool hidden = targetGraphicalDisplay.Hidden;

                Comparer.Duplicate(source, this);

                targetGraphicalDisplay.X = x;
                targetGraphicalDisplay.Y = y;
                targetGraphicalDisplay.Width = width;
                targetGraphicalDisplay.Height = height;
                targetGraphicalDisplay.Hidden = hidden;
            }
            else
            {
                Comparer.Duplicate(source, this);
            }
        }

        /// <summary>
        ///     Base function to remove update information
        /// </summary>
        public virtual void RecoverUpdateInformation()
        {
            if (Updates != null)
            {
                setUpdates(Updates.getUpdates());
            }
        }

        /// <summary>
        ///     Indicates that this model element should be ignored
        /// </summary>
        public override bool IsRemoved
        {
            get
            {
                bool retVal = getIsRemoved();

                if (!retVal)
                {
                    foreach (ModelElement update in UpdatedBy)
                    {
                        if (update.IsRemoved)
                        {
                            retVal = true;
                            break;
                        }
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Creates the status message
        /// </summary>
        /// <returns>the status string for the selected element</returns>
        public virtual string CreateStatusMessage()
        {
            return "";
        }

        /// <summary>
        ///     Provides the number of a new element in the collection provided
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static int GetElementNumber(ICollection enclosingCollection)
        {
            int retVal = 1;

            if (enclosingCollection != null)
            {
                retVal = enclosingCollection.Count + 1;
            }

            return retVal;
        }
    }

    /// <summary>
    ///     Something that can be explained
    /// </summary>
    public interface ITextualExplain
    {
        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        void GetExplain(TextualExplanation explanation, bool explainSubElements);
    }

    /// <summary>
    ///     Utilities for explain boxes
    /// </summary>
    public static class TextualExplanationUtils
    {
        /// <summary>
        ///     Provides the explanation associated to the ITextualExplain
        /// </summary>
        /// <param name="textualExplain"></param>
        /// <param name="explainSubElements"></param>
        /// <returns></returns>
        public static string GetText(ITextualExplain textualExplain, bool explainSubElements)
        {
            TextualExplanation text = new TextualExplanation();
            textualExplain.GetExplain(text, explainSubElements);
            return text.Text;
        }
    }

    /// <summary>
    ///     Utilities for explain boxes
    /// </summary>
    public class TextualExplanation
    {
        /// <summary>
        ///     The data currently being built
        /// </summary>
        private StringBuilder Data { get; set; }

        /// <summary>
        ///     Indicates that we are at the beginning of a new line
        /// </summary>
        private bool NewLine { get; set; }

        /// <summary>
        ///     The current indent level
        /// </summary>
        private int IndentLevel { get; set; }

        /// <summary>
        ///     The current indent string
        /// </summary>
        private string IndentString { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public TextualExplanation()
        {
            Data = new StringBuilder();
            IndentLevel = 0;
            IndentString = "";
            NewLine = true;
        }

        /// <summary>
        ///     Provides the textual explanation
        /// </summary>
        public string Text
        {
            get { return Data.ToString(); }
        }

        /// <summary>
        ///     This code creates explanation which shall be indented
        /// </summary>
        public delegate void CodeToIndent();

        /// <summary>
        ///     Increases the indent level
        /// </summary>
        /// <param name="size"></param>
        /// <param name="indentCode">The action used to create the indented code</param>
        public void Indent(int size, CodeToIndent indentCode)
        {
            int previousIndentLevel = IndentLevel;
            string previousIndentString = IndentString;

            IndentLevel += size;
            IndentString = "".PadLeft(IndentLevel);
            try
            {
                indentCode();
            }
            finally
            {
                IndentLevel = previousIndentLevel;
                IndentString = previousIndentString;
            }
        }

        /// <summary>
        ///     Appends a string to the current result
        /// </summary>
        /// <param name="data"></param>
        public void Write(string data = "")
        {
            if (NewLine)
            {
                Data.Append(IndentString);
                NewLine = false;
            }
            Data.Append(data);
        }

        /// <summary>
        ///     Appends a string to the current result and adds an end of line
        /// </summary>
        /// <param name="data"></param>
        public void WriteLine(string data = "")
        {
            Write(data);
            Data.Append("\n");
            NewLine = true;
        }

        /// <summary>
        ///     Provides the expression
        /// </summary>
        /// <param name="element"></param>
        public void Expression(ModelElement element)
        {
            IExpressionable expressionable = element as IExpressionable;
            if (expressionable != null)
            {
                if (expressionable.Tree != null)
                {
                    expressionable.Tree.GetExplain(this);
                }
                else
                {
                    if (string.IsNullOrEmpty(expressionable.ExpressionText))
                    {
                        Write("<Undefined expression or statement>");
                    }
                    else
                    {
                        Write(expressionable.ExpressionText);
                    }
                }
            }
        }

        /// <summary>
        ///     Comments a section of text
        /// </summary>
        /// <param name="data">the data to pad</param>
        public void Comment(string data)
        {
            foreach (string line in data.Split('\n'))
            {
                WriteLine("// " + line);
            }
        }

        /// <summary>
        ///     Comments an Icommentable
        /// </summary>
        /// <param name="element"></param>
        public void Comment(ModelElement element)
        {
            ICommentable commentable = element as ICommentable;
            if (commentable != null && !string.IsNullOrEmpty(commentable.Comment))
            {
                Comment(commentable.Comment);
            }
        }

        /// <summary>
        ///     The name of the element
        /// </summary>
        /// <param name="element"></param>
        public void Name(ModelElement element)
        {
            Namable namable = element as Namable;
            if (namable != null)
            {
                Comment(namable.Name);
            }
        }

        /// <summary>
        ///     Provides the header of the element
        /// </summary>
        /// <param name="element"></param>
        public void Header(ModelElement element)
        {
            Comment(element);
            Name(element);
        }

        /// <summary>
        ///     Method used to explain a single element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        public delegate void ExplainElement<in T>(T element);

        /// <summary>
        ///     Explains a list of elements
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="explainSubElements"></param>
        /// <param name="separator"></param>
        /// <param name="explainElement">The action used to explain a single element</param>
        public void ExplainList<T>(IEnumerable<T> elements, bool explainSubElements, string separator,
            ExplainElement<T> explainElement)
        {
            bool first = true;
            foreach (T element in elements)
            {
                if (!first)
                {
                    Write(separator);
                }
                first = false;
                explainElement(element);
            }
        }
    }
}