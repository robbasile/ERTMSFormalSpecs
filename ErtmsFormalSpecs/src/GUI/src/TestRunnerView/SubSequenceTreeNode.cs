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
using GUI.Report;
using Utils;
using Action = DataDictionary.Rules.Action;
using ModelElement = Utils.ModelElement;

namespace GUI.TestRunnerView
{
    public class SubSequenceTreeNode : ModelElementTreeNode<SubSequence>
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

            [Category("Process"),
             Description("This flag indicates that the sequence is complete and can be executed during nightbuild")]
            public Boolean Completed
            {
                get { return Item.getCompleted(); }
                set { Item.setCompleted(value); }
            }

            [Category("Subset-076 Description")]
            public string D_LRBG
            {
                get { return Item.getD_LRBG(); }
            }

            [Category("Subset-076 Description")]
            public string Level
            {
                get { return Item.getLevel(); }
            }

            [Category("Subset-076 Description")]
            public string Mode
            {
                get { return Item.getMode(); }
            }

            [Category("Subset-076 Description")]
            public string NID_LRBG
            {
                get { return Item.getNID_LRBG(); }
            }

            [Category("Subset-076 Description")]
            public string Q_DIRLRBG
            {
                get { return Item.getQ_DIRLRBG(); }
            }

            [Category("Subset-076 Description")]
            public string Q_DIRTRAIN
            {
                get { return Item.getQ_DIRTRAIN(); }
            }

            [Category("Subset-076 Description")]
            public string Q_DLRBG
            {
                get { return Item.getQ_DLRBG(); }
            }

            [Category("Subset-076 Description")]
            public string RBC_Phone
            {
                get { return Item.getRBCPhone(); }
            }

            [Category("Subset-076 Description")]
            public string RBC_ID
            {
                get { return Item.getRBC_ID(); }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public SubSequenceTreeNode(SubSequence item, bool buildSubNodes)
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

            foreach (TestCase testCase in Item.TestCases)
            {
                Nodes.Add(new TestCaseTreeNode(testCase, buildSubNodes));
            }
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
        ///     Creates a new test case (e.g. feature + test case)
        /// </summary>
        /// <param name="testCase"></param>
        /// <returns></returns>
        public TestCaseTreeNode createTestCase(TestCase testCase)
        {
            TestCaseTreeNode retVal = new TestCaseTreeNode(testCase, true);

            Item.appendTestCases(testCase);
            Nodes.Add(retVal);

            return retVal;
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
            /// <param name="arg"></param>
            public override void ExecuteWork()
            {
                FinderRepository.INSTANCE.ClearCache();
                SubSequence.Translate(SubSequence.Dictionary.TranslationDictionary);
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
            progress.ShowDialog(GUIUtils.MDIWindow);
            GUIUtils.MDIWindow.RefreshModel();
        }

        public void AddHandler(object sender, EventArgs args)
        {
            TestCase testCase = TestCase.createDefault("Test case" + (Item.TestCases.Count + 1));
            testCase.Enclosing = Item;

            createTestCase(testCase);
        }

        /// <summary>
        ///     Provides the EFS System in which this element belongs
        /// </summary>
        public EFSSystem EFSSystem
        {
            get { return EnclosingFinder<EFSSystem>.find(Item); }
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
            ///     The EFS system
            /// </summary>
            private EFSSystem EFSSystem
            {
                get { return SubSequence.EFSSystem; }
            }

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
            /// <param name="arg"></param>
            public override void ExecuteWork()
            {
                if (Window != null)
                {
                    Window.setSubSequence(SubSequence);
                    EFSSystem.Runner = new Runner(SubSequence, true, false, true);
                    EFSSystem.Runner.RunUntilStep(null);
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
            ModelElement.LogCount = 0;

            ExecuteTestsHandler executeTestHandler = new ExecuteTestsHandler(BaseForm as Window, Item);
            executeTestHandler.ExecuteUsingProgressDialog("Executing test steps");

            Window window = BaseForm as Window;
            if (window != null)
            {
                window.tabControl1.SelectedTab = window.testExecutionTabPage;
            }

            string runtimeErrors = "Succesful sub sequence execution.\n";
            if (ModelElement.LogCount > 0)
            {
                runtimeErrors = "Errors were raised while executing sub sequence.\n";
            }

            if (!executeTestHandler.Dialog.Canceled)
            {
                MessageBox.Show(
                    "Sub sequence execution report.\n" + runtimeErrors + "Test duration : " +
                    Math.Round(executeTestHandler.Span.TotalSeconds) + " seconds", "Execution report");
            }

            GUIUtils.MDIWindow.RefreshAfterStep();
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

            TestCase duplicate = driverId.Duplicate() as TestCase;
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
                        expectation.ExpressionText = expectation.ExpressionText.Replace("DriverId", dataEntry.Value);
                    }
                }
            }

            createTestCase(duplicate);
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add test case", new EventHandler(AddHandler)));
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(6, new MenuItem("Apply translation rules", new EventHandler(TranslateHandler)));
            retVal.Insert(7, new MenuItem("-"));
            retVal.Insert(8, new MenuItem("Execute", new EventHandler(RunHandler)));
            retVal.Insert(9, new MenuItem("Create report", new EventHandler(ReportHandler)));
            retVal.Insert(10, new MenuItem("-"));
            retVal.Insert(9, new MenuItem("Insert test", new EventHandler(InsertTest)));

            return retVal;
        }

        /// <summary>
        ///     Handles the drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);
            if (SourceNode is TestCaseTreeNode)
            {
                TestCaseTreeNode testCase = SourceNode as TestCaseTreeNode;
                testCase.Delete();

                createTestCase(testCase.Item);
            }
        }

        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(displayStatistics);

            Window window = BaseForm as Window;
            if (window != null)
            {
                window.testDescriptionTimeLineControl.SubSequence = Item;
                window.testDescriptionTimeLineControl.Refresh();
            }
        }
    }
}