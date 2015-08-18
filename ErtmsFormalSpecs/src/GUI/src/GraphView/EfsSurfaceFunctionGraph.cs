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

using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using GUIUtils.GraphVisualization;
using GUIUtils.GraphVisualization.Functions;
using GUIUtils.GraphVisualization.Graphs;

namespace GUI.GraphView
{
    public class EfsSurfaceFunctionGraph : SurfaceGraph
    {
        public EfsSurfaceFunctionGraph(GraphVisualizer graphVisualizer, SurfaceFunction function, string functionName)
            : base(graphVisualizer, function)
        {
            InitializeProperties(SeriesChartType.Line, functionName, functionName);
        }

        /// <summary>
        ///     Overrides the properties for surfaces graphs
        /// </summary>
        /// <param name="chartType"></param>
        /// <param name="name"></param>
        /// <param name="tooltip"></param>
        /// <param name="color"></param>
        protected override void InitializeProperties(SeriesChartType chartType, string name, string tooltip,
            Color color = default(Color))
        {
            base.InitializeProperties(chartType, name, tooltip, color);
            Data.BorderDashStyle = ChartDashStyle.Dash;
            Data.BorderWidth = 2;
        }
    }
}