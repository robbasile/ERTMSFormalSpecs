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
using GUI.StateDiagram;

namespace GUI.DataDictionaryView
{
    public class StateMachineTreeNode : ReqRelatedTreeNode<StateMachine>
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

            [Category("Default"), TypeConverter(typeof (InternalStateTypeConverter))]
            public string InitialState
            {
                get { return Item.Default; }
                set { Item.Default = value; }
            }
        }

        private StateMachineStatesTreeNode states;
        private StateMachineRulesTreeNode rules;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="children"></param>
        public StateMachineTreeNode(StateMachine item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            states = new StateMachineStatesTreeNode(Item, buildSubNodes);
            Nodes.Add(states);

            rules = new StateMachineRulesTreeNode(Item, buildSubNodes);
            Nodes.Add(rules);
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
            states.AddHandler(sender, args);
        }

        /// <summary>
        ///     Adds a new state
        /// </summary>
        /// <param name="state"></param>
        public StateTreeNode AddState(State state)
        {
            return states.AddState(state);
        }

        public void AddRuleHandler(object sender, EventArgs args)
        {
            rules.AddHandler(sender, args);
        }

        /// <summary>
        ///     Adds a new state
        /// </summary>
        /// <param name="state"></param>
        public RuleTreeNode AddRule(Rule rule)
        {
            return rules.AddRule(rule);
        }

        /// <summary>
        ///     Display the associated state diagram
        /// </summary>
        public void ViewDiagram()
        {
            StateDiagramWindow window = new StateDiagramWindow();
            GUIUtils.MDIWindow.AddChildWindow(window);
            window.SetStateMachine(Item);
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
                    if (Item.EnclosingState != null)
                    {
                        Item.EnclosingState.StateMachine = stateMachine;
                    }

                    // Update the view
                    TreeNode parent = Parent;
                    parent.Nodes.Remove(this);
                    parent.Nodes.Add(stateMachineTreeNode);
                }
            }

            base.AcceptDrop(SourceNode);
        }

        public void AddStateMachineUpdate(object sender, EventArgs args)
        {
            Dictionary dictionary = GetPatchDictionary();

            if (dictionary != null)
            {
                ModelElement updatedElement = dictionary.findByFullName(Item.FullName) as ModelElement;
                if (updatedElement == null)
                {
                    // If the element does not already exist in the patch, add a copy to it
                    updatedElement = Item.CreateStateMachineUpdate(dictionary);
                }
                // navigate to the state machine, whether it was created or not
                GUIUtils.MDIWindow.RefreshModel();
                GUIUtils.MDIWindow.Select(updatedElement);
            }
        }


        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("State", new EventHandler(AddStateHandler)));
            newItem.MenuItems.Add(new MenuItem("Rule", new EventHandler(AddRuleHandler)));
            newItem.MenuItems.Add(new MenuItem("-"));
            newItem.MenuItems.Add(new MenuItem("Update", new EventHandler(AddStateMachineUpdate)));
            retVal.Add(newItem);
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(6, new MenuItem("-"));
            retVal.Insert(7, new MenuItem("View state diagram", new EventHandler(ViewStateDiagramHandler)));

            return retVal;
        }
    }
}