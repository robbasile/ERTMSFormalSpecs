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
using System.Windows.Forms.DataVisualization.Charting;
using DataDictionary.Functions;
using GUIUtils.GraphVisualization.Functions;

namespace GUIUtils.GraphVisualization.Graphs
{
    public class ProfileFunctionGraph : Graph
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graphVisualizer"></param>
        /// <param name="function"></param>
        public ProfileFunctionGraph(GraphVisualizer graphVisualizer, ProfileFunction function)
            : base(graphVisualizer)
        {
            Function = function;
        }

        /// <summary>
        /// Handles the display
        /// </summary>
        /// <param name="maxDistance"></param>
        /// <param name="minDistance"></param>
        /// <param name="height"></param>
        protected override void HandleDisplay(double maxDistance, double minDistance, double height)
        {
            SpeedDistancePoint startingPoint = DisplayPreviousData(maxDistance);

            ProfileSetFunction profileSetFunction = Function as ProfileSetFunction;
            if (profileSetFunction != null)
            {
                foreach (IGraph graph in profileSetFunction.Functions)
                {
                    DisplayGraph(graph, maxDistance, startingPoint);
                }
            }
            else
            {
                ProfileFunction profileFunction = Function as ProfileFunction;
                if (profileFunction != null)
                {
                    DisplayGraph(profileFunction.Function, maxDistance, startingPoint);
                }
            }
        }

        /// <summary>
        /// Adds a new point
        /// </summary>
        /// <param name="point"></param>
        protected override void AddPoint(SpeedDistancePoint point)
        {
            DataPoint newPoint = new DataPoint(point.Distance, point.Speed);
            if (Data.Points.Count > 0 && Data.Points[Data.Points.Count - 1].XValue == point.Distance)
            {
                Data.Points[Data.Points.Count - 1] = newPoint;
            }
            else
            {
                Data.Points.Add(newPoint);
            }
        }

        /// <summary>
        /// Displays the provided function
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="maxDistance"></param>
        /// <param name="startingPoint"></param>
        private void DisplayGraph(IGraph graph, double maxDistance, SpeedDistancePoint startingPoint)
        {
            if (graph != null && startingPoint != null)
            {
                // This empty data point is added in order to allow displaying several
                // deceleration curves without linking the end of the previous curve
                // with the start of the following curve

                DataPoint dataPoint = new DataPoint(0, 0);
                for (int i = 0; i < graph.CountSegments(); i++)
                {
                    ISegment segment = graph.GetSegment(i);
                    if (segment.D0 <= maxDistance && (segment.D0 >= startingPoint.Distance || (segment.Length == double.MaxValue || segment.D0 + segment.Length > startingPoint.Distance)))
                    {
                        double startLocation = Math.Max(startingPoint.Distance, segment.D0);
                        double endLocation = maxDistance;
                        if (!double.IsNaN(segment.Length))
                        {
                            endLocation = Math.Min(segment.D0 + segment.Length, endLocation);
                        }

                        if (segment.A == 0) // this is a flat segment
                        {
                            AddPoint(new DataPoint(startLocation, segment.V0));
                            AddPoint(new DataPoint(endLocation, segment.V0));
                        }
                        else // this is a curve
                        {
                            double distanceInterval = (endLocation - startLocation) /
                                                      GraphVisualizer.DecelerationCurvePrecision;
                            double distance = startLocation;
                            for (int j = 0; j < GraphVisualizer.DecelerationCurvePrecision; j++)
                            {
                                AddDataPoint(distance, segment);
                                distance += distanceInterval;
                            }
                            AddDataPoint(distance, segment);
                        }
                    }
                }
                dataPoint = new DataPoint(0, 0);
                dataPoint.IsEmpty = true;
                AddPoint(dataPoint);
            }
        }

        /// <summary>
        /// Adds a data point for a segment
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="segment"></param>
        private void AddDataPoint(double distance, ISegment segment)
        {
            double speed = segment.Evaluate(distance);
            DataPoint dataPoint = new DataPoint(distance, speed);
            Data.Points.Add(dataPoint);
        }

        /// <summary>
        /// Nothing to save
        /// </summary>
        protected override void SaveSettings()
        {
        }
    }
}
