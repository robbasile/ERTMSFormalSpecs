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
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Functions;

namespace GUI.DataDictionaryView
{
    public class ProcedureTreeNode : ReqRelatedTreeNode<Procedure>
    {
        private class ItemEditor : ReqRelatedEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            /// <summary>
            ///     The item name
            /// </summary>
            [Category("Description")]
            public override string Name
            {
                get { return base.Name; }
                set { base.Name = value; }
            }
        }

        /// <summary>
        ///     The rules associates to this procedure
        /// </summary>
        public ProcedureRulesTreeNode rules;

        /// <summary>
        ///     The parameters of this procedure
        /// </summary>
        public ProcedureParametersTreeNode parameters;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public ProcedureTreeNode(Procedure item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public ProcedureTreeNode(Procedure item, bool buildSubNodes, string name, bool isFolder, bool addRequirements)
            : base(item, buildSubNodes, name, isFolder, addRequirements)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            rules = new ProcedureRulesTreeNode(Item, buildSubNodes);
            parameters = new ProcedureParametersTreeNode(Item, buildSubNodes);
            Nodes.Add(parameters);
            Nodes.Add(rules);
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        public void AddProcedureUpdate(object sender, EventArgs args)
        {
            Dictionary dictionary = GetPatchDictionary();

            if (dictionary != null)
            {
                ModelElement updatedElement = dictionary.findByFullName(Item.FullName) as ModelElement;
                if (updatedElement == null)
                {
                    // If the element does not already exist in the patch, add a copy of the function to it
                    // Get the enclosing namespace (by splitting the fullname and asking a recursive function to provide or make it)
                    updatedElement = Item.CreateProcedureUpdate(dictionary);
                }
                // navigate to the procedure, whether it was created or not
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
            newItem.MenuItems.Add(new MenuItem("Update", new EventHandler(AddProcedureUpdate)));
            retVal.Add(newItem);
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}