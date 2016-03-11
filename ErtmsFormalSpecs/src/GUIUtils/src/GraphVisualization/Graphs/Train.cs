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

using System;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using GUIUtils.Properties;

namespace GUIUtils.GraphVisualization.Graphs
{
    public class Train : Graph
    {
        /// <summary>
        ///     The position of the train
        /// </summary>
        public DataPoint Position;

        /// <summary>
        /// This point is used to separate the point designating the train position
        /// from the rectangle designating the train odometry
        /// </summary>
        private readonly DataPoint hidden_point;

        /// <summary>
        /// The annotation representing the train
        /// </summary>
        public ImageAnnotation TrainAnnotation;

        /// <summary>
        /// The annotation representing the train position
        /// </summary>
        public VerticalLineAnnotation TrainLineAnnotation;

        /// <summary>
        /// The lower bound of the measured speed
        /// </summary>
        public DataPoint BottomLeft;

        /// <summary>
        /// The upper bound of the measured speed
        /// </summary>
        public DataPoint TopLeft;

        /// <summary>
        /// The under-reading amount
        /// </summary>
        private double under_reading_amount;

        /// <summary>
        /// The under-reading amount
        /// </summary>
        public DataPoint BottomRight;

        /// <summary>
        /// The over-reading amount
        /// </summary>
        private double over_reading_amount;

        /// <summary>
        /// The over-reading amount
        /// </summary>
        public DataPoint TopRight;

        /// <summary>
        /// The fixed speed lower bound in odometer accuracy
        /// </summary>
        public double SpeedLowerBound;

        /// <summary>
        /// The fixed speed upper bound in odometer accuracy
        /// </summary>
        public double SpeedUpperBound;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graph_visualizer"></param>
        public Train(GraphVisualizer graph_visualizer)
            : base(graph_visualizer)
        {
            Position = new DataPoint();
            Position.XValue = 0;
            hidden_point = new DataPoint(0, 0);
            hidden_point.IsEmpty = true;

            TopLeft = new DataPoint();
            BottomLeft = new DataPoint();
            BottomRight = new DataPoint();
            TopRight = new DataPoint();

            under_reading_amount = 0;
            over_reading_amount = 0;
            SpeedLowerBound = 0.08;
            SpeedUpperBound = 0.08;

            TrainAnnotation = new ImageAnnotation();
            TrainAnnotation.AnchorDataPoint = Position;
            TrainAnnotation.AnchorOffsetY = -4;
            TrainAnnotation.Image = "TrainMoving";

            TrainLineAnnotation = new VerticalLineAnnotation();
            TrainLineAnnotation.AxisX = GraphVisualizer.AxisX;
            TrainLineAnnotation.AxisY = GraphVisualizer.AxisY;
            TrainLineAnnotation.LineColor = Color.LightGray;
            TrainLineAnnotation.LineDashStyle = ChartDashStyle.Dash;
            TrainLineAnnotation.LineWidth = 2;
            TrainLineAnnotation.IsSizeAlwaysRelative = false;

            UpdateToolTip();

            InitializeProperties(SeriesChartType.Line, "Train position and accuracy",
                "Represents the position of the train and the odometry accuracy", Color.Red);
        }

        /// <summary>
        /// Initializes the train with the provided parameters
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="speed"></param>
        /// <param name="an_under_reading_amount"></param>
        /// <param name="an_over_reading_amount"></param>
        public void InitializeTrain(double distance, double speed, double an_under_reading_amount, double an_over_reading_amount)
        {
            AddPoint(Position);
            AddPoint(hidden_point);
            AddPoint(BottomLeft);
            AddPoint(TopLeft);
            AddPoint(TopRight);
            AddPoint(BottomRight);
            AddPoint(BottomLeft);
            Move(distance, speed);
            UpdateOdometryAccuracy(an_under_reading_amount, an_over_reading_amount);
        }

        /// <summary>
        /// Handles the display
        /// </summary>
        /// <param name="max_distance"></param>
        /// <param name="min_distance"></param>
        /// <param name="height"></param>
        protected override void HandleDisplay(double max_distance, double min_distance, double height)
        {
        }

        /// <summary>
        /// Updates and saves the settings
        /// </summary>
        protected override void SaveSettings()
        {
        }

        /// <summary>
        /// Moves the train at the provided position and speed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="speed"></param>
        public void Move(double position, double speed)
        {
            if (!double.IsNaN(position))
            {
                TrainAnnotation.Visible = true;
                Position.IsEmpty = false;
                MoveAtPosition(position);
                MoveAtSpeed(speed);
            }
            else
            {
                TrainAnnotation.Visible = false;
                Position.IsEmpty = true;
            }
        }

        /// <summary>
        /// Moves the train at the provided position
        /// </summary>
        /// <param name="position"></param>
        public void MoveAtPosition(double position)
        {
            Position.XValue = position;

            if (position == 0)
            {
                TrainLineAnnotation.Height = 0;
            }
            else
            {
                TrainLineAnnotation.Y = GraphVisualizer.ChartAreas[0].AxisY.Minimum;
                TrainLineAnnotation.Height = Math.Abs(GraphVisualizer.ChartAreas[0].AxisY.Minimum) + GraphVisualizer.ChartAreas[0].AxisY.Maximum;
            }

            TrainLineAnnotation.X = Position.XValue;

            UpdateToolTip();
        }

        /// <summary>
        /// Moves the train at the provided speed
        /// </summary>
        /// <param name="speed"></param>
        public void MoveAtSpeed(double speed)
        {
            if (!double.IsNaN(speed))
            {
                BottomLeft.XValue = Math.Max(Position.XValue - under_reading_amount, 0);
                BottomLeft.YValues[0] = Math.Max(speed - SpeedLowerBound, 0);
                TopLeft.XValue = Math.Max(Position.XValue - under_reading_amount, 0);
                TopLeft.YValues[0] = speed + SpeedUpperBound;
                BottomRight.XValue = Position.XValue + over_reading_amount;
                BottomRight.YValues[0] = Math.Max(speed - SpeedLowerBound, 0);
                TopRight.XValue = Position.XValue + over_reading_amount;
                TopRight.YValues[0] = speed + SpeedUpperBound;
            }
            else
            {
                BottomLeft.XValue = 0;
                BottomLeft.YValues[0] = 0;
                TopLeft.XValue = 0;
                TopLeft.YValues[0] = 0;
                BottomRight.XValue = 0;
                BottomRight.YValues[0] = 0;
                TopRight.XValue = 0;
                TopRight.YValues[0] = 0;
            }
        }

        /// <summary>
        /// Updates the odometry accuracy
        /// </summary>
        /// <param name="an_under_reading_amount"></param>
        /// <param name="an_over_reading_amount"></param>
        public void UpdateOdometryAccuracy(double an_under_reading_amount, double an_over_reading_amount)
        {
            under_reading_amount = an_under_reading_amount;
            over_reading_amount = an_over_reading_amount;
        }

        /// <summary>
        /// Removes the train
        /// </summary>
        public void RemoveTrain()
        {
            Data.Points.Clear();
        }

        /// <summary>
        ///     Updates the tooltip of the train according to its position
        /// </summary>
        private void UpdateToolTip()
        {
            TrainAnnotation.ToolTip = Math.Round(Position.XValue, 2) + "m";
        }
    }
}