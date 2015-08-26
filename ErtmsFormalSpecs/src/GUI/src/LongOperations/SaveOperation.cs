// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System.Windows.Forms;
using DataDictionary;
using GUIUtils.LongOperations;

namespace GUI.LongOperations
{
    /// <summary>
    ///     A save file configuration
    /// </summary>
    public class SaveOperation : BaseLongOperation
    {
        /// <summary>
        ///     The dictionary to save
        /// </summary>
        private Dictionary Dictionary { get; set; }

        /// <summary>
        ///     Constructor used to save a single dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        public SaveOperation(Dictionary dictionary)
        {
            Dictionary = dictionary;
        }

        /// <summary>
        ///     Constructor used to save to complete system
        /// </summary>
        public SaveOperation()
        {
        }

        /// <summary>
        ///     Performs the job as a background task
        /// </summary>
        public override void ExecuteWork()
        {
            Util.UnlockAllFiles();

            try
            {
                if (Dictionary != null)
                {
                    Dictionary.Save();
                }
                else
                {
                    // Save all dictionaries
                    foreach (Dictionary dictionary in EFSSystem.INSTANCE.Dictionaries)
                    {
                        dictionary.Save();
                    }
                }
            }
            finally
            {
                Util.LockAllFiles();
                EFSSystem.INSTANCE.ShouldSave = false;
                GuiUtils.MdiWindow.Invoke((MethodInvoker) (() => GuiUtils.MdiWindow.UpdateTitle()));
            }
        }
    }
}