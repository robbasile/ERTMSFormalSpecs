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
    ///     Creates a new function which describes the minimal value of two functions
    /// </summary>
    public class MinSurface : FunctionOnSurface
    {
        /// <summary>
        ///     The first parameter
        /// </summary>
        public Parameter First { get; private set; }

        /// <summary>
        ///     The second parameter
        /// </summary>
        public Parameter Second { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        public MinSurface(EfsSystem efsSystem)
            : base(efsSystem, "MINSURFACE")
        {
            First = (Parameter) acceptor.getFactory().createParameter();
            First.Name = "First";
            First.Type = EFSSystem.AnyType;
            First.setFather(this);
            FormalParameters.Add(First);

            Second = (Parameter) acceptor.getFactory().createParameter();
            Second.Name = "Second";
            Second.Type = EFSSystem.AnyType;
            Second.setFather(this);
            FormalParameters.Add(Second);
        }

        /// <summary>
        ///     Perform additional checks based on the parameter types
        /// </summary>
        /// <param name="root">The element on which the errors should be reported</param>
        /// <param name="context">The evaluation context</param>
        /// <param name="actualParameters">The parameters applied to this function call</param>
        public override void AdditionalChecks(ModelElement root, InterpretationContext context,
            Dictionary<string, Expression> actualParameters)
        {
            CheckFunctionalParameter(root, context, actualParameters[First.Name], 2);
            CheckFunctionalParameter(root, context, actualParameters[Second.Name], 0);

            Function function1 = actualParameters[First.Name].GetExpressionType() as Function;
            Function function2 = actualParameters[Second.Name].GetExpressionType() as Function;

            if (function1 != null && function2 != null)
            {
                if (function1.FormalParameters.Count == 2 && function2.FormalParameters.Count == 0)
                {
                    Parameter p1 = (Parameter) function1.FormalParameters[0];
                    Parameter p2 = (Parameter) function1.FormalParameters[1];

                    if (p1.Type != EFSSystem.DoubleType || p2.Type != EFSSystem.DoubleType)
                    {
                        root.AddError("The formal parameters for the first function are not double");
                    }
                }

                if (function1.ReturnType != function2.ReturnType && function1.ReturnType != EFSSystem.DoubleType &&
                    function2.ReturnType != EFSSystem.DoubleType)
                {
                    root.AddError("The return values for the functions provided as parameter are not the same");
                }
            }
        }

        /// <summary>
        ///     Provides the surface of this function if it has been statically defined
        /// </summary>
        /// <param name="context">the context used to create the surface</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override Surface CreateSurface(InterpretationContext context, ExplanationPart explain)
        {
            Surface retVal = null;

            IValue firstValue = context.FindOnStack(First).Value;
            IValue secondValue = context.FindOnStack(Second).Value;
            Surface firstSurface = createSurfaceForValue(context, firstValue, explain);
            if (firstSurface != null)
            {
                Surface secondSurface = createSurfaceForValue(context, secondValue, explain);
                if (secondSurface != null)
                {
                    retVal = firstSurface.Min(secondSurface);
                }
                else
                {
                    Second.AddError("Cannot create surface for " + Second);
                }
            }
            else
            {
                First.AddError("Cannot create surface for " + First);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value of the function
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actuals">the actual parameters values</param>
        /// <param name="explain"></param>
        /// <returns>The value for the function application</returns>
        public override IValue Evaluate(InterpretationContext context, Dictionary<Actual, IValue> actuals,
            ExplanationPart explain)
        {
            IValue retVal;

            int token = context.LocalScope.PushContext();
            AssignParameters(context, actuals);

            Function function = (Function) acceptor.getFactory().createFunction();
            function.Name = "MINSURFACE (" + getName(First) + ", " + getName(Second) + ")";
            function.Enclosing = EFSSystem;
            function.Surface = CreateSurface(context, explain);

            Parameter parameterX = (Parameter) acceptor.getFactory().createParameter();
            parameterX.Name = "X";
            parameterX.Type = EFSSystem.DoubleType;
            function.appendParameters(parameterX);

            Parameter parameterY = (Parameter) acceptor.getFactory().createParameter();
            parameterY.Name = "Y";
            parameterY.Type = EFSSystem.DoubleType;
            function.appendParameters(parameterY);

            function.ReturnType = EFSSystem.DoubleType;

            retVal = function;
            context.LocalScope.PopContext(token);

            return retVal;
        }
    }
}