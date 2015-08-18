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
using DataDictionary.Functions;

namespace EFSServiceClient.EFSService
{
    /// <summary>
    ///     Manually written code to access EFSModel
    /// </summary>
    public partial class Segment : ISegment
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
        ///     Evaluates the value of the function for the given X
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Evaluate(double x)
        {
            // Quadratic curve segment equation follows
            // f(x) = Sqrt(V0^2 + (2.0 * A * (x-D0))
            // Where dx is the delta between
            // In case of a step function, A = 0

            // V0 is expressed in km/h and has to be
            // converted to m/s
            double speedInKmH = V0/3.6;
            double retVal = Math.Sqrt(speedInKmH*speedInKmH + 2*A*(x - D0));
            retVal *= 3.6; // the returned speed is expressed in km/h

            return retVal;
        }
    }

    /// <summary>
    ///     Manually written code to access EFSModel
    /// </summary>
    public partial class FunctionValue : Value, IFunctionValue, IGraph
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

        public int CountSegments()
        {
            return Segments.Length;
        }

        public ISegment GetSegment(int i)
        {
            return Segments[i];
        }

        /// <summary>
        ///     Adds a new segment
        /// </summary>
        /// <param name="segment"></param>
        public void AddSegment(ISegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Evaluates the function for a given X
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Evaluate(double x)
        {
            double retVal = 0;

            double start = 0;
            foreach (Segment segment in Segments)
            {
                if (x >= start && x < start + segment.Length)
                {
                    retVal = segment.Evaluate(x);
                    break;
                }

                start = start + segment.Length;
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the last X value where there is some interest to show the graph
        /// </summary>
        /// <returns></returns>
        public double ExpectedEndX()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Negates the values of the curve
        /// </summary>
        public void Negate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Reduces the graph to the boundaries provided as parameter
        /// </summary>
        /// <param name="boundaries"></param>
        /// <returns>The reduced graph</returns>
        public void Reduce(List<ISegment> boundaries)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Adds a graph to this graph
        /// </summary>
        /// <param name="other"></param>
        /// <returns>the new graph</returns>
        public IGraph AddGraph(IGraph other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Substract a graph from this graph
        /// </summary>
        /// <param name="other"></param>
        /// <returns>the new graph</returns>
        public IGraph SubstractGraph(IGraph other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Multiply this graph values of another graph
        /// </summary>
        /// <param name="other"></param>
        /// <returns>the new graph</returns>
        public IGraph MultGraph(IGraph other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Divides this graph values by values of another graph
        /// </summary>
        /// <param name="other"></param>
        /// <returns>the new graph</returns>
        public IGraph DivGraph(IGraph other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Provides the graph of the minimal value between this graph and another graph
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public IGraph Min(IGraph other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Merges a graph within this one
        /// </summary>
        /// <param name="subGraph"></param>
        public void Merge(IGraph subGraph)
        {
            throw new NotImplementedException();
        }
    }
}