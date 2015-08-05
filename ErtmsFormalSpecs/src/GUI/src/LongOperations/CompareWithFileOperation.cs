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
using DataDictionary.Compare;

namespace GUI.LongOperations
{
    public class CompareWithFileOperation : BaseLongOperation
    {
        /// <summary>
        ///     The dictionary to compare with the file
        /// </summary>
        private Dictionary Dictionary { get; set; }

        /// <summary>
        ///     The path to the file to compare with
        /// </summary>
        private string OtherFilePath { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary">The dictionary to compare with</param>
        /// <param name="filePath">The path to the file to compare with</param>
        public CompareWithFileOperation(Dictionary dictionary, string filePath)
        {
            Dictionary = dictionary;
            OtherFilePath = filePath;
        }

        /// <summary>
        ///     Generates the file in the background thread
        /// </summary>
        /// <param name="arg"></param>
        public override void ExecuteWork()
        {
            // Open the dictionary but do not store it in the EFS System
            bool allowErrors = true;
            OpenFileOperation openFileOperation = new OpenFileOperation(OtherFilePath, null, allowErrors, false);
            openFileOperation.ExecuteWork();

            // Compare the files
            if (openFileOperation.Dictionary != null)
            {
                VersionDiff versionDiff = new VersionDiff();
                Comparer.ensureGuidDictionary(Dictionary, openFileOperation.Dictionary);
                Comparer.compareDictionary(Dictionary, openFileOperation.Dictionary, versionDiff);
                versionDiff.MarkVersionChanges(Dictionary);
            }
            else
            {
                MessageBox.Show("Cannot open file, please see log file (GUI.Log) for more information",
                    "Cannot open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}