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
using DataDictionary.Interpreter.Filter;
using DataDictionary.Types;
using DataDictionary.Values;
using Utils;

namespace DataDictionary.Interpreter.ListOperators
{
    public abstract class ConditionBasedListExpression : ListOperatorExpression
    {
        /// <summary>
        ///     The condition used for THERE_IS, FORALL, FIRST, LAST
        /// </summary>
        public Expression Condition { get; private set; }

        /// <summary>
        ///     Constructor for THERE_IS, FORALL, FIRST, LAST
        /// </summary>
        /// <param name="listExpression"></param>
        /// <param name="condition"></param>
        /// <param name="root">the root element for which this expression should be parsed</param>
        /// <param name="iteratorVariableName"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public ConditionBasedListExpression(ModelElement root, ModelElement log, Expression listExpression,
            string iteratorVariableName, Expression condition, int start, int end)
            : base(root, log, listExpression, iteratorVariableName, start, end)
        {
            Condition = condition;
            if (Condition != null)
            {
                Condition.Enclosing = this;
            }
        }

        /// <summary>
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <paraparam name="expectation">Indicates the kind of element we are looking for</paraparam>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(INamable instance, BaseFilter expectation)
        {
            bool retVal = base.SemanticAnalysis(instance, expectation);

            if (retVal)
            {
                // Condition
                if (Condition != null)
                {
                    Condition.SemanticAnalysis(instance, expectation);
                    StaticUsage.AddUsages(Condition.StaticUsage, Usage.ModeEnum.Read);
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
            base.Fill(retVal, filter);

            if (Condition != null)
            {
                Condition.Fill(retVal, filter);
            }
        }

        /// <summary>
        ///     Indicates whether the condition is satisfied with the value provided
        ///     Hyp : the value of the iterator variable has been assigned before
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public bool conditionSatisfied(InterpretationContext context, ExplanationPart explain)
        {
            bool retVal = true;

            if (Condition != null)
            {
                BoolValue b = Condition.GetValue(context, explain) as BoolValue;
                if (b == null)
                {
                    retVal = false;
                }
                else
                {
                    retVal = b.Val;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public override void CheckExpression()
        {
            base.CheckExpression();

            Type conditionType = null;
            if (Condition != null)
            {
                Condition.CheckExpression();
                conditionType = Condition.GetExpressionType() as BoolType;
                if (conditionType == null)
                {
                    AddError("Conditions on list expressions should be a predicate (return a boolean value)");
                }
            }
        }
    }
}