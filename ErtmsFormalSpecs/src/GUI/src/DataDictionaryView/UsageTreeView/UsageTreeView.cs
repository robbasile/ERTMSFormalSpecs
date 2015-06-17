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
        private UsageTreeNode __tests = null;

        private UsageTreeNode TestNode
        {
            get
            {
                if (__tests == null)
                {
                    __tests = new UsageTreeNode("Test", true);
                    __tests.setImageIndex(false);
                    Nodes.Add(__tests);
                }

                return __tests;
            }
            set { __tests = value; }
        }

        private UsageTreeNode __models = null;

        private UsageTreeNode ModelNode
        {
            get
            {
                if (__models == null)
                {
                    __models = new UsageTreeNode("Model", true);
                    __models.setImageIndex(false);
                    Nodes.Add(__models);
                }

                return __models;
            }
            set { __models = value; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public UsageTreeView()
            : base()
        {
            KeepTrackOfSelection = false;
            MouseMove += new MouseEventHandler(UsageTreeView_MouseMove);
        }

        private void UsageTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            ToolTip toolTip = GUIUtils.MDIWindow.ToolTip;

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

        private ModelElement previousModel = null;

        protected override void BuildModel()
        {
            ModelElement model = Root as ModelElement;
            if (model != null && model != previousModel)
            {
                previousModel = model;

                Nodes.Clear();
                ModelNode = null;
                TestNode = null;
                foreach (Usage usage in model.EFSSystem.FindReferences(model))
                {
                    UsageTreeNode current = new UsageTreeNode(usage, true);
                    current.setImageIndex(false);

                    if (IsModel(usage.User))
                    {
                        ModelNode.Nodes.Add(current);
                    }
                    else if (IsTest(usage.User))
                    {
                        TestNode.Nodes.Add(current);
                    }
                    else
                    {
                        Nodes.Add(current);
                    }
                }

                Sort();
                if (__models != null)
                {
                    ModelNode.ExpandAll();
                }
            }
        }
    }
}