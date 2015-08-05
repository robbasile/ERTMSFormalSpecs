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
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public FunctionsTreeNode(NameSpace item, bool buildSubNodes)
            : base(item, buildSubNodes, "Functions", true)
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

            foreach (Function function in Item.Functions)
            {
                subNodes.Add(new FunctionTreeNode(function, recursive));
            }
            subNodes.Sort();
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddHandler(object sender, EventArgs args)
        {
            Item.appendFunctions(Function.CreateDefault(Item.Functions));
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Add", AddHandler)};

            return retVal;
        }

        /// <summary>
        ///     Accepts drop of a tree node, in a drag & drop operation
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);

            if (sourceNode is FunctionTreeNode)
            {
                FunctionTreeNode node = sourceNode as FunctionTreeNode;
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
                            if (temp != null && temp.Item.Name == function.Name)
                            {
                                temp.Delete();
                            }
                        }
                        node.Delete();
                        Item.appendFunctions(function);
                    }
                }
                else
                {
                    node.Delete();
                    Item.appendFunctions(function);
                }
            }
            else if (sourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode node = sourceNode as ParagraphTreeNode;
                Paragraph paragaph = node.Item;

                Function function = (Function) acceptor.getFactory().createFunction();
                function.Name = paragaph.Name;

                ReqRef reqRef = (ReqRef) acceptor.getFactory().createReqRef();
                reqRef.Name = paragaph.FullId;
                function.appendRequirements(reqRef);
                Item.appendFunctions(function);
            }
        }
    }
}