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
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Generated;
using DataDictionary.Tests.Runner;
using GUI.IPCInterface;
using Utils;
using Dictionary = DataDictionary.Dictionary;
using Frame = DataDictionary.Tests.Frame;
using Step = DataDictionary.Tests.Step;
using SubSequence = DataDictionary.Tests.SubSequence;

namespace GUI.TestRunnerView
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


        public override BaseTreeView TreeView
        {
            get { return testBrowserTreeView; }
        }

        public override ExplainTextBox ExplainTextBox
        {
            get { return null; }
        }

        /// <summary>
        ///     The data dictionary for this view
        /// </summary>
        private EFSSystem efsSystem;

        public EFSSystem EFSSystem
        {
            get { return efsSystem; }
            private set
            {
                efsSystem = value;
                testBrowserTreeView.Root = efsSystem;
            }
        }

        /// <summary>
        ///     The runner
        /// </summary>
        public Runner getRunner(SubSequence subSequence)
        {
            Runner runner = EFSSystem.Runner;

            if (runner == null || runner.SubSequence != subSequence)
            {
                if (subSequence != null)
                {
                    EFSSystem.Runner = new Runner(subSequence, false, false, true);
                }
            }

            return EFSSystem.Runner;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public Window()
        {
            InitializeComponent();

            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            frameToolStripComboBox.DropDown += new EventHandler(frameToolStripComboBox_DropDown);
            subSequenceSelectorComboBox.DropDown += new EventHandler(subSequenceSelectorComboBox_DropDown);
            Text = "System test view";
            Visible = false;
            EFSSystem = EFSSystem.INSTANCE;
            Refresh();
        }

        private void frameToolStripComboBox_DropDown(object sender, EventArgs e)
        {
            RebuildFramesComboBox();
        }

        private void subSequenceSelectorComboBox_DropDown(object sender, EventArgs e)
        {
            RebuildSubSequencesComboBox();
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
        ///     Indicates that a refresh is ongoing
        /// </summary>
        private bool DoingRefresh { get; set; }

        /// <summary>
        ///     Sets the current frame parameters
        /// </summary>
        /// <param name="frame"></param>
        public void setFrame(Frame frame)
        {
            Invoke((MethodInvoker) delegate
            {
                frameToolStripComboBox.Text = frame.Name;
                Refresh();
            });
        }

        /// <summary>
        ///     Sets the current sub sequence window parameters
        /// </summary>
        /// <param name="subSequence"></param>
        public void setSubSequence(SubSequence subSequence)
        {
            Invoke((MethodInvoker) delegate
            {
                subSequenceSelectorComboBox.Text = subSequence.Name;
                setFrame(subSequence.Frame);
                Refresh();
            });
        }

        /// <summary>
        ///     Refreshes the display
        /// </summary>
        public override void Refresh()
        {
            if (!DoingRefresh)
            {
                try
                {
                    DoingRefresh = true;

                    string selectedFrame = frameToolStripComboBox.Text;
                    string selectedSequence = subSequenceSelectorComboBox.Text;

                    if (EFSSystem.Runner == null)
                    {
                        toolStripTimeTextBox.Text = "0";
                        toolStripCurrentStepTextBox.Text = "<none>";
                    }
                    else
                    {
                        toolStripTimeTextBox.Text = "" + EFSSystem.Runner.Time;
                        Step currentStep = EFSSystem.Runner.CurrentStep();
                        if (currentStep != null)
                        {
                            toolStripCurrentStepTextBox.Text = currentStep.Name;
                        }
                        else
                        {
                            toolStripCurrentStepTextBox.Text = "<none>";
                        }

                        if (EFSSystem.Runner.SubSequence != null && EFSSystem.Runner.SubSequence.Frame != null)
                        {
                            Frame = EFSSystem.Runner.SubSequence.Frame;
                            selectedFrame = EFSSystem.Runner.SubSequence.Frame.Name;
                            selectedSequence = EFSSystem.Runner.SubSequence.Name;
                        }
                    }

                    if (!GUIUtils.MDIWindow.AfterStep)
                    {
                        testBrowserTreeView.Refresh();
                    }
                    testDescriptionTimeLineControl.Refresh();
                    testExecutionTimeLineControl.Refresh();

                    RebuildFramesComboBox();
                    frameToolStripComboBox.Text = selectedFrame;
                    frameToolStripComboBox.ToolTipText = selectedFrame;

                    if (Frame == null || frameToolStripComboBox.Text.CompareTo(Frame.Name) != 0)
                    {
                        RebuildSubSequencesComboBox();

                        if (EFSSystem.Runner != null && EFSSystem.Runner.SubSequence != null)
                        {
                            EFSSystem.Runner = null;
                        }
                    }

                    if (EFSSystem.Runner != null && EFSSystem.Runner.SubSequence != null)
                    {
                        subSequenceSelectorComboBox.Text = EFSSystem.Runner.SubSequence.Name;
                    }

                    subSequenceSelectorComboBox.ToolTipText = subSequenceSelectorComboBox.Text;
                }
                finally
                {
                    DoingRefresh = false;
                }
            }

            base.Refresh();
        }

        /// <summary>
        ///     Rebuilds the contents of the frames combo box
        /// </summary>
        private void RebuildFramesComboBox()
        {
            frameToolStripComboBox.Items.Clear();
            List<string> frames = new List<string>();
            foreach (Dictionary dictionary in EFSSystem.Dictionaries)
            {
                foreach (Frame frame in dictionary.Tests)
                {
                    frames.Add(frame.Name);
                }
            }
            frames.Sort();

            foreach (string frame in frames)
            {
                if (frame != null)
                {
                    frameToolStripComboBox.Items.Add(frame);
                }
            }
        }

        /// <summary>
        ///     Rebuilds the contents of the subsequence combo box, according to the frame selected in the frames combo box
        /// </summary>
        private void RebuildSubSequencesComboBox()
        {
            subSequenceSelectorComboBox.Items.Clear();
            foreach (Dictionary dictionary in EFSSystem.Dictionaries)
            {
                Frame = dictionary.findFrame(frameToolStripComboBox.Text);
                if (Frame != null)
                {
                    List<string> subSequences = new List<string>();
                    foreach (SubSequence subSequence in Frame.SubSequences)
                    {
                        subSequences.Add(subSequence.Name);
                    }
                    subSequences.Sort();
                    foreach (string subSequence in subSequences)
                    {
                        subSequenceSelectorComboBox.Items.Add(subSequence);
                    }
                    break;
                }
            }
            if (subSequenceSelectorComboBox.Items.Count > 0)
            {
                subSequenceSelectorComboBox.Text = subSequenceSelectorComboBox.Items[0].ToString();
            }
            else
            {
                subSequenceSelectorComboBox.Text = "";
            }
        }

        /// <summary>
        ///     Step once
        /// </summary>
        public void StepOnce()
        {
            Util.DontNotify(() =>
            {
                CheckRunner();
                if (EFSSystem.Runner != null)
                {
                    EFSSystem.Runner.RunUntilTime(EFSSystem.Runner.Time + EFSSystem.Runner.Step);
                    GUIUtils.MDIWindow.RefreshAfterStep();
                }
            });
        }

        private void stepOnce_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = testExecutionTabPage;
            StepOnce();
        }

        private void restart_Click(object sender, EventArgs e)
        {
            if (EFSSystem.Runner != null)
            {
                EFSSystem.Runner.EndExecution();
                EFSSystem.Runner = null;
            }
            Clear();
            GUIUtils.MDIWindow.RefreshAfterStep();
            tabControl1.SelectedTab = testExecutionTabPage;
        }

        public void Clear()
        {
            EFSSystem.Runner = null;

            foreach (Dictionary dictionary in EFSSystem.Dictionaries)
            {
                dictionary.ClearMessages();
            }
        }

        /// <summary>
        ///     Ensures that the runner is not empty
        /// </summary>
        private void CheckRunner()
        {
            if (EFSSystem.Runner == null)
            {
                if (Frame != null)
                {
                    SubSequence subSequence = Frame.findSubSequence(subSequenceSelectorComboBox.Text);
                    if (subSequence != null)
                    {
                        EFSSystem.Runner = new Runner(subSequence, true, false, true);
                    }
                }
                else
                {
                    EFSSystem.Runner = EFSService.INSTANCE.Runner;
                }
            }
        }

        private void rewindButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = testExecutionTabPage;
            StepBack();
        }

        public void StepBack()
        {
            CheckRunner();
            if (EFSSystem.Runner != null)
            {
                EFSSystem.Runner.StepBack();
                GUIUtils.MDIWindow.RefreshAfterStep();
            }
        }

        private void testCaseSelectorComboBox_SelectionChanged(object sender, EventArgs e)
        {
            Runner runner = EFSSystem.Runner;
            if (runner != null &&
                (runner.SubSequence == null || runner.SubSequence.Name.CompareTo(subSequenceSelectorComboBox.Text) != 0))
            {
                EFSSystem.Runner = null;
            }
            Refresh();
        }

        /// <summary>
        ///     Refreshes the model of the window
        /// </summary>
        public override void RefreshModel()
        {
            testBrowserTreeView.RefreshModel();
        }

        /// <summary>
        ///     Selects the current step by clicking on the label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripLabel4_Click(object sender, EventArgs e)
        {
            if (EFSSystem.Runner != null)
            {
                Step step = EFSSystem.Runner.CurrentStep();
                if (step != null)
                {
                    GUIUtils.MDIWindow.Select(step);
                }
            }
        }

        /// <summary>
        ///     Selects the current test sequence by clicking on the label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            if (EFSSystem.Runner != null)
            {
                SubSequence subSequence = EFSSystem.Runner.SubSequence;
                if (subSequence != null)
                {
                    GUIUtils.MDIWindow.Select(subSequence);
                }
            }
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
        ///     Selects the next node where warning message is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextWarningToolStripButton_Click(object sender, EventArgs e)
        {
            TreeView.SelectNext(ElementLog.LevelEnum.Warning);
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
        ///     The frame currently selected
        /// </summary>
        private Frame Frame { get; set; }

        private void frameSelectorComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (Frame == null || Frame.Name.CompareTo(frameToolStripComboBox.Text) != 0)
            {
                EFSSystem.Runner = null;
            }
            Refresh();
        }

        public void RefreshAfterStep()
        {
            Refresh();
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

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Runner runner = EFSSystem.Runner;
            if (runner != null)
            {
                runner.PleaseWait = !runner.PleaseWait;
            }
        }
    }
}