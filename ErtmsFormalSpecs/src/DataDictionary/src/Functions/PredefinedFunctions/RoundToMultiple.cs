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
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Values;
using DataDictionary.Variables;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Functions.PredefinedFunctions
{
    /// <summary>
    ///     Creates a new function which rounds a value to a multiple of a certain value
    /// </summary>
    public class RoundToMultiple : PredefinedFunction
    {
        /// <summary>
        ///     The value to be rounded
        /// </summary>
        public Parameter Value { get; private set; }

        /// <summary>
        ///     The multiple
        /// </summary>
        public Parameter Multiple { get; private set; }

        /// <summary>
        ///     The return type of the function
        /// </summary>
        public override Type ReturnType
        {
            get { return EFSSystem.DoubleType; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        public RoundToMultiple(EFSSystem efsSystem)
            : base(efsSystem, "RoundToMultiple")
        {
            Value = (Parameter) acceptor.getFactory().createParameter();
            Value.Name = "Value";
            Value.Type = EFSSystem.DoubleType;
            Value.setFather(this);
            FormalParameters.Add(Value);

            Multiple = (Parameter) acceptor.getFactory().createParameter();
            Multiple.Name = "Multiple";
            Multiple.Type = EFSSystem.DoubleType;
            Multiple.setFather(this);
            FormalParameters.Add(Multiple);
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
            IValue retVal = null;

            int token = context.LocalScope.PushContext();
            AssignParameters(context, actuals);

            DoubleValue value = context.FindOnStack(Value).Value as DoubleValue;
            DoubleValue multiple = context.FindOnStack(Multiple).Value as DoubleValue;
            if (value != null && multiple != null)
            {
                double res = Math.Floor(value.Val);
                while (res > 0 && res%multiple.Val != 0)
                {
                    res--;
                }
                retVal = new DoubleValue(ReturnType, res);
            }

            context.LocalScope.PopContext(token);

            return retVal;
        }
    }
}