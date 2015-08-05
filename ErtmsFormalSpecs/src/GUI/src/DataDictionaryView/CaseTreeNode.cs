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
using DataDictionary.Generated;
using GUI.Converters;
using Case = DataDictionary.Functions.Case;
using PreCondition = DataDictionary.Rules.PreCondition;

namespace GUI.DataDictionaryView
{
    public class CaseTreeNode : ModelElementTreeNode<Case>
    {
        private class ItemEditor : CommentableEditor
        {
            [Category("Description")]
            [Editor(typeof (ExpressionableUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (ExpressionableUITypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public Case Expression
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
        /// <param name="aCase"></param>
        /// <param name="buildSubNodes"></param>
        public CaseTreeNode(Case aCase, bool buildSubNodes)
            : base(aCase, buildSubNodes)
        {
        }

        /// <summary>
        ///     Protected contstructor for Precondition folder
        /// </summary>
        /// <param name="aCase"></param>
        /// <param name="buildSubNodes"></param>
        /// <param name="name"></param>
        /// <param name="isFolder"></param>
        protected CaseTreeNode(Case aCase, bool buildSubNodes, string name, bool isFolder)
            : base(aCase, buildSubNodes, name, isFolder)
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

            subNodes.Add(new PreConditionsTreeNode(Item, recursive));
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddPreConditionHandler(object sender, EventArgs args)
        {
            PreCondition preCondition = (PreCondition) acceptor.getFactory().createPreCondition();
            preCondition.Condition = "<empty>";
            Item.appendPreConditions(preCondition);
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>
            {
                new MenuItem("Add pre-condition", AddPreConditionHandler),
                new MenuItem("Delete", DeleteHandler)
            };

            return retVal;
        }
    }
}