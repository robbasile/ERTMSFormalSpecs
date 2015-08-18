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

using System;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using DataDictionary.Functions;
using GUIUtils.GraphVisualization.Functions;

namespace GUIUtils.GraphVisualization.Graphs
{
    public class SurfaceGraph : Graph
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="graphVisualizer"></param>
        /// <param name="function"></param>
        public SurfaceGraph(GraphVisualizer graphVisualizer, SurfaceFunction function)
            : base(graphVisualizer)
        {
            Function = function;
        }

        /// <summary>
        ///     Handles the display
        /// </summary>
        /// <param name="maxDistance"></param>
        /// <param name="minDistance"></param>
        /// <param name="height"></param>
        protected override void HandleDisplay(double maxDistance, double minDistance, double height)
        {
            SurfaceFunction surface = Function as SurfaceFunction;
            if (surface != null)
            {
                if (double.IsNaN(height))
                {
                    if (!double.IsNaN(GraphVisualizer.MaxY))
                    {
                        height = GraphVisualizer.MaxY;
                    }
                }
                if (double.IsNaN(height))
                {
                    height = surface.MaxVal();
                }
                foreach (ISurfaceSegment surfaceSegment in surface.Surface.Segments)
                {
                    for (int i = 0; i < surfaceSegment.Graph.CountSegments(); i++)
                    {
                        ISegment segment = surfaceSegment.Graph.GetSegment(i);
                        DrawLine(surfaceSegment.Start, height);
                        DrawSegment(segment, Math.Max(surfaceSegment.Start, minDistance),
                            Math.Min(surfaceSegment.End, maxDistance), height);
                    }
                }
            }
        }

        /// <summary>
        ///     Draws a vertical line until maxValue
        /// </summary>
        /// <param name="position"></param>
        /// <param name="maxValue"></param>
        private void DrawLine(double position, double maxValue)
        {
            Data.Points.Add(new DataPoint(position, 0));
            Data.Points.Add(new DataPoint(position, maxValue));
            DataPoint emptyPoint = new DataPoint(position, maxValue);
            emptyPoint.IsEmpty = true;
            Data.Points.Add(emptyPoint);
        }

        /// <summary>
        ///     Draws the provided segment
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="maxValue"></param>
        private void DrawSegment(ISegment segment, double start, double end, double maxValue)
        {
            if (segment.D0 < maxValue)
            {
                Data.Points.Add(new DataPoint(start, segment.D0));
                Data.Points.Add(new DataPoint(end, segment.D0));
                DataPoint emptyPoint = new DataPoint((start + end)/2,
                    segment.D0 + (Math.Min(segment.Length, (maxValue - segment.D0))/2));
                emptyPoint.IsEmpty = true;
                Data.Points.Add(emptyPoint);
                TextAnnotation annotation = new TextAnnotation();
                annotation.AxisX = GraphVisualizer.ChartAreas[0].AxisX;
                annotation.AxisY = GraphVisualizer.ChartAreas[0].AxisY;
                annotation.AnchorAlignment = ContentAlignment.MiddleCenter;
                annotation.AnchorX = emptyPoint.XValue;
                annotation.AnchorY = emptyPoint.YValues[0];
                annotation.Text = segment.V0 + "m/s²";
                GraphVisualizer.Annotations.Add(annotation);
            }
        }

        /// <summary>
        ///     Nothing to save
        /// </summary>
        protected override void SaveSettings()
        {
        }
    }
}