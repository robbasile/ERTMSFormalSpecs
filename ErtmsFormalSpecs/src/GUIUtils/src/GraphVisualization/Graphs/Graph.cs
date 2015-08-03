using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using GUIUtils.GraphVisualization.Functions;

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
        protected virtual void InitializeProperties(SeriesChartType chartType, string name, string tooltip, Color color)
        {
            Data.BorderDashStyle = ChartDashStyle.Solid;
            Data.BorderWidth = 4;
            Data.ChartType = chartType;
            Data.LegendText = name;
            Data.LegendToolTip = tooltip;
            Data.Color = color;
            Data.Name = name;
            Data.ChartArea = "ScenarioChartArea";
            Data.Legend = "Legend";
        }

        /// <summary>
        /// Displays the graph
        /// </summary>
        /// <param name="maxDistance"></param>
        /// <param name="minDistance"></param>
        /// <param name="height"></param>
        public void Display(double maxDistance, double minDistance = 0, double height = 0)
        {
            ClearData();
            if (IsEnabled)
            {
                HandleDisplay(maxDistance, minDistance, height);
            }
            foreach (DataPoint point in Data.Points)
            {
                if (point.YValues[0] > GraphVisualizer.ChartAreas[0].AxisY.Maximum)
                {
                    GraphVisualizer.ChartAreas[0].AxisY.Maximum = point.YValues[0];
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
        /// Displays the previously recorded data
        /// </summary>
        /// <returns></returns>
        protected SpeedDistancePoint DisplayPreviousData()
        {
            SpeedDistancePoint retVal = new SpeedDistancePoint(0, 0);
            List<SpeedDistancePoint> tmp = new List<SpeedDistancePoint>(Function.PreviousData.Points);
            foreach (SpeedDistancePoint point in tmp)
            {
                AddPoint(point);
                retVal.Distance = point.Distance;
                retVal.Speed = point.Speed;
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