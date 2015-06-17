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
using System.Collections;
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
    /// <summary>
    ///     Something that can be called
    /// </summary>
    public interface ICallable : INamable
    {
        /// <summary>
        ///     Formal parameters of the callable
        /// </summary>
        ArrayList FormalParameters { get; }

        /// <summary>
        ///     Provides the formal parameter which matches the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Parameter getFormalParameter(string name);

        /// <summary>
        ///     Provides the return type of the called element
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        ///     Perform additional checks based on the parameter types
        /// </summary>
        /// <param name="root">The element on which the errors should be reported</param>
        /// <param name="actualParameters">The parameters applied to this function call</param>
        void additionalChecks(ModelElement root, Dictionary<string, Expression> actualParameters);
    }

    public class Call : Expression
    {
        /// <summary>
        ///     The expression which identifies the function
        /// </summary>
        public Expression Called { get; private set; }

        /// <summary>
        ///     The unnamed actual parameters
        /// </summary>
        private List<Expression> actualParameters;

        public List<Expression> ActualParameters
        {
            get
            {
                if (actualParameters == null)
                {
                    actualParameters = new List<Expression>();
                }
                return actualParameters;
            }
            set { actualParameters = null; }
        }

        /// <summary>
        ///     The list of named actual parameters
        /// </summary>
        private Dictionary<Designator, Expression> namedActualParameters;

        public Dictionary<Designator, Expression> NamedActualParameters
        {
            get
            {
                if (namedActualParameters == null)
                {
                    namedActualParameters = new Dictionary<Designator, Expression>();
                }
                return namedActualParameters;
            }
            set { namedActualParameters = value; }
        }

        /// <summary>
        ///     Provides the association between parameters and their corresponding expression
        /// </summary>
        public Dictionary<Parameter, Expression> ParameterAssociation { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root element for which this element is built</param>
        /// <param name="called">The called function</param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public Call(ModelElement root, ModelElement log, Expression called, int start, int end)
            : base(root, log, start, end)
        {
            Called = called;
            Called.Enclosing = this;
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
        public override ICallable getCalled(InterpretationContext context, ExplanationPart explain)
        {
            ICallable retVal = null;

            Call calledFunction = Called as Call;
            if (calledFunction != null)
            {
                retVal = Called.GetValue(context, explain) as ICallable;
            }
            else
            {
                retVal = Called.getCalled(context, explain);
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
        public Procedure getProcedure(InterpretationContext context, ExplanationPart explain)
        {
            Procedure retVal = getCalled(context, explain) as Procedure;

            return retVal;
        }

        /// <summary>
        ///     The function which is called by this call statement
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        public Function getFunction(InterpretationContext context, ExplanationPart explain)
        {
            Function retVal = getCalled(context, explain) as Function;

            return retVal;
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
                // Called
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
                        pair.Key.Ref = called.getFormalParameter(pair.Key.Image);
                        StaticUsage.AddUsage(pair.Key.Ref, Root, Usage.ModeEnum.Parameter);
                    }
                    pair.Value.SemanticAnalysis(instance, IsActualParameter.INSTANCE);
                    StaticUsage.AddUsages(pair.Value.StaticUsage, Usage.ModeEnum.Read);
                }

                ParameterAssociation = createParameterAssociation(Called.Ref as ICallable);
            }

            return retVal;
        }

        /// <summary>
        ///     Creates the association between parameter (from the called ICallable) and its associated expression
        /// </summary>
        /// <param name="callable"></param>
        /// <returns></returns>
        private Dictionary<Parameter, Expression> createParameterAssociation(ICallable callable)
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
                        Parameter parameter = callable.FormalParameters[i] as Parameter;
                        retVal.Add(parameter, expression);
                        i = i + 1;
                    }

                    foreach (KeyValuePair<Designator, Expression> pair in NamedActualParameters)
                    {
                        Parameter parameter = callable.getFormalParameter(pair.Key.Image);
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
        private Dictionary<Parameter, Expression> getParameterAssociation(ICallable callable)
        {
            Dictionary<Parameter, Expression> retVal = ParameterAssociation;

            if (retVal == null)
            {
                retVal = createParameterAssociation(callable);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the ICallable that is statically defined
        /// </summary>
        public override ICallable getStaticCallable()
        {
            ICallable retVal = base.getStaticCallable();

            if (retVal == null)
            {
                retVal = Called.getStaticCallable().ReturnType as ICallable;
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            Type retVal = null;

            Function function = Called.getStaticCallable() as Function;
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
        public override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            IValue retVal = null;

            ExplanationPart subExplanation = ExplanationPart.CreateSubExplanation(explain, this);
            Function function = getFunction(context, explain);
            if (function != null)
            {
                long start = Environment.TickCount;

                Dictionary<Actual, IValue> parameterValues = null;
                try
                {
                    parameterValues = AssignParameterValues(context, function, true, subExplanation);
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
                        context.LocalScope.setGraphParameter(parameters[0]);
                        Graph graph = function.createGraphForParameter(context, parameters[0], subExplanation);
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
                        Surface surface = function.createSurfaceForParameters(context, parameters[0], parameters[1],
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
                }
                catch (Exception e)
                {
                    AddErrorAndExplain("Cannot evaluate function call " + function.Name, subExplanation);
                    throw new Exception("inner evaluation failure");
                }
                finally
                {
                    long stop = Environment.TickCount;
                    long span = (stop - start);
                    function.ExecutionTimeInMilli += span;
                    function.ExecutionCount += 1;

                    ExplanationPart.SetNamable(subExplanation, retVal);
                }
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
                    if (!Utils.Utils.isEmpty(parameters))
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
                    Parameter parameter = callable.FormalParameters[i] as Parameter;
                    ExplanationPart subExplanation = ExplanationPart.CreateSubExplanation(explain, parameter);
                    IValue val = expression.GetValue(context, subExplanation);
                    if (val != null)
                    {
                        Actual actual = parameter.createActual();
                        val = val.RightSide(actual, false, false);
                        retVal.Add(actual, val);
                    }
                    else
                    {
                        AddError("Cannot evaluate value for parameter " + i + " (" + expression.ToString() +
                                 ") of function " + callable.Name);
                        throw new Exception("Evaluation of parameters failed");
                    }
                    ExplanationPart.SetNamable(subExplanation, val);

                    i = i + 1;
                }

                foreach (KeyValuePair<Designator, Expression> pair in NamedActualParameters)
                {
                    Parameter parameter = callable.getFormalParameter(pair.Key.Image);
                    ExplanationPart subExplanation = ExplanationPart.CreateSubExplanation(explain, parameter);
                    IValue val = pair.Value.GetValue(context, subExplanation);
                    if (val != null)
                    {
                        Actual actual = parameter.createActual();
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
        public override void fill(List<INamable> retVal, BaseFilter filter)
        {
            foreach (Expression expression in NamedActualParameters.Values)
            {
                expression.fill(retVal, filter);
            }

            foreach (Expression expression in ActualParameters)
            {
                expression.fill(retVal, filter);
            }
        }

        /// <summary>
        ///     Provides the indented expression text
        /// </summary>
        /// <param name="indentLevel"></param>
        /// <returns></returns>
        public override string ToString(int indentLevel)
        {
            string retVal = Called.ToString(indentLevel) + "(";

            bool first = true;
            foreach (Expression argument in ActualParameters)
            {
                if (!first)
                {
                    retVal += ", ";
                }
                first = false;
                retVal += argument.ToString(indentLevel);
            }
            foreach (KeyValuePair<Designator, Expression> pair in NamedActualParameters)
            {
                if (!first)
                {
                    retVal += ", ";
                }
                first = false;
                retVal += pair.Key.ToString(indentLevel) + " => " + pair.Value.ToString(indentLevel);
            }
            retVal = retVal + ")";

            return retVal;
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public override void checkExpression()
        {
            base.checkExpression();

            Called.checkExpression();
            ICallable called = Called.getStaticCallable();
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
                        Parameter parameter = called.getFormalParameter(name);
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

                    called.additionalChecks(Root, actuals);
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

            expression.checkExpression();
            Type argumentType = expression.GetExpressionType();
            if (argumentType == null)
            {
                AddError("Cannot evaluate argument type for argument " + expression.ToString());
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
                        AddError("Invalid argument " + expression.ToString() + " type, expected " +
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
        public override Graph createGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = base.createGraph(context, parameter, explain);

            Cast cast = Called.Ref as Cast;
            if (cast != null)
            {
                // In case of cast, just take the graph of the enclosed expression
                Parameter param = (Parameter) cast.FormalParameters[0];
                retVal = cast.createGraphForParameter(context, param, explain);
            }

            Function calledFunction = Called.Ref as Function;
            Dictionary<Parameter, Expression> parameterAssociation = null;
            if (calledFunction == null)
            {
                calledFunction = Called.GetValue(context, explain) as Function;
                parameterAssociation = createParameterAssociation(calledFunction);
            }
            else
            {
                parameterAssociation = ParameterAssociation;
            }

            Parameter Xaxis = null;
            foreach (KeyValuePair<Parameter, Expression> pair in parameterAssociation)
            {
                if (pair.Value.Ref == parameter)
                {
                    if (Xaxis == null)
                    {
                        Xaxis = pair.Key;
                    }
                    else
                    {
                        Root.AddError("Cannot evaluate graph for function call " + ToString() +
                                      " which has more than 1 parameter used as X axis");
                        Xaxis = null;
                        break;
                    }
                }
            }

            int token = context.LocalScope.PushContext();
            calledFunction.AssignParameters(context, AssignParameterValues(context, calledFunction, false, explain));
            if (Xaxis != null)
            {
                retVal = calledFunction.createGraphForParameter(context, Xaxis, explain);
            }
            else
            {
                retVal = Function.createGraphForValue(GetValue(context, explain));
            }
            context.LocalScope.PopContext(token);

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

            Function function = getFunction(context, explain);
            Cast cast = Called.Ref as Cast;
            if (cast != null)
            {
                // In case of cast, just take the surface of the enclosed expression
                Expression actual = (Expression) ActualParameters[0];
                retVal = actual.createSurface(context, xParam, yParam, explain);
            }
            else
            {
                Parameter Xaxis = null;
                Parameter Yaxis = null;

                if (function == null)
                {
                    function = Called.getCalled(context, explain) as Function;
                }

                SelectXandYAxis(context, xParam, yParam, function, out Xaxis, out Yaxis);
                if (Xaxis != null || Yaxis != null)
                {
                    int token = context.LocalScope.PushContext();
                    function.AssignParameters(context, AssignParameterValues(context, function, true, explain));
                    retVal = function.createSurfaceForParameters(context, Xaxis, Yaxis, explain);
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
            retVal.XParameter = xParam;
            retVal.YParameter = yParam;

            return retVal;
        }

        /// <summary>
        ///     Selects the X and Y axis of the surface to be created according to the function for which the surface need be
        ///     created and the parameters on which the surface is created
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="xParam">The X parameter for which the surface need be created</param>
        /// <param name="yParam">The Y parameter for which the surface need be created</param>
        /// <param name="function">The function creating the surface</param>
        /// <param name="Xaxis">The resulting X axis</param>
        /// <param name="Yaxis">The resulting Y axis</param>
        /// <returns>true if the axis could be selected</returns>
        private void SelectXandYAxis(InterpretationContext context, Parameter xParam, Parameter yParam,
            Function function, out Parameter Xaxis, out Parameter Yaxis)
        {
            Xaxis = null;
            Yaxis = null;

            Dictionary<Parameter, Expression> association = getParameterAssociation(function);
            if (association != null)
            {
                foreach (KeyValuePair<Parameter, Expression> pair in association)
                {
                    if (pair.Value.Ref == xParam)
                    {
                        if (Xaxis == null)
                        {
                            Xaxis = pair.Key;
                        }
                        else
                        {
                            Root.AddError("Cannot evaluate surface for function call " + ToString() +
                                          " which has more than 1 X axis parameter");
                            Xaxis = null;
                            break;
                        }
                    }

                    if (pair.Value.Ref == yParam)
                    {
                        if (Yaxis == null)
                        {
                            Yaxis = pair.Key;
                        }
                        else
                        {
                            Root.AddError("Cannot evaluate surface for function call " + ToString() +
                                          " which has more than 1 Y axis parameter");
                            Yaxis = null;
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

            Function function = Called.getStaticCallable() as Function;
            if (function != null)
            {
                retVal = function.Reads(variable);
            }

            return retVal;
        }
    }
}