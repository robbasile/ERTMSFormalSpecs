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
using DataDictionary.Types;
using GUI.Converters;

namespace GUI.DataDictionaryView
{
    public class ParameterTreeNode : ModelElementTreeNode<Parameter>
    {
        private class InternalNameSpaceConverter : NameSpaceConverter
        {
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                ItemEditor editor = ((ItemEditor) context.Instance);

                return new StandardValuesCollection(GetValues(editor.Item.Dictionary));
            }
        }

        private class ItemEditor : NamedEditor
        {
            /// <summary>
            ///     The parameter namespace
            /// </summary>
            private string _namSpace;

            [Category("Description"), TypeConverter(typeof (InternalNameSpaceConverter))]
            // ReSharper disable once UnusedMember.Local
            public string NameSpace
            {
                get
                {
                    if (_namSpace == null)
                    {
                        ITypedElement element = OverallTypedElementFinder.INSTANCE.findByName(Item, Item.NameSpace.Name);

                        if (element != null && element.NameSpace != null)
                        {
                            _namSpace = element.NameSpace.Name;
                        }
                    }

                    if (_namSpace == null)
                    {
                        if (Item.NameSpace != null)
                        {
                            _namSpace = Item.NameSpace.Name;
                        }
                        else
                        {
                            _namSpace = "Default";
                        }
                    }

                    return _namSpace;
                }

                set { _namSpace = value; }
            }

            /// <summary>
            ///     The parameter type
            /// </summary>
            [Category("Description")]
            [Editor(typeof (TypeUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (TypeUITypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public Parameter Type
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
        /// <param name="parameter"></param>
        /// <param name="buildSubNodes"></param>
        /// <param name="name"></param>
        public ParameterTreeNode(Parameter parameter, bool buildSubNodes, string name = null)
            : base(parameter, buildSubNodes, name)
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

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Delete", DeleteHandler)};

            return retVal;
        }
    }
}