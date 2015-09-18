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
using DataDictionary.Functions.PredefinedFunctions;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter
{
    public class Call : Expression
    {
        /// <summary>
        ///     The expression which identifies the function
        /// </summary>
        public Expression Called { get; private set; }

        /// <summary>
        ///     The unnamed actual parameters
        /// </summary>
        private List<Expression> _actualParameters;

        public List<Expression> ActualParameters
        {
            get
            {
                if (_actualParameters == null)
                {
                    _actualParameters = new List<Expression>();
                }
                return _actualParameters;
            }
            set { _actualParameters = value; }
        }

        /// <summary>
        ///     The list of named actual parameters
        /// </summary>
        private Dictionary<Designator, Expression> _namedActualParameters;

        public Dictionary<Designator, Expression> NamedActualParameters
        {
            get
            {
                if (_namedActualParameters == null)
                {
                    _namedActualParameters = new Dictionary<Designator, Expression>();
                }
                return _namedActualParameters;
            }
            set { _namedActualParameters = value; }
        }

        /// <summary>
        ///     Provides the association between parameters and their corresponding expression
        /// </summary>
        public Dictionary<Parameter, Expression> ParameterAssociation { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root element for which this element is built</param>
        /// <param name="log"></param>
        /// <param name="called">The called function</param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public Call(ModelElement root, ModelElement log, Expression called, int start, int end)
            : base(root, log, start, end)
        {
            Called = SetEnclosed(called);
        }

        /// <summary>
        ///     Provides all the parameters for this call (both named and unnamed)
        /// </summary>
        public List<Expression> AllParameters
        {
            get
            {
                List<Expression> retVal = new List<Expression>();

                retVal.AddRange(ActualParameters);
                retVal.AddRange(NamedActualParameters.Values);

                return retVal;
            }
        }

        /// <summary>
        ///     Adds an expression as a parameter
        /// </summary>
        /// <param name="designator">the name of the actual parameter</param>
        /// <param name="expression">the actual parameter value</param>
        public void AddActualParameter(Designator designator, Expression expression)
        {
            if (designator == null)
            {
                ActualParameters.Add(expression);
            }
            else
            {
                bool found = false;
                foreach (KeyValuePair<Designator, Expression> pair in NamedActualParameters)
                {
                    if (pair.Key.Image == designator.Image)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    NamedActualParameters[designator] = expression;
                }
                else
                {
                    AddError("Actual parameter " + designator.Image + " is bound twice");
                }
            }

            expression.Enclosing = this;
        }

        /// <summary>
        ///     Provides the callable that is called by this expression
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override ICallable GetCalled(InterpretationContext context, ExplanationPart explain)
        {
            ICallable retVal;

            Call calledFunction = Called as Call;
            if (calledFunction != null)
            {
                retVal = Called.GetValue(context, explain) as ICallable;
            }
            else
            {
                retVal = Called.GetCalled(context, explain);
                if (retVal == null)
                {
                    Type type = Called.GetExpressionType();
                    if (type != null)
                    {
                        retVal = type.CastFunction;
                    }

                    if (retVal == null)
                    {
                        retVal = Called.GetValue(context, explain) as ICallable;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The procedure which is called by this call statement
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public Procedure GetProcedure(InterpretationContext context, ExplanationPart explain)
        {
            Procedure retVal = GetCalled(context, explain) as Procedure;

            return retVal;
        }

        /// <summary>
        ///     The function which is called by this call statement
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        public Function GetFunction(InterpretationContext context, ExplanationPart explain)
        {
            Function retVal = GetCalled(context, explain) as Function;

            return retVal;
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
                // Called
                if (Called != null)
                {
                    Called.SemanticAnalysis(instance, IsCallable.INSTANCE);
                    StaticUsage.AddUsages(Called.StaticUsage, Usage.ModeEnum.Call);

                    // Actual parameters
                    foreach (Expression actual in ActualParameters)
                    {
                        actual.SemanticAnalysis(instance, IsActualParameter.INSTANCE);
                        StaticUsage.AddUsages(actual.StaticUsage, Usage.ModeEnum.Read);
                    }

                    foreach (KeyValuePair<Designator, Expression> pair in NamedActualParameters)
                    {
                        ICallable called = Called.Ref as ICallable;
                        if (called != null)
                        {
                            pair.Key.Ref = called.GetFormalParameter(pair.Key.Image);
                            StaticUsage.AddUsage(pair.Key.Ref, Root, Usage.ModeEnum.Parameter);
                        }
                        pair.Value.SemanticAnalysis(instance, IsActualParameter.INSTANCE);
                        StaticUsage.AddUsages(pair.Value.StaticUsage, Usage.ModeEnum.Read);
                    }

                    ParameterAssociation = CreateParameterAssociation(Called.Ref as ICallable);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Creates the association between parameter (from the called ICallable) and its associated expression
        /// </summary>
        /// <param name="callable"></param>
        /// <returns></returns>
        private Dictionary<Parameter, Expression> CreateParameterAssociation(ICallable callable)
        {
            Dictionary<Parameter, Expression> retVal = null;

            if (callable != null)
            {
                if (callable.FormalParameters.Count == NamedActualParameters.Count + ActualParameters.Count)
                {
                    retVal = new Dictionary<Parameter, Expression>();

                    int i = 0;
                    foreach (Expression expression in ActualParameters)
                    {
                        Parameter parameter = (Parameter) callable.FormalParameters[i];
                        retVal.Add(parameter, expression);
                        i = i + 1;
                    }

                    foreach (KeyValuePair<Designator, Expression> pair in NamedActualParameters)
                    {
                        Parameter parameter = callable.GetFormalParameter(pair.Key.Image);
                        if (parameter != null)
                        {
                            retVal.Add(parameter, pair.Value);
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the parameter association according to the icallable provided.
        ///     If the call is statically determined, take the cached association
        /// </summary>
        /// <param name="callable"></param>
        /// <returns></returns>
        private Dictionary<Parameter, Expression> GetParameterAssociation(ICallable callable)
        {
            Dictionary<Parameter, Expression> retVal = ParameterAssociation;

            if (retVal == null)
            {
                retVal = CreateParameterAssociation(callable);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the ICallable that is statically defined
        /// </summary>
        public override ICallable GetStaticCallable()
        {
            ICallable retVal = base.GetStaticCallable();

            if (retVal == null)
            {
                retVal = Called.GetStaticCallable().ReturnType as ICallable;
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

            Function function = Called.GetStaticCallable() as Function;
            if (function != null)
            {
                retVal = function.ReturnType;
            }
            else
            {
                AddError("Cannot get type of function call " + ToString());
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

            ExplanationPart subExplanation = ExplanationPart.CreateSubExplanation(explain, this);
            Function function = GetFunction(context, explain);
            if (function != null)
            {
                long start = Environment.TickCount;

                Dictionary<Actual, IValue> parameterValues = AssignParameterValues(context, function, true,
                    subExplanation);
                List<Parameter> parameters = GetPlaceHolders(function, parameterValues);
                if (parameters == null)
                {
                    retVal = function.Evaluate(context, parameterValues, subExplanation);
                    if (retVal == null)
                    {
                        AddErrorAndExplain(
                            "Call " + function.Name + " ( " + ParameterValues(parameterValues) +
                            " ) returned nothing", subExplanation);
                    }
                }
                else if (parameters.Count == 1) // graph
                {
                    int token = context.LocalScope.PushContext();
                    context.LocalScope.SetGraphParameter(parameters[0]);
                    Graph graph = function.CreateGraphForParameter(context, parameters[0], subExplanation);
                    context.LocalScope.PopContext(token);
                    if (graph != null)
                    {
                        retVal = graph.Function;
                    }
                    else
                    {
                        AddError("Cannot create graph on Call " + function.Name + " ( " +
                                 ParameterValues(parameterValues) + " )");
                    }
                }
                else // surface
                {
                    Surface surface = function.CreateSurfaceForParameters(context, parameters[0], parameters[1],
                        subExplanation);
                    if (surface != null)
                    {
                        retVal = surface.Function;
                    }
                    else
                    {
                        AddError("Cannot create surface on Call " + function.Name + " ( " +
                                 ParameterValues(parameterValues) + " )");
                    }
                }

                long stop = Environment.TickCount;
                long span = (stop - start);
                function.ExecutionTimeInMilli += span;
                function.ExecutionCount += 1;

                ExplanationPart.SetNamable(subExplanation, retVal);
            }
            else
            {
                AddErrorAndExplain("Cannot find function for " + ToString(), subExplanation);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the parameters whose value are place holders
        /// </summary>
        /// <param name="function">The function on which the call is performed</param>
        /// <param name="parameterValues">The actual parameter values</param>
        /// <returns></returns>
        private List<Parameter> GetPlaceHolders(Function function, Dictionary<Actual, IValue> parameterValues)
        {
            List<Parameter> retVal = new List<Parameter>();

            foreach (KeyValuePair<Actual, IValue> pair in parameterValues)
            {
                Actual actual = pair.Key;

                PlaceHolder placeHolder = pair.Value as PlaceHolder;
                if (placeHolder != null && actual.Parameter.Enclosing == function)
                {
                    retVal.Add(actual.Parameter);
                }
            }

            if (retVal.Count != parameterValues.Count || retVal.Count == 0)
            {
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the parameter's values along with their name
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        private static string ParameterValues(Dictionary<Actual, IValue> parameterValues)
        {
            string parameters = "";

            if (parameterValues != null)
            {
                foreach (KeyValuePair<Actual, IValue> pair in parameterValues)
                {
                    if (!Utils.Util.isEmpty(parameters))
                    {
                        parameters += ", ";
                    }
                    parameters += pair.Key.Name + " => " + pair.Value.FullName;
                }
            }

            return parameters;
        }

        /// <summary>
        ///     Creates the parameter value associationg according to actual parameters
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="callable">The callable</param>
        /// <param name="log">Indicates whether errors should be logged</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public Dictionary<Actual, IValue> AssignParameterValues(InterpretationContext context, ICallable callable,
            bool log, ExplanationPart explain)
        {
            // Compute the unnamed actual parameter values
            Dictionary<Actual, IValue> retVal = new Dictionary<Actual, IValue>();

            if (callable.FormalParameters.Count == NamedActualParameters.Count + ActualParameters.Count)
            {
                int i = 0;
                foreach (Expression expression in ActualParameters)
                {
                    Parameter parameter = (Parameter) callable.FormalParameters[i];
                    ExplanationPart subExplanation = ExplanationPart.CreateSubExplanation(explain, parameter);
                    IValue val = expression.GetValue(context, subExplanation);
                    if (val != null)
                    {
                        Actual actual = parameter.CreateActual();
                        val = val.RightSide(actual, false, false);
                        retVal.Add(actual, val);
                    }
                    else
                    {
                        AddError("Cannot evaluate value for parameter " + i + " (" + expression +
                                 ") of function " + callable.Name);
                        throw new Exception("Evaluation of parameters failed");
                    }
                    ExplanationPart.SetNamable(subExplanation, val);

                    i = i + 1;
                }

                foreach (KeyValuePair<Designator, Expression> pair in NamedActualParameters)
                {
                    Parameter parameter = callable.GetFormalParameter(pair.Key.Image);
                    ExplanationPart subExplanation = ExplanationPart.CreateSubExplanation(explain, parameter);
                    IValue val = pair.Value.GetValue(context, subExplanation);
                    if (val != null)
                    {
                        Actual actual = parameter.CreateActual();
                        val = val.RightSide(actual, false, false);
                        actual.Value = val;
                        retVal.Add(actual, val);
                    }
                    else
                    {
                        AddError("Cannot evaluate value for parameter " + pair.Key + " of function " + callable.Name);
                        throw new Exception("Evaluation of parameters failed");
                    }
                    ExplanationPart.SetNamable(subExplanation, val);
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
            foreach (Expression expression in NamedActualParameters.Values)
            {
                expression.Fill(retVal, filter);
            }

            foreach (Expression expression in ActualParameters)
            {
                expression.Fill(retVal, filter);
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            Called.GetExplain(explanation);
            explanation.Write("(");
            explanation.ExplainList(ActualParameters, explainSubElements, ", ",
                element => element.GetExplain(explanation));

            if (NamedActualParameters.Count > 0)
            {
                explanation.Indent(2, () =>
                {
                    if (ActualParameters.Count > 0)
                    {
                        explanation.Write(", ");
                    }
                    explanation.ExplainList(NamedActualParameters, explainSubElements, ", ", pair =>
                    {
                        if (AllParameters.Count > 1)
                        {
                            explanation.WriteLine();
                        }
                        pair.Key.GetExplain(explanation);
                        explanation.Write(" => ");
                        pair.Value.GetExplain(explanation);
                    });
                });
            }
            explanation.Write(")");
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public override void CheckExpression()
        {
            base.CheckExpression();

            Called.CheckExpression();
            ICallable called = Called.GetStaticCallable();
            if (called != null)
            {
                if (called.FormalParameters.Count != AllParameters.Count)
                {
                    AddError("Invalid number of arguments provided for function call " + ToString() + " expected " +
                             called.FormalParameters.Count + " actual " + AllParameters.Count);
                }
                else
                {
                    Dictionary<string, Expression> actuals = new Dictionary<string, Expression>();

                    int i = 0;
                    foreach (Expression expression in ActualParameters)
                    {
                        Parameter parameter = called.FormalParameters[i] as Parameter;
                        CheckActualAgainstFormal(actuals, expression, parameter);
                        i = i + 1;
                    }

                    foreach (KeyValuePair<Designator, Expression> pair in NamedActualParameters)
                    {
                        string name = pair.Key.Image;
                        Expression expression = pair.Value;
                        Parameter parameter = called.GetFormalParameter(name);
                        if (parameter == null)
                        {
                            AddError("Parameter " + name + " is not defined as formal parameter of function " +
                                     called.FullName);
                        }
                        else
                        {
                            if (actuals.ContainsKey(name))
                            {
                                AddError("Parameter " + name + " isassigned twice in " + ToString());
                            }
                            else
                            {
                                CheckActualAgainstFormal(actuals, expression, parameter);
                            }
                        }
                    }

                    if (called.FormalParameters.Count > 2)
                    {
                        if (ActualParameters.Count > 0)
                        {
                            AddWarning(
                                "Calls where more than two parameters are provided must be performed using named association");
                        }
                    }

                    called.AdditionalChecks(Root, actuals);
                }
            }
            else
            {
                AddError("Cannot determine callable referenced by " + ToString());
            }
        }

        private void CheckActualAgainstFormal(Dictionary<string, Expression> actuals, Expression expression,
            Parameter parameter)
        {
            actuals[parameter.Name] = expression;

            expression.CheckExpression();
            Type argumentType = expression.GetExpressionType();
            if (argumentType == null)
            {
                AddError("Cannot evaluate argument type for argument " + expression);
            }
            else
            {
                if (parameter.Type == null)
                {
                    AddError("Cannot evaluate formal parameter type for " + parameter.Name);
                }
                else
                {
                    if (!parameter.Type.Match(argumentType))
                    {
                        AddError("Invalid argument " + expression + " type, expected " +
                                 parameter.Type.FullName + ", actual " + argumentType.FullName);
                    }
                }
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

            Cast cast = Called.Ref as Cast;
            if (cast != null)
            {
                // In case of cast, just take the graph of the enclosed expression
                Parameter param = (Parameter) cast.FormalParameters[0];
                retVal = cast.CreateGraphForParameter(context, param, explain);
            }
            else
            {
                Function calledFunction = Called.Ref as Function;
                Dictionary<Parameter, Expression> parameterAssociation;
                if (calledFunction == null)
                {
                    calledFunction = Called.GetValue(context, explain) as Function;
                    parameterAssociation = CreateParameterAssociation(calledFunction);
                }
                else
                {
                    parameterAssociation = ParameterAssociation;
                }

                if (calledFunction != null)
                {
                    Parameter xAxis = null;
                    foreach (KeyValuePair<Parameter, Expression> pair in parameterAssociation)
                    {
                        if (pair.Value.Ref == parameter)
                        {
                            if (xAxis == null)
                            {
                                xAxis = pair.Key;
                            }
                            else
                            {
                                Root.AddError("Cannot evaluate graph for function call " + ToString() +
                                              " which has more than 1 parameter used as X axis");
                                xAxis = null;
                                break;
                            }
                        }
                    }

                    int token = context.LocalScope.PushContext();
                    calledFunction.AssignParameters(context,
                        AssignParameterValues(context, calledFunction, false, explain));
                    if (xAxis != null)
                    {
                        retVal = calledFunction.CreateGraphForParameter(context, xAxis, explain);
                    }
                    else
                    {
                        retVal = Function.CreateGraphForValue(GetValue(context, explain));
                    }
                    context.LocalScope.PopContext(token);
                }
                else
                {
                    AddError("Cannot determine called function");
                }
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

            Function function = GetFunction(context, explain);
            Cast cast = Called.Ref as Cast;
            if (cast != null)
            {
                // In case of cast, just take the surface of the enclosed expression
                Expression actual = ActualParameters[0];
                retVal = actual.CreateSurface(context, xParam, yParam, explain);
            }
            else
            {
                if (function == null)
                {
                    function = Called.GetCalled(context, explain) as Function;
                }

                if (function != null)
                {
                    Parameter xAxis;
                    Parameter yAxis;
                    SelectXandYAxis(xParam, yParam, function, out xAxis, out yAxis);
                    if (xAxis != null || yAxis != null)
                    {
                        int token = context.LocalScope.PushContext();
                        function.AssignParameters(context, AssignParameterValues(context, function, true, explain));
                        retVal = function.CreateSurfaceForParameters(context, xAxis, yAxis, explain);
                        context.LocalScope.PopContext(token);
                    }
                    else
                    {
                        IValue value = GetValue(context, explain);
                        if (value != null)
                        {
                            retVal = Surface.createSurface(value, xParam, yParam);
                        }
                        else
                        {
                            throw new Exception("Cannot create surface for expression");
                        }
                    }
                }
                else
                {
                    AddError("Cannot determine called function");
                }
            }
            retVal.XParameter = xParam;
            retVal.YParameter = yParam;

            return retVal;
        }

        /// <summary>
        ///     Selects the X and Y axis of the surface to be created according to the function for which the surface need be
        ///     created and the parameters on which the surface is created
        /// </summary>
        /// <param name="xParam">The X parameter for which the surface need be created</param>
        /// <param name="yParam">The Y parameter for which the surface need be created</param>
        /// <param name="function">The function creating the surface</param>
        /// <param name="xAxis">The resulting X axis</param>
        /// <param name="yAxis">The resulting Y axis</param>
        /// <returns>true if the axis could be selected</returns>
        private void SelectXandYAxis(Parameter xParam, Parameter yParam,
            Function function, out Parameter xAxis, out Parameter yAxis)
        {
            xAxis = null;
            yAxis = null;

            Dictionary<Parameter, Expression> association = GetParameterAssociation(function);
            if (association != null)
            {
                foreach (KeyValuePair<Parameter, Expression> pair in association)
                {
                    if (pair.Value.Ref == xParam)
                    {
                        if (xAxis == null)
                        {
                            xAxis = pair.Key;
                        }
                        else
                        {
                            Root.AddError("Cannot evaluate surface for function call " + ToString() +
                                          " which has more than 1 X axis parameter");
                            xAxis = null;
                            break;
                        }
                    }

                    if (pair.Value.Ref == yParam)
                    {
                        if (yAxis == null)
                        {
                            yAxis = pair.Key;
                        }
                        else
                        {
                            Root.AddError("Cannot evaluate surface for function call " + ToString() +
                                          " which has more than 1 Y axis parameter");
                            yAxis = null;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Indicates whether this call may read a given variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public bool Reads(ITypedElement variable)
        {
            bool retVal = false;

            Function function = Called.GetStaticCallable() as Function;
            if (function != null)
            {
                retVal = function.Reads(variable);
            }

            return retVal;
        }
    }
}