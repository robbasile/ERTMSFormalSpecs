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

using DataDictionary.Generated;
using Utils;

namespace DataDictionary.Types
{

    /// <summary>
    ///     This is an element which has a type
    /// </summary>
    public interface ITypedElement : IModelElement
    {
        /// <summary>
        ///     The namespace related to the typed element
        /// </summary>
        NameSpace NameSpace { get; }

        /// <summary>
        ///     Provides the type name of the element
        /// </summary>
        string TypeName { get; set; }

        /// <summary>
        ///     The type of the element
        /// </summary>
        Type Type { get; set; }

        /// <summary>
        ///     Provides the mode of the typed element
        /// </summary>
        acceptor.VariableModeEnumType Mode { get; }

        /// <summary>
        ///     Provides the default value of the typed element
        /// </summary>
        string Default { get; set; }
    }
}
