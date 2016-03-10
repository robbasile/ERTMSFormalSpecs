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

using System.ComponentModel;
using System.Drawing;
using DataDictionary.Types;

namespace GUI.DataDictionaryView
{
    public abstract class TypeTreeNode<T> : ReqRelatedTreeNode<T>
        where T : Type
    {
        /// <summary>
        ///     The editor for message variables
        /// </summary>
        protected class TypeEditor : ReqRelatedEditor
        {
            [Category("Display")]
            public Size Size
            {
                get { return new Size(Item.Width, Item.Height); }
                set
                {
                    Item.Width = value.Width;
                    Item.Height = value.Height;

                }
            }

            [Category("Display")]
            public Point Location
            {
                get { return new Point(Item.X, Item.Y); }
                set
                {
                    Item.X = value.X;
                    Item.Y = value.Y;
                }
            }

            [Category("Display")]
            public bool Hidden
            {
                get { return Item.Hidden; }
                set { Item.Hidden = value; }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="buildSubNodes"></param>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <param name="isFolder"></param>
        /// <param name="addRequirements"></param>
        protected TypeTreeNode(T item, bool buildSubNodes, string name = null, bool isFolder = false,
            bool addRequirements = true)
            : base(item, buildSubNodes, name, isFolder, addRequirements)
        {
        }
    }
}