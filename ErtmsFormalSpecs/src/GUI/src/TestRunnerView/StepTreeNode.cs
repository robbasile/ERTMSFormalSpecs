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
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Tests.Runner;
using DataDictionary.Values;
using GUI.LongOperations;
using Utils;
using DBMessage = DataDictionary.Tests.DBElements.DBMessage;
using Step = DataDictionary.Tests.Step;
using SubStep = DataDictionary.Tests.SubStep;
using Translation = DataDictionary.Tests.Translations.Translation;
using TranslationDictionary = DataDictionary.Tests.Translations.TranslationDictionary;

namespace GUI.TestRunnerView
{
    public class StepTreeNode : ReferencesParagraphTreeNode<Step>
    {
        /// <summary>
        ///     The value editor
        /// </summary>
        private class ItemEditor : ReferencesParagraphEditor
        {
            /// <summary>
            ///     The step name
            /// </summary>
            [Category("Description")]
            public override string Name
            {
                get
                {
                    string retVal = Item.Name;

                    if (Item.getTCS_Order() != 0)
                    {
                        retVal = "Step " + Item.getTCS_Order() + ": " + Item.getDescription();
                    }

                    return retVal;
                }
            }

            /// <summary>
            ///     The step description
            /// </summary>
            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public string Description
            {
                get { return Item.getDescription(); }
                set { Item.setDescription(value); }
            }

            /// <summary>
            ///     The step order number
            /// </summary>
            [Category("Subset76")]
            // ReSharper disable once UnusedMember.Local
            public int Order
            {
                get { return Item.getTCS_Order(); }
                set { Item.setTCS_Order(value); }
            }

            /// <summary>
            ///     The step distance
            /// </summary>
            [Category("Subset76")]
            // ReSharper disable once UnusedMember.Local
            public int Distance
            {
                get { return Item.getDistance(); }
                set { Item.setDistance(value); }
            }

            /// <summary>
            ///     The step I/O mode
            /// </summary>
            [Category("Subset76")]
            // ReSharper disable once UnusedMember.Local
            public acceptor.ST_IO InputOutput
            {
                get { return Item.getIO(); }
                set { Item.setIO(value); }
            }

            /// <summary>
            ///     The step Interface
            /// </summary>
            [Category("Subset76")]
            // ReSharper disable once UnusedMember.Local
            public acceptor.ST_INTERFACE Interface
            {
                get { return Item.getInterface(); }
                set { Item.setInterface(value); }
            }

            /// <summary>
            ///     The step level in
            /// </summary>
            [Category("Subset76")]
            // ReSharper disable once UnusedMember.Local
            public acceptor.ST_LEVEL TestLevelIn
            {
                get { return Item.getLevelIN(); }
                set { Item.setLevelIN(value); }
            }

            /// <summary>
            ///     The step level out
            /// </summary>
            [Category("Subset76")]
            // ReSharper disable once UnusedMember.Local
            public acceptor.ST_LEVEL TestLevelOut
            {
                get { return Item.getLevelOUT(); }
                set { Item.setLevelOUT(value); }
            }

            /// <summary>
            ///     The step mode in
            /// </summary>
            [Category("Subset76")]
            // ReSharper disable once UnusedMember.Local
            public acceptor.ST_MODE TestModeIn
            {
                get { return Item.getModeIN(); }
                set { Item.setModeIN(value); }
            }

            /// <summary>
            ///     The step mode out
            /// </summary>
            [Category("Subset76")]
            // ReSharper disable once UnusedMember.Local
            public acceptor.ST_MODE TestModeOut
            {
                get { return Item.getModeOUT(); }
                set { Item.setModeOUT(value); }
            }

            /// <summary>
            ///     The step is translated or not
            /// </summary>
            [Category("Subset76")]
            // ReSharper disable once UnusedMember.Local
            public bool TranslationRequired
            {
                get { return Item.getTranslationRequired(); }
                set { Item.setTranslationRequired(value); }
            }

            /// <summary>
            ///     The step is translated or not
            /// </summary>
            [Category("Subset76")]
            // ReSharper disable once UnusedMember.Local
            public bool Translated
            {
                get { return Item.getTranslated(); }
                set { Item.setTranslated(value); }
            }

            /// <summary>
            ///     The item user comment
            /// </summary>
            [Category("Subset76")]
            // ReSharper disable once UnusedMember.Local
            public string UserComment
            {
                get { return Item.getUserComment(); }
                set { Item.setUserComment(value); }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public StepTreeNode(Step item, bool buildSubNodes)
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

            foreach (SubStep subStep in Item.SubSteps)
            {
                subNodes.Add(new SubStepTreeNode(subStep, recursive));
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

        /// <summary>
        ///     Shows the messages associated to this step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ShowMessagesHandler(object sender, EventArgs args)
        {
            string messageExpression = "[";

            bool first = true;
            foreach (DBMessage message in Item.StepMessages)
            {
                if (!first)
                {
                    messageExpression = messageExpression + ",";
                }

                if (message != null)
                {
                    messageExpression = messageExpression + Translation.format_message(message);
                    first = false;
                }
            }
            messageExpression += "]";

            Expression expression = EFSSystem.INSTANCE.Parser.Expression(Item.Dictionary, messageExpression);
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);

            StructureEditor.Window editor = new StructureEditor.Window();
            editor.SetModel(value);
            editor.ShowDialog();
        }

        /// <summary>
        ///     Shows the translation which should be applied on this step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ShowTranslationHandler(object sender, EventArgs args)
        {
            TranslationDictionary translationDictionary = Item.Dictionary.TranslationDictionary;

            if (translationDictionary != null)
            {
                Translation translation = translationDictionary.findTranslation(Item.getDescription(), Item.Comment);
                if (translation != null)
                {
                    // Finds the translation window which corresponds to this translation
                    TranslationRules.Window translationWindow = null;
                    foreach (IBaseForm form in GuiUtils.MdiWindow.SubWindows)
                    {
                        translationWindow = form as TranslationRules.Window;
                        if (translationWindow != null)
                        {
                            TypedTreeView<TranslationDictionary> treeView =
                                translationWindow.TreeView as TypedTreeView<TranslationDictionary>;
                            if (treeView != null && treeView.Root == translation.TranslationDictionary)
                            {
                                break;
                            }
                        }
                    }

                    if (translationWindow == null)
                    {
                        translationWindow = new TranslationRules.Window(translation.TranslationDictionary);
                        GuiUtils.MdiWindow.AddChildWindow(translationWindow);
                    }

                    EFSSystem.INSTANCE.Context.SelectElement(translation, this, Context.SelectionCriteria.DoubleClick);
                }
            }
        }

        /// <summary>
        ///     Translates the corresponding step, according to translation rules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void TranslateHandler(object sender, EventArgs args)
        {
            FinderRepository.INSTANCE.ClearCache();
            Item.Translate(Item.Dictionary.TranslationDictionary);
            EFSSystem.INSTANCE.Context.HandleChangeEvent(Item, Context.ChangeKind.Translation);
        }

        /// <summary>
        ///     Ensures that the runner corresponds to test case
        /// </summary>
        private void CheckRunner()
        {
            Window window = BaseForm as Window;
            if (window != null && window.EfsSystem.Runner != null &&
                window.EfsSystem.Runner.SubSequence != Item.SubSequence)
            {
                window.Clear();
            }
        }

        /// <summary>
        ///     Adds a step after this one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddSubStepHandler(object sender, EventArgs args)
        {
            Item.appendSubSteps(SubStep.CreateDefault(Item.SubSteps));
        }

        private class ExecuteTestsHandler : BaseLongOperation
        {
            /// <summary>
            ///     The window for which theses tests should be executed
            /// </summary>
            private Window Window { get; set; }

            /// <summary>
            ///     The subsequence which should be executed
            /// </summary>
            private Step Step { get; set; }

            /// <summary>
            ///     Indicates that the engine should be run until all blocking expectations are reached
            /// </summary>
            private bool RunForExpectations { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="window"></param>
            /// <param name="step"></param>
            /// <param name="runForBlockingExpectations"></param>
            public ExecuteTestsHandler(Window window, Step step, bool runForBlockingExpectations)
            {
                Window = window;
                Step = step;
                RunForExpectations = runForBlockingExpectations;
            }

            /// <summary>
            ///     Executes the tests in the background thread
            /// </summary>
            public override void ExecuteWork()
            {
                if (Window != null)
                {
                    Window.SetSubSequence(Step.SubSequence);
                    Runner runner = Window.GetRunner(Step.SubSequence);

                    runner.RunUntilStep(Step);
                    foreach (SubStep subStep in Step.SubSteps)
                    {
                        runner.SetupSubStep(subStep);
                        if (!subStep.getSkipEngine())
                        {
                            if (RunForExpectations)
                            {
                                runner.RunForBlockingExpectations(true);
                            }
                            else
                            {
                                runner.Cycle();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Handles a run event on this step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RunHandler(object sender, EventArgs args)
        {
            CheckRunner();

            Window window = BaseForm as Window;
            if (window != null)
            {
                ExecuteTestsHandler executeTestHandler = new ExecuteTestsHandler(window, Item, false);
                executeTestHandler.ExecuteUsingProgressDialog("Executing test steps");

                EFSSystem.INSTANCE.Context.HandleEndOfCycle();
                window.tabControl1.SelectedTab = window.testExecutionTabPage;
            }
        }

        /// <summary>
        ///     Handles a run event on this step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RunForExpectationsHandler(object sender, EventArgs args)
        {
            CheckRunner();

            Window window = BaseForm as Window;
            if (window != null)
            {
                ExecuteTestsHandler executeTestHandler = new ExecuteTestsHandler(window, Item, false);
                executeTestHandler.ExecuteUsingProgressDialog("Executing test steps");

                EFSSystem.INSTANCE.Context.HandleEndOfCycle();
                window.tabControl1.SelectedTab = window.testExecutionTabPage;
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
                new MenuItem("Add sub-step", AddSubStepHandler),
                new MenuItem("Delete", DeleteHandler)
            };

            retVal.AddRange(base.GetMenuItems());

            // ReSharper disable RedundantAssignment
            int index = 6;
            retVal.Insert(index++, new MenuItem("Show messages", ShowMessagesHandler));
            retVal.Insert(index++, new MenuItem("Show translation rule", ShowTranslationHandler));
            retVal.Insert(index++, new MenuItem("Apply translation rules", TranslateHandler));
            retVal.Insert(index++, new MenuItem("-"));
            retVal.Insert(index++, new MenuItem("Run, not checking expectations", RunHandler));
            retVal.Insert(index++, new MenuItem("Run until expectation reached", RunForExpectationsHandler));
            retVal.Insert(index++, new MenuItem("-"));
            // ReSharper restore RedundantAssignment

            return retVal;
        }

        /// <summary>
        ///     Handles the drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);
            if (sourceNode is SubStepTreeNode)
            {
                SubStepTreeNode subStep = sourceNode as SubStepTreeNode;

                subStep.Delete();
                Item.appendSubSteps(subStep.Item);
            }
        }
    }
}