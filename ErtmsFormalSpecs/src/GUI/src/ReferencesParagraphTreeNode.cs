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
        ///     Indicates whether this node handles requirements
        /// </summary>
        protected bool HandleRequirements { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        /// <param name="name"></param>
        /// <param name="isFolder"></param>
        /// <param name="addRequirements"></param>
        protected ReferencesParagraphTreeNode(T item, bool buildSubNodes, string name = null, bool isFolder = false,
            bool addRequirements = true)
            : base(item, buildSubNodes, name, isFolder)
        {
            HandleRequirements = addRequirements;

            if (buildSubNodes)
            {
                // State of the node changed, rebuild the subnodes.
                BuildOrRefreshSubNodes(null);
            }
        }

        /// <summary>
        ///     Indicates that requirements are handled by this tree node and there are requirements to display
        /// </summary>
        /// <returns></returns>
        private bool HasRequirements()
        {
            return HandleRequirements && Item.Requirements.Count > 0;
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            base.BuildSubNodes(subNodes, recursive);

            if (HasRequirements())
            {
                subNodes.Add(new ReqRefsTreeNode(Item, recursive));
            }
        }

        /// <summary>
        ///     Handles a drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);

            ParagraphTreeNode paragraphTreeNode = sourceNode as ParagraphTreeNode;
            if (paragraphTreeNode != null)
            {
                Item.FindOrCreateReqRef(paragraphTreeNode.Item);
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