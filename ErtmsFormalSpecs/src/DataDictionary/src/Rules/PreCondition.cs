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
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Types;
using Utils;
using Case = DataDictionary.Functions.Case;
using Function = DataDictionary.Functions.Function;
using NameSpace = DataDictionary.Types.NameSpace;
using Structure = DataDictionary.Types.Structure;
using Translation = DataDictionary.Tests.Translations.Translation;

namespace DataDictionary.Rules
{
    public class PreCondition : Generated.PreCondition, IExpressionable, ITextualExplain, ICommentable
    {
        /// <summary>
        ///     The precondition display name
        /// </summary>
        public override string Name
        {
            get { return ExpressionText; }
            set { }
        }

        /// <summary>
        ///     The precondition condition
        /// </summary>
        public string Condition
        {
            get { return getCondition(); }
            set
            {
                setCondition(value);
                __expression = null;
            }
        }

        public override string ExpressionText
        {
            get
            {
                string retVal = Condition;

                if (retVal == null)
                {
                    retVal = "";
                }

                return retVal;
            }
            set
            {
                Condition = value;
                __expression = null;
            }
        }

        /// <summary>
        ///     Provides the expression tree associated to this action's expression
        /// </summary>
        private Expression __expression;

        public Expression Expression
        {
            get
            {
                if (__expression == null)
                {
                    __expression = EFSSystem.Parser.Expression(this, ExpressionText);
                }

                return __expression;
            }
            set { __expression = value; }
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
        ///     The enclosing rule, if any
        /// </summary>
        public Rule Rule
        {
            get { return Enclosing as Rule; }
        }

        /// <summary>
        ///     The enclosing rule condition, if any
        /// </summary>
        public RuleCondition RuleCondition
        {
            get { return Enclosing as RuleCondition; }
        }

        /// <summary>
        ///     Finds the enclosing structure
        /// </summary>
        public Structure EnclosingStructure
        {
            get { return EnclosingFinder<Structure>.find(this); }
        }

        /// <summary>
        ///     Finds the enclosing function
        /// </summary>
        public Function EnclosingFunction
        {
            get { return EnclosingFinder<Function>.find(this); }
        }

        /// <summary>
        ///     Finds the enclosing namespace
        /// </summary>
        public NameSpace NameSpace
        {
            get { return EnclosingNameSpaceFinder.find(this); }
        }

        /// <summary>
        ///     The enclosing translation, if any
        /// </summary>
        public Translation Translation
        {
            get { return Enclosing as Translation; }
        }

        /// <summary>
        ///     The enclosing test step
        /// </summary>
        public Case Case
        {
            get { return Enclosing as Case; }
        }

        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = null;

                if (RuleCondition != null)
                {
                    retVal = RuleCondition.PreConditions;
                }
                else if (Case != null)
                {
                    retVal = Case.PreConditions;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Indicates whether this preCondition reads the variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public bool Reads(ITypedElement variable)
        {
            bool retVal = false;

            if (Expression != null)
            {
                foreach (ITypedElement el in Expression.GetVariables())
                {
                    if (el == variable)
                    {
                        retVal = true;
                        break;
                    }
                }

                if (!retVal)
                {
                    Call call = Expression as Call;
                    if (call != null)
                    {
                        retVal = call.Reads(variable);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Finds the variable checked by the precondition
        /// </summary>
        /// <returns></returns>
        public string findVariable()
        {
            string[] words = Condition.Split(' ');
            if (words.Length > 0)
            {
                return words[0];
            }
            return null;
        }

        /// <summary>
        ///     Finds the operator checked by the precondition
        /// </summary>
        /// <returns></returns>
        public string findOperator()
        {
            string[] words = Condition.Split(' ');
            if (words.Length > 1)
            {
                return words[1];
            }
            return null;
        }

        /// <summary>
        ///     Finds the operator checked by the precondition
        /// </summary>
        /// <returns></returns>
        public string findValue()
        {
            string[] words = Condition.Split(' ');
            if (words.Length > 2)
            {
                return words[2];
            }
            return null;
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Comment(this);
            explanation.Expression(this);
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
        }

        /// <summary>
        ///     Duplicates this model element
        /// </summary>
        /// <returns></returns>
        public PreCondition duplicate()
        {
            PreCondition retVal = (PreCondition) acceptor.getFactory().createPreCondition();
            retVal.Name = Name;
            retVal.ExpressionText = ExpressionText;

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