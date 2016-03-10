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
using System.Drawing;
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
            ///     The item name
            /// </summary>
            [Category("Description"), TypeConverter(typeof (RulePriorityConverter))]
            // ReSharper disable once UnusedMember.Local
            public acceptor.RulePriority Priority
            {
                get { return Item.getPriority(); }
                set { Item.setPriority(value); }
            }


            [Category("Display")]
            public Size Size
            {
                get { return new Size(Item.Width, Item.Height); }
                set
                {
                    Item.Width = value.Width;
                    Item.Height = value.Height;

                }
            }

            [Category("Display")]
            public Point Location
            {
                get { return new Point(Item.X, Item.Y); }
                set
                {
                    Item.X = value.X;
                    Item.Y = value.Y;
                }
            }

            [Category("Display")]
            public bool Hidden
            {
                get { return Item.Hidden; }
                set { Item.Hidden = value; }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public RuleTreeNode(Rule item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public RuleTreeNode(string name, Rule item, bool buildSubNodes)
            : base(item, buildSubNodes, name, false, true)
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

            subNodes.Add(new RuleConditionsTreeNode(Item, recursive));
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
        ///     Finds or creates an update for the current element.
        /// </summary>
        /// <returns></returns>
        protected override ModelElement FindOrCreateUpdate()
        {
            ModelElement retVal = null;

            Dictionary dictionary = GetPatchDictionary();

            if (dictionary != null)
            {
                retVal = dictionary.FindByFullName(Item.FullName) as ModelElement;
                if (retVal == null)
                {
                    // If the element does not already exist in the patch, add a copy to it
                    retVal = Item.CreateRuleUpdate(dictionary);
                }
                // Navigate to the rule, whether it was created or not
                EfsSystem.Instance.Context.SelectElement(Model, this, Context.SelectionCriteria.DoubleClick);
            }

            return retVal;
        }


        /// <summary>
        ///     Adds a condition to the rule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddCondition(object sender, EventArgs args)
        {
            RuleCondition condition = DataDictionary.Rules.RuleCondition.CreateDefault(Item.RuleConditions);
            Item.appendConditions(condition);
        }

        /// <summary>
        /// Deletes the selected rule
        /// </summary>
        public override void DeleteHandler(object sender, EventArgs args)
        {
            if (Item.EnclosingState != null)
            {
                State enclosingState = Item.EnclosingState;
                if (enclosingState.getEnterAction () == Item)
                {
                    enclosingState.setEnterAction (null);
                }
                else if (enclosingState.getLeaveAction() == Item)
                {
                    enclosingState.setLeaveAction(null);
                }
                else
                {
                    base.DeleteHandler(sender, args);                    
                }
            }
            else
            {
                base.DeleteHandler (sender, args);
            }
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem addItem = new MenuItem("Add...");
            addItem.MenuItems.Add(new MenuItem("Condition", AddCondition));
            retVal.Add(addItem);

            MenuItem updateItem = new MenuItem("Update...");
            updateItem.MenuItems.Add(new MenuItem("Update", AddUpdate));
            updateItem.MenuItems.Add(new MenuItem("Remove", RemoveInUpdate));
            retVal.Add(updateItem);
            retVal.Add(new MenuItem("Delete", DeleteHandler));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}