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
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Rules;
using DataDictionary.Variables;
using GUI.BoxArrowDiagram;
using Utils;
using Action = DataDictionary.Rules.Action;
using Dictionary = DataDictionary.Dictionary;
using Rule = DataDictionary.Rules.Rule;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using State = DataDictionary.Constants.State;
using StateMachine = DataDictionary.Types.StateMachine;

namespace GUI.StateDiagram
{
    public class StatePanel : BoxArrowPanel<StateMachine, State, Transition>
    {
        /// <summary>
        ///     Method used to create a box
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override BoxControl<StateMachine, State, Transition> CreateBox(State model)
        {
            return new StateControl(this, model);
        }

        /// <summary>
        ///     Method used to create an arrow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ArrowControl<StateMachine, State, Transition> CreateArrow(Transition model)
        {
            return new TransitionControl(this, model);
        }


        /// <summary>
        ///     Sets the state machine type
        /// </summary>
        /// <param name="stateMachine"></param>
        public void SetStateMachine(StateMachine stateMachine)
        {
            Model = stateMachine;

            Model = stateMachine;
            RefreshControl();
        }

        /// <summary>
        ///     Sets the state machine variable (and type)
        /// </summary>
        /// <param name="stateMachine">The state machine variable to display</param>
        /// <param name="stateMachineType">
        ///     The state machine type which should be displayed. If null, the default state machine is
        ///     displayed
        /// </param>
        public void SetStateMachine(IVariable stateMachine, StateMachine stateMachineType = null)
        {
            if (stateMachineType == null)
            {
                stateMachineType = stateMachine.Type as StateMachine;
            }

            if (stateMachineType != null)
            {
                Model = stateMachineType;
            }

            Model = Model;
            if (stateMachine != null)
            {
                StateMachineVariableExpression =
                    EFSSystem.INSTANCE.Parser.Expression(EnclosingFinder<Dictionary>.find(stateMachine),
                        stateMachine.FullName);
            }
            else
            {
                StateMachineVariableExpression = null;
            }
            RefreshControl();
        }

        /// <summary>
        ///     The expressiong required to get the state machine variable (if any) displayed by this panel
        /// </summary>
        private Expression StateMachineVariableExpression { get; set; }

        /// <summary>
        ///     Provides the state machine variable (if any)
        /// </summary>
        public IVariable StateMachineVariable
        {
            get
            {
                IVariable retVal = null;

                if (StateMachineVariableExpression != null)
                {
                    retVal = StateMachineVariableExpression.GetVariable(new InterpretationContext());
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the boxes that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<State> GetBoxes()
        {
            List<State> retVal = new List<State>();

            foreach (State state in Model.States)
            {
                retVal.Add(state);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the arrows that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<Transition> GetArrows()
        {
            return Model.Transitions;
        }

        /// <summary>
        ///     Builds teh context menu associated to either the selected element or the panel
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override ContextMenu BuildContextMenu(GraphicElement element)
        {
            ContextMenu retVal = base.BuildContextMenu(element);

            if (element == null)
            {
                if (retVal != null)
                {
                    ContextMenu tmp = retVal;
                    retVal = new ContextMenu();
                    foreach (MenuItem item in tmp.MenuItems)
                    {
                        if (item != null)
                        {
                            retVal.MenuItems.Add(item);
                        }
                    }
                }
                else
                {
                    retVal = new ContextMenu();
                }

                // 
                // addTransitionMenuItem
                // 
                MenuItem addTransitionMenuItem = new MenuItem {Name = "addTransitionMenuItem", Text = "Add transition"};
                addTransitionMenuItem.Click += HandleAddTransition;
                retVal.MenuItems.Add(addTransitionMenuItem);
                // 
                // toolStripMenuItem1
                // 
                MenuItem deleteMenuItem = new MenuItem {Name = "deleteMenuItem", Text = "Delete selected"};
                deleteMenuItem.Click += HandleDelete;
                retVal.MenuItems.Add(addTransitionMenuItem);
            }

            return retVal;
        }

        /// <summary>
        ///     Adds a transition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleAddTransition(object sender, EventArgs e)
        {
            if (Model.States.Count > 1)
            {
                ObjectFactory factory = (ObjectFactory) acceptor.getFactory();
                Rule rule = (Rule) factory.createRule();
                rule.Name = "Rule" + (Model.Rules.Count + 1);

                RuleCondition ruleCondition = (RuleCondition) factory.createRuleCondition();
                ruleCondition.Name = "RuleCondition" + (rule.RuleConditions.Count + 1);
                rule.appendConditions(ruleCondition);

                Action action = (Action) factory.createAction();
                action.ExpressionText = "THIS <- " + ((State) Model.States[1]).LiteralName;
                ruleCondition.appendActions(action);
                State sourceState = Model.States[0] as State;
                if (sourceState != null)
                {
                    sourceState.StateMachine.appendRules(rule);
                }

                EFSSystem.INSTANCE.Context.SelectElement(action, this, Context.SelectionCriteria.LeftClick);
            }
        }

        /// <summary>
        ///     Allow to delete a transition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleDelete(object sender, EventArgs e)
        {
            IModelElement model = null;

            if (Selected is BoxControl<StateMachine, State, Transition>)
            {
                model = (Selected as BoxControl<StateMachine, State, Transition>).TypedModel;
            }
            else if (Selected is ArrowControl<StateMachine, State, Transition>)
            {
                ArrowControl<StateMachine, State, Transition> control =
                    Selected as ArrowControl<StateMachine, State, Transition>;
                RuleCondition ruleCondition = control.TypedModel.RuleCondition;
                Rule rule = ruleCondition.EnclosingRule;
                if (rule.countConditions() == 1)
                {
                    model = rule;
                }
                else
                {
                    model = ruleCondition;
                }
            }

            if (model != null)
            {
                model.Delete();
            }
        }

        /// <summary>
        ///     Factory for BoxEditor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override BoxEditor<StateMachine, State, Transition> CreateBoxEditor(
            BoxControl<StateMachine, State, Transition> control)
        {
            return new StateEditor(control);
        }


        /// <summary>
        ///     Factory for arrow editor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override ArrowEditor<StateMachine, State, Transition> CreateArrowEditor(
            ArrowControl<StateMachine, State, Transition> control)
        {
            return new TransitionEditor(control);
        }
    }
}