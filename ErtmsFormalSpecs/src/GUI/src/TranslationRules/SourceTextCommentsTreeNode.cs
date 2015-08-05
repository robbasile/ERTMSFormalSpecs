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
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public SourceTextCommentsTreeNode(SourceText item, bool buildSubNodes)
            : base(item, buildSubNodes, "Comments", true)
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

            foreach (SourceTextComment comment in Item.Comments)
            {
                subNodes.Add(new SourceTextCommentTreeNode(comment, recursive));
            }
            subNodes.Sort();subNodes.Sort();
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
            SourceTextComment comment = (SourceTextComment) acceptor.getFactory().createSourceTextComment();
            comment.Name = "<Comment" + (Item.Comments.Count + 1) + ">";
            Item.appendComments(comment);
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Add comment", AddHandler)};

            return retVal;
        }

        /// <summary>
        ///     Handles drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);

            SourceTextTreeNode sourceTextTreeNode = Parent as SourceTextTreeNode;
            SourceTextCommentTreeNode comment = sourceNode as SourceTextCommentTreeNode; 
            if (comment != null && sourceTextTreeNode != null)
            {
                SourceTextComment otherText = (SourceTextComment)comment.Item.Duplicate();
                sourceTextTreeNode.Item.appendComments(otherText);
                comment.Delete();
            }
        }

        /// <summary>
        ///     Accepts the drop event
        /// </summary>
        /// <param name="sourceTextTreeNode"></param>
        /// <param name="sourceNode"></param>
        public static void AcceptDropForSourceText(SourceTextTreeNode sourceTextTreeNode, BaseTreeNode sourceNode)
        {
            if (sourceNode is SourceTextCommentTreeNode)
            {
                SourceTextCommentTreeNode comment = sourceNode as SourceTextCommentTreeNode;

                SourceTextComment otherText = (SourceTextComment)comment.Item.Duplicate();
                sourceTextTreeNode.Item.appendComments(otherText);
                comment.Delete();
            }
        }
    }
}