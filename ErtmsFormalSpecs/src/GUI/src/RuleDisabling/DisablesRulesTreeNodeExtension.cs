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
using RuleCheckDisabling = DataDictionary.RuleCheck.RuleCheckDisabling;
using RuleCheckIdentifier = DataDictionary.RuleCheck.RuleCheckIdentifier;

namespace GUI.RuleDisabling
{
    public class DisablesRulesTreeNodeExtension<T> 
        where T : ModelElement, IRuleCheckDisabling
    {
        /// <summary>
        ///     Indicates whether this node handles disablings
        /// </summary>
        public bool HandleDisablings { get; private set; }

        /// <summary>
        ///     Reference to the model element that can disable rule checks
        /// </summary>
        private readonly T _item;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="addDisablings"></param>
        public DisablesRulesTreeNodeExtension(T item, bool addDisablings = true)
        {
            HandleDisablings = addDisablings;
           _item = item;
        }

        /// <summary>
        ///     Indicates that rule disablings are handled by this tree node and there are rule disablings to display
        /// </summary>
        /// <returns></returns>
        private bool DisablesRules()
        {
            return HandleDisablings && _item.Disabling != null && _item.Disabling.DisabledRuleChecks.Count > 0;
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            if (DisablesRules())
            {
                subNodes.Add(new DisabledRuleChecksTreeNode<T>(_item, recursive));
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
                if (_item.Disabling == null)
                {
                    _item.Disabling = (RuleCheckDisabling) acceptor.getFactory().createRuleCheckDisabling();
                }
                _item.Disabling.appendDisabledRuleChecks(identifier);
            }
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        public List<MenuItem> AddMenuItems(List<MenuItem> baseList)
        {
            List<MenuItem> retVal = baseList;

            retVal.Add(new MenuItem("Add rule disabling", AddRuleDisabling));

            return retVal;
        }
    }
}