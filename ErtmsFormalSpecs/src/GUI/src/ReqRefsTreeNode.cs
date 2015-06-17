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
using DataDictionary.Generated;
using GUI.SpecificationView;
using Paragraph = DataDictionary.Specification.Paragraph;
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
        public ReqRefsTreeNode(ReferencesParagraph item, bool buildSubNodes)
            : base(item, buildSubNodes, "Requirements", true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (ReqRef req in Item.Requirements)
            {
                Nodes.Add(new ReqRefTreeNode(req, buildSubNodes, true));
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

        /// <summary>
        ///     Creates are reference to a requirement
        /// </summary>
        /// <param name="paragraph"></param>
        public void CreateReqRef(Paragraph paragraph)
        {
            bool found = false;
            foreach (ReqRef reqRef in Item.Requirements)
            {
                if (reqRef.Paragraph == paragraph)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                ReqRef req = (ReqRef) acceptor.getFactory().createReqRef();
                req.Paragraph = paragraph;
                Item.appendRequirements(req);
                Nodes.Add(new ReqRefTreeNode(req, true, true));
                SortSubNodes();
                RefreshNode();
            }
            else
            {
                MessageBox.Show("Reference to paragraph " + paragraph.FullId +
                                " has not been added because it already exists.");
            }
        }

        /// <summary>
        ///     Handles a drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            if (SourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode paragraphTreeNode = (ParagraphTreeNode) SourceNode;
                CreateReqRef(paragraphTreeNode.Item);
            }
            else if (SourceNode is ReqRefTreeNode)
            {
                ReqRefTreeNode reqRefTreeNode = (ReqRefTreeNode) SourceNode;
                CreateReqRef(reqRefTreeNode.Item.Paragraph);
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