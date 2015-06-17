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

using System.Collections.Generic;
using System.Windows.Forms;
using DataDictionary;
using GUI.Properties;
using Utils;

namespace GUI.LongOperations
{
    public class OpenFileOperation : BaseLongOperation
    {
        /// <summary>
        ///     The name of the file to open
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        ///     The system in which the dictionary should be loaded
        /// </summary>
        private EFSSystem System { get; set; }

        /// <summary>
        ///     The dictionary that has been opened
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        ///     Indicates that errors can occur during load, for instance, for comparison purposes
        /// </summary>
        public bool AllowErrorsDuringLoad
        {
            get { return ErrorsDuringLoad != null; }
        }

        /// <summary>
        ///     Indicates that errors are allowed during load.
        /// </summary>
        public bool AllowErrors
        {
            get { return ErrorsDuringLoad != null; }
        }

        /// <summary>
        ///     The errors encountered during load of the file.
        ///     null indicates that no errors are tolerated
        /// </summary>
        public List<ElementLog> ErrorsDuringLoad { get; private set; }

        /// <summary>
        ///     Indicates that Guid should be updated during load
        /// </summary>
        private bool UpdateGuid { get; set; }

        /// <summary>
        ///     Indicates that files should be locked
        /// </summary>
        public bool PleaseLockFiles { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="fileName">The file path of the file to load</param>
        /// <param name="system">The EFS system for which the load is performed</param>
        /// <param name="allowErrors">Indicates that errors are allowed during load</param>
        /// <param name="updateGuid">Indicates that the GUID should be set during load</param>
        public OpenFileOperation(string fileName, EFSSystem system, bool allowErrors, bool updateGuid)
        {
            FileName = fileName;
            System = system;

            if (allowErrors)
            {
                ErrorsDuringLoad = new List<ElementLog>();
            }
            else
            {
                ErrorsDuringLoad = null;
            }
            UpdateGuid = updateGuid;
            PleaseLockFiles = Util.PleaseLockFiles;
        }

        /// <summary>
        ///     Performs the job as a background task
        /// </summary>
        public override void ExecuteWork()
        {
            Dictionary = Util.load(System, new Util.LoadParams(FileName)
            {
                LockFiles = PleaseLockFiles,
                Errors = ErrorsDuringLoad,
                UpdateGuid = UpdateGuid,
                ConvertObsolete = Settings.Default.ConvertObsoleteVersionOfModelFile
            });
            if (Dictionary != null && System != null && !AllowErrors)
            {
                Dictionary.CheckRules();
            }
        }

        /// <summary>
        ///     Displays errors during load, when the flag AllowErrorDuringLoad is active
        /// </summary>
        public void DisplayErrors()
        {
            if (AllowErrorsDuringLoad)
            {
                if (Dictionary != null)
                {
                    if (ErrorsDuringLoad.Count > 0)
                    {
                        string errors = "";
                        foreach (ElementLog log in ErrorsDuringLoad)
                        {
                            errors += log.Level + ": " + log.Log + "\n";
                        }

                        MessageBox.Show("Errors while opening file " + FileName + "\n\n" + errors,
                            "Errors where encountered while opening file", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Cannot open file " + FileName, "Cannot open file", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}