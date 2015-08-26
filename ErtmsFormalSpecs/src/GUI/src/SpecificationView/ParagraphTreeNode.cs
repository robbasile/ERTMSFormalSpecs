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
using DataDictionary;
using DataDictionary.Generated;
using GUI.Converters;
using GUI.Properties;
using Dictionary = DataDictionary.Dictionary;
using Paragraph = DataDictionary.Specification.Paragraph;
using RequirementSetReference = DataDictionary.Specification.RequirementSetReference;

namespace GUI.SpecificationView
{
    public class ParagraphTreeNode : ReferencesParagraphTreeNode<Paragraph>
    {
        /// <summary>
        ///     The value editor
        /// </summary>
        private class ItemEditor : UnnamedReferencesParagraphEditor
        {
            /// <summary>
            ///     The item name
            /// </summary>
            [Category("\t\tDescription")]
            // ReSharper disable once UnusedMember.Local
            public string Id
            {
                get { return Item.getId(); }
                set
                {
                    Item.setId(value);
                    RefreshNode();
                }
            }

            /// <summary>
            ///     Provides the type of the paragraph
            /// </summary>
            [Category("\t\tDescription"), TypeConverter(typeof (SpecTypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public acceptor.Paragraph_type Type
            {
                get { return Item.getType(); }
                set
                {
                    Item.SetType(value);
                    RefreshNode();
                }
            }

            /// <summary>
            ///     Indicates if the paragraph has been reviewed (content & structure)
            /// </summary>
            [Category("Meta data")]
            // ReSharper disable once UnusedMember.Local
            public bool Reviewed
            {
                get { return Item.getReviewed(); }
                set { Item.setReviewed(value); }
            }

            /// <summary>
            ///     Indicates if the paragraph can be implemented by the EFS
            /// </summary>
            [Category("Meta data"), TypeConverter(typeof (ImplementationStatusConverter))]
            // ReSharper disable once UnusedMember.Local
            public acceptor.SPEC_IMPLEMENTED_ENUM ImplementationStatus
            {
                get { return Item.getImplementationStatus(); }
                set { Item.setImplementationStatus(value); }
            }

            /// <summary>
            ///     Indicates if the paragraph has been tested
            /// </summary>
            [Category("Meta data")]
            // ReSharper disable once UnusedMember.Local
            public bool Tested
            {
                get { return Item.getTested(); }
                set { Item.setTested(value); }
            }

            /// <summary>
            ///     Indicates that more information is required to understand the requirement
            /// </summary>
            [Category("Meta data")]
            // ReSharper disable once UnusedMember.Local
            public bool MoreInfoRequired
            {
                get { return Item.getMoreInfoRequired(); }
                set { Item.setMoreInfoRequired(value); }
            }

            /// <summary>
            ///     Indicates that this paragraph has an issue
            /// </summary>
            [Category("Meta data")]
            // ReSharper disable once UnusedMember.Local
            public bool SpecIssue
            {
                get { return Item.getSpecIssue(); }
                set { Item.setSpecIssue(value); }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public ParagraphTreeNode(Paragraph item, bool buildSubNodes)
            : base(item, buildSubNodes)
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

            foreach (Paragraph paragraph in Item.SubParagraphs)
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

        public void ImplementedHandler(object sender, EventArgs args)
        {
            if (Item.IsApplicable())
            {
                Item.setImplementationStatus(acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented);
            }
        }

        public void ReviewedHandler(object sender, EventArgs args)
        {
            Item.setReviewed(true);
        }

        public void NotImplementableHandler(object sender, EventArgs args)
        {
            Item.setImplementationStatus(acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NotImplementable);
        }

        public void AddParagraphHandler(object sender, EventArgs args)
        {
            Item.appendParagraphs(Paragraph.CreateDefault(Item.SubParagraphs, Item.FullId));
        }

        public void AddParagraphFromClipboardHandler(object sender, EventArgs args)
        {
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText(TextDataFormat.Text);

                string id;
                string data;

                int i = text.IndexOf(' ');
                int k = text.IndexOf('\t');
                if (k < i)
                {
                    i = k;
                }
                int j = text.IndexOf('\n');
                if (i < 0)
                {
                    i = j;
                }
                else
                {
                    if (j > 0)
                    {
                        i = Math.Min(i, j);
                    }
                }
                if (i > 0)
                {
                    id = text.Substring(0, i).Trim();
                    if (id.Length > 0 && char.IsDigit(id[0]))
                    {
                        data = text.Substring(i + 1);
                    }
                    else
                    {
                        id = Item.GetNewSubParagraphId(true);
                        data = text;
                    }
                }
                else
                {
                    id = Item.GetNewSubParagraphId(true);
                    data = text;
                }
                data = data.Replace("\r", "");
                data = data.Replace("\n", "");

                Paragraph paragraph = Paragraph.CreateDefault(Item.SubParagraphs, Item.FullId);
                paragraph.FullId = id;
                paragraph.Text = data;
                Item.appendParagraphs(paragraph);
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
                Paragraph current = Item;
                while (current != null && current != paragraphTreeNode.Model)
                {
                    current = current.EnclosingParagraph;
                }

                if (current == null)
                {
                    if (
                        MessageBox.Show(
                            Resources
                                .ParagraphTreeNode_AcceptDrop_Are_you_sure_you_want_to_move_the_corresponding_paragraph_,
                            Resources.ParagraphTreeNode_AcceptDrop_Move_paragraph,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Paragraph paragraph = paragraphTreeNode.Item;
                        paragraphTreeNode.Delete();
                        Item.appendParagraphs(paragraph);
                    }
                }
                else
                {
                    MessageBox.Show(
                        Resources.ParagraphTreeNode_AcceptDrop_Cannot_move_a_paragraph_in_its_sub_paragraphs,
                        Resources.ParagraphTreeNode_AcceptDrop_Move_paragraph,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else
            {
                base.AcceptDrop(sourceNode);
            }
        }

        public override void AcceptCopy(BaseTreeNode sourceNode)
        {
            ParagraphTreeNode paragraphTreeNode = sourceNode as ParagraphTreeNode;
            if (paragraphTreeNode != null)
            {
                Item.FindOrCreateReqRef(paragraphTreeNode.Item);
            }
        }

        /// <summary>
        ///     Updates the paragraph name by appending the "Table " string at its beginning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddTableHandler(object sender, EventArgs args)
        {
            Item.setId("Table " + Item.getId());
            RefreshNode();
        }

        /// <summary>
        ///     Updates the paragraph name by appending the "Entry " string at its beginning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddEntryHandler(object sender, EventArgs args)
        {
            Item.setId("Entry " + Item.getId());
            RefreshNode();
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

        private void FindOrCreateUpdate(object sender, EventArgs args)
        {
            Dictionary dictionary = GetPatchDictionary();
            if (dictionary != null)
            {
                Paragraph paragraphUpdate = dictionary.FindByFullName(Item.FullName) as Paragraph;
                if (paragraphUpdate == null)
                {
                    // If the element does not already exist in the patch, add a copy to it
                    paragraphUpdate = Item.CreateParagraphUpdate(dictionary);
                }
                // Navigate to the element, whether it was created or not
                EfsSystem.Instance.Context.SelectElement(paragraphUpdate, this, Context.SelectionCriteria.DoubleClick);
            }
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>
            {
                new MenuItem("Add paragraph", AddParagraphHandler),
                new MenuItem("Add paragraph from clipboard", AddParagraphFromClipboardHandler),
                new MenuItem("Delete", DeleteHandler),
                new MenuItem("-"),
                new MenuItem("Update", FindOrCreateUpdate)
            };

            retVal.AddRange(base.GetMenuItems());
            MenuItem newItem = new MenuItem("Mark as...");
            newItem.MenuItems.Add(new MenuItem("Reviewed", ReviewedHandler));
            newItem.MenuItems.Add(new MenuItem("Implemented", ImplementedHandler));
            newItem.MenuItems.Add(new MenuItem("Not implementable", NotImplementableHandler));
            retVal.Insert(4, newItem);
            retVal.Insert(7, new MenuItem("Add Table to Id", AddTableHandler));
            retVal.Insert(8, new MenuItem("Add Entry to Id", AddEntryHandler));

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