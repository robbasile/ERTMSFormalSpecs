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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Generated;
using DataDictionary.Types;
using GUI.Converters;
using GUI.DictionarySelector;
using GUI.LongOperations;
using GUI.Properties;
using Utils;
using XmlBooster;
using Chapter = DataDictionary.Specification.Chapter;
using Frame = DataDictionary.Tests.Frame;
using ModelElement = DataDictionary.ModelElement;
using ReqRef = DataDictionary.ReqRef;
using ReqRelated = DataDictionary.Generated.ReqRelated;
using ShortcutDictionary = DataDictionary.Shortcuts.ShortcutDictionary;
using Specification = DataDictionary.Specification.Specification;
using Step = DataDictionary.Tests.Step;
using SubSequence = DataDictionary.Tests.SubSequence;
using TestCase = DataDictionary.Tests.TestCase;

namespace GUI
{
    /// <summary>
    ///     The base class for all tree nodes
    /// </summary>
    public class BaseTreeNode : TreeNode, IComparable<BaseTreeNode>
    {
        /// <summary>
        ///     The editor for this tree node
        /// </summary>
        public class BaseEditor
        {
            /// <summary>
            ///     The model element currently edited
            /// </summary>
            [Browsable(false)]
            public IModelElement Model { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="model"></param>
            protected BaseEditor()
            {
            }
        }

        /// <summary>
        ///     The editor used to edit the node contents
        /// </summary>
        public BaseEditor NodeEditor { get; set; }

        /// <summary>
        ///     The fixed node name
        /// </summary>
        private string defaultName;

        private string DefaultName
        {
            get { return defaultName; }
            set { defaultName = value; }
        }

        /// <summary>
        ///     The model represented by this node
        /// </summary>
        public IModelElement Model { get; set; }

        /// <summary>
        ///     Provides the base tree view which holds this node
        /// </summary>
        public BaseTreeView BaseTreeView
        {
            get { return TreeView as BaseTreeView; }
        }

        /// <summary>
        ///     Provides the base form which holds this node
        /// </summary>
        public IBaseForm BaseForm
        {
            get
            {
                IBaseForm retVal = null;

                BaseTreeView treeView = BaseTreeView;
                if (treeView != null)
                {
                    retVal = treeView.ParentForm;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public BaseTreeNode(IModelElement value, string name = null, bool isFolder = false)
            : base(name)
        {
            Model = value;

            if (name != null)
            {
                DefaultName = name;
            }

            setImageIndex(isFolder);
            RefreshNode();
        }

        /// <summary>
        ///     Indicates that the subNodes have already been built, hence, does not require to build its contents anymore
        /// </summary>
        public bool SubNodesBuilt = false;

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public virtual void BuildSubNodes(bool buildSubNodes)
        {
            Nodes.Clear();
            SubNodesBuilt = true;
        }

        /// <summary>
        ///     Changes the image index
        /// </summary>
        /// <param name="value"></param>
        protected void ChangeImageIndex(int value)
        {
            ImageIndex = value;
            SelectedImageIndex = value;
        }

        /// <summary>
        ///     Sets the image index for this node
        /// </summary>
        /// <param name="isFolder">Indicates whether this item represents a folder</param>
        public virtual void setImageIndex(bool isFolder)
        {
            if (ImageIndex == -1)
            {
                // Image index not yet selected
                ChangeImageIndex(BaseTreeView.ModelImageIndex);

                if (isFolder)
                {
                    ChangeImageIndex(BaseTreeView.ClosedFolderImageIndex);
                }
                else
                {
                    IModelElement element = Model;
                    while (element != null && ImageIndex == BaseTreeView.ModelImageIndex)
                    {
                        element = element.Enclosing as IModelElement;
                        if (element is Frame
                            || element is SubSequence
                            || element is TestCase
                            || element is Step)
                        {
                            ChangeImageIndex(BaseTreeView.TestImageIndex);
                        }

                        if (element is Specification
                            || element is Chapter
                            || element is DataDictionary.Specification.Paragraph)
                        {
                            ChangeImageIndex(BaseTreeView.RequirementImageIndex);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Called before the selection changes
        /// </summary>
        public virtual void BeforeSelectionChange()
        {
        }

        /// <summary>
        ///     Handles a selection change event
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public virtual void SelectionChanged(bool displayStatistics)
        {
            if (Model != null && BaseTreeView != null && BaseTreeView.RefreshNodeContent)
            {
                IBaseForm baseForm = BaseForm;
                if (baseForm != null)
                {
                    RefreshViewAccordingToModel(baseForm, false);
                }
            }

            if (displayStatistics && !isShortCut())
            {
                GUIUtils.MDIWindow.SetCoverageStatus(EFSSystem.INSTANCE);
            }

            ModelElement modelElement = Model as ModelElement;
            if (modelElement != null && GUIUtils.MDIWindow.HistoryWindow != null)
            {
                if (!isShortCut())
                {
                    GUIUtils.MDIWindow.HistoryWindow.Model = modelElement;
                }
            }
        }

        /// <summary>
        ///     Indicates that the model is related to a shortcut
        /// </summary>
        /// <returns></returns>
        private bool isShortCut()
        {
            return EnclosingFinder<ShortcutDictionary>.find(Model) != null
                   || (Model is ShortcutDictionary);
        }

        /// <summary>
        ///     Indicates whether the explain box should be displayed
        /// </summary>
        /// <returns></returns>
        private bool ShouldExplain()
        {
            bool retVal = (Model is IDefaultValueElement) || !(Model is IExpressionable);

            return retVal;
        }

        /// <summary>
        ///     Refreshes the view according to the model
        /// </summary>
        /// <param name="baseForm"></param>
        /// <param name="ignoreFocused"></param>
        public void RefreshViewAccordingToModel(IBaseForm baseForm, bool ignoreFocused)
        {
            if (baseForm.subTreeView != null)
            {
                baseForm.subTreeView.SetRoot(Model);
            }

            // By default, the explain text box is visible
            if (baseForm.ExplainTextBox != null && ShouldExplain())
            {
                if (!(baseForm.ExplainTextBox.ContainsFocus && ignoreFocused))
                {
                    baseForm.ExplainTextBox.SetModel(Model);
                    if (!baseForm.ExplainTextBox.Visible)
                    {
                        baseForm.ExplainTextBox.Visible = true;
                        if (baseForm.ExpressionEditorTextBox != null)
                        {
                            baseForm.ExpressionEditorTextBox.Visible = false;
                        }
                    }
                }
            }

            if (baseForm.RequirementsTextBox != null && !ignoreFocused)
            {
                string requirements = "";

                ReqRef reqRef = Model as ReqRef;
                if (reqRef != null && reqRef.Paragraph != null)
                {
                    requirements = reqRef.RequirementDescription();
                }
                else
                {
                    DataDictionary.ReqRelated reqRelated = EnclosingFinder<DataDictionary.ReqRelated>.find(Model, true);
                    if (reqRelated != null)
                    {
                        requirements = reqRelated.RequirementDescription();
                    }
                }

                baseForm.RequirementsTextBox.Text = requirements;
            }

            // Display the expression editor instead of the explain text box when the element can hold an expression
            if (baseForm.ExpressionEditorTextBox != null)
            {
                if (!(baseForm.ExpressionEditorTextBox.ContainsFocus && ignoreFocused))
                {
                    IExpressionable expressionable = Model as IExpressionable;
                    if (expressionable != null && !ShouldExplain())
                    {
                        baseForm.ExpressionEditorTextBox.Instance = Model as ModelElement;
                        baseForm.ExpressionEditorTextBox.Text = expressionable.ExpressionText;
                        if (!baseForm.ExpressionEditorTextBox.Visible)
                        {
                            baseForm.ExpressionEditorTextBox.Visible = true;
                            baseForm.ExplainTextBox.Visible = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Handles a double click event on this tree node
        /// </summary>
        public virtual void DoubleClickHandler()
        {
            // By default, nothing to do
        }

        /// <summary>
        ///     Compares two tree nodes, used by the sort
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(BaseTreeNode other)
        {
            if (Model != null && other.Model != null)
            {
                return Model.CompareTo(other.Model);
            }
            else
            {
                return Text.CompareTo(other.Text);
            }
        }

        /// <summary>
        ///     Updates the node color according to the associated messages
        /// </summary>
        public virtual void UpdateColor()
        {
            Color color = ComputedColor;

            if (color != ForeColor)
            {
                ForeColor = color;
            }

            foreach (BaseTreeNode node in Nodes)
            {
                node.UpdateColor();
            }
        }

        /// <summary>
        ///     The colors used to display things
        /// </summary>
        private Color ERROR_COLOR = Color.Red;

        private Color PATH_TO_ERROR_COLOR = Color.Orange;
        private Color WARNING_COLOR = Color.Brown;
        private Color PATH_TO_WARNING_COLOR = Color.LightCoral;
        private Color INFO_COLOR = Color.Blue;
        private Color PATH_TO_INFO_COLOR = Color.LightBlue;
        private Color NOTHING_COLOR = Color.Black;

        /// <summary>
        ///     Provides the color according to the info status
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected Color ColorBasedOnInfo(MessagePathInfoEnum info)
        {
            Color retVal = NOTHING_COLOR;

            switch (info)
            {
                case MessagePathInfoEnum.Error:
                    retVal = ERROR_COLOR;
                    break;

                case MessagePathInfoEnum.PathToError:
                    retVal = PATH_TO_ERROR_COLOR;
                    break;

                case MessagePathInfoEnum.Warning:
                    retVal = WARNING_COLOR;
                    break;

                case MessagePathInfoEnum.PathToWarning:
                    retVal = PATH_TO_WARNING_COLOR;
                    break;

                case MessagePathInfoEnum.Info:
                    retVal = INFO_COLOR;
                    break;

                case MessagePathInfoEnum.PathToInfo:
                    retVal = PATH_TO_INFO_COLOR;
                    break;

                case MessagePathInfoEnum.Nothing:
                case MessagePathInfoEnum.NotComputed:
                    retVal = NOTHING_COLOR;
                    break;
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the path to a message info
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private MessagePathInfoEnum PathTo(MessagePathInfoEnum info)
        {
            MessagePathInfoEnum retVal = info;

            if (info == MessagePathInfoEnum.Error)
            {
                retVal = MessagePathInfoEnum.PathToError;
            }
            else if (info == MessagePathInfoEnum.Warning)
            {
                retVal = MessagePathInfoEnum.PathToWarning;
            }
            else if (info == MessagePathInfoEnum.Info)
            {
                retVal = MessagePathInfoEnum.PathToInfo;
            }

            return retVal;
        }

        /// <summary>
        ///     Combines two colors
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private MessagePathInfoEnum CombineInfo(MessagePathInfoEnum info1, MessagePathInfoEnum info2)
        {
            MessagePathInfoEnum retVal;

            if (info1 < info2)
            {
                retVal = info2;
            }
            else
            {
                retVal = info1;
            }

            return retVal;
        }

        /// <summary>
        ///     Computes this node's color based on its sub nodes
        /// </summary>
        /// <returns></returns>
        protected Color ComputeColorBasedOnItsSubNodes()
        {
            if (!SubNodesBuilt)
            {
                BuildSubNodes(false);
                UpdateColor();
            }

            MessagePathInfoEnum retVal = MessagePathInfoEnum.Nothing;
            foreach (BaseTreeNode subNode in Nodes)
            {
                retVal = CombineInfo(retVal, PathTo(subNode.Model.MessagePathInfo));
            }

            return ColorBasedOnInfo(retVal);
        }

        /// <summary>
        ///     Provides the computed color
        /// </summary>
        public virtual Color ComputedColor
        {
            get { return Color.Black; }
        }

        /// <summary>
        ///     Updates the node name text according to the modelized item
        /// </summary>
        public virtual void UpdateText()
        {
            string name = "";
            if (DefaultName != null)
            {
                name = DefaultName;
            }
            else
            {
                if (Model != null)
                {
                    name = Model.Name;
                }
            }
            if (Text != name && !Utils.Utils.isEmpty(name))
            {
                Text = name;
            }
        }

        /// <summary>
        ///     Deletes the item modelised by this tree node
        /// </summary>
        public virtual void Delete()
        {
            BaseTreeNode parent = Parent as BaseTreeNode;
            if ((parent != null) && (parent.Nodes != null))
            {
                parent.Nodes.Remove(this);
                Model.Delete();

                if (Model is DataDictionary.ReqRelated)
                {
                    DataDictionary.ReqRelated reqRelated = (DataDictionary.ReqRelated) Model;
                    reqRelated.setVerified(false);
                }

                ControllersManager.BaseModelElementController.alertChange(null, null);
            }
            else
            {
                Model.Delete();
            }
        }

        /// <summary>
        ///     Deletes the selected item
        /// </summary>
        public virtual void DeleteHandler(object sender, EventArgs args)
        {
            Delete();
        }

        /// <summary>
        ///     Marks all model elements as implemented
        /// </summary>
        private class MarkAsImplementedVisitor : Visitor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public MarkAsImplementedVisitor(IModelElement element)
            {
                if (element is ModelElement)
                {
                    visit((ModelElement) element);
                    dispatch((ModelElement) element);
                }
            }

            /// <summary>
            ///     Marks all req related as implemented
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(ReqRelated obj, bool visitSubNodes)
            {
                obj.setImplemented(true);

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            ///     Marks all req related as implemented
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Paragraph obj, bool visitSubNodes)
            {
                obj.setImplementationStatus(acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented);

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Checks the node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Check(object sender, EventArgs e)
        {
            try
            {
                GUIUtils.MDIWindow.ClearMarks();
                ModelElement modelElement = Model as ModelElement;
                if (modelElement != null)
                {
                    RuleCheckerVisitor visitor = new RuleCheckerVisitor(modelElement.Dictionary);

                    visitor.visit(modelElement, true);
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show("Exception raised", excp.Message);
            }
        }

        /// <summary>
        ///     Recursively marks all model elemeSnts as implemented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MarkAsImplemented(object sender, EventArgs e)
        {
            MarkAsImplementedVisitor visitor = new MarkAsImplementedVisitor(Model);
        }

        /// <summary>
        ///     Marks all model elements as verified
        /// </summary>
        private class MarkAsVerifiedVisitor : Visitor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public MarkAsVerifiedVisitor(IModelElement element)
            {
                if (element is ModelElement)
                {
                    visit((ModelElement) element);
                    dispatch((ModelElement) element);
                }
            }

            /// <summary>
            ///     Marks all req related as implemented
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(ReqRelated obj, bool visitSubNodes)
            {
                obj.setVerified(true);

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            ///     Marks all req related as implemented
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Paragraph obj, bool visitSubNodes)
            {
                obj.setReviewed(true);

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Recursively marks all model elements as verified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MarkAsVerified(object sender, EventArgs e)
        {
            MarkAsVerifiedVisitor visitor = new MarkAsVerifiedVisitor(Model);
        }

        /// <summary>
        ///     Recursively marks all model elements as verified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RefreshNodeHandler(object sender, EventArgs e)
        {
            RefreshNode();
        }

        /// <summary>
        ///     Launches label editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void LabelEditHandler(object sender, EventArgs args)
        {
            BeginEdit();
        }

        /// <summary>
        ///     Selects this element to display its history in the history view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ShowHistoryHandler(object sender, EventArgs args)
        {
            if (GUIUtils.MDIWindow.HistoryWindow != null)
            {
                GUIUtils.MDIWindow.HistoryWindow.Model = Model;
            }
        }

        /// <summary>
        ///     Provides the menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected virtual List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Rename", new EventHandler(LabelEditHandler)));
            retVal.Add(new MenuItem("-"));
            MenuItem newItem = new MenuItem("Recursive actions...");
            newItem.MenuItems.Add(new MenuItem("Mark as implemented", new EventHandler(MarkAsImplemented)));
            newItem.MenuItems.Add(new MenuItem("Mark as verified", new EventHandler(MarkAsVerified)));
            retVal.Add(newItem);
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Show history", new EventHandler(ShowHistoryHandler)));
            retVal.Add(new MenuItem("Check", new EventHandler(Check)));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Refresh", new EventHandler(RefreshNodeHandler)));

            return retVal;
        }

        /// <summary>
        ///     Provides the context menu for this item
        /// </summary>
        public override ContextMenu ContextMenu
        {
            get { return new ContextMenu(GetMenuItems().ToArray()); }
        }

        /// <summary>
        ///     Provides the sub node whose text matches the string provided as parameter
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public BaseTreeNode findSubNode(string text)
        {
            BaseTreeNode retVal = null;

            foreach (TreeNode node in Nodes)
            {
                if (node.Text.CompareTo(text) == 0)
                {
                    retVal = node as BaseTreeNode;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     refreshes the node text and color
        /// </summary>
        public virtual void RefreshNode()
        {
            UpdateText();
        }

        /// <summary>
        ///     Clears messages for all nodes of the system
        /// </summary>
        public void ClearMessages()
        {
            EFSSystem.INSTANCE.ClearMessages();
        }

        /// <summary>
        ///     Sort the sub nodes of this node
        /// </summary>
        public virtual void SortSubNodes()
        {
            List<BaseTreeNode> subNodes = new List<BaseTreeNode>();

            foreach (BaseTreeNode node in Nodes)
            {
                subNodes.Add(node);
            }
            subNodes.Sort();

            Nodes.Clear();
            foreach (BaseTreeNode node in subNodes)
            {
                Nodes.Add(node);
            }
        }

        /// <summary>
        ///     Accepts the drop of a base tree node on this node
        /// </summary>
        /// <param name="SourceNode"></param>
        public virtual void AcceptDrop(BaseTreeNode SourceNode)
        {
        }

        /// <summary>
        ///     Generates new GUID for the element
        /// </summary>
        private class RegererateGuidVisitor : Visitor
        {
            /// <summary>
            ///     Ensures that all elements have a new Guid
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(BaseModelElement obj, bool visitSubNodes)
            {
                ModelElement element = (ModelElement) obj;

                // Side effect : creates a new Guid if it is empty
                element.setGuid(null);
                string guid = element.Guid;

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Accepts the drop of a base tree node on this node
        /// </summary>
        /// <param name="SourceNode"></param>
        public virtual void AcceptCopy(BaseTreeNode SourceNode)
        {
            XmlBBase xmlBBase = SourceNode.Model as XmlBBase;
            if (xmlBBase != null)
            {
                string data = xmlBBase.ToXMLString();
                XmlBStringContext ctxt = new XmlBStringContext(data);
                try
                {
                    ModelElement copy = acceptor.accept(ctxt) as ModelElement;
                    RegererateGuidVisitor visitor = new RegererateGuidVisitor();
                    visitor.visit(copy, true);

                    Model.AddModelElement(copy);
                    ArrayList targetCollection = copy.EnclosingCollection;
                    copy.Delete();
                    INamable namable = copy as INamable;
                    if (namable != null && targetCollection != null)
                    {
                        int previousIndex = -1;
                        int index = 0;
                        while (previousIndex != index)
                        {
                            previousIndex = index;
                            foreach (INamable other in targetCollection)
                            {
                                if (index > 0)
                                {
                                    if (other.Name.Equals(namable.Name + "_" + index))
                                    {
                                        index += 1;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (other.Name.Equals(namable.Name))
                                    {
                                        index += 1;
                                        break;
                                    }
                                }
                            }
                        }

                        // Renaming is mandatory
                        if (index > 0)
                        {
                            namable.Name = namable.Name + "_" + index;
                        }
                    }

                    Model.AddModelElement(copy);
                    Nodes.Clear();
                    BuildSubNodes(true);
                    UpdateColor();
                }
                catch (Exception)
                {
                    MessageBox.Show("Cannot copy element\n" + data);
                }
            }
        }

        /// <summary>
        ///     Accepts the move of a base tree node on this node
        /// </summary>
        /// <param name="SourceNode"></param>
        public virtual void AcceptMove(BaseTreeNode SourceNode)
        {
            ArrayList SourceCollection = SourceNode.Model.EnclosingCollection;
            ArrayList ThisCollection = Model.EnclosingCollection;

            if (ThisCollection != null && SourceCollection == ThisCollection)
            {
                // This is a reordering
                int sourceIndex = ThisCollection.IndexOf(SourceNode.Model);
                int thisIndex = ThisCollection.IndexOf(Model);
                if (thisIndex >= 0 && thisIndex >= 0 && thisIndex != sourceIndex)
                {
                    ThisCollection.Remove(SourceNode.Model);
                    thisIndex = ThisCollection.IndexOf(Model);
                    ThisCollection.Insert(thisIndex, SourceNode.Model);

                    BaseTreeNode parentNode = Parent as BaseTreeNode;
                    if (parentNode != null)
                    {
                        parentNode.Nodes.Clear();
                        parentNode.BuildSubNodes(true);
                        parentNode.UpdateColor();
                    }
                    else
                    {
                        GUIUtils.MDIWindow.RefreshModel();
                    }
                }
            }
        }

        /// <summary>
        ///     Called when an expand event is performed in this tree node
        /// </summary>
        public virtual void HandleExpand()
        {
            if (ImageIndex == BaseTreeView.ClosedFolderImageIndex)
            {
                ChangeImageIndex(BaseTreeView.ExpandedFolderImageIndex);
            }
        }

        /// <summary>
        ///     Called when a collapse event is performed in this tree node
        /// </summary>
        public virtual void HandleCollapse()
        {
            if (ImageIndex == BaseTreeView.ExpandedFolderImageIndex)
            {
                ChangeImageIndex(BaseTreeView.ClosedFolderImageIndex);
            }
        }

        /// <summary>
        ///     Called when a label edit event is performed in this tree node
        /// </summary>
        public void HandleLabelEdit(string newLabel)
        {
            if (!string.IsNullOrEmpty(newLabel))
            {
                if (Model.Name != newLabel)
                {
                    if (Settings.Default.AllowRefactor)
                    {
                        RefactorOperation refactorOperation = new RefactorOperation(EFSSystem.INSTANCE, Model as ModelElement, newLabel);
                        refactorOperation.ExecuteUsingProgressDialog("Refactoring", false);
                    }
                    else
                    {
                        Model.Name = newLabel;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     A tree node which hold a reference to a data item.
    ///     This item can be edited by a PropertyGrid
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ModelElementTreeNode<T> : BaseTreeNode
        where T : class, IModelElement
    {
        /// <summary>
        ///     An editor for an item. It is the responsibility of this class to implement attributes
        ///     for the elements to be edited.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class Editor : BaseEditor
        {
            /// <summary>
            ///     The item that is edited.
            /// </summary>
            private T item;

            [Browsable(false)]
            public T Item
            {
                get { return item; }
                set
                {
                    item = value;
                    Model = value;
                    UpdateActivation();
                }
            }

            /// <summary>
            ///     The node that holds the item.
            /// </summary>
            private ModelElementTreeNode<T> node;

            internal ModelElementTreeNode<T> Node
            {
                get { return node; }
                set { node = value; }
            }

            public void RefreshNode()
            {
                Node.RefreshNode();
            }

            public void RefreshTree()
            {
                Node.BaseTreeView.Refresh();
            }

            /// <summary>
            ///     Constructor
            /// </summary>
            protected Editor()
                : base()
            {
            }

            /// <summary>
            ///     Updates the field activation according to the displayed data
            /// </summary>
            protected virtual void UpdateActivation()
            {
            }

            /// <summary>
            ///     Updates the activation of a single field
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            protected void UpdateFieldActivation(string name, bool value)
            {
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())[name];
                ReadOnlyAttribute attribute = (ReadOnlyAttribute) descriptor.Attributes[typeof (ReadOnlyAttribute)];
                FieldInfo fieldToChange = attribute.GetType()
                    .GetField("isReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
                fieldToChange.SetValue(attribute, value);
            }
        }

        /// <summary>
        ///     The element that is represented by this tree node
        /// </summary>
        public T Item { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item">The element to be represented by this tree node</param>
        /// <param name="buildSubNodes">Indicates that subnodes should also be built</param>
        /// <param name="name">The display name of the node</param>
        /// <param name="isFolder">Indicates whether this node is a folder</param>
        protected ModelElementTreeNode(T item, bool buildSubNodes, string name = null, bool isFolder = false)
            : base(item, name, isFolder)
        {
            Item = item;
            BaseBuildSubNodes(buildSubNodes);
        }

        /// <summary>
        ///     Provides the dictionary on which operation should be performed
        /// </summary>
        /// <returns></returns>
        public DataDictionary.Dictionary GetPatchDictionary()
        {
            DataDictionary.Dictionary retVal = null;

            MainWindow mainWindow = GUIUtils.MDIWindow;
            EFSSystem efsSystem = mainWindow.EFSSystem;
            if (efsSystem != null)
            {
                ModelElement modelElement = Item as ModelElement;
                if (modelElement != null)
                {
                    int updates = 0;
                    foreach (DataDictionary.Dictionary dict in efsSystem.Dictionaries)
                    {
                        if (modelElement.Dictionary.IsUpdatedBy(dict))
                        {
                            // Set retVal to the update in case it is the only one for the base dictionary
                            retVal = dict;
                            updates++;
                        }
                    }

                    if (updates > 1)
                    {
                        // if there are 0 or 1 possible updates, it will already have the correct value
                        // if there are more, choose the update from a list of possibilities
                        DictionarySelector.DictionarySelector dictionarySelector =
                            new DictionarySelector.DictionarySelector(efsSystem, FilterOptions.Updates, modelElement.Dictionary);
                        dictionarySelector.ShowDictionaries(mainWindow);

                        if (dictionarySelector.Selected != null)
                        {
                            retVal = dictionarySelector.Selected;
                        }
                    }
                    
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Builds the sub nodes of this node if required
        /// </summary>
        /// <param name="buildSubNodes"></param>
        protected void BaseBuildSubNodes(bool buildSubNodes)
        {
            if (buildSubNodes)
            {
                BuildSubNodes(false);
                UpdateColor();
                RefreshNode();
            }
        }

        /// <summary>
        ///     Lazy create the subnodes when it is expanded
        /// </summary>
        public override void HandleExpand()
        {
            foreach (BaseTreeNode node in Nodes)
            {
                if (!node.SubNodesBuilt)
                {
                    node.BuildSubNodes(false);
                    node.UpdateColor();
                }
            }
            RefreshNode();


            base.HandleExpand();
        }


        /// <summary>
        ///     An editor for an item. It is the responsibility of this class to implement attributes
        ///     for the elements to be edited.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class NamedEditor : Editor
        {
            /// <summary>
            ///     The item name
            /// </summary>
            [Category("Description")]
            public virtual string Name
            {
                get { return Item.Name; }
                set
                {
                    if (Item.EnclosingCollection != null)
                    {
                        bool unique = true;
                        foreach (IModelElement model in Item.EnclosingCollection)
                        {
                            INamable namable = model as INamable;
                            INamable namableItem = Item as INamable;
                            if (namable != namableItem && namable != null && namable.Name.CompareTo(value) == 0)
                            {
                                unique = false;
                                break;
                            }
                        }

                        if (unique)
                        {
                            Item.Name = value;
                        }
                        else
                        {
                            MessageBox.Show(
                                "Cannot set the name to " + value +
                                "because it conflits with another element of the same collection", "Name conflict",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        Item.Name = value;
                    }
                    RefreshNode();
                }
            }

            /// <summary>
            ///     Provides the unique identifier
            /// </summary>
            [Category("Meta data")]
            public virtual string UniqueIdentifier
            {
                get { return Item.FullName; }
            }

            /// <summary>
            ///     Provides the unique identifier
            /// </summary>
            [Category("Meta data")]
            public virtual string Update
            {
                get
                {
                    string retVal = "";
                    ModelElement element = Item as ModelElement;
                    if (element != null)
                    {
                        if (element.Updates != null)
                        {
                            retVal = "Updates " + element.Updates.FullName;
                        }
                    }

                    return retVal;
                }
            }

            /// <summary>
            ///     Constructor
            /// </summary>
            protected NamedEditor()
            {
            }
        }

        /// <summary>
        ///     The editor for a namable which can hold a comment
        /// </summary>
        public abstract class CommentableEditor : NamedEditor
        {
            [Category("Description")]
            [Editor(typeof (CommentableUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (CommentableUITypeConverter))]
            public ICommentable Comment
            {
                get { return Item as ICommentable; }
                set
                {
                    Item = value as T;
                    RefreshNode();
                }
            }

            /// <summary>
            ///     Constructor
            /// </summary>
            protected CommentableEditor()
            {
            }
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract Editor createEditor();

        /// <summary>
        ///     Handles a selection change event
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(displayStatistics);

            if (BaseTreeView != null && BaseTreeView.RefreshNodeContent)
            {
                IBaseForm baseForm = BaseForm;
                if (baseForm != null)
                {
                    Editor editor = createEditor();
                    editor.Item = Item;
                    editor.Node = this;
                    NodeEditor = editor;

                    if (baseForm.Properties != null)
                    {
                        baseForm.Properties.SelectedObject = NodeEditor;
                    }
                }
            }
        }

        public override Color ComputedColor
        {
            get
            {
                Color retVal = base.ComputedColor;

                if (Item != null)
                {
                    BaseTreeNode parentNode;

                    switch (Item.MessagePathInfo)
                    {
                        case MessagePathInfoEnum.Nothing:
                        case MessagePathInfoEnum.NotComputed:
                            retVal = Color.Black;
                            break;

                        case MessagePathInfoEnum.Error:
                            parentNode = (BaseTreeNode) Parent;
                            if (parentNode == null || parentNode.Model != Model)
                            {
                                retVal = Color.Red;
                            }
                            else
                            {
                                retVal = ComputeColorBasedOnItsSubNodes();
                            }
                            break;

                        case MessagePathInfoEnum.PathToError:
                            retVal = ComputeColorBasedOnItsSubNodes();
                            break;

                        case MessagePathInfoEnum.Warning:
                            parentNode = (BaseTreeNode) Parent;
                            if (parentNode == null || parentNode.Model != Model)
                            {
                                retVal = Color.Brown;
                            }
                            else
                            {
                                retVal = ComputeColorBasedOnItsSubNodes();
                            }
                            break;

                        case MessagePathInfoEnum.PathToWarning:
                            retVal = ComputeColorBasedOnItsSubNodes();
                            break;

                        case MessagePathInfoEnum.Info:
                            parentNode = (BaseTreeNode) Parent;
                            if (parentNode == null || parentNode.Model != Model)
                            {
                                retVal = Color.Blue;
                            }
                            else
                            {
                                retVal = ComputeColorBasedOnItsSubNodes();
                            }
                            break;

                        case MessagePathInfoEnum.PathToInfo:
                            retVal = ComputeColorBasedOnItsSubNodes();
                            break;
                    }
                }

                return retVal;
            }
        }
    }
}