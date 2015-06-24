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

namespace EFSServiceClient.EFSService
{
    /// <summary>
    ///     Manually written code to access EFSModel
    /// </summary>
    public partial class Segment
    {
        /// <summary>
        ///     Provides the display value of this value
        /// </summary>
        /// <returns></returns>
        public virtual string DisplayValue()
        {
            return String.Format("{0}[A={1},V0={2},D0={3}]", Length, A, V0, D0);
        }

        /// <summary>
        /// Evaluates the value of the function for the given X
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Evaluate(double x)
        {
            // Quadratic curve segment equation follows
            // f(x) = Sqrt(V0^2 + (2.0 * A * (x-D0))
            // Where dx is the delta between
            // In case of a step function, A = 0
            double retVal = Math.Sqrt(V0*V0 + 2*A*(x - D0));

            return retVal;
        }
    }

    /// <summary>
    ///     Manually written code to access EFSModel
    /// </summary>
    public partial class FunctionValue : Value, IFunctionValue
    {
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
        /// Evaluates the function for a given X
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Evaluate(double x)
        {
            double retVal = 0;

            double start = 0;
            foreach (Segment segment in Segments)
            {
                if (x >= start && x <= segment.Length)
                {
                    retVal = segment.Evaluate(x);
                    break;
                }

                start = start + segment.Length;
            }

            return retVal;
        }
    }
}
