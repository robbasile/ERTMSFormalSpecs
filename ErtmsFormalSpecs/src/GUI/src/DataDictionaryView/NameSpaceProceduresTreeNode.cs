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
        public NameSpaceProceduresTreeNode(NameSpace item, bool buildSubNodes)
            : base(item, buildSubNodes, "Procedures", true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (Procedure procedure in Item.Procedures)
            {
                Nodes.Add(new ProcedureTreeNode(procedure, buildSubNodes));
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
            Procedure procedure = (Procedure) acceptor.getFactory().createProcedure();
            procedure.Name = "<Procedure" + (GetNodeCount(false) + 1) + ">";
            AddProcedure(procedure);
        }

        /// <summary>
        ///     Adds a procedure in the corresponding namespace
        /// </summary>
        /// <param name="procedure"></param>
        public ProcedureTreeNode AddProcedure(Procedure procedure)
        {
            Item.appendProcedures(procedure);
            ProcedureTreeNode retVal = new ProcedureTreeNode(procedure, true);
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
        ///     Accepts a new procedure
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);

            if (SourceNode is ProcedureTreeNode)
            {
                ProcedureTreeNode procedureTreeNode = SourceNode as ProcedureTreeNode;
                Procedure procedure = procedureTreeNode.Item;

                procedureTreeNode.Delete();
                AddProcedure(procedure);
            }
            else if (SourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode node = SourceNode as ParagraphTreeNode;
                Paragraph paragaph = node.Item;

                Procedure procedure = (Procedure) acceptor.getFactory().createProcedure();
                procedure.Name = paragaph.Name;

                ReqRef reqRef = (ReqRef) acceptor.getFactory().createReqRef();
                reqRef.Name = paragaph.FullId;
                procedure.appendRequirements(reqRef);
                AddProcedure(procedure);
            }
        }

        /// <summary>
        ///     Update counts according to the selected folder
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(false);

            GUIUtils.MDIWindow.SetStatus(Item.Procedures.Count +
                                         (Item.Procedures.Count > 1 ? " procedures " : " procedure ") + "selected.");
        }
    }
}