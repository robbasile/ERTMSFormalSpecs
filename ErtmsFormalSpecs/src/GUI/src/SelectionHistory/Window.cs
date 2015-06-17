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
using System.Windows.Forms;
using Utils;
using WeifenLuo.WinFormsUI.Docking;
using ModelElement = DataDictionary.ModelElement;

namespace GUI.SelectionHistory
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            historyDataGridView.DoubleClick += new EventHandler(historyDataGridView_DoubleClick);

            Visible = false;
            Text = "Selection history view";

            DockAreas = DockAreas.DockRight;
            Refresh();
        }

        private void historyDataGridView_DoubleClick(object sender, EventArgs e)
        {
            ModelElement selected = null;

            if (historyDataGridView.SelectedCells.Count == 1)
            {
                selected =
                    ((List<HistoryObject>) historyDataGridView.DataSource)[
                        historyDataGridView.SelectedCells[0].OwningRow.Index].Reference;
            }

            if (selected != null)
            {
                int i = GUIUtils.MDIWindow.SelectionHistory.IndexOf(selected);
                while (i > 0)
                {
                    GUIUtils.MDIWindow.SelectionHistory.RemoveAt(0);
                    i -= 1;
                }

                GUIUtils.MDIWindow.Select(selected, true);
                RefreshModel();
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
        ///     Refreshed the model of the window
        /// </summary>
        public override void RefreshModel()
        {
            if (GUIUtils.MDIWindow != null)
            {
                List<HistoryObject> history = new List<HistoryObject>();

                foreach (IModelElement element in GUIUtils.MDIWindow.SelectionHistory)
                {
                    ModelElement modelElement = element as ModelElement;
                    if (modelElement != null)
                    {
                        history.Add(new HistoryObject(modelElement));
                    }
                }

                historyDataGridView.DataSource = history;
            }

            Refresh();
        }

        /// <summary>
        ///     Provides the model element currently selected in this IBaseForm
        /// </summary>
        public override IModelElement Selected
        {
            get
            {
                IModelElement retVal = null;

                if (TreeView != null && TreeView.Selected != null)
                {
                    retVal = TreeView.Selected.Model;
                }

                return retVal;
            }
        }

        private class HistoryObject
        {
            /// <summary>
            ///     The object that is referenced for history
            /// </summary>
            [Browsable(false)]
            public ModelElement Reference { get; private set; }

            /// <summary>
            ///     The identification of the history element
            /// </summary>
            public string Model
            {
                get { return Reference.Name; }
            }

            /// <summary>
            ///     The type of the referenced object
            /// </summary>
            public string Type
            {
                get { return Reference.GetType().Name; }
            }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="reference"></param>
            public HistoryObject(ModelElement reference)
            {
                Reference = reference;
            }
        }
    }
}