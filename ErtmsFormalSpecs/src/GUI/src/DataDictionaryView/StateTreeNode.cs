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
using DataDictionary.Constants;
using DataDictionary.Rules;
using DataDictionary.Types;
using GUI.Converters;
using GUI.LongOperations;
using GUI.Properties;
using GUI.StateDiagram;
using Action = DataDictionary.Rules.Action;

namespace GUI.DataDictionaryView
{
    public class StateTreeNode : ReqRelatedTreeNode<State>
    {
        private class InternalStateTypeConverter : StateTypeConverter
        {
            public override StandardValuesCollection
                GetStandardValues(ITypeDescriptorContext context)
            {
                return GetValues(((ItemEditor) context.Instance).Item);
            }
        }

        private class ItemEditor : ReqRelatedEditor
        {
            [Category("Default")]
            public override string Name
            {
                get { return Item.Name; }
                set { Item.Name = value; }
            }

            [Category("Default"), TypeConverter(typeof (InternalStateTypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public string InitialState
            {
                get { return Item.StateMachine.Default; }
                set { Item.StateMachine.Default = value; }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public StateTreeNode(State item, bool buildSubNodes)
            : base(item, buildSubNodes, null, false, true)
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

            subNodes.Add(new StateSubStatesTreeNode(Item, recursive));
            subNodes.Add(new StateRulesTreeNode(Item, recursive));

            if (Item.getEnterAction() != null)
            {
                subNodes.Add(new RuleTreeNode((Rule) Item.getEnterAction(), recursive));
            }
            if (Item.getLeaveAction() != null)
            {
                subNodes.Add(new RuleTreeNode((Rule) Item.getLeaveAction(), recursive));
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

        public void AddStateHandler(object sender, EventArgs args)
        {
            Item.StateMachine.appendStates(State.CreateDefault(Item.StateMachine.States));
        }

        public void AddTransitionHandler(object sender, EventArgs args)
        {
            SelectStartAndTargetStateForTransition dialog = new SelectStartAndTargetStateForTransition();
            dialog.SetStateMachine(Item.EnclosingStateMachine, Item);
            dialog.ShowDialog(GuiUtils.MdiWindow);

            if (dialog.OkCkicked)
            {
                State sourceState = Item.EnclosingStateMachine.FindState(dialog.StartStateName);
                if (sourceState != null)
                {
                    Rule rule = Rule.CreateDefault(sourceState.StateMachine.Rules);
                    sourceState.StateMachine.appendRules(rule);
                    RuleCondition ruleCondition = (RuleCondition) rule.RuleConditions[0];

                    Action action = Action.CreateDefault(ruleCondition.Actions);
                    action.ExpressionText = "THIS <- " + dialog.EndStateName;
                    ruleCondition.appendActions(action);

                    RefreshModel.Execute();
                }
            }
        }

        public void AddEnterActionHandler(object sender, EventArgs args)
        {
            Item.setEnterAction(Rule.CreateDefault(null));
            Item.getEnterAction().Name = "Enter action";
        }

        public void AddLeaveActionHandler(object sender, EventArgs args)
        {
            Item.setLeaveAction(Rule.CreateDefault(null));
            Item.getLeaveAction().Name = "Leave action";
        }

        /// <summary>
        ///     Handles the drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            StateMachineTreeNode stateMachineTreeNode = sourceNode as StateMachineTreeNode;
            if (stateMachineTreeNode != null)
            {
                if (
                    MessageBox.Show("Are you sure you want to override the state machine ? ", "Override state machine",
                        MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    StateMachine stateMachine = stateMachineTreeNode.Item;
                    stateMachineTreeNode.Delete();
                    Item.StateMachine = stateMachine;
                }
            }

            base.AcceptDrop(sourceNode);
        }

        /// <summary>
        ///     Display the associated state diagram
        /// </summary>
        public void ViewDiagram()
        {
            StateDiagramWindow window = new StateDiagramWindow();
            GuiUtils.MdiWindow.AddChildWindow(window);
            window.StatePanel.SetStateMachine(Item.StateMachine);
            window.Text = Item.Name + @" " + Resources.StateTreeNode_ViewDiagram_state_diagram;
        }

        protected void ViewStateDiagramHandler(object sender, EventArgs args)
        {
            ViewDiagram();
        }

        public override void DoubleClickHandler()
        {
            ViewDiagram();
        }

        protected override ModelElement FindOrCreateUpdate()
        {
            ModelElement retVal = null;

            Dictionary dictionary = GetPatchDictionary();
            if (dictionary != null)
            {
                retVal = dictionary.FindByFullName(Item.FullName) as ModelElement;
                if (retVal == null)
                {
                    // If the element does not already exist in the patch, add a copy to it
                    retVal = Item.CreateStateUpdate(dictionary);
                }
                // Navigate to the element, whether it was created or not
                EfsSystem.Instance.Context.SelectElement(retVal, this, Context.SelectionCriteria.DoubleClick);
            }

            return retVal;
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>
            {
                new MenuItem("Add sub state", AddStateHandler)
            };

            MenuItem updatesItem = new MenuItem("Update...");
            updatesItem.MenuItems.Add(new MenuItem("Update", AddUpdate));
            updatesItem.MenuItems.Add(new MenuItem("Remove", RemoveInUpdate));
            retVal.Add(updatesItem);
            retVal.Add(new MenuItem("Delete", DeleteHandler));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(6, new MenuItem("-"));
            retVal.Insert(7, new MenuItem("Add transition", AddTransitionHandler));
            retVal.Insert(8, new MenuItem("Add enter action", AddEnterActionHandler));
            retVal.Insert(9, new MenuItem("Add leave action", AddLeaveActionHandler));
            retVal.Insert(10, new MenuItem("-"));
            retVal.Insert(11, new MenuItem("View state diagram", ViewStateDiagramHandler));

            return retVal;
        }
    }
}