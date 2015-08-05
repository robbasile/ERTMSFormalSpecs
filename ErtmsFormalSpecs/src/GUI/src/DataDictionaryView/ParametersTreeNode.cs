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
using Function = DataDictionary.Functions.Function;
using Parameter = DataDictionary.Parameter;

namespace GUI.DataDictionaryView
{
    public class ParametersTreeNode : FunctionTreeNode
    {
        private class ItemEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Constructor (for function)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public ParametersTreeNode(Function item, bool buildSubNodes)
            : base(item, buildSubNodes, "Parameters", true, false)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            // Do not use the base version
            SubNodesBuilt = true;

            foreach (Parameter parameter in Item.FormalParameters)
            {
                subNodes.Add(new ParameterTreeNode(parameter, recursive));
            }
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
        ///     Create structure based on the subsystem structure
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);

            if (sourceNode is ParameterTreeNode)
            {
                ParameterTreeNode node = sourceNode as ParameterTreeNode;
                Parameter parameter = node.Item;
                node.Delete();
                Item.appendParameters(parameter);
            }
        }

        public void AddHandler(object sender, EventArgs args)
        {
            Item.appendParameters(Parameter.CreateDefault(Item.FormalParameters));
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Add", AddHandler)};

            return retVal;
        }
    }
}