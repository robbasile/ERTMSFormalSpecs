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
using System.Globalization;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Values;
using DataDictionary.Variables;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Functions.PredefinedFunctions
{
    /// <summary>
    ///     Checks the validity of a string
    /// </summary>
    public class CheckNumber : PredefinedFunction
    {
        /// <summary>
        ///     The number being checked
        /// </summary>
        public Parameter Number { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        public CheckNumber(EfsSystem efsSystem)
            : base(efsSystem, "CheckNumber")
        {
            Number = (Parameter) acceptor.getFactory().createParameter();
            Number.Name = "Number";
            Number.Type = EFSSystem.AnyType;
            Number.setFather(this);
            FormalParameters.Add(Number);
        }

        /// <summary>
        ///     The return type of the before function
        /// </summary>
        public override Type ReturnType
        {
            get { return EFSSystem.BoolType; }
        }

        /// <summary>
        ///     Provides the value of the function.
        ///     The function returns true if the string passes the check.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actuals">the actual parameters values</param>
        /// <param name="explain"></param>
        /// <returns>The value for the function application</returns>
        public override IValue Evaluate(InterpretationContext context, Dictionary<Actual, IValue> actuals,
            ExplanationPart explain)
        {
            IValue retVal = EFSSystem.BoolType.False;

            int token = context.LocalScope.PushContext();
            AssignParameters(context, actuals);

            StringValue number = context.FindOnStack(Number).Value as StringValue;
            if (number != null && number.Val != "")
            {
                string textValue = number.Val;
                NumberStyles style = NumberStyles.Number; 
                if (textValue.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
                {
                    textValue = textValue.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                if (textValue.EndsWith(" h", StringComparison.CurrentCultureIgnoreCase))
                {
                    textValue = textValue.Substring(0, textValue.Length - 2);
                    style = NumberStyles.HexNumber;
                }
                while (textValue[textValue.Length - 1] == 'F')
                {
                    textValue = textValue.Substring(0, textValue.Length - 1);
                    style = NumberStyles.HexNumber;
                }

                if (style == NumberStyles.HexNumber)
                {
                    ulong val;
                    if (ulong.TryParse(textValue, style, CultureInfo.InvariantCulture, out val))
                    {
                        retVal = EFSSystem.BoolType.True;
                    }
                }
                else
                {
                    Decimal val;
                    if (Decimal.TryParse(textValue, style, CultureInfo.InvariantCulture, out val))
                    {
                        retVal = EFSSystem.BoolType.True;
                    }                    
                }
            }
            context.LocalScope.PopContext(token);

            return retVal;
        }
    }
}