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
using DataDictionary.Generated;
using Importers;
using Dictionary = DataDictionary.Dictionary;
using Frame = DataDictionary.Tests.Frame;

namespace GUI.LongOperations
{
    public class ImportTestDataBaseOperation : BaseLongOperation
    {
        /// <summary>
        ///     The name of the frame for the subset 76
        /// </summary>
        private const string Subset076 = "Subset-076";

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
        ///     Constructor
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dictionary"></param>
        /// <param name="mode"></param>
        public ImportTestDataBaseOperation(string fileName, Dictionary dictionary, Mode mode)
        {
            FileName = fileName;
            _dictionary = dictionary;
            ImportMode = mode;
        }

        /// <summary>
        ///     Imports the database
        /// </summary>
        public override void ExecuteWork()
        {
            Frame frame = _dictionary.FindFrame(Subset076);
            if (frame == null)
            {
                frame = (Frame) acceptor.getFactory().createFrame();
                frame.Name = Subset076;
                _dictionary.appendTests(frame);
            }

            if (ImportMode == Mode.File)
            {
                TestImporter importer = new TestImporter(FileName, DbPassword);
                importer.Import(frame);
            }
            else
            {
                foreach (string fName in Directory.GetFiles(FileName, "*.mdb"))
                {
                    TestImporter importer = new TestImporter(fName, DbPassword);
                    importer.Import(frame);
                }
            }
        }
    }
}