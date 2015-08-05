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

namespace GUIUtils.GraphVisualization
{
    public class SpeedDistancePoint
    {
        /// <summary>
        /// The distance
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// The speed
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SpeedDistancePoint()
        {
            Distance = double.NaN;
            Speed = double.NaN;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="speed"></param>
        /// <param name="increment"></param>
        public SpeedDistancePoint(double distance, double speed, double increment = 0)
        {
            Distance = distance;
            Speed = speed;
            if (!double.IsNaN(Distance))
            {
                Distance += increment;
            }
        }

        /// <summary>
        /// Reinitializes the data
        /// </summary>
        public void ClearData()
        {
            Distance = 0;
            Speed = 0;
        }
    }
}
