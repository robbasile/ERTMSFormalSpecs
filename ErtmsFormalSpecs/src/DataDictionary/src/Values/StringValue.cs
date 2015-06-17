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

using DataDictionary.Types;

namespace DataDictionary.Values
{
    public class StringValue : BaseValue<StringType, string>
    {
        public override string Name
        {
            get
            {
                string retVal = Val;

                string tmp = retVal;
                if (!tmp.StartsWith("'"))
                {
                    retVal = "'" + retVal;
                }
                if (!tmp.EndsWith("'"))
                {
                    retVal = retVal + "'";
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type"></param>
        public StringValue(StringType type, string val)
            : base(type, val)
        {
        }
    }
}