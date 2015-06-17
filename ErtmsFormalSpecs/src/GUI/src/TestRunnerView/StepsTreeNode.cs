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
using DataDictionary.Tests;

namespace GUI.TestRunnerView
{
    public class StepsTreeNode : ModelElementTreeNode<TestCase>
    {
        /// <summary>
        ///     The value editor
        /// </summary>
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
        public StepsTreeNode(TestCase item, bool buildSubNodes)
            : base(item, buildSubNodes, "Steps", true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (Step step in Item.Steps)
            {
                Nodes.Add(new StepTreeNode(step, buildSubNodes));
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
        ///     Creates a new step
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public StepTreeNode createStep(Step step)
        {
            step.Enclosing = Item;
            StepTreeNode retVal = new StepTreeNode(step, true);

            Item.appendSteps(step);
            Nodes.Add(retVal);

            return retVal;
        }

        public void AddHandler(object sender, EventArgs args)
        {
            createStep(Step.createDefault("Step" + (GetNodeCount(false) + 1)));
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add", new EventHandler(AddHandler)));
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
            if (SourceNode is StepTreeNode)
            {
                StepTreeNode step = SourceNode as StepTreeNode;
                step.Delete();

                createStep(step.Item);
            }
        }
    }
}