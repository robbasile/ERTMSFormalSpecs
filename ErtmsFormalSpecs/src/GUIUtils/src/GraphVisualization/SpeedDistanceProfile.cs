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
