using System.Windows.Forms.VisualStyles;
using DataDictionary;
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

using Dictionary = DataDictionary.Dictionary;

namespace GUI.LongOperations
{
    public class MergeUpdateOperation: BaseLongOperation
    {
        /// <summary>
        /// The update dictionary
        /// </summary>
        private Dictionary UpdateDictionary { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="updateDictionary"></param>
        public MergeUpdateOperation(Dictionary updateDictionary)
        {
            UpdateDictionary = updateDictionary;
        }

        /// <summary>
        /// Merges the update
        /// </summary>
        public override void ExecuteWork()
        {
            UpdateDictionary.MergeUpdate();
            EFSSystem.INSTANCE.Compiler.Compile_Synchronous(true);
        }
    }
}
