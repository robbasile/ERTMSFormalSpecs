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
using DataDictionary.Interpreter;
using DataDictionary.Tests.Translations;
using Utils;

namespace DataDictionary.Tests
{
    public class Expectation : Generated.Expectation, IExpressionable, ITextualExplain
    {
        /// <summary>
        ///     The enclosing step, if any
        /// </summary>
        public Step Step
        {
            get { return SubStep.Step; }
        }

        /// <summary>
        ///     The enclosing sub-step, if any
        /// </summary>
        public SubStep SubStep
        {
            get { return Enclosing as SubStep; }
        }

        /// <summary>
        ///     The enclosing translation, if any
        /// </summary>
        public Translation Translation
        {
            get
            {
                Translation result = null;
                if (SubStep != null)
                {
                    result = SubStep.Translation;
                }
                return result;
            }
        }

        /// <summary>
        ///     The enclosing frame
        /// </summary>
        public Frame Frame
        {
            get { return EnclosingFinder<Frame>.find(this); }
        }

        /// <summary>
        ///     The expected value
        /// </summary>
        public string Value
        {
            get { return getValue(); }
            set
            {
                setValue(value);
                __expression = null;
            }
        }

        /// <summary>
        ///     Indicates if this expectation is blocking
        /// </summary>
        public bool Blocking
        {
            get { return getBlocking(); }
            set { setBlocking(value); }
        }

        /// <summary>
        ///     When blocking, this indicates the deadling before the expectation should be achieved
        /// </summary>
        public double DeadLine
        {
            get { return getDeadLine(); }
            set { setDeadLine(value); }
        }

        public override string ExpressionText
        {
            get
            {
                string retVal = Value;
                if (retVal == null)
                {
                    retVal = "";
                }
                return retVal;
            }
            set { Value = value; }
        }

        public Expression __expression;

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
        ///     Clears the tree to ensure new compilation
        /// </summary>
        public void CleanCompilation()
        {
            Expression = null;
            ConditionTree = null;
        }

        /// <summary>
        ///     Creates the tree according to the expression text
        /// </summary>
        public InterpreterTreeNode Compile()
        {
            // Side effect, builds the expressions if they are not already built
            Expression expression = ConditionTree;
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

            Expression tree = EFSSystem.Parser.Expression(this, expression);
            retVal = tree != null;

            return retVal;
        }

        public Expression conditionTree;

        public Expression ConditionTree
        {
            get
            {
                if (conditionTree == null && getCondition() != null)
                {
                    conditionTree = EFSSystem.Parser.Expression(this, getCondition());
                }
                return conditionTree;
            }
            set { conditionTree = value; }
        }

        public override string Name
        {
            get { return ExpressionText; }
            set { }
        }

        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = null;

                if (SubStep != null)
                {
                    retVal = SubStep.Expectations;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Indicates the name of the checked variable, if any
        /// </summary>
        /// <returns></returns>
        public Designator CheckedVariable()
        {
            Designator retVal = null;

            BinaryExpression binaryExpression = Expression as BinaryExpression;
            if (binaryExpression != null)
            {
                UnaryExpression unaryExpression = binaryExpression.Left as UnaryExpression;
                if (unaryExpression != null && unaryExpression.Term != null && unaryExpression.Term.Designator != null)
                {
                    retVal = unaryExpression.Term.Designator;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Comment(this);

            if (!string.IsNullOrEmpty(getCondition()))
            {
                explanation.Write("IF ");
                if (ConditionTree != null)
                {
                    ConditionTree.GetExplain(explanation);
                }
                else
                {
                    explanation.Write(getCondition());
                }
                explanation.WriteLine(" THEN");
                explanation.Indent(2, () => explanation.Expression(this));
                explanation.WriteLine("END IF");

            }
            else
            {
                explanation.Expression(this);
                explanation.WriteLine();
            }
        }
    }
}