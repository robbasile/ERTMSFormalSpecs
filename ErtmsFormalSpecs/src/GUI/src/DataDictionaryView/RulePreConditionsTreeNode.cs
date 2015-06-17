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
using PreCondition = DataDictionary.Rules.PreCondition;
using RuleCondition = DataDictionary.Rules.RuleCondition;

namespace GUI.DataDictionaryView
{
    public class RulePreConditionsTreeNode : RuleConditionTreeNode
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="children"></param>
        public RulePreConditionsTreeNode(RuleCondition item, bool buildSubNodes)
            : base(item, buildSubNodes, "Pre conditions", true, false)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            Nodes.Clear();

            foreach (PreCondition preCondition in Item.PreConditions)
            {
                Nodes.Add(new PreConditionTreeNode(preCondition, buildSubNodes));
            }
            SubNodesBuilt = true;
        }

        /// <summary>
        ///     Adds a preCondition to the modelized item
        /// </summary>
        /// <param name="preCondition"></param>
        /// <returns></returns>
        public override PreConditionTreeNode AddPreCondition(PreCondition preCondition)
        {
            PreConditionTreeNode retVal = new PreConditionTreeNode(preCondition, true);

            Item.appendPreConditions(preCondition);
            Nodes.Add(retVal);
            SortSubNodes();

            Item.setVerified(false);

            return retVal;
        }

        public void AddHandler(object sender, EventArgs args)
        {
            PreCondition preCondition = (PreCondition) acceptor.getFactory().createPreCondition();
            preCondition.Condition = "";
            AddPreCondition(preCondition);
        }

        /// <summary>
        ///     Handles a drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            if (SourceNode is PreConditionTreeNode)
            {
                if (
                    MessageBox.Show("Are you sure you want to move the corresponding pre-condition ?",
                        "Move pre-condition", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    PreConditionTreeNode preConditionTreeNode = (PreConditionTreeNode) SourceNode;

                    PreCondition preCondition = preConditionTreeNode.Item;
                    preConditionTreeNode.Delete();
                    AddPreCondition(preCondition);
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

            return retVal;
        }
    }
}