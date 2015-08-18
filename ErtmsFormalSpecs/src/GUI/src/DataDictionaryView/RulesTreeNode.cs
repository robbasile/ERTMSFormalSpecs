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
using Structure = DataDictionary.Types.Structure;

namespace GUI.DataDictionaryView
{
    public class RulesTreeNode : TypeTreeNode<Structure>
    {
        private class ItemEditor : TypeEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public RulesTreeNode(Structure item, bool buildSubNodes)
            : base(item, buildSubNodes, "Rules", true, false)
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

            foreach (Rule rule in Item.Rules)
            {
                subNodes.Add(new RuleTreeNode(rule, recursive));
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

        /// <summary>
        ///     Create structure based on the subsystem structure
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);

            if (sourceNode is RuleTreeNode)
            {
                RuleTreeNode node = sourceNode as RuleTreeNode;
                Rule rule = node.Item;
                node.Delete();
                Item.appendRules(rule);
            }
            else if (sourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode node = sourceNode as ParagraphTreeNode;
                Paragraph paragraph = node.Item;

                Rule rule = Rule.CreateDefault(Item.Rules);
                Item.appendRules(rule);
                rule.FindOrCreateReqRef(paragraph);
            }
        }

        private void AddHandler(object sender, EventArgs args)
        {
            Item.appendRules(Rule.CreateDefault(Item.Rules));
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Add", AddHandler)};

            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}