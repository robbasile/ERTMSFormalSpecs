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
using GUI.SpecificationView;
using Function = DataDictionary.Functions.Function;
using NameSpace = DataDictionary.Types.NameSpace;
using Paragraph = DataDictionary.Specification.Paragraph;
using ReqRef = DataDictionary.ReqRef;

namespace GUI.DataDictionaryView
{
    public class FunctionsTreeNode : ModelElementTreeNode<NameSpace>
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
        public FunctionsTreeNode(NameSpace item, bool buildSubNodes)
            : base(item, buildSubNodes, "Functions", true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (Function function in Item.Functions)
            {
                Nodes.Add(new FunctionTreeNode(function, buildSubNodes));
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
                Function function = (Function) acceptor.getFactory().createFunction();
                function.Name = "<Function" + (GetNodeCount(false) + 1) + ">";
                AddFunction(function);
            }
        }

        /// <summary>
        ///     Adds a new function
        /// </summary>
        /// <param name="function"></param>
        public FunctionTreeNode AddFunction(Function function)
        {
            // Ensure that functions always have a type
            if (function.ReturnType == null)
            {
                function.ReturnType = function.EFSSystem.BoolType;
            }

            Item.appendFunctions(function);
            FunctionTreeNode retVal = new FunctionTreeNode(function, true);
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

            if (SourceNode is FunctionTreeNode)
            {
                FunctionTreeNode node = SourceNode as FunctionTreeNode;
                Function function = node.Item;
                Function duplFunction = OverallFunctionFinder.INSTANCE.findByName(function.Dictionary, function.Name);
                if (duplFunction != null) // If there is a function with the same name, we must delete it
                {
                    if (
                        MessageBox.Show("Are you sure you want to move the corresponding function?", "Move action",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        for (int i = 0; i < Nodes.Count; i++)
                        {
                            FunctionTreeNode temp = Nodes[i] as FunctionTreeNode;
                            if (temp.Item.Name == function.Name)
                            {
                                temp.Delete();
                            }
                        }
                        node.Delete();
                        AddFunction(function);
                    }
                }
                else
                {
                    node.Delete();
                    AddFunction(function);
                }
            }
            else if (SourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode node = SourceNode as ParagraphTreeNode;
                Paragraph paragaph = node.Item;

                Function function = (Function) acceptor.getFactory().createFunction();
                function.Name = paragaph.Name;

                ReqRef reqRef = (ReqRef) acceptor.getFactory().createReqRef();
                reqRef.Name = paragaph.FullId;
                function.appendRequirements(reqRef);
                AddFunction(function);
            }
        }

        /// <summary>
        ///     Update counts according to the selected folder
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(false);

            GUIUtils.MDIWindow.SetStatus(Item.Functions.Count +
                                         (Item.Functions.Count > 1 ? " functions " : " function ") + "selected.");
        }
    }
}