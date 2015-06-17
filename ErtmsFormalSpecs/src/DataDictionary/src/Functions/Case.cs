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

using System.Collections;
using System.Collections.Generic;
using DataDictionary.Interpreter;
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;

namespace DataDictionary.Functions
{
    public class Case : Generated.Case, TextualExplain, IExpressionable, ICommentable
    {
        private Expression expression;

        /// <summary>
        ///     The enclosing function
        /// </summary>
        public Function EnclosingFunction
        {
            get { return Enclosing as Function; }
        }


        /// <summary>
        ///     Pre-conditions of the case
        /// </summary>
        public ArrayList PreConditions
        {
            get
            {
                ArrayList retVal = allPreConditions();
                if (retVal == null)
                    retVal = new ArrayList();
                return retVal;
            }
            set { this.setAllPreConditions(value); }
        }

        /// <summary>
        ///     Expression of the case as string
        /// </summary>
        public override string ExpressionText
        {
            get
            {
                if (getExpression() == null)
                {
                    setExpression("");
                }
                return getExpression();
            }
            set
            {
                setExpression(value);
                expression = null;
            }
        }

        /// <summary>
        ///     Expression of the case
        /// </summary>
        public Expression Expression
        {
            get
            {
                if (expression == null)
                {
                    expression = EFSSystem.Parser.Expression(this, ExpressionText);
                }
                return expression;
            }
            set { expression = value; }
        }

        public InterpreterTreeNode Tree
        {
            get { return Expression; }
        }


        /// <summary>
        ///     Clears the expression tree to ensure new compilation
        /// </summary>
        public void CleanCompilation()
        {
            Expression = null;
        }

        /// <summary>
        ///     Creates the tree according to the expression text
        /// </summary>
        public InterpreterTreeNode Compile()
        {
            // Side effect, builds the statement if it is not already built
            return Tree;
        }

        /// <summary>
        ///     Indicates that the expression is valid for this IExpressionable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool checkValidExpression(string expression)
        {
            bool retVal = false;

            Expression tree = EFSSystem.Parser.Expression(this, expression, null, false, null, true);
            retVal = tree != null;

            return retVal;
        }

        /// <summary>
        ///     The enclosing collection of the parameter
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get { return EnclosingFinder<Function>.find(this).Cases; }
        }

        /// <summary>
        ///     Expression of the case
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        public bool EvaluatePreConditions(InterpretationContext context, ExplanationPart explain)
        {
            bool retVal = true;
            foreach (PreCondition preCondition in PreConditions)
            {
                Expression expression = preCondition.Expression;
                BoolValue value = expression.GetValue(context, explain) as BoolValue;

                if (value != null)
                {
                    retVal = retVal && value.Val;
                }
                else
                {
                    retVal = false;
                }

                if (!retVal)
                {
                    break;
                }
            }
            return retVal;
        }

        /// <summary>
        ///     Provides an explanation of the rule's behaviour
        /// </summary>
        /// <param name="indentLevel">the number of white spaces to add at the beginning of each line</param>
        /// <returns></returns>
        public string getExplain(int indentLevel)
        {
            string retVal = "";
            if (PreConditions.Count > 0)
            {
                retVal = retVal + " {\\b IF }";
                bool first = true;
                foreach (PreCondition preCondition in PreConditions)
                {
                    if (first)
                    {
                        retVal = retVal + TextualExplainUtilities.Pad(preCondition.getExplain(true), 0);
                        first = false;
                    }
                    else
                    {
                        retVal = retVal + " {\\b AND} " + preCondition.getExplain(true);
                    }
                }
                retVal = retVal + " {\\b THEN }\\par";
                retVal = retVal + TextualExplainUtilities.Header(this, indentLevel);
                retVal = retVal + TextualExplainUtilities.Expression(this, indentLevel + 2);
            }
            else
            {
                retVal = retVal + TextualExplainUtilities.Header(this, indentLevel);
                retVal = retVal + TextualExplainUtilities.Expression(this, indentLevel + 2);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides an explanation of the rule's behaviour
        /// </summary>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        public string getExplain(bool explainSubElements)
        {
            string retVal = getExplain(0);

            return TextualExplainUtilities.Encapsule(retVal);
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                PreCondition item = element as PreCondition;
                if (item != null)
                {
                    appendPreConditions(item);
                }
            }
        }


        public bool Read(ITypedElement variable)
        {
            bool retVal = false;

            foreach (PreCondition preCondition in PreConditions)
            {
                if (preCondition.Reads(variable))
                {
                    retVal = true;
                    break;
                }
            }

            if (!retVal && Expression != null)
            {
                foreach (IVariable var in Expression.GetVariables())
                {
                    if (var == variable)
                    {
                        retVal = true;
                        break;
                    }
                }
            }

            return retVal;
        }

        public List<IValue> GetLiterals()
        {
            List<IValue> retVal = new List<IValue>();

            foreach (PreCondition preCondition in PreConditions)
            {
                if (preCondition.Expression != null)
                {
                    retVal.AddRange(preCondition.Expression.GetLiterals());
                }
            }

            if (expression != null)
            {
                retVal.AddRange(Expression.GetLiterals());
            }

            return retVal;
        }

        /// <summary>
        ///     The comment related to this element
        /// </summary>
        public string Comment
        {
            get { return getComment(); }
            set { setComment(value); }
        }
    }
}