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
using DataDictionary.Tests;
using DataDictionary.Tests.Runner;
using GUI.IPCInterface;
using GUI.LongOperations;
using GUI.Properties;
using Utils;
using Step = DataDictionary.Tests.Step;
using Util = DataDictionary.Util;

namespace GUI.TestRunnerView
{
    public partial class Window : BaseForm
    {
        public override BaseTreeView TreeView
        {
            get { return testBrowserTreeView; }
        }

        /// <summary>
        ///     The data dictionary for this view
        /// </summary>
        private EfsSystem _efsSystem;

        public EfsSystem EfsSystem
        {
            get { return _efsSystem; }
            private set
            {
                _efsSystem = value;
                testBrowserTreeView.Root = _efsSystem;
            }
        }

        /// <summary>
        ///     The runner
        /// </summary>
        public Runner GetRunner(SubSequence subSequence)
        {
            Runner runner = EfsSystem.Runner;

            if (runner == null || runner.SubSequence != subSequence)
            {
                if (subSequence != null)
                {
                    EfsSystem.Runner = new Runner(subSequence, true, true, Settings.Default.CheckForCompatibleChanges);
                }
            }

            return EfsSystem.Runner;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            frameToolStripComboBox.DropDown += frameToolStripComboBox_DropDown;
            subSequenceSelectorComboBox.DropDown += subSequenceSelectorComboBox_DropDown;
            Text = Resources.Window_Window_System_test_view;
            EfsSystem = EfsSystem.Instance;
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
        ///     Indicates that a refresh is ongoing
        /// </summary>
        private bool DoingRefresh { get; set; }

        /// <summary>
        ///     Sets the current frame parameters
        /// </summary>
        /// <param name="frame"></param>
        public void SetFrame(Frame frame)
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
        public void SetSubSequence(SubSequence subSequence)
        {
            Invoke((MethodInvoker) delegate
            {
                subSequenceSelectorComboBox.Text = subSequence.Name;
                SetFrame(subSequence.Frame);
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
                    if (EfsSystem.Runner == null)
                    {
                        toolStripTimeTextBox.Text = @"0";
                        toolStripCurrentStepTextBox.Text = Resources.Window_Refresh__none_;
                    }
                    else
                    {
                        toolStripTimeTextBox.Text = "" + EfsSystem.Runner.Time;
                        Step currentStep = EfsSystem.Runner.CurrentStep();
                        if (currentStep != null)
                        {
                            toolStripCurrentStepTextBox.Text = currentStep.Name;
                        }
                        else
                        {
                            toolStripCurrentStepTextBox.Text = Resources.Window_Refresh__none_;
                        }

                        if (EfsSystem.Runner.SubSequence != null && EfsSystem.Runner.SubSequence.Frame != null)
                        {
                            Frame = EfsSystem.Runner.SubSequence.Frame;
                            selectedFrame = EfsSystem.Runner.SubSequence.Frame.Name;
                        }
                    }

                    testBrowserTreeView.Refresh();
                    testDescriptionTimeLineControl.Refresh();
                    testExecutionTimeLineControl.Refresh();

                    RebuildFramesComboBox();
                    frameToolStripComboBox.Text = selectedFrame;
                    frameToolStripComboBox.ToolTipText = selectedFrame;

                    if (Frame == null || !frameToolStripComboBox.Text.Equals(Frame.Name))
                    {
                        RebuildSubSequencesComboBox();

                        if (EfsSystem.Runner != null && EfsSystem.Runner.SubSequence != null)
                        {
                            EfsSystem.Runner = null;
                        }
                    }

                    if (EfsSystem.Runner != null && EfsSystem.Runner.SubSequence != null)
                    {
                        subSequenceSelectorComboBox.Text = EfsSystem.Runner.SubSequence.Name;
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
            foreach (Dictionary dictionary in EfsSystem.Dictionaries)
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
            foreach (Dictionary dictionary in EfsSystem.Dictionaries)
            {
                Frame = dictionary.FindFrame(frameToolStripComboBox.Text);
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
                if (EfsSystem.Runner != null)
                {
                    EfsSystem.Runner.StepOnce();
                    EfsSystem.Instance.Context.HandleEndOfCycle();
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
            if (EfsSystem.Runner != null)
            {
                EfsSystem.Runner.EndExecution();
                EfsSystem.Runner = null;
            }
            Clear();
            EfsSystem.Instance.Context.HandleEndOfCycle();
            RefreshModel.Execute();
            tabControl1.SelectedTab = testExecutionTabPage;
        }

        public void Clear()
        {
            EfsSystem.Runner = null;
            EfsSystem.ClearMessages(false);
        }

        /// <summary>
        ///     Ensures that the runner is not empty
        /// </summary>
        private void CheckRunner()
        {
            if (EfsSystem.Runner == null)
            {
                if (Frame != null)
                {
                    SubSequence subSequence = Frame.findSubSequence(subSequenceSelectorComboBox.Text);
                    if (subSequence != null)
                    {
                        EfsSystem.Runner = new Runner(subSequence, true, true, Settings.Default.CheckForCompatibleChanges);
                    }
                }
                else
                {
                    EfsSystem.Runner = EFSService.Instance.Runner;
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
            if (EfsSystem.Runner != null)
            {
                EfsSystem.Runner.StepBack();
                EfsSystem.Instance.Context.HandleEndOfCycle();
            }
        }

        private void testCaseSelectorComboBox_SelectionChanged(object sender, EventArgs e)
        {
            Runner runner = EfsSystem.Runner;
            if (runner != null &&
                (runner.SubSequence == null || !runner.SubSequence.Name.Equals(subSequenceSelectorComboBox.Text)))
            {
                EfsSystem.Runner = null;
            }
            Refresh();
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
                testBrowserTreeView.RefreshModel(modelElement);
                Refresh();
            }

            if (testDescriptionTimeLineControl.ShouldDisplayModelElement(modelElement))
            {
                testDescriptionTimeLineControl.Refresh();
            }
            if (testExecutionTimeLineControl.ShouldDisplayModelElement(modelElement))
            {
                testExecutionTimeLineControl.Refresh();
            }

            if (changeKind == Context.ChangeKind.EndOfCycle)
            {
                tabControl1.SelectedTab = testExecutionTabPage;                
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
                SubSequence subSequence = DisplayedModel as SubSequence;
                if (subSequence != null)
                {
                    testDescriptionTimeLineControl.SubSequence = subSequence;
                    testDescriptionTimeLineControl.Refresh();
                    tabControl1.SelectedTab = testDescriptionTabPage;
                }

                TestCase testCase = DisplayedModel as TestCase;
                if (testCase != null)
                {
                    testDescriptionTimeLineControl.TestCase = testCase;
                    testDescriptionTimeLineControl.Refresh();
                    tabControl1.SelectedTab = testDescriptionTabPage;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Selects the current step by clicking on the label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripLabel4_Click(object sender, EventArgs e)
        {
            if (EfsSystem.Runner != null)
            {
                Step step = EfsSystem.Runner.CurrentStep();
                if (step != null)
                {
                    EfsSystem.Instance.Context.SelectElement(step, this, Context.SelectionCriteria.DoubleClick);
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
            if (EfsSystem.Runner != null)
            {
                SubSequence subSequence = EfsSystem.Runner.SubSequence;
                if (subSequence != null)
                {
                    EfsSystem.Instance.Context.SelectElement(subSequence, this, Context.SelectionCriteria.DoubleClick);
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
            if (Frame == null || !Frame.Name.Equals(frameToolStripComboBox.Text))
            {
                EfsSystem.Runner = null;
            }
            Refresh();
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

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Runner runner = EfsSystem.Runner;
            if (runner != null)
            {
                runner.PleaseWait = !runner.PleaseWait;
            }
        }
    }
}