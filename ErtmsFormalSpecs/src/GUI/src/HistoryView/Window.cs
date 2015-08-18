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
using DataDictionary.Shortcuts;
using HistoricalData;
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.HistoryView
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     The property grid used to display information about the selected history information
        /// </summary>
        public MyPropertyGrid Properties
        {
            get { return propertyGrid; }
        }

        /// <summary>
        ///     The tree view which displays all history information
        /// </summary>
        public override BaseTreeView TreeView
        {
            get { return historyTreeView; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();
            DockAreas = DockAreas.DockRight;

            historyTreeView.AfterSelect += historyTreeView_AfterSelect;
            ResizeDescriptionArea(propertyGrid, 20);
        }

        /// <summary>
        ///     Indicates that the model element should be displayed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <returns></returns>
        protected override bool ShouldDisplay(IModelElement modelElement)
        {
            bool retVal = base.ShouldDisplay(modelElement);

            if (retVal)
            {
                // Don't handle shortcuts in history
                retVal = EnclosingFinder<ShortcutDictionary>.find(modelElement, true) == null;
            }

            return retVal;
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
                INamable namable = DisplayedModel;
                if (namable != null)
                {
                    Text = namable.Name + " history";
                }
                else
                {
                    Text = "History";
                }

                historyTreeView.Root = DisplayedModel;
                if (historyTreeView.Nodes.Count > 0)
                {
                    historyTreeView.SelectedNode = historyTreeView.Nodes[0] as ChangeTreeNode;
                }
                else
                {
                    historyTreeView.SelectedNode = null;
                    Properties.SelectedObject = null;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Updates the window according to the new selected change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void historyTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Properties.SelectedObject = null;

            ChangeTreeNode changeTreeNode = e.Node as ChangeTreeNode;
            if (changeTreeNode != null)
            {
                Change item = changeTreeNode.Item;
                if (item != null)
                {
                    beforeRichTextBox.Text = item.getBefore();
                    afterRichTextBox.Text = item.getAfter();
                    Properties.SelectedObject = item;
                }
                else
                {
                    beforeRichTextBox.Text = "";
                    afterRichTextBox.Text = "";
                }
            }
        }
    }
}