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
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Values;
using DataDictionary.Variables;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Functions.PredefinedFunctions
{
    /// <summary>
    /// Provides the concatenation of two strings
    /// </summary>
    public class Concat : PredefinedFunction
    {
        /// <summary>
        /// The first string
        /// </summary>
        public Parameter String1 { get; private set; }

        /// <summary>
        /// The second string
        /// </summary>
        public Parameter String2 { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        public Concat(EfsSystem efsSystem)
            : base(efsSystem, "Concat")
        {
            String1 = (Parameter)acceptor.getFactory().createParameter();
            String1.Name = "String1";
            String1.Type = EFSSystem.StringType;
            String1.setFather(this);
            FormalParameters.Add(String1);

            String2 = (Parameter)acceptor.getFactory().createParameter();
            String2.Name = "String2";
            String2.Type = EFSSystem.StringType;
            String2.setFather(this);
            FormalParameters.Add(String2);
        }

        /// <summary>
        /// The return type of the function
        /// </summary>
        public override Type ReturnType
        {
            get { return EFSSystem.StringType; }
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
            StringValue retVal = null;

            int token = context.LocalScope.PushContext();
            AssignParameters(context, actuals);

            StringValue string1 = context.FindOnStack(String1).Value as StringValue;
            StringValue string2 = context.FindOnStack(String2).Value as StringValue;
            if (string1 != null && string2 != null)
            {
                retVal = new StringValue(EfsSystem.Instance.StringType, string1.Val + string2.Val);
            }

            context.LocalScope.PopContext(token);

            return retVal;
        }
    }
}