using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using GUIUtils.GraphVisualization;
using GUIUtils.GraphVisualization.Functions;
using GUIUtils.GraphVisualization.Graphs;

namespace GUI.GraphView
{
    public class EfsProfileFunctionGraph : ProfileFunctionGraph
    {
        public EfsProfileFunctionGraph(GraphVisualizer graphVisualizer, ProfileFunction function, string functionName)
            : base(graphVisualizer, function)
        {
            InitializeProperties(SeriesChartType.Line, functionName, functionName);
        }
    }
}
