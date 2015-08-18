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

namespace DataDictionary.Interpreter
{
    /// <summary>
    ///     Stores the association between a interpreter tree node and a value
    /// </summary>
    public class ReturnValueElement : IComparable<ReturnValueElement>
    {
        /// <summary>
        ///     The previous return value element in the
        /// </summary>
        public ReturnValueElement PreviousElement { get; set; }

        /// <summary>
        ///     The value
        /// </summary>
        public INamable Value { get; set; }

        /// <summary>
        ///     Indicates that the return value element was found as a type of its instance, instead of the instance itself.
        ///     This allow to differenciate between a structure and  the return value of a function of type structure
        ///     (in the latter case, the specific element cannot be statically identified in the model)
        /// </summary>
        public bool AsType { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="previous"></param>
        /// <param name="asType"></param>
        public ReturnValueElement(INamable value, ReturnValueElement previous = null, bool asType = false)
        {
            PreviousElement = previous;
            Value = value;
            AsType = asType;
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
        public int CompareTo(ReturnValueElement other)
        {
            int retVal = 1;

            if (other != null)
            {
                if (Value == other.Value && AsType == other.AsType)
                {
                    // This seem to be the same return value
                    retVal = 0;

                    // Ensure that previous elements match.
                    if (PreviousElement != null)
                    {
                        retVal = PreviousElement.CompareTo(other.PreviousElement);
                    }
                    else
                    {
                        if (other.PreviousElement != null)
                        {
                            retVal = -1;
                        }
                    }
                }
            }

            return retVal;
        }

        public override string ToString()
        {
            string retVal = Value.ToString();

            if (PreviousElement != null)
            {
                retVal += " -> " + PreviousElement;
            }

            return retVal;
        }
    }
}
