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
using GUIUtils.LongOperations;
using Utils;
using Action = DataDictionary.Rules.Action;
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

                RefreshModel.Execute();
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
                    MarkingHistory.PerformMark(() =>
                    {
                        Window.SetSubSequence(SubSequence);
                        EfsSystem.Instance.Runner = new Runner(SubSequence, true, true, Settings.Default.CheckForCompatibleChanges);
                        EfsSystem.Instance.Runner.RunUntilStep(null);
                    });
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
            executeTestHandler.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Executing test steps");

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

            EfsSystem.Instance.Context.HandleEndOfCycle();
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


        public void GenerateThings(object sender, EventArgs args)
        {
            // ajoute un test case pour chaque ligne de la table Table 6.6.3.2.3.15.1
            // structure:   Step 1 : initializeTestEnvionent
            //              Step 2 : receive balise message avec 1 packet 39 où M_TRACTION déped de la ligne
            //                          Expectation: traduction du packet est présent dans CurrentBaliseMessage
            DataDictionary.Specification.Specification spec = (DataDictionary.Specification.Specification)Item.Dictionary.Specifications[0];
            DataDictionary.Specification.Chapter chap = spec.FindChapter("6");
            DataDictionary.Specification.Paragraph tableHeader = chap.FindParagraph("6.6.3.2.3.15", false);
            tableHeader = (DataDictionary.Specification.Paragraph)tableHeader.SubParagraphs[0];

            foreach(DataDictionary.Specification.Paragraph entry in tableHeader.SubParagraphs)
            {
                // 3 valeurs: tout jusqu'au premier ;
                //            tout jusqu'au suivant ;
                //            tout le reste
                char[] separator = new char[] { ';', ' ' };
                string[] values = entry.Text.Split(separator);
                string M_TRACTION = values[0];
                string M_VOLTAGE = values[2];
                string NID_CTRACTION = values[4];


                TestCase testCase = (TestCase)DataDictionary.Generated.acceptor.getFactory().createTestCase();
                testCase.Name = "M_TRACTION = " + M_TRACTION; 

                // 2 steps: init test environment & msg
                // 1st step
                Step step1 = (Step)DataDictionary.Generated.acceptor.getFactory().createStep();
                step1.Name = "Step 1 - Initialize";
                SubStep subStep1 = (SubStep)DataDictionary.Generated.acceptor.getFactory().createSubStep();
                subStep1.Name = "Initialize test environment";
                Action initialize = (Action)DataDictionary.Generated.acceptor.getFactory().createAction();
                initialize.ExpressionText = "Testing.InitializeTestEnvironment(0.0)";
                subStep1.appendActions(initialize);
                step1.appendSubSteps(subStep1);
                testCase.appendSteps(step1);
                // 1st step END

                // 2nd step
                /* GAUTIER */
                /*  Crée un deuxième step, avec un seul sub-step qui contient une action et une expectation */
                Step step2 = (Step)DataDictionary.Generated.acceptor.getFactory().createStep();
                step2.Name = "Check translation of Packet 39";
                SubStep subStep2 = (SubStep)DataDictionary.Generated.acceptor.getFactory().createSubStep();
                subStep2.Name = "Receive messages";
                Action receiveMessage = (Action)DataDictionary.Generated.acceptor.getFactory().createAction();
                receiveMessage.ExpressionText = "BTM.Message_SystemVersion1 <- " + GetBaliseMessage(M_TRACTION);
                Expectation testTranslation = (Expectation)DataDictionary.Generated.acceptor.getFactory().createExpectation();
                testTranslation.ExpressionText = "(FIRST X IN BTM.CurrentBaliseGroup).Telegram == " + GetCorrectTranslation(M_VOLTAGE, NID_CTRACTION);
                subStep2.appendActions(receiveMessage);
                subStep2.appendExpectations(testTranslation);
                step2.appendSubSteps(subStep2);
                testCase.appendSteps(step2);
                // 2nd step END

                Item.appendTestCases(testCase);
            }
        }

        private string GetBaliseMessage(string M_TRACTION)
        {
            string retVal = "";

            DataDictionary.Types.Structure BTMStructure = Item.Dictionary.FindByFullName("Messages.SystemVersion1.EUROBALISE.Message") as DataDictionary.Types.Structure;
            if (BTMStructure != null)
            {
                DataDictionary.Values.StructureValue theMessage = new DataDictionary.Values.StructureValue(BTMStructure, true);

                DataDictionary.Values.ListValue sequence1 = theMessage.SubVariables["Sequence1"].Value as DataDictionary.Values.ListValue;
                if (sequence1 != null)
                {
                    DataDictionary.Types.Structure substructure1 = Item.Dictionary.FindByFullName("Messages.SystemVersion1.EUROBALISE.SubStructure1") as DataDictionary.Types.Structure;

                    if (substructure1 != null)
                    {
                        DataDictionary.Values.StructureValue sstruct = substructure1.DefaultValue as DataDictionary.Values.StructureValue;

                        DataDictionary.Types.Structure track_to_train = Item.Dictionary.FindByFullName("Messages.SystemVersion1.PACKET.TRACK_TO_TRAIN.Message") as DataDictionary.Types.Structure;
                        if (track_to_train != null)
                        {
                            DataDictionary.Values.StructureValue tracktotrain = track_to_train.DefaultValue as DataDictionary.Values.StructureValue;

                            DataDictionary.Types.Structure packet39 = Item.Dictionary.FindByFullName("Messages.SystemVersion1.PACKET.TRACK_TO_TRAIN.TRACK_CONDITION_CHANGE_OF_TRACTION_SYSTEM.Message") as DataDictionary.Types.Structure;
                            if (packet39 != null)
                            {
                                DataDictionary.Values.StructureValue Packet_39 = packet39.DefaultValue as DataDictionary.Values.StructureValue;

                                DataDictionary.Values.IntValue M_Traction = Packet_39.SubVariables["M_TRACTION"].Value as DataDictionary.Values.IntValue;
                                if (M_Traction != null)
                                {
                                    M_Traction.Val = decimal.Parse(M_TRACTION);
                                }

                                tracktotrain.SubVariables["TRACK_CONDITION_CHANGE_OF_TRACTION_SYSTEM"].Value = Packet_39;
                            }

                            sstruct.SubVariables["TRACK_TO_TRAIN"].Value = tracktotrain;
                        }
                        sequence1.Val.Add(sstruct);
                    }

                }

                retVal = theMessage.LiteralName;
            }
            return retVal;
        }

        private string GetCorrectTranslation(string M_VOLTAGE, string NID_CTRACTION)
        {
            string retVal = "";

            DataDictionary.Types.Structure BTMStructure = Item.Dictionary.FindByFullName("Messages.EUROBALISE.Message") as DataDictionary.Types.Structure;
            if (BTMStructure != null)
            {
                DataDictionary.Values.StructureValue theMessage = new DataDictionary.Values.StructureValue(BTMStructure, true);

                DataDictionary.Values.ListValue sequence1 = theMessage.SubVariables["Sequence1"].Value as DataDictionary.Values.ListValue;
                if (sequence1 != null)
                {
                    DataDictionary.Types.Structure substructure1 = Item.Dictionary.FindByFullName("Messages.EUROBALISE.SubStructure1") as DataDictionary.Types.Structure;

                    if (substructure1 != null)
                    {
                        DataDictionary.Values.StructureValue sstruct = substructure1.DefaultValue as DataDictionary.Values.StructureValue;

                        DataDictionary.Types.Structure track_to_train = Item.Dictionary.FindByFullName("Messages.PACKET.TRACK_TO_TRAIN.Message") as DataDictionary.Types.Structure;
                        if (track_to_train != null)
                        {
                            DataDictionary.Values.StructureValue tracktotrain = track_to_train.DefaultValue as DataDictionary.Values.StructureValue;

                            DataDictionary.Types.Structure packet39 = Item.Dictionary.FindByFullName("Messages.PACKET.TRACK_TO_TRAIN.TRACK_CONDITION_CHANGE_OF_TRACTION_SYSTEM.Message") as DataDictionary.Types.Structure;
                            if (packet39 != null)
                            {
                                DataDictionary.Values.StructureValue Packet_39 = packet39.DefaultValue as DataDictionary.Values.StructureValue;

                                DataDictionary.Values.IntValue M_Voltage = Packet_39.SubVariables["M_VOLTAGE"].Value as DataDictionary.Values.IntValue;
                                if (M_Voltage != null)
                                {
                                    M_Voltage.Val = decimal.Parse(M_VOLTAGE);
                                }
                                DataDictionary.Values.IntValue NID_CTraction = Packet_39.SubVariables["NID_CTRACTION"].Value as DataDictionary.Values.IntValue;
                                if (NID_CTraction != null && NID_CTRACTION != "-")
                                {
                                    NID_CTraction.Val = decimal.Parse(NID_CTRACTION);
                                }

                                tracktotrain.SubVariables["TRACK_CONDITION_CHANGE_OF_TRACTION_SYSTEM"].Value = Packet_39;
                            }

                            sstruct.SubVariables["TRACK_TO_TRAIN"].Value = tracktotrain;
                        }
                        sequence1.Val.Add(sstruct);
                    }

                }

                retVal = theMessage.LiteralName;
            }
            return retVal;
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


            retVal.Add(new MenuItem("Generate packet 39 tests", GenerateThings));
            retVal.Add(new MenuItem("-"));

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