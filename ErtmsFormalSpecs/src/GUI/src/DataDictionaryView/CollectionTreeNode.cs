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
using DataDictionary.Types;
using GUI.Converters;

namespace GUI.DataDictionaryView
{
    public class CollectionTreeNode : TypeTreeNode<Collection>
    {
        private class InternalTypesConverter : TypesConverter
        {
            public override StandardValuesCollection
                GetStandardValues(ITypeDescriptorContext context)
            {
                return GetValues(((ItemEditor) context.Instance).Item);
            }
        }

        /// <summary>
        ///     The editor for Range types
        /// </summary>
        private class ItemEditor : TypeEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            [Category("Description")]
            public override string Name
            {
                get { return base.Name; }
                set { base.Name = value; }
            }

            /// <summary>
            ///     The variable type
            /// </summary>
            [Category("Description")]
            [Editor(typeof (TypeUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (TypeUITypeConverter))]
            public Collection Type
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            /// <summary>
            ///     The structure element default value
            /// </summary>
            [Category("Description")]
            [Editor(typeof (DefaultValueUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (DefaultValueUITypeConverter))]
            public Collection DefaultValue
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            /// <summary>
            ///     The default value of the variable
            /// </summary>
            [Category("Description")]
            public int MaxSize
            {
                get { return Item.getMaxSize(); }
                set { Item.setMaxSize(value); }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public CollectionTreeNode(Collection item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
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
        ///     Adds an update of the seleced collection to the chosen patch dictionary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AddCollectionUpdate(object sender, EventArgs args)
        {
            Dictionary dictionary = GetPatchDictionary();

            if (dictionary != null)
            {
                ModelElement updatedElement = dictionary.findByFullName(Item.FullName) as ModelElement;
                if (updatedElement == null)
                {
                    // If the element does not already exist in the patch, add a copy to it
                    updatedElement = Item.CreateCollectionUpdate(dictionary);
                }
                // navigate to the collection, whether it was created or not
                GUIUtils.MDIWindow.RefreshModel();
                GUIUtils.MDIWindow.Select(updatedElement);
            }
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("Update", new EventHandler(AddCollectionUpdate)));
            retVal.Add(newItem);
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}