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
using GUI.DataDictionaryView;
using Action = DataDictionary.Rules.Action;
using Expectation = DataDictionary.Tests.Expectation;
using SubStep = DataDictionary.Tests.SubStep;

namespace GUI.TestRunnerView
{
    public class SubStepTreeNode : ModelElementTreeNode<SubStep>
    {
        /// <summary>
        ///     The value editor
        /// </summary>
        private class ItemEditor : CommentableEditor
        {
            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public bool SkipEngine
            {
                get { return Item.getSkipEngine(); }
                set { Item.setSkipEngine(value); }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public SubStepTreeNode(SubStep item, bool buildSubNodes)
            : base(item, buildSubNodes, null, true)
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

            subNodes.Add(new ActionsTreeNode(Item, recursive));
            subNodes.Add(new ExpectationsTreeNode(Item, recursive));
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
        ///     Adds an action for this sub-step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddActionHandler(object sender, EventArgs args)
        {
            Item.appendActions(Action.CreateDefault(Item.Actions));
        }

        /// <summary>
        ///     Adds an expectation for this sub-step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddExpectationHandler(object sender, EventArgs args)
        {
            Item.appendExpectations(Expectation.CreateDefault(Item.Expectations));
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("Action", AddActionHandler));
            newItem.MenuItems.Add(new MenuItem("Expectation", AddExpectationHandler));
            retVal.Add(newItem);
            retVal.Add(new MenuItem("Delete", DeleteHandler));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }

        /// <summary>
        ///     Handles the drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);

            ActionTreeNode action = sourceNode as ActionTreeNode;
            if (action != null)
            {
                action.Delete();
                Item.appendActions(action.Item);
            }

            ExpectationTreeNode expectation = sourceNode as ExpectationTreeNode;
            if (expectation != null)
            {
                expectation.Delete();
                Item.appendExpectations(expectation.Item);
            }
        }
    }
}