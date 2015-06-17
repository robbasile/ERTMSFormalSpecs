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
    public class SourceTextTreeNode : ModelElementTreeNode<SourceText>
    {
        private class ItemEditor : CommentableEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }
        }

        private SourceTextCommentsTreeNode comments = null;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public SourceTextTreeNode(SourceText item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            if (Item.countComments() > 0)
            {
                comments = createFolder();
            }
        }

        /// <summary>
        ///     Creates the folder for comments
        /// </summary>
        /// <returns></returns>
        private SourceTextCommentsTreeNode createFolder()
        {
            if (comments == null)
            {
                comments = new SourceTextCommentsTreeNode(Item, true);
                Nodes.Add(comments);
            }

            return comments;
        }

        /// <summary>
        ///     Creates a new source text
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SourceTextCommentTreeNode createComment(SourceTextComment comment)
        {
            SourceTextCommentTreeNode retVal;

            if (comments == null)
            {
                comments = createFolder();
            }

            retVal = comments.createComment(comment);
            return retVal;
        }


        /// <summary>
        ///     Handles a selection change event
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(displayStatistics);
            if (Item.Translation != null)
            {
                if (BaseTreeView != null && BaseTreeView.RefreshNodeContent)
                {
                    IBaseForm baseForm = BaseForm;
                    if (baseForm != null)
                    {
                        if (baseForm.RequirementsTextBox != null)
                        {
                            baseForm.RequirementsTextBox.Text = Item.Translation.getSourceTextExplain();
                        }
                    }
                }
            }
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
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add comment", new EventHandler(AddHandler)));
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));

            return retVal;
        }

        /// <summary>
        ///     Deletes the selected item
        /// </summary>
        public void AddHandler(object sender, EventArgs args)
        {
            SourceTextComment comment = (SourceTextComment) acceptor.getFactory().createSourceTextComment();
            comment.Name = "<unknown>";
            createComment(comment);
        }

        /// <summary>
        ///     Accepts the drop event
        /// </summary>
        /// <param name="sourceTextTreeNode"></param>
        /// <param name="SourceNode"></param>
        public static void AcceptDropForSourceText(SourceTextTreeNode sourceTextTreeNode, BaseTreeNode SourceNode)
        {
            if (SourceNode is SourceTextCommentTreeNode)
            {
                SourceTextCommentTreeNode comment = SourceNode as SourceTextCommentTreeNode;

                SourceTextComment otherText = (SourceTextComment) comment.Item.Duplicate();
                sourceTextTreeNode.createComment(otherText);
                comment.Delete();
            }
        }
    }
}