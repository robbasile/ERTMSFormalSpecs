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
using GUI.FunctionalView;
using Dictionary = DataDictionary.Dictionary;
using NameSpace = DataDictionary.Types.NameSpace;

namespace GUI.DataDictionaryView
{
    public class NameSpacesTreeNode : ModelElementTreeNode<Dictionary>
    {
        private class ItemEditor : NamedEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        public NameSpacesTreeNode(Dictionary item, bool buildSubNodes)
            : base(item, buildSubNodes, "Name spaces", true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (NameSpace nameSpace in Item.NameSpaces)
            {
                Nodes.Add(new NameSpaceTreeNode(nameSpace, buildSubNodes));
            }
            SortSubNodes();
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        public void AddHandler(object sender, EventArgs args)
        {
            NameSpace nameSpace = (NameSpace) acceptor.getFactory().createNameSpace();
            nameSpace.Name = "<NameSpace" + (GetNodeCount(false) + 1) + ">";
            AddNameSpace(nameSpace);
        }

        /// <summary>
        ///     Adds a namespace in the corresponding namespace
        /// </summary>
        /// <param name="nameSpace"></param>
        public NameSpaceTreeNode AddNameSpace(NameSpace nameSpace)
        {
            Item.appendNameSpaces(nameSpace);
            NameSpaceTreeNode retVal = new NameSpaceTreeNode(nameSpace, true);
            Nodes.Add(retVal);
            SortSubNodes();

            return retVal;
        }

        /// <summary>
        ///     Shows the functional view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void ShowFunctionalViewHandler(object sender, EventArgs args)
        {
            FunctionalAnalysisWindow window = new FunctionalAnalysisWindow();
            GUIUtils.MDIWindow.AddChildWindow(window);
            window.SetNameSpaceContainer(Item);
            window.Text = Item.Name + " functional view";
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add", new EventHandler(AddHandler)));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(4, new MenuItem("-"));
            retVal.Insert(5, new MenuItem("Functional view", new EventHandler(ShowFunctionalViewHandler)));

            return retVal;
        }

        /// <summary>
        ///     Accepts drop of a tree node, in a drag & drop operation
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);

            if (SourceNode is NameSpaceTreeNode)
            {
                NameSpaceTreeNode nameSpaceTreeNode = SourceNode as NameSpaceTreeNode;
                NameSpace nameSpace = nameSpaceTreeNode.Item;

                nameSpaceTreeNode.Delete();
                AddNameSpace(nameSpace);
            }
        }

        /// <summary>
        ///     Update counts according to the selected folder
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(false);

            Window window = BaseForm as Window;
            if (window != null)
            {
                window.modelDiagramPanel.Model = Item;
                window.modelDiagramPanel.RefreshControl();
            }

            List<NameSpace> namespaces = new List<NameSpace>();
            foreach (NameSpace aNamespace in Item.NameSpaces)
            {
                namespaces.Add(aNamespace);
            }
            GUIUtils.MDIWindow.SetStatus(NameSpaceTreeNode.CreateStatMessage(namespaces, true));
        }
    }
}