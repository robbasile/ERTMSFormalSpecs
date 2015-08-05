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

using System.Collections.Generic;
using System.Windows.Forms;
using GUI.SpecificationView;
using ReferencesParagraph = DataDictionary.ReferencesParagraph;
using ReqRef = DataDictionary.ReqRef;

namespace GUI
{
    public class ReqRefsTreeNode : ModelElementTreeNode<ReferencesParagraph>
    {
        /// <summary>
        ///     The editor for message variables
        /// </summary>
        protected class ItemEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public ReqRefsTreeNode(ReferencesParagraph item, bool buildSubNodes)
            : base(item, buildSubNodes, "Requirements", true)
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

            foreach (ReqRef req in Item.Requirements)
            {
                subNodes.Add(new ReqRefTreeNode(req, recursive, true));
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
        
        /// <summary>
        ///     Handles a drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            ParagraphTreeNode paragraphTreeNode = sourceNode as ParagraphTreeNode;
            if (paragraphTreeNode != null)
            {
                Item.FindOrCreateReqRef(paragraphTreeNode.Item);
            }
            else
            {
                ReqRefTreeNode reqRefTreeNode = sourceNode as ReqRefTreeNode;
                if (reqRefTreeNode != null)
                {
                    Item.FindOrCreateReqRef(reqRefTreeNode.Item.Paragraph);
                }
            }
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            return retVal;
        }
    }
}