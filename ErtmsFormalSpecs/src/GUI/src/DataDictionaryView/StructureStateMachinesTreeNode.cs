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
using StateMachine = DataDictionary.Types.StateMachine;
using Structure = DataDictionary.Types.Structure;

namespace GUI.DataDictionaryView
{
    public class StructureStateMachinesTreeNode : ModelElementTreeNode<Structure>
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
        /// <param name="name"></param>
        public StructureStateMachinesTreeNode(Structure item, bool buildSubNodes)
            : base(item, buildSubNodes, "State Machines", true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (StateMachine stateMachine in Item.StateMachines)
            {
                Nodes.Add(new StateMachineTreeNode(stateMachine, buildSubNodes));
            }
            SortSubNodes();
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        public void AddHandler(object sender, EventArgs args)
        {
            DataDictionaryTreeView treeView = BaseTreeView as DataDictionaryTreeView;
            if (treeView != null)
            {
                StateMachine stateMachine = (StateMachine) acceptor.getFactory().createStateMachine();
                stateMachine.Name = "<StateMachine" + (GetNodeCount(false) + 1) + ">";
                AddStateMachine(stateMachine);
            }
        }

        /// <summary>
        ///     Adds a new state machine
        /// </summary>
        /// <param name="collection"></param>
        public StateMachineTreeNode AddStateMachine(StateMachine stateMachine)
        {
            StateMachineTreeNode retVal = new StateMachineTreeNode(stateMachine, true);
            Item.appendStateMachines(stateMachine);
            Nodes.Add(retVal);
            SortSubNodes();

            return retVal;
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


        /// <summary>
        ///     Accepts drop of a tree node, in a drag & drop operation
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);

            if (SourceNode is StateMachineTreeNode)
            {
                StateMachineTreeNode stateMachineTreeNode = SourceNode as StateMachineTreeNode;
                StateMachine stateMachine = stateMachineTreeNode.Item;

                stateMachineTreeNode.Delete();
                AddStateMachine(stateMachine);
            }
            else if (SourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode node = SourceNode as ParagraphTreeNode;
                Paragraph paragaph = node.Item;

                StateMachine stateMachine = (StateMachine) acceptor.getFactory().createStateMachine();
                stateMachine.Name = paragaph.Name;

                ReqRef reqRef = (ReqRef) acceptor.getFactory().createReqRef();
                reqRef.Name = paragaph.FullId;
                stateMachine.appendRequirements(reqRef);
                AddStateMachine(stateMachine);
            }
        }
    }
}