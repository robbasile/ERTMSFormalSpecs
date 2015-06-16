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
using DataDictionary.Generated;
using GUI.Converters;
using Dictionary = DataDictionary.Dictionary;
using Rule = DataDictionary.Rules.Rule;

namespace GUI.DataDictionaryView
{
    public class RuleTreeNode : ReqRelatedTreeNode<Rule>
    {
        private class ItemEditor : ReqRelatedEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            /// <summary>
            ///     The item name
            /// </summary>
            [Category("Description"), TypeConverter(typeof (RulePriorityConverter))]
            public acceptor.RulePriority Priority
            {
                get { return Item.getPriority(); }
                set { Item.setPriority(value); }
            }
        }

        private RuleConditionsTreeNode Conditions;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public RuleTreeNode(Rule item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public RuleTreeNode(string name, Rule item, bool buildSubNodes)
            : base(item, buildSubNodes, name, false, true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            Conditions = new RuleConditionsTreeNode(Item, buildSubNodes);
            Nodes.Add(Conditions);
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
        ///     Handles a drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);
        }

        /// <summary>
        ///     Disable the rule, in a specific data dictionary
        /// </summary>
        public void DisableHandler(object sender, EventArgs args)
        {
            Dictionary dictionary = GUIUtils.MDIWindow.GetActiveDictionary();

            if (dictionary != null)
            {
                dictionary.AppendRuleDisabling(Item);
                GUIUtils.MDIWindow.RefreshModel();
            }
        }


        private void AddRuleUpdate(object sender, EventArgs args)
        {
            Dictionary dictionary = GetPatchDictionary();

            if (dictionary != null)
            {
                ModelElement updatedElement = dictionary.findByFullName(Item.FullName) as ModelElement;
                if (updatedElement == null)
                {
                    // If the element does not already exist in the patch, add a copy to it
                    updatedElement = Item.CreateRuleUpdate(dictionary);
                }
                // navigate to the rule, whether it was created or not
                GUIUtils.MDIWindow.RefreshModel();
                GUIUtils.MDIWindow.Select(updatedElement);
            }
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("Update", new EventHandler(AddRuleUpdate)));
            retVal.Add(newItem);
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(5, new MenuItem("-"));
            retVal.Insert(6, new MenuItem("Disable", new EventHandler(DisableHandler)));

            return retVal;
        }
    }
}