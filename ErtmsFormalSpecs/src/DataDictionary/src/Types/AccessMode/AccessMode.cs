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

namespace DataDictionary.Types.AccessMode
{
    /// <summary>
    ///     References an access to a model element, referencing the source and target namespaces
    /// </summary>
    public abstract class AccessMode : IGraphicalArrow<NameSpace>, IComparable<AccessMode>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public AccessMode(NameSpace source, NameSpace target)
        {
            Source = source;
            Target = target;
        }

        /// <summary>
        ///     The source of the arrow
        /// </summary>
        public NameSpace Source { get; private set; }

        /// <summary>
        ///     Sets the source box for this arrow
        /// </summary>
        /// <param name="initialBox"></param>
        public void SetInitialBox(IGraphicalDisplay initialBox)
        {
            // We cannot change a call through this
        }

        /// <summary>
        ///     The target of the arrow
        /// </summary>
        public NameSpace Target { get; private set; }

        /// <summary>
        ///     Sets the target box for this arrow
        /// </summary>
        /// <param name="targetBox"></param>
        public void SetTargetBox(IGraphicalDisplay targetBox)
        {
            // We cannot change a call through this
        }

        /// <summary>
        ///     The name to be displayed
        /// </summary>
        public abstract string GraphicalName { get; }

        /// <summary>
        ///     The referenced model element
        /// </summary>
        public abstract ModelElement ReferencedModel { get; }

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
        public int CompareTo(AccessMode other)
        {
            int retVal = 0;

            if (retVal == 0)
            {
                retVal = Source.CompareTo(other.Source);
            }

            if (retVal == 0)
            {
                retVal = Target.CompareTo(other.Target);
            }

            return retVal;
        }
    }
}