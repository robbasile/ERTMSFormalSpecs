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
using GUIUtils.GraphVisualization;
using GUIUtils.GraphVisualization.Functions;
using GUIUtils.GraphVisualization.Graphs;

namespace GUI.GraphView
{
    public class EfsGradientProfileGraph : GradientProfileGraph
    {
        public EfsGradientProfileGraph(GraphVisualizer graphVisualizer, ProfileFunction function, string functionName)
            : base(graphVisualizer, function)
        {
            InitializeProperties(SeriesChartType.Line, functionName, functionName);
        }
    }
}
