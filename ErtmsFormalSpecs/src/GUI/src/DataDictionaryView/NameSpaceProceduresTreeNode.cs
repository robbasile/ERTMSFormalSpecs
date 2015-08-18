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
using Procedure = DataDictionary.Functions.Procedure;
using ReqRef = DataDictionary.ReqRef;

namespace GUI.DataDictionaryView
{
    public class NameSpaceProceduresTreeNode : ModelElementTreeNode<NameSpace>
    {
        /// <summary>
        ///     The editor for message variables
        /// </summary>
        private class ItemEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public NameSpaceProceduresTreeNode(NameSpace item, bool buildSubNodes)
            : base(item, buildSubNodes, "Procedures", true)
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

            foreach (Procedure procedure in Item.Procedures)
            {
                subNodes.Add(new ProcedureTreeNode(procedure, recursive));
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
            Item.appendProcedures(Procedure.CreateDefault(Item.Procedures));
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

        /// <summary>
        ///     Accepts a new procedure
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);

            if (sourceNode is ProcedureTreeNode)
            {
                ProcedureTreeNode procedureTreeNode = sourceNode as ProcedureTreeNode;
                Procedure procedure = procedureTreeNode.Item;

                procedureTreeNode.Delete();
                Item.appendProcedures(procedure);
            }
            else if (sourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode node = sourceNode as ParagraphTreeNode;
                Paragraph paragraph = node.Item;

                Procedure procedure = Procedure.CreateDefault(Item.Procedures);
                Item.appendProcedures(procedure);
                procedure.FindOrCreateReqRef(paragraph);
            }
        }
    }
}