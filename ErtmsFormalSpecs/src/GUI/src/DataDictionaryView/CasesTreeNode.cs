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
using Case = DataDictionary.Functions.Case;
using Function = DataDictionary.Functions.Function;

namespace GUI.DataDictionaryView
{
    public class CasesTreeNode : FunctionTreeNode
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
        public CasesTreeNode(Function item, bool buildSubNodes)
            : base(item, buildSubNodes, "Cases", true, false)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates that subnodes of the nodes built should also </param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            Nodes.Clear();

            foreach (Case aCase in Item.Cases)
            {
                Nodes.Add(new CaseTreeNode(aCase, buildSubNodes));
            }
            SubNodesBuilt = true;
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
        ///     Create structure based on the subsystem structure
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);

            if (SourceNode is CaseTreeNode)
            {
                CaseTreeNode node = SourceNode as CaseTreeNode;
                Case aCase = node.Item;
                node.Delete();
                AddCase(aCase);
            }
        }

        public void AddHandler(object sender, EventArgs args)
        {
            DataDictionaryTreeView treeView = BaseTreeView as DataDictionaryTreeView;
            if (treeView != null)
            {
                Case aCase = (Case) acceptor.getFactory().createCase();
                aCase.Name = "<Case" + (GetNodeCount(false) + 1) + ">";
                AddCase(aCase);
            }
        }

        /// <summary>
        ///     Adds a new case
        /// </summary>
        /// <param name="function"></param>
        public CaseTreeNode AddCase(Case aCase)
        {
            Item.appendCases(aCase);
            CaseTreeNode retVal = new CaseTreeNode(aCase, true);
            Nodes.Add(retVal);

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
    }
}