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
using System.Windows.Forms;
using DataDictionary.Generated;
using EnumValue = DataDictionary.Constants.EnumValue;
using Range = DataDictionary.Types.Range;

namespace GUI.DataDictionaryView
{
    public class RangeValuesTreeNode : RangeTreeNode
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public RangeValuesTreeNode(Range item, bool buildSubNodes)
            : base(item, buildSubNodes, "Special values", true, false)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            Nodes.Clear();

            foreach (EnumValue value in Item.SpecialValues)
            {
                Nodes.Add(new EnumerationValueTreeNode(value, buildSubNodes));
            }
            SortSubNodes();
            SubNodesBuilt = true;
        }

        public override void AddSpecialValueHandler(object sender, EventArgs e)
        {
            EnumValue value = (EnumValue) acceptor.getFactory().createEnumValue();
            value.Name = "<unnamed>";
            AppendSpecialValue(value);
        }

        /// <summary>
        ///     Adds a new special value to this range
        /// </summary>
        /// <param name="value"></param>
        public void AppendSpecialValue(EnumValue value)
        {
            Item.appendSpecialValues(value);
            Nodes.Add(new EnumerationValueTreeNode(value, true));
            SortSubNodes();
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add", new EventHandler(AddSpecialValueHandler)));

            return retVal;
        }
    }
}