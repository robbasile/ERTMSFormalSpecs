using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DataDictionary;
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.MessagesView
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     The element for which this message window is built
        /// </summary>
        private IModelElement Model { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            DockAreas = DockAreas.DockRight;

            messagesDataGridView.DoubleClick += new EventHandler(messagesDataGridView_DoubleClick);
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
        ///     Handles the close event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            GUIUtils.MDIWindow.HandleSubWindowClosed(this);
        }


        /// <summary>
        ///     Sets the model element for which messages should be displayed
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(IModelElement model)
        {
            Model = model;
            RefreshModel();
        }

        /// <summary>
        ///     Refreshes the displayed messages according to the window model
        /// </summary>
        public override void RefreshModel()
        {
            List<MessageEntry> messages = new List<MessageEntry>();

            IModelElement current = Model;
            while (current != null)
            {
                if (current.Messages != null)
                {
                    foreach (ElementLog log in Model.Messages)
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

            messagesDataGridView.DataSource = null;
            messagesDataGridView.DataSource = messages;

            messagesDataGridView.Columns["Level"].FillWeight = 10F;
            messagesDataGridView.Columns["Message"].FillWeight = 90F;
            Refresh();
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
            public ElementLog.LevelEnum Level
            {
                get { return Log.Level; }
            }

            /// <summary>
            ///     The message
            /// </summary>
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