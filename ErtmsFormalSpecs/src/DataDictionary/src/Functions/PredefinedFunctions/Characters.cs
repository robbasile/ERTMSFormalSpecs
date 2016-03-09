// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpecs is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Values;
using DataDictionary.Variables;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Functions.PredefinedFunctions
{
    /// <summary>
    /// Provides the characters of a string
    /// </summary>
    public class Characters : PredefinedFunction
    {
        /// <summary>
        /// The string
        /// </summary>
        public Parameter Value { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        public Characters(EfsSystem efsSystem)
            : base(efsSystem, "Characters")
        {
            Value = (Parameter)acceptor.getFactory().createParameter();
            Value.Name = "String";
            Value.Type = EFSSystem.StringType;
            Value.setFather(this);
            FormalParameters.Add(Value);
        }

        /// <summary>
        /// The return type of the function
        /// </summary>
        public override Type ReturnType
        {
            get { return EFSSystem.StringCollection; }
        }

        /// <summary>
        /// Provides the value of the function
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actuals">the actual parameters values</param>
        /// <param name="explain"></param>
        /// <returns>The value for the function application</returns>
        public override IValue Evaluate(InterpretationContext context, Dictionary<Actual, IValue> actuals,
            ExplanationPart explain)
        {
            ListValue retVal = null;

            int token = context.LocalScope.PushContext();
            AssignParameters(context, actuals);

            StringValue value = context.FindOnStack(Value).Value as StringValue;
            if (value != null)
            {
                retVal = new ListValue(EfsSystem.Instance.StringCollection, new List<IValue>());

                foreach (char c in value.Val)
                {
                    StringValue character = new StringValue(EfsSystem.Instance.StringType, c.ToString(CultureInfo.InvariantCulture));
                    retVal.Val.Add(character);
                }
            }

            context.LocalScope.PopContext(token);

            return retVal;
        }
    }
}