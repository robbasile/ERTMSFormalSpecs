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
using System.Drawing.Design;
using System.Windows.Forms;
using DataDictionary;
using GUI.Converters;
using GUI.SpecificationView;

namespace GUI
{
    public class ReqRefTreeNode : ModelElementTreeNode<ReqRef>
    {
        /// <summary>
        ///     Indicates that this req ref can be removed from its model
        /// </summary>
        private bool CanBeDeleted { get; set; }

        public class InternalTracesConverter : TracesConverter
        {
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return GetValues(((ItemEditor) context.Instance).Item);
            }
        }

        private class ItemEditor : Editor
        {
            [Category("Description"), TypeConverter(typeof (InternalTracesConverter))]
            // ReSharper disable once UnusedMember.Local
            public string Name
            {
                get { return Item.Name; }
            }

            [Category("Description")]
            [Editor(typeof (CommentableUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (CommentableUITypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public ReqRef Comment
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
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        /// <param name="canBeDeleted"></param>
        /// <param name="name"></param>
        public ReqRefTreeNode(ReqRef item, bool buildSubNodes, bool canBeDeleted, string name = null)
            : base(item, buildSubNodes, name)
        {
            CanBeDeleted = canBeDeleted;
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public override void DoubleClickHandler()
        {
            EfsSystem.Instance.Context.SelectElement(Item.Paragraph, this, Context.SelectionCriteria.DoubleClick);
            EfsSystem.Instance.Context.SelectElement(Item.Model, this, Context.SelectionCriteria.DoubleClick);
        }

        /// <summary>
        ///     Handles the selection of the requirement
        /// </summary>
        public void SelectHandler(object sender, EventArgs args)
        {
            DoubleClickHandler();
        }

        /// <summary>
        ///     Finds or creates an update for the current element
        /// </summary>
        /// <returns></returns>
        protected override ModelElement FindOrCreateUpdate()
        {
            ModelElement retVal = null;

            Dictionary dictionary = GetPatchDictionary();

            if (dictionary != null)
            {
                ModelElement model = dictionary.FindByFullName(Item.Model.FullName) as ModelElement;
                if (model != null)
                {
                    retVal = Item.CreateReqRefUpdate(model);
                }
                // Navigate to the element, whether it was created or not
                EfsSystem.Instance.Context.SelectElement(retVal, this, Context.SelectionCriteria.DoubleClick);
            }

            return retVal;
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Select", SelectHandler)};

            MenuItem updateItem = new MenuItem("Update...");
            updateItem.MenuItems.Add(new MenuItem("Remove link", RemoveInUpdate));
            retVal.Add(updateItem);

            if (CanBeDeleted)
            {
                retVal.Add(new MenuItem("-"));
                retVal.Add(new MenuItem("Delete", DeleteHandler));
            }

            return retVal;
        }


        /// <summary>
        ///     Accepts a drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);

            if (sourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode paragraph = sourceNode as ParagraphTreeNode;

                Item.Name = paragraph.Item.FullId;
                RefreshNode();
            }
        }
    }
}