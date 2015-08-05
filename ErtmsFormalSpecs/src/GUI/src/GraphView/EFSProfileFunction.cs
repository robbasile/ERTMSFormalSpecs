using DataDictionary.Functions;
using GUIUtils.GraphVisualization.Functions;

namespace GUI.GraphView
{
    public class EfsProfileFunction : ProfileFunction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graph"></param>
        public EfsProfileFunction(IGraph graph)
        {
            Function = graph;
        }

        /// <summary>
        /// Updates the function value according to the provided parameter
        /// </summary>
        /// <param name="parameter"></param>
        protected override void UpdateValue(double parameter)
        {
            // the value is already updated
        }
    }
}
