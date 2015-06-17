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

namespace DataDictionary.Types.AccessMode
{
    /// <summary>
    ///     This class represents a procedure or function call
    /// </summary>
    public class ProcedureOrFunctionCall : AccessMode, IComparable<ProcedureOrFunctionCall>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="call"></param>
        public ProcedureOrFunctionCall(NameSpace source, NameSpace target, ICallable call)
            : base(source, target)
        {
            Called = call;
        }

        /// <summary>
        ///     The model element which is referenced by this arrow
        /// </summary>
        public ICallable Called { get; private set; }

        /// <summary>
        ///     The referenced model
        /// </summary>
        public override ModelElement ReferencedModel
        {
            get { return (ModelElement) Called; }
        }

        /// <summary>
        ///     The name to be displayed
        /// </summary>
        public override string GraphicalName
        {
            get { return Called.Name; }
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
        public int CompareTo(ProcedureOrFunctionCall other)
        {
            int retVal = base.CompareTo((AccessMode) other);

            if (retVal == 0)
            {
                retVal = Called.FullName.CompareTo(other.Called.FullName);
            }

            return retVal;
        }
    }
}