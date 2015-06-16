using DataDictionary;
using Utils;

namespace GUI.LongOperations
{
    /// <summary>
    ///     Closes a dictionary
    /// </summary>
    public class CloseDictionary : BaseLongOperation
    {
        /// <summary>
        ///     The dictionary to close
        /// </summary>
        private Dictionary Dictionary { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public CloseDictionary(Dictionary dictionary)
        {
            Dictionary = dictionary;
        }

        public override void ExecuteWork()
        {
            EFSSystem.INSTANCE.Dictionaries.Remove(Dictionary);
            FinderRepository.INSTANCE.ClearCache();
            EFSSystem.INSTANCE.Compiler.Compile_Synchronous(true);
        }
    }
}