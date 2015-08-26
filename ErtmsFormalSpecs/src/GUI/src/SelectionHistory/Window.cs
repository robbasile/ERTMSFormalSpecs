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
using DataDictionary;
using GUI.Properties;
using WeifenLuo.WinFormsUI.Docking;

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

            historyDataGridView.DoubleClick += historyDataGridView_DoubleClick;
            Text = Resources.Window_Window_Selection_history_view;

            DockAreas = DockAreas.DockRight;
        }

        /// <summary>
        ///     Selects an historical data element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void historyDataGridView_DoubleClick(object sender, EventArgs e)
        {
            ModelElement selected = null;

            if (historyDataGridView.SelectedCells.Count == 1)
            {
                int i = historyDataGridView.SelectedCells[0].OwningRow.Index;
                List<HistoryObject> historyObjects = (List<HistoryObject>) historyDataGridView.DataSource;
                selected = historyObjects[i].Reference;
            }

            if (selected != null)
            {
                MouseEventArgs mouseEventArgs = e as MouseEventArgs;
                if (mouseEventArgs != null)
                {
                    EfsSystem.Instance.Context.ClearHistoryUntilElement(selected);
                    Context.SelectionCriteria criteria = GuiUtils.SelectionCriteriaBasedOnMouseEvent(mouseEventArgs);
                    EfsSystem.Instance.Context.SelectElement(selected, this, criteria);
                }
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
                if (GuiUtils.MdiWindow != null)
                {
                    List<HistoryObject> history = new List<HistoryObject>();
                    foreach (Context.SelectionContext selectionContext in EfsSystem.Instance.Context.SelectionHistory)
                    {
                        ModelElement historyElement = selectionContext.Element as ModelElement;
                        if (historyElement != null)
                        {
                            history.Add(new HistoryObject(historyElement));
                        }
                    }

                    historyDataGridView.DataSource = history;
                }
            }

            return retVal;
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
            // ReSharper disable once UnusedMember.Local
            public string Model
            {
                get { return Reference.Name; }
            }

            /// <summary>
            ///     The type of the referenced object
            /// </summary>
            // ReSharper disable once UnusedMember.Local
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