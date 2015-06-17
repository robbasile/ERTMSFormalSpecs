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
using System.ComponentModel;
using System.Windows.Forms;
using DataDictionary.Tests;
using DataDictionary.Tests.Runner;
using GUI.LongOperations;
using GUI.Report;
using Utils;

namespace GUI.TestRunnerView
{
    public class FrameTreeNode : ModelElementTreeNode<Frame>
    {
        /// <summary>
        ///     The value editor
        /// </summary>
        private class ItemEditor : CommentableEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            /// <summary>
            ///     The variable that identifies the time in the current system
            /// </summary>
            [Category("Description")]
            public string CycleDuration
            {
                get { return Item.getCycleDuration(); }
                set { Item.setCycleDuration(value); }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public FrameTreeNode(Frame item, bool buildSubNodes)
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

            foreach (SubSequence subSequence in Item.SubSequences)
            {
                Nodes.Add(new SubSequenceTreeNode(subSequence, buildSubNodes));
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
        ///     Creates a new sub sequence in the corresponding frame
        /// </summary>
        /// <param name="subSequenceName"></param>
        /// <returns></returns>
        public SubSequenceTreeNode createSubSequence(SubSequence subSequence)
        {
            SubSequenceTreeNode retVal;

            subSequence.Enclosing = Item;
            Item.appendSubSequences(subSequence);

            retVal = new SubSequenceTreeNode(subSequence, true);
            Nodes.Add(retVal);
            SortSubNodes();

            return retVal;
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
            progress.ShowDialog(GUIUtils.MDIWindow);
            GUIUtils.MDIWindow.RefreshModel();
        }

        #region Apply rules

        private class ApplyRulesOperation : ProgressHandler
        {
            /// <summary>
            ///     The frams on which the rules should be applied
            /// </summary>
            private Frame Frame { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="frame"></param>
            public ApplyRulesOperation(Frame frame)
            {
                Frame = frame;
            }

            /// <summary>
            ///     Perform the work as a background task
            /// </summary>
            public override void ExecuteWork()
            {
                FinderRepository.INSTANCE.ClearCache();
                Frame.Translate(Frame.Dictionary.TranslationDictionary);
            }
        }

        #endregion

        public void AddHandler(object sender, EventArgs args)
        {
            createSubSequence(SubSequence.createDefault("Sequence" + (GetNodeCount(false) + 1)));
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

        #region ExecuteTests

        private class ExecuteTestsOperation : BaseLongOperation
        {
            /// <summary>
            ///     The number of failed tests
            /// </summary>
            public int Failed { get; private set; }

            /// <summary>
            ///     The window in which the tests are executed
            /// </summary>
            private Window Window { get; set; }

            /// <summary>
            ///     The frame to test
            /// </summary>
            private Frame Frame { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="window"></param>
            /// <param name="frame"></param>
            public ExecuteTestsOperation(Window window, Frame frame)
            {
                Window = window;
                Frame = frame;
            }

            /// <summary>
            ///     Executes the work in the background task
            /// </summary>
            /// <param name="arg"></param>
            public override void ExecuteWork()
            {
                SynchronizerList.SuspendSynchronization();
                if (Window != null)
                {
                    Window.setFrame(Frame);

                    try
                    {
                        // Compile everything
                        Frame.EFSSystem.Compiler.Compile_Synchronous(Frame.EFSSystem.ShouldRebuild);
                        Frame.EFSSystem.ShouldRebuild = false;

                        Failed = 0;
                        ArrayList subSequences = Frame.SubSequences;
                        subSequences.Sort();
                        foreach (SubSequence subSequence in subSequences)
                        {
                            Dialog.UpdateMessage("Executing " + subSequence.Name);

                            const bool explain = false;
                            const bool logEvents = false;
                            const bool ensureCompiled = false;
                            Frame.EFSSystem.Runner = new Runner(subSequence, explain, logEvents, ensureCompiled);

                            int testCasesFailed = subSequence.ExecuteAllTestCases(Frame.EFSSystem.Runner);
                            if (testCasesFailed > 0)
                            {
                                subSequence.AddError("Execution failed");
                                Failed += 1;
                            }
                        }
                    }
                    finally
                    {
                        Frame.EFSSystem.Runner = null;
                    }
                }
                SynchronizerList.ResumeSynchronization();
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
            ModelElement.LogCount = 0;

            ExecuteTestsOperation executeTestsOperation = new ExecuteTestsOperation(BaseForm as Window, Item);
            executeTestsOperation.ExecuteUsingProgressDialog("Executing test sequences");

            string runtimeErrors = "";
            if (ModelElement.LogCount > 0)
            {
                runtimeErrors += "Errors were raised while executing sub sequences(s).\n";
            }

            if (!executeTestsOperation.Dialog.Canceled)
            {
                MessageBox.Show(
                    Item.SubSequences.Count + " sub sequence(s) executed, " + executeTestsOperation.Failed +
                    " sub sequence(s) failed.\n" + runtimeErrors + "Test duration : " +
                    Math.Round(executeTestsOperation.Span.TotalSeconds) + " seconds", "Execution report");
            }
        }

        #endregion

        /// <summary>
        ///     Handles a run event on this frame and creates the associated report
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

            retVal.Add(new MenuItem("Add sub-sequence", new EventHandler(AddHandler)));
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(5, new MenuItem("-"));
            retVal.Insert(6, new MenuItem("Apply translation rules", new EventHandler(TranslateHandler)));
            retVal.Insert(7, new MenuItem("-"));
            retVal.Insert(8, new MenuItem("Execute", new EventHandler(RunHandler)));
            retVal.Insert(9, new MenuItem("Create report", new EventHandler(ReportHandler)));

            return retVal;
        }

        /// <summary>
        ///     Finds the subsequence tree node which corresponds to the name provided
        /// </summary>
        /// <param name="subSequenceName"></param>
        /// <returns></returns>
        public SubSequenceTreeNode findSubSequence(string subSequenceName)
        {
            return findSubNode(subSequenceName) as SubSequenceTreeNode;
        }

        /// <summary>
        ///     Handles the drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);
            if (SourceNode is SubSequenceTreeNode)
            {
                SubSequenceTreeNode subSequence = SourceNode as SubSequenceTreeNode;
                subSequence.Delete();

                createSubSequence(subSequence.Item);
            }
        }
    }
}