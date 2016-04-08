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

namespace DataDictionary
{
    /// <summary>
    /// References something that can be edited
    /// </summary>
    public interface IEditable
    {
        /// <summary>
        /// The editable text
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Indicates whether the syntax highlighting should be used while editing
        /// </summary>
        bool SyntaxHightlight { get; }

        /// <summary>
        /// The title of expression that are edited
        /// </summary>
        string Title { get; }
    }
}