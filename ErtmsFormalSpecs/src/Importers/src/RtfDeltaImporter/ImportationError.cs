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

namespace Importers.RtfDeltaImporter
{
    public class ImportationError
    {
        /// <summary>
        ///     The error message
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        ///     The paragraph on which the error message applies
        /// </summary>
        public Paragraph Paragraph { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="paragraph"></param>
        public ImportationError(string message, Paragraph paragraph)
        {
            Message = message;
            Paragraph = paragraph;
        }
    }
}