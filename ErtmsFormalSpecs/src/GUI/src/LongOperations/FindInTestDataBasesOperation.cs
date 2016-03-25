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
using System.IO;
using DataDictionary.Generated;
using GUIUtils;
using GUIUtils.LongOperations;
using Importers;
using Dictionary = DataDictionary.Dictionary;
using Frame = DataDictionary.Tests.Frame;

namespace GUI.LongOperations
{
    /// <summary>
    /// Finds a specific packet in a set of test databases located in a folder
    /// </summary>
    public class FindInTestDataBasesOperation : BaseLongOperation
    {
        /// <summary>
        ///     The password requireed to access the database
        /// </summary>
        private const string DbPassword = "papagayo";

        /// <summary>
        ///     The name of the folder containing the tests
        /// </summary>
        private string FolderName { get; set; }

        /// <summary>
        ///     The number of the packet to look for
        /// </summary>
        private int PacketNumber { get; set; }

        /// <summary>
        ///     The list of selected filenames
        /// </summary>
        public List<string> FileNames { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="packetNumber"></param>
        public FindInTestDataBasesOperation(string folderName, int packetNumber)
        {
            FolderName = folderName;
            PacketNumber = packetNumber;
            FileNames = new List<string>();
        }

        /// <summary>
        ///     Find the test sequences containing the given packet.
        /// </summary>
        public override void ExecuteWork()
        {
            foreach (string fName in Directory.GetFiles(FolderName, "*.mdb"))
            {
                Dialog.UpdateMessage(FileNames.Count + " test found");
                TestImporter importer = new TestImporter(fName, DbPassword);
                if (importer.FindSpecificPacket(PacketNumber))
                {
                    FileNames.Add(fName);
                }
            }
        }
    }
}