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
using DataDictionary.Generated;
using Action = DataDictionary.Rules.Action;
using RuleCondition = DataDictionary.Rules.RuleCondition;

namespace GUI.DataDictionaryView
{
    public class ActionsTreeNode : RuleConditionTreeNode
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public ActionsTreeNode(RuleCondition item, bool buildSubNodes)
            : base(item, buildSubNodes, "Actions", true, false)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            Nodes.Clear();

            foreach (Action action in Item.Actions)
            {
                Nodes.Add(new ActionTreeNode(action, buildSubNodes));
            }
            if (Item.EnclosingRule != null && !Item.EnclosingRule.BelongsToAProcedure())
            {
                SortSubNodes();
            }
            SubNodesBuilt = true;
        }

        public override ActionTreeNode AddAction(Action action)
        {
            ActionTreeNode retVal = new ActionTreeNode(action, true);
            Item.appendActions(action);

            Nodes.Add(retVal);
            if (Item.EnclosingRule != null && !Item.EnclosingRule.BelongsToAProcedure())
            {
                SortSubNodes();
            }

            Item.setVerified(false);
            return retVal;
        }

        public void AddHandler(object sender, EventArgs args)
        {
            Action action = (Action) acceptor.getFactory().createAction();
            action.ExpressionText = "";
            AddAction(action);
        }

        public void AddCustomHandler(object sender, EventArgs args)
        {
            CustomAction customAction = new CustomAction(Item.EnclosingStructure);
            customAction.CreateCustomAction = AddAction;
            customAction.ShowDialog();
        }

        /// <summary>
        ///     Handles a drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            if (SourceNode is ActionTreeNode)
            {
                if (
                    MessageBox.Show("Are you sure you want to move the corresponding action ?", "Move action",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ActionTreeNode actionTreeNode = (ActionTreeNode) SourceNode;

                    Action action = actionTreeNode.Item;
                    actionTreeNode.Delete();
                    AddAction(action);
                }
            }
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add", new EventHandler(AddHandler)));
            retVal.Add(new MenuItem("Add custom...", new EventHandler(AddCustomHandler)));

            return retVal;
        }
    }
}