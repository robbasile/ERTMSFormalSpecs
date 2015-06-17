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
using GUI.SpecificationView;
using NameSpace = DataDictionary.Types.NameSpace;
using Paragraph = DataDictionary.Specification.Paragraph;
using ReqRef = DataDictionary.ReqRef;
using Structure = DataDictionary.Types.Structure;

namespace GUI.DataDictionaryView
{
    public class InterfacesTreeNode : ModelElementTreeNode<NameSpace>
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
        public InterfacesTreeNode(NameSpace item, bool buildSubNodes)
            : base(item, buildSubNodes, "Interfaces", true)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        /// <param name="name"></param>
        /// <param name="isFolder"></param>
        public InterfacesTreeNode(NameSpace item, bool buildSubNodes, string name, bool isFolder)
            : base(item, buildSubNodes, name, isFolder)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (Structure structure in Item.Structures)
            {
                if (structure.IsAbstract)
                {
                    Nodes.Add(new InterfaceTreeNode(structure, buildSubNodes));
                }
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

        /// <summary>
        ///     Adds an interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public virtual void AddHandler(object sender, EventArgs args)
        {
            Structure structure = (Structure) acceptor.getFactory().createStructure();
            structure.Name = "<Interface" + (GetNodeCount(false) + 1) + ">";
            structure.IsAbstract = true;
            AddInterface(structure);
        }

        /// <summary>
        ///     Adds an interface in this collection of interfaces
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        public InterfaceTreeNode AddInterface(Structure structure)
        {
            Item.appendStructures(structure);

            InterfaceTreeNode retVal = new InterfaceTreeNode(structure, true);
            Nodes.Add(retVal);
            SortSubNodes();

            return retVal;
        }

        /// <summary>
        ///     Adds a structure in this collection of structures
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        public StructureTreeNode AddStructure(Structure structure)
        {
            Item.appendStructures(structure);

            StructureTreeNode retVal = new StructureTreeNode(structure, true);
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
        ///     Accepts drop of a tree node, in a drag & drop operation
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);

            if (SourceNode is InterfaceTreeNode)
            {
                InterfaceTreeNode interfaceTreeNode = SourceNode as InterfaceTreeNode;
                Structure structure = interfaceTreeNode.Item;

                interfaceTreeNode.Delete();
                AddStructure(structure);
            }
        }

        /// <summary>
        ///     Update counts according to the selected folder
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(false);

            GUIUtils.MDIWindow.SetStatus(Item.Structures.Count +
                                         (Item.Structures.Count > 1 ? " interfaces " : " interface ") + "selected.");
        }
    }


    public class StructuresTreeNode : InterfacesTreeNode
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
        public StructuresTreeNode(NameSpace item, bool buildSubNodes)
            : base(item, buildSubNodes, "Structures", true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);
            Nodes.Clear();

            foreach (Structure structure in Item.Structures)
            {
                if (!structure.IsAbstract)
                {
                    Nodes.Add(new StructureTreeNode(structure, buildSubNodes));
                }
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

        /// <summary>
        ///     Adds a structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void AddHandler(object sender, EventArgs args)
        {
            Structure structure = (Structure) acceptor.getFactory().createStructure();
            structure.Name = "<Structure" + (GetNodeCount(false) + 1) + ">";
            AddStructure(structure);
        }


        /// <summary>
        ///     Accepts drop of a tree node, in a drag & drop operation
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);

            if (SourceNode is StructureTreeNode)
            {
                StructureTreeNode structureTreeNode = SourceNode as StructureTreeNode;
                Structure structure = structureTreeNode.Item;

                structureTreeNode.Delete();
                AddStructure(structure);
            }
            else if (SourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode node = SourceNode as ParagraphTreeNode;
                Paragraph paragaph = node.Item;

                Structure structure = (Structure) acceptor.getFactory().createStructure();
                structure.Name = paragaph.Name;

                ReqRef reqRef = (ReqRef) acceptor.getFactory().createReqRef();
                reqRef.Name = paragaph.FullId;
                structure.appendRequirements(reqRef);
                AddStructure(structure);
            }
        }

        /// <summary>
        ///     Update counts according to the selected folder
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(false);

            GUIUtils.MDIWindow.SetStatus(Item.Structures.Count +
                                         (Item.Structures.Count > 1 ? " structures " : " structure ") + "selected.");
        }
    }
}