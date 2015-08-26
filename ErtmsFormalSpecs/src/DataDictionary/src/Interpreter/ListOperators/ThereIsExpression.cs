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
    public class ThereIsExpression : ConditionBasedListExpression
    {
        /// <summary>
        ///     The operator for this expression
        /// </summary>
        public static string Operator = "THERE_IS";

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">the root element for which this expression should be parsed</param>
        /// <param name="log"></param>
        /// <param name="listExpression"></param>
        /// <param name="iteratorVariableName"></param>
        /// <param name="condition"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public ThereIsExpression(ModelElement root, ModelElement log, Expression listExpression,
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
            return EfsSystem.Instance.BoolType;
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
                retVal = EfsSystem.Instance.BoolType.False;
                foreach (IValue v in value.Val)
                {
                    if (v != EfsSystem.Instance.EmptyValue)
                    {
                        ElementFound = true;
                        IteratorVariable.Value = v;
                        if (Condition != null)
                        {
                            BoolValue b = Condition.GetValue(context, explain) as BoolValue;
                            if (b != null && b.Val)
                            {
                                MatchingElementFound = true;
                                retVal = EfsSystem.Instance.BoolType.True;
                                break;
                            }
                        }
                        else
                        {
                            retVal = EfsSystem.Instance.BoolType.True;
                            break;
                        }
                    }
                    NextIteration();
                }
                EndIteration(context, explain, token);
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