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
using HistoricalData;

namespace DataDictionary.Compare
{
    /// <summary>
    ///     The model related history
    /// </summary>
    public class History : HistoricalData.History
    {
        /// <summary>
        ///     Provides the ordered set of changes on a given model element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public SortedSet<Change> GetChanges(ModelElement element)
        {
            return GetChanges(element.Guid);
        }
    }
}