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
using GUI.DataDictionaryView;
using Utils;
using Action = DataDictionary.Rules.Action;
using Rule = DataDictionary.Rules.Rule;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using State = DataDictionary.Constants.State;
using StateMachine = DataDictionary.Types.StateMachine;

namespace GUI.StateDiagram
{
    public class StatePanel : BoxArrowPanel<State, Transition>
    {
        private ToolStripMenuItem addStateMenuItem;
        private ToolStripMenuItem addTransitionMenuItem;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripMenuItem deleteMenuItem;

        /// <summary>
        ///     Initializes the start menu
        /// </summary>
        public override void InitializeStartMenu()
        {
            base.InitializeStartMenu();

            addStateMenuItem = new ToolStripMenuItem();
            addTransitionMenuItem = new ToolStripMenuItem();
            toolStripSeparator = new ToolStripSeparator();
            deleteMenuItem = new ToolStripMenuItem();
            // 
            // addStateMenuItem
            // 
            addStateMenuItem.Name = "addStateMenuItem";
            addStateMenuItem.Size = new Size(161, 22);
            addStateMenuItem.Text = "Add State";
            addStateMenuItem.Click += new EventHandler(addBoxMenuItem_Click);
            // 
            // addTransitionMenuItem
            // 
            addTransitionMenuItem.Name = "addTransitionMenuItem";
            addTransitionMenuItem.Size = new Size(161, 22);
            addTransitionMenuItem.Text = "Add transition";
            addTransitionMenuItem.Click += new EventHandler(addArrowMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator.Name = "toolStripSeparator1";
            toolStripSeparator.Size = new Size(158, 6);
            // 
            // toolStripMenuItem1
            // 
            deleteMenuItem.Name = "toolStripMenuItem1";
            deleteMenuItem.Size = new Size(153, 22);
            deleteMenuItem.Text = "Delete selected";
            deleteMenuItem.Click += new EventHandler(deleteMenuItem1_Click);

            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                addStateMenuItem,
                addTransitionMenuItem,
                toolStripSeparator,
                deleteMenuItem
            });
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public StatePanel()
            : base()
        {
            InitializeStartMenu();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public StatePanel(IContainer container)
            : base()
        {
            container.Add(this);

            InitializeStartMenu();
        }

        /// <summary>
        ///     Method used to create a box
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override BoxControl<State, Transition> createBox(State model)
        {
            BoxControl<State, Transition> retVal = new StateControl();
            retVal.Model = model;

            return retVal;
        }

        /// <summary>
        ///     Method used to create an arrow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ArrowControl<State, Transition> createArrow(Transition model)
        {
            ArrowControl<State, Transition> retVal = new TransitionControl();
            retVal.Model = model;

            return retVal;
        }

        /// <summary>
        ///     The state machine displayed by this panel
        /// </summary>
        public StateMachine StateMachine
        {
            get { return Model as StateMachine; }
            set { Model = value; }
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
        public override List<State> getBoxes()
        {
            List<State> retVal = new List<State>();

            foreach (State state in StateMachine.States)
            {
                retVal.Add(state);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the arrows that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<Transition> getArrows()
        {
            return StateMachine.Transitions;
        }

        private void addBoxMenuItem_Click(object sender, EventArgs e)
        {
            State state = (State) acceptor.getFactory().createState();
            state.Name = "State" + (StateMachine.States.Count + 1);

            if (GUIUtils.MDIWindow.DataDictionaryWindow != null)
            {
                StateMachineTreeNode node =
                    GUIUtils.MDIWindow.DataDictionaryWindow.FindNode(StateMachine) as StateMachineTreeNode;
                if (node != null)
                {
                    node.AddState(state);
                }
                else
                {
                    StateMachine.appendStates(state);
                }
            }

            RefreshControl();
        }

        private void addArrowMenuItem_Click(object sender, EventArgs e)
        {
            if (StateMachine.States.Count > 1)
            {
                ObjectFactory factory = (ObjectFactory) acceptor.getFactory();
                Rule rule = (Rule) factory.createRule();
                rule.Name = "Rule" + (StateMachine.Rules.Count + 1);

                RuleCondition ruleCondition = (RuleCondition) factory.createRuleCondition();
                ruleCondition.Name = "RuleCondition" + (rule.RuleConditions.Count + 1);
                rule.appendConditions(ruleCondition);

                Action action = (Action) factory.createAction();
                action.ExpressionText = "THIS <- " + ((State) StateMachine.States[1]).LiteralName;
                ruleCondition.appendActions(action);

                foreach (Form form in GUIUtils.MDIWindow.SubForms)
                {
                    Window dataDictionaryWindows = form as Window;
                    if (dataDictionaryWindows != null)
                    {
                        StateTreeNode stateNode =
                            dataDictionaryWindows.FindNode((State) StateMachine.States[0]) as StateTreeNode;
                        if (stateNode != null)
                        {
                            if (!stateNode.SubNodesBuilt)
                            {
                                stateNode.BuildSubNodes(false);
                                stateNode.UpdateColor();
                            }
                            RuleTreeNode ruleNode = stateNode.Rules.AddRule(rule);
                            break;
                        }
                    }
                }

                RefreshControl();
                Refresh();

                ArrowControl<State, Transition> control = getArrowControl(ruleCondition);
                Select(control, false);
            }
        }

        private void deleteMenuItem1_Click(object sender, EventArgs e)
        {
            IModelElement model = null;

            if (Selected is BoxControl<State, Transition>)
            {
                model = (Selected as BoxControl<State, Transition>).Model;
            }
            else if (Selected is ArrowControl<State, Transition>)
            {
                ArrowControl<State, Transition> control = Selected as ArrowControl<State, Transition>;
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

            if (GUIUtils.MDIWindow.DataDictionaryWindow != null)
            {
                BaseTreeNode node = GUIUtils.MDIWindow.DataDictionaryWindow.FindNode(model);
                if (node != null)
                {
                    node.Delete();
                }
            }
            Select(null, false);

            RefreshControl();
            Refresh();
        }
    }
}