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

using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Constants;
using DataDictionary.Rules;
using DataDictionary.Tests.Runner;
using DataDictionary.Types;
using GUI.BoxArrowDiagram;
using Utils;

namespace GUI.StateDiagram
{
    public class TransitionControl : ArrowControl<StateMachine, State, Transition>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public TransitionControl(StatePanel panel, Transition model)
            : base(panel, model)
        {
        }

        /// <summary>
        ///     Indicates that the arrow should be displayed in the DEDUCED color
        /// </summary>
        /// <returns></returns>
        public override bool IsDeduced()
        {
            bool retVal = base.IsDeduced();

            if (!retVal)
            {
                if (TypedModel.GraphicalName.CompareTo(Transition.InitialTransitionName) != 0)
                {
                    StateMachine transitionStateMachine = EnclosingFinder<StateMachine>.find(TypedModel.RuleCondition);
                    if (transitionStateMachine == null)
                    {
                        // A deduced case is a arrow that is not defined in any state machine
                        retVal = true;
                    }
                    else
                    {
                        StatePanel panel = (StatePanel) Panel;
                        if (TypedModel.RuleCondition != null &&
                            panel.Model.Rules.Contains(TypedModel.RuleCondition.EnclosingRule))
                        {
                            // A deduced case is a arrow that is defined in the rules of the state machines (not in its states)
                            retVal = true;
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates that the arrow should be displayed in the ACTIVE color
        /// </summary>
        /// <returns></returns>
        public override bool IsActive()
        {
            bool retVal = base.IsActive();

            if (!retVal)
            {
                if (TypedModel.RuleCondition != null)
                {
                    Runner runner = TypedModel.RuleCondition.EFSSystem.Runner;
                    if (runner != null)
                    {
                        StatePanel panel = (StatePanel) Panel;
                        if (runner.RuleActivatedAtTime(TypedModel.RuleCondition, runner.LastActivationTime,
                            panel.StateMachineVariable))
                        {
                            retVal = true;
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the name of the target state
        /// </summary>
        /// <returns></returns>
        public override string GetTargetName()
        {
            string retVal = "<Unknown>";

            if (TypedModel.Target != null)
            {
                retVal = TypedModel.Target.FullName;
            }
            else
            {
                State targetState = TypedModel.Update.Expression.Ref as State;
                if (targetState != null)
                {
                    retVal = targetState.LiteralName;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Handles a mouse click event on a graphical element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        public override void HandleClick(object sender, MouseEventArgs mouseEventArgs)
        {
            IModelElement model = TypedModel.RuleCondition;
            if (model != null)
            {
                Context.SelectionCriteria criteria = GuiUtils.SelectionCriteriaBasedOnMouseEvent(mouseEventArgs);
                EFSSystem.INSTANCE.Context.SelectElement(model, Panel, criteria);
            }
        }
    }
}