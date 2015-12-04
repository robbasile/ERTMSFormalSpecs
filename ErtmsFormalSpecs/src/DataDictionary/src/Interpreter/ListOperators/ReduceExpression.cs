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

using System;
using DataDictionary.Functions;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Collection = DataDictionary.Types.Collection;
using Function = DataDictionary.Functions.Function;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter.ListOperators
{
    public class ReduceExpression : ExpressionBasedListExpression
    {
        /// <summary>
        ///     The operator for this expression
        /// </summary>
        public static string Operator = "REDUCE";

        /// <summary>
        ///     The reduce initial value
        /// </summary>
        public Expression InitialValue { get; private set; }

        /// <summary>
        ///     The accumulator variable
        /// </summary>
        public IVariable AccumulatorVariable { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">the root element for which this expression should be parsed</param>
        /// <param name="log"></param>
        /// <param name="listExpression"></param>
        /// <param name="iteratorVariableName"></param>
        /// <param name="condition"></param>
        /// <param name="function"></param>
        /// <param name="initialValue"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public ReduceExpression(ModelElement root, ModelElement log, Expression listExpression,
            string iteratorVariableName, Expression condition, Expression function, Expression initialValue, int start,
            int end)
            : base(root, log, listExpression, iteratorVariableName, condition, function, start, end)
        {
            InitialValue = SetEnclosed(initialValue);

            AccumulatorVariable = CreateBoundVariable("RESULT", null);
            ISubDeclaratorUtils.AppendNamable(this, AccumulatorVariable);
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
                if (InitialValue != null)
                {
                    InitialValue.SemanticAnalysis(instance, AllMatches.INSTANCE);
                    StaticUsage.AddUsages(InitialValue.StaticUsage, Usage.ModeEnum.Read);

                    AccumulatorVariable.Type = InitialValue.GetExpressionType();
                }
            }

            return retVal;
        }

        public override ICallable GetStaticCallable()
        {
            return InitialValue.GetStaticCallable();
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
                context.LocalScope.SetVariable(AccumulatorVariable);
                AccumulatorVariable.Value = InitialValue.GetValue(context, explain);
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
                            AccumulatorVariable.Value = IteratorExpression.GetValue(context, explain);
                        }
                    }
                    NextIteration();
                }
                EndIteration(context, explain, token);
                retVal = AccumulatorVariable.Value;
            }
            else
            {
                AddError("Cannot evaluate list value " + ListExpression);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the callable that is called by this expression
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override ICallable GetCalled(InterpretationContext context, ExplanationPart explain)
        {
            ICallable retVal = null;

            Function function = InitialValue.Ref as Function;
            if (function == null)
            {
                function = InitialValue.GetCalled(context, explain) as Function;
            }

            if (function != null)
            {
                if (function.FormalParameters.Count == 1)
                {
                    int token = context.LocalScope.PushContext();
                    context.LocalScope.SetGraphParameter((Parameter) function.FormalParameters[0]);
                    Graph graph = CreateGraph(context, (Parameter) function.FormalParameters[0], explain);
                    context.LocalScope.PopContext(token);
                    if (graph != null)
                    {
                        retVal = graph.Function;
                    }
                }
                else if (function.FormalParameters.Count == 2)
                {
                    int token = context.LocalScope.PushContext();
                    context.LocalScope.SetSurfaceParameters((Parameter) function.FormalParameters[0],
                        (Parameter) function.FormalParameters[1]);
                    Surface surface = CreateSurface(context, (Parameter) function.FormalParameters[0],
                        (Parameter) function.FormalParameters[1], explain);
                    context.LocalScope.PopContext(token);
                    if (surface != null)
                    {
                        retVal = surface.Function;
                    }
                }
                else
                {
                    AddError("Cannot evaluate REDUCE expression to a function");
                }
            }
            else
            {
                AddError("Cannot evaluate REDUCE expression to a function");
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
            explanation.Write(" INITIAL_VALUE ");
            InitialValue.GetExplain(explanation);
        }

        /// <summary>
        ///     Prepares the iteration on the context provided
        /// </summary>
        /// <param name="context"></param>
        protected override int PrepareIteration(InterpretationContext context)
        {
            int retVal = base.PrepareIteration(context);

            context.LocalScope.SetVariable(AccumulatorVariable);

            return retVal;
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public override void CheckExpression()
        {
            base.CheckExpression();

            Type initialValueType = InitialValue.GetExpressionType();
            if (initialValueType != null)
            {
                Collection listExpressionType = ListExpression.GetExpressionType() as Collection;
                if (listExpressionType != null)
                {
                    IteratorExpression.CheckExpression();
                }
            }
            else
            {
                AddError("Cannot determine initial value expression type for " + ToString());
            }


            bool refToResultFound = false;
            foreach (Usage usage in IteratorExpression.StaticUsage.AllUsages)
            {
                if (usage.Referenced == AccumulatorVariable && usage.Mode == Usage.ModeEnum.Read)
                {
                    refToResultFound = true;
                    break;
                }
            }
            if (!refToResultFound)
            {
                AddWarning("REDUCE expressions should reference RESULT variable");
            }
        }

        /// <summary>
        ///     Creates the graph associated to this expression, when the given parameter ranges over the X axis
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="parameter">The parameters of *the enclosing function* for which the graph should be created</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override Graph CreateGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = base.CreateGraph(context, parameter, explain);

            Graph graph = InitialValue.CreateGraph(context, parameter, explain);
            if (graph != null)
            {
                ListValue value = ListExpression.GetValue(context, explain) as ListValue;
                if (value != null)
                {
                    int token = PrepareIteration(context);
                    AccumulatorVariable.Value = graph.Function;

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
                                AccumulatorVariable.Value = IteratorExpression.GetValue(context, explain);
                            }
                        }
                        NextIteration();
                    }
                    Function function = AccumulatorVariable.Value as Function;
                    if (function != null)
                    {
                        retVal = function.Graph;
                    }
                    else
                    {
                        retVal = Function.CreateGraphForValue(AccumulatorVariable.Value);
                    }
                    EndIteration(context, explain, token);
                }
            }
            else
            {
                throw new Exception("Cannot create graph for initial value " + InitialValue);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the surface of this function if it has been statically defined
        /// </summary>
        /// <param name="context">the context used to create the surface</param>
        /// <param name="xParam">The X axis of this surface</param>
        /// <param name="yParam">The Y axis of this surface</param>
        /// <param name="explain"></param>
        /// <returns>The surface which corresponds to this expression</returns>
        public override Surface CreateSurface(InterpretationContext context, Parameter xParam, Parameter yParam,
            ExplanationPart explain)
        {
            Surface retVal = base.CreateSurface(context, xParam, yParam, explain);

            Surface surface = InitialValue.CreateSurface(context, xParam, yParam, explain);
            if (surface != null)
            {
                ListValue value = ListExpression.GetValue(context, explain) as ListValue;
                if (value != null)
                {
                    int token = PrepareIteration(context);
                    AccumulatorVariable.Value = surface.Function;

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
                                AccumulatorVariable.Value = IteratorExpression.GetValue(context, explain);
                            }
                        }
                        NextIteration();
                    }
                    Function function = AccumulatorVariable.Value as Function;
                    if (function != null)
                    {
                        retVal = function.Surface;
                    }
                    else
                    {
                        throw new Exception("Expression does not reduces to a function");
                    }
                    EndIteration(context, explain, token);
                }
            }
            else
            {
                throw new Exception("Cannot create surface for initial value " + InitialValue);
            }
            retVal.XParameter = xParam;
            retVal.YParameter = yParam;

            return retVal;
        }
    }
}