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
using System.Drawing;
using System.Windows.Forms;
using DataDictionary.Interpreter;
using DataDictionary.Tests;
using DataDictionary.Types;
using Utils;
using ModelElement = DataDictionary.ModelElement;

namespace GUI.UsageView
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

        private ModelElement _previousModel;

        /// <summary>
        /// Finds a node for a folder in the usage view
        /// </summary>
        /// <param name="node"></param>
        /// <param name="folderElement"></param>
        /// <returns></returns>
        public UsageTreeNode FindOrCreateFolderNode(UsageTreeNode node, IModelElement folderElement)
        {
            UsageTreeNode retVal = null;

            foreach (UsageTreeNode subNode in node.Nodes)
            {
                if (subNode.FolderElement == folderElement)
                {
                    retVal = subNode;
                    break;
                }
            }

            if (retVal == null)
            {
                retVal = new UsageTreeNode(folderElement, true);
                node.Nodes.Add(retVal);
            }

            return retVal;
        }

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

                    NameSpace nameSpace = EnclosingFinder<NameSpace>.find(usage.User, true);
                    Frame frame = EnclosingFinder<Frame>.find(usage.User, true);
                    if (nameSpace != null)
                    {
                        List<NameSpace> nameSpaces = new List<NameSpace>();
                        while (nameSpace != null)
                        {
                            nameSpaces.Insert(0, nameSpace);
                            nameSpace = EnclosingFinder<NameSpace>.find(nameSpace);
                        }

                        UsageTreeNode currentTreeNode = models;
                        foreach (NameSpace currentNameSpace in nameSpaces)
                        {
                            currentTreeNode = FindOrCreateFolderNode(currentTreeNode, currentNameSpace);
                        }                  
                        currentTreeNode.Nodes.Add(current);
                    }
                    else if (frame != null)
                    {
                        UsageTreeNode currentNode = FindOrCreateFolderNode(tests, frame);
                        SubSequence subSequence = EnclosingFinder<SubSequence>.find(usage.User, true);
                        if (subSequence != null)
                        {
                            currentNode = FindOrCreateFolderNode(currentNode, subSequence);                            
                        }
                        currentNode.Nodes.Add(current);
                    }
                    else
                    {
                        retVal.Add(current);
                    }
                }

                Sort();
                models.ExpandAll();
                tests.ExpandAll();
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