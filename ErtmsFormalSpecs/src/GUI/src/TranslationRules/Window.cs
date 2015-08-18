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
using DataDictionary.Tests.Translations;
using Utils;

namespace GUI.TranslationRules
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     The treeview used to display the several translations
        /// </summary>
        public override BaseTreeView TreeView
        {
            get { return translationTreeView; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public Window(TranslationDictionary dictionary)
        {
            InitializeComponent();

            translationTreeView.Root = dictionary;
            testBrowserStatusLabel.Text = translationTreeView.Root.TranslationsCount + " translation rule(s) loaded";
            translationTreeView.AfterSelect += translationTreeView_AfterSelect;
            Text = dictionary.Dictionary.Name + " test translation view";
        }

        /// <summary>
        ///     The translation dictionary displayed in this window
        /// </summary>
        public TranslationDictionary TranslationDictionary
        {
            get { return translationTreeView.Root; }
        }

        /// <summary>
        ///     Clears messages for the element stored in the tree view in the window
        /// </summary>
        public void Clear()
        {
            translationTreeView.ClearMessages();
            GuiUtils.MdiWindow.Refresh();
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

        /// <summary>
        ///     Handles the selection of a translation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void translationTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            staticTimeLineControl.Translation = DisplayedModel as Translation;
        }
    }
}