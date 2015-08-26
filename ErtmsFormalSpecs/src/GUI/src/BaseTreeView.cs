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
using System.Drawing;
using System.Windows.Forms;
using DataDictionary;
using GUI.LongOperations;
using GUI.Properties;
using GUIUtils.LongOperations;
using Utils;
using ModelElement = DataDictionary.ModelElement;
using Util = DataDictionary.Util;

namespace GUI
{
    public abstract class BaseTreeView : TreeView
    {
        /// <summary>
        ///     The parent form
        /// </summary>
        public IBaseForm ParentForm
        {
            get { return GuiUtils.EnclosingFinder<IBaseForm>.Find(this); }
        }

        public static int FileImageIndex;
        public static int ClosedFolderImageIndex;
        public static int ExpandedFolderImageIndex;
        public static int RequirementImageIndex;
        public static int ModelImageIndex;
        public static int TestImageIndex;
        public static int ReadAccessImageIndex;
        public static int WriteAccessImageIndex;
        public static int CallImageIndex;
        public static int TypeImageIndex;
        public static int ParameterImageIndex;
        public static int InterfaceImageIndex;
        public static int RedefinesImageIndex;

        /// <summary>
        ///     Indicates whether refactoring should occur during drag & drop
        /// </summary>
        protected bool Refactor { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected BaseTreeView()
        {
            AfterSelect += AfterSelectHandler;
            DoubleClick += DoubleClickHandler;
            ItemDrag += ItemDragHandler;
            DragEnter += DragEnterHandler;
            DragDrop += DragDropHandler;
            AllowDrop = true;

            BeforeExpand += BeforeExpandHandler;
            BeforeCollapse += BeforeCollapseHandler;
            KeyUp += BaseTreeView_KeyUp;
            AfterLabelEdit += LabelEditHandler;
            LabelEdit = true;
            HideSelection = false;

            ImageList = new ImageList();
            ImageList.Images.Add(Resources.file);
            ImageList.Images.Add(Resources.folder_closed);
            ImageList.Images.Add(Resources.folder_opened);
            ImageList.Images.Add(Resources.req_icon);
            ImageList.Images.Add(Resources.model_icon);
            ImageList.Images.Add(Resources.test_icon);
            ImageList.Images.Add(Resources.read_icon);
            ImageList.Images.Add(Resources.write_icon);
            ImageList.Images.Add(Resources.call_icon);
            ImageList.Images.Add(Resources.type_icon);
            ImageList.Images.Add(Resources.parameter_icon);
            ImageList.Images.Add(Resources.interface_icon);
            ImageList.Images.Add(Resources.redefines_icon);

            ImageIndex = 0;
            FileImageIndex = 0;
            ClosedFolderImageIndex = 1;
            ExpandedFolderImageIndex = 2;
            RequirementImageIndex = 3;
            ModelImageIndex = 4;
            TestImageIndex = 5;
            ReadAccessImageIndex = 6;
            WriteAccessImageIndex = 7;
            CallImageIndex = 8;
            TypeImageIndex = 9;
            ParameterImageIndex = 10;
            InterfaceImageIndex = 11;
            RedefinesImageIndex = 12;

            DoubleBuffered = true;

            Refactor = true;
        }

        private void BaseTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F2:
                    if (Selected != null)
                    {
                        Selected.BeginEdit();
                    }
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        ///     Handles an expand event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeExpandHandler(object sender, TreeViewCancelEventArgs e)
        {
            BaseTreeNode node = e.Node as BaseTreeNode;
            if (node != null)
            {
                node.HandleExpand();
            }
        }

        /// <summary>
        ///     Handles an collapse event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeCollapseHandler(object sender, TreeViewCancelEventArgs e)
        {
            BaseTreeNode node = e.Node as BaseTreeNode;
            if (node != null)
            {
                node.HandleCollapse();
            }
        }

        /// <summary>
        ///     Handles a label edit event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LabelEditHandler(object sender, NodeLabelEditEventArgs e)
        {
            Selected = e.Node as BaseTreeNode;
            if (Selected != null)
            {
                Selected.HandleLabelEdit(e.Label);
            }
        }

        /// <summary>
        ///     Called when the drag & drop operation begins
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemDragHandler(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        /// <summary>
        ///     Called to initiate a drag & drop operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragEnterHandler(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private const int Ctrl = 8;
        private const int Alt = 32;

        /// <summary>
        ///     Called when the drop operation is performed on a node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragDropHandler(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("WindowsForms10PersistentObject", false))
            {
                Point pt = ((TreeView) sender).PointToClient(new Point(e.X, e.Y));
                BaseTreeNode destinationNode = (BaseTreeNode) ((BaseTreeView) sender).GetNodeAt(pt);
                object data = e.Data.GetData("WindowsForms10PersistentObject");
                BaseTreeNode sourceNode = data as BaseTreeNode;
                if (sourceNode != null && destinationNode != null)
                {
                    if ((e.KeyState & Ctrl) != 0)
                    {
                        destinationNode.AcceptCopy(sourceNode);
                    }
                    else if ((e.KeyState & Alt) != 0)
                    {
                        destinationNode.AcceptMove(sourceNode);
                    }
                    else
                    {
                        destinationNode.AcceptDrop(sourceNode);
                        if (Refactor && Settings.Default.AllowRefactor)
                        {
                            RefactorAndRelocateOperation refactorAndRelocate =
                                new RefactorAndRelocateOperation(sourceNode.Model as ModelElement);
                            refactorAndRelocate.ExecuteUsingProgressDialog("Refactoring", false);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Refreshes all nodes of this tree view
        /// </summary>
        public virtual void RefreshNodes()
        {
            foreach (BaseTreeNode node in Nodes)
            {
                RefreshNode(node);
            }
        }

        private void RefreshNode(BaseTreeNode node)
        {
            if (node != null)
            {
                foreach (TreeNode subNode in node.Nodes)
                {
                    RefreshNode(subNode as BaseTreeNode);
                }
                node.RefreshNode();
            }
        }

        /// <summary>
        ///     The selected tree node
        /// </summary>
        public BaseTreeNode Selected
        {
            get { return SelectedNode as BaseTreeNode; }
            set { SelectedNode = value; }
        }

        /// <summary>
        ///     Indicates that the selection should not trigger AfterSelect
        /// </summary>
        public bool SilentSelect { get; set; }

        /// <summary>
        ///     Handles a selection event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AfterSelectHandler(object sender, TreeViewEventArgs e)
        {
            if (!SilentSelect)
            {
                BaseTreeNode node = e.Node as BaseTreeNode;
                if (node != null)
                {
                    node.SelectionHandler();
                }
            }
        }

        /// <summary>
        ///     Handles a double click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DoubleClickHandler(object sender, EventArgs e)
        {
            MouseEventArgs args = e as MouseEventArgs;
            if (args != null)
            {
                Selected = GetNodeAt(new Point(args.X, args.Y)) as BaseTreeNode;
            }

            if (Selected != null)
            {
                Selected.DoubleClickHandler();
            }
        }

        /// <summary>
        ///     Clears messages associated to the elements on the tree view
        /// </summary>
        public void ClearMessages()
        {
            foreach (BaseTreeNode node in Nodes)
            {
                node.ClearMessages();
            }
            RefreshNodes();
        }

        /// <summary>
        ///     Sets the root elements of the tree view (untyped)
        /// </summary>
        /// <param name="model"></param>
        public abstract void SetRoot(IModelElement model);

        /// <summary>
        ///     Finds the node which references the element provided
        /// </summary>
        /// <param name="node"></param>
        /// <param name="element"></param>
        /// <param name="buildSubNodes">Indicates that the sub nodes should be built before trying to select a sub element</param>
        /// <returns></returns>
        private BaseTreeNode InnerFindNode(BaseTreeNode node, IModelElement element, bool buildSubNodes)
        {
            BaseTreeNode retVal = null;

            if (node.Model == element)
            {
                retVal = node;
            }
            else
            {
                // Ensures that the sub nodes have been built before trying to find the corresponding element
                if (buildSubNodes && !node.SubNodesBuilt)
                {
                    node.BuildOrRefreshSubNodes(null);
                }

                foreach (BaseTreeNode subNode in node.Nodes)
                {
                    if (subNode.Model.IsParent(element))
                    {
                        retVal = InnerFindNode(subNode, element, buildSubNodes);
                        if (retVal != null)
                        {
                            break;
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the node which corresponds to the model element provided
        /// </summary>
        /// <param name="model"></param>
        /// <param name="buildSubNodes"></param>
        /// <returns></returns>
        public BaseTreeNode FindNode(IModelElement model, bool buildSubNodes)
        {
            BaseTreeNode retVal = null;

            foreach (BaseTreeNode node in Nodes)
            {
                retVal = InnerFindNode(node, model, buildSubNodes);
                if (retVal != null)
                {
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Selects the node which references the element provided
        /// </summary>
        /// <param name="element"></param>
        /// <param name="getFocus">Indicates whether the focus should be given to the enclosing form</param>
        /// <returns>the selected node</returns>
        public BaseTreeNode Select(IModelElement element, bool getFocus = false)
        {
            BaseTreeNode retVal = null;

            if (element != null)
            {
                retVal = FindNode(element, true);
                if (retVal != null)
                {
                    // ReSharper disable once RedundantCheckBeforeAssignment
                    if (Selected != retVal)
                    {
                        Selected = retVal;
                    }

                    if (getFocus)
                    {
                        Form form = GuiUtils.EnclosingFinder<Form>.Find(this);
                        form.BringToFront();
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Build the model of this tree view
        /// </summary>
        /// <returns>the root nodes of the tree</returns>
        protected abstract List<BaseTreeNode> BuildModel();

        /// <summary>
        ///     Refreshes the model of the tree view
        /// </summary>
        /// <param name="modifiedElement">The element that has been modified</param>
        public void RefreshModel(IModelElement modifiedElement)
        {
            BaseTreeNode selected = Selected;
            Util.DontNotify(() =>
            {
                try
                {
                    SuspendLayout();

                    // Ensure the root nodes are correct
                    List<BaseTreeNode> rootNodes = BuildModel();
                    if (rootNodes.Count != Nodes.Count)
                    {
                        Nodes.Clear();
                        foreach (BaseTreeNode node in rootNodes)
                        {
                            Nodes.Add(node);
                        }
                    }

                    // Refresh the selected node
                    foreach (BaseTreeNode node in Nodes)
                    {
                        if (modifiedElement == null || (node.Model != null && node.Model.IsParent(modifiedElement)))
                        {
                            node.BuildOrRefreshSubNodes(modifiedElement);
                        }
                    }

                    if (selected != null)
                    {
                        Select(selected.Model);
                    }
                }
                finally
                {
                    ResumeLayout(true);
                }
            });
        }

        /// <summary>
        ///     Selects the next node whose error level corresponds to the levelEnum provided
        /// </summary>
        /// <param name="current">the model element that is currently displayed</param>
        /// <param name="node">the node from which the selection process must begin</param>
        /// <param name="levelEnum"></param>
        /// <param name="considerThisOne">Indicates that the current node should be considered by the search</param>
        /// <returns>The node which corresponds to the search criteria, null otherwise</returns>
        private BaseTreeNode RecursivelySelectNext(IModelElement current, BaseTreeNode node,
            ElementLog.LevelEnum levelEnum, bool considerThisOne)
        {
            BaseTreeNode retVal = null;

            if (current != null)
            {
                IModelElement model = node.Model;
                if (considerThisOne && (node.Parent == null || node.Model != ((BaseTreeNode) node.Parent).Model))
                {
                    if (levelEnum == ElementLog.LevelEnum.Error && (model.MessagePathInfo & MessageInfoEnum.Error) != 0)
                    {
                        retVal = node;
                    }
                    else if (levelEnum == ElementLog.LevelEnum.Warning &&
                             (model.MessagePathInfo & MessageInfoEnum.Warning) != 0)
                    {
                        retVal = node;
                    }
                    else if (levelEnum == ElementLog.LevelEnum.Info &&
                             (model.MessagePathInfo & MessageInfoEnum.Info) != 0)
                    {
                        retVal = node;
                    }
                }

                if (retVal == null)
                {
                    if (levelEnum == ElementLog.LevelEnum.Error &&
                        (model.MessagePathInfo & MessageInfoEnum.PathToError) != 0)
                    {
                        retVal = InnerSelectNext(current, node, levelEnum);
                    }
                    else if (levelEnum == ElementLog.LevelEnum.Warning &&
                             (model.MessagePathInfo & MessageInfoEnum.PathToWarning) != 0)
                    {
                        retVal = InnerSelectNext(current, node, levelEnum);
                    }
                    else if (levelEnum == ElementLog.LevelEnum.Info &&
                             (model.MessagePathInfo & MessageInfoEnum.PathToInfo) != 0)
                    {
                        retVal = InnerSelectNext(current, node, levelEnum);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Builds the subnodes if needed, then try to select the next Error/Warning/Info
        /// </summary>
        /// <param name="current">the model element that is currently displayed</param>
        /// <param name="node">the node from which the selection process must begin</param>
        /// <param name="levelEnum"></param>
        /// <returns>The node which corresponds to the search criteria, null otherwise</returns>
        private BaseTreeNode InnerSelectNext(IModelElement current, BaseTreeNode node, ElementLog.LevelEnum levelEnum)
        {
            BaseTreeNode retVal = null;

            if (!node.SubNodesBuilt)
            {
                node.BuildOrRefreshSubNodes(null);
            }

            foreach (BaseTreeNode subNode in node.Nodes)
            {
                retVal = RecursivelySelectNext(current, subNode, levelEnum, true);
                if (retVal != null)
                {
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Selects the next node whose error level corresponds to the levelEnum provided
        /// </summary>
        /// <param name="levelEnum"></param>
        public void SelectNext(ElementLog.LevelEnum levelEnum)
        {
            BaseTreeNode node = Selected;
            BaseTreeNode toSelect;

            if (node != null)
            {
                IModelElement current = node.Model;
                toSelect = RecursivelySelectNext(current, node, levelEnum, false);
                while (toSelect == null && node != null)
                {
                    while (node != null && node.NextNode == null)
                    {
                        node = node.Parent as BaseTreeNode;
                    }

                    if (node != null)
                    {
                        node = node.NextNode as BaseTreeNode;
                        toSelect = RecursivelySelectNext(current, node, levelEnum, true);
                    }
                }
            }
            else
            {
                toSelect = RecursivelySelectNext(null, Nodes[0] as BaseTreeNode, levelEnum, true);
            }

            if (toSelect != null)
            {
                EfsSystem.Instance.Context.SelectElement(toSelect.Model, toSelect, Context.SelectionCriteria.DoubleClick);
            }
            else
            {
                MessageBox.Show(
                    Resources.BaseTreeView_SelectNext_No_more_element_found,
                    Resources.BaseTreeView_SelectNext_End_of_selection);
            }
        }
    }

    public abstract class TypedTreeView<T> : BaseTreeView
        where T : class, IModelElement
    {
        /// <summary>
        ///     The root of this tree view
        /// </summary>
        private T _root;

        public T Root
        {
            get { return _root; }
            set
            {
                Nodes.Clear();
                _root = value;
                if (value != null)
                {
                    RefreshModel(null);
                }
            }
        }

        /// <summary>
        ///     Sets the root of this tree view
        /// </summary>
        /// <param name="model"></param>
        public override void SetRoot(IModelElement model)
        {
            Root = model as T;
        }

        /// <summary>
        ///     Refreshes the tree view
        /// </summary>
        public override void Refresh()
        {
            SuspendLayout();
            RefreshNodes();

            ResumeLayout();
            PerformLayout();

            base.Refresh();
        }
    }
}