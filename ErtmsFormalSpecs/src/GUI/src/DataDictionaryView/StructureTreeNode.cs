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
using DataDictionary;
using DataDictionary.Types;

namespace GUI.DataDictionaryView
{
    public class InterfaceTreeNode : TypeTreeNode<Structure>
    {
        private class ItemEditor : TypeEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }
        }

        private NameSpaceTreeNode types
        {
            get { return Parent.Parent as NameSpaceTreeNode; }
        }


        public StructureElementsTreeNode Elements;
        public StructureInterfacesTreeNode Interfaces;


        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public InterfaceTreeNode(Structure item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public InterfaceTreeNode(Structure item, bool buildSubNodes, string name, bool isFolder, bool addRequirements)
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

            Elements = new StructureElementsTreeNode(Item, buildSubNodes);
            Interfaces = new StructureInterfacesTreeNode(Item, buildSubNodes);

            Nodes.Add(Interfaces);
            Nodes.Add(Elements);
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        public void AddStructureElementHandler(object sender, EventArgs args)
        {
            if (Elements != null)
            {
                Elements.AddHandler(sender, args);
            }
        }

        public void AddInterfaceHandler(object sender, EventArgs args)
        {
            if (Interfaces != null)
            {
                Interfaces.AddHandler(sender, args);
            }
        }

        private void GenerateInheritedFieldsHandler(object sender, EventArgs args)
        {
            Item.GenerateInheritedFields();
            Interfaces.Nodes.Clear();
            Interfaces.BuildSubNodes(false);
        }

        /// <summary>
        ///     Finds or creates an update for the current element.
        ///     Used for both interfaces and structures.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override ModelElement FindOrCreateUpdate()
        {
            ModelElement retVal = null;

            Dictionary dictionary = GetPatchDictionary();

            if (dictionary != null)
            {
                retVal = dictionary.findByFullName(Item.FullName) as ModelElement;
                if (retVal == null)
                {
                    // If the element does not already exist in the patch, add a copy to it
                    retVal = Item.CreateStructureUpdate(dictionary);
                }
                // navigate to the structure, whether it was created or not
                GUIUtils.MDIWindow.RefreshModel();
                GUIUtils.MDIWindow.Select(retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            if (Item.IsAbstract) // this is an interface, otherwise it is a structure
            {
                MenuItem newItem = new MenuItem("Add...");
                newItem.MenuItems.Add(new MenuItem("Element", new EventHandler(AddStructureElementHandler)));
                newItem.MenuItems.Add(new MenuItem("Interface", new EventHandler(AddInterfaceHandler)));
                retVal.Add(newItem);

                MenuItem updateItem = new MenuItem("Update...");
                updateItem.MenuItems.Add(new MenuItem("Update", new EventHandler(AddUpdate)));
                updateItem.MenuItems.Add(new MenuItem("Remove", new EventHandler(RemoveInUpdate)));
                retVal.Add(updateItem);
            }

            if (Item.Interfaces.Count > 0)
            {
                retVal.Add(new MenuItem("Generate inherited fields", new EventHandler(GenerateInheritedFieldsHandler)));
            }

            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }

    public class StructureTreeNode : InterfaceTreeNode
    {
        private NameSpaceTreeNode types
        {
            get { return Parent.Parent as NameSpaceTreeNode; }
        }

        private StructureProceduresTreeNode procedures;
        private StructureStateMachinesTreeNode stateMachines;
        private RulesTreeNode rules;


        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public StructureTreeNode(Structure item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public StructureTreeNode(Structure item, bool buildSubNodes, string name, bool isFolder, bool addRequirements)
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

            procedures = new StructureProceduresTreeNode(Item, buildSubNodes);
            stateMachines = new StructureStateMachinesTreeNode(Item, buildSubNodes);
            rules = new RulesTreeNode(Item, buildSubNodes);

            Nodes.Add(procedures);
            Nodes.Add(stateMachines);
            Nodes.Add(rules);
        }


        private void AddProcedureHandler(object sender, EventArgs args)
        {
            if (procedures != null)
            {
                procedures.AddHandler(sender, args);
            }
        }

        private void AddStateMachineHandler(object sender, EventArgs args)
        {
            if (stateMachines != null)
            {
                stateMachines.AddHandler(sender, args);
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
            newItem.MenuItems.Add(new MenuItem("Interface", new EventHandler(AddInterfaceHandler)));
            newItem.MenuItems.Add(new MenuItem("Structure element", new EventHandler(AddStructureElementHandler)));
            newItem.MenuItems.Add(new MenuItem("Procedure", new EventHandler(AddProcedureHandler)));
            newItem.MenuItems.Add(new MenuItem("State machine", new EventHandler(AddStateMachineHandler)));
            retVal.Add(newItem);

            MenuItem updateItem = new MenuItem("Update...");
            updateItem.MenuItems.Add(new MenuItem("Update", new EventHandler(AddUpdate)));
            updateItem.MenuItems.Add(new MenuItem("Remove", new EventHandler(RemoveInUpdate)));
            retVal.Add(updateItem);
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}