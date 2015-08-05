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

using System.Collections.Generic;

namespace GUIUtils.GraphVisualization.Functions
{
    public abstract class PointFunction : Function
    {
        /// <summary>
        /// The point
        /// </summary>
        public SpeedDistancePoint Point { get; private set; }

        /// <summary>
        /// The values simulated for the different targets
        /// </summary>
        public List<SpeedDistanceProfile> SimulatedValues { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public PointFunction()
        {
            Point = new SpeedDistancePoint();
            SimulatedValues = new List<SpeedDistanceProfile>();
        }

        /// <summary>
        /// Computes the value according to the provided parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected override void UpdateValue(double parameter)
        {
            Point = GetValue(parameter);
        }

        /// <summary>
        /// Relocates the function according to the LRBG position
        /// </summary>
        /// <param name="lrbgPosition"></param>
        protected override void Relocate(double lrbgPosition)
        {
        }

        /// <summary>
        /// Records the current value
        /// </summary>
        /// <param name="currentSpeed"></param>
        public void RecordCurrentValue(double currentSpeed)
        {
            PreviousData.Add(GetValue(currentSpeed));
        }

        /// <summary>
        /// Clears the data related to that function
        /// </summary>
        public override void ClearData()
        {
            base.ClearData();
            Point = new SpeedDistancePoint();
            SimulatedValues = new List<SpeedDistanceProfile>();
        }

        /// <summary>
        /// Simulates the function values for different speeds
        /// </summary>
        public abstract void SimulateFunctionValues();
    }
}
