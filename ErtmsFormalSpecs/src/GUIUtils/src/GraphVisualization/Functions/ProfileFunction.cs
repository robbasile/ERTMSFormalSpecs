// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpecs software and documentation
// --
// --  ERTMSFormalSpecs is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpecs is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using DataDictionary.Functions;

namespace GUIUtils.GraphVisualization.Functions
{
    public abstract class ProfileFunction : Function
    {
        /// <summary>
        ///     The function to display
        /// </summary>
        public IGraph Function { get; set; }

        /// <summary>
        ///     Computes the function value according to the provided parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected override SpeedDistancePoint GetValue(double parameter)
        {
            SpeedDistancePoint retVal = null;

            if (Function != null)
            {
                retVal = new SpeedDistancePoint(parameter, Function.Evaluate(parameter));
            }

            return retVal;
        }

        /// <summary>
        ///     Relocates the function according to the LRBG position
        /// </summary>
        /// <param name="lrbgPosition"></param>
        protected override void Relocate(double lrbgPosition)
        {
            if (Function != null)
            {
                UpdateFunction(Function, lrbgPosition);
            }
        }

        /// <summary>
        ///     Updates the provided function and relocates it according to the value of increment
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="lrbgPosition"></param>
        protected void UpdateFunction(IGraph graph, double lrbgPosition)
        {
            if (graph != null)
            {
                for (int i = 0; i < graph.CountSegments(); i++)
                {
                    graph.GetSegment(i).D0 += lrbgPosition;
                }
            }
        }

        /// <summary>
        ///     Records the current value
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="relocatedPosition"></param>
        public virtual void RecordCurrentValue(double currentPosition, double relocatedPosition)
        {
            SpeedDistancePoint currentPoint = GetValue(relocatedPosition);
            if (currentPoint != null)
            {
                PreviousData.Add(new SpeedDistancePoint(currentPosition, currentPoint.Speed));
            }
        }

        /// <summary>
        ///     Clears the data related to that function
        /// </summary>
        public override void ClearData()
        {
            base.ClearData();
            Function = null;
        }
    }
}