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

using DataDictionary.Interpreter.Filter;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Collection = DataDictionary.Types.Collection;
using Range = DataDictionary.Types.Range;
using Type = DataDictionary.Types.Type;

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
        public IVariable AccumulatorVariable { get; private set; }

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
        /// <param name="parsingData">Additional information about the parsing process</param>
        public SumExpression(ModelElement root, ModelElement log, Expression listExpression, string iteratorVariableName,
            Expression condition, Expression expression, ParsingData parsingData)
            : base(root, log, listExpression, iteratorVariableName, condition, expression, parsingData)
        {
            AccumulatorVariable = CreateBoundVariable("RESULT", null);
            ISubDeclaratorUtils.AppendNamable(this, AccumulatorVariable);

            if (expression != null)
            {
                DefinedAccumulator = SetEnclosed(expression);
                Accumulator =
                    SetEnclosed(new BinaryExpression(
                        Root,
                        RootLog,
                        DefinedAccumulator,
                        BinaryExpression.Operator.Add,
                        new UnaryExpression(
                            Root,
                            RootLog,
                            new Term(
                                Root,
                                RootLog,
                                new Designator(Root, RootLog, "RESULT", ParsingData.SyntheticElement),
                                ParsingData.SyntheticElement),
                            ParsingData.SyntheticElement),
                        ParsingData.SyntheticElement));
            }
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

                if (DefinedAccumulator != null)
                {
                    DefinedAccumulator.SemanticAnalysis(instance, AllMatches.INSTANCE);
               
                    Accumulator.SemanticAnalysis(instance, AllMatches.INSTANCE);
                    StaticUsage.AddUsages(Accumulator.StaticUsage, Usage.ModeEnum.Read);
                }
                else
                {
                    AddError("Accumulator expression not provided");
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            Type retVal = null;

            if (IteratorExpression != null)
            {
                retVal = IteratorExpression.GetExpressionType();
            }

            return retVal;
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
                context.LocalScope.SetVariable(AccumulatorVariable);

                Type resultType = GetExpressionType();
                if (resultType != null)
                {
                    AccumulatorVariable.Value = resultType.getValue("0");
                    foreach (IValue v in value.Val)
                    {
                        if (v != EfsSystem.Instance.EmptyValue)
                        {
                            // All elements should always be != from EmptyValue
                            ElementFound = true;
                            IteratorVariable.Value = v;
                            if (ConditionSatisfied(context, explain))
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