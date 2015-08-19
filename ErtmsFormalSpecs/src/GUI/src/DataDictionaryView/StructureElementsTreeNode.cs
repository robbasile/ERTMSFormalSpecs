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
using System.Drawing;
using System.Windows.Forms;
using DataDictionary.Specification;
using DataDictionary.Types;
using GUI.SpecificationView;

namespace GUI.DataDictionaryView
{
    public class StructureElementsTreeNode : StructureTreeNode
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public StructureElementsTreeNode(Structure item, bool buildSubNodes)
            : base(item, buildSubNodes, "Sub elements", true, false)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            // Do not use the base version
            SubNodesBuilt = true;

            foreach (StructureElement structureElement in Item.Elements)
            {
                StructureElementTreeNode aNode = new StructureElementTreeNode(structureElement, recursive);
                if (Item.StructureElementIsInherited(structureElement))
                {
                    aNode.NodeFont = new Font("Arial", 8, FontStyle.Italic);
                }
                subNodes.Add(aNode);
            }
            subNodes.Sort();
        }

        public void AddHandler(object sender, EventArgs args)
        {
            Item.appendElements(StructureElement.CreateDefault(Item.Elements));
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

            if (sourceNode is StructureElementTreeNode)
            {
                StructureElementTreeNode structureElementTreeNode = sourceNode as StructureElementTreeNode;
                StructureElement element = structureElementTreeNode.Item;

                structureElementTreeNode.Delete();
                Item.appendElements(element);
            }
            else if (sourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode node = sourceNode as ParagraphTreeNode;
                Paragraph paragraph = node.Item;

                StructureElement element = StructureElement.CreateDefault(Item.Elements);
                Item.appendElements(element);
                element.FindOrCreateReqRef(paragraph);
            }
        }
    }
}