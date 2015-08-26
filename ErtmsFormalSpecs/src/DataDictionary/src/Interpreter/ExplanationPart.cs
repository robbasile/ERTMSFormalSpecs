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

using System.Collections.Generic;
using DataDictionary.Functions;
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Values;
using Utils;

namespace DataDictionary.Interpreter
{
    /// <summary>
    ///     Part of the explanation of an evaluation
    /// </summary>
    public class ExplanationPart
    {
        /// <summary>
        ///     The explanation message
        /// </summary>
        private string _message;

        public string Message
        {
            get
            {
                string retVal = _message;
                if (retVal == null)
                {
                    retVal = "";
                }

                if (Change != null)
                {
                    if (Change.NewValue != null)
                    {
                        retVal += Change.Variable.FullName + " <- " + explainNamable(Change.NewValue);
                    }
                    else
                    {
                        retVal += Change.Variable.FullName + " <- <cannot evaluate value>";
                    }
                }

                if (LeftPart != null)
                {
                    retVal += ExplainLeftPart(LeftPart);
                }

                if (RightPart != null && !(RightPart is Procedure))
                {
                    if (LeftPart is RuleCondition)
                    {
                        if (RightPart == EFSSystem.INSTANCE.BoolType.True)
                        {
                            retVal = "SATISFIED " + retVal;
                        }
                        else
                        {
                            retVal = "NOT APPLICABLE " + retVal;
                        }
                    }
                    else
                    {
                        retVal += explainNamable(RightPart);
                    }
                }

                return retVal;
            }
            set { _message = value; }
        }

        /// <summary>
        ///     Explains a left part
        /// </summary>
        /// <param name="leftPart"></param>
        /// <returns></returns>
        private string ExplainLeftPart(object leftPart)
        {
            string retVal;

            INamable namable = leftPart as INamable;
            if (namable != null)
            {
                retVal = namable.Name;
                if (namable is Function)
                {
                    retVal += "(...)";
                }
                else if (namable is Procedure)
                {
                    retVal += "(...)";
                    ITypedElement instance = RightPart as ITypedElement;
                    if (instance != null)
                    {
                        retVal = instance.Type.Name + "." + retVal;
                    }
                }
                else if (namable is Case)
                {
                    retVal = "CASE " + retVal;
                }
            }
            else
            {
                retVal = leftPart.ToString();
            }

            if (RightPart != null && !(leftPart is RuleCondition) && !(leftPart is Procedure))
            {
                retVal += " = ";
            }

            return retVal;
        }

        /// <summary>
        ///     The list of sub explanations
        /// </summary>
        public List<ExplanationPart> SubExplanations { get; private set; }

        /// <summary>
        ///     The model element for which the explanation is created
        /// </summary>
        public ModelElement Element { get; private set; }

        /// <summary>
        ///     The (optional) change for which this explanation part is created
        /// </summary>
        public Change Change { get; private set; }

        /// <summary>
        ///     The (optional) left part of the explanationfor which this explanation part is created
        /// </summary>
        public object LeftPart { get; set; }

        /// <summary>
        ///     The (optional) value (right part) for which this explanation part is created
        /// </summary>
        public INamable RightPart { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="element">The element for which this explanation part is created</param>
        /// <param name="message">The message to display. MAKE SURE you do not use string concatenation to create this message</param>
        /// <param name="rightPart">The value associated to the message, if any</param>
        public ExplanationPart(ModelElement element, string message, INamable rightPart = null)
        {
            Element = element;
            Message = message;
            RightPart = rightPart;
            SubExplanations = new List<ExplanationPart>();
        }

        /// <summary>
        ///     Constructor for an explanation, based on a change
        /// </summary>
        /// <param name="element">The element for which this explanation part is created</param>
        /// <param name="change">The change performed</param>
        public ExplanationPart(ModelElement element, Change change)
        {
            Element = element;
            Change = change;
            SubExplanations = new List<ExplanationPart>();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="element">The element for which this explanation part is created</param>
        /// <param name="leftPart">The left path to associate to this explanation</param>
        /// <param name="rightPart">The value associate to this left part</param>
        public ExplanationPart(ModelElement element, object leftPart, INamable rightPart = null)
        {
            Element = element;
            LeftPart = leftPart;
            RightPart = rightPart;
            SubExplanations = new List<ExplanationPart>();
        }


        /// <summary>
        ///     Provides the textual representation of the namable provided
        /// </summary>
        /// <param name="namable"></param>
        /// <returns></returns>
        private string explainNamable(INamable namable)
        {
            string retVal = "";

            if (namable != null)
            {
                retVal = namable.Name;

                Function fonction = namable as Function;
                if (fonction != null)
                {
                    if (fonction.Graph != null)
                    {
                        retVal = fonction.Graph.ToString();
                    }
                    else if (fonction.Surface != null)
                    {
                        retVal = fonction.Surface.ToString();
                    }
                }
                else
                {
                    IValue value = namable as IValue;
                    if (value != null)
                    {
                        retVal = value.LiteralName;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Adds a sub explanation for the explain provided as parameter
        /// </summary>
        /// <param name="explain"></param>
        /// <param name="subExplain"></param>
        public static void AddSubExplanation(ExplanationPart explain, ExplanationPart subExplain)
        {
            if (explain != null)
            {
                explain.SubExplanations.Add(subExplain);
            }
        }

        /// <summary>
        ///     Creates a sub explanation for the explain provided as parameter
        /// </summary>
        /// <param name="explain"></param>
        /// <param name="leftPart">What is evaluated</param>
        /// <param name="rightPart">The evaluated value</param>
        /// <returns></returns>
        public static ExplanationPart CreateSubExplanation(ExplanationPart explain, object leftPart,
            INamable rightPart = null)
        {
            ExplanationPart retVal = null;

            if (explain != null)
            {
                retVal = new ExplanationPart(explain.Element, leftPart, rightPart);
                explain.SubExplanations.Add(retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     Creates a sub explanation for the explain provided as parameter
        /// </summary>
        /// <param name="explain"></param>
        /// <param name="message"></param>
        /// <param name="leftPart"></param>
        /// <returns></returns>
        public static ExplanationPart CreateNamedSubExplanation(ExplanationPart explain, string message, object leftPart)
        {
            ExplanationPart retVal = null;

            if (explain != null)
            {
                retVal = new ExplanationPart(explain.Element, message);
                retVal.LeftPart = leftPart;
                explain.SubExplanations.Add(retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     Creates a sub explanation for the explain provided as parameter
        /// </summary>
        /// <param name="explain"></param>
        /// <param name="root"></param>
        /// <param name="change"></param>
        /// <returns></returns>
        public static ExplanationPart CreateSubExplanation(ExplanationPart explain, ModelElement root, Change change)
        {
            ExplanationPart retVal = null;

            if (explain != null)
            {
                retVal = new ExplanationPart(root, change);
                explain.SubExplanations.Add(retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     Creates a sub explanation for the explain provided as parameter
        /// </summary>
        /// <param name="explain"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ExplanationPart CreateSubExplanation(ExplanationPart explain, string name)
        {
            ExplanationPart retVal = null;

            if (explain != null)
            {
                retVal = new ExplanationPart(explain.Element, name);
                explain.SubExplanations.Add(retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     Sets the value associated to the explanation
        /// </summary>
        /// <param name="explain"></param>
        /// <param name="namable"></param>
        public static void SetNamable(ExplanationPart explain, INamable namable)
        {
            if (explain != null)
            {
                explain.RightPart = namable;
            }
        }
    }
}