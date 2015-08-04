using System.Collections.Generic;
using DataDictionary.Functions;

namespace GUIUtils.GraphVisualization.Functions
{
    public abstract class ProfileSetFunction : ProfileFunction
    {
        /// <summary>
        /// The set of function
        /// </summary>
        public List<IGraph> Functions;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProfileSetFunction()
        {
            Functions = new List<IGraph>();
        }

        /// <summary>
        /// Erases the data of the function
        /// </summary>
        public override void ClearData()
        {
            base.ClearData();
            Functions.Clear();
        }

        /// <summary>
        /// Relocates the function according to the LRBG position
        /// </summary>
        /// <param name="lrbgPosition"></param>
        protected override void Relocate(double lrbgPosition)
        {
            base.Relocate(lrbgPosition);
            foreach (IGraph function in Functions)
            {
                UpdateFunction(function, lrbgPosition);
            }
        }
    }
}
