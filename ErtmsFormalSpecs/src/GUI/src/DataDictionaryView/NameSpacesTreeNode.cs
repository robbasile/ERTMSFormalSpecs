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
using GUI.FunctionalView;
using GUI.Properties;
using Dictionary = DataDictionary.Dictionary;
using NameSpace = DataDictionary.Types.NameSpace;

namespace GUI.DataDictionaryView
{
    public class NameSpacesTreeNode : ModelElementTreeNode<Dictionary>
    {
        private class ItemEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public NameSpacesTreeNode(Dictionary item, bool buildSubNodes)
            : base(item, buildSubNodes, "Name spaces", true)
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

            foreach (NameSpace nameSpace in Item.NameSpaces)
            {
                subNodes.Add(new NameSpaceTreeNode(nameSpace, recursive));
            }
            subNodes.Sort();
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddHandler(object sender, EventArgs args)
        {
            Item.appendNameSpaces(NameSpace.CreateDefault(Item.NameSpaces));
        }

        /// <summary>
        ///     Shows the functional view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void ShowFunctionalViewHandler(object sender, EventArgs args)
        {
            FunctionalAnalysisWindow window = new FunctionalAnalysisWindow();
            GuiUtils.MdiWindow.AddChildWindow(window);
            window.SetNameSpaceContainer(Item);
            window.Text = Item.Name + @" " + Resources.NameSpacesTreeNode_ShowFunctionalViewHandler_functional_view;
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Add", AddHandler)};

            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(4, new MenuItem("-"));
            retVal.Insert(5, new MenuItem("Functional view", ShowFunctionalViewHandler));

            return retVal;
        }

        /// <summary>
        ///     Accepts drop of a tree node, in a drag & drop operation
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);

            if (sourceNode is NameSpaceTreeNode)
            {
                NameSpaceTreeNode nameSpaceTreeNode = sourceNode as NameSpaceTreeNode;
                NameSpace nameSpace = nameSpaceTreeNode.Item;

                nameSpaceTreeNode.Delete();
                Item.appendNameSpaces(nameSpace);
            }
        }
    }
}