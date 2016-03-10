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
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Variables;
using GUI.FunctionalView;
using GUI.Properties;
using Enum = DataDictionary.Types.Enum;

namespace GUI.DataDictionaryView
{
    public class NameSpaceTreeNode : GraphicalDisplayElementNode<NameSpace>
    {
        private class ItemEditor : GraphicalDisplayEditor
        {
        }

        private readonly bool _isDirectory;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public NameSpaceTreeNode(NameSpace item, bool buildSubNodes)
            : base(item, buildSubNodes, null, false)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        /// <param name="name"></param>
        /// <param name="isFolder"></param>
        protected NameSpaceTreeNode(NameSpace item, bool buildSubNodes, string name, bool isFolder)
            : base(item, buildSubNodes, name, isFolder)
        {
            _isDirectory = true;
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            base.BuildSubNodes(subNodes, recursive);

            subNodes.Add(new NameSpaceSubNameSpacesTreeNode(Item, recursive));
            subNodes.Add(new RangesTreeNode(Item, recursive));
            subNodes.Add(new EnumerationsTreeNode(Item, recursive));
            subNodes.Add(new InterfacesTreeNode(Item, recursive));
            subNodes.Add(new StructuresTreeNode(Item, recursive));
            subNodes.Add(new CollectionsTreeNode(Item, recursive));
            subNodes.Add(new StateMachinesTreeNode(Item, recursive));
            subNodes.Add(new FunctionsTreeNode(Item, recursive));
            subNodes.Add(new NameSpaceProceduresTreeNode(Item, recursive));
            subNodes.Add(new NameSpaceVariablesTreeNode(Item, recursive));
            subNodes.Add(new NameSpaceRulesTreeNode(Item, recursive));
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        private void AddNamespaceHandler(object sender, EventArgs args)
        {
            Item.appendNameSpaces(NameSpace.CreateDefault(Item.NameSpaces));
        }

        private void AddRangeHandler(object sender, EventArgs args)
        {
            Item.appendRanges(Range.CreateDefault(Item.Ranges));
        }

        private void AddEnumerationHandler(object sender, EventArgs args)
        {
            Item.appendEnumerations(Enum.CreateDefault(Item.Enumerations));
        }

        private void AddInterfaceHandler(object sender, EventArgs args)
        {
            Item.appendStructures(Structure.CreateDefault(Item.Structures, true));
        }

        private void AddStructureHandler(object sender, EventArgs args)
        {
            Item.appendStructures(Structure.CreateDefault(Item.Structures, false));
        }

        private void AddCollectionHandler(object sender, EventArgs args)
        {
            Item.appendCollections(Collection.CreateDefault(Item.Collections));
        }

        private void AddStateMachineHandler(object sender, EventArgs args)
        {
            Item.appendStateMachines(StateMachine.CreateDefault(Item.StateMachines));
        }

        private void AddFunctionHandler(object sender, EventArgs args)
        {
            Item.appendFunctions(Function.CreateDefault(Item.Functions));
        }

        private void AddProcedureHandler(object sender, EventArgs args)
        {
            Item.appendProcedures(Procedure.CreateDefault(Item.Procedures));
        }

        private void AddVariableHandler(object sender, EventArgs args)
        {
            Item.appendVariables(Variable.CreateDefault(Item.Variables));
        }

        private void AddRuleHandler(object sender, EventArgs args)
        {
            Item.appendRules(Rule.CreateDefault(Item.Rules));
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
            window.Text = Item.Name + @" " + Resources.NameSpaceTreeNode_ShowFunctionalViewHandler_functional_view;
        }

        /// <summary>
        ///     Find or creates an update for the current element
        /// </summary>
        /// <returns></returns>
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
                    retVal = Item.CreateUpdateInDictionary(dictionary);
                }
                // Navigate to the element, whether it was created or not
                EfsSystem.Instance.Context.SelectElement(retVal, this, Context.SelectionCriteria.DoubleClick);
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

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("Namespace", AddNamespaceHandler));
            newItem.MenuItems.Add(new MenuItem("Range", AddRangeHandler));
            newItem.MenuItems.Add(new MenuItem("Enumeration", AddEnumerationHandler));
            newItem.MenuItems.Add(new MenuItem("Interface", AddInterfaceHandler));
            newItem.MenuItems.Add(new MenuItem("Structure", AddStructureHandler));
            newItem.MenuItems.Add(new MenuItem("Collection", AddCollectionHandler));
            newItem.MenuItems.Add(new MenuItem("State machine", AddStateMachineHandler));
            newItem.MenuItems.Add(new MenuItem("Function", AddFunctionHandler));
            newItem.MenuItems.Add(new MenuItem("Procedure", AddProcedureHandler));
            newItem.MenuItems.Add(new MenuItem("Variable", AddVariableHandler));
            newItem.MenuItems.Add(new MenuItem("Rule", AddRuleHandler));
            retVal.Add(newItem);

            MenuItem updatesItem = new MenuItem("Update...");
            updatesItem.MenuItems.Add(new MenuItem("Update", AddUpdate));
            updatesItem.MenuItems.Add(new MenuItem("Remove", RemoveInUpdate));
            retVal.Add(updatesItem);

            retVal.Add(new MenuItem("Delete", DeleteHandler));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(5, new MenuItem("Functional view", ShowFunctionalViewHandler));

            return retVal;
        }

        /// <summary>
        ///     Accepts a drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);

            if (_isDirectory)
            {
                BaseTreeNode parent = Parent as BaseTreeNode;
                if (parent != null)
                {
                    parent.AcceptDrop(sourceNode);
                }
            }
            else
            {
                if (sourceNode is VariableTreeNode)
                {
                    NameSpaceVariablesTreeNode node = SubNode<NameSpaceVariablesTreeNode>();
                    if (node != null)
                    {
                        node.AcceptDrop(sourceNode);
                    }
                }
                else if (sourceNode is ProcedureTreeNode)
                {
                    NameSpaceProceduresTreeNode node = SubNode<NameSpaceProceduresTreeNode>();
                    if (node != null)
                    {
                        node.AcceptDrop(sourceNode);
                    }
                }
                else if (sourceNode is RuleTreeNode)
                {
                    NameSpaceRulesTreeNode node = SubNode<NameSpaceRulesTreeNode>();
                    if (node != null)
                    {
                        node.AcceptDrop(sourceNode);
                    }
                }
                else if (sourceNode is StructureTreeNode)
                {
                    StructuresTreeNode node = SubNode<StructuresTreeNode>();
                    if (node != null)
                    {
                        node.AcceptDrop(sourceNode);
                    }
                }
                else if (sourceNode is FunctionTreeNode)
                {
                    FunctionsTreeNode node = SubNode<FunctionsTreeNode>();
                    if (node != null)
                    {
                        node.AcceptDrop(sourceNode);
                    }
                }
                else if (sourceNode is InterfaceTreeNode)
                {
                    InterfacesTreeNode node = SubNode<InterfacesTreeNode>();
                    if (node != null)
                    {
                        node.AcceptDrop(sourceNode);
                    }
                }
                else if (sourceNode is NameSpaceTreeNode)
                {
                    DialogResult result = MessageBox.Show(
                        Resources.NameSpaceTreeNode_AcceptDrop_This_will_move_the_namespace__are_you_sure___,
                        Resources.NameSpaceTreeNode_AcceptDrop_Confirm_moving_the_namespace,
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question);
                    if (result == DialogResult.OK)
                    {
                        NameSpaceTreeNode nameSpaceTreeNode = sourceNode as NameSpaceTreeNode;
                        NameSpace nameSpace = nameSpaceTreeNode.Item;

                        nameSpaceTreeNode.Delete();
                        Item.appendNameSpaces(nameSpace);
                    }
                }
            }
        }
    }
}