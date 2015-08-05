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
using GUI.Properties;
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
            [Category("Default"), TypeConverter(typeof (InternalStateTypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public string InitialState
            {
                get { return Item.Default; }
                set { Item.Default = value; }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public StateMachineTreeNode(StateMachine item, bool buildSubNodes)
            : base(item, buildSubNodes)
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

            subNodes.Add(new StateMachineStatesTreeNode(Item, recursive));
            subNodes.Add(new StateMachineRulesTreeNode(Item, recursive));
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
            Item.appendStates(State.CreateDefault(Item.States));
        }
        
        public void AddRuleHandler(object sender, EventArgs args)
        {
            Item.appendRules(Rule.CreateDefault(Item.Rules));
        }

        /// <summary>
        ///     Display the associated state diagram
        /// </summary>
        public void ViewDiagram()
        {
            StateDiagramWindow window = new StateDiagramWindow();
            GuiUtils.MdiWindow.AddChildWindow(window);
            window.SetStateMachine(Item);
            window.Text = Item.Name + @" " + Resources.StateMachineTreeNode_ViewDiagram_state_diagram;
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
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            StateMachineTreeNode stateMachineTreeNode = sourceNode as StateMachineTreeNode;
            if (stateMachineTreeNode != null)
            {
                if (
                    MessageBox.Show(
                        Resources.StateMachineTreeNode_AcceptDrop_Are_you_sure_you_want_to_override_the_state_machine___, 
                        Resources.StateMachineTreeNode_AcceptDrop_Override_state_machine,
                        MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
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

            base.AcceptDrop(sourceNode);
        }

        /// <summary>
        /// Finds or creates an update for the current element.
        /// </summary>
        /// <returns></returns>
        protected override ModelElement FindOrCreateUpdate()
        {
            ModelElement retVal = null;

            Dictionary dictionary = GetPatchDictionary();
            if (dictionary != null)
            {
                retVal = dictionary.findByFullName(Item.FullName) as ModelElement;
                if (retVal == null)
                {
                    // If the element does not already exist in the patch, add a copy to it
                    retVal = Item.CreateStateMachineUpdate(dictionary);
                }
                // Navigate to the element, whether it was created or not
                EFSSystem.INSTANCE.Context.SelectElement(retVal, this, Context.SelectionCriteria.DoubleClick);
            }

            return retVal;
        }


        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("State", AddStateHandler));
            newItem.MenuItems.Add(new MenuItem("Rule", AddRuleHandler));
            retVal.Add(newItem);

            MenuItem updateItem = new MenuItem("Update...");
            updateItem.MenuItems.Add(new MenuItem("Update", AddUpdate));
            updateItem.MenuItems.Add(new MenuItem("Remove", RemoveInUpdate));
            retVal.Add(updateItem);

            retVal.Add(new MenuItem("Delete", DeleteHandler));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(6, new MenuItem("-"));
            retVal.Insert(7, new MenuItem("View state diagram", ViewStateDiagramHandler));

            return retVal;
        }
    }
}