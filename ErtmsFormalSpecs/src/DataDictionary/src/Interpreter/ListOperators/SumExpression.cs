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

using DataDictionary.Generated;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Values;
using Utils;
using Collection = DataDictionary.Types.Collection;
using Range = DataDictionary.Types.Range;
using Type = DataDictionary.Types.Type;
using Variable = DataDictionary.Variables.Variable;

namespace DataDictionary.Interpreter.ListOperators
{
    public class SumExpression : ExpressionBasedListExpression
    {
        /// <summary>
        ///     The operator for this expression
        /// </summary>
        public static string Operator = "SUM";

        /// <summary>
        ///     The accumulator variable
        /// </summary>
        public Variable AccumulatorVariable { get; private set; }

        /// <summary>
        ///     The accumulation expression, as defined in the statement
        /// </summary>
        private Expression DefinedAccumulator { get; set; }

        /// <summary>
        ///     The accumulator expression to be used for evaluation
        /// </summary>
        public Expression Accumulator { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="log"></param>
        /// <param name="listExpression"></param>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <param name="root">the root element for which this expression should be parsed</param>
        /// <param name="iteratorVariableName"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public SumExpression(ModelElement root, ModelElement log, Expression listExpression, string iteratorVariableName,
            Expression condition, Expression expression, int start, int end)
            : base(root, log, listExpression, iteratorVariableName, condition, expression, start, end)
        {
            AccumulatorVariable = (Variable) acceptor.getFactory().createVariable();
            AccumulatorVariable.Enclosing = this;
            AccumulatorVariable.Name = "RESULT";
            ISubDeclaratorUtils.AppendNamable(this, AccumulatorVariable);

            DefinedAccumulator = expression;
            DefinedAccumulator.Enclosing = this;

            Accumulator = new BinaryExpression(Root, RootLog, DefinedAccumulator, BinaryExpression.Operator.Add,
                new UnaryExpression(Root, RootLog,
                    new Term(Root, RootLog, new Designator(Root, RootLog, "RESULT", -1, -1), -1, -1), -1, -1), -1, -1)
            {
                Enclosing = this
            };
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
                // Accumulator
                AccumulatorVariable.Type = GetExpressionType();

                DefinedAccumulator.SemanticAnalysis(instance, AllMatches.INSTANCE);

                Accumulator.SemanticAnalysis(instance, AllMatches.INSTANCE);
                StaticUsage.AddUsages(Accumulator.StaticUsage, Usage.ModeEnum.Read);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            return IteratorExpression.GetExpressionType();
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        protected internal override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            IValue retVal = null;

            ListValue value = ListExpression.GetValue(context, explain) as ListValue;
            if (value != null)
            {
                int token = PrepareIteration(context);
                context.LocalScope.setVariable(AccumulatorVariable);

                Type resultType = GetExpressionType();
                if (resultType != null)
                {
                    AccumulatorVariable.Value = resultType.getValue("0");
                    foreach (IValue v in value.Val)
                    {
                        if (v != EFSSystem.EmptyValue)
                        {
                            ElementFound = true;
                            IteratorVariable.Value = v;
                            if (conditionSatisfied(context, explain))
                            {
                                MatchingElementFound = true;
                                AccumulatorVariable.Value = Accumulator.GetValue(context, explain);
                            }
                        }
                        NextIteration();
                    }
                }
                EndIteration(context, explain, token);

                retVal = AccumulatorVariable.Value;
            }

            return retVal;
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            explanation.Write(Operator);
            explanation.Write(" ");
            ListExpression.GetExplain(explanation);

            if (Condition != null)
            {
                explanation.Write(" | ");
                Condition.GetExplain(explanation);
            }

            explanation.Write(" USING ");
            explanation.Write(IteratorVariable.Name);
            explanation.Write(" IN ");
            IteratorExpression.GetExplain(explanation);
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public override void CheckExpression()
        {
            base.CheckExpression();

            Collection listExpressionType = ListExpression.GetExpressionType() as Collection;
            if (listExpressionType != null)
            {
                IteratorExpression.CheckExpression();
            }

            Accumulator.CheckExpression();
            if (!(DefinedAccumulator.GetExpressionType() is Range))
            {
                AddError("Accumulator expression should be a range");
            }
        }
    }
}