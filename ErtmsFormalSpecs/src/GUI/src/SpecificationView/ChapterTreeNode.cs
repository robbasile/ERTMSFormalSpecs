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
using System.ComponentModel;
using System.Windows.Forms;
using DataDictionary.Generated;
using Chapter = DataDictionary.Specification.Chapter;
using Paragraph = DataDictionary.Specification.Paragraph;
using RequirementSetReference = DataDictionary.Specification.RequirementSetReference;

namespace GUI.SpecificationView
{
    public class ChapterTreeNode : ModelElementTreeNode<Chapter>
    {
        /// <summary>
        ///     The value editor
        /// </summary>
        private class ItemEditor : Editor
        {
            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public string Identifier
            {
                get { return Item.getId(); }
                set
                {
                    Item.setId(value);
                    RefreshNode();
                }
            }

            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public string Name
            {
                get { return Item.Name; }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public ChapterTreeNode(Chapter item, bool buildSubNodes)
            : base(item, buildSubNodes, null, true)
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

            foreach (Paragraph paragraph in Item.Paragraphs)
            {
                subNodes.Add(new ParagraphTreeNode(paragraph, recursive));
            }
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddParagraphHandler(object sender, EventArgs args)
        {
            Item.appendParagraphs(Paragraph.CreateDefault(Item.Paragraphs, Item.getId()));
        }

        public void ChangeRequirementToNoteHandler(object sender, EventArgs args)
        {
            foreach (Paragraph paragraph in Item.Paragraphs)
            {
                paragraph.ChangeType(acceptor.Paragraph_type.aREQUIREMENT, acceptor.Paragraph_type.aNOTE);
            }
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
                if (
                    MessageBox.Show("Are you sure you want to move the corresponding paragraph?", "Move paragraph",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Paragraph paragraph = paragraphTreeNode.Item;
                    paragraphTreeNode.Delete();
                    Item.appendParagraphs(paragraph);
                }
            }
        }

        /// <summary>
        ///     Recursively marks all model elements as verified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RemoveRequirementSets(object sender, EventArgs e)
        {
            RequirementSetReference.RemoveReferencesVisitor remover =
                new RequirementSetReference.RemoveReferencesVisitor();
            remover.visit(Item);
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = base.GetMenuItems();

            retVal.Add(new MenuItem("Add paragraph", AddParagraphHandler));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Change 'Requirement' to 'Note'", ChangeRequirementToNoteHandler));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Delete", DeleteHandler));

            MenuItem recursiveActions = retVal.Find(x => x.Text.StartsWith("Recursive"));
            if (recursiveActions != null)
            {
                recursiveActions.MenuItems.Add(new MenuItem("-"));
                recursiveActions.MenuItems.Add(new MenuItem("Remove requirement sets", RemoveRequirementSets));
            }

            return retVal;
        }
    }
}