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
using GUI.Report;

namespace GUI.TestRunnerView
{
    public class TestsTreeNode : ModelElementTreeNode<Dictionary>
    {
        private class ItemEditor : NamedEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public TestsTreeNode(Dictionary item, bool buildSubNodes)
            : base(item, buildSubNodes, null, true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (Frame frame in Item.Tests)
            {
                Nodes.Add(new FrameTreeNode(frame, buildSubNodes));
            }
            SortSubNodes();
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        /// <summary>
        ///     Creates a new frame
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FrameTreeNode createFrame(string name)
        {
            FrameTreeNode retVal;

            Frame frame = Frame.createDefault(name);
            Item.appendTests(frame);

            retVal = new FrameTreeNode(frame, true);
            Nodes.Add(retVal);
            SortSubNodes();

            return retVal;
        }

        public void AddHandler(object sender, EventArgs args)
        {
            createFrame("Frame" + (GetNodeCount(false) + 1));
        }

        private void ClearAll()
        {
            TestTreeView treeView = TreeView as TestTreeView;
            if (treeView != null)
            {
                Window window = treeView.ParentForm as Window;
                window.Clear();
            }
        }

        #region Execute tests

        private class ExecuteTestsHandler : BaseLongOperation
        {
            /// <summary>
            ///     The window for which theses tests should be executed
            /// </summary>
            private Window Window { get; set; }

            /// <summary>
            ///     The subsequence which should be executed
            /// </summary>
            private Dictionary Dictionary { get; set; }

            /// <summary>
            ///     The EFS system
            /// </summary>
            private EFSSystem EFSSystem
            {
                get { return Dictionary.EFSSystem; }
            }

            /// <summary>
            ///     The number of failed tests
            /// </summary>
            public int Failed { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="window"></param>
            /// <param name="dictionary"></param>
            public ExecuteTestsHandler(Window window, Dictionary dictionary)
            {
                Window = window;
                Dictionary = dictionary;
            }

            /// <summary>
            ///     Executes the tests in the background thread
            /// </summary>
            /// <param name="arg"></param>
            public override void ExecuteWork()
            {
                DateTime start = DateTime.Now;

                SynchronizerList.SuspendSynchronization();

                // Compile everything
                EFSSystem.Compiler.Compile_Synchronous(EFSSystem.ShouldRebuild);
                EFSSystem.ShouldRebuild = false;

                Failed = 0;
                ArrayList tests = Dictionary.Tests;
                tests.Sort();
                foreach (Frame frame in tests)
                {
                    Dialog.UpdateMessage("Executing " + frame.Name);

                    const bool ensureCompilationDone = false;
                    int failedFrames = frame.ExecuteAllTests(ensureCompilationDone);
                    if (failedFrames > 0)
                    {
                        Failed += 1;
                    }
                }
                Dictionary.EFSSystem.Runner = null;
                SynchronizerList.ResumeSynchronization();

                Span = DateTime.Now.Subtract(start);
            }
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

            ExecuteTestsHandler executeTestsHandler = new ExecuteTestsHandler(BaseForm as Window, Item);
            executeTestsHandler.ExecuteUsingProgressDialog("Executing test frames");

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
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add frame", new EventHandler(AddHandler)));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Import braking curves verification set",
                new EventHandler(ImportBrakingCurvesHandler)));
            retVal.Add(new MenuItem("Mark as not translatable", new EventHandler(DoNotTranslateHandler)));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Execute", new EventHandler(RunHandler)));
            retVal.Add(new MenuItem("Create report", new EventHandler(ReportHandler)));

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
                Frm_ExcelImport excelImport = new Frm_ExcelImport(this.Item);
                excelImport.ShowDialog();
                GUIUtils.MDIWindow.RefreshModel();
            }
        }

        /// <summary>
        ///     Display the number of frames
        /// </summary>
        /// <param name="displayStatistics"></param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(false);

            GUIUtils.MDIWindow.SetStatus(Item.Tests.Count + " test frames");
        }
    }
}