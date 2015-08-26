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
using DataDictionary.Generated;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Values;
using Utils;
using Type = DataDictionary.Types.Type;
using Variable = DataDictionary.Variables.Variable;

namespace DataDictionary.Interpreter
{
    /// <summary>
    ///     LET variable &lt;- expression IN expression
    ///     LET variable '=>' expression IN expression
    /// </summary>
    public class LetExpression : Expression, ISubDeclarator
    {
        /// <summary>
        ///     The variable bound by the LET expression
        /// </summary>
        public Variable BoundVariable { get; private set; }

        /// <summary>
        ///     The binding expression
        /// </summary>
        public Expression BindingExpression { get; private set; }

        /// <summary>
        ///     The expression to be evaluated
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">the root element for which this expression should be parsed</param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        /// <param name="log"></param>
        /// <param name="boundVariableName">The name of the bound variable</param>
        /// <param name="bindingExpression">The binding expression which provides the value of the variable</param>
        /// <param name="expression">The expression to be evaluated</param>
        public LetExpression(ModelElement root, ModelElement log, string boundVariableName, Expression bindingExpression,
            Expression expression, int start, int end)
            : base(root, log, start, end)
        {
            BoundVariable = (Variable) acceptor.getFactory().createVariable();
            BoundVariable.Enclosing = this;
            BoundVariable.Name = boundVariableName;

            BindingExpression = SetEnclosed(bindingExpression);
            Expression = SetEnclosed(expression);

            InitDeclaredElements();
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();

            ISubDeclaratorUtils.AppendNamable(this, BoundVariable);
        }

        /// <summary>
        ///     The elements declared by this declarator
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements { get; private set; }

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
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <param name="expectation">Indicates the kind of element we are looking for</param>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(INamable instance, BaseFilter expectation)
        {
            bool retVal = base.SemanticAnalysis(instance, expectation);

            if (retVal)
            {
                // Binding expression
                if (BindingExpression != null)
                {
                    BindingExpression.SemanticAnalysis(instance, IsRightSide.INSTANCE);
                    StaticUsage.AddUsages(BindingExpression.StaticUsage, Usage.ModeEnum.Read);

                    Type bindingExpressionType = BindingExpression.GetExpressionType();
                    if (bindingExpressionType != null)
                    {
                        StaticUsage.AddUsage(bindingExpressionType, Root, Usage.ModeEnum.Type);
                        BoundVariable.Type = bindingExpressionType;

                    }
                    else
                    {
                        AddError("Cannot determine binding expression type for " + ToString());
                    }
                }

                // The evaluated expression
                if (Expression != null)
                {
                    Expression.SemanticAnalysis(instance, expectation);
                    StaticUsage.AddUsages(Expression.StaticUsage, Usage.ModeEnum.Read);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public override void Fill(List<INamable> retVal, BaseFilter filter)
        {
            BindingExpression.Fill(retVal, filter);
            Expression.Fill(retVal, filter);
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public override void CheckExpression()
        {
            base.CheckExpression();

            BindingExpression.CheckExpression();
            Expression.CheckExpression();
        }


        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            return Expression.GetExpressionType();
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        protected internal override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            ExplanationPart subPart = ExplanationPart.CreateSubExplanation(explain, BoundVariable);
            BoundVariable.Value = BindingExpression.GetValue(context, explain);
            ExplanationPart.SetNamable(subPart, BoundVariable.Value);

            int token = context.LocalScope.PushContext();
            context.LocalScope.SetVariable(BoundVariable);
            IValue retVal = Expression.GetValue(context, explain);
            context.LocalScope.PopContext(token);

            return retVal;
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            explanation.Write("LET ");
            explanation.Write(BoundVariable.Name);
            explanation.Write(" <- ");
            BindingExpression.GetExplain(explanation);
            explanation.Write(" IN ");
            Expression.GetExplain(explanation);
        }
    }
}