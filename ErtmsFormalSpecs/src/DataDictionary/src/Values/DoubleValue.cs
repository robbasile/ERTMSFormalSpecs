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

using System.Globalization;
using System.Linq;
using DataDictionary.Types;

namespace DataDictionary.Values
{
    public class DoubleValue : BaseValue<Type, double>
    {
        public override string Name
        {
            get
            {
                CultureInfo info = CultureInfo.InvariantCulture;
                string retVal = Val.ToString(CultureInfo.InvariantCulture);
                if (!retVal.Contains('.'))
                {
                    retVal = retVal + ".0";
                }
                return retVal;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type"></param>
        public DoubleValue(Type type, double val)
            : base(type, val)
        {
        }
    }
}