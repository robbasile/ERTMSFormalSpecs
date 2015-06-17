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
using SourceText = DataDictionary.Tests.Translations.SourceText;
using SourceTextComment = DataDictionary.Tests.Translations.SourceTextComment;

namespace GUI.TranslationRules
{
    public class SourceTextCommentsTreeNode : ModelElementTreeNode<SourceText>
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
        public SourceTextCommentsTreeNode(SourceText item, bool buildSubNodes)
            : base(item, buildSubNodes, "Comments", true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (SourceTextComment comment in Item.Comments)
            {
                Nodes.Add(new SourceTextCommentTreeNode(comment, buildSubNodes));
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
        ///     Creates a new source text
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SourceTextCommentTreeNode createComment(SourceTextComment comment)
        {
            SourceTextCommentTreeNode retVal;

            Item.appendComments(comment);
            retVal = new SourceTextCommentTreeNode(comment, true);
            Nodes.Add(retVal);
            SortSubNodes();

            return retVal;
        }

        public void AddHandler(object sender, EventArgs args)
        {
            SourceTextComment comment = (SourceTextComment) acceptor.getFactory().createSourceTextComment();
            comment.Name = "<Comment" + (Item.Comments.Count + 1) + ">";
            createComment(comment);
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add comment", new EventHandler(AddHandler)));

            return retVal;
        }

        /// <summary>
        ///     Handles drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);
            SourceTextTreeNode.AcceptDropForSourceText((SourceTextTreeNode) Parent, SourceNode);
        }
    }
}