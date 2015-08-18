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
using DataDictionary.Interpreter;
using DataDictionary.Values;
using DataDictionary.Variables;

namespace DataDictionary.Functions.PredefinedFunctions
{
    /// <summary>
    ///     Adds an increment to a function
    /// </summary>
    public class AddIncrement : FunctionOnGraph
    {
        /// <summary>
        ///     The function to be modified
        /// </summary>
        public Parameter Function { get; private set; }

        /// <summary>
        ///     The increment function to add
        /// </summary>
        public Parameter Increment { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        /// <param name="name">the name of the cast function</param>
        public AddIncrement(EFSSystem efsSystem)
            : base(efsSystem, "AddIncrement")
        {
            Function = (Parameter) acceptor.getFactory().createParameter();
            Function.Name = "Function";
            Function.Type = EFSSystem.AnyType;
            Function.setFather(this);
            FormalParameters.Add(Function);

            Increment = (Parameter) acceptor.getFactory().createParameter();
            Increment.Name = "Increment";
            Increment.Type = EFSSystem.AnyType;
            Increment.setFather(this);
            FormalParameters.Add(Increment);
        }

        /// <summary>
        ///     Perform additional checks based on the parameter types
        /// </summary>
        /// <param name="root">The element on which the errors should be reported</param>
        /// <param name="context">The evaluation context</param>
        /// <param name="actualParameters">The parameters applied to this function call</param>
        public override void additionalChecks(ModelElement root, InterpretationContext context,
            Dictionary<string, Expression> actualParameters)
        {
            CheckFunctionalParameter(root, context, actualParameters[Function.Name], 1);
            CheckFunctionalParameter(root, context, actualParameters[Increment.Name], 1);
        }

        /// <summary>
        ///     Provides the graph of this function if it has been statically defined
        /// </summary>
        /// <param name="context">the context used to create the graph</param>
        /// <param name="parameter"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override Graph createGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = null;

            Graph graph = null;

            Function function = context.FindOnStack(Function).Value as Function;
            if (function != null)
            {
                int token = context.LocalScope.PushContext();
                Parameter p = (Parameter) function.FormalParameters[0];
                context.LocalScope.setGraphParameter(p);
                graph = createGraphForValue(context, function, explain, p);
                context.LocalScope.PopContext(token);
            }

            if (graph != null)
            {
                Function increment = context.FindOnStack(Increment).Value as Function;
                retVal = graph.AddIncrement(context, increment, explain);
            }
            else
            {
                Log.Error("Cannot create graph for " + Function.ToString());
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value of the function
        /// </summary>
        /// <param name="context"></param>
        /// <param name="instance">the instance on which the function is evaluated</param>
        /// <param name="actuals">the actual parameters values</param>
        /// <param name="localScope">the values of local variables</param>
        /// <param name="explain"></param>
        /// <returns>The value for the function application</returns>
        public override IValue Evaluate(InterpretationContext context, Dictionary<Actual, IValue> actuals,
            ExplanationPart explain)
        {
            IValue retVal = null;

            int token = context.LocalScope.PushContext();
            AssignParameters(context, actuals);

            Function function = (Function) acceptor.getFactory().createFunction();
            function.Name = "AddIncrement ( Function => " + getName(Function) + ", Value => " + getName(Increment) + ")";
            function.Enclosing = EFSSystem;
            Parameter parameter = (Parameter) acceptor.getFactory().createParameter();
            parameter.Name = "X";
            parameter.Type = EFSSystem.DoubleType;
            function.appendParameters(parameter);
            function.ReturnType = EFSSystem.DoubleType;
            function.Graph = createGraph(context, parameter, explain);

            retVal = function;
            context.LocalScope.PopContext(token);

            return retVal;
        }
    }
}