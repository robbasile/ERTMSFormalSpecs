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

namespace GUIUtils.GraphVisualization.Functions
{
    public abstract class Function
    {
        /// <summary>
        /// The list of previously recorded points
        /// </summary>
        public SpeedDistanceProfile PreviousData { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Function()
        {
            PreviousData = new SpeedDistanceProfile();
        }

        /// <summary>
        /// Erases the data of the function
        /// </summary>
        public virtual void ClearData()
        {
            PreviousData.Clear();
        }

        /// <summary>
        /// Computes the function value according to the provided parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected abstract SpeedDistancePoint GetValue(double parameter);

        /// <summary>
        /// Computes the value of the function for the provided parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="lrbgPosition"></param>
        public void UpdateFunctionAndRelocate(double parameter, double lrbgPosition)
        {
            UpdateValue(parameter);
            Relocate(lrbgPosition);
        }

        /// <summary>
        /// Relocates the function according to the LRBG position
        /// </summary>
        /// <param name="lrbgPosition"></param>
        protected abstract void Relocate(double lrbgPosition);

        /// <summary>
        /// Updates the function value according to the provided parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected abstract void UpdateValue(double parameter);
    }
}
