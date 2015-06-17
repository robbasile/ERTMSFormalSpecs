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

using System.Runtime.Serialization;
using DataDictionary.Types;
using DataDictionary.Values;

namespace GUI.IPCInterface.Values
{
    [DataContract]
    public class BoolValue : Value
    {
        /// <summary>
        ///     The actual value
        /// </summary>
        [DataMember]
        public bool Value { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value"></param>
        public BoolValue(bool value)
        {
            Value = value;
        }

        /// <summary>
        ///     Provides the display value of this value
        /// </summary>
        /// <returns></returns>
        public override string DisplayValue()
        {
            return Value.ToString();
        }

        /// <summary>
        ///     Converts the value provided as an EFS value
        /// </summary>
        /// <returns></returns>
        public override IValue convertBack(Type type)
        {
            IValue retVal = type.getValue(Value.ToString());

            CheckReturnValue(retVal, type);
            return retVal;
        }
    }
}