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
using GUIUtils.GraphVisualization;
using GUIUtils.GraphVisualization.Functions;

namespace GUI.GraphView
{
    public class EfsSurfaceFunction : SurfaceFunction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="surface"></param>
        public EfsSurfaceFunction(ISurface surface)
        {
            Surface = surface;
        }

        /// <summary>
        /// Computes the function value according to the provided parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected override SpeedDistancePoint GetValue(double parameter)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Relocates the function according to the LRBG position
        /// </summary>
        /// <param name="lrbgPosition"></param>
        protected override void Relocate(double lrbgPosition)
        {
            // not applicable for surfaces
        }

        /// <summary>
        /// Updates the function value according to the provided parameter
        /// </summary>
        /// <param name="parameter"></param>
        protected override void UpdateValue(double parameter)
        {
            // the value is already updated
        }
    }
}
