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
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.SpecificationView
{
    public partial class Window : BaseForm
    {
        public override BaseTreeView TreeView
        {
            get { return specBrowserTreeView; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="specification"></param>
        public Window()
        {
            InitializeComponent();
            SpecificInitialisation();

            specBrowserTreeView.Root = EFSSystem.INSTANCE;
        }

        /// <summary>
        ///     Perform speicific initializations
        /// </summary>
        private void SpecificInitialisation()
        {
            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            Visible = false;
            DockAreas = DockAreas.DockLeft;
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
            specBrowserTreeView.Refresh();
            base.Refresh();
        }

        /// <summary>
        ///     Refreshes the model of the window
        /// </summary>
        public override void RefreshModel()
        {
            specBrowserTreeView.RefreshModel();
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
            if (!EFSSystem.INSTANCE.Markings.selectPreviousMarking())
            {
                MessageBox.Show("No more marking to show", "No more markings", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (!EFSSystem.INSTANCE.Markings.selectNextMarking())
            {
                MessageBox.Show("No more marking to show", "No more markings", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }
}