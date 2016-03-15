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

using System.Collections.Generic;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.RuleCheck;

namespace GUI.RuleDisabling
{
    public class DisabledRuleChecksTreeNode<T> : ModelElementTreeNode<T> where T : ModelElement, IRuleCheckDisabling
    {
        /// <summary>
        ///     The editor for message variables
        /// </summary>
        protected class ItemEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public DisabledRuleChecksTreeNode(T item, bool buildSubNodes)
            : base(item, buildSubNodes, "Disabled Rules", true)
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

            if (Item.Disabling != null)
            {
                foreach (RuleCheckIdentifier disabledRule in Item.Disabling.DisabledRuleChecks)
                {
                    subNodes.Add(new DisabledRuleCheckTreeNode(disabledRule, recursive, true, disabledRule.Name));
                }
                subNodes.Sort();
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
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            return retVal;
        }
    }
}