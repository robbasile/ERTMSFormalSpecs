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
using DataDictionary.Generated;
using DataDictionary.Interpreter.Statement;
using Utils;
using State = DataDictionary.Constants.State;
using StateMachine = DataDictionary.Types.StateMachine;

namespace DataDictionary.Rules
{
    /// <summary>
    ///     Transitions are translations of rules which have the following properties
    ///     - there is an action in the rule (only direct) which modifies a state of the the state machine
    ///     - if there is a pre condition of the rule (either direct or inherited from parent rule)
    ///     which refers to a state S, this provvides the initial state of the transition. Otherwise,
    ///     there is no initial state
    /// </summary>
    public class Transition : IGraphicalArrow<State>
    {
        /// <summary>
        ///     The pre condition associated to this transaction
        /// </summary>
        public PreCondition PreCondition { get; private set; }

        /// <summary>
        ///     The statment which modifies the state machine's state
        /// </summary>
        public VariableUpdateStatement Update { get; private set; }

        /// <summary>
        ///     The action associated to this transaction
        /// </summary>
        public Action Action
        {
            get
            {
                if (Update != null)
                {
                    return (Action) Update.Root;
                }
                return null;
            }
        }

        /// <summary>
        ///     The rule associated to this transition
        /// </summary>
        public RuleCondition RuleCondition
        {
            get
            {
                if (Action != null)
                {
                    return Action.RuleCondition;
                }
                return null;
            }
        }

        /// <summary>
        ///     The initial state associated to this transition
        /// </summary>
        public State Source { get; private set; }

        /// <summary>
        ///     The target state associated to this transition
        /// </summary>
        public State Target { get; private set; }

        /// <summary>
        ///     The model element which is referenced by this transition
        /// </summary>
        public ModelElement ReferencedModel
        {
            get { return RuleCondition; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="preCondition">The precondition which setup the initial state</param>
        /// <param name="initialState">The initial stae of this transition</param>
        /// <param name="update">The statement which set up the target state</param>
        /// <param name="targetState">The target state of this transition</param>
        public Transition(PreCondition preCondition, State initialState, VariableUpdateStatement update,
            State targetState)
        {
            PreCondition = preCondition;
            Source = initialState;
            Update = update;
            Target = targetState;
        }

        public class CannotChangeRuleException : Exception
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="message"></param>
            public CannotChangeRuleException(string message)
                : base(message)
            {
            }
        }

        /// <summary>
        ///     Updates (if possible) the initial state for this transition
        /// </summary>
        /// <param name="initialBox"></param>
        public void SetInitialBox(IGraphicalDisplay initialBox)
        {
            State initialState = (State) initialBox;

            if (Action != null)
            {
                if (PreCondition != null)
                {
                    if (PreCondition.Rule == Action.Rule)
                    {
                        List<State> states = StateMachine.GetStates(PreCondition.Expression);
                        if (states.Count == 1)
                        {
                            PreCondition.ExpressionText = "THIS == " + initialState.FullName;
                            Source = initialState;
                        }
                        else
                        {
                            throw new CannotChangeRuleException("More than one state in the pre condition expression");
                        }
                    }
                    else
                    {
                        throw new CannotChangeRuleException("Precondition is not at the same level as the action");
                    }
                }
                else
                {
                    RuleCondition ruleCondition = Action.Enclosing as RuleCondition;
                    if (ruleCondition != null)
                    {
                        Rule rule = ruleCondition.EnclosingRule;

                        if (EnclosingFinder<State>.find(rule) == Source)
                        {
                            if (rule.RuleConditions.Count == 1)
                            {
                                Source.StateMachine.removeRules(rule);
                                Source = initialState;
                                Source.StateMachine.appendRules(rule);
                            }
                            else
                            {
                                rule.removeConditions(ruleCondition);
                                Source = initialState;
                                Rule newRule = (Rule) acceptor.getFactory().createRule();
                                newRule.Name = rule.Name;
                                newRule.appendConditions(ruleCondition);
                                Source.StateMachine.appendRules(newRule);
                            }
                        }
                        else
                        {
                            throw new CannotChangeRuleException("Action is not defined directly in the state");
                        }
                    }
                }
            }
            Source = initialState;
        }

        /// <summary>
        ///     Updates (if possible) the target state of this transition
        /// </summary>
        /// <param name="targetBox"></param>
        public void SetTargetBox(IGraphicalDisplay targetBox)
        {
            State targetState = (State) targetBox;

            if (Action != null)
            {
                Action.ExpressionText = "THIS <- " + targetState.LiteralName;
            }
            Target = targetState;
        }

        /// <summary>
        ///     Provides the name of the target state
        /// </summary>
        /// <returns></returns>
        public string GetTargetStateName()
        {
            string retVal = "<Unknown>";

            State targetState = Target;
            if (targetState != null)
            {
                retVal = targetState.FullName;
            }
            else
            {
                targetState = Update.Expression.Ref as State;
                if (targetState != null)
                {
                    retVal = targetState.LiteralName;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The name to be displayed
        /// </summary>
        public string GraphicalName
        {
            get
            {
                string retVal;

                if (RuleCondition != null)
                {
                    if ((RuleCondition.Name == null) || (RuleCondition.Name.Equals("")))
                    {
                        retVal = UnnamedTransitionName;
                    }
                    else
                    {
                        retVal = RuleCondition.Name;
                    }
                }
                else
                {
                    retVal = InitialTransitionName;
                }

                return retVal;
            }
        }

        /// <summary>
        /// The name of the initial transition
        /// </summary>
        public static string InitialTransitionName = "Initial state";

        /// <summary>
        /// The name of an unnamed transition
        /// </summary>
        public static string UnnamedTransitionName = "<Unnamed transition>";
    }
}