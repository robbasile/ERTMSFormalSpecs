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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using DataDictionary;
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.MessagesView
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            DockAreas = DockAreas.DockRight;

            messagesDataGridView.DoubleClick += messagesDataGridView_DoubleClick;
        }

        /// <summary>
        ///     Handles a double click event on an element of the messages data grid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void messagesDataGridView_DoubleClick(object sender, EventArgs e)
        {
            MessageEntry selected = null;

            if (messagesDataGridView.SelectedCells.Count == 1)
            {
                List<MessageEntry> messages = (List<MessageEntry>) messagesDataGridView.DataSource;
                selected = messages[messagesDataGridView.SelectedCells[0].OwningRow.Index];
            }

            if (selected != null)
            {
                MessageDetail detail = new MessageDetail();
                detail.SetMessage(selected.Log);
                detail.ShowDialog();
            }
        }

        /// <summary>
        ///     Allows to refresh the view, when the selected model changed
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if refresh should be performed</returns>
        public override bool HandleSelectionChange(Context.SelectionContext context)
        {
            bool retVal = base.HandleSelectionChange(context);

            if (retVal)
            {
                messagesDataGridView.DataSource = null;
                if (DisplayedModel != null)
                {
                    IModelElement current = DisplayedModel;
                    List<MessageEntry> messages = new List<MessageEntry>();
                    while (current != null)
                    {
                        if (current.Messages != null)
                        {
                            foreach (ElementLog log in current.Messages)
                            {
                                messages.Add(new MessageEntry(log));
                            }
                        }

                        if (EFSSystem.INSTANCE.DisplayEnclosingMessages)
                        {
                            current = current.Enclosing as IModelElement;
                        }
                        else
                        {
                            current = null;
                        }
                    }

                    messagesDataGridView.DataSource = messages;

                    // ReSharper disable PossibleNullReferenceException
                    messagesDataGridView.Columns["Level"].FillWeight = 10F;
                    messagesDataGridView.Columns["Message"].FillWeight = 90F;
                    // ReSharper restore PossibleNullReferenceException
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Displays a single message entry
        /// </summary>
        private class MessageEntry
        {
            /// <summary>
            ///     The element that is logged
            /// </summary>
            [Browsable(false)]
            public ElementLog Log { get; private set; }

            /// <summary>
            ///     The message level
            /// </summary>
            // ReSharper disable once UnusedMember.Local
            public ElementLog.LevelEnum Level
            {
                get { return Log.Level; }
            }

            /// <summary>
            ///     The message
            /// </summary>
            // ReSharper disable once UnusedMember.Local
            public String Message
            {
                get { return Log.Log; }
            }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="elementLog"></param>
            public MessageEntry(ElementLog elementLog)
            {
                Log = elementLog;
            }
        }
    }
}