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
using DataDictionary.Rules;
using GUI.Converters;

namespace GUI.DataDictionaryView
{
    public class PreConditionTreeNode : ModelElementTreeNode<PreCondition>
    {
        private class ItemEditor : Editor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            [Category("Description")]
            [Editor(typeof (ExpressionableUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (ExpressionableUITypeConverter))]
            public PreCondition Expression
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            [Category("Description")]
            [Editor(typeof (CommentableUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (CommentableUITypeConverter))]
            public PreCondition Comment
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            /// <summary>
            ///     Sets the verified flag of the the enclosing rule
            /// </summary>
            /// <param name="val"></param>
            private void SetRuleAsVerified(bool val)
            {
                if (Item.Rule != null)
                {
                    Item.Rule.setVerified(val);
                }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public PreConditionTreeNode(PreCondition item, bool buildSubNodes)
            : base(item, buildSubNodes, null)
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
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}