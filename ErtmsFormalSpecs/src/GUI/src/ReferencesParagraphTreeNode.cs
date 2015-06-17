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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using DataDictionary;
using GUI.Converters;
using GUI.SpecificationView;

namespace GUI
{
    public abstract class ReferencesParagraphTreeNode<T> : ModelElementTreeNode<T>
        where T : ReferencesParagraph
    {
        /// <summary>
        ///     The editor for message variables
        /// </summary>
        protected class ReferencesParagraphEditor : CommentableEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            protected ReferencesParagraphEditor()
                : base()
            {
            }
        }

        /// <summary>
        ///     The editor for message variables
        /// </summary>
        protected class UnnamedReferencesParagraphEditor : Editor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            protected UnnamedReferencesParagraphEditor()
                : base()
            {
            }

            [Category("Meta data")]
            [Editor(typeof (CommentableUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (CommentableUITypeConverter))]
            public T Comment
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }
        }

        /// <summary>
        ///     The tree node that holds the references to requirements
        /// </summary>
        protected ReqRefsTreeNode ReqReferences;

        /// <summary>
        ///     Indicates whether this node handles requirements
        /// </summary>
        protected bool HandleRequirements { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        protected ReferencesParagraphTreeNode(T item, bool buildSubNodes, string name = null, bool isFolder = false,
            bool addRequirements = true)
            : base(item, buildSubNodes, name, isFolder)
        {
            HandleRequirements = addRequirements;

            // State of the node changed, rebuild the subnodes.
            BaseBuildSubNodes(buildSubNodes);
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            if (HandleRequirements && Item.Requirements.Count > 0)
            {
                ReqReferences = new ReqRefsTreeNode(Item, buildSubNodes);
                Nodes.Add(ReqReferences);
            }
        }

        /// <summary>
        ///     Handles a drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);

            if (SourceNode is ParagraphTreeNode)
            {
                if (HandleRequirements && ReqReferences == null)
                {
                    ReqReferences = new ReqRefsTreeNode(Item, false);
                    Nodes.Add(ReqReferences);
                }

                if (ReqReferences != null)
                {
                    ParagraphTreeNode paragraphTreeNode = (ParagraphTreeNode) SourceNode;
                    ReqReferences.CreateReqRef(paragraphTreeNode.Item);
                }
            }
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = base.GetMenuItems();

            return retVal;
        }
    }
}