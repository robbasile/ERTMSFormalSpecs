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
using DataDictionary.Generated;
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
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            [Category("Description")]
            public bool SkipEngine
            {
                get { return Item.getSkipEngine(); }
                set { Item.setSkipEngine(value); }
            }
        }

        private ActionsTreeNode actions;
        private ExpectationsTreeNode expectations;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public SubStepTreeNode(SubStep item, bool buildSubNodes)
            : base(item, buildSubNodes, null, true)
        {
        }

        /// <summary>
        ///     Handles a selection change event
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(displayStatistics);
            if (Item.Translation != null)
            {
                if (BaseTreeView != null && BaseTreeView.RefreshNodeContent)
                {
                    IBaseForm baseForm = BaseForm;
                    if (baseForm != null)
                    {
                        if (baseForm.RequirementsTextBox != null)
                        {
                            baseForm.RequirementsTextBox.Text = Item.Translation.getSourceTextExplain();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            actions = new ActionsTreeNode(Item, buildSubNodes);
            expectations = new ExpectationsTreeNode(Item, buildSubNodes);

            Nodes.Add(actions);
            Nodes.Add(expectations);
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
        ///     Ensures that the runner corresponds to test case
        /// </summary>
        private void CheckRunner()
        {
            Window window = BaseForm as Window;
            if (window != null && window.EFSSystem.Runner != null &&
                window.EFSSystem.Runner.SubSequence != Item.Step.SubSequence)
            {
                window.Clear();
            }
        }

        /// <summary>
        ///     Adds an action for this sub-step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddActionHandler(object sender, EventArgs args)
        {
            Action action = (Action) acceptor.getFactory().createAction();
            action.Enclosing = Item;
            action.Name = "Action" + actions.Nodes.Count;
            createAction(action);
        }

        /// <summary>
        ///     Creates a new action
        /// </summary>
        /// <param name="testCase"></param>
        /// <returns></returns>
        public ActionTreeNode createAction(Action action)
        {
            ActionTreeNode retVal = new ActionTreeNode(action, true);

            Item.appendActions(action);
            actions.Nodes.Add(retVal);

            return retVal;
        }

        /// <summary>
        ///     Adds an expectation for this sub-step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddExpectationHandler(object sender, EventArgs args)
        {
            Expectation expectation = (Expectation) acceptor.getFactory().createExpectation();
            expectation.Enclosing = Item;
            expectation.Name = "Expectation" + expectations.Nodes.Count;
            createExpectation(expectation);
        }

        /// <summary>
        ///     Creates a new expectation
        /// </summary>
        /// <param name="testCase"></param>
        /// <returns></returns>
        public ExpectationTreeNode createExpectation(Expectation expectation)
        {
            ExpectationTreeNode retVal = new ExpectationTreeNode(expectation, true);

            Item.appendExpectations(expectation);
            expectations.Nodes.Add(retVal);

            return retVal;
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("Action", new EventHandler(AddActionHandler)));
            newItem.MenuItems.Add(new MenuItem("Expectation", new EventHandler(AddExpectationHandler)));
            retVal.Add(newItem);
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }

        /// <summary>
        ///     Handles the drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);
            if (SourceNode is ActionTreeNode)
            {
                ActionTreeNode action = SourceNode as ActionTreeNode;
                if (action.Parent is ActionsTreeNode)
                {
                    createAction(action.Item);
                }
                action.Delete();
            }
            else if (SourceNode is ExpectationTreeNode)
            {
                ExpectationTreeNode expectation = SourceNode as ExpectationTreeNode;
                createExpectation(expectation.Item);
                expectation.Delete();
            }
        }
    }
}