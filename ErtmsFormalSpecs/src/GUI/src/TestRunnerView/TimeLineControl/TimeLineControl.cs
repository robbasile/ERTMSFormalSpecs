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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Generated;
using DataDictionary.Interpreter.Statement;
using DataDictionary.Tests.Runner.Events;
using GUI.Properties;
using Utils;
using ModelElement = DataDictionary.ModelElement;
using NameSpace = DataDictionary.Types.NameSpace;
using Step = DataDictionary.Tests.Step;

namespace GUI.TestRunnerView.TimeLineControl
{
    /// <summary>
    ///     this control displays all execution events on a timeline
    /// </summary>
    public abstract class TimeLineControl : Panel
    {
        /// <summary>
        ///     Components for the tool tip
        /// </summary>
        private IContainer components;

        /// <summary>
        ///     This label is used to allow auto scrolling by positionning it at the botton right bounds of the visible rectangle
        /// </summary>
        protected Label AutoScrollEnabler { get; set; }

        /// <summary>
        ///     Label used to define the size of the panel
        /// </summary>
        protected Label AutoPanelSize { get; set; }

        private void InitializeComponent()
        {
            this.components = new Container();
            AutoScroll = true;
            AutoSize = false;
            DoubleBuffered = true;

            Click += new EventHandler(TimeLineControl_Click);

            AutoScrollEnabler = new Label();
            AutoScrollEnabler.Text = "";
            AutoScrollEnabler.Parent = this;
            AutoScrollEnabler.Location = new Point(0, 0);
            AutoScrollEnabler.Visible = true;

            AutoPanelSize = new Label();
            AutoPanelSize.Text = "";
            AutoPanelSize.Parent = this;
            AutoPanelSize.Location = new Point(0, 0);
            AutoPanelSize.Visible = true;
        }

        /// <summary>
        ///     Provides the event under the mouse pointer
        /// </summary>
        /// <returns></returns>
        protected ModelEvent GetEventUnderMouse()
        {
            ModelEvent retVal = null;

            Point position = PointToClient(new Point(MousePosition.X, MousePosition.Y));
            position.Offset(HorizontalScroll.Value, VerticalScroll.Value);
            foreach (KeyValuePair<ModelEvent, Rectangle> pair in PositionHandler.EventPositions)
            {
                if (pair.Value.Contains(position))
                {
                    retVal = pair.Key;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Handles a click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeLineControl_Click(object sender, EventArgs e)
        {
            ModelEvent evt = GetEventUnderMouse();

            RuleFired ruleFired = evt as RuleFired;
            if (ruleFired != null)
            {
                if (GUIUtils.MDIWindow.DataDictionaryWindow != null)
                {
                    GUIUtils.MDIWindow.DataDictionaryWindow.TreeView.Select(ruleFired.RuleCondition);
                }
            }

            VariableUpdate variableUpdate = evt as VariableUpdate;
            if (variableUpdate != null)
            {
                BaseTreeNode treeNode = null;

                if (GUIUtils.MDIWindow.DataDictionaryWindow != null)
                {
                    treeNode = GUIUtils.MDIWindow.DataDictionaryWindow.TreeView.Select(variableUpdate.Action);
                }

                if (treeNode == null)
                {
                    if (GUIUtils.MDIWindow.TestWindow != null)
                    {
                        treeNode = GUIUtils.MDIWindow.TestWindow.TreeView.Select(variableUpdate.Action);
                    }
                }

                if (treeNode == null)
                {
                    foreach (Form form in GUIUtils.MDIWindow.SubWindows)
                    {
                        TranslationRules.Window translationWindow = form as TranslationRules.Window;
                        if (translationWindow != null)
                        {
                            translationWindow.TreeView.Select(variableUpdate.Action);
                        }
                    }
                }
            }

            Expect expect = evt as Expect;
            if (expect != null)
            {
                if (GUIUtils.MDIWindow.TestWindow != null)
                {
                    GUIUtils.MDIWindow.TestWindow.TreeView.Select(expect.Expectation);
                }

                foreach (Form form in GUIUtils.MDIWindow.SubWindows)
                {
                    TranslationRules.Window translationWindow = form as TranslationRules.Window;
                    if (translationWindow != null)
                    {
                        translationWindow.TreeView.Select(expect.Expectation);
                    }
                }
            }

            ModelInterpretationFailure failure = evt as ModelInterpretationFailure;
            if (failure != null)
            {
                if (GUIUtils.MDIWindow.DataDictionaryWindow != null)
                {
                    GUIUtils.MDIWindow.DataDictionaryWindow.TreeView.Select(failure.Instance as IModelElement);
                }
            }

            SubStepActivated subStepActivated = evt as SubStepActivated;
            if (subStepActivated != null)
            {
                if (GUIUtils.MDIWindow.TestWindow != null)
                {
                    GUIUtils.MDIWindow.TestWindow.TreeView.Select(subStepActivated.SubStep);
                }
            }

            StepActivation stepActivation = evt as StepActivation;
            if (stepActivation != null)
            {
                if (GUIUtils.MDIWindow.TestWindow != null)
                {
                    GUIUtils.MDIWindow.TestWindow.TreeView.Select(stepActivation.Step);
                }
            }
        }

        /// <summary>
        ///     The position handler for the events
        /// </summary>
        protected EventPositionHandler PositionHandler { get; set; }

        /// <summary>
        ///     The images used by this time line control
        /// </summary>
        private ImageList Images { get; set; }

        /// <summary>
        ///     The image indexes used to retrieve images
        /// </summary>
        private const int ToolsImageIndex = 0;

        private const int ErrorImageIndex = 1;
        private const int SuccessImageIndex = 2;
        private const int QuestionMarkImageIndex = 3;
        private const int AntennaImageIndex = 4;
        private const int BalanceImageIndex = 5;
        private const int SpeedControlImageIndex = 6;
        private const int TrainImageIndex = 7;
        private const int WheelImageIndex = 8;
        private const int InImageIndex = 9;
        private const int OutImageIndex = 10;
        private const int InOutImageIndex = 11;
        private const int InternalImageIndex = 12;
        private const int CallImageIndex = 13;
        private const int CircularArrowIndex = 14;
        private const int DownArrowIndex = 15;

        /// <summary>
        ///     Constructor
        /// </summary>
        public TimeLineControl()
        {
            InitializeComponent();

            PositionHandler = new EventPositionHandler(this);

            Images = new ImageList();
            Images.Images.Add(Resources.tools);
            Images.Images.Add(Resources.error);
            Images.Images.Add(Resources.success);
            Images.Images.Add(Resources.question_mark);
            Images.Images.Add(Resources.antenna);
            Images.Images.Add(Resources.balance);
            Images.Images.Add(Resources.speed_control);
            Images.Images.Add(Resources.train);
            Images.Images.Add(Resources.wheel);
            Images.Images.Add(Resources.in_icon);
            Images.Images.Add(Resources.out_icon);
            Images.Images.Add(Resources.in_out_icon);
            Images.Images.Add(Resources.internal_icon);
            Images.Images.Add(Resources.call);
            Images.Images.Add(Resources.circular_arrow);
            Images.Images.Add(Resources.down_arrow);
        }

        /// <summary>
        ///     Synthetic class to represent a  Step activation
        /// </summary>
        protected class StepActivation : ModelEvent
        {
            /// <summary>
            ///     The step that has been activated
            /// </summary>
            public Step Step { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="step"></param>
            public StepActivation(Step step)
                : base(step.Name, step, null)
            {
                Step = step;
            }

            /// <summary>
            ///     The namespace associated to this event
            /// </summary>
            public override NameSpace NameSpace
            {
                get { return null; }
            }
        }

        /// <summary>
        ///     Handles the position of the events displayed by the time line
        /// </summary>
        protected class EventPositionHandler
        {
            /// <summary>
            ///     Provides the cycle time of the engine
            /// </summary>
            public TimeLineControl TimeLine { get; private set; }

            /// <summary>
            ///     The allocated positios alongs with their corresponding event
            ///     The first list represents the X axis
            ///     The second list represents the Y axis
            ///     This is related to EventPositions
            /// </summary>
            public List<List<ModelEvent>> AllocatedPositions { get; private set; }

            /// <summary>
            ///     The position of each event.
            ///     This is related to AllocatedPositions.
            /// </summary>
            public Dictionary<ModelEvent, Rectangle> EventPositions { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="timeLine">The time line for which this position handler is built</param>
            public EventPositionHandler(TimeLineControl timeLine)
            {
                TimeLine = timeLine;
                AllocatedPositions = new List<List<ModelEvent>>();
                EventPositions = new Dictionary<ModelEvent, Rectangle>();
                CleanPositions();
            }

            /// <summary>
            ///     Provides the column of a single event
            /// </summary>
            /// <param name="evt"></param>
            /// <returns></returns>
            private int EventColumn(ModelEvent evt)
            {
                int retVal = (int) (evt.Time/EFSSystem.INSTANCE.Runner.Step);

                return retVal;
            }

            /// <summary>
            ///     Clears the position of all events
            /// </summary>
            public void CleanPositions()
            {
                NextY = 0;
                LastActivationTime = -1.0;
                AllocatedPositions.Clear();
                EventPositions.Clear();
                LastStepActivation = null;
                LastSubStepActivation = null;
            }

            /// <summary>
            ///     The activation time of the last event
            /// </summary>
            private double LastActivationTime { get; set; }

            /// <summary>
            ///     The last sub step activation
            /// </summary>
            private SubStepActivated LastSubStepActivation { get; set; }

            /// <summary>
            ///     The last step activation
            /// </summary>
            private StepActivation LastStepActivation { get; set; }

            /// <summary>
            ///     The next Y position to place an event
            /// </summary>
            private int NextY { get; set; }

            /// <summary>
            ///     Registers an event according to its column
            /// </summary>
            /// <param name="evt"></param>
            public void RegisterEvent(ModelEvent evt)
            {
                SubStepActivated currentSubStepActivation = evt as SubStepActivated;
                if (evt.Time > LastActivationTime || currentSubStepActivation != null)
                {
                    LastActivationTime = evt.Time;
                    AllocatedPositions.Add(new List<ModelEvent>());
                    NextY = 0;

                    if (LastSubStepActivation != null)
                    {
                        if (currentSubStepActivation == null)
                        {
                            AllocatedPositions[AllocatedPositions.Count - 1].Add(LastSubStepActivation);
                        }
                    }
                }

                List<ModelEvent> events = AllocatedPositions[AllocatedPositions.Count - 1];
                if (!events.Contains(evt))
                {
                    if (currentSubStepActivation != null)
                    {
                        if (currentSubStepActivation.SubStep.Step != null)
                        {
                            if (LastSubStepActivation != null &&
                                LastSubStepActivation.SubStep.Step == currentSubStepActivation.SubStep.Step)
                            {
                                // Extends the step size
                                Rectangle lastRectangle = EventPositions[LastStepActivation];
                                lastRectangle.Width = lastRectangle.Width + EVENT_MARGING.Width + STEP_SIZE.Width;
                                EventPositions[LastStepActivation] = lastRectangle;
                            }
                            else
                            {
                                // Create a new step activation
                                LastStepActivation = new StepActivation(currentSubStepActivation.SubStep.Step);
                                Point location =
                                    new Point((AllocatedPositions.Count - 1)*(STEP_SIZE.Width + EVENT_MARGING.Width),
                                        NextY);
                                events.Add(LastStepActivation);
                                EventPositions.Add(LastStepActivation, new Rectangle(location, STEP_SIZE));
                            }
                            NextY += STEP_SIZE.Height;
                        }

                        // Setup the substep activation size
                        if (LastSubStepActivation != null &&
                            LastSubStepActivation.SubStep.Step == currentSubStepActivation.SubStep.Step)
                        {
                            // Increase the previous sub step activation size as it belongs to the same step
                            // This is used to remove gaps between substeps of the same step
                            Rectangle lastRectangle = EventPositions[LastSubStepActivation];
                            lastRectangle.Width = lastRectangle.Width + (EVENT_MARGING.Width + 1)/2;
                            EventPositions[LastSubStepActivation] = lastRectangle;

                            events.Add(evt);
                            Point location =
                                new Point(
                                    (AllocatedPositions.Count - 1)*(EVENT_SIZE.Width + EVENT_MARGING.Width) -
                                    (EVENT_MARGING.Width/2), NextY);
                            EventPositions.Add(evt,
                                new Rectangle(location,
                                    new Size(SUBSTEP_SIZE.Width + EVENT_MARGING.Width/2, SUBSTEP_SIZE.Height)));
                        }
                        else
                        {
                            events.Add(evt);
                            Point location =
                                new Point((AllocatedPositions.Count - 1)*(SUBSTEP_SIZE.Width + EVENT_MARGING.Width),
                                    NextY);
                            EventPositions.Add(evt, new Rectangle(location, SUBSTEP_SIZE));
                        }
                        NextY += SUBSTEP_SIZE.Height + EVENT_MARGING.Height;
                    }
                    else
                    {
                        // Create the sub step activation
                        {
                            events.Add(evt);
                            Point location =
                                new Point((AllocatedPositions.Count - 1)*(EVENT_SIZE.Width + EVENT_MARGING.Width), NextY);
                            EventPositions.Add(evt, new Rectangle(location, EVENT_SIZE));
                            NextY += EVENT_SIZE.Height + EVENT_MARGING.Height;
                        }
                    }
                }
                else
                {
                    if (evt == LastSubStepActivation)
                    {
                        // Extent the sub step activation
                        Rectangle lastRectangle = EventPositions[evt];
                        lastRectangle.Width = lastRectangle.Width + EVENT_SIZE.Width + EVENT_MARGING.Width;
                        EventPositions[evt] = lastRectangle;
                    }
                }

                if (evt is SubStepActivated)
                {
                    LastSubStepActivation = evt as SubStepActivated;
                }
            }

            /// <summary>
            ///     Provides the bottom right position of the panel
            /// </summary>
            public Point BottomRightPosition
            {
                get
                {
                    int right = 0;
                    int bottom = 0;

                    foreach (Rectangle rectangle in EventPositions.Values)
                    {
                        if (rectangle.Right > right)
                        {
                            right = rectangle.Right;
                        }

                        if (rectangle.Bottom > bottom)
                        {
                            bottom = rectangle.Bottom;
                        }
                    }

                    return new Point(right, bottom);
                }
            }
        }

        /// <summary>
        ///     Cleans the event positions
        /// </summary>
        public void CleanEventPositions()
        {
            HandledEvents = -1;
            PositionHandler.CleanPositions();
        }

        /// <summary>
        ///     The number of events that were handled
        /// </summary>
        protected int HandledEvents = -1;

        protected abstract void UpdatePositionHandler();

        /// <summary>
        ///     The size of an event
        /// </summary>
        private static Size EVENT_SIZE = new Size(71, 44);

        /// <summary>
        ///     The size of a substep event
        /// </summary>
        private static Size SUBSTEP_SIZE = new Size(71, 33);

        /// <summary>
        ///     The size of a substep event
        /// </summary>
        private static Size STEP_SIZE = new Size(71, 20);

        /// <summary>
        ///     The marging between events
        /// </summary>
        private static Size EVENT_MARGING = new Size(5, 2);

        /// <summary>
        ///     Updates the size of the panel according to the number of events to handle
        /// </summary>
        protected virtual void UpdatePanelSize()
        {
            Point bottomRightPosition = PositionHandler.BottomRightPosition;
            Point location = new Point(
                bottomRightPosition.X - HorizontalScroll.Value,
                bottomRightPosition.Y - VerticalScroll.Value
                );
            AutoPanelSize.Location = location;

            location = new Point(
                location.X,
                0
                );
            AutoScrollEnabler.Location = location;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
            drawEvents(pe);
        }

        /// <summary>
        ///     Draws the event for the panel
        /// </summary>
        /// <param name="pe"></param>
        private void drawEvents(PaintEventArgs pe)
        {
            foreach (KeyValuePair<ModelEvent, Rectangle> pair in PositionHandler.EventPositions)
            {
                drawEvent(pe, pair.Key, pair.Value);
            }
        }

        /// <summary>
        ///     Attributes used to display a single event
        /// </summary>
        private class EventDisplayAttributes
        {
            /// <summary>
            ///     The event fill color
            /// </summary>
            public Color FillColor { get; private set; }

            /// <summary>
            ///     The pen used to draw the form
            /// </summary>
            public Pen DrawPen { get; private set; }

            /// <summary>
            ///     The text to display at the bottom of the event
            /// </summary>
            public string BottomText { get; private set; }

            /// <summary>
            ///     The image index of the left icon
            /// </summary>
            public int LeftIconImageIndex { get; private set; }

            /// <summary>
            ///     The image index of the Right icon
            /// </summary>
            public int RightIconImageIndex { get; private set; }

            /// <summary>
            ///     The icons to display on the top right of the event
            /// </summary>
            public List<int> TopRightIconImageIndex { get; private set; }

            /// <summary>
            ///     The icon to display near the RightIcon
            /// </summary>
            public int RightIconModifierImageIndex { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="fillColor"></param>
            /// <param name="drawPen"></param>
            /// <param name="bottomText"></param>
            /// <param name="leftIcon"></param>
            /// <param name="rightIcon"></param>
            /// <param name="rightIconModifier"></param>
            public EventDisplayAttributes(Color fillColor, Pen drawPen, string bottomText, int leftIcon, int rightIcon,
                int rightIconModifier)
            {
                FillColor = fillColor;
                DrawPen = drawPen;
                BottomText = bottomText;
                LeftIconImageIndex = leftIcon;
                RightIconImageIndex = rightIcon;
                RightIconModifierImageIndex = rightIconModifier;
                TopRightIconImageIndex = new List<int>();
            }
        }

        /// <summary>
        ///     Provides the display attributes for a model event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        private EventDisplayAttributes GetDisplayAttributes(Graphics graphics, ModelEvent evt)
        {
            EventDisplayAttributes retVal = new EventDisplayAttributes(Color.White, new Pen(Color.Black), "<undefined>",
                -1, -1, -1);
            ;

            ModelElement.DontRaiseError(() =>
            {
                Expect expect = evt as Expect;
                if (expect != null)
                {
                    string name = GUIUtils.AdjustForDisplay(graphics, ShortName(expect.Expectation.Name),
                        EVENT_SIZE.Width - 4, BOTTOM_FONT);

                    switch (expect.State)
                    {
                        case Expect.EventState.Active:
                            retVal = new EventDisplayAttributes(Color.Violet, new Pen(Color.Black), name,
                                QuestionMarkImageIndex, GetImageIndex(expect.Expectation), -1);
                            break;
                        case Expect.EventState.Fullfilled:
                            retVal = new EventDisplayAttributes(Color.LightGreen, new Pen(Color.Green), name,
                                SuccessImageIndex, GetImageIndex(expect.Expectation), -1);
                            break;
                        case Expect.EventState.TimeOut:
                            retVal = new EventDisplayAttributes(Color.Red, new Pen(Color.DarkRed), name, ErrorImageIndex,
                                GetImageIndex(expect.Expectation), -1);
                            break;
                    }

                    if (expect.Expectation.getKind() == acceptor.ExpectationKind.aContinuous)
                    {
                        retVal.TopRightIconImageIndex.Add(CircularArrowIndex);
                    }
                    if (expect.Expectation.Blocking)
                    {
                        retVal.TopRightIconImageIndex.Add(DownArrowIndex);
                    }
                }

                ModelInterpretationFailure modelInterpretationFailure = evt as ModelInterpretationFailure;
                if (modelInterpretationFailure != null)
                {
                    string name = GUIUtils.AdjustForDisplay(graphics, modelInterpretationFailure.Message,
                        EVENT_SIZE.Width - 4, BOTTOM_FONT);

                    retVal = new EventDisplayAttributes(Color.Red, new Pen(Color.DarkRed), name, ErrorImageIndex,
                        GetImageIndex(modelInterpretationFailure.Instance as ModelElement), -1);
                }

                RuleFired ruleFired = evt as RuleFired;
                if (ruleFired != null)
                {
                    string name = GUIUtils.AdjustForDisplay(graphics, ShortName(ruleFired.RuleCondition.Name),
                        EVENT_SIZE.Width - 4, BOTTOM_FONT);

                    retVal = new EventDisplayAttributes(Color.LightBlue, new Pen(Color.Blue), name, -1,
                        GetImageIndex(ruleFired.RuleCondition), ToolsImageIndex);
                }

                VariableUpdate variableUpdate = evt as VariableUpdate;
                if (variableUpdate != null)
                {
                    string name = variableUpdate.Action.ExpressionText;
                    int rightIcon = -1;
                    int rightModifier = -1;
                    if (variableUpdate.Action.Statement != null)
                    {
                        name = variableUpdate.Action.Statement.ShortShortDescription();
                        rightIcon = GetImageIndex(variableUpdate.Action.Statement.AffectedElement());

                        switch (variableUpdate.Action.Statement.UsageDescription())
                        {
                            case Statement.ModeEnum.Call:
                                rightModifier = CallImageIndex;
                                break;
                            case Statement.ModeEnum.In:
                                rightModifier = InImageIndex;
                                break;

                            case Statement.ModeEnum.InOut:
                                rightModifier = InOutImageIndex;
                                break;

                            case Statement.ModeEnum.Internal:
                                rightModifier = InternalImageIndex;
                                break;

                            case Statement.ModeEnum.Out:
                                rightModifier = OutImageIndex;
                                break;
                        }
                    }
                    name = GUIUtils.AdjustForDisplay(graphics, ShortName(name), EVENT_SIZE.Width - 4, BOTTOM_FONT);

                    NameSpace nameSpace = EnclosingFinder<NameSpace>.find(variableUpdate.Action);
                    if (nameSpace == null)
                    {
                        retVal = new EventDisplayAttributes(Color.LightGray, new Pen(Color.Black), name, -1, rightIcon,
                            rightModifier);
                    }
                    else
                    {
                        retVal = new EventDisplayAttributes(Color.BlanchedAlmond, new Pen(Color.Black), name, -1,
                            rightIcon, rightModifier);
                    }
                }

                SubStepActivated subStepActivated = evt as SubStepActivated;
                if (subStepActivated != null)
                {
                    retVal = new EventDisplayAttributes(Color.LightGray, new Pen(Color.Black), "SubStep", -1, -1, -1);
                }
            });

            return retVal;
        }

        /// <summary>
        ///     Provides the image index of the element provided
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private int GetImageIndex(ModelElement element)
        {
            int retVal = -1;

            NameSpace nameSpace = EnclosingNameSpaceFinder.find(element);
            while (nameSpace != null && nameSpace.EnclosingNameSpace != null)
            {
                nameSpace = nameSpace.EnclosingNameSpace;
            }

            // TODO : Allow to define the image in the namespace itself
            if (nameSpace != null)
            {
                if (nameSpace.Name == "BTM" || nameSpace.Name == "EURORADIO")
                {
                    retVal = AntennaImageIndex;
                }
                else if (nameSpace.Name == "JRU")
                {
                    retVal = BalanceImageIndex;
                }
                else if (nameSpace.Name == "DMI")
                {
                    retVal = SpeedControlImageIndex;
                }
                else if (nameSpace.Name == "Odometry")
                {
                    retVal = WheelImageIndex;
                }
                else
                {
                    retVal = TrainImageIndex;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The font used to display the top text
        /// </summary>
        private static Font TOP_FONT = new Font(FontFamily.GenericSansSerif, 8.0f, FontStyle.Bold);

        /// <summary>
        ///     The font used to display the bottom text
        /// </summary>
        private static Font BOTTOM_FONT = new Font(FontFamily.GenericSansSerif, 8.0f);

        /// <summary>
        ///     Draws a single event
        /// </summary>
        /// <param name="pe"></param>
        /// <param name="evt"></param>
        /// <param name="X">The column in which the event should be drawn</param>
        /// <param name="Y">The line in which the event should be drawn</param>
        private void drawEvent(PaintEventArgs pe, ModelEvent evt, Rectangle bounds)
        {
            EventDisplayAttributes attributes = GetDisplayAttributes(pe.Graphics, evt);
            int CornerRadius = 5;

            StepActivation stepActivation = evt as StepActivation;
            if (stepActivation != null)
            {
                string name = GUIUtils.AdjustForDisplay(pe.Graphics, stepActivation.Step.Name, bounds.Width - 4,
                    TOP_FONT);
                pe.Graphics.FillRectangle(STEP_BOX_PEN, bounds);
                pe.Graphics.DrawString(
                    name,
                    TOP_FONT,
                    new SolidBrush(STEP_BOX_COLOR),
                    new Rectangle(new Point(bounds.Left + 2, bounds.Top + 2),
                        new Size(bounds.Width - 4, Bounds.Height - 4)));
                pe.Graphics.DrawLine(new Pen(STEP_BOX_COLOR), bounds.Left, bounds.Bottom - 1, bounds.Right - 1,
                    bounds.Bottom - 1);
            }
            else
            {
                SubStepActivated subStepActivation = evt as SubStepActivated;
                if (subStepActivation != null)
                {
                    pe.Graphics.FillRectangle(new SolidBrush(attributes.FillColor), bounds);
                    if (subStepActivation.SubStep.EnclosingCollection != null)
                    {
                        int index = subStepActivation.SubStep.EnclosingCollection.IndexOf(subStepActivation.SubStep) + 1;
                        if (index == 1)
                        {
                            pe.Graphics.DrawString("Substep", BOTTOM_FONT, new SolidBrush(attributes.DrawPen.Color),
                                new Point(bounds.Left + 2, bounds.Top + 2));
                        }
                        pe.Graphics.DrawString("" + index, BOTTOM_FONT, new SolidBrush(attributes.DrawPen.Color),
                            new Point(bounds.Left + bounds.Width/2, bounds.Bottom - 2 - BOTTOM_FONT.Height));
                    }
                }
                else
                {
                    int strokeOffset = Convert.ToInt32(Math.Ceiling(attributes.DrawPen.Width));
                    bounds = Rectangle.Inflate(bounds, -strokeOffset, -strokeOffset);

                    attributes.DrawPen.EndCap = attributes.DrawPen.StartCap = LineCap.Round;

                    GraphicsPath gfxPath = new GraphicsPath();
                    gfxPath.AddArc(bounds.X, bounds.Y, CornerRadius, CornerRadius, 180, 90);
                    gfxPath.AddArc(bounds.X + bounds.Width - CornerRadius, bounds.Y, CornerRadius, CornerRadius, 270, 90);
                    gfxPath.AddArc(bounds.X + bounds.Width - CornerRadius, bounds.Y + bounds.Height - CornerRadius,
                        CornerRadius, CornerRadius, 0, 90);
                    gfxPath.AddArc(bounds.X, bounds.Y + bounds.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
                    gfxPath.CloseAllFigures();

                    pe.Graphics.FillPath(new SolidBrush(attributes.FillColor), gfxPath);
                    pe.Graphics.DrawPath(attributes.DrawPen, gfxPath);

                    pe.Graphics.DrawString(attributes.BottomText, BOTTOM_FONT, new SolidBrush(attributes.DrawPen.Color),
                        new Point(bounds.Left + 2, bounds.Bottom - 2 - BOTTOM_FONT.Height));

                    if (attributes.LeftIconImageIndex >= 0)
                    {
                        pe.Graphics.DrawImage(Images.Images[attributes.LeftIconImageIndex], bounds.Left + 4,
                            bounds.Top + 4, 20, 20);
                    }

                    if (attributes.RightIconImageIndex >= 0)
                    {
                        pe.Graphics.DrawImage(Images.Images[attributes.RightIconImageIndex], bounds.Right - 4 - 20,
                            bounds.Top + 4, 20, 20);
                    }

                    if (attributes.RightIconImageIndex >= 0 && attributes.RightIconModifierImageIndex >= 0)
                    {
                        pe.Graphics.DrawImage(Images.Images[attributes.RightIconModifierImageIndex],
                            bounds.Right - 4 - 30, bounds.Top + 10, 16, 16);
                    }

                    int shift = 0;
                    foreach (int index in attributes.TopRightIconImageIndex)
                    {
                        pe.Graphics.DrawImage(Images.Images[index], bounds.Right - 16 + shift, bounds.Top, 16, 16);
                        shift = shift - 16;
                    }
                }
            }
        }

        /// <summary>
        ///     The color used to display a step box
        /// </summary>
        private static Color STEP_BOX_COLOR = Color.Black;

        /// <summary>
        ///     The pen used to display a step box
        /// </summary>
        private static Brush STEP_BOX_PEN = new SolidBrush(Color.LightGray);

        /// <summary>
        ///     Reduces the string to the most important thing in it
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ShortName(string text)
        {
            string retVal = text.Trim();

            // Remove leading prefixes
            string prefix = "";
            int parentCount = 0;
            for (int i = 0; i < retVal.Length; i++)
            {
                if (retVal[i] == '(')
                {
                    if (string.IsNullOrEmpty(prefix))
                    {
                        parentCount += 1;
                    }
                    else
                    {
                        break;
                    }
                }
                if (retVal[i] == ')')
                {
                    parentCount -= 1;
                }

                if (parentCount == 0)
                {
                    if (retVal[i] == '.')
                    {
                        prefix = "";
                    }
                    else
                    {
                        if (char.IsSeparator(retVal, i))
                        {
                            break;
                        }
                        else
                        {
                            prefix = prefix + retVal[i];
                        }
                    }
                }
                else
                {
                    prefix = prefix + retVal[i];
                }
            }
            retVal = prefix;

            return retVal;
        }
    }
}