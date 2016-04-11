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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Tests;
using GUI.ExcelImport;
using GUI.LongOperations;
using GUI.Properties;
using GUI.Report;
using GUIUtils;
using GUIUtils.LongOperations;
using Utils;

namespace GUI.TestRunnerView
{
    public class TestsTreeNode : ModelElementTreeNode<Dictionary>
    {
        private class ItemEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public TestsTreeNode(Dictionary item, bool buildSubNodes)
            : base(item, buildSubNodes, null, true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            base.BuildSubNodes(subNodes, recursive);

            foreach (Frame frame in Item.Tests)
            {
                subNodes.Add(new FrameTreeNode(frame, recursive));
            }
            subNodes.Sort();
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddHandler(object sender, EventArgs args)
        {
            Item.appendTests(Frame.CreateDefault(Item.Tests));
        }

        private void ClearAll()
        {
            TestTreeView treeView = TreeView as TestTreeView;
            if (treeView != null)
            {
                Window window = treeView.ParentForm as Window;
                if (window != null)
                {
                    window.Clear();
                }
            }
        }

        #region Execute tests

        private class ExecuteTestsHandler : BaseLongOperation
        {
            /// <summary>
            ///     The subsequence which should be executed
            /// </summary>
            private Dictionary Dictionary { get; set; }

            /// <summary>
            ///     The number of failed tests
            /// </summary>
            public int Failed { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="dictionary"></param>
            public ExecuteTestsHandler(Dictionary dictionary)
            {
                Dictionary = dictionary;
            }

            /// <summary>
            ///     Executes the tests in the background thread
            /// </summary>
            public override void ExecuteWork()
            {
                DateTime start = DateTime.Now;

                SynchronizerList.SuspendSynchronization();

                // Compile everything
                EfsSystem.Instance.Compiler.Compile_Synchronous(EfsSystem.Instance.ShouldRebuild);
                EfsSystem.Instance.ShouldRebuild = false;

                Failed = 0;
                ArrayList tests = Dictionary.Tests;
                tests.Sort();
                foreach (Frame frame in tests)
                {
                    Dialog.UpdateMessage("Executing " + frame.Name);

                    const bool ensureCompilationDone = false;
                    int failedFrames = frame.ExecuteAllTests(ensureCompilationDone, Settings.Default.CheckForCompatibleChanges);
                    if (failedFrames > 0)
                    {
                        Failed += 1;
                    }
                }
                EfsSystem.Instance.Runner = null;
                SynchronizerList.ResumeSynchronization();

                Span = DateTime.Now.Subtract(start);
            }
        }

        private class ApplyRulesOperation : ProgressHandler
        {
            /// <summary>
            ///     The dictionary on which the rules should be applied
            /// </summary>
            private Dictionary Dictionary{ get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="dictionary"></param>
            public ApplyRulesOperation(Dictionary dictionary)
            {
                Dictionary = dictionary;
            }

            /// <summary>
            ///     Perform the work as a background task
            /// </summary>
            public override void ExecuteWork()
            {
                MarkingHistory.PerformMark(() =>
                {
                    FinderRepository.INSTANCE.ClearCache();
                    foreach (Frame frame in Dictionary.Tests)
                    {
                        frame.Translate();
                    }
                });
                RefreshModel.Execute();
            }
        }
        /// <summary>
        ///     Translates the corresponding sub sequence, according to translation rules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void TranslateHandler(object sender, EventArgs args)
        {
            ApplyRulesOperation applyRulesOperation = new ApplyRulesOperation(Item);
            ProgressDialog progress = new ProgressDialog("Applying translation rules", applyRulesOperation);
            progress.ShowDialog(GuiUtils.MdiWindow);
        }
        
        /// <summary>
        ///     Handles a run event on this test case
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RunHandler(object sender, EventArgs args)
        {
            ClearAll();
            ClearMessages();

            ExecuteTestsHandler executeTestsHandler = new ExecuteTestsHandler(Item);
            executeTestsHandler.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Executing test frames");

            if (!executeTestsHandler.Dialog.Canceled)
            {
                MessageBox.Show(
                    Item.Tests.Count + " test frame(s) executed, " + executeTestsHandler.Failed +
                    " test frame(s) failed.\nTest duration : " + Math.Round(executeTestsHandler.Span.TotalSeconds) +
                    " seconds", "Execution report");
            }
        }

        #endregion

        /// <summary>
        ///     Handles a run event on these tests and creates the associated report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ReportHandler(object sender, EventArgs args)
        {
            TestReport aReport = new TestReport(Item);
            aReport.Show();
        }


        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>
            {
                new MenuItem("Add frame", AddHandler),
                new MenuItem("-"),
                new MenuItem("Import braking curves verification set", ImportBrakingCurvesHandler),
                new MenuItem("Mark as not translatable", DoNotTranslateHandler),
                new MenuItem("-"),
                new MenuItem("Apply translation rules", TranslateHandler),
                new MenuItem("Execute", RunHandler),
                new MenuItem("Create report", ReportHandler)
            };

            return retVal;
        }


        /// <summary>
        ///     Indicates that the steps of this frame should not be translated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DoNotTranslateHandler(object sender, EventArgs args)
        {
            foreach (Frame frame in Item.Tests)
            {
                foreach (SubSequence subSequence in frame.SubSequences)
                {
                    foreach (TestCase testCase in subSequence.TestCases)
                    {
                        foreach (Step step in testCase.Steps)
                        {
                            step.setTranslationRequired(false);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Imports a test scenario from the ERA braking curves simulation tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ImportBrakingCurvesHandler(object sender, EventArgs args)
        {
            Window window = BaseForm as Window;
            if (window != null)
            {
                Frm_ExcelImport excelImport = new Frm_ExcelImport(Item);
                excelImport.ShowDialog();
            }
        }
    }
}