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

namespace GUIUtils.GraphVisualization
{
    public class SpeedDistanceProfile
    {
        /// <summary>
        /// The points of the profile
        /// </summary>
        public List<SpeedDistancePoint> Points;


        /// <summary>
        /// Constructor
        /// </summary>
        public SpeedDistanceProfile()
        {
            Points = new List<SpeedDistancePoint>();
        }

        /// <summary>
        /// Adds the provided point
        /// </summary>
        /// <param name="point"></param>
        public void Add(SpeedDistancePoint point)
        {
            if (!double.IsNaN(point.Distance) && !double.IsNaN(point.Speed))
            {
                Points.Add(point);
            }
        }

        /// <summary>
        /// Clears the data
        /// </summary>
        public void Clear()
        {
            Points.Clear();
        }

        /// <summary>
        /// Provides the number of points
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return Points.Count;
        }
    }
}
