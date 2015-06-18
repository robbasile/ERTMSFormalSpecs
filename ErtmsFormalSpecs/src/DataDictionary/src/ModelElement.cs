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
using System.Text;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;
using Utils;
using XmlBooster;
using StateMachine = DataDictionary.Types.StateMachine;
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

            bool previousSilent = BeSilent;
            try
            {
                BeSilent = true;
                Expression expression = EFSSystem.INSTANCE.Parser.Expression(modelElement, name,
                    AllMatches.INSTANCE, true, null, true);

                foreach (
                    ReturnValueElement target in
                        expression.getReferences(null, AllMatches.INSTANCE, true).Values)
                {
                    if (target.Value == this)
                    {
                        retVal = true;
                        break;
                    }
                }
            }
            finally
            {
                BeSilent = previousSilent;
            }

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

            return retVal;
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
                    retVal = GuidCache.INSTANCE.GetModel(getUpdates());
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
    }

    /// <summary>
    /// Something that can be explained
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
        /// Provides the explanation associated to the ITextualExplain
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
        /// The data currently being built
        /// </summary>
        private StringBuilder Data { get; set; }

        /// <summary>
        /// The current indent level
        /// </summary>
        private int IndentLevel { get; set; }

        /// <summary>
        /// The current indent string
        /// </summary>
        private string IndentString{ get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public TextualExplanation()
        {
            Data = new StringBuilder();
            IndentLevel = 0;
            IndentString = "";
        }

        /// <summary>
        /// Provides the textual explanation
        /// </summary>
        public string Text
        {
            get { return Data.ToString(); }
        }

        /// <summary>
        /// This code creates explanation which shall be indented
        /// </summary>
        public delegate void CodeToIndent();

        /// <summary>
        /// Increases the indent level
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
        /// Pads a given string
        /// </summary>
        /// <param name="data"></param>
        public void Pad(string data)
        {
            Data.Append(IndentString);
            Data.Append(data);
        }

        /// <summary>
        /// Pads the data and adds a newline
        /// </summary>
        /// <param name="data"></param>
        public void PadLine(string data)
        {
            Pad(data);
            Data.Append("\n");
        }

        /// <summary>
        /// Appends a string to the current result
        /// </summary>
        /// <param name="data"></param>
        public void Write(string data)
        {
            Data.Append(data);
        }

        /// <summary>
        /// Appends a string to the current result and adds an end of line
        /// </summary>
        /// <param name="data"></param>
        public void WriteLine(string data = "")
        {
            Write(data);
            Data.Append("\n");
        }

        /// <summary>
        ///     Provides the expression
        /// </summary>
        /// <param name="element"></param>
        public void Expression(ModelElement element)
        {
            string retVal = "";

            IExpressionable expressionable = element as IExpressionable;
            if (expressionable != null)
            {
                if (string.IsNullOrEmpty(expressionable.ExpressionText))
                {
                    Pad("<Undefined expression or statement>");
                }
                else
                {
                    Pad(expressionable.ExpressionText);
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
                PadLine("// " + line);
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
    }
}