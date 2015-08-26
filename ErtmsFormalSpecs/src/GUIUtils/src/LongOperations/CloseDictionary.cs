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

using DataDictionary;
using Utils;

namespace GUIUtils.LongOperations
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

        /// <summary>
        ///     Closes the dictionary
        /// </summary>
        public override void ExecuteWork()
        {
            EFSSystem.INSTANCE.Dictionaries.Remove(Dictionary);
            FinderRepository.INSTANCE.ClearCache();
            EFSSystem.INSTANCE.Compiler.Compile_Synchronous(true);
        }
    }
}