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
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Rules;
using DataDictionary.Variables;
using GUI.BoxArrowDiagram;
using Utils;
using Action = DataDictionary.Rules.Action;
using Rule = DataDictionary.Rules.Rule;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using State = DataDictionary.Constants.State;
using StateMachine = DataDictionary.Types.StateMachine;

namespace GUI.StateDiagram
{
    public class StatePanel : BoxArrowPanel<StateMachine, State, Transition>
    {
        private ToolStripMenuItem _addStateMenuItem;
        private ToolStripMenuItem _addTransitionMenuItem;
        private ToolStripSeparator _toolStripSeparator;
        private ToolStripMenuItem _deleteMenuItem;

        /// <summary>
        ///     Initializes the start menu
        /// </summary>
        public override void InitializeStartMenu()
        {
            base.InitializeStartMenu();

            _addStateMenuItem = new ToolStripMenuItem();
            _addTransitionMenuItem = new ToolStripMenuItem();
            _toolStripSeparator = new ToolStripSeparator();
            _deleteMenuItem = new ToolStripMenuItem();
            // 
            // addStateMenuItem
            // 
            _addStateMenuItem.Name = "addStateMenuItem";
            _addStateMenuItem.Size = new Size(161, 22);
            _addStateMenuItem.Text = "Add State";
            _addStateMenuItem.Click += addBoxMenuItem_Click;
            // 
            // addTransitionMenuItem
            // 
            _addTransitionMenuItem.Name = "addTransitionMenuItem";
            _addTransitionMenuItem.Size = new Size(161, 22);
            _addTransitionMenuItem.Text = "Add transition";
            _addTransitionMenuItem.Click += addArrowMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            _toolStripSeparator.Name = "toolStripSeparator1";
            _toolStripSeparator.Size = new Size(158, 6);
            // 
            // toolStripMenuItem1
            // 
            _deleteMenuItem.Name = "toolStripMenuItem1";
            _deleteMenuItem.Size = new Size(153, 22);
            _deleteMenuItem.Text = "Delete selected";
            _deleteMenuItem.Click += deleteMenuItem1_Click;

            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                _addStateMenuItem,
                _addTransitionMenuItem,
                _toolStripSeparator,
                _deleteMenuItem
            });
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public StatePanel()
        {
            InitializeStartMenu();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public StatePanel(IContainer container)
        {
            container.Add(this);

            InitializeStartMenu();
        }

        /// <summary>
        ///     Method used to create a box
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override BoxControl<StateMachine, State, Transition> CreateBox(State model)
        {
            BoxControl<StateMachine, State, Transition> retVal = new StateControl();
            retVal.Model = model;

            return retVal;
        }

        /// <summary>
        ///     Method used to create an arrow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ArrowControl<StateMachine, State, Transition> CreateArrow(Transition model)
        {
            ArrowControl<StateMachine, State, Transition> retVal = new TransitionControl();
            retVal.Model = model;

            return retVal;
        }
        
        /// <summary>
        ///     The expressiong required to get the state machine variable (if any) displayed by this panel
        /// </summary>
        public Expression StateMachineVariableExpression { get; set; }

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

        private void addBoxMenuItem_Click(object sender, EventArgs e)
        {
            Model.appendStates(State.CreateDefault(Model.States));
        }

        private void addArrowMenuItem_Click(object sender, EventArgs e)
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
                action.ExpressionText = "THIS <- " + ((State)Model.States[1]).LiteralName;
                ruleCondition.appendActions(action);
                State sourceState = Model.States[0] as State;
                if (sourceState != null)
                {
                    sourceState.StateMachine.appendRules(rule);
                }

                EFSSystem.INSTANCE.Context.SelectElement(action, this, Context.SelectionCriteria.LeftClick);
            }
        }

        private void deleteMenuItem1_Click(object sender, EventArgs e)
        {
            IModelElement model = null;

            if (Selected is BoxControl<StateMachine, State, Transition>)
            {
                model = (Selected as BoxControl<StateMachine, State, Transition>).Model;
            }
            else if (Selected is ArrowControl<StateMachine, State, Transition>)
            {
                ArrowControl<StateMachine, State, Transition> control = Selected as ArrowControl<StateMachine, State, Transition>;
                RuleCondition ruleCondition = control.Model.RuleCondition;
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

            if (GuiUtils.MdiWindow.DataDictionaryWindow != null)
            {
                BaseTreeNode node = GuiUtils.MdiWindow.DataDictionaryWindow.FindNode(model);
                if (node != null)
                {
                    node.Delete();
                }
            }
        }
    }
}