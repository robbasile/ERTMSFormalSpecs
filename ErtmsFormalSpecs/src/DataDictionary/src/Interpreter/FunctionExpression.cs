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
using System.Collections.Generic;
using DataDictionary.Functions;
using DataDictionary.Generated;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Values;
using Utils;
using Function = DataDictionary.Functions.Function;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter
{
    public class FunctionExpression : Expression, ISubDeclarator
    {
        /// <summary>
        ///     The parameters for this function expression
        /// </summary>
        public List<Parameter> Parameters { get; private set; }

        /// <summary>
        ///     The expression associated to this function
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="expression">the functional expression</param>
        /// <param name="parameters">the function parameters</param>
        /// <param name="root"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public FunctionExpression(ModelElement root, ModelElement log, List<Parameter> parameters, Expression expression,
            int start, int end)
            : base(root, log, start, end)
        {
            Parameters = parameters;

            Expression = expression;
            Expression.Enclosing = this;

            InitDeclaredElements();
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();

            foreach (Parameter parameter in Parameters)
            {
                parameter.Enclosing = this;
                ISubDeclaratorUtils.AppendNamable(this, parameter);
            }
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
        /// <paraparam name="expectation">Indicates the kind of element we are looking for</paraparam>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(INamable instance, BaseFilter expectation)
        {
            bool retVal = base.SemanticAnalysis(instance, expectation);

            if (retVal)
            {
                Expression.SemanticAnalysis(instance, AllMatches.INSTANCE);
                StaticUsage.AddUsages(Expression.StaticUsage, null);
            }

            return retVal;
        }

        private ICallable __staticCallable = null;

        /// <summary>
        ///     Provides the ICallable that is statically defined
        /// </summary>
        public override ICallable getStaticCallable()
        {
            if (__staticCallable == null)
            {
                __staticCallable = GetExpressionType() as ICallable;
            }

            return __staticCallable;
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            Function retVal = (Function) acceptor.getFactory().createFunction();
            retVal.Name = ToString();
            retVal.ReturnType = Expression.GetExpressionType();

            foreach (Parameter parameter in Parameters)
            {
                Parameter param = (Parameter) acceptor.getFactory().createParameter();
                param.Name = parameter.Name;
                param.Type = parameter.Type;
                retVal.appendParameters(param);
            }
            retVal.Enclosing = Root;

            return retVal;
        }

        /// <summary>
        ///     Provides the callable that is called by this expression
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override ICallable getCalled(InterpretationContext context, ExplanationPart explain)
        {
            return GetValue(context, explain) as ICallable;
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        public override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            IValue retVal = null;

            ExplanationPart subExplanation = ExplanationPart.CreateSubExplanation(explain, this);
            try
            {
                if (Parameters.Count == 1)
                {
                    int token = context.LocalScope.PushContext();
                    context.LocalScope.setGraphParameter(Parameters[0]);
                    Graph graph = createGraph(context, Parameters[0], subExplanation);
                    context.LocalScope.PopContext(token);
                    if (graph != null)
                    {
                        retVal = graph.Function;
                    }
                }
                else if (Parameters.Count == 2)
                {
                    int token = context.LocalScope.PushContext();
                    context.LocalScope.setSurfaceParameters(Parameters[0], Parameters[1]);
                    Surface surface = createSurface(context, Parameters[0], Parameters[1], subExplanation);
                    context.LocalScope.PopContext(token);
                    if (surface != null)
                    {
                        retVal = surface.Function;
                    }
                }
            }
            catch (Exception)
            {
                /// TODO Ugly hack, because functions & function types are merged.
                /// This provides an empty function as the type of this
                retVal = GetExpressionType() as IValue;
            }
            finally
            {
                ExplanationPart.SetNamable(subExplanation, retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public override void fill(List<INamable> retVal, BaseFilter filter)
        {
            if (Parameters != null)
            {
                foreach (Parameter parameter in Parameters)
                {
                    if (filter.AcceptableChoice(parameter))
                    {
                        retVal.Add(parameter);
                    }
                }
            }

            if (Expression != null)
            {
                Expression.fill(retVal, filter);
            }
        }

        /// <summary>
        ///     Provides the indented expression text
        /// </summary>
        /// <param name="indentLevel"></param>
        /// <returns></returns>
        public override string ToString(int indentLevel)
        {
            string retVal = "FUNCTION ";

            bool first = true;
            foreach (Parameter parameter in Parameters)
            {
                if (!first)
                {
                    retVal += ", ";
                }
                retVal += parameter.Name + " : " + parameter.TypeName;
                first = false;
            }

            retVal += " => " + Expression.ToString(indentLevel);

            return retVal;
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        /// <param name="context">The interpretation context</param>
        public override void checkExpression()
        {
            base.checkExpression();

            Expression.checkExpression();
        }

        /// <summary>
        ///     Creates the graph associated to this expression, when the given parameter ranges over the X axis
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="parameter">The parameters of *the enclosing function* for which the graph should be created</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override Graph createGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = base.createGraph(context, parameter, explain);

            if (parameter == Parameters[0] || parameter == Parameters[1])
            {
                retVal = Expression.createGraph(context, parameter, explain);
            }
            else
            {
                throw new Exception("Cannot create graph for parameter " + parameter.Name);
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
        public override Surface createSurface(InterpretationContext context, Parameter xParam, Parameter yParam,
            ExplanationPart explain)
        {
            Surface retVal = base.createSurface(context, xParam, yParam, explain);

            if (xParam == null || yParam == null)
            {
                AddError("Cannot have null parameters for Function expression " + ToString());
            }
            else
            {
                int token = context.LocalScope.PushContext();
                Parameter xAxis = Parameters[0];
                Parameter yAxis = Parameters[1];
                context.LocalScope.setSurfaceParameters(xAxis, yAxis);
                retVal = Expression.createSurface(context, xAxis, yAxis, explain);
                context.LocalScope.PopContext(token);
            }
            retVal.XParameter = xParam;
            retVal.YParameter = yParam;

            return retVal;
        }
    }
}