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
using DataDictionary.Interpreter;
using DataDictionary.Variables;

namespace DataDictionary.Types.AccessMode
{
    /// <summary>
    ///     This class represents an access to a variable
    /// </summary>
    public class AccessToVariable : AccessMode, IComparable<AccessToVariable>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="variable"></param>
        /// <param name="accessMode"></param>
        public AccessToVariable(NameSpace source, NameSpace target, IVariable variable, Usage.ModeEnum accessMode)
            : base(source, target)
        {
            Variable = variable;
            AccessMode = accessMode;
        }

        /// <summary>
        ///     The model element which is referenced by this arrow
        /// </summary>
        public IVariable Variable { get; private set; }

        /// <summary>
        ///     The referenced model
        /// </summary>
        public override ModelElement ReferencedModel
        {
            get { return (ModelElement) Variable; }
        }

        /// <summary>
        ///     Indicates the way the variable has been accessed
        /// </summary>
        public Usage.ModeEnum AccessMode { get; private set; }

        /// <summary>
        ///     The name to be displayed
        /// </summary>
        public override string GraphicalName
        {
            get { return Variable.Name; }
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
        public int CompareTo(AccessToVariable other)
        {
            int retVal = base.CompareTo((AccessMode) other);

            if (retVal == 0)
            {
                retVal = Variable.FullName.CompareTo(other.Variable.FullName);
            }

            if (retVal == 0)
            {
                if (AccessMode != other.AccessMode)
                {
                    // Different access modes
                    if (AccessMode == Usage.ModeEnum.Read)
                    {
                        retVal = -1;
                    }
                    else if (AccessMode == Usage.ModeEnum.ReadAndWrite)
                    {
                        if (other.AccessMode == Usage.ModeEnum.Read)
                        {
                            retVal = 1;
                        }
                        else
                        {
                            retVal = -1;
                        }
                    }
                    else
                    {
                        // AccessMode == Write
                        retVal = 1;
                    }
                }
            }

            return retVal;
        }
    }
}