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

using System.Collections.Generic;
using System.Windows.Forms;
using DataDictionary.Interpreter;
using DataDictionary.Tests;
using DataDictionary.Types;
using Utils;
using ModelElement = DataDictionary.ModelElement;

namespace GUI.DataDictionaryView.UsageTreeView
{
    public class UsageTreeView : TypedTreeView<IModelElement>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public UsageTreeView()
        {
            SilentSelect = true;
            MouseMove += UsageTreeView_MouseMove;
            LabelEdit = false;
        }

        private void UsageTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            ToolTip toolTip = GuiUtils.MdiWindow.ToolTip;

            TreeNode theNode = GetNodeAt(e.X, e.Y);
            if ((theNode != null))
            {
                if (theNode.ToolTipText != null)
                {
                    if (theNode.ToolTipText != toolTip.GetToolTip(this))
                    {
                        toolTip.SetToolTip(this, theNode.ToolTipText);
                    }
                }
                else
                {
                    toolTip.SetToolTip(this, "");
                }
            }
            else
            {
                toolTip.SetToolTip(this, "");
            }
        }

        /// <summary>
        ///     Indicates that the element is a model element (as opposed to a test)
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private bool IsModel(IModelElement element)
        {
            bool retVal = false;

            IModelElement current = element;
            while (current != null && !retVal)
            {
                retVal = current is NameSpace;
                current = current.Enclosing as IModelElement;
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates that the element belongs to a test
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private bool IsTest(IModelElement element)
        {
            bool retVal = false;

            IModelElement current = element;
            while (current != null && !retVal)
            {
                retVal = current is Frame;
                current = current.Enclosing as IModelElement;
            }

            return retVal;
        }

        private ModelElement _previousModel;

        /// <summary>
        ///     Build the model of this tree view
        /// </summary>
        /// <returns>the root nodes of the tree</returns>
        protected override List<BaseTreeNode> BuildModel()
        {
            List<BaseTreeNode> retVal = new List<BaseTreeNode>();

            ModelElement model = Root as ModelElement;
            if (model != null && model != _previousModel)
            {
                _previousModel = model;

                UsageTreeNode models = new UsageTreeNode("Model", true);
                models.SetImageIndex(false);
                models.SubNodesBuilt = true;
                retVal.Add(models);

                UsageTreeNode tests = new UsageTreeNode("Test", true);
                tests.SetImageIndex(false);
                tests.SubNodesBuilt = true;
                retVal.Add(tests);

                foreach (Usage usage in model.EFSSystem.FindReferences(model))
                {
                    UsageTreeNode current = new UsageTreeNode(usage, true);
                    current.SetImageIndex(false);

                    if (IsModel(usage.User))
                    {
                        models.Nodes.Add(current);
                    }
                    else if (IsTest(usage.User))
                    {
                        tests.Nodes.Add(current);
                    }
                    else
                    {
                        retVal.Add(current);
                    }
                }

                Sort();
            }
            else
            {
                foreach (BaseTreeNode node in Nodes)
                {
                    retVal.Add(node);
                }
            }

            return retVal;
        }
    }
}