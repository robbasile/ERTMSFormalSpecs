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
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Specification;
using GUI.Properties;
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.SpecificationView
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     The tree view used to display the requirements
        /// </summary>
        public override BaseTreeView TreeView
        {
            get { return specBrowserTreeView; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();
            DockAreas = DockAreas.DockLeft;

            specBrowserTreeView.Root = EfsSystem.Instance;
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
                RefreshModel();
            }

            return retVal;
        }

        /// <summary>
        ///     Refreshes the displayed model
        /// </summary>
        private void RefreshModel()
        {
            specBrowserRuleView.Nodes.Clear();
            Paragraph paragraph = EnclosingFinder<Paragraph>.find(DisplayedModel, true);
            if ( paragraph == null )
            {
                ReqRef reqRef = DisplayedModel as ReqRef;
                if ( reqRef != null )
                {
                    paragraph = reqRef.Paragraph;
                }
            }
            if (paragraph != null)
            {
                foreach (ReqRef reqRef in paragraph.Implementations)
                {
                    specBrowserRuleView.Nodes.Add(new ReqRefTreeNode(reqRef, true, false, reqRef.Model.Name));
                }

                functionalBlocksTreeView.SetRoot(paragraph);
            }
            specBrowserTreeView.RefreshModel(null);
        }

        /// <summary>
        ///     Allows to refresh the view, when the value of a model changed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns>True if the view should be refreshed</returns>
        public override bool HandleValueChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            bool retVal = base.HandleValueChange(modelElement, changeKind);

            if (retVal)
            {
                RefreshModel();
            }

            return retVal;
        }

        /// <summary>
        ///     Selects the next node where error message is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextErrortoolStripButton_Click(object sender, EventArgs e)
        {
            TreeView.SelectNext(ElementLog.LevelEnum.Error);
        }

        /// <summary>
        ///     Selects the next node where warning message is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextWarningToolStripButton_Click(object sender, EventArgs e)
        {
            TreeView.SelectNext(ElementLog.LevelEnum.Warning);
        }

        /// <summary>
        ///     Selects the next node where info message is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextInfoToolStripButton_Click(object sender, EventArgs e)
        {
            TreeView.SelectNext(ElementLog.LevelEnum.Info);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (!EfsSystem.Instance.Markings.SelectPreviousMarking())
            {
                MessageBox.Show(
                    Resources.Window_toolStripButton1_Click_No_more_marking_to_show,
                    Resources.Window_toolStripButton1_Click_No_more_markings,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (!EfsSystem.Instance.Markings.SelectNextMarking())
            {
                MessageBox.Show(
                    Resources.Window_toolStripButton1_Click_No_more_marking_to_show,
                    Resources.Window_toolStripButton1_Click_No_more_markings,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }
}