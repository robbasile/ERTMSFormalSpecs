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
using DataDictionary.Values;

namespace DataDictionary.Types
{

    /// <summary>
    ///     This is a type which can enumerate its possible values
    /// </summary>
    public interface IEnumerateValues
    {
        /// <summary>
        ///     Provides all constant values from this type
        /// </summary>
        /// <param name="scope">the current scope to identify the constant</param>
        /// <param name="retVal">the dictionary to fill which maps name->value</param>
        void Constants(string scope, Dictionary<string, object> retVal);

        /// <summary>
        ///     Provides the values whose name matches the name provided
        /// </summary>
        /// <param name="index">the index in names to consider</param>
        /// <param name="names">the simple value names</param>
        IValue findValue(string[] names, int index);
    }
}
