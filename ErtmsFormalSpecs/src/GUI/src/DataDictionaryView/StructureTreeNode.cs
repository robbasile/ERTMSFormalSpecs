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
using DataDictionary.Functions;
using DataDictionary.Types;

namespace GUI.DataDictionaryView
{
    public class InterfaceTreeNode : TypeTreeNode<Structure>
    {
        private class ItemEditor : TypeEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public InterfaceTreeNode(Structure item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        /// <param name="name"></param>
        /// <param name="isFolder"></param>
        /// <param name="addRequirements"></param>
        public InterfaceTreeNode(Structure item, bool buildSubNodes, string name, bool isFolder, bool addRequirements)
            : base(item, buildSubNodes, name, isFolder, addRequirements)
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

            subNodes.Add(new StructureElementsTreeNode(Item, recursive));
            subNodes.Add(new StructureInterfacesTreeNode(Item, recursive));
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddStructureElementHandler(object sender, EventArgs args)
        {
            Item.appendElements(StructureElement.CreateDefault(Item.Elements));
        }

        public void AddInterfaceHandler(object sender, EventArgs args)
        {
            Item.appendInterfaces(StructureRef.CreateDefault(Item.Interfaces));
        }

        private void GenerateInheritedFieldsHandler(object sender, EventArgs args)
        {
            Item.GenerateInheritedFields();
        }

        /// <summary>
        ///     Finds or creates an update for the current element.
        ///     Used for both interfaces and structures.
        /// </summary>
        protected override ModelElement FindOrCreateUpdate()
        {
            ModelElement retVal = null;

            Dictionary dictionary = GetPatchDictionary();

            if (dictionary != null)
            {
                retVal = dictionary.FindByFullName(Item.FullName) as ModelElement;
                if (retVal == null)
                {
                    // If the element does not already exist in the patch, add a copy to it
                    retVal = Item.CreateStructureUpdate(dictionary);
                }
                // Navigate to the rule, whether it was created or not
                EFSSystem.INSTANCE.Context.SelectElement(Model, this, Context.SelectionCriteria.DoubleClick);
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
                newItem.MenuItems.Add(new MenuItem("Element", AddStructureElementHandler));
                newItem.MenuItems.Add(new MenuItem("Interface", AddInterfaceHandler));
                retVal.Add(newItem);

                MenuItem updateItem = new MenuItem("Update...");
                updateItem.MenuItems.Add(new MenuItem("Update", AddUpdate));
                updateItem.MenuItems.Add(new MenuItem("Remove", RemoveInUpdate));
                retVal.Add(updateItem);
            }

            if (Item.Interfaces.Count > 0)
            {
                retVal.Add(new MenuItem("Generate inherited fields", GenerateInheritedFieldsHandler));
            }

            retVal.Add(new MenuItem("Delete", DeleteHandler));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }

    public class StructureTreeNode : InterfaceTreeNode
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public StructureTreeNode(Structure item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        /// <param name="name"></param>
        /// <param name="isFolder"></param>
        /// <param name="addRequirements"></param>
        public StructureTreeNode(Structure item, bool buildSubNodes, string name, bool isFolder, bool addRequirements)
            : base(item, buildSubNodes, name, isFolder, addRequirements)
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

            subNodes.Add(new StructureProceduresTreeNode(Item, recursive));
            subNodes.Add(new StructureStateMachinesTreeNode(Item, recursive));
            subNodes.Add(new RulesTreeNode(Item, recursive));
        }


        private void AddProcedureHandler(object sender, EventArgs args)
        {
            Item.appendProcedures(Procedure.CreateDefault(Item.Procedures));
        }

        private void AddStateMachineHandler(object sender, EventArgs args)
        {
            Item.appendStateMachines(StateMachine.CreateDefault(Item.StateMachines));
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("Interface", AddInterfaceHandler));
            newItem.MenuItems.Add(new MenuItem("Structure element", AddStructureElementHandler));
            newItem.MenuItems.Add(new MenuItem("Procedure", AddProcedureHandler));
            newItem.MenuItems.Add(new MenuItem("State machine", AddStateMachineHandler));
            retVal.Add(newItem);

            MenuItem updateItem = new MenuItem("Update...");
            updateItem.MenuItems.Add(new MenuItem("Update", AddUpdate));
            updateItem.MenuItems.Add(new MenuItem("Remove", RemoveInUpdate));
            retVal.Add(updateItem);
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}