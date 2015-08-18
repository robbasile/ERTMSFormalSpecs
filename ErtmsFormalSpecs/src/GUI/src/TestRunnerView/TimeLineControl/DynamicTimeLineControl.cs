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
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Interpreter;
using DataDictionary.Tests;
using DataDictionary.Tests.Runner;
using DataDictionary.Tests.Runner.Events;
using Utils;

namespace GUI.TestRunnerView.TimeLineControl
{
    /// <summary>
    ///     The static time line according to a TimeLine
    /// </summary>
    public class DynamicTimeLineControl : TimeLineControl
    {
        /// <summary>
        ///     The time line handled by this window
        /// </summary>
        public EventTimeLine TimeLine
        {
            get
            {
                EventTimeLine retVal = null;

                Runner runner = EFSSystem.INSTANCE.Runner;
                if (runner != null)
                {
                    retVal = runner.EventTimeLine;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public DynamicTimeLineControl()
        {
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(new MenuItem("Configure filter...", OpenFilter));
            DrawArea.DoubleClick += TimeLineControl_DoubleClick;

            FilterConfiguration = new FilterConfiguration();
        }

        /// <summary>
        ///     The time line filtering configuration
        /// </summary>
        public FilterConfiguration FilterConfiguration { get; private set; }

        /// <summary>
        ///     Opens the filtering dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFilter(object sender, EventArgs e)
        {
            Filtering filtering = new Filtering();
            filtering.Configure(GuiUtils.MdiWindow.EfsSystem, FilterConfiguration);
            filtering.ShowDialog(this);
            filtering.UpdateConfiguration(FilterConfiguration);
            CleanEventPositions();
            if (TimeLine != null)
            {
                TimeLine.Changed = true;
            }
            Refresh();
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
                ExplanationPart explain = variableUpdate.Explanation;
                ExplainBox explainTextBox = new ExplainBox();
                explainTextBox.SetExplanation(explain);
                GuiUtils.MdiWindow.AddChildWindow(explainTextBox);
            }

            RuleFired rulefired = evt as RuleFired;
            if (rulefired != null)
            {
                ExplanationPart explain = rulefired.Explanation;
                ExplainBox explainTextBox = new ExplainBox();
                explainTextBox.SetExplanation(explain);
                GuiUtils.MdiWindow.AddChildWindow(explainTextBox);
            }

            Expect expect = evt as Expect;
            if (expect != null)
            {
                Expectation expectation = expect.Expectation;

                if (expectation != null)
                {
                    ExplanationPart explanation = expect.Explanation;
                    if (explanation == null)
                    {
                        explanation = expectation.Explain;
                    }

                    ExplainBox explainTextBox = new ExplainBox();
                    explainTextBox.SetExplanation(explanation);
                    GuiUtils.MdiWindow.AddChildWindow(explainTextBox);
                }
            }

            ModelInterpretationFailure modelInterpretationFailure = evt as ModelInterpretationFailure;
            if (modelInterpretationFailure != null)
            {
                ExplainBox explainTextBox = new ExplainBox();
                explainTextBox.SetExplanation(modelInterpretationFailure.Explanation);
                GuiUtils.MdiWindow.AddChildWindow(explainTextBox);
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

            return retVal;
        }

        /// <summary>
        ///     Refreshes the view
        /// </summary>
        public override void Refresh()
        {
            if (TimeLine == null)
            {
                UpdatePositionHandler();
            }
            else
            {
                if (TimeLine != null && TimeLine.Changed)
                {
                    TimeLine.Changed = false;
                    UpdatePositionHandler();
                    HandledEvents = TimeLine.Events.Count;
                    base.Refresh();
                }
            }
        }

        /// <summary>
        ///     Update the information stored in the position handler according to the time line
        /// </summary>
        protected override void UpdatePositionHandler()
        {
            PositionHandler.CleanPositions();
            if (TimeLine != null)
            {
                List<ModelEvent> events = new List<ModelEvent>(TimeLine.Events);
                foreach (ModelEvent evt in events)
                {
                    if (FilterConfiguration.VisibleEvent(evt) || evt is SubStepActivated)
                    {
                        PositionHandler.RegisterEvent(evt);
                    }
                }
            }

            base.UpdatePositionHandler();

            HorizontalScroll.Value = Math.Max(0, DrawArea.Size.Width - Size.Width);
        }
    }
}