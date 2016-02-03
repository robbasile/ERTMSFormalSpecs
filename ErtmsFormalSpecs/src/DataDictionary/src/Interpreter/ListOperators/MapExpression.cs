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
using DataDictionary.Values;
using Collection = DataDictionary.Types.Collection;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter.ListOperators
{
    public class MapExpression : ExpressionBasedListExpression
    {
        /// <summary>
        ///     The operator for this expression
        /// </summary>
        public static string Operator = "MAP";

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root"></param>
        /// <param name="log"></param>
        /// <param name="listExpression"></param>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <param name="iteratorVariableName"></param>
        /// <param name="parsingData">Additional information about the parsing process</param>
        public MapExpression(ModelElement root, ModelElement log, Expression listExpression, string iteratorVariableName,
            Expression condition, Expression expression, ParsingData parsingData)
            : base(root, log, listExpression, iteratorVariableName, condition, expression, parsingData)
        {
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            Type retVal = null;

            Type iteratorType = IteratorExpression.GetExpressionType();
            if (iteratorType != null)
            {
                Collection collection = (Collection) acceptor.getFactory().createCollection();
                collection.Enclosing = EfsSystem.Instance;
                collection.Type = iteratorType;
                Collection originalListType = ListExpression.GetExpressionType() as Collection;
                if ( originalListType != null )
                {
                    collection.setMaxSize(originalListType.getMaxSize());
                }

                retVal = collection;
            }
            else
            {
                AddError("Cannot evaluate iterator type for " + ToString());
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
            ListValue retVal = null;

            ListValue value = ListExpression.GetValue(context, explain) as ListValue;
            if (value != null)
            {
                int token = PrepareIteration(context);
                retVal = new ListValue((Collection) GetExpressionType(), new List<IValue>());
                List<IValue> range = new List<IValue>(value.Val);
                foreach (IValue v in range)
                {
                    if (v != EfsSystem.Instance.EmptyValue)
                    {
                        // All elements should always be != from EmptyValue
                        ElementFound = true;
                        IteratorVariable.Value = v;

                        if (ConditionSatisfied(context, explain))
                        {
                            MatchingElementFound = true;
                            retVal.Val.Add(IteratorExpression.GetValue(context, explain));
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

            IteratorExpression.CheckExpression();
        }
    }
}