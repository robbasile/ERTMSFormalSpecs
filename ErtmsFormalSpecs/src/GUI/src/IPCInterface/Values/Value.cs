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
using System.ServiceModel;
using DataDictionary.Types;
using DataDictionary.Values;

namespace GUI.IPCInterface.Values
{
    [DataContract]
    [KnownType(typeof (BoolValue))]
    [KnownType(typeof (IntValue))]
    [KnownType(typeof (DoubleValue))]
    [KnownType(typeof (StringValue))]
    [KnownType(typeof (StateValue))]
    [KnownType(typeof (EnumValue))]
    [KnownType(typeof (ListValue))]
    [KnownType(typeof (StructureValue))]
    [KnownType(typeof (FunctionValue))]
    public abstract class Value
    {
        /// <summary>
        ///     Provides the display value of this value
        /// </summary>
        /// <returns></returns>
        public abstract string DisplayValue();

        /// <summary>
        ///     Converts the value provided as an EFS value
        /// </summary>
        /// <param name="type">the value expected type</param>
        /// <returns></returns>
        public abstract IValue ConvertBack(Type type);

        /// <summary>
        ///     Checks the return value for a conversion to EFS type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public void CheckReturnValue(IValue value, Type type)
        {
            if (value == null)
            {
                throw new FaultException<EFSServiceFault>(
                    new EFSServiceFault("Cannot convert to EFS value " + DisplayValue() + " for type " + type.FullName));
            }
        }
    }
}