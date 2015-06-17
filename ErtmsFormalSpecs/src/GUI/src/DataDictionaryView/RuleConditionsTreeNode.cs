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
using GUI.SpecificationView;
using Paragraph = DataDictionary.Specification.Paragraph;
using ReqRef = DataDictionary.ReqRef;
using Rule = DataDictionary.Rules.Rule;
using RuleCondition = DataDictionary.Rules.RuleCondition;

namespace GUI.DataDictionaryView
{
    public class RuleConditionsTreeNode : ModelElementTreeNode<Rule>
    {
        private class ItemEditor : NamedEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public RuleConditionsTreeNode(Rule item, bool buildSubNodes)
            : base(item, buildSubNodes, "Conditions", true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (RuleCondition ruleCondition in Item.RuleConditions)
            {
                Nodes.Add(new RuleConditionTreeNode(ruleCondition, buildSubNodes));
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

        /// <summary>
        ///     Create structure based on the subsystem structure
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);

            if (SourceNode is RuleConditionTreeNode)
            {
                RuleConditionTreeNode node = SourceNode as RuleConditionTreeNode;
                RuleCondition ruleCondition = node.Item;
                node.Delete();
                AddRuleCondition(ruleCondition);
            }
            else if (SourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode node = SourceNode as ParagraphTreeNode;
                Paragraph paragaph = node.Item;

                RuleCondition ruleCondition = (RuleCondition) acceptor.getFactory().createRuleCondition();
                ruleCondition.Name = paragaph.Name;

                ReqRef reqRef = (ReqRef) acceptor.getFactory().createReqRef();
                reqRef.Name = paragaph.FullId;
                ruleCondition.appendRequirements(reqRef);
                AddRuleCondition(ruleCondition);
            }
        }

        private void AddHandler(object sender, EventArgs args)
        {
            RuleCondition rule = (RuleCondition) acceptor.getFactory().createRuleCondition();
            if (Item.RuleConditions.Count == 0)
            {
                rule.Name = Item.Name;
            }
            else
            {
                rule.Name = Item.Name + (Item.RuleConditions.Count + 1);
            }
            AddRuleCondition(rule);
        }

        /// <summary>
        ///     Adds a new rule to the model
        /// </summary>
        /// <param name="ruleCondition"></param>
        public void AddRuleCondition(RuleCondition ruleCondition)
        {
            Item.appendConditions(ruleCondition);
            Nodes.Add(new RuleConditionTreeNode(ruleCondition, true));
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