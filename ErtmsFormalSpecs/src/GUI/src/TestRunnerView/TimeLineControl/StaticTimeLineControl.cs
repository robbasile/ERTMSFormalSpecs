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
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Tests.Runner.Events;
using DataDictionary.Values;
using GUI.DataDictionaryView;
using GUI.EditorView;
using GUI.Properties;
using Utils;
using WeifenLuo.WinFormsUI.Docking;
using Action = DataDictionary.Rules.Action;
using Expectation = DataDictionary.Tests.Expectation;
using ModelElement = DataDictionary.ModelElement;
using Step = DataDictionary.Tests.Step;
using Structure = DataDictionary.Types.Structure;
using SubSequence = DataDictionary.Tests.SubSequence;
using SubStep = DataDictionary.Tests.SubStep;
using TestCase = DataDictionary.Tests.TestCase;
using Translation = DataDictionary.Tests.Translations.Translation;
using Util = DataDictionary.Util;

namespace GUI.TestRunnerView.TimeLineControl
{
    /// <summary>
    ///     The static time line according to a test case
    /// </summary>
    public class StaticTimeLineControl : TimeLineControl
    {
        /// <summary>
        ///     The subsequence currently being displayed
        /// </summary>
        private SubSequence _subSequence;

        /// <summary>
        ///     The test case for which this time line is built
        /// </summary>
        public SubSequence SubSequence
        {
            get { return _subSequence; }
            set
            {
                _testCase = null;
                _subSequence = value;
                _translation = null;
                CleanEventPositions();
            }
        }

        /// <summary>
        ///     The test case
        /// </summary>
        private TestCase _testCase;

        /// <summary>
        ///     The test case for which this time line is built
        /// </summary>
        public TestCase TestCase
        {
            get { return _testCase; }
            set
            {
                _subSequence = null;
                _testCase = value;
                _translation = null;
                CleanEventPositions();
            }
        }

        /// <summary>
        ///     The translation currently being displayed
        /// </summary>
        private Translation _translation;

        /// <summary>
        /// </summary>
        public Translation Translation
        {
            get { return _translation; }
            set
            {
                _translation = value;
                _testCase = null;
                _subSequence = null;
                CleanEventPositions();
            }
        }

        /// <summary>
        ///     Indicates whether the timeline should display the element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool ShouldDisplayModelElement(IModelElement element)
        {
            bool retVal = element == null;

            retVal = retVal || (Translation != null && Translation.IsParent(element));
            retVal = retVal || (SubSequence != null && SubSequence.IsParent(element));
            retVal = retVal || (TestCase != null && TestCase.IsParent(element));

            return retVal;
        }

        /// <summary>
        ///     The steps to be displayed
        /// </summary>
        public ArrayList Steps
        {
            get
            {
                ArrayList retVal = null;

                if (TestCase != null)
                {
                    retVal = TestCase.Steps;
                }
                else if (SubSequence != null)
                {
                    retVal = new ArrayList();
                    foreach (TestCase testCase in SubSequence.TestCases)
                    {
                        retVal.AddRange(testCase.Steps);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public StaticTimeLineControl()
        {
            AllowDrop = true;
            DragEnter += TimeLineControl_DragEnter;
            DragDrop += TimeLineControl_DragDrop;

            DrawArea.DoubleClick += TimeLineControl_DoubleClick;
            DrawArea.MouseDown += StaticTimeLineControl_MouseDown;

            TestCase = null;
            SubSequence = null;
        }

        private const int Ctrl = 8;

        /// <summary>
        ///     Changes the cursor according to the modifier key when a drag & drop operation is performed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeLineControl_DragEnter(object sender, DragEventArgs e)
        {
            if ((e.KeyState & Ctrl) != 0)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        /// <summary>
        ///     Handles a drop event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeLineControl_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("WindowsForms10PersistentObject", false))
            {
                BaseTreeNode sourceNode = e.Data.GetData("WindowsForms10PersistentObject") as BaseTreeNode;
                if (sourceNode != null)
                {
                    VariableTreeNode variableNode = sourceNode as VariableTreeNode;
                    if (variableNode != null)
                    {
                        SubStep subStep = SubStepRelatedToMousePosition();
                        if (subStep != null)
                        {
                            // Create the default value
                            IValue value = null;
                            Expression expression = null;
                            string defaultValue = variableNode.Item.Default;
                            if (defaultValue != null)
                            {
                                const bool doSemanticalAnalysis = true;
                                const bool silent = true;
                                expression = new Parser().Expression(variableNode.Item, defaultValue,
                                    AllMatches.INSTANCE, doSemanticalAnalysis, null, silent);
                            }

                            if (expression != null)
                            {
                                InterpretationContext context = new InterpretationContext {UseDefaultValue = false};
                                value = expression.GetExpressionValue(context, null);
                            }

                            if (value == null || value is EmptyValue)
                            {
                                Structure structureType = variableNode.Item.Type as Structure;
                                if (structureType != null)
                                {
                                    const bool setDefaultValue = false;
                                    value = new StructureValue(structureType, setDefaultValue);
                                }
                            }

                            // Create the action or the expectation according to the keyboard modifier keys
                            if (value != null)
                            {
                                if ((e.KeyState & Ctrl) != 0)
                                {
                                    Expectation expectation = (Expectation) acceptor.getFactory().createExpectation();
                                    expectation.ExpressionText = variableNode.Item.FullName + " == " + value.FullName;
                                    subStep.appendExpectations(expectation);
                                }
                                else
                                {
                                    Action action = (Action) acceptor.getFactory().createAction();
                                    action.ExpressionText = variableNode.Item.FullName + " <- " + value.FullName;
                                    subStep.appendActions(action);
                                }
                            }
                            else
                            {
                                MessageBox.Show(
                                    Resources
                                        .StaticTimeLineControl_TimeLineControl_DragDrop_Cannot_evaluate_variable_default_value,
                                    Resources.StaticTimeLineControl_TimeLineControl_DragDrop_Cannot_create_event,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     The sub step relative to the mouse position
        /// </summary>
        /// <returns></returns>
        private SubStep SubStepRelatedToMousePosition()
        {
            SubStep retVal = null;

            ModelEvent evt = GetEventUnderMouse();
            if (evt != null && evt.Instance != null)
            {
                retVal = EnclosingFinder<SubStep>.find(evt.Instance as IEnclosed, true);
                if (retVal == null)
                {
                    Step step = evt.Instance as Step;
                    if (step != null && step.SubSteps.Count > 0)
                    {
                        retVal = (SubStep) step.SubSteps[step.SubSteps.Count - 1];
                    }
                }
            }

            if (retVal == null && Steps != null && Steps.Count > 0)
            {
                Step step = (Step) Steps[Steps.Count - 1];
                if (step.SubSteps.Count > 0)
                {
                    retVal = (SubStep) step.SubSteps[step.SubSteps.Count - 1];
                }
            }

            return retVal;
        }

        /// <summary>
        ///     A base menu item element
        /// </summary>
        private class BaseToolStripButton : MenuItem
        {
            /// <summary>
            ///     The selected event, if any
            /// </summary>
            protected ModelEvent Selected { get; private set; }

            /// <summary>
            ///     The time line control for which this menu item is built
            /// </summary>
            protected StaticTimeLineControl TimeLineControl { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="timeLineControl"></param>
            /// <param name="modelEvent"></param>
            /// <param name="caption"></param>
            protected BaseToolStripButton(StaticTimeLineControl timeLineControl, ModelEvent modelEvent, string caption)
                : base(caption)
            {
                TimeLineControl = timeLineControl;
                Selected = modelEvent;
            }

            /// <summary>
            ///     Provides the step enclosing the selected event
            /// </summary>
            protected Step Step
            {
                get
                {
                    Step retVal = null;

                    if (Selected != null)
                    {
                        retVal = EnclosingFinder<Step>.find(Selected.Instance as IEnclosed, true);
                    }

                    return retVal;
                }
            }

            /// <summary>
            ///     Provides the substep enclosing the selected event
            /// </summary>
            protected SubStep SubStep
            {
                get
                {
                    SubStep retVal = null;

                    if (Selected != null)
                    {
                        retVal = EnclosingFinder<SubStep>.find(Selected.Instance as IEnclosed, true);
                    }

                    return retVal;
                }
            }
        }

        /// <summary>
        ///     Deletes the current element
        /// </summary>
        private class DeleteMenuItem : BaseToolStripButton
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="timeLineControl"></param>
            /// <param name="modelEvent"></param>
            public DeleteMenuItem(StaticTimeLineControl timeLineControl, ModelEvent modelEvent)
                : base(timeLineControl, modelEvent, "Delete")
            {
                Enabled = modelEvent != null;
            }

            /// <summary>
            ///     Executes the action
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                if (Selected != null && Selected.Instance != null)
                {
                    IModelElement model = Selected.Instance as IModelElement;
                    if (model != null)
                    {
                        ArrayList collection = model.EnclosingCollection;
                        if (collection != null)
                        {
                            collection.Remove(model);
                        }
                    }
                }
                base.OnClick(e);
            }
        }

        /// <summary>
        ///     Adds a step after the selected step
        /// </summary>
        private class AddStepMenuItem : BaseToolStripButton
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="timeLineControl"></param>
            /// <param name="modelEvent"></param>
            public AddStepMenuItem(StaticTimeLineControl timeLineControl, ModelEvent modelEvent)
                : base(timeLineControl, modelEvent, "Add step")
            {
                Enabled = GetTestCase() != null;
            }

            /// <summary>
            ///     Executes the action
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                Step newStep = (Step) acceptor.getFactory().createStep();
                newStep.Enclosing = TimeLineControl.TestCase;

                SubStep subStep = (SubStep) acceptor.getFactory().createSubStep();
                subStep.Name = "Substep 1";
                newStep.appendSubSteps(subStep);

                TestCase testCase = GetTestCase();
                if (Step != null)
                {
                    newStep.Name = "NewStep";
                    int index = TimeLineControl.Steps.IndexOf(Step);
                    testCase.Steps.Insert(index + 1, newStep);
                    newStep.setFather(testCase);
                }
                else
                {
                    newStep.Name = "Step " + (TimeLineControl.Steps.Count + 1);
                    testCase.Steps.Add(newStep);
                    newStep.setFather(testCase);
                }

                base.OnClick(e);
            }

            /// <summary>
            ///     Gets the test case in which steps should be created
            /// </summary>
            private TestCase GetTestCase()
            {
                TestCase retVal = TimeLineControl.TestCase;

                if (retVal == null)
                {
                    if (TimeLineControl.SubSequence != null)
                    {
                        if (TimeLineControl.SubSequence.TestCases.Count == 0)
                        {
                            retVal = (TestCase) acceptor.getFactory().createTestCase();
                            TimeLineControl.SubSequence.appendTestCases(retVal);
                        }
                        else
                        {
                            retVal = (TestCase) TimeLineControl.SubSequence.TestCases[
                                TimeLineControl.SubSequence.TestCases.Count - 1];
                        }
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Adds a substep or the selected step
        /// </summary>
        private class AddSubStepMenuItem : BaseToolStripButton
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="timeLineControl"></param>
            /// <param name="modelEvent"></param>
            public AddSubStepMenuItem(StaticTimeLineControl timeLineControl, ModelEvent modelEvent)
                : base(timeLineControl, modelEvent, "Add substep")
            {
                Enabled = Step != null || TimeLineControl.Translation != null;
            }

            /// <summary>
            ///     Executes the action
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                SubStep newSubStep = (SubStep) acceptor.getFactory().createSubStep();
                if (Step != null)
                {
                    newSubStep.Enclosing = Step;

                    if (SubStep != null)
                    {
                        newSubStep.Name = "NewSubStep";
                        int index = Step.SubSteps.IndexOf(SubStep);
                        Step.SubSteps.Insert(index + 1, newSubStep);
                    }
                    else
                    {
                        newSubStep.Name = "SubStep " + (Step.SubSteps.Count + 1);
                        Step.SubSteps.Add(newSubStep);
                    }
                }
                else if (TimeLineControl.Translation != null)
                {
                    newSubStep.Enclosing = TimeLineControl.Translation;

                    if (SubStep != null)
                    {
                        newSubStep.Name = "NewSubStep";
                        int index = TimeLineControl.Translation.SubSteps.IndexOf(SubStep);
                        TimeLineControl.Translation.SubSteps.Insert(index + 1, newSubStep);
                    }
                    else
                    {
                        newSubStep.Name = "SubStep " + (TimeLineControl.Translation.SubSteps.Count + 1);
                        TimeLineControl.Translation.SubSteps.Add(newSubStep);
                    }
                }
                base.OnClick(e);
            }
        }

        /// <summary>
        ///     Adds a step after the selected step
        /// </summary>
        private class AddChangeMenuItem : BaseToolStripButton
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="timeLineControl"></param>
            /// <param name="modelEvent"></param>
            public AddChangeMenuItem(StaticTimeLineControl timeLineControl, ModelEvent modelEvent)
                : base(timeLineControl, modelEvent, "Add change")
            {
                Enabled = SubStep != null;
            }

            /// <summary>
            ///     Executes the action
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                if (SubStep != null)
                {
                    Action action = (Action) acceptor.getFactory().createAction();
                    action.Name = "";
                    SubStep.appendActions(action);
                }
                base.OnClick(e);
            }
        }

        /// <summary>
        ///     Adds a step after the selected step
        /// </summary>
        private class AddExpectationMenuItem : BaseToolStripButton
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="timeLineControl"></param>
            /// <param name="modelEvent"></param>
            public AddExpectationMenuItem(StaticTimeLineControl timeLineControl, ModelEvent modelEvent)
                : base(timeLineControl, modelEvent, "Add expectation")
            {
                Enabled = SubStep != null;
            }

            /// <summary>
            ///     Executes the action
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                if (SubStep != null)
                {
                    Expectation expectation = (Expectation) acceptor.getFactory().createExpectation();
                    expectation.Name = "";
                    SubStep.appendExpectations(expectation);
                }
                base.OnClick(e);
            }
        }

        /// <summary>
        ///     Updates the contextual menu according to the position of the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StaticTimeLineControl_MouseDown(object sender, MouseEventArgs e)
        {
            ContextMenu = new ContextMenu();

            ModelEvent evt = GetEventUnderMouse();
            ContextMenu.MenuItems.Add(new AddStepMenuItem(this, evt));
            ContextMenu.MenuItems.Add(new AddSubStepMenuItem(this, evt));
            ContextMenu.MenuItems.Add(new AddChangeMenuItem(this, evt));
            ContextMenu.MenuItems.Add(new AddExpectationMenuItem(this, evt));
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(new DeleteMenuItem(this, evt));
        }

        /// <summary>
        ///     Sets the string value into the right property
        /// </summary>
        private class TimeLineExpressionableTextChangeHandler : ExpressionableTextChangeHandler
        {
            /// <summary>
            ///     The time line control
            /// </summary>
            private StaticTimeLineControl TimeLine { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="timeLine"></param>
            /// <param name="instance"></param>
            public TimeLineExpressionableTextChangeHandler(StaticTimeLineControl timeLine, IExpressionable instance)
                : base(instance as ModelElement)
            {
                TimeLine = timeLine;
            }

            /// <summary>
            ///     The way text is set back in the instance
            /// </summary>
            /// <returns></returns>
            public override void SetText(string text)
            {
                base.SetText(text);
                TimeLine.Refresh();
            }
        }

        /// <summary>
        ///     Handles a double click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeLineControl_DoubleClick(object sender, EventArgs e)
        {
            ModelEvent evt = GetEventUnderMouse();

            VariableUpdate variableUpdate = evt as VariableUpdate;
            if (variableUpdate != null)
            {
                EditorView.Window form = new EditorView.Window();
                TimeLineExpressionableTextChangeHandler handler = new TimeLineExpressionableTextChangeHandler(this,
                    variableUpdate.Action);
                form.setChangeHandler(handler);
                GuiUtils.MdiWindow.AddChildWindow(form, DockAreas.Float);
            }

            Expect expect = evt as Expect;
            if (expect != null)
            {
                EditorView.Window form = new EditorView.Window();
                TimeLineExpressionableTextChangeHandler handler = new TimeLineExpressionableTextChangeHandler(this,
                    expect.Expectation);
                form.setChangeHandler(handler);
                GuiUtils.MdiWindow.AddChildWindow(form, DockAreas.Float);
            }
        }

        /// <summary>
        ///     Refreshes the view according to the test case
        /// </summary>
        public override void Refresh()
        {
            if (TestCase != null && TestCase.ActionCount != HandledEvents)
            {
                UpdatePositionHandler();
                HandledEvents = TestCase.ActionCount;
            }
            else if (SubSequence != null)
            {
                UpdatePositionHandler();
            }
            else if (Translation != null)
            {
                UpdatePositionHandler();
            }

            base.Refresh();
        }

        /// <summary>
        ///     Update the information stored in the position handler according to the test case
        /// </summary>
        protected override void UpdatePositionHandler()
        {
            Util.DontNotify(() =>
            {
                PositionHandler.CleanPositions();
                if ((TestCase != null) || SubSequence != null)
                {
                    double currentTime = 0.0;
                    foreach (Step step in Steps)
                    {
                        if (step.SubSteps.Count > 0)
                        {
                            foreach (SubStep subStep in step.SubSteps)
                            {
                                PositionSubStep(currentTime, subStep);
                                currentTime += 1;
                            }
                        }
                        else
                        {
                            StepActivation stepActivated = new StepActivation(step) {Time = currentTime};
                            PositionHandler.RegisterEvent(stepActivated);
                            currentTime += 1;
                        }
                    }
                }
                else if (Translation != null)
                {
                    double currentTime = 0.0;
                    if (Translation.SubSteps.Count > 0)
                    {
                        foreach (SubStep subStep in Translation.SubSteps)
                        {
                            PositionSubStep(currentTime, subStep);
                            currentTime += 1;
                        }
                    }
                }
            });

            base.UpdatePositionHandler();
        }

        /// <summary>
        ///     Positions a substep in the time line
        /// </summary>
        /// <param name="currentTime"></param>
        /// <param name="subStep"></param>
        /// <returns></returns>
        private void PositionSubStep(double currentTime, SubStep subStep)
        {
            SubStepActivated subStepActivated = new SubStepActivated(subStep, null) {Time = currentTime};
            PositionHandler.RegisterEvent(subStepActivated);
            foreach (Action action in subStep.Actions)
            {
                VariableUpdate variableUpdate = new VariableUpdate(action, action, null);
                PositionHandler.RegisterEvent(variableUpdate);
            }
            foreach (Expectation expectation in subStep.Expectations)
            {
                Expect expect = new Expect(expectation, null);
                PositionHandler.RegisterEvent(expect);
            }
        }
    }
}