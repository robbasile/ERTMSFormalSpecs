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
        public static bool BeSilent { get; set; }

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
                ObjectFactory factory = (ObjectFactory) acceptor.getFactory();
                if (string.IsNullOrEmpty(getGuid()))
                {
                    EnsureGuid();
                }

                return getGuid();
            }
        }

        /// <summary>
        ///     Provides the common prefix between s1 and s2
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        private string CommonPrefix(string s1, string s2)
        {
            int i = 0;
            while (i < s1.Length && i < s2.Length && s1[i] == s2[i])
            {
                i += 1;
            }

            do
            {
                i -= 1;
            } while (i >= 0 && s1[i] != '.');
            i += 1;

            return s1.Substring(0, i);
        }

        /// <summary>
        ///     Provides the name of this model element when accessing it from the other model element (provided as parameter)
        /// </summary>
        /// <param name="modelElement">The user of this element</param>
        /// <returns></returns>
        public virtual string ReferenceName(ModelElement modelElement)
        {
            string retVal = Name;

            if (this != modelElement)
            {
                ModelElement enclosing = Enclosing as ModelElement;
                while (enclosing != null && !(enclosing is Dictionary) && enclosing != modelElement)
                {
                    StateMachine enclosingStateMachine = enclosing as StateMachine;

                    if (enclosingStateMachine != null && enclosingStateMachine.EnclosingStateMachine != null)
                    {
                        // Ignore state machines because they have the same name as their enclosing state
                        // This is not true for the first state machine in the chain
                        enclosing = enclosing.Enclosing as ModelElement;
                    }
                    else
                    {
                        bool sharePrefix = false;
                        ModelElement current = modelElement;
                        while (!sharePrefix && current != null)
                        {
                            sharePrefix = enclosing == current;
                            current = current.Enclosing as ModelElement;
                        }

                        if (sharePrefix)
                        {
                            enclosing = null;
                        }
                        else
                        {
                            retVal = enclosing.Name + "." + retVal;
                            enclosing = enclosing.Enclosing as ModelElement;
                        }
                    }
                }
            }

            // Check if the name provided still references the same item
            bool referenceFound = false;
            Expression expression  = EFSSystem.INSTANCE.Parser.Expression(modelElement, retVal, Interpreter.Filter.AllMatches.INSTANCE, true, null, true);

            foreach (ReturnValueElement target in expression.getReferences(null, Interpreter.Filter.AllMatches.INSTANCE, true).Values)
            {
                if (target.Value == this)
                {
                    referenceFound = true;
                    break;
                }
            }

            foreach (ReturnValueElement target in expression.getReferences(modelElement, Interpreter.Filter.AllMatches.INSTANCE, true).Values)
            {
                if (target.Value == this)
                {
                    referenceFound = true;
                    break;
                }
            }

            // If the string created did not reference this, provide the fullname instead (to be on the safe side)
            if (!referenceFound)
            {
                retVal = FullName;
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
            catch (Exception e)
            {
            }

            return retVal;
        }

        /// <summary>
        /// Provides the model element that has been updated by this one (if any)
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
        /// Provides the list of the model elements which update this model element
        /// </summary>
        public List<ModelElement> UpdatedBy = new List<ModelElement>();

        /// <summary>
        /// Sets the update information for this model element (this model element updates source)
        /// </summary>
        /// <param name="source"></param>
        public virtual void SetUpdateInformation(ModelElement source)
        {
            if (string.IsNullOrEmpty(getUpdates()) || getUpdates() != source.Guid )
            {
                setUpdates(source.Guid);
            }
        }
    }

    public interface TextualExplain
    {
        /// <summary>
        ///     The explanation of the element
        /// </summary>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        string getExplain(bool explainSubElements);
    }

    /// <summary>
    ///     Utilities for RTF explain boxes
    /// </summary>
    public class TextualExplainUtilities
    {
        /// <summary>
        ///     Left pads the string provided
        /// </summary>
        /// <param name="data">the data to pad</param>
        /// <param name="padlen">the size of the pad</param>
        /// <returns></returns>
        public static string Pad(string data, int padlen)
        {
            return "".PadLeft(padlen) + data;
        }

        /// <summary>
        ///     Provides the expression
        /// </summary>
        /// <param name="element"></param>
        /// <param name="padlen"></param>
        /// <returns></returns>
        public static string Expression(ModelElement element, int padlen)
        {
            string retVal = "";

            IExpressionable expressionable = element as IExpressionable;
            if (expressionable != null)
            {
                if (string.IsNullOrEmpty(expressionable.ExpressionText))
                {
                    retVal = Pad("<Undefined expression or statement>", padlen);
                }
                else
                {
                    retVal = Pad(expressionable.ExpressionText, padlen);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Comments a section of text
        /// </summary>
        /// <param name="data">the data to pad</param>
        /// <param name="padlen">the size of the pad</param>
        /// <returns></returns>
        public static string Comment(string data, int padlen)
        {
            string retVal = "";

            foreach (string line in data.Split('\n'))
            {
                retVal = retVal + Pad("{\\cf11//" + line + "}\\cf1\\par ", padlen);
            }

            return retVal;
        }

        /// <summary>
        ///     Comments an Icommentable
        /// </summary>
        /// <param name="element"></param>
        /// <param name="padlen"></param>
        /// <returns></returns>
        public static string Comment(ModelElement element, int padlen)
        {
            string retVal = "";

            ICommentable commentable = element as ICommentable;
            if (commentable != null && !string.IsNullOrEmpty(commentable.Comment))
            {
                retVal = Comment(commentable.Comment, padlen);
            }

            return retVal;
        }

        /// <summary>
        ///     The name of the element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="padlen"></param>
        /// <returns></returns>
        public static string Name(ModelElement element, int padlen)
        {
            string retVal = "";

            Namable namable = element as Namable;
            if (namable != null)
            {
                retVal = Pad("{\\cf11 // " + namable.Name + "}\\cf1\\par", padlen);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the header of the element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="padlen"></param>
        /// <returns></returns>
        public static string Header(ModelElement element, int padlen)
        {
            string retVal = "";

            retVal += Comment(element, padlen);
            retVal += Name(element, padlen);

            return retVal;
        }

        /// <summary>
        ///     Iterates the same character a given number of times
        /// </summary>
        /// <param name="c">the character to iterate</param>
        /// <param name="padlen">the size of the expected result</param>
        /// <returns></returns>
        public static string Iterate(char c, int len)
        {
            string retVal = "";
            for (int i = 0; i < len; i++)
            {
                retVal = retVal + c;
            }
            return retVal;
        }

        /// <summary>
        ///     The kind of opening brace
        /// </summary>
        private enum BraceType
        {
            command,
            character
        };

        /// <summary>
        ///     Adds RTF prefixes and postfixes
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encapsule(string data)
        {
            string retVal = data;

            if (!retVal.StartsWith("{\\rtf"))
            {
                // Replaces all end of lines with \par
                retVal = retVal.Replace("\n", "\\par ");

                // Replaces braces
                // Hyp : Braces are always balanced
                Stack<BraceType> braces = new Stack<BraceType>();
                StringBuilder tmp = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    if (retVal[i] == '{')
                    {
                        if (i < retVal.Length - 4)
                        {
                            if (retVal[i + 1] == '{')
                            {
                                tmp.Append("{");
                                braces.Push(BraceType.command);
                            }
                            if (retVal[i + 1] == ' ')
                            {
                                tmp.Append("{");
                                braces.Push(BraceType.command);
                            }
                            else if (retVal[i + 1] != '\\')
                            {
                                tmp.Append("\\{");
                                braces.Push(BraceType.character);
                            }
                            else if (retVal.Substring(i + 2, 3) == "par")
                            {
                                tmp.Append("\\{ ");
                                braces.Push(BraceType.character);
                            }
                            else
                            {
                                tmp.Append("{");
                                braces.Push(BraceType.command);
                            }
                        }
                        else
                        {
                            tmp.Append("\\{");
                            braces.Push(BraceType.character);
                        }
                    }
                    else if (retVal[i] == '}')
                    {
                        if (braces.Count > 0)
                        {
                            BraceType braceType = braces.Pop();
                            switch (braceType)
                            {
                                case BraceType.character:
                                    tmp.Append("\\}");
                                    break;

                                case BraceType.command:
                                    tmp.Append("}");
                                    break;
                            }
                        }
                        else
                        {
                            tmp.Append("\\}");
                        }
                    }
                    else
                    {
                        tmp.Append(retVal[i]);
                    }
                }
                retVal = tmp.ToString();

                // This is used to ensure that the right style is used for the text
                retVal = "{\\cf11}\\cf1 " + retVal;

                // Common prefix to handle the colors
                retVal =
                    "{\\rtf1\\ansi{\\colortbl;\\red0\\green0\\blue0;\\red0\\green0\\blue255;\\red0\\green255\\blue255;\\red0\\green255\\blue0;\\red255\\green0\\blue255;\\red255\\green0\\blue0;\\red255\\green255\\blue0;\\red255\\green255\\blue255;\\red0\\green0\\blue128;\\red0\\green128\\blue128;\\red0\\green128\\blue0;\\red128\\green0\\blue128;\\red128\\green0\\blue0;\\red128\\green128\\blue0;\\red128\\green128\\blue128;\\red192\\green192\\blue192;}" +
                    retVal + "}";
            }

            return retVal;
        }
    }
}