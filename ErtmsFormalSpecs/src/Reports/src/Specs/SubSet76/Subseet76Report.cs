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

using MigraDoc.DocumentObjectModel;

namespace Reports.Specs.SubSet76
{
    /// <summary>
    /// Handles specific S76 related functionalities while creating the corresponding report
    /// </summary>
    internal class Subseet76Report : ReportTools
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="document"></param>
        public Subseet76Report(Document document)
            : base(document)
        {
        }
    }
}