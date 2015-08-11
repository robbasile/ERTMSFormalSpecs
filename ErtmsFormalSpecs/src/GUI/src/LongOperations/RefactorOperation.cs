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

namespace GUI.LongOperations
{
    public class RefactorOperation : BaseLongOperation
    {
        /// <summary>
        ///     The element to be refactored
        /// </summary>
        private ModelElement Model { get; set; }

        /// <summary>
        ///     The new element name
        /// </summary>
        private string NewName { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="newName"></param>
        public RefactorOperation(ModelElement model, string newName)
        {
            Model = model;
            NewName = newName;
        }

        /// <summary>
        ///     Generates the file in the background thread
        /// </summary>
        public override void ExecuteWork()
        {
            EFSSystem.INSTANCE.Compiler.Refactor(Model, NewName);
        }

        /// <summary>
        ///     Executes the operation in background using a progress handler
        /// </summary>
        /// <param name="message">The message to display on the dialog window</param>
        /// <param name="allowCancel">Indicates that the opeation can be canceled</param>
        public override void ExecuteUsingProgressDialog(string message, bool allowCancel = true)
        {
            base.ExecuteUsingProgressDialog(message, allowCancel);

            // Long operations do not notify the listeners. 
            // Update the entire model
            EFSSystem.INSTANCE.Context.HandleChangeEvent(null);
        }
    }
}