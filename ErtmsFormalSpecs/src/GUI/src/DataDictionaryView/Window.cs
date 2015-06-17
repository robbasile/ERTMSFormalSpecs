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

namespace GUI.DataDictionaryView
{
    public partial class Window : BaseForm
    {
        public override BaseTreeView TreeView
        {
            get { return dataDictTree; }
        }

        /// <summary>
        ///     The Dictionary handled by this view
        /// </summary>
        private Dictionary dictionary;

        public Dictionary Dictionary
        {
            get { return dictionary; }
            set
            {
                dictionary = value;
                dataDictTree.Root = dictionary;
                Text = dictionary.Name + " model view";
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();
            SpecificInitialization();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public Window(Dictionary dictionary)
        {
            InitializeComponent();
            SpecificInitialization();

            Dictionary = dictionary;

            Refresh();
        }

        /// <summary>
        ///     Perform specific initialization
        /// </summary>
        private void SpecificInitialization()
        {
            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            Visible = false;
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
            if (!GUIUtils.MDIWindow.AfterStep)
            {
                dataDictTree.Refresh();
            }
            base.Refresh();
        }

        /// <summary>
        ///     Refreshes the model of the window
        /// </summary>
        public override void RefreshModel()
        {
            dataDictTree.RefreshModel();
        }

        /// <summary>
        ///     Finds the tree node which corresponds to the model element
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseTreeNode FindNode(IModelElement model)
        {
            return TreeView.FindNode(model);
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