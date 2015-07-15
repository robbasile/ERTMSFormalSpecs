using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataDictionary;
using DataDictionary.Generated;
using GUI.DictionarySelector;
using GUI.LongOperations;
using Dictionary = DataDictionary.Dictionary;

namespace GUI.src.LongOperations
{
    public class MergeUpdateOperation: BaseLongOperation
    {
        /// <summary>
        ///     The EFS system
        /// </summary>
        private EFSSystem efsSystem;

        /// <summary>
        ///     Constructor
        /// </summary>
        public MergeUpdateOperation(EFSSystem System)
        {
            efsSystem = System;
        }

        public override void ExecuteWork()
        {
            Dictionary UpdateDictionary = GetDictionary();

            if (UpdateDictionary != null)
            {
                UpdateDictionary.MergeUpdate();
            }
        }

        /// <summary>
        ///     Opens a dictionary selector window for the user
        /// </summary>
        /// <returns></returns>
        private Dictionary GetDictionary()
        {
            Dictionary retVal = null;

            MainWindow mainWindow = GUIUtils.MDIWindow;
            if (efsSystem != null)
            {
                DictionarySelector.DictionarySelector dictionarySelector =
                            new DictionarySelector.DictionarySelector(efsSystem, FilterOptions.Updates);
                dictionarySelector.ShowDictionaries(mainWindow);

                if (dictionarySelector.Selected != null)
                {
                    retVal = dictionarySelector.Selected;
                }
            }

            return retVal;
        }
    }
}
