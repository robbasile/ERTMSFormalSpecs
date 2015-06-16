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
        public override MyPropertyGrid Properties
        {
            get { return null; }
        }

        public override EditorTextBox RequirementsTextBox
        {
            get { return null; }
        }

        public override EditorTextBox ExpressionEditorTextBox
        {
            get { return null; }
        }

        public override ExplainTextBox ExplainTextBox
        {
            get { return null; }
        }

        public override BaseTreeView TreeView
        {
            get { return translationTreeView; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public Window()
        {
            InitializeComponent();
            FormClosed += new FormClosedEventHandler(Window_FormClosed);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public Window(TranslationDictionary dictionary)
        {
            InitializeComponent();

            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            Visible = false;
            translationTreeView.Root = dictionary;
            Text = dictionary.Dictionary.Name + " test translation view";

            Refresh();
        }

        /// <summary>
        ///     The translation dictionary displayed in this window
        /// </summary>
        public TranslationDictionary TranslationDictionary
        {
            get { return translationTreeView.Root as TranslationDictionary; }
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
        ///     Refreshes the display
        /// </summary>
        public override void Refresh()
        {
            translationTreeView.Refresh();
            staticTimeLineControl.Refresh();

            testBrowserStatusLabel.Text = translationTreeView.Root.TranslationsCount + " translation rule(s) loaded";
            base.Refresh();
        }

        /// <summary>
        ///     Clears messages for the element stored in the tree view in the window
        /// </summary>
        public void Clear()
        {
            translationTreeView.ClearMessages();
            GUIUtils.MDIWindow.Refresh();
        }

        /// <summary>
        ///     Refreshed the model of the window
        /// </summary>
        public override void RefreshModel()
        {
            translationTreeView.RefreshModel();
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
        ///     Selects the current translation
        /// </summary>
        /// <param name="translation"></param>
        public void SetSelection(Translation translation)
        {
            staticTimeLineControl.Translation = translation;
            staticTimeLineControl.Refresh();
        }
    }
}