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
        /// <param name="buildSubNodes"></param>
        public ActionsTreeNode(RuleCondition item, bool buildSubNodes)
            : base(item, buildSubNodes, "Actions", false)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            // Do not use the base version
            SubNodesBuilt = true;

            foreach (Action action in Item.Actions)
            {
                subNodes.Add(new ActionTreeNode(action, recursive));
            }
            if (Item.EnclosingRule != null && !Item.EnclosingRule.BelongsToAProcedure())
            {
                subNodes.Sort();
            }
        }
        
        public void AddHandler(object sender, EventArgs args)
        {
            Item.appendActions(Action.CreateDefault(Item.Actions));
            Item.setVerified(false);
        }

        /// <summary>
        ///     Handles a drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            ActionTreeNode actionTreeNode = sourceNode as ActionTreeNode;
            if (actionTreeNode != null)
            {
                if (
                    MessageBox.Show("Are you sure you want to move the corresponding action ?", "Move action",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Action action = actionTreeNode.Item;
                    actionTreeNode.Delete();
                    Item.appendActions(action);
                    Item.setVerified(false);
                }
            }
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Add", AddHandler)};

            return retVal;
        }
    }
}