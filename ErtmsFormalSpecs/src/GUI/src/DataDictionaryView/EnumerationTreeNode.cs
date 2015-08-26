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
using DataDictionary.Constants;
using GUI.Converters;
using Enum = DataDictionary.Types.Enum;

namespace GUI.DataDictionaryView
{
    public class EnumerationTreeNode : TypeTreeNode<Enum>
    {
        private class ItemEditor : TypeEditor
        {
            /// <summary>
            ///     The enumeration default value
            /// </summary>
            [Category("Description")]
            [Editor(typeof (DefaultValueUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (DefaultValueUITypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public Enum DefaultValue
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            [Category("Description"),
             Editor(
                 @"System.Windows.Forms.Design.StringCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                 typeof (UITypeEditor))]
            // ReSharper disable once UnusedMember.Local
            public List<string> Values
            {
                get
                {
                    List<string> result = new List<string>();
                    foreach (EnumValue val in Item.Values)
                    {
                        result.Add(val.getName());
                    }
                    return result;
                }
                set
                {
                    Item.Values.Clear();
                    foreach (string s in value)
                    {
                        EnumValue val = new EnumValue {Name = s};
                        Item.Values.Add(val);
                    }
                }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public EnumerationTreeNode(Enum item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        /// <param name="name"></param>
        /// <param name="isFolder"></param>
        /// <param name="addRequirements"></param>
        public EnumerationTreeNode(Enum item, bool buildSubNodes, string name, bool isFolder, bool addRequirements)
            : base(item, buildSubNodes, name, isFolder, addRequirements)
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

            subNodes.Add(new EnumerationValuesTreeNode(Item, recursive));
            subNodes.Add(new SubEnumerationsTreeNode(Item, recursive));
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddValueHandler(object sender, EventArgs args)
        {
            Item.appendValues(EnumValue.CreateDefault(Item.Values));
        }

        /// <summary>
        ///     Finds or creates an update for the current element.
        /// </summary>
        /// <returns></returns>
        protected override ModelElement FindOrCreateUpdate()
        {
            ModelElement retVal = null;

            Dictionary dictionary = GetPatchDictionary();

            if (dictionary != null)
            {
                retVal = dictionary.FindByFullName(Item.FullName) as ModelElement;
                if (retVal == null)
                {
                    // If the element does not already exist in the patch, add a copy of the enumeration to it
                    // Get the enclosing namespace (by splitting the fullname and asking a recursive function to provide or make it)
                    retVal = Item.CreateEnumUpdate(dictionary);
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
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("Value", AddValueHandler));
            retVal.Add(newItem);

            MenuItem updateItem = new MenuItem("Update...");
            updateItem.MenuItems.Add(new MenuItem("Update", AddUpdate));
            updateItem.MenuItems.Add(new MenuItem("Remove", RemoveInUpdate));
            retVal.Add(updateItem);

            retVal.Add(new MenuItem("Delete", DeleteHandler));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}