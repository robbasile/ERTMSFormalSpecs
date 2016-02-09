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
using Utils;

namespace GUIUtils.Editor
{
    /// <summary>
    ///     A reference to an object, displayed in the combo box
    /// </summary>
    public class ObjectReference : IComparable<ObjectReference>
    {
        /// <summary>
        ///     The display name of the object reference
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        ///     The model elements referenced by this object reference
        /// </summary>
        public INamable Model { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        public ObjectReference(string name, INamable model)
        {
            DisplayName = name;
            Model = model;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        // Summary:
        //     Compares the current object with another object of the same type.
        //
        // Parameters:
        //   other:
        //     An object to compare with this object.
        //
        // Returns:
        //     A value that indicates the relative order of the objects being compared.
        //     The return value has the following meanings: Value Meaning Less than zero
        //     This object is less than the other parameter.Zero This object is equal to
        //     other. Greater than zero This object is greater than other.
        public int CompareTo(ObjectReference other)
        {
            return String.Compare(DisplayName, other.DisplayName, StringComparison.Ordinal);
        }
    }
}
