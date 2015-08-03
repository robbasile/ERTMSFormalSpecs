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
