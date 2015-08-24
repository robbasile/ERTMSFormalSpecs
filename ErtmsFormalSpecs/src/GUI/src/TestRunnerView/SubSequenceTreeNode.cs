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
using System.ComponentModel;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Tests;
using DataDictionary.Tests.Runner;
using GUI.LongOperations;
using GUI.Properties;
using GUI.Report;
using GUIUtils;
using Utils;
using Action = DataDictionary.Rules.Action;
using ModelElement = Utils.ModelElement;
using Util = DataDictionary.Util;

namespace GUI.TestRunnerView
{
    public class SubSequenceTreeNode : ModelElementTreeNode<SubSequence>
    {
        /// <summary>
        ///     The value editor
        /// </summary>
        private class ItemEditor : CommentableEditor
        {
            [Category("Process")]
            [DisplayName("Completed")]
            [Description("This flag indicates that the sequence is complete and can be executed during nightbuild")]
            // ReSharper disable once UnusedMember.Local
            public Boolean Completed
            {
                get { return Item.getCompleted(); }
                set { Item.setCompleted(value); }
            }

            [Category("Subset-076 Description")]
            [DisplayName("D_LRBG")]
            // ReSharper disable once UnusedMember.Local
            public string DLrbg
            {
                get { return Item.getD_LRBG(); }
            }

            [Category("Subset-076 Description")]
            [DisplayName("Level")]
            // ReSharper disable once UnusedMember.Local
            public string Level
            {
                get { return Item.getLevel(); }
            }

            [Category("Subset-076 Description")]
            [DisplayName("Mode")]
            // ReSharper disable once UnusedMember.Local
            public string Mode
            {
                get { return Item.getMode(); }
            }

            [Category("Subset-076 Description")]
            [DisplayName("NID_LRBG")]
            // ReSharper disable once UnusedMember.Local
            public string NidLrbg
            {
                get { return Item.getNID_LRBG(); }
            }

            [Category("Subset-076 Description")]
            [DisplayName("Q_DIRLRBG")]
            // ReSharper disable once UnusedMember.Local
            public string QDirlrbg
            {
                get { return Item.getQ_DIRLRBG(); }
            }

            [Category("Subset-076 Description")]
            [DisplayName("Q_DIRTRAIN")]
            // ReSharper disable once UnusedMember.Local
            public string QDirtrain
            {
                get { return Item.getQ_DIRTRAIN(); }
            }

            [Category("Subset-076 Description")]
            [DisplayName("Q_DLRBG")]
            // ReSharper disable once UnusedMember.Local
            public string QDlrbg
            {
                get { return Item.getQ_DLRBG(); }
            }

            [Category("Subset-076 Description")]
            [DisplayName("RBC_Phone")]
            // ReSharper disable once UnusedMember.Local
            public string RbcPhone
            {
                get { return Item.getRBCPhone(); }
            }

            [Category("Subset-076 Description")]
            [DisplayName("RBC_ID")]
            // ReSharper disable once UnusedMember.Local
            public string RbcId
            {
                get { return Item.getRBC_ID(); }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public SubSequenceTreeNode(SubSequence item, bool buildSubNodes)
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

            foreach (TestCase testCase in Item.TestCases)
            {
                subNodes.Add(new TestCaseTreeNode(testCase, recursive));
            }
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        #region Apply translation rules

        private class ApplyTranslationRulesHandler : ProgressHandler
        {
            /// <summary>
            ///     The subsequence on which the translation rules should be applied
            /// </summary>
            private SubSequence SubSequence { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="subSequence"></param>
            public ApplyTranslationRulesHandler(SubSequence subSequence)
            {
                SubSequence = subSequence;
            }

            /// <summary>
            ///     Generates the file in the background thread
            /// </summary>
            public override void ExecuteWork()
            {
                FinderRepository.INSTANCE.ClearCache();
                SubSequence.Translate(SubSequence.Dictionary.TranslationDictionary);
                EFSSystem.INSTANCE.Context.HandleChangeEvent(SubSequence, Context.ChangeKind.Translation);
            }
        }

        #endregion

        /// <summary>
        ///     Translates the corresponding sub sequence, according to translation rules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void TranslateHandler(object sender, EventArgs args)
        {
            ApplyTranslationRulesHandler applyTranslationRulesHandler = new ApplyTranslationRulesHandler(Item);
            ProgressDialog progress = new ProgressDialog("Applying translation rules", applyTranslationRulesHandler);
            progress.ShowDialog(GuiUtils.MdiWindow);
        }

        public void AddHandler(object sender, EventArgs args)
        {
            Item.appendTestCases(TestCase.CreateDefault(Item.TestCases));
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
            private SubSequence SubSequence { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="window"></param>
            /// <param name="subSequence"></param>
            public ExecuteTestsHandler(Window window, SubSequence subSequence)
            {
                Window = window;
                SubSequence = subSequence;
            }

            /// <summary>
            ///     Executes the tests in the background thread
            /// </summary>
            public override void ExecuteWork()
            {
                if (Window != null)
                {
                    Window.SetSubSequence(SubSequence);
                    EFSSystem.INSTANCE.Runner = new Runner(SubSequence, true, false, true);
                    EFSSystem.INSTANCE.Runner.RunUntilStep(null);
                }
            }
        }

        /// <summary>
        ///     Handles a run event on this test case
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RunHandler(object sender, EventArgs args)
        {
            ClearMessages();

            ExecuteTestsHandler executeTestHandler = new ExecuteTestsHandler(BaseForm as Window, Item);
            executeTestHandler.ExecuteUsingProgressDialog("Executing test steps");

            Window window = BaseForm as Window;
            if (window != null)
            {
                window.tabControl1.SelectedTab = window.testExecutionTabPage;
            }

            string runtimeErrors = "Succesful sub sequence execution.\n";
            Util.IsThereAnyError isThereAnyError = new Util.IsThereAnyError();
            if (isThereAnyError.ErrorsFound.Count > 0)
            {
                runtimeErrors = "Errors were raised while executing sub sequence.\n";
            }

            if (!executeTestHandler.Dialog.Canceled)
            {
                MessageBox.Show(
                    Resources.SubSequenceTreeNode_RunHandler_ + runtimeErrors +
                    Resources.SubSequenceTreeNode_RunHandler_Test_duration___ +
                    Math.Round(executeTestHandler.Span.TotalSeconds) + Resources.SubSequenceTreeNode_RunHandler__seconds,
                    Resources.SubSequenceTreeNode_RunHandler_Execution_report);
            }

            EFSSystem.INSTANCE.Context.HandleEndOfCycle();
        }

        #endregion

        /// <summary>
        ///     Handles a run event on sub sequence case and creates the associated report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ReportHandler(object sender, EventArgs args)
        {
            TestReport aReport = new TestReport(Item);
            aReport.Show();
        }

        public void InsertTest(object sender, EventArgs args)
        {
            TextEntry dataEntry = new TextEntry();
            dataEntry.ShowDialog();

            TestCase driverId = null;
            SubSequence driverIdSubSequence = Item.Frame.findSubSequence("IN Driver id");
            foreach (TestCase testCase in driverIdSubSequence.TestCases)
            {
                if (testCase.Name == "IN Driver Id")
                {
                    driverId = testCase;
                    break;
                }
            }

            if (driverId != null)
            {
                TestCase duplicate = driverId.Duplicate() as TestCase;
                if (duplicate != null)
                {
                    duplicate.Name = "IN " + dataEntry.Value;
                    foreach (Step step in duplicate.Steps)
                    {
                        foreach (SubStep subStep in step.SubSteps)
                        {
                            foreach (Action action in subStep.Actions)
                            {
                                action.ExpressionText = action.ExpressionText.Replace("DriverId", dataEntry.Value);
                            }
                            foreach (Expectation expectation in subStep.Expectations)
                            {
                                expectation.ExpressionText = expectation.ExpressionText.Replace("DriverId",
                                    dataEntry.Value);
                            }
                        }
                    }
                }

                Item.appendTestCases(duplicate);
            }
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>
            {
                new MenuItem("Add test case", AddHandler),
                new MenuItem("Delete", DeleteHandler)
            };

            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(6, new MenuItem("Apply translation rules", TranslateHandler));
            retVal.Insert(7, new MenuItem("-"));
            retVal.Insert(8, new MenuItem("Execute", RunHandler));
            retVal.Insert(9, new MenuItem("Create report", ReportHandler));
            retVal.Insert(10, new MenuItem("-"));
            retVal.Insert(9, new MenuItem("Insert test", InsertTest));

            return retVal;
        }

        /// <summary>
        ///     Handles the drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);
            if (sourceNode is TestCaseTreeNode)
            {
                TestCaseTreeNode testCase = sourceNode as TestCaseTreeNode;
                testCase.Delete();

                Item.appendTestCases(testCase.Item);
            }
        }
    }
}