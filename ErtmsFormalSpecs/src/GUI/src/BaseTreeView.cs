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
using System.Drawing;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using GUI.Properties;
using Utils;
using ModelElement = Utils.ModelElement;

namespace GUI
{
    public abstract class BaseTreeView : TreeView
    {
        /// <summary>
        ///     The parent form
        /// </summary>
        public IBaseForm ParentForm
        {
            get
            {
                Control parent = Parent;

                while (parent != null && !(parent is IBaseForm))
                {
                    parent = parent.Parent;
                }

                return parent as IBaseForm;
            }
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
        ///     The thread used to synchronize node names with their model
        /// </summary>
        private class ColorSynchronizer : GenericSynchronizationHandler<BaseTreeView>
        {
            /// <summary>
            ///     The last count of messages
            /// </summary>
            private int LastMessageCount { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="instance"></param>
            public ColorSynchronizer(BaseTreeView instance, int cycleTime)
                : base(instance, cycleTime)
            {
                LastMessageCount = 0;
            }

            /// <summary>
            ///     Synchronization
            /// </summary>
            /// <param name="instance"></param>
            public override void HandleSynchronization(BaseTreeView instance)
            {
                if (LastMessageCount != ModelElement.LogCount)
                {
                    LastMessageCount = ModelElement.LogCount;

                    foreach (BaseTreeNode node in instance.Nodes)
                    {
                        if (node.Model is DataDictionary.ModelElement)
                        {
                            Util.UpdateMessageInfo((DataDictionary.ModelElement) node.Model);
                        }
                    }
                    instance.Invoke((MethodInvoker) delegate
                    {
                        instance.SuspendLayout();
                        foreach (BaseTreeNode node in instance.Nodes)
                        {
                            node.UpdateColor();
                        }
                        instance.ResumeLayout(true);
                    });
                }
            }
        }

        /// <summary>
        ///     The thread used to synchronize node names with their model
        /// </summary>
        private class NameSynchronizer : GenericSynchronizationHandler<BaseTreeView>
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="instance"></param>
            public NameSynchronizer(BaseTreeView instance, int cycleTime)
                : base(instance, cycleTime)
            {
            }

            /// <summary>
            ///     Synchronization
            /// </summary>
            /// <param name="instance"></param>
            public override void HandleSynchronization(BaseTreeView instance)
            {
                instance.Invoke((MethodInvoker) delegate
                {
                    instance.SuspendLayout();
                    if (instance.Selected != null)
                    {
                        instance.Selected.UpdateText();
                    }
                    instance.ResumeLayout();
                });
            }
        }

        /// <summary>
        ///     Indicates that synchronization is required
        /// </summary>
        private ColorSynchronizer NodeColorSynchronizer { get; set; }

        private NameSynchronizer NodeNameSynchronizer { get; set; }

        /// <summary>
        ///     Indicates that selection should be taken into account while considering history
        /// </summary>
        protected bool KeepTrackOfSelection { get; set; }

        /// <summary>
        ///     Indicates whether refactoring should occur during drag & drop
        /// </summary>
        protected bool Refactor { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected BaseTreeView()
            : base()
        {
            BeforeSelect += new TreeViewCancelEventHandler(BeforeSelectHandler);
            AfterSelect += new TreeViewEventHandler(AfterSelectHandler);
            DoubleClick += new EventHandler(DoubleClickHandler);
            ItemDrag += new ItemDragEventHandler(ItemDragHandler);
            DragEnter += new DragEventHandler(DragEnterHandler);
            DragDrop += new DragEventHandler(DragDropHandler);
            AllowDrop = true;

            BeforeExpand += new TreeViewCancelEventHandler(BeforeExpandHandler);
            BeforeCollapse += new TreeViewCancelEventHandler(BeforeCollapseHandler);
            KeyUp += new KeyEventHandler(BaseTreeView_KeyUp);
            AfterLabelEdit += new NodeLabelEditEventHandler(LabelEditHandler);
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

            NodeColorSynchronizer = new ColorSynchronizer(this, 300);
            NodeNameSynchronizer = new NameSynchronizer(this, 300);

            KeepTrackOfSelection = true;
            DoubleBuffered = true;

            Selecting = false;
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
        ///     Indicates that an expand all operation is currently being done
        /// </summary>
        private bool ExpandingAll = false;

        /// <summary>
        ///     Handles an expand event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeExpandHandler(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                GUIUtils.MDIWindow.HandlingSelection = true;
                Selected = e.Node as BaseTreeNode;
                if (Selected != null)
                {
                    if (ModifierKeys == Keys.Control && !ExpandingAll && !Selecting)
                    {
                        try
                        {
                            ExpandingAll = true;
                            Selected.ExpandAll();
                        }
                        finally
                        {
                            ExpandingAll = false;
                        }
                    }
                    else
                    {
                        Selected.HandleExpand();
                    }
                }
            }
            finally
            {
                GUIUtils.MDIWindow.HandlingSelection = false;
            }
        }

        /// <summary>
        ///     Handles an collapse event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeCollapseHandler(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                GUIUtils.MDIWindow.HandlingSelection = true;
                Selected = e.Node as BaseTreeNode;
                Selected.HandleCollapse();
            }
            finally
            {
                GUIUtils.MDIWindow.HandlingSelection = false;
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
            Selected.HandleLabelEdit(e.Label);
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

        private const int CTRL = 8;
        private const int ALT = 32;

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
                if (destinationNode != null)
                {
                    if ((e.KeyState & CTRL) != 0)
                    {
                        destinationNode.AcceptCopy(sourceNode);
                    }
                    else if ((e.KeyState & ALT) != 0)
                    {
                        destinationNode.AcceptMove(sourceNode);
                    }
                    else
                    {
                        Compiler compiler = EFSSystem.INSTANCE.Compiler;

                        compiler.Compile_Synchronous(false, true);
                        destinationNode.AcceptDrop(sourceNode);
                        if (Refactor && Settings.Default.AllowRefactor)
                        {
                            compiler.RefactorAndRelocate(sourceNode.Model as DataDictionary.ModelElement);
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
                RefreshNode(node as BaseTreeNode);
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
        ///     The node that is currently selected
        /// </summary>
        private BaseTreeNode currentSelection;

        /// <summary>
        ///     Handler called before another node is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeSelectHandler(object sender, TreeViewCancelEventArgs e)
        {
            if (currentSelection != null)
            {
                currentSelection.BeforeSelectionChange();
            }
        }

        /// <summary>
        ///     Handles a selection event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AfterSelectHandler(object sender, TreeViewEventArgs e)
        {
            Selected = e.Node as BaseTreeNode;
            currentSelection = Selected;
            if (Selected != null)
            {
                Selected.SelectionChanged(true);
                if (KeepTrackOfSelection)
                {
                    GUIUtils.MDIWindow.HandleSelection(Selected);
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
        /// <param name="Model"></param>
        public abstract void SetRoot(IModelElement Model);


        /// <summary>
        ///     Indicates whether the second argument (parent) is a parent of the first argument (element).
        ///     It also returns true then parent==element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private bool IsParent(IModelElement element, IModelElement parent)
        {
            bool retVal;

            IModelElement current = element;
            do
            {
                retVal = current == parent;
                current = EnclosingFinder<IModelElement>.find(current);
            } while (!retVal && current != null);

            return retVal;
        }

        /// <summary>
        ///     Finds the node which references the element provided
        /// </summary>
        /// <param name="node"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        private BaseTreeNode InnerFindNode(BaseTreeNode node, IModelElement element)
        {
            BaseTreeNode retVal = null;

            if (node.Model == element)
            {
                retVal = node;
            }
            else
            {
                // Ensures that the sub nodes have been built before trying to find the corresponding element
                if (!node.SubNodesBuilt)
                {
                    node.BuildSubNodes(false);
                    node.UpdateColor();
                }

                foreach (BaseTreeNode subNode in node.Nodes)
                {
                    if (IsParent(element, subNode.Model))
                    {
                        retVal = InnerFindNode(subNode, element);
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
        /// <returns></returns>
        public BaseTreeNode FindNode(IModelElement model)
        {
            BaseTreeNode retVal = null;

            foreach (BaseTreeNode node in Nodes)
            {
                retVal = InnerFindNode(node, model);
                if (retVal != null)
                {
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates that a programatic selection is currently occuring
        /// </summary>
        private bool Selecting { get; set; }

        /// <summary>
        ///     Selects the node which references the element provided
        /// </summary>
        /// <param name="element"></param>
        /// <param name="getFocus">Indicates whether the focus should be given to the enclosing form</param>
        /// <returns>the selected node</returns>
        public BaseTreeNode Select(IModelElement element, bool getFocus = false)
        {
            BaseTreeNode retVal = null;

            try
            {
                Selecting = true;
                if (element != null)
                {
                    retVal = FindNode(element);
                    if (retVal != null)
                    {
                        Selected = retVal;

                        if (getFocus)
                        {
                            Form form = GUIUtils.EnclosingFinder<Form>.find(this);
                            form.BringToFront();
                        }
                    }
                }
            }
            finally
            {
                Selecting = false;
            }

            return retVal;
        }

        /// <summary>
        ///     Build the model of this tree view
        /// </summary>
        protected abstract void BuildModel();

        /// <summary>
        ///     Indicates that the node contents should be refreshed
        /// </summary>
        public bool RefreshNodeContent { get; private set; }

        /// <summary>
        ///     Refreshes the model of the tree view
        /// </summary>
        public void RefreshModel()
        {
            BaseTreeNode selected = Selected;
            Util.DontNotify(() =>
            {
                try
                {
                    SuspendLayout();
                    RefreshNodeContent = false;

                    BuildModel();
                    if (selected != null)
                    {
                        Select(selected.Model);
                    }
                }
                finally
                {
                    ResumeLayout(true);
                    RefreshNodeContent = true;
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
        /// <returns>the node to select</returns>
        private BaseTreeNode RecursivelySelectNext(IModelElement current, BaseTreeNode node,
            ElementLog.LevelEnum levelEnum, bool considerThisOne)
        {
            BaseTreeNode retVal = null;

            if (current != null)
            {
                if (considerThisOne && (node.Parent == null || node.Model != ((BaseTreeNode) node.Parent).Model))
                {
                    if (node.Model.HasMessage(levelEnum) && node.Model != current)
                    {
                        retVal = node;
                    }
                }

                if (retVal == null)
                {
                    if (!node.SubNodesBuilt)
                    {
                        // Maybe the correspponding subnodes have not yet been built
                        // Do we need to look through these ? 
                        if (levelEnum == ElementLog.LevelEnum.Error &&
                            (current.MessagePathInfo == MessagePathInfoEnum.PathToError ||
                             current.MessagePathInfo == MessagePathInfoEnum.Error))
                        {
                            node.BuildSubNodes(false);
                            node.UpdateColor();
                        }
                        else if (levelEnum == ElementLog.LevelEnum.Warning &&
                                 (current.MessagePathInfo == MessagePathInfoEnum.PathToError ||
                                  current.MessagePathInfo == MessagePathInfoEnum.Error ||
                                  current.MessagePathInfo == MessagePathInfoEnum.Warning ||
                                  current.MessagePathInfo == MessagePathInfoEnum.PathToWarning))
                        {
                            node.BuildSubNodes(false);
                            node.UpdateColor();
                        }
                        else if (levelEnum == ElementLog.LevelEnum.Info &&
                                 current.MessagePathInfo != MessagePathInfoEnum.Nothing)
                        {
                            node.BuildSubNodes(false);
                            node.UpdateColor();
                        }
                    }
                    if (node.Nodes.Count > 0)
                    {
                        foreach (BaseTreeNode subNode in node.Nodes)
                        {
                            retVal = RecursivelySelectNext(current, subNode, levelEnum, true);
                            if (retVal != null)
                            {
                                break;
                            }
                        }
                    }
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
            BaseTreeNode toSelect = null;

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
                Selected = toSelect;
            }
            else
            {
                MessageBox.Show("No more element found", "End of selection");
            }
        }
    }

    public abstract class TypedTreeView<RootType> : BaseTreeView
        where RootType : class, IModelElement
    {
        /// <summary>
        ///     The root of this tree view
        /// </summary>
        private RootType root;

        public RootType Root
        {
            get { return root; }
            set
            {
                root = value;
                if (value != null)
                {
                    RefreshModel();
                }
                else
                {
                    Nodes.Clear();
                }
            }
        }

        /// <summary>
        ///     Sets the root of this tree view
        /// </summary>
        /// <param name="Model"></param>
        public override void SetRoot(IModelElement Model)
        {
            Root = Model as RootType;
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