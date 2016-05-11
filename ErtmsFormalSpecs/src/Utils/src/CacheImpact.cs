using System.Collections.Generic;

namespace Utils
{
    /// <summary>
    /// Handles the impact of a set of changes on the caches 
    /// </summary>
    public class CacheImpact
    {
        /// <summary>
        /// The elements that are impacted and for which the cache should be cleaned
        /// </summary>
        private HashSet<IModelElement> Impact { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CacheImpact()
        {            
            Impact = new HashSet<IModelElement>();
        }

        /// <summary>
        /// Adds a new elements as impacted by the change
        /// </summary>
        /// <param name="modelElement"></param>
        public void Add(IModelElement modelElement)
        {
            Impact.Add(modelElement);
        }

        /// <summary>
        /// Clears the caches of the impacted model elements
        /// </summary>
        public void ClearCaches()
        {
            ISubDeclaratorUtils.CriticalSection.WaitOne();
            try
            {
                foreach (IModelElement modelElement in Impact)
                {
                    modelElement.ClearCache();
                }
            }
            finally
            {
                ISubDeclaratorUtils.CriticalSection.ReleaseMutex();
            }
            Impact.Clear();
        }
    }
}
