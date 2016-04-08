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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using DataDictionary;
using DataDictionary.Generated;
using GUI.DictionarySelector;
using GUI.EditorView;
using GUI.LongOperations;
using GUI.Report;
using GUI.RequirementSetDiagram;
using GUI.RulePerformances;
using GUI.src.LongMessageView;
using GUI.Status;
using GUIUtils;
using GUIUtils.LongOperations;
using Importers.ExcelImporter;
using Microsoft.Win32;
using Utils;
using WeifenLuo.WinFormsUI.Docking;
using Dictionary = DataDictionary.Dictionary;
using Specification = DataDictionary.Specification.Specification;
using SubSequence = DataDictionary.Tests.SubSequence;
using Util = DataDictionary.Util;
using Window = GUI.MessagesView.Window;

namespace GUI
{
    public partial class MainWindow : Form
    {
        /// <summary>
        ///     The sub forms for this window
        /// </summary>
        public HashSet<Form> SubForms { get; set; }

        /// <summary>
        ///     The sub IBaseForms handled in this MDI
        /// </summary>
        public HashSet<IBaseForm> SubWindows
        {
            get
            {
                HashSet<IBaseForm> retVal = new HashSet<IBaseForm>();

                foreach (Form form in SubForms)
                {
                    IBaseForm baseForm = form as IBaseForm;
                    if (baseForm != null)
                    {
                        retVal.Add(baseForm);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Handles the fact that a sub window has been closed
        /// </summary>
        /// <param name="form"></param>
        public void HandleSubWindowClosed(Form form)
        {
            try
            {
                SubForms.Remove(form);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     Finds a  specific window in a collection of windows
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static class GenericWindowHandling<T>
            where T : Form, new()
        {
            /// <summary>
            ///     Finds the specified element in the collection provided
            /// </summary>
            /// <param name="subWindows">The collection of forms in which the search should be performed</param>
            /// <returns></returns>
            public static T Find(ICollection<Form> subWindows)
            {
                T retVal = null;

                foreach (Form form in subWindows)
                {
                    retVal = form as T;
                    if (retVal != null)
                    {
                        break;
                    }
                }

                return retVal;
            }

            /// <summary>
            ///     Finds the specified element in the collection provided
            /// </summary>
            /// <param name="subWindows"></param>
            /// <returns></returns>
            public static T Find(ICollection<IBaseForm> subWindows)
            {
                T retVal = null;

                foreach (IBaseForm form in subWindows)
                {
                    retVal = form as T;
                    if (retVal != null)
                    {
                        break;
                    }
                }

                return retVal;
            }

            /// <summary>
            ///     Displays or shows the window, at the specified location
            /// </summary>
            /// <param name="window"></param>
            /// <param name="subWindow"></param>
            /// <param name="area"></param>
            public static void AddOrShow(MainWindow window, T subWindow, DockAreas area)
            {
                if (subWindow == null)
                {
                    subWindow = new T();
                    window.AddChildWindow(subWindow, area);
                }
                else
                {
                    subWindow.Show();
                }
            }
        }

        /// <summary>
        ///     Provides a message view window
        /// </summary>
        public Window MessagesWindow
        {
            get { return GenericWindowHandling<Window>.Find(SubWindows); }
        }

        /// <summary>
        ///     Provides a navigation view window
        /// </summary>
        public NavigationView.Window NavigationWindow
        {
            get { return GenericWindowHandling<NavigationView.Window>.Find(SubWindows); }
        }

        /// <summary>
        ///     Provides a more info view window
        /// </summary>
        public MoreInfoView.Window MoreInfoWindow
        {
            get { return GenericWindowHandling<MoreInfoView.Window>.Find(SubWindows); }
        }

        /// <summary>
        ///     Provides a property view window
        /// </summary>
        public PropertyView.Window PropertyWindow
        {
            get { return GenericWindowHandling<PropertyView.Window>.Find(SubWindows); }
        }

        /// <summary>
        ///     Provides a requirement view window
        /// </summary>
        public RequirementsView.Window RequirementsWindow
        {
            get { return GenericWindowHandling<RequirementsView.Window>.Find(SubWindows); }
        }

        /// <summary>
        ///     Provides a usage view window
        /// </summary>
        public UsageView.Window UsageWindow
        {
            get { return GenericWindowHandling<UsageView.Window>.Find(SubWindows); }
        }


        /// <summary>
        ///     Provides a data dictionary window
        /// </summary>
        public DataDictionaryView.Window DataDictionaryWindow
        {
            get { return GenericWindowHandling<DataDictionaryView.Window>.Find(SubWindows); }
        }

        /// <summary>
        ///     Provides a specification window
        /// </summary>
        public SpecificationView.Window SpecificationWindow
        {
            get { return GenericWindowHandling<SpecificationView.Window>.Find(SubWindows); }
        }

        /// <summary>
        ///     Provides the history window
        /// </summary>
        public HistoryView.Window HistoryWindow
        {
            get { return GenericWindowHandling<HistoryView.Window>.Find(SubWindows); }
        }

        /// <summary>
        ///     Provides a test runner window
        /// </summary>
        public TestRunnerView.Window TestWindow
        {
            get { return GenericWindowHandling<TestRunnerView.Window>.Find(SubWindows); }
        }

        /// <summary>
        ///     Provides a watch window
        /// </summary>
        public TestRunnerView.Watch.Window WatchWindow
        {
            get { return GenericWindowHandling<TestRunnerView.Watch.Window>.Find(SubWindows); }
        }

        /// <summary>
        ///     The translation window
        /// </summary>
        public TranslationRules.Window TranslationWindow
        {
            get
            {
                TranslationRules.Window retVal = null;
                Dictionary dictionary = GetActiveDictionary();
                if (dictionary != null)
                {
                    retVal = new TranslationRules.Window(dictionary.TranslationDictionary);
                    AddChildWindow(retVal);
                }
                return retVal;
            }
        }

        /// <summary>
        ///     The shortcuts window
        /// </summary>
        private Shortcuts.Window ShortcutsWindow
        {
            get { return GenericWindowHandling<Shortcuts.Window>.Find(SubWindows); }
        }

        /// <summary>
        ///     The selection history window
        /// </summary>
        private SelectionHistory.Window SelectionHistoryWindow
        {
            get { return GenericWindowHandling<SelectionHistory.Window>.Find(SubWindows); }
        }


        /// <summary>
        ///     The editor window
        /// </summary>
        private ExpressionWindow ExpressionEditorWindow
        {
            get { return GenericWindowHandling<ExpressionWindow>.Find(SubWindows); }
        }

        /// <summary>
        ///     The comment window
        /// </summary>
        private CommentWindow CommentEditorWindow
        {
            get { return GenericWindowHandling<CommentWindow>.Find(SubWindows); }
        }

        /// <summary>
        ///     The status handler
        /// </summary>
        private StatusHandler StatusHandler { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SubForms = new HashSet<Form>();
            GuiUtils.MdiWindow = this;
            GuiUtils.Graphics = CreateGraphics();
            StatusHandler = new StatusHandler();

            DataDictionary.EfsSystem.Instance.Context.ValueChange += Context_ValueChange;

            FormClosing += MainWindow_FormClosing;
            KeyUp += MainWindow_KeyUp;
        }

        /// <summary>
        ///     Updates the title when value change
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        private void Context_ValueChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            try
            {
                BeginInvoke((MethodInvoker) UpdateTitle);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     Handles the closing of the main window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool canceled = CheckSave();
            e.Cancel = canceled;
        }

        /// <summary>
        ///     Handles specific key actions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        Close();
                        e.Handled = true;
                        break;
                }
            }
        }

        /// <summary>
        ///     Destructor
        /// </summary>
        ~MainWindow()
        {
            try
            {
                GuiUtils.Graphics.Dispose();
                GuiUtils.Graphics = null;
            }
            catch (Exception)
            {
            }
            GuiUtils.MdiWindow = null;
        }

        /// <summary>
        ///     Updates the title according to the windows state
        /// </summary>
        public void UpdateTitle()
        {
            String windowTitle = "ERTMSFormalSpecs Workbench";

            foreach (Dictionary dictionary in EfsSystem.Dictionaries)
            {
                windowTitle += " " + dictionary.FilePath;
            }

            if (EfsSystem != null && EfsSystem.ShouldSave)
            {
                windowTitle += " [modified]";
            }

            Text = windowTitle;
        }

        /// <summary>
        ///     Provides the initial rectangle for each child window to provide a location for undocking
        /// </summary>
        private readonly Dictionary<Form, Rectangle> _initialRectangle = new Dictionary<Form, Rectangle>();

        /// <summary>
        ///     Adds a child window to this parent MDI
        /// </summary>
        /// <param name="window"></param>
        /// <param name="dockArea">where to place the window</param>
        /// <returns></returns>
        public void AddChildWindow(Form window, DockAreas dockArea = DockAreas.Document)
        {
            if (window != null)
            {
                _initialRectangle[window] = new Rectangle(new Point(50, 50), window.Size);

                DockContent docContent = window as DockContent;
                if (docContent != null)
                {
                    SubForms.Add(docContent);

                    docContent.DockAreas = dockArea;
                    if (dockArea == DockAreas.DockLeft)
                    {
                        docContent.Show(dockPanel, DockState.DockLeftAutoHide);
                    }
                    else if (dockArea == DockAreas.Float)
                    {
                        docContent.Show(dockPanel, DockState.Float);
                    }
                    else
                    {
                        docContent.Show(dockPanel);
                    }
                }
                else
                {
                    SubForms.Add(window);
                    window.MdiParent = this;
                    window.Show();

                    window.Activate();
                    ActivateMdiChild(window);
                }
            }
        }

        #region OpenFile

        /// ------------------------------------------------------
        /// OPEN OPERATIONS
        /// ------------------------------------------------------
        /// <summary>
        ///     The efs system
        /// </summary>
        public EfsSystem EfsSystem
        {
            get { return EfsSystem.Instance; }
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open ERTMS Formal Spec file";
            openFileDialog.Filter = "EFS Files (*.efs)|*.efs|All Files (*.*)|*.*";

            // Retrieve the location where the model files have been installed
            // or use ../doc/specs when the EFS has not been installed (development environment)
            const string keyName =
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall\ERTMSFormalSpecs_is1";
            const string valueName = @"Inno Setup CodeFile: ModelFiles";
            openFileDialog.InitialDirectory = (string)Registry.GetValue(keyName, valueName, @"..\doc\specs"); ;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                OpenFile(openFileDialog.FileName);
            }
        }

        /// <summary>
        ///     Opens a specific file
        /// </summary>
        /// <param name="fileName"></param>
        public void OpenFile(string fileName)
        {
            const bool allowErrors = false;
            bool shouldPlace = EfsSystem.Dictionaries.Count == 0;
            OpenFileOperation openFileOperation = new OpenFileOperation(fileName, EfsSystem, allowErrors, true);
            openFileOperation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Opening file " + fileName, false);

            // Open the windows
            if (openFileOperation.Dictionary != null)
            {
                SetupWindows(openFileOperation.Dictionary, shouldPlace);

                CheckModelOperation checkModel = new CheckModelOperation();
                checkModel.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Checking model");
            }
            else if (!openFileOperation.Dialog.Canceled)
            {
                Console.WriteLine("Error: cannot open file " + fileName);
                // DefaultDesktopOnly option is added in order to avoid exceptions during nightbuild execution
                MessageBox.Show ("Cannot open file, please see the console for more information", "Cannot open file",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                 MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        /// <summary>
        ///     Setup the window according to the dictionary provided
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="shouldPlace"></param>
        public void SetupWindows(Dictionary dictionary, bool shouldPlace)
        {
            Util.DontNotify(() =>
            {
                // Display the document views
                // Only open the model view window if model elements are available in the opened file
                DataDictionaryView.Window modelWindow = null;
                if (dictionary.NameSpaces.Count > 0)
                {
                    modelWindow = new DataDictionaryView.Window(dictionary);
                    AddChildWindow(modelWindow);
                }
                GenericWindowHandling<TestRunnerView.Window>.AddOrShow(this, TestWindow, DockAreas.Document);

                if (dictionary.TranslationDictionary != null &&
                    dictionary.TranslationDictionary.TranslationsCount > 0)
                {
                    TranslationRules.Window translationWindow = new TranslationRules.Window(dictionary.TranslationDictionary);
                    AddChildWindow(translationWindow);
                }

                GenericWindowHandling<NavigationView.Window>.AddOrShow(this, NavigationWindow, DockAreas.DockTop);
                dockPanel.DockTopPortion = 65;

                // Display the views in the left pane
                GenericWindowHandling<SpecificationView.Window>.AddOrShow(this, SpecificationWindow,
                    DockAreas.DockLeft);

                // Display the views in the bottom pane
                GenericWindowHandling<RequirementsView.Window>.AddOrShow(this, RequirementsWindow,
                    DockAreas.DockBottom);
                GenericWindowHandling<UsageView.Window>.AddOrShow(this, UsageWindow, DockAreas.DockBottom);

                GenericWindowHandling<MoreInfoView.Window>.AddOrShow(this, MoreInfoWindow, DockAreas.DockBottom);
                if (shouldPlace)
                {
                    MoreInfoWindow.Show(RequirementsWindow.Pane, DockAlignment.Right, 0.66);
                }

                GenericWindowHandling<CommentWindow>.AddOrShow(this, CommentEditorWindow, DockAreas.DockBottom);
                if (shouldPlace)
                {
                    CommentEditorWindow.Show(MoreInfoWindow.Pane, DockAlignment.Right, 0.5);
                }
                GenericWindowHandling<TestRunnerView.Watch.Window>.AddOrShow(this, WatchWindow,
                    DockAreas.DockBottom);
                if (shouldPlace)
                {
                    WatchWindow.Show(CommentEditorWindow.Pane, CommentEditorWindow);
                }
                CommentEditorWindow.Show();

                // Display the views in the right pane
                GenericWindowHandling<PropertyView.Window>.AddOrShow(this, PropertyWindow, DockAreas.DockRight);
                GenericWindowHandling<ExpressionWindow>.AddOrShow(this, ExpressionEditorWindow,
                    DockAreas.DockRight);
                if (shouldPlace)
                {
                    ExpressionEditorWindow.Show(PropertyWindow.Pane, DockAlignment.Bottom, 0.6);
                }
                GenericWindowHandling<HistoryView.Window>.AddOrShow(this, HistoryWindow, DockAreas.DockRight);
                if (shouldPlace)
                {
                    HistoryWindow.Show(ExpressionEditorWindow.Pane, ExpressionEditorWindow);
                }

                GenericWindowHandling<Shortcuts.Window>.AddOrShow(this, ShortcutsWindow, DockAreas.DockRight);
                if (shouldPlace)
                {
                    ShortcutsWindow.Show(ExpressionEditorWindow.Pane, ExpressionEditorWindow);
                }

                GenericWindowHandling<SelectionHistory.Window>.AddOrShow(this, SelectionHistoryWindow,
                    DockAreas.DockRight);
                if (shouldPlace)
                {
                    SelectionHistoryWindow.Show(ShortcutsWindow.Pane, ShortcutsWindow);
                }
                ExpressionEditorWindow.Show();

                GenericWindowHandling<Window>.AddOrShow(this, MessagesWindow, DockAreas.DockRight);
                if (shouldPlace)
                {
                    MessagesWindow.Show(HistoryWindow.Pane, DockAlignment.Bottom, 0.3);
                }

                if (modelWindow != null)
                {
                    modelWindow.Focus();
                }
            });
        }

        #endregion

        #region SaveFile

        /// ------------------------------------------------------
        /// SAVE OPERATIONS
        /// ------------------------------------------------------
        /// <summary>
        ///     Provides the dictionary on which operation should be performed
        /// </summary>
        /// <returns></returns>
        public Dictionary GetActiveDictionary()
        {
            Dictionary retVal = null;

            if (EfsSystem != null)
            {
                if (EfsSystem.Dictionaries.Count == 1)
                {
                    retVal = EfsSystem.Dictionaries[0];
                }
                else if (EfsSystem.Dictionaries.Count > 1)
                {
                    DictionarySelector.DictionarySelector dictionarySelector = new DictionarySelector.DictionarySelector();
                    dictionarySelector.ShowDialog(this);

                    if (dictionarySelector.Selected != null)
                    {
                        retVal = dictionarySelector.Selected;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The tooltip associated to this form
        /// </summary>
        public ToolTip ToolTip
        {
            get { return toolTip; }
        }


        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary activeDictionary = GetActiveDictionary();

            if (activeDictionary != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Saving EFS file " + activeDictionary.Name;
                saveFileDialog.Filter = "EFS files (*.efs)|*.efs|All Files (*.*)|*.*";
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    activeDictionary.FilePath = saveFileDialog.FileName;
                    SaveOperation operation = new SaveOperation(activeDictionary);
                    operation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Saving file " + activeDictionary.FilePath, false);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Dictionary dictionary in EfsSystem.Dictionaries)
            {
                SaveOperation operation = new SaveOperation(dictionary);
                operation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Saving file " + dictionary.Name, false);
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            foreach (Dictionary dictionary in EfsSystem.Dictionaries)
            {
                SaveOperation operation = new SaveOperation(dictionary);
                operation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Saving file " + dictionary.Name, false);
            }
        }

        #endregion

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Checks if the file should be saved before closing the window
        /// </summary>
        /// <returns></returns>
        private bool CheckSave()
        {
            bool retVal = false;

            if (EfsSystem.ShouldSave)
            {
                DialogResult result = MessageBox.Show("Model has been changed, do you want to save it ?",
                    "Model changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
                switch (result)
                {
                    case DialogResult.Yes:
                        SaveOperation operation = new SaveOperation();
                        operation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Saving files", false);
                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        retVal = true;
                        break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The rich text box currently selected
        /// </summary>
        public EditorTextBox SelectedRichTextBox { get; set; }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRichTextBox != null)
            {
                SelectedRichTextBox.Undo();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRichTextBox != null)
            {
                SelectedRichTextBox.Redo();
            }
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRichTextBox != null)
            {
                SelectedRichTextBox.Cut();
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRichTextBox != null)
            {
                SelectedRichTextBox.Copy();
            }
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRichTextBox != null)
            {
                SelectedRichTextBox.Paste();
            }
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        #region Check model

        private void checkModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckModelOperation operation = new CheckModelOperation();
            operation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Check model");

            MessageCounter counter = new MessageCounter(EfsSystem);
            MessageBox.Show(
                counter.Error + " error(s)\n" + counter.Warning + " warning(s)\n" + counter.Info +
                " info message(s) found", "Check result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        private void implementedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    if (dictionary.Specifications != null)
                    {
                        foreach (Specification specification in dictionary.Specifications)
                        {
                            specification.CheckImplementation();
                        }
                    }
                }
            });
        }

        private void implementationRequiredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    dictionary.MarkUnimplementedItems();
                }
            });
        }

        private void verificationRequiredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    dictionary.MarkNotVerifiedRules();
                }
            });
        }

        private void reviewedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    if (dictionary.Specifications != null)
                    {
                        foreach (Specification specification in dictionary.Specifications)
                        {
                            specification.CheckReview();
                        }
                    }
                }
            });
        }

        /// ------------------------------------------------------
        /// MARKS OPERATIONS
        /// ------------------------------------------------------
        private void clearMarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() => { });
        }

        private void markRequirementsWhereMoreInfoIsRequiredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    if (dictionary.Specifications != null)
                    {
                        foreach (Specification specification in dictionary.Specifications)
                        {
                            specification.CheckMoreInfo();
                        }
                    }
                }
            });
        }

        private void markImplementedButNoFunctionalTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    if (dictionary.Specifications != null)
                    {
                        foreach (Specification specification in dictionary.Specifications)
                        {
                            specification.CheckImplementedWithNoFunctionalTest();
                        }
                    }
                }
            });
        }

        private void markNotImplementedButImplementationExistsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    if (dictionary.Specifications != null)
                    {
                        foreach (Specification specification in dictionary.Specifications)
                        {
                            specification.CheckNotImplementedButImplementationExists();
                        }
                    }
                }
            });
        }

        private void markApplicableParagraphsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    if (dictionary.Specifications != null)
                    {
                        foreach (Specification specification in dictionary.Specifications)
                        {
                            specification.CheckApplicable();
                        }
                    }
                }
            });
        }

        private void markImplementationRequiredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    dictionary.MarkUnimplementedTests();
                }
            });
        }

        private void markNotTranslatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    dictionary.MarkNotTranslatedTests();
                }
            });
        }

        private void markNotImplementedTranslationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    dictionary.MarkNotImplementedTranslations();
                }
            });
        }

        private void markNonApplicableRequirementsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    if (dictionary.Specifications != null)
                    {
                        foreach (Specification specification in dictionary.Specifications)
                        {
                            specification.CheckNonApplicable();
                        }
                    }
                }
            });
        }

        private void markSpecIssuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    if (dictionary.Specifications != null)
                    {
                        foreach (Specification specification in dictionary.Specifications)
                        {
                            specification.CheckSpecIssues();
                        }
                    }
                }
            });
        }

        #region Import test database

        /// ------------------------------------------------------
        /// IMPORT TEST DATABASE OPERATIONS
        /// ------------------------------------------------------
        private void importDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Open test sequence database";
                openFileDialog.Filter = "Access Files (*.mdb)|*.mdb";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string sequenceName = Path.GetFileName(openFileDialog.FileName);
                    bool keepManualTranslations = true;
                    SubSequence subSequence = dictionary.FindSubSequence(sequenceName);
                    if (subSequence != null && subSequence.ContainsManualTranslation())
                    {
                        DialogResult dialogResult = MessageBox.Show(this, "Keep manual translations ?", "Manual translations found in " + sequenceName,
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.No)
                        {
                            keepManualTranslations = false;
                        }
                        else if (dialogResult == DialogResult.Cancel)
                        {
                            return;
                        }
                    }

                    ImportTestDataBaseOperation operation = new ImportTestDataBaseOperation(openFileDialog.FileName,
                        dictionary, ImportTestDataBaseOperation.Mode.File, keepManualTranslations);
                    operation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Import database");
                }
            }
        }

        private void importFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog();
                if (selectFolderDialog.ShowDialog(this) == DialogResult.OK)
                {
                    bool keepManualTranslations = true;
                    DialogResult dialogResult = MessageBox.Show(this, "Keep manual translations ?",
                        "Manual translations",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.No)
                    {
                        keepManualTranslations = false;
                    }
                    else if (dialogResult == DialogResult.Cancel)
                    {
                        return;
                    }

                    ImportTestDataBaseOperation operation =
                        new ImportTestDataBaseOperation(selectFolderDialog.SelectedPath, dictionary,
                            ImportTestDataBaseOperation.Mode.Directory, keepManualTranslations);
                    operation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Import database directory");
                }
            }
        }

        #endregion

        /// ------------------------------------------------------
        /// CREATE REPORT OPERATIONS
        /// ------------------------------------------------------
        private void specCoverageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                SpecReport aReport = new SpecReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void testsCoverageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                TestReport aReport = new TestReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void generateDynamicCoverageReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                TestReport aReport = new TestReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void generateCoverageReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                SpecReport aReport = new SpecReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void generateSpecIssuesReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                MarkingHistory.PerformMark(() =>
                {

                    // Apply translation rule to get the spec issues from the translated steps
                    foreach (DataDictionary.Tests.Frame frame in dictionary.Tests)
                    {
                        frame.Translate();
                    }
                });
                RefreshModel.Execute();

                SpecIssuesReport aReport = new SpecIssuesReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void generateDataDictionaryReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                ModelReport aReport = new ModelReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void generateFindingsReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                FindingsReport aReport = new FindingsReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void searchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SearchDialog.SearchDialog dialog = new SearchDialog.SearchDialog();
            dialog.Initialise();
            dialog.ShowDialog(this);
        }

        private void refreshWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ensure the system has been compiled
            EfsSystem efsSystem = EfsSystem.Instance;
            efsSystem.Compiler.Compile_Synchronous(efsSystem.ShouldRebuild);
            efsSystem.ShouldRebuild = false;

            RefreshModel.Execute();
        }

        private void showRulePerformancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RulesPerformances rulePerformances = new RulesPerformances();
            AddChildWindow(rulePerformances);
        }

        /// <summary>
        ///     ReInit counters in rules
        /// </summary>
        private class ResetTimeStamps : Visitor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="efsSystem"></param>
            public ResetTimeStamps(EfsSystem efsSystem)
            {
                foreach (Dictionary dictionary in efsSystem.Dictionaries)
                {
                    visit(dictionary, true);
                }
            }

            public override void visit(Rule obj, bool visitSubNodes)
            {
                DataDictionary.Rules.Rule rule = obj as DataDictionary.Rules.Rule;

                if (rule != null)
                {
                    rule.ExecutionTimeInMilli = 0;
                    rule.ExecutionCount = 0;
                }

                base.visit(obj, visitSubNodes);
            }

            public override void visit(Function obj, bool visitSubNodes)
            {
                DataDictionary.Functions.Function function = obj as DataDictionary.Functions.Function;

                function.ExecutionTimeInMilli = 0;
                function.ExecutionCount = 0;

                base.visit(obj, visitSubNodes);
            }

            public override void visit(Frame obj, bool visitSubNodes)
            {
                // No rules in frames
            }
        }

        private void resetCountersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EfsSystem != null)
            {
                ResetTimeStamps reset = new ResetTimeStamps(EfsSystem);
            }
        }

        private void showFunctionsPerformancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FunctionsPerformances.FunctionsPerformances functionsPerformances =
                new FunctionsPerformances.FunctionsPerformances();
            AddChildWindow(functionsPerformances);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Creates a new dictionary
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Create new dictionary. Select dictionary file location";
            openFileDialog.Filter = "EFS Files (*.efs)|*.efs";
            openFileDialog.CheckFileExists = false;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                Dictionary dictionary = new Dictionary();
                dictionary.FilePath = filePath;
                dictionary.Name = Path.GetFileNameWithoutExtension(filePath);
                EfsSystem.AddDictionary(dictionary);

                // Open a data dictionary window if none is yet present
                bool found = false;
                foreach (IBaseForm form in SubWindows)
                {
                    if (form is DataDictionaryView.Window)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    AddChildWindow(new DataDictionaryView.Window(dictionary), DockAreas.Document);
                }
            }
        }

        private void markParagraphsFromNewRevisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    foreach (Specification specification in dictionary.Specifications)
                    {
                        specification.CheckNewRevision();
                    }
                }
            });
        }

        private void generateERTMSAcademyReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                ErtmsAcademyReport aReport = new ErtmsAcademyReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void compareWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open ERTMS Formal Spec file";
            openFileDialog.Filter = "EFS Files (*.efs)|*.efs|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                Dictionary dictionary = GetActiveDictionary();

                CompareWithFileOperation operation = new CompareWithFileOperation(dictionary, openFileDialog.FileName);
                operation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Compare with file");

                Refresh();
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options.Options optionForm = new Options.Options();
            optionForm.ShowDialog(this);
            Options.Options.SetSettings();
        }

        private void compareWithGitRevisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Compares with the active dictionary
            Dictionary dictionary = GetActiveDictionary();

            // Retrieve the hash tag and the corresponding dictionary version
            VersionSelector.VersionSelector selector = new VersionSelector.VersionSelector(dictionary);
            selector.Text = "Compare current version with with repository version";
            selector.ShowDialog();
            if (selector.Selected != null)
            {
                CompareWithRepositoryOperation operation = new CompareWithRepositoryOperation(dictionary,
                    selector.Selected);
                operation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Compare with repository version " + selector.Selected.MessageShort);
                Refresh();
            }
        }

        private void generateFunctionalAnalysisReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                FunctionalAnalysisReport aReport = new FunctionalAnalysisReport(dictionary);
                aReport.ShowDialog(this);
            }
        }

        private void dockedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockContent dockContent = SelectedForm() as DockContent;
            if (dockContent != null)
            {
                if (dockContent.DockAreas == DockAreas.Document)
                {
                    dockContent.Hide();
                    Rectangle rectangle = _initialRectangle[dockContent];
                    dockContent.DockAreas = DockAreas.Float;
                    dockContent.DockState = DockState.Float;
                    dockContent.Show(dockPanel, rectangle);
                    dockContent.ParentForm.FormBorderStyle = FormBorderStyle.Sizable;
                }
            }
        }

        /// <summary>
        ///     Provides the selected form
        /// </summary>
        /// <returns></returns>
        private Form SelectedForm()
        {
            Form retVal = null;

            foreach (DockContent dockContent in dockPanel.Contents)
            {
                if (dockContent.IsActivated)
                {
                    retVal = dockContent;
                    break;
                }
            }

            return retVal;
        }

        private void showSpecificationViewToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            AddChildWindow(new SpecificationView.Window(), DockAreas.DockLeft);
        }

        private void showModelViewToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                AddChildWindow(new DataDictionaryView.Window(dictionary), DockAreas.Document);
            }
        }

        private void showShortcutsViewToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            GenericWindowHandling<Shortcuts.Window>.AddOrShow(this, ShortcutsWindow, DockAreas.DockRight);
        }

        private void showTestsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            GenericWindowHandling<TestRunnerView.Window>.AddOrShow(this, TestWindow, DockAreas.Document);
        }

        private void showTranslationViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddChildWindow(TranslationWindow);
        }

        /// <summary>
        ///     Sets the status of the window
        /// </summary>
        /// <param name="statusText"></param>
        public void SetStatus(string statusText)
        {
            BeginInvoke((MethodInvoker) (() => toolStripStatusLabel.Text = statusText));
        }

        private void blameUntilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Compares with the active dictionary
            Dictionary dictionary = GetActiveDictionary();

            // Retrieve the hash tag
            VersionSelector.VersionSelector selector = new VersionSelector.VersionSelector(dictionary);
            selector.Text = "Select the version up to which blame mode should be built";
            selector.ShowDialog();

            UpdateBlameInformationOperation operation = new UpdateBlameInformationOperation(dictionary,
                selector.Selected);
            operation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Update blame information");
        }

        private void showHistoryViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HistoryWindow == null)
            {
                HistoryView.Window window = new HistoryView.Window();
                AddChildWindow(window);
            }
        }

        private void markNotTestedButFunctionalTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    if (dictionary.Specifications != null)
                    {
                        foreach (Specification specification in dictionary.Specifications)
                        {
                            specification.CheckNotTestedWithFunctionalTests();
                        }
                    }
                }
            });
        }

        private void checkModelToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CheckModelOperation operation = new CheckModelOperation();
            operation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Check model");

            MessageCounter counter = new MessageCounter(EfsSystem);
            MessageBox.Show(
                counter.Error + " error(s)\n" + counter.Warning + " warning(s)\n" + counter.Info +
                " info message(s) found", "Check result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checkToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CheckDeadModelOperation operation = new CheckDeadModelOperation();
            operation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Check dead model");

            MessageCounter counter = new MessageCounter(EfsSystem);
            MessageBox.Show(
                counter.Error + " error(s)\n" + counter.Warning + " warning(s)\n" + counter.Info +
                " info message(s) found", "Check result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void showRequirementSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequirementSetDiagramWindow window = new RequirementSetDiagramWindow();
            GuiUtils.MdiWindow.AddChildWindow(window);

            Dictionary dictionary = GuiUtils.MdiWindow.GetActiveDictionary();
            window.Text = "Requirement sets for " + dictionary.Name;
            OpenRequirementSetOperation openRequirementSet = new OpenRequirementSetOperation(window, dictionary);
            openRequirementSet.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Opening requirement set for " + dictionary.Name);
        }

        private void showWatchViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<TestRunnerView.Watch.Window>.AddOrShow(this, WatchWindow, DockAreas.DockBottom);
        }

        private void showMessagesVoewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<Window>.AddOrShow(this, MessagesWindow, DockAreas.DockBottom);
        }

        private void showMoreInfoViewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<MoreInfoView.Window>.AddOrShow(this, MoreInfoWindow, DockAreas.DockBottom);
        }

        private void showProperyViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<PropertyView.Window>.AddOrShow(this, PropertyWindow, DockAreas.DockRight);
        }

        private void showRequirementViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<RequirementsView.Window>.AddOrShow(this, RequirementsWindow, DockAreas.DockBottom);
        }

        private void showUsageViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<UsageView.Window>.AddOrShow(this, UsageWindow, DockAreas.DockBottom);
        }

        private void showSelectionHistoryViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<SelectionHistory.Window>.AddOrShow(this, SelectionHistoryWindow, DockAreas.DockBottom);
        }

        private void showExpressionEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<ExpressionWindow>.AddOrShow(this, ExpressionEditorWindow, DockAreas.DockRight);
        }

        private void showCommentEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<CommentWindow>.AddOrShow(this, CommentEditorWindow, DockAreas.DockBottom);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutDialogBox aboutDialogBox = new AboutDialogBox();
            aboutDialogBox.ShowDialog();
        }

        private void importStartStopConditionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select excel file...";
            openFileDialog.Filter = "Microsof Excel (.xls, .xlsm)|*.xls;*.xlsm";
            SpecsImporter specsImporter = new SpecsImporter();

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (DataDictionaryWindow.Dictionary != null)
                {
                    specsImporter.TheDictionary = DataDictionaryWindow.Dictionary;
                    specsImporter.FileName = openFileDialog.FileName;

                    ProgressDialog dialog = new ProgressDialog("Importing excel file....", specsImporter);
                    dialog.ShowDialog(Owner);
                }
            }
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // First select the base dictionary that will be updated
            // This ensures that the update will have a base dictionary
            string updatedGuid = "";
            {
                DictionarySelector.DictionarySelector dictionarySelector = new DictionarySelector.DictionarySelector();
                dictionarySelector.ShowDictionaries(this);

                if (dictionarySelector.Selected != null)
                {
                    updatedGuid = dictionarySelector.Selected.Guid;
                }
            }

            // Creates a new dictionary
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Create update for an existing dictionary. Select update file location";
            openFileDialog.Filter = "EFS Files (*.efs)|*.efs";
            openFileDialog.CheckFileExists = false;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                Dictionary dictionary = new Dictionary();
                dictionary.FilePath = filePath;
                dictionary.Name = Path.GetFileNameWithoutExtension(filePath);
                dictionary.setUpdates(updatedGuid);
                EfsSystem.AddDictionary(dictionary);
                AddChildWindow(new DataDictionaryView.Window(dictionary), DockAreas.Document);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary dictionary = GetActiveDictionary();
            if (dictionary != null)
            {
                // Identify the forms that need to be closed
                List<BaseForm> toClose = new List<BaseForm>();
                foreach (Form form in SubForms)
                {
                    DataDictionaryView.Window dictionaryWindow = form as DataDictionaryView.Window;
                    if (dictionaryWindow != null && dictionaryWindow.Dictionary == dictionary)
                    {
                        toClose.Add(dictionaryWindow);
                    }

                    TranslationRules.Window translationWindow = form as TranslationRules.Window;
                    if (translationWindow != null && translationWindow.TranslationDictionary != null &&
                        translationWindow.TranslationDictionary.Dictionary == dictionary)
                    {
                        toClose.Add(translationWindow);
                    }
                }

                // And close them (this modifies the SubForm list => must be done in two steps
                foreach (BaseForm form in toClose)
                {
                    form.Close();
                }

                // Remove all references to the closed dictionary
                CloseDictionary closeDictionary = new CloseDictionary(dictionary);
                closeDictionary.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Closing dictionary", false);
            }
        }

        private void refactorToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            CleanUpModelOperation cleanUpModel = new CleanUpModelOperation();
            cleanUpModel.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Cleaning up");
        }

        private void mergeUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DictionarySelector.DictionarySelector dictionarySelector = new DictionarySelector.DictionarySelector(FilterOptions.Updates);
            dictionarySelector.ShowDictionaries(GuiUtils.MdiWindow);

            if (dictionarySelector.Selected != null)
            {
                MergeUpdateOperation merge = new MergeUpdateOperation(dictionarySelector.Selected);
                merge.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Merging", false);
            }
        }

        private void showNavigationViewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GenericWindowHandling<NavigationView.Window>.AddOrShow(this, NavigationWindow, DockAreas.DockTop);
        }

        private class MarkVariables : Visitor
        {
            /// <summary>
            /// Which mode should be marked.
            /// </summary>
            private acceptor.VariableModeEnumType Mode { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="mode"></param>
            public MarkVariables(acceptor.VariableModeEnumType mode)
            {
                Mode = mode;
            }

            public override void visit(Variable obj, bool visitSubNodes)
            {   // IN OUT are also IN or OUT.
                DataDictionary.Variables.Variable myVariable = (DataDictionary.Variables.Variable) obj;
                
                if (myVariable.Mode == Mode || 
                    myVariable.Mode == acceptor.VariableModeEnumType.aInOut)
                {
                    myVariable.AddInfo(acceptor.Enum_VariableModeEnumType_ToString(Mode) + " variable");
                }

                base.visit(obj, visitSubNodes);
            }
        }

        private void iNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    MarkVariables markVariables = new MarkVariables(acceptor.VariableModeEnumType.aIncoming);
                    markVariables.visit(dictionary, true);
                }
            }
            );
        }

        private void oUTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    MarkVariables markVariables = new MarkVariables(acceptor.VariableModeEnumType.aOutgoing);
                    markVariables.visit(dictionary, true);
                }
            }
            );
        }

        private void iNOUTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    MarkVariables markVariables = new MarkVariables(acceptor.VariableModeEnumType.aInOut);
                    markVariables.visit(dictionary, true);
                }
            }
            );
        }


        private class MarkRules : Visitor
        {
            /// <summary>
            /// Which priority should be marked.
            /// </summary>
            private acceptor.RulePriority Priority { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="priority"></param>
            public MarkRules(acceptor.RulePriority priority)
            {
                Priority = priority;
            }

            public override void visit(Rule obj, bool visitSubNodes)
            {
                DataDictionary.Rules.Rule myRule = (DataDictionary.Rules.Rule)obj;

                if (myRule.getPriority() == Priority)
                {
                    myRule.AddInfo(acceptor.Enum_RulePriority_ToString(Priority) + " priority");
                }
                
                base.visit(obj, visitSubNodes);
            }
        }

        private void processingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Dictionaries)
                {
                    MarkRules markrules = new MarkRules(acceptor.RulePriority.aProcessing);
                    markrules.visit(dictionary, true);
                }
            }
            );
        }

        private void updateOUTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Dictionary dictionary in EfsSystem.Dictionaries)
            {
                MarkRules markrules = new MarkRules(acceptor.RulePriority.aUpdateOUT);
                markrules.visit(dictionary, true);
            }
        }

        private void updateINTERNALToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Dictionary dictionary in EfsSystem.Dictionaries)
            {
                MarkRules markrules = new MarkRules(acceptor.RulePriority.aUpdateINTERNAL);
                markrules.visit(dictionary, true);
            }
        }

        private void cleanUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Dictionary dictionary in EfsSystem.Dictionaries)
            {
                MarkRules markrules = new MarkRules(acceptor.RulePriority.aCleanUp);
                markrules.visit(dictionary, true);
            }
        }

        private void verificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Dictionary dictionary in EfsSystem.Dictionaries)
            {
                MarkRules markrules = new MarkRules(acceptor.RulePriority.aVerification);
                markrules.visit(dictionary, true);
            }
        }

        private void findInSubset076ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog();
            if (selectFolderDialog.ShowDialog(this) == DialogResult.OK)
            {
                FindInTestDataBasesOperation operation =
                        new FindInTestDataBasesOperation(selectFolderDialog.SelectedPath, 136);
                operation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Find in databases");
                string result = "";
                foreach (string file in operation.FileNames)
                {
                    result += file + "\n";
                }
                LongMessageForm textPresentation = new LongMessageForm {richTextBox1 = {Text = result}};
                textPresentation.Show();
            }
        }
    }
}