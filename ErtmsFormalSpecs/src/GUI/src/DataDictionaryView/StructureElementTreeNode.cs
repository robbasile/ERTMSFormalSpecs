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
using DataDictionary.Generated;
using GUI.Converters;
using Dictionary = DataDictionary.Dictionary;
using StructureElement = DataDictionary.Types.StructureElement;

namespace GUI.DataDictionaryView
{
    public class StructureElementTreeNode : ReqRelatedTreeNode<StructureElement>
    {
        private class ItemEditor : ReqRelatedEditor
        {
            [Category("Description")]
            public override string Name
            {
                get { return base.Name; }
                set { base.Name = value; }
            }

            /// <summary>
            ///     The structure element type
            /// </summary>
            [Category("Description")]
            [Editor(typeof (TypeUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (TypeUITypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public StructureElement Type
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
            // ReSharper disable once UnusedMember.Local
            public StructureElement DefaultValue
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            /// <summary>
            ///     The variable mode
            /// </summary>
            [Category("Description"), TypeConverter(typeof (VariableModeConverter))]
            // ReSharper disable once UnusedMember.Local
            public acceptor.VariableModeEnumType Mode
            {
                get { return Item.Mode; }
                set { Item.Mode = value; }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public StructureElementTreeNode(StructureElement item, bool buildSubNodes)
            : base(item, buildSubNodes, null, false)
        {
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }


        protected override ModelElement FindOrCreateUpdate()
        {
            ModelElement retVal = null;

            Dictionary dictionary = GetPatchDictionary();

            if (dictionary != null)
            {
                retVal = dictionary.FindByFullName(Item.FullName) as ModelElement;
                if (retVal == null)
                {
                    // If the element does not already exist in the patch, add a copy to it
                    retVal = Item.CreateStructureElementUpdate(dictionary);
                }
                // Navigate to the element, whether it was created or not
                EFSSystem.INSTANCE.Context.SelectElement(retVal, this, Context.SelectionCriteria.DoubleClick);
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

            MenuItem updatesItem = new MenuItem("Update...");
            updatesItem.MenuItems.Add(new MenuItem("Update", AddUpdate));
            updatesItem.MenuItems.Add(new MenuItem("Remove", RemoveInUpdate));
            retVal.Add(updatesItem);

            retVal.Add(new MenuItem("Delete", DeleteHandler));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}