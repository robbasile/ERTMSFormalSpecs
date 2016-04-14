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

using System.IO;
using System.Linq;
using GUIUtils.LongOperations;
using Importers;
using Dictionary = DataDictionary.Dictionary;
using Frame = DataDictionary.Tests.Frame;

namespace GUI.LongOperations
{
    public class ImportTestDataBaseOperation : BaseLongOperation
    {
        /// <summary>
        ///     The password requireed to access the database
        /// </summary>
        private const string DbPassword = "papagayo";

        /// <summary>
        ///     The dictionary in which the database should be imported
        /// </summary>
        private readonly Dictionary _dictionary;

        /// <summary>
        ///     The name of the database to import
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        ///     Should we import a file, or a directory containing a set of files?
        /// </summary>
        public enum Mode
        {
            File,
            Directory
        };

        /// <summary>
        ///     The import mode
        /// </summary>
        private Mode ImportMode { get; set; }

        /// <summary>
        /// Indicates that manual translations should be kept while importing the file(s)
        /// </summary>
        private bool KeepManualTranslations { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dictionary"></param>
        /// <param name="mode"></param>
        /// <param name="keepManualTranslations">Indicates that manual translation for be kept during import</param>
        public ImportTestDataBaseOperation(string fileName, Dictionary dictionary, Mode mode, bool keepManualTranslations)
        {
            FileName = fileName;
            _dictionary = dictionary;
            ImportMode = mode;
            KeepManualTranslations = keepManualTranslations;
        }

        /// <summary>
        ///     Imports the database
        /// </summary>
        public override void ExecuteWork()
        {

            if (ImportMode == Mode.File)
            {
                Frame frame = GetFrame(FileName);
                if (frame != null)
                {
                    TestImporter importer = new TestImporter(FileName, DbPassword);
                    importer.Import(frame, KeepManualTranslations);
                }
            }
            else
            {
                foreach (string fName in Directory.GetFiles(FileName, "*.mdb"))
                {
                    Frame frame = GetFrame(fName);
                    if (frame != null)
                    {
                        TestImporter importer = new TestImporter(fName, DbPassword);
                        importer.Import(frame, KeepManualTranslations);
                    }
                }
            }

            RefreshModel.Execute();
        }

        /// <summary>
        /// Provides the frame according to the sequence file name
        /// </summary>
        /// <returns></returns>
        private Frame GetFrame(string fileName)
        {
            Frame frame = null;
            string[] items = fileName.Split('_');

            if (items.Count() > 1)
            {
                string frameName = "Frame " + items[1];
                frame = _dictionary.FindFrame(frameName);
                if (frame == null)
                {
                    frame = Frame.CreateDefault(_dictionary.Tests);
                    // Do not use the default subsequence
                    frame.allSubSequences().Clear();
                    frame.Name = frameName;
                    _dictionary.appendTests(frame);
                }
            }

            return frame;
        }
    }
}