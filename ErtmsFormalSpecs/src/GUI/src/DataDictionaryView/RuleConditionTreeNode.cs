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
using DataDictionary.Rules;
using Action = DataDictionary.Rules.Action;

namespace GUI.DataDictionaryView
{
    public class RuleConditionTreeNode : ReqRelatedTreeNode<RuleCondition>
    {
        private class ItemEditor : ReqRelatedEditor
        {
        };

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public RuleConditionTreeNode(RuleCondition item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        /// <param name="name"></param>
        /// <param name="addRequirements"></param>
        public RuleConditionTreeNode(RuleCondition item, bool buildSubNodes, string name,
            bool addRequirements = true)
            : base(item, buildSubNodes, name, false, addRequirements)
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

            subNodes.Add(new RulePreConditionsTreeNode(Item, recursive));
            subNodes.Add(new ActionsTreeNode(Item, recursive));
            subNodes.Add(new SubRulesTreeNode(Item, recursive));
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        /// <summary>
        ///     Handles a drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);

            if (sourceNode is ActionTreeNode)
            {
                ActionsTreeNode actionsTreeNode = SubNode<ActionsTreeNode>();
                if (actionsTreeNode != null)
                {
                    actionsTreeNode.AcceptDrop(sourceNode);
                }
            }
            else if (sourceNode is PreConditionTreeNode)
            {
                RulePreConditionsTreeNode preConditionsTreeNode = SubNode<RulePreConditionsTreeNode>();
                if (preConditionsTreeNode != null)
                {
                    preConditionsTreeNode.AcceptDrop(sourceNode);
                }
            }
            else if (sourceNode is RuleTreeNode)
            {
                SubRulesTreeNode subRulesTreeNode = SubNode<SubRulesTreeNode>();
                if (subRulesTreeNode != null)
                {
                    subRulesTreeNode.AcceptDrop(sourceNode);
                }
            }
        }

        /// <summary>
        ///     Adds a precondition
        /// </summary>
        public void AddPreConditionHandler(object sender, EventArgs args)
        {
            Item.appendPreConditions(PreCondition.CreateDefault(Item.PreConditions));
        }

        /// <summary>
        ///     Adds an action
        /// </summary>
        public void AddActionHandler(object sender, EventArgs args)
        {
            Item.appendActions(Action.CreateDefault(Item.Actions));
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("Pre-condition", AddPreConditionHandler));
            newItem.MenuItems.Add(new MenuItem("Action", AddActionHandler));
            retVal.Add(newItem);
            retVal.Add(new MenuItem("Delete", DeleteHandler));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}