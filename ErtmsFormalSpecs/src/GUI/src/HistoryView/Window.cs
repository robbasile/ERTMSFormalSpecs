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
using HistoricalData;
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.HistoryView
{
    public partial class Window : BaseForm
    {
        public override MyPropertyGrid Properties
        {
            get { return propertyGrid; }
        }

        public RichTextBox ExpressionTextBox
        {
            get { return null; }
        }

        public override BaseTreeView TreeView
        {
            get { return historyTreeView; }
        }

        /// <summary>
        ///     The rule set which is used to check the specifications
        /// </summary>
        private IModelElement model;

        public IModelElement Model
        {
            get { return model; }
            set
            {
                model = value;
                historyTreeView.Root = model;

                INamable namable = model as INamable;
                if (namable != null)
                {
                    Text = namable.Name + " history";
                }
                else
                {
                    Text = "history";
                }

                if (historyTreeView.Nodes.Count > 0)
                {
                    BaseTreeNode node = historyTreeView.Nodes[0] as BaseTreeNode;
                    historyTreeView.Select(node.Model);
                    node.SelectionChanged(true);
                }
                else
                {
                    SelectionChanged(null);
                    Properties.SelectedObject = null;
                }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="specification"></param>
        public Window()
        {
            InitializeComponent();

            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            Visible = false;

            ResizeDescriptionArea(propertyGrid, 20);

            Refresh();

            DockAreas = DockAreas.DockRight;
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

        public override void Refresh()
        {
            historyTreeView.Refresh();
            base.Refresh();
        }

        /// <summary>
        ///     Refreshes the model of the window
        /// </summary>
        public override void RefreshModel()
        {
            historyTreeView.RefreshModel();
        }

        /// <summary>
        ///     Updates the window according to the new selected change
        /// </summary>
        /// <param name="Item"></param>
        public void SelectionChanged(Change Item)
        {
            if (Item != null)
            {
                beforeRichTextBox.Text = Item.getBefore();
                afterRichTextBox.Text = Item.getAfter();
            }
            else
            {
                beforeRichTextBox.Text = "";
                afterRichTextBox.Text = "";
            }
        }
    }
}