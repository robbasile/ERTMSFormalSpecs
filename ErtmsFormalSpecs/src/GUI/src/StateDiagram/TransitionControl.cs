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

using System.ComponentModel;
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
        public TransitionControl()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public TransitionControl(IContainer container)
        {
            container.Add(this);
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
                if (Name.CompareTo("Initial State") != 0)
                {
                    StateMachine transitionStateMachine = EnclosingFinder<StateMachine>.find(Model.RuleCondition);
                    if (transitionStateMachine == null && Name.CompareTo("Initial State") != 0)
                    {
                        // A deduced case is a arrow that is not defined in any state machine
                        retVal = true;
                    }
                    else
                    {
                        StatePanel panel = (StatePanel) Panel;
                        if (Model.RuleCondition != null &&
                            panel.Model.Rules.Contains(Model.RuleCondition.EnclosingRule))
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
                if (Model.RuleCondition != null)
                {
                    Runner runner = Model.RuleCondition.EFSSystem.Runner;
                    if (runner != null)
                    {
                        StatePanel panel = (StatePanel) Panel;
                        if (runner.RuleActivatedAtTime(Model.RuleCondition, runner.LastActivationTime,
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

            if (Model.Target != null)
            {
                retVal = Model.Target.FullName;
            }
            else
            {
                State targetState = Model.Update.Expression.Ref as State;
                if (targetState != null)
                {
                    retVal = targetState.LiteralName;
                }
            }

            return retVal;
        }
    }
}