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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using GUIUtils.GraphVisualization.Graphs;

namespace GUIUtils.GraphVisualization
{
    public class GraphVisualizer : Chart
    {
        /// <summary>
        ///     The list of displayed graphs
        /// </summary>
        public List<Graph> Graphs { get; private set; }

        /// <summary>
        ///     The minimum displayed X value
        /// </summary>
        public double MinX
        {
            get { return ChartAreas[0].AxisX.ScaleView.ViewMinimum; }
        }

        /// <summary>
        ///     The maximum displayed X value
        /// </summary>
        public double MaxX
        {
            get { return ChartAreas[0].AxisX.ScaleView.ViewMaximum; }
        }

        /// <summary>
        ///     The minimum displayed Y value
        /// </summary>
        public double MinY
        {
            get { return ChartAreas[0].AxisY.ScaleView.ViewMinimum; }
        }

        /// <summary>
        ///     The maximum displayed Y value
        /// </summary>
        public double MaxY
        {
            get { return ChartAreas[0].AxisY.ScaleView.ViewMaximum; }
        }

        /// <summary>
        ///     The axis X
        /// </summary>
        public Axis AxisX
        {
            get { return ChartAreas[0].AxisX; }
        }

        /// <summary>
        ///     The axis Y
        /// </summary>
        public Axis AxisY
        {
            get { return ChartAreas[0].AxisY; }
        }

        /// <summary>
        ///     Indicates if the previously computed values of the functions should be recorded
        /// </summary>
        public bool RecordPreviousValuesInTsm { get; set; }

        /// <summary>
        ///     Gives the number of points computed to draw deceleration curves
        /// </summary>
        public int DecelerationCurvePrecision { get; set; }

        /// <summary>
        ///     Default contstructor
        /// </summary>
        public GraphVisualizer()
        {
            Graphs = new List<Graph>();
            RecordPreviousValuesInTsm = false;
            DecelerationCurvePrecision = 51;
        }

        /// <summary>
        ///     Initializes the properties
        /// </summary>
        /// <param name="recordPreviousValues"></param>
        /// <param name="decelerationCurvePrecision"></param>
        public void InitializeProperties(bool recordPreviousValues, int decelerationCurvePrecision)
        {
            RecordPreviousValuesInTsm = recordPreviousValues;
            DecelerationCurvePrecision = decelerationCurvePrecision;
        }

        /// <summary>
        ///     Initializes the chart area
        /// </summary>
        public void InitializeChart()
        {
            // Initialization of the chart area
            ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;

            ChartAreas[0].AxisX.MinorGrid.LineColor = Color.GhostWhite;
            ChartAreas[0].AxisX.MinorGrid.Enabled = true;
            ChartAreas[0].AxisY.MinorGrid.LineColor = Color.GhostWhite;
            ChartAreas[0].AxisY.MinorGrid.Enabled = true;

            ChartAreas[0].AxisX.Minimum = 0;
            ChartAreas[0].AxisY.Maximum = 50;
            ChartAreas[0].AxisY.Crossing = 0;

            ChartAreas[0].AxisX.LabelStyle.Format = "{#}";
            ChartAreas[0].AxisY.LabelStyle.Format = "{#}";

            ChartAreas[0].AxisY.IsMarginVisible = false;
            ChartAreas[0].AxisX.IsMarginVisible = false;
        }

        /// <summary>
        ///     Sets the minimum displayed value for X axis
        /// </summary>
        /// <param name="value"></param>
        public void SetMinX(double value)
        {
            ChartAreas[0].AxisX.Minimum = value;
        }

        /// <summary>
        ///     Sets the maximum displayed value for X axis
        /// </summary>
        /// <param name="value"></param>
        public void SetMaxX(double value)
        {
            ChartAreas[0].AxisX.Maximum = value;
        }

        /// <summary>
        ///     Sets the minimum displayed value for Y axis
        /// </summary>
        /// <param name="value"></param>
        public void SetMinY(double value)
        {
            ChartAreas[0].AxisY.Minimum = value;
        }

        /// <summary>
        ///     Sets the maximum displayed value for Y axis
        /// </summary>
        /// <param name="value"></param>
        public void SetMaxY(double value)
        {
            ChartAreas[0].AxisY.Maximum = value;
        }

        /// <summary>
        ///     Adds the provided graph to the list of displayed graphs
        /// </summary>
        /// <param name="graph"></param>
        public void AddGraph(Graph graph)
        {
            Graphs.Add(graph);
        }

        /// <summary>
        ///     Adds the provided range of graphs to the list of displayed graphs
        /// </summary>
        /// <param name="graphs"></param>
        public void AddGraphRange(List<Graph> graphs)
        {
            Graphs.AddRange(graphs);
        }

        /// <summary>
        ///     Clears the displayed annotations
        /// </summary>
        public void ClearAnnotations()
        {
            Annotations.Clear();
        }

        /// <summary>
        ///     Clears the displayed data
        /// </summary>
        public void ClearData()
        {
            ClearAnnotations();
            Graphs.Clear();
        }

        /// <summary>
        ///     Clears the data and erases the series
        /// </summary>
        public void Reset()
        {
            ClearData();
            Series.Clear();
        }

        /// <summary>
        ///     Draws the functions
        /// </summary>
        /// <param name="end"></param>
        /// <param name="start"></param>
        /// <param name="height"></param>
        public void DrawGraphs(double end = double.NaN, double start = double.NaN, double height = double.NaN)
        {
            if (double.IsNaN(end))
            {
                end = MaxX;
            }
            if (double.IsNaN(start))
            {
                start = 0;
            }
            foreach (Graph graph in Graphs)
            {
                graph.Display(end, start, height);
            }
        }

        /// <summary>
        ///     Performs the zoom operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="negativeOffset"></param>
        public void HandleZoom(object sender, MouseEventArgs e, int negativeOffset)
        {
            try
            {
                Chart chart = sender as Chart;
                if (chart != null)
                {
                    if (e.Location.X >= 0 && e.Location.X <= chart.Size.Width &&
                        e.Location.Y >= 0 && e.Location.Y <= chart.Size.Height)
                    {
                        HandleZoomforAxis(ChartAreas[0].AxisX, e.Location.X, 0, ChartAreas[0].AxisX.Maximum, e.Delta > 0);
                        HandleZoomforAxis(ChartAreas[0].AxisY, e.Location.Y, negativeOffset, ChartAreas[0].AxisY.Maximum,
                            e.Delta > 0);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Handles the zoom for the provided axis
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="mouseLocation"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="zoomIn"></param>
        private void HandleZoomforAxis(Axis axis, int mouseLocation, double minValue, double maxValue, bool zoomIn)
        {
            double min = axis.ScaleView.ViewMinimum;
            double max = axis.ScaleView.ViewMaximum;

            double start, end;

            if (zoomIn)
            {
                start = Math.Floor(min + (axis.PixelPositionToValue(mouseLocation) - min)/2);
                end = Math.Floor(max - (max - axis.PixelPositionToValue(mouseLocation))/2);
            }
            else
            {
                start = Math.Floor(min - (axis.PixelPositionToValue(mouseLocation) - min));
                end = Math.Floor(max + (max - axis.PixelPositionToValue(mouseLocation)));
            }

            if (start <= minValue || end >= maxValue)
            {
                ResetZoom();
            }
            else
            {
                start = Math.Max(minValue, start);
                end = Math.Min(maxValue, end);

                if (start < end)
                {
                    axis.ScaleView.Zoom(start, end);
                }
            }
        }

        /// <summary>
        ///     Resets the zoom of both axis
        /// </summary>
        public void ResetZoom()
        {
            ChartAreas[0].AxisX.ScaleView.ZoomReset();
            ChartAreas[0].AxisY.ScaleView.ZoomReset();
        }
    }
}