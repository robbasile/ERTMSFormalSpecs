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

namespace GUI.Status
{
    public class StatusHandler
    {
        /// <summary>
        ///     The class that is used to update the status
        /// </summary>
        private class StatusSynchronizer : GenericSynchronizationHandler<MainWindow>
        {
            /// <summary>
            ///     The model element used to update the status bar
            /// </summary>
            private ModelElement Model { get; set; }

            /// <summary>
            ///     Indicates that the model has changed
            /// </summary>
            private bool ModelChanged { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="window"></param>
            public StatusSynchronizer(MainWindow window)
                : base(window, 100)
            {
                Model = null;
                ModelChanged = false;
            }

            /// <summary>
            ///     Sets the instance to display
            /// </summary>
            /// <param name="model"></param>
            public void SetModel(ModelElement model)
            {
                Model = model;
                ModelChanged = true;
            }

            /// <summary>
            ///     Synchronization
            /// </summary>
            /// <param name="instance"></param>
            public override void HandleSynchronization(MainWindow instance)
            {
                if (ModelChanged)
                {
                    ModelChanged = false;

                    // Build the status message in a background thread, because it can take a long time
                    Instance.BeginInvoke((MethodInvoker) (() => Instance.SetStatus("Updating status...")));
                    string status = Model.CreateStatusMessage();
                    Instance.BeginInvoke((MethodInvoker) (() => Instance.SetStatus(status)));
                }
            }
        }

        /// <summary>
        ///     Indicates that synchronization is required
        /// </summary>
        private StatusSynchronizer StatusSynchronizerTask { get; set; }

        /// <summary>
        ///     Handles the displayed status
        /// </summary>
        public StatusHandler()
        {
            StatusSynchronizerTask = new StatusSynchronizer(GuiUtils.MdiWindow);
            EFSSystem.INSTANCE.Context.SelectionChange += HandleSelectionChange;
        }

        /// <summary>
        ///     Updates the status when the selection changes
        /// </summary>
        /// <param name="context"></param>
        private void HandleSelectionChange(Context.SelectionContext context)
        {
            ModelElement element = context.Element as ModelElement;
            if (element != null)
            {
                StatusSynchronizerTask.SetModel(element);
            }
        }
    }
}