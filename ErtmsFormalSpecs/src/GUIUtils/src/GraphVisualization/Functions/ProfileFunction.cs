using DataDictionary.Functions;

namespace GUIUtils.GraphVisualization.Functions
{
    public abstract class ProfileFunction : Function
    {
        /// <summary>
        /// The function
        /// </summary>
        public IGraph Function { get; set; }

        /// <summary>
        /// Contructor
        /// </summary>
        public ProfileFunction()
        {
        }

        /// <summary>
        /// Computes the function value according to the provided parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected override SpeedDistancePoint GetValue(double parameter)
        {
            SpeedDistancePoint retVal = null;

            if (Function != null)
            {
                retVal = new SpeedDistancePoint(parameter, Function.Evaluate(parameter) * 3.6);
            }

            return retVal;
        }

        /// <summary>
        /// Relocates the function according to the LRBG position
        /// </summary>
        /// <param name="lrbgPosition"></param>
        protected override void Relocate(double lrbgPosition)
        {
            if (Function != null)
            {
                UpdateFunction(Function, lrbgPosition);
            }
        }

        /// <summary>
        /// Updates the provided function and relocates it according to the value of increment
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="lrbgPosition"></param>
        protected void UpdateFunction(IGraph graph, double lrbgPosition)
        {
            if (graph != null)
            {
                for (int i = 0; i < graph.CountSegments(); i++)
                {
                    graph.GetSegment(i).D0 += lrbgPosition;
                }
            }
        }

        /// <summary>
        /// Records the current value
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="relocatedPosition"></param>
        public void RecordCurrentValue(double currentPosition, double relocatedPosition)
        {
            PreviousData.Add(new SpeedDistancePoint(currentPosition, GetValue(relocatedPosition).Speed));
        }

        /// <summary>
        /// Clears the data related to that function
        /// </summary>
        public override void ClearData()
        {
            base.ClearData();
            Function = null;
        }
    }
}
