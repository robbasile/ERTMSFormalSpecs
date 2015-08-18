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
using System.Collections.Generic;
using System.Runtime.Serialization;
using DataDictionary.Values;
using Type = DataDictionary.Types.Type;

namespace GUI.IPCInterface.Values
{
    [DataContract]
    public class Segment
    {
        /// Quadratic curve segment equation follows
        /// f(x) = Sqrt(V0^2 + (2.0 * A * (x-D0))
        /// Where dx is the delta between
        /// In case of a step function, A = 0
        /// <summary>
        ///     The A factor in the quadratic curve segment
        /// </summary>
        [DataMember]
        public double A { get; set; }

        /// <summary>
        ///     The V0 factor in the quadratic curve segment
        /// </summary>
        [DataMember]
        public double V0 { get; set; }

        /// <summary>
        ///     The D0 factor in the quadratic curve segment
        ///     This is the start position of the segment
        /// </summary>
        [DataMember]
        public double D0 { get; set; }

        /// <summary>
        ///     The length of the segment
        /// </summary>
        [DataMember]
        public double Length { get; set; }

        /// <summary>
        ///     Provides the display value of this value
        /// </summary>
        /// <returns></returns>
        public virtual string DisplayValue()
        {
            return String.Format("{0}[A={1},V0={2},D0={3}]", Length, A, V0, D0);
        }
    }

    [DataContract]
    public class FunctionValue : Value
    {
        /// <summary>
        ///     The actual value
        /// </summary>
        [DataMember]
        public List<Segment> Segments { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value"></param>
        public FunctionValue(List<Segment> value)
        {
            Segments = value;
        }

        /// <summary>
        ///     Provides the display value of this value
        /// </summary>
        /// <returns></returns>
        public override string DisplayValue()
        {
            string retVal = "Function <";

            bool first = true;
            foreach (Segment segment in Segments)
            {
                if (!first)
                {
                    retVal += ", ";
                }

                retVal += segment.DisplayValue();
                first = false;
            }

            retVal += ">";

            return retVal;
        }

        /// <summary>
        ///     Converts the value provided as an EFS value
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override IValue ConvertBack(Type type)
        {
            // TODO : Cannot receive a function from the external world

            throw new NotImplementedException();
        }
    }
}