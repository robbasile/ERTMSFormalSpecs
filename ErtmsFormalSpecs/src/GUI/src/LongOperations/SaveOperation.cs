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

namespace GUI.LongOperations
{
    /// <summary>
    ///     A save file configuration
    /// </summary>
    public class SaveOperation : BaseLongOperation
    {
        /// <summary>
        ///     The form that invoked this progress handler
        /// </summary>
        private MainWindow MainWindow { get; set; }

        /// <summary>
        ///     The dictionary to save
        /// </summary>
        private Dictionary Dictionary { get; set; }

        /// <summary>
        ///     The system to save
        /// </summary>
        private EFSSystem System { get; set; }

        /// <summary>
        ///     Constructor used to save a single dictionary
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="dictionary"></param>
        public SaveOperation(MainWindow mainWindow, Dictionary dictionary)
        {
            MainWindow = mainWindow;
            Dictionary = dictionary;
            System = Dictionary.EFSSystem;
        }

        /// <summary>
        ///     Constructor used to save to complete system
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="system"></param>
        public SaveOperation(MainWindow mainWindow, EFSSystem system)
        {
            MainWindow = mainWindow;
            System = system;
        }

        /// <summary>
        ///     Performs the job as a background task
        /// </summary>
        /// <param name="arg"></param>
        public override void ExecuteWork()
        {
            Util.UnlockAllFiles();

            try
            {
                if (Dictionary != null)
                {
                    Dictionary.save();
                }
                else
                {
                    // Save all dictionaries
                    foreach (Dictionary dictionary in System.Dictionaries)
                    {
                        dictionary.save();
                    }
                }
            }
            finally
            {
                Util.LockAllFiles();
                System.ShouldSave = false;
                MainWindow.Invoke((MethodInvoker) delegate { MainWindow.UpdateTitle(); });
            }
        }
    }
}