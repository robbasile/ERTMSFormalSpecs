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
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using Function = GUIUtils.GraphVisualization.Functions.Function;

namespace GUIUtils.GraphVisualization.Graphs
{
    public abstract class Graph
    {
        /// <summary>
        /// The function
        /// </summary>
        protected Function Function;

        /// <summary>
        /// The chart on which is displayed this graph
        /// </summary>
        protected GraphVisualizer GraphVisualizer { get; set; }

        /// <summary>
        /// The graphical information
        /// </summary>
        public Series Data { get; set; }

        /// <summary>
        /// Indicates if the display of the graph is enabled
        /// </summary>
        public bool IsEnabled
        {
            get { return Data.Enabled; }
            set
            {
                Data.Enabled = value;
                SaveSettings();
            }
        }

        /// <summary>
        /// The name of the graph
        /// </summary>
        public string Name
        {
            get { return Data.Name; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graphVisualizer"></param>
        public Graph(GraphVisualizer graphVisualizer)
        {
            GraphVisualizer = graphVisualizer;

            Data = new Series();
            GraphVisualizer.Series.Add(Data);
        }

        /// <summary>
        /// Initializes graph properties using provided parameters
        /// </summary>
        /// <param name="chartType"></param>
        /// <param name="name"></param>
        /// <param name="tooltip"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        protected virtual void InitializeProperties(SeriesChartType chartType, string name, string tooltip, Color color = default(Color))
        {
            Data.BorderDashStyle = ChartDashStyle.Solid;
            Data.BorderWidth = 4;
            Data.ChartType = chartType;
            Data.LegendText = name;
            Data.LegendToolTip = tooltip;
            if (!color.IsEmpty)
            {
                Data.Color = color;
            }
            Data.Name = name;
            Data.ChartArea = "ChartArea";
            Data.Legend = "Legend";
        }

        /// <summary>
        /// Displays the graph
        /// </summary>
        /// <param name="maxDistance"></param>
        /// <param name="minDistance"></param>
        /// <param name="height"></param>
        public void Display(double maxDistance, double minDistance = 0, double height = double.NaN)
        {
            ClearData();
            if (IsEnabled)
            {
                HandleDisplay(maxDistance, minDistance, height);
            }
            // in case the maximum value of Y axis is not determined automatically => compute it
            if (!double.IsNaN(GraphVisualizer.MaxY))
            {
                foreach (DataPoint point in Data.Points)
                {
                    if (point.YValues[0] > GraphVisualizer.ChartAreas[0].AxisY.Maximum && !point.IsEmpty)
                    {
                        GraphVisualizer.SetMaxY(point.YValues[0]);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the display
        /// </summary>
        /// <param name="maxDistance"></param>
        /// <param name="minDistance"></param>
        /// <param name="height"></param>
        protected abstract void HandleDisplay(double maxDistance, double minDistance, double height);

        /// <summary>
        /// Displays the previously recorded data until the provided distance
        /// </summary>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        protected SpeedDistancePoint DisplayPreviousData(double maxDistance)
        {
            SpeedDistancePoint retVal = new SpeedDistancePoint(0, 0);
            List<SpeedDistancePoint> tmp = new List<SpeedDistancePoint>(Function.PreviousData.Points);
            foreach (SpeedDistancePoint point in tmp)
            {
                if (point.Distance <= maxDistance)
                {
                    AddPoint(point);
                    retVal.Distance = point.Distance;
                    retVal.Speed = point.Speed;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds a new point
        /// </summary>
        /// <param name="point"></param>
        protected virtual void AddPoint(SpeedDistancePoint point)
        {
            Data.Points.Add(new DataPoint(point.Distance, point.Speed));
        }

        /// <summary>
        /// Updates and saves the settings
        /// </summary>
        protected abstract void SaveSettings();

        /// <summary>
        ///     Adds a data point to the list of points
        /// </summary>
        /// <param name="point"></param>
        public void AddPoint(DataPoint point)
        {
            Data.Points.Add(point);
        }

        /// <summary>
        ///     Clears all the data of that graph
        /// </summary>
        public void ClearData()
        {
            Data.Points.Clear();
        }
    }
}