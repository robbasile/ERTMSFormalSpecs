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
using DataDictionary;
using DataDictionary.Tests.Runner;
using GUI.LongOperations;
using GUI.Properties;
using GUI.Report;
using GUI.RuleDisabling;
using GUIUtils;
using GUIUtils.LongOperations;
using Utils;
using Frame = DataDictionary.Tests.Frame;
using SubSequence = DataDictionary.Tests.SubSequence;
using Util = DataDictionary.Util;

namespace GUI.TestRunnerView
{
    public class FrameTreeNode : ModelElementTreeNode<Frame>, RuleDisabling.IDisablesRules<Frame>
    {
        /// <summary>
        ///     The value editor
        /// </summary>
        private class ItemEditor : CommentableEditor
        {
            /// <summary>
            ///     The variable that identifies the time in the current system
            /// </summary>
            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public string CycleDuration
            {
                get { return Item.getCycleDuration(); }
                set { Item.setCycleDuration(value); }
            }
        }

        public DisablesRulesTreeNodeExtension<Frame> Disabling { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public FrameTreeNode(Frame item, bool buildSubNodes)
            : base(item, buildSubNodes, null, true)
        {
            Disabling = new DisablesRulesTreeNodeExtension<Frame>(item);
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            base.BuildSubNodes(subNodes, recursive);

            Disabling.BuildSubNodes(subNodes, recursive);

            foreach (SubSequence subSequence in Item.SubSequences)
            {
                subNodes.Add(new SubSequenceTreeNode(subSequence, recursive));
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
                MarkingHistory.PerformMark(() =>
                {
                    FinderRepository.INSTANCE.ClearCache();
                    Frame.Translate();
                });
                RefreshModel.Execute();
            }
        }

        #endregion

        public void AddHandler(object sender, EventArgs args)
        {
            Item.appendSubSequences(SubSequence.CreateDefault(Item.SubSequences));
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
            public override void ExecuteWork()
            {
                SynchronizerList.SuspendSynchronization();

                if (Window != null)
                {
                    Window.SetFrame(Frame);
                    MarkingHistory.PerformMark(() =>
                    {
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
                                const bool ensureCompiled = false;
                                Frame.EFSSystem.Runner = new Runner(subSequence, explain, ensureCompiled, Settings.Default.CheckForCompatibleChanges);

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
                    });
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

            ExecuteTestsOperation executeTestsOperation = new ExecuteTestsOperation(BaseForm as Window, Item);
            executeTestsOperation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Executing test sequences");

            string runtimeErrors = "";
            Util.IsThereAnyError isThereAnyError = new Util.IsThereAnyError();
            if (isThereAnyError.ErrorsFound.Count > 0)
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
            List<MenuItem> retVal = new List<MenuItem>
            {
                new MenuItem("Add sub-sequence", AddHandler),
                new MenuItem("Delete", DeleteHandler)
            };

            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(5, new MenuItem("Apply translation rules", TranslateHandler));
            retVal.Insert(6, new MenuItem("-"));
            retVal.Insert(7, new MenuItem("Execute", RunHandler));
            retVal.Insert(8, new MenuItem("Create report", ReportHandler));

            Disabling.AddMenuItems(retVal);

            return retVal;
        }

        /// <summary>
        ///     Handles the drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);
            if (sourceNode is SubSequenceTreeNode)
            {
                SubSequenceTreeNode subSequence = sourceNode as SubSequenceTreeNode;
                subSequence.Delete();
                Item.appendSubSequences(subSequence.Item);
            }
        }
    }
}