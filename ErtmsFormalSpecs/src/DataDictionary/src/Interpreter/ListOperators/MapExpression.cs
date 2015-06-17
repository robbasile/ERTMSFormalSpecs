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
        public static string OPERATOR = "MAP";

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="listExpression"></param>
        /// <param name="condition"></param>
        /// <param name="function"></param>
        /// <param name="iteratorVariableName"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public MapExpression(ModelElement root, ModelElement log, Expression listExpression, string iteratorVariableName,
            Expression condition, Expression function, int start, int end)
            : base(root, log, listExpression, iteratorVariableName, condition, function, start, end)
        {
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            Type retVal = null;

            Type iteratorType = IteratorExpression.GetExpressionType();
            if (iteratorType != null)
            {
                Collection collection = (Collection) acceptor.getFactory().createCollection();
                collection.Enclosing = EFSSystem;
                collection.Type = iteratorType;

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
        public override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            ListValue retVal = null;

            ListValue value = ListExpression.GetValue(context, explain) as ListValue;
            if (value != null)
            {
                int token = PrepareIteration(context);
                retVal = new ListValue((Collection) GetExpressionType(), new List<IValue>());
                foreach (IValue v in value.Val)
                {
                    if (v != EFSSystem.EmptyValue)
                    {
                        ElementFound = true;
                        IteratorVariable.Value = v;

                        if (conditionSatisfied(context, explain))
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
        ///     Provides the indented expression text
        /// </summary>
        /// <param name="indentLevel"></param>
        /// <returns></returns>
        public override string ToString(int indentLevel)
        {
            string retVal = OPERATOR + " " + ListExpression.ToString(indentLevel);

            if (Condition != null)
            {
                retVal += " | " + Condition.ToString(indentLevel);
            }

            retVal = retVal + " USING " + IteratorVariable.Name + " IN " + IteratorExpression.ToString(indentLevel);

            return retVal;
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        /// <param name="context">The interpretation context</param>
        public override void checkExpression()
        {
            base.checkExpression();

            IteratorExpression.checkExpression();
        }
    }
}