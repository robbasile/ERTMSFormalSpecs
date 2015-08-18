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
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Functions.PredefinedFunctions
{
    /// <summary>
    ///     Casts a value as a new value of range type
    /// </summary>
    public class Cast : PredefinedFunction
    {
        /// <summary>
        ///     The target type for which the cast is performed
        /// </summary>
        public Type TargetType { get; private set; }

        /// <summary>
        ///     The value which is casted
        /// </summary>
        public Parameter Value { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type">The type in which the cast is performed</param>
        public Cast(Type type)
            : base(type.EFSSystem, type.Name)
        {
            TargetType = type;

            Value = (Parameter) acceptor.getFactory().createParameter();
            Value.Name = "Value";
            Value.Type = EFSSystem.AnyType;
            Value.setFather(this);
            FormalParameters.Add(Value);
        }

        /// <summary>
        ///     The return type of the cast function
        /// </summary>
        public override Type ReturnType
        {
            get { return TargetType; }
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
            IValue value = context.FindOnStack(Value).Value;
            if (value is Function)
            {
                retVal = value;
            }
            else
            {
                retVal = TargetType.convert(value);
            }
            context.LocalScope.PopContext(token);

            return retVal;
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

            IVariable variable = context.FindOnStack(Value);

            if (variable != null)
            {
                retVal = Graph.createGraph(getDoubleValue(variable.Value), parameter);
            }
            else
            {
                AddError("Cannot find variable " + Value + " on the stack");
            }

            return retVal;
        }
    }
}