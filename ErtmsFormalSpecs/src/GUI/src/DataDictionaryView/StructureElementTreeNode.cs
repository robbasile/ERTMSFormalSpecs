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
using DataDictionary.Generated;
using GUI.Converters;
using NameSpace = DataDictionary.Types.NameSpace;
using StructureElement = DataDictionary.Types.StructureElement;
using Type = DataDictionary.Types.Type;

namespace GUI.DataDictionaryView
{
    public class StructureElementTreeNode : ReqRelatedTreeNode<StructureElement>
    {
        private class InternalTypesConverter : TypesConverter
        {
            public override StandardValuesCollection
                GetStandardValues(ITypeDescriptorContext context)
            {
                return GetValues(((ItemEditor) context.Instance).Item);
            }
        }

        private class InternalValuesConverter : ValuesConverter
        {
            public override StandardValuesCollection
                GetStandardValues(ITypeDescriptorContext context)
            {
                ItemEditor editor = (ItemEditor) context.Instance;
                NameSpace nameSpace = editor.Item.NameSpace;
                Type type = editor.Item.Type;

                return GetValues(nameSpace, type);
            }
        }

        private class ItemEditor : ReqRelatedEditor
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
            ///     The structure element type
            /// </summary>
            [Category("Description")]
            [Editor(typeof (TypeUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (TypeUITypeConverter))]
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
        public StructureElementTreeNode(StructureElement item, bool buildSubNodes)
            : base(item, buildSubNodes, null, false)
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


        protected override DataDictionary.ModelElement FindOrCreateUpdate()
        {
            ModelElement retVal = null;

            DataDictionary.Dictionary dictionary = GetPatchDictionary();

            if (dictionary != null)
            {
                retVal = dictionary.findByFullName(Item.FullName) as ModelElement;
                if (retVal == null)
                {
                    // If the 
                    retVal = Item.CreateStructureElementUpdate(dictionary);
                }
                GUIUtils.MDIWindow.RefreshModel();
                GUIUtils.MDIWindow.Select(retVal);
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
            updatesItem.MenuItems.Add(new MenuItem("Update", new EventHandler(AddUpdate)));
            updatesItem.MenuItems.Add(new MenuItem("Remove", new EventHandler(RemoveInUpdate)));
            retVal.Add(updatesItem);

            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}