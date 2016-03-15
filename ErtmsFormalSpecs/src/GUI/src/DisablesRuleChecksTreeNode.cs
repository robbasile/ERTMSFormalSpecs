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
using DataDictionary;
using DataDictionary.Generated;
using DataDictionary.RuleCheck;
using GUI.RuleDisabling;
using RuleCheckDisabling = DataDictionary.RuleCheck.RuleCheckDisabling;
using RuleCheckIdentifier = DataDictionary.RuleCheck.RuleCheckIdentifier;

namespace GUI
{
    public abstract class DisablesRuleChecksTreeNode<T> : ModelElementTreeNode<T> where T : ModelElement, IRuleCheckDisabling
    {
        /// <summary>
        ///     The editor for message variables
        /// </summary>
        protected class DisablesRuleChecksEditor : CommentableEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            protected DisablesRuleChecksEditor()
            {
            }
        }

        /// <summary>
        ///     Indicates whether this node handles disablings
        /// </summary>
        protected bool HandleDisablings { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        /// <param name="name"></param>
        /// <param name="isFolder"></param>
        /// <param name="addDisablings"></param>
        protected DisablesRuleChecksTreeNode(T item, bool buildSubNodes, string name = null, bool isFolder = false,
            bool addDisablings = true)
            : base(item, buildSubNodes, name, isFolder)
        {
            HandleDisablings = addDisablings;

            if (buildSubNodes)
            {
                // State of the node changed, rebuild the subnodes.
                BuildOrRefreshSubNodes(null);
            }
        }

        /// <summary>
        ///     Indicates that rule disablings are handled by this tree node and there are rule disablings to display
        /// </summary>
        /// <returns></returns>
        private bool DisablesRules()
        {
            return HandleDisablings && Item.Disabling != null && Item.Disabling.DisabledRuleChecks.Count > 0;
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            base.BuildSubNodes(subNodes, recursive);

            if (DisablesRules())
            {
                subNodes.Add(new DisabledRuleChecksTreeNode<T>(Item, recursive));
            }
        }

        /// <summary>
        ///     Disables a rule checker rule for this and all sub-elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddRuleDisabling(object sender, EventArgs args)
        {
            Frm_SelectRule selectRule = new Frm_SelectRule();
            selectRule.ShowDialog(GuiUtils.MdiWindow);

            if (selectRule.SelectedRule != null)
            {
                RuleCheckIdentifier identifier = (RuleCheckIdentifier) acceptor.getFactory().createRuleCheckIdentifier();
                identifier.Name = selectRule.SelectedRule.ToString();
                if (Item.Disabling == null)
                {
                    Item.Disabling = (RuleCheckDisabling) acceptor.getFactory().createRuleCheckDisabling();
                }
                Item.Disabling.appendDisabledRuleChecks(identifier);
                BuildOrRefreshSubNodes(null);
            }
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = base.GetMenuItems();

            retVal.Add(new MenuItem("Add rule disabling", AddRuleDisabling));

            return retVal;
        }
    }
}