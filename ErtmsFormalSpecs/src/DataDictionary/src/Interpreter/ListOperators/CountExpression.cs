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

using DataDictionary.Types;
using DataDictionary.Values;

namespace DataDictionary.Interpreter.ListOperators
{
    public class CountExpression : ConditionBasedListExpression
    {
        /// <summary>
        ///     The operator for this expression
        /// </summary>
        public static string Operator = "COUNT";

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="log"></param>
        /// <param name="listExpression"></param>
        /// <param name="condition"></param>
        /// <param name="root">the root element for which this expression should be parsed</param>
        /// <param name="iteratorVariableName"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public CountExpression(ModelElement root, ModelElement log, Expression listExpression,
            string iteratorVariableName, Expression condition, int start, int end)
            : base(root, log, listExpression, iteratorVariableName, condition, start, end)
        {
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            return EfsSystem.Instance.IntegerType;
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        protected internal override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            int count = 0;
            ListValue value = ListExpression.GetValue(context, explain) as ListValue;
            if (value != null)
            {
                int token = PrepareIteration(context);
                foreach (IValue v in value.Val)
                {
                    if (v != EfsSystem.Instance.EmptyValue)
                    {
                        ElementFound = true;
                        IteratorVariable.Value = v;
                        if (ConditionSatisfied(context, explain))
                        {
                            MatchingElementFound = true;
                            count += 1;
                        }
                    }
                    NextIteration();
                }
                EndIteration(context, explain, token);
            }

            return new IntValue(EfsSystem.Instance.IntegerType, count);
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
            explanation.Write(IteratorVariable.Name);
            explanation.Write(" IN ");
            ListExpression.GetExplain(explanation);

            if (Condition != null)
            {
                explanation.Write(" | ");
                Condition.GetExplain(explanation);
            }
        }
    }
}