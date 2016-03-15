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
using System.Windows.Forms;
using DataDictionary.RuleCheck;

namespace GUI.RuleDisabling
{
    public class DisabledRuleCheckTreeNode : ModelElementTreeNode<RuleCheckIdentifier>
    {
        /// <summary>
        ///     Indicates that this req ref can be removed from its model
        /// </summary>
        private bool CanBeDeleted { get; set; }

        private class ItemEditor : Editor
        {
            /// <summary>
            ///     The item name
            /// </summary>
            [Category("Description")]
            public virtual string Name
            {
                get
                {
                    string retVal = "";

                    if (Item != null)
                    {
                        retVal = Item.Name;
                    }

                    return retVal;
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
        public DisabledRuleCheckTreeNode(RuleCheckIdentifier item, bool buildSubNodes, bool canBeDeleted, string name = null)
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

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {};

            if (CanBeDeleted)
            {
                retVal.Add(new MenuItem("Delete", DeleteHandler));
            }

            return retVal;
        }
    }
}