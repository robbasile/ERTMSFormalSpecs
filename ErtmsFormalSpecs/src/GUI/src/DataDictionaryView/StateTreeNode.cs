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
using DataDictionary.Generated;
using GUI.Converters;
using GUI.StateDiagram;
using Rule = DataDictionary.Rules.Rule;
using State = DataDictionary.Constants.State;
using StateMachine = DataDictionary.Types.StateMachine;

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
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            [Category("Default")]
            public override string Name
            {
                get { return Item.Name; }
                set { Item.Name = value; }
            }

            [Category("Default"), TypeConverter(typeof (InternalStateTypeConverter))]
            public string InitialState
            {
                get { return Item.StateMachine.Default; }
                set { Item.StateMachine.Default = value; }
            }
        }

        /// <summary>
        ///     The sub state machine
        /// </summary>
        public StateSubStatesTreeNode SubStates;

        public StateRulesTreeNode Rules;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public StateTreeNode(State item, bool buildSubNodes)
            : base(item, buildSubNodes, null, false, true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            SubStates = new StateSubStatesTreeNode(Item, buildSubNodes);
            Nodes.Add(SubStates);
            Rules = new StateRulesTreeNode(Item, buildSubNodes);
            Nodes.Add(Rules);

            if (Item.getEnterAction() != null)
            {
                Nodes.Add(new RuleTreeNode((Rule) Item.getEnterAction(), buildSubNodes));
            }
            if (Item.getLeaveAction() != null)
            {
                Nodes.Add(new RuleTreeNode((Rule) Item.getLeaveAction(), buildSubNodes));
            }
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        public void AddStateHandler(object sender, EventArgs args)
        {
            State state = (State) acceptor.getFactory().createState();
            state.Name = "State" + (GetNodeCount(true) + 1);
            Item.StateMachine.appendStates(state);
            Nodes.Add(new StateTreeNode(state, true));
            SortSubNodes();
        }

        public void AddEnterActionHandler(object sender, EventArgs args)
        {
            Rule rule = (Rule) acceptor.getFactory().createRule();
            rule.Name = "Enter action";
            Item.setEnterAction(rule);
            Nodes.Add(new RuleTreeNode(rule, true));
            SortSubNodes();
        }

        public void AddLeaveActionHandler(object sender, EventArgs args)
        {
            Rule rule = (Rule) acceptor.getFactory().createRule();
            rule.Name = "Leave action";
            Item.setLeaveAction(rule);
            Nodes.Add(new RuleTreeNode(rule, true));
            SortSubNodes();
        }

        /// <summary>
        ///     Handles the drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            if (SourceNode is StateMachineTreeNode)
            {
                if (
                    MessageBox.Show("Are you sure you want to override the state machine ? ", "Override state machine",
                        MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    StateMachineTreeNode stateMachineTreeNode = (StateMachineTreeNode) SourceNode;
                    StateMachine stateMachine = stateMachineTreeNode.Item;
                    stateMachineTreeNode.Delete();

                    // Update the model
                    Item.StateMachine = stateMachine;

                    // Update the view
                    TreeNode parent = Parent;
                    parent.Nodes.Remove(this);
                    parent.Nodes.Add(stateMachineTreeNode);
                }
            }

            base.AcceptDrop(SourceNode);
        }

        /// <summary>
        ///     Display the associated state diagram
        /// </summary>
        public void ViewDiagram()
        {
            StateDiagramWindow window = new StateDiagramWindow();
            GUIUtils.MDIWindow.AddChildWindow(window);
            window.SetStateMachine(Item.StateMachine);
            window.Text = Item.Name + " state diagram";
        }

        protected void ViewStateDiagramHandler(object sender, EventArgs args)
        {
            ViewDiagram();
        }

        public override void DoubleClickHandler()
        {
            ViewDiagram();
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add sub state", new EventHandler(AddStateHandler)));
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(6, new MenuItem("-"));
            retVal.Insert(7, new MenuItem("Add enter action", new EventHandler(AddEnterActionHandler)));
            retVal.Insert(8, new MenuItem("Add leave action", new EventHandler(AddLeaveActionHandler)));
            retVal.Insert(9, new MenuItem("-"));
            retVal.Insert(10, new MenuItem("View state diagram", new EventHandler(ViewStateDiagramHandler)));

            return retVal;
        }
    }
}