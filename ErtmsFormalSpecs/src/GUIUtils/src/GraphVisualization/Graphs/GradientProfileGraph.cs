// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpecs software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System.Windows.Forms.DataVisualization.Charting;
using GUIUtils.GraphVisualization.Functions;

namespace GUIUtils.GraphVisualization.Graphs
{
    public class GradientProfileGraph : ProfileFunctionGraph
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="graphVisualizer"></param>
        /// <param name="function"></param>
        public GradientProfileGraph(GraphVisualizer graphVisualizer, ProfileFunction function)
            : base(graphVisualizer, function)
        {
            Function = function;
            graphVisualizer.EnableAxisY2 (); // the gradients are displayed on axis Y2
            Data.YAxisType = AxisType.Secondary;
        }
    }
}
