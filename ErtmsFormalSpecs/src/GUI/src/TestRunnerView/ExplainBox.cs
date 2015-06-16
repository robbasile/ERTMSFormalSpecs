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
using DataDictionary.Interpreter;
using DataDictionary.Rules;
using DataDictionary.Variables;
using GUI.DataDictionaryView;
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.TestRunnerView
{
    public partial class ExplainBox : DockContent
    {
        /// <summary>
        ///     A node of the tree
        /// </summary>
        private class ExplainTreeNode : TreeNode
        {
            /// <summary>
            ///     The explanation which corresponds to this node
            /// </summary>
            public ExplanationPart Explanation { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="explanation"></param>
            public ExplainTreeNode(ExplanationPart explanation)
            {
                Explanation = explanation;
            }

            /// <summary>
            ///     Provides the explain box in which this node lies
            /// </summary>
            private ExplainBox ExplainBox
            {
                get { return GUIUtils.EnclosingFinder<ExplainBox>.find(TreeView); }
            }

            /// <summary>
            ///     Selects the corresponding model element
            /// </summary>
            public void SelectModel(bool selectModel)
            {
                if (Explanation != null && Explanation.Element != null)
                {
                    if (selectModel)
                    {
                        GUIUtils.MDIWindow.Select(Explanation.Element, true);
                    }
                    ExplainBox.explainRichTextBox.Instance = Explanation;
                    ExplainBox.explainRichTextBox.Text = Explanation.Message;
                }
            }

            /// <summary>
            ///     Updates the node text according to the explanation
            /// </summary>
            public void UpdateText()
            {
                if (Explanation != null)
                {
                    Text = Explanation.Message;
                }
            }

            /// <summary>
            ///     Expands the tree nodes which correspond to the path provided
            /// </summary>
            /// <param name="path"></param>
            /// <param name="index">The current position in the path</param>
            public void ShowPath(List<ExplanationPart> path, int index)
            {
                if (index < path.Count)
                {
                    if (Explanation == path[index])
                    {
                        Expand();
                        foreach (ExplainTreeNode subNode in Nodes)
                        {
                            subNode.ShowPath(path, index + 1);
                        }

                        if (index == path.Count - 1)
                        {
                            TreeView.SelectedNode = this;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     The explanation displayed in the explain box
        /// </summary>
        private ExplanationPart Explanation { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public ExplainBox()
        {
            InitializeComponent();

            explainTreeView.AfterSelect += new TreeViewEventHandler(explainTreeView_AfterSelect);
            explainTreeView.BeforeExpand += new TreeViewCancelEventHandler(explainTreeView_BeforeExpand);
            explainTreeView.DragEnter += new DragEventHandler(explainTreeView_DragEnter);
            explainTreeView.DragDrop += new DragEventHandler(explainTreeView_DragDrop);
            searchTextBox.KeyPress += searchTextBox_KeyPress;
            explainTreeView.ContextMenu = new ContextMenu();
            explainTreeView.ContextMenu.MenuItems.Add("Select", SelectModel);
            explainTreeView.DoubleClick += explainTreeView_DoubleClick;
        }

        /// <summary>
        ///     Handles a double click event on a tree node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void explainTreeView_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs mouseEventArgs = e as MouseEventArgs;
            if (mouseEventArgs != null)
            {
                SelectModelElement(explainTreeView.GetNodeAt(mouseEventArgs.Location) as ExplainTreeNode);
            }
        }

        /// <summary>
        ///     Handles the context menu entry "Select"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectModel(object sender, EventArgs e)
        {
            SelectModelElement(explainTreeView.SelectedNode as ExplainTreeNode);
        }

        /// <summary>
        ///     Selects the model element which corresponds to the explain tree node
        ///     If the current node does not refer to a model element, selects the one from its parent node
        /// </summary>
        /// <param name="node"></param>
        private void SelectModelElement(ExplainTreeNode node)
        {
            while (node != null)
            {
                if (node.Explanation.Element != null)
                {
                    GUIUtils.MDIWindow.Select(node.Explanation.Element);
                    node = null;
                }
                else
                {
                    node = node.Parent as ExplainTreeNode;
                }
            }
        }

        private void searchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                ExpandAndShowVariable(new VariableSelector(searchTextBox.Text, caseSensitiveCheckBox.Checked));
            }
        }

        private void explainTreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void explainTreeView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("WindowsForms10PersistentObject", false))
            {
                object data = e.Data.GetData("WindowsForms10PersistentObject");
                BaseTreeNode sourceNode = data as BaseTreeNode;
                if (sourceNode != null)
                {
                    VariableTreeNode variableTreeNode = sourceNode as VariableTreeNode;
                    if (variableTreeNode != null)
                    {
                        ExpandAndShowVariable(new VariableSelector(variableTreeNode.Item));
                    }
                }
            }
        }

        private class VariableSelector
        {
            /// <summary>
            ///     Part of the variable name
            /// </summary>
            private string VariablePartName { get; set; }

            /// <summary>
            ///     Performs a case sensitive search
            /// </summary>
            private bool CaseSensitive { get; set; }

            /// <summary>
            ///     The variable to find
            /// </summary>
            private IVariable Variable { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="variable"></param>
            public VariableSelector(IVariable variable)
            {
                Variable = variable;
                VariablePartName = null;
            }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="variablePartName"></param>
            /// <param name="caseSensitive"></param>
            public VariableSelector(string variablePartName, bool caseSensitive)
            {
                Variable = null;
                VariablePartName = variablePartName;
                CaseSensitive = caseSensitive;
                if (!CaseSensitive)
                {
                    VariablePartName = VariablePartName.ToUpper();
                }
            }

            /// <summary>
            ///     Indicates whether the string matches using the parameters provided in this VariableSelector
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            private bool MatchName(string text)
            {
                bool retVal = false;

                if (!CaseSensitive)
                {
                    text = text.ToUpper();
                }

                retVal = text.Contains(VariablePartName);

                return retVal;
            }

            /// <summary>
            ///     Indicates whether the explanation part is related to the variable
            /// </summary>
            /// <param name="explanationPart"></param>
            /// <returns></returns>
            public bool Match(ExplanationPart explanationPart)
            {
                bool retVal = false;

                Change change = explanationPart.Change;
                if (change != null)
                {
                    if (Variable != null)
                    {
                        retVal = retVal || change.ImpactVariable(Variable);
                    }
                    if (VariablePartName != null)
                    {
                        retVal = retVal || (change.Variable != null && MatchName(change.Variable.FullName));
                    }
                }

                Expression expression = explanationPart.LeftPart as Expression;
                if (expression != null)
                {
                    if (VariablePartName != null)
                    {
                        retVal = retVal || MatchName(expression.FullName);
                    }
                }

                ModelElement element = explanationPart.Element;
                if (element != null)
                {
                    if (VariablePartName != null)
                    {
                        retVal = retVal || MatchName(element.FullName);
                    }
                }

                return retVal;
            }
        }


        /// <summary>
        ///     Inner primitive to expand nodes, based on a tree node
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="variableSelector"></param>
        /// <param name="path"></param>
        private void InnerExpandAndShowVariable(ExplanationPart explanation, VariableSelector variableSelector,
            List<ExplanationPart> path)
        {
            path.Add(explanation);
            if (variableSelector.Match(explanation))
            {
                foreach (ExplainTreeNode node in explainTreeView.Nodes)
                {
                    node.ShowPath(path, 0);
                }
            }

            foreach (ExplanationPart subExplanation in explanation.SubExplanations)
            {
                InnerExpandAndShowVariable(subExplanation, variableSelector, path);
            }
            path.Remove(explanation);
        }

        /// <summary>
        ///     Expands all pathes which lead to the selected variable
        /// </summary>
        /// <param name="variableSelector"></param>
        private void ExpandAndShowVariable(VariableSelector variableSelector)
        {
            // Build the complete tree
            explainTreeView.SuspendLayout();
            explainTreeView.CollapseAll();

            List<ExplanationPart> path = new List<ExplanationPart>();
            InnerExpandAndShowVariable(Explanation, variableSelector, path);

            explainTreeView.ResumeLayout();
            explainTreeView.Focus();
        }

        private void explainTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            ExplainTreeNode node = e.Node as ExplainTreeNode;

            if (node != null)
            {
                foreach (ExplainTreeNode subNode in node.Nodes)
                {
                    subNode.Nodes.Clear();
                    innerSetExplanation(subNode.Explanation, subNode, 1);
                }

                foreach (ExplainTreeNode subNode in node.Nodes)
                {
                    subNode.UpdateText();
                }
            }
        }

        /// <summary>
        ///     Sets the node, and its subnode according to the content of the explanation
        /// </summary>
        /// <param name="part"></param>
        /// <param name="node"></param>
        /// <param param name="level">the level in which the explanation is inserted</param>
        private void innerSetExplanation(ExplanationPart part, ExplainTreeNode node, int level)
        {
            if (part != null)
            {
                foreach (ExplanationPart subPart in part.SubExplanations)
                {
                    int nextLevel = level;
                    if (level >= 0)
                    {
                        nextLevel += 1;
                    }

                    if (level < 2)
                    {
                        ExplainTreeNode subNode = new ExplainTreeNode(subPart);
                        innerSetExplanation(subPart, subNode, nextLevel);
                        node.Nodes.Add(subNode);
                    }
                }
            }
        }

        /// <summary>
        ///     Sets the explanation for this explain box
        /// </summary>
        /// <param name="explanation"></param>
        public void setExplanation(ExplanationPart explanation)
        {
            Explanation = explanation;

            explainTreeView.Nodes.Clear();
            if (explanation != null)
            {
                ExplainTreeNode node = new ExplainTreeNode(explanation);
                node.UpdateText();
                innerSetExplanation(explanation, node, 0);
                explainTreeView.Nodes.Add(node);
            }
        }

        /// <summary>
        ///     Handles the selection of an element of the treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void explainTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ExplainTreeNode node = explainTreeView.SelectedNode as ExplainTreeNode;
            if (node != null)
            {
                node.SelectModel(ModifierKeys == Keys.Control);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExpandAndShowVariable(new VariableSelector(searchTextBox.Text, caseSensitiveCheckBox.Checked));
        }
    }
}