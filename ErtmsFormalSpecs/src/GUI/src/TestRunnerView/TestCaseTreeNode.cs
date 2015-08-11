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
using DataDictionary.Tests.Runner;
using GUI.Report;
using GUIUtils;
using Utils;
using Step = DataDictionary.Tests.Step;
using SubSequence = DataDictionary.Tests.SubSequence;
using TestCase = DataDictionary.Tests.TestCase;

namespace GUI.TestRunnerView
{
    public class TestCaseTreeNode : ReqRelatedTreeNode<TestCase>
    {
        /// <summary>
        ///     The value editor
        /// </summary>
        private class ItemEditor : ReqRelatedEditor
        {
            /// <summary>
            ///     The item name
            /// </summary>
            [Category("Description")]
            public override string Name
            {
                get { return Item.Name; }
                set
                {
                    if (Item.getFeature() == 0 && Item.getCase() == 0)
                    {
                        base.Name = value;
                    }

                    if (Item.getFeature() == 9999 && Item.getCase() == 9999)
                    {
                        base.Name = value;
                    }
                }
            }

            /// <summary>
            ///     The item feature
            /// </summary>
            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public int Feature
            {
                get { return Item.getFeature(); }
                set { Item.setFeature(value); }
            }

            /// <summary>
            ///     The item test case
            /// </summary>
            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public int TestCase
            {
                get { return Item.getCase(); }
                set { Item.setCase(value); }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public TestCaseTreeNode(TestCase item, bool buildSubNodes)
            : base(item, buildSubNodes)
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

            subNodes.Add(new StepsTreeNode(Item, recursive));
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
        ///     Ensures that the runner corresponds to test sequence
        /// </summary>
        private void CheckRunner()
        {
            Window window = BaseForm as Window;
            if (window != null && Item.EFSSystem.Runner != null && Item.EFSSystem.Runner.SubSequence != Item.SubSequence)
            {
                window.Clear();
            }
        }

        /// <summary>
        ///     Translates the corresponding test case, according to translation rules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void TranslateHandler(object sender, EventArgs args)
        {
            FinderRepository.INSTANCE.ClearCache();
            Item.Translate(Item.Dictionary.TranslationDictionary);
            EFSSystem.INSTANCE.Context.HandleChangeEvent(Item, Context.ChangeKind.Translation);
        }

        #region Execute tests

        private class ExecuteTestsHandler : ProgressHandler
        {
            /// <summary>
            ///     The window for which theses tests should be executed
            /// </summary>
            private Window Window { get; set; }

            /// <summary>
            ///     The subsequence which should be executed
            /// </summary>
            private TestCase TestCase { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="window"></param>
            /// <param name="testCase"></param>
            public ExecuteTestsHandler(Window window, TestCase testCase)
            {
                Window = window;
                TestCase = testCase;
            }

            /// <summary>
            ///     Executes the tests in the background thread
            /// </summary>
            public override void ExecuteWork()
            {
                if (Window != null)
                {
                    SynchronizerList.SuspendSynchronization();
                    SubSequence subSequence = TestCase.Enclosing as SubSequence;
                    if (subSequence != null && TestCase.Steps.Count > 0)
                    {
                        Step step = null;
                        bool found = false;
                        foreach (TestCase current in subSequence.TestCases)
                        {
                            if (found && current.Steps.Count > 0)
                            {
                                step = (Step) current.Steps[0];
                                break;
                            }

                            found = (current == TestCase);
                        }

                        Runner runner = Window.GetRunner(subSequence);
                        runner.RunUntilStep(step);
                    }
                    SynchronizerList.ResumeSynchronization();
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
            CheckRunner();
            ClearMessages();

            ExecuteTestsHandler executeTestsHandler = new ExecuteTestsHandler(BaseForm as Window, Item);
            ProgressDialog dialog = new ProgressDialog("Executing test steps", executeTestsHandler);
            dialog.ShowDialog();

            EFSSystem.INSTANCE.Context.HandleChangeEvent(null);
        }

        #endregion

        /// <summary>
        ///     Handles a run event on this test case and creates the associated report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ReportHandler(object sender, EventArgs args)
        {
            TestReport aReport = new TestReport(Item);
            aReport.Show();
        }
        
        public void AddHandler(object sender, EventArgs args)
        {
            Item.appendSteps(Step.CreateDefault(Item.Steps));
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>
            {
                new MenuItem("Add step", AddHandler),
                new MenuItem("Delete", DeleteHandler)
            };

            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(7, new MenuItem("Apply translation rules", TranslateHandler));
            retVal.Insert(8, new MenuItem("-"));

           retVal.Insert(11, new MenuItem("Execute", RunHandler));
            retVal.Insert(12, new MenuItem("Create report", ReportHandler));
            retVal.Insert(13, new MenuItem("-"));

            return retVal;
        }
    }
}