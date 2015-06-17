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
using NameSpace = DataDictionary.Types.NameSpace;

namespace GUI.DataDictionaryView
{
    public class NameSpaceSubNameSpacesTreeNode : NameSpaceTreeNode
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        public NameSpaceSubNameSpacesTreeNode(NameSpace item, bool buildSubNodes)
            : base(item, buildSubNodes, "Namespaces", true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            Nodes.Clear();

            foreach (NameSpace nameSpace in Item.NameSpaces)
            {
                Nodes.Add(new NameSpaceTreeNode(nameSpace, buildSubNodes));
            }
            SortSubNodes();
            SubNodesBuilt = true;
        }

        public void AddHandler(object sender, EventArgs args)
        {
            NameSpace nameSpace = (NameSpace) acceptor.getFactory().createNameSpace();
            nameSpace.Name = "<NameSpace" + (GetNodeCount(false) + 1) + ">";
            AddSubNameSpace(nameSpace);
        }

        /// <summary>
        ///     Adds a namespace in the corresponding namespace
        /// </summary>
        /// <param name="nameSpace"></param>
        public NameSpaceTreeNode AddSubNameSpace(NameSpace nameSpace)
        {
            Item.appendNameSpaces(nameSpace);
            NameSpaceTreeNode retVal = new NameSpaceTreeNode(nameSpace, true);
            Nodes.Add(retVal);
            SortSubNodes();

            return retVal;
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add", new EventHandler(AddHandler)));

            return retVal;
        }

        /// <summary>
        ///     Update counts according to the selected folder
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(false);

            List<NameSpace> namespaces = new List<NameSpace>();
            foreach (NameSpace aNamespace in Item.NameSpaces)
            {
                namespaces.Add(aNamespace);
            }

            GUIUtils.MDIWindow.SetStatus(CreateStatMessage(namespaces, true));
        }
    }
}