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
using System.Globalization;
using System.Runtime.Serialization;
using DataDictionary.Values;
using Type = DataDictionary.Types.Type;

namespace GUI.IPCInterface.Values
{
    [DataContract]
    public class DoubleValue : Value
    {
        /// <summary>
        ///     The actual image value
        /// </summary>
        [DataMember]
        public string Image { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value"></param>
        public DoubleValue(double value)
        {
            Image = value.ToString(CultureInfo.InvariantCulture);
            if (Math.Floor(value) == value)
            {
                Image = Image + ".0";
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="image"></param>
        public DoubleValue(string image)
        {
            Image = image;
        }

        /// <summary>
        ///     Provides the display value of this value
        /// </summary>
        /// <returns></returns>
        public override string DisplayValue()
        {
            return Image;
        }

        /// <summary>
        ///     Converts the value provided as an EFS value
        /// </summary>
        /// <returns></returns>
        public override IValue convertBack(Type type)
        {
            IValue retVal = type.getValue(Image);

            CheckReturnValue(retVal, type);
            return retVal;
        }
    }
}