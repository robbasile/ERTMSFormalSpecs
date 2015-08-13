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
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Generated;
using GUI.Converters;
using GUI.DictionarySelector;
using GUI.LongOperations;
using GUI.Properties;
using Utils;
using XmlBooster;
using Chapter = DataDictionary.Specification.Chapter;
using Dictionary = DataDictionary.Dictionary;
using Frame = DataDictionary.Tests.Frame;
using ModelElement = DataDictionary.ModelElement;
using ReqRelated = DataDictionary.Generated.ReqRelated;
using Specification = DataDictionary.Specification.Specification;
using Step = DataDictionary.Tests.Step;
using SubSequence = DataDictionary.Tests.SubSequence;
using TestCase = DataDictionary.Tests.TestCase;

namespace GUI
{
    /// <summary>
    ///     The base class for all tree nodes
    /// </summary>
    public abstract class BaseTreeNode : TreeNode, IComparable<BaseTreeNode>
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
            protected BaseEditor()
            {
            }
        }

        /// <summary>
        ///     The editor used to edit the node contents
        /// </summary>
        public BaseEditor NodeEditor { protected get; set; }

        /// <summary>
        /// Provides the editor
        /// </summary>
        /// <returns></returns>
        public abstract BaseEditor GetEditor();


        /// <summary>
        ///     The fixed node name
        /// </summary>
        private string DefaultName { get; set; }

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
            get { return GuiUtils.EnclosingFinder<IBaseForm>.Find(BaseTreeView); }
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
            UpdateText();
            SetImageIndex(isFolder);
        }

        /// <summary>
        ///     Indicates that the subNodes have already been built, hence, does not require to build its contents anymore
        /// </summary>
        public bool SubNodesBuilt = false;

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public virtual void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            SubNodesBuilt = true;
        }

        /// <summary>
        ///     Builds the sub nodes of this node if required
        /// </summary>
        /// <param name="modifiedElement">The element that has been modified</param>
        public void BuildOrRefreshSubNodes(IModelElement modifiedElement)
        {
            List<BaseTreeNode> subNodes = new List<BaseTreeNode>();
            BuildSubNodes(subNodes, false);

            if (SubNodesAreDifferent(subNodes))
            {
                Nodes.Clear();
                foreach (BaseTreeNode node in subNodes)
                {
                    Nodes.Add(node);
                }
            }

            foreach (BaseTreeNode node in Nodes)
            {
                bool refreshNode = node.Model.IsParent(modifiedElement) || node.IsExpanded;
                if (Parent != null)
                {
                    refreshNode = refreshNode || Parent.IsExpanded;
                }

                if (refreshNode)
                {
                    node.BuildOrRefreshSubNodes(modifiedElement);
                }
            }

            UpdateColor();
            RefreshNode();
        }

        /// <summary>
        /// Indicates that the sub nodes provided are different from the ones stored in this data tree node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <returns></returns>
        private bool SubNodesAreDifferent(List<BaseTreeNode> subNodes)
        {
            bool retVal = Nodes.Count != subNodes.Count;

            if (!retVal)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    BaseTreeNode source = Nodes[i] as BaseTreeNode;
                    if (source != null)
                    {
                        BaseTreeNode target = subNodes[i];
                        if (source.Model != target.Model)
                        {
                            retVal = true;
                            break;
                        }
                    }
                }
            }

            return retVal;
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
        public virtual void SetImageIndex(bool isFolder)
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
        ///     Handles a selection event
        /// </summary>
        public virtual void SelectionHandler()
        {
            EFSSystem.INSTANCE.Context.SelectElement(Model, this, Context.SelectionCriteria.LeftClick);            
        }

        /// <summary>
        ///     Handles a double click event on this tree node
        /// </summary>
        public virtual void DoubleClickHandler()
        {
            EFSSystem.INSTANCE.Context.SelectElement(Model, this, Context.SelectionCriteria.DoubleClick);
        }

        /// <summary>
        ///     Compares two tree nodes, used by the sort
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(BaseTreeNode other)
        {
            int retVal;

            if (Model != null && other.Model != null)
            {
                retVal = Model.CompareTo(other.Model);
            }
            else
            {
                retVal = String.Compare(Text, other.Text);
            }

            return retVal;
        }

        /// <summary>
        ///     The colors used to display things
        /// </summary>
        private static readonly Color ErrorColor = Color.Red;
        private static readonly Color PathToErrorColor = Color.Orange;
        private static readonly Color WarningColor = Color.Brown;
        private static readonly Color PathToWarningColor = Color.LightCoral;
        private static readonly Color InfoColor = Color.Blue;
        private static readonly Color PathToInfoColor = Color.LightBlue;
        private static readonly Color NothingColor = Color.Black;

        /// <summary>
        ///     Provides the color according to the info status
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected Color ColorBasedOnInfo(MessageInfoEnum info)
        {
            Color retVal = NothingColor;

            if ((info & MessageInfoEnum.Error) != 0)
            {
                retVal = ErrorColor;
            }
            else if ((info & MessageInfoEnum.PathToError) != 0)
            {
                retVal = PathToErrorColor;
            }
            else if ((info & MessageInfoEnum.Warning) != 0)
            {
                retVal = WarningColor;
            }
            else if ((info & MessageInfoEnum.PathToWarning) != 0)
            {
                retVal = PathToWarningColor;
            }
            else if ((info & MessageInfoEnum.Info) != 0)
            {
                retVal = InfoColor;
            }
            else if ((info & MessageInfoEnum.PathToInfo) != 0)
            {
                retVal = PathToInfoColor;
            }

            return retVal;
        }
        
        /// <summary>
        ///     Updates the node color according to the associated messages
        /// </summary>
        /// <returns>true if the node color has been changed</returns>
        public bool UpdateColor()
        {
            bool retVal = false;
            Color color = NothingColor;

            if (Model != null)
            {
                MessageInfoEnum info;

                BaseTreeNode parent = Parent as BaseTreeNode;
                if (parent != null && parent.Model != Model)
                {
                    info = Model.MessagePathInfo;
                }
                else
                {
                    // The color of this node is computed according to the color of its sub nodes
                    if (!SubNodesBuilt)
                    {
                        BuildOrRefreshSubNodes(null);
                    }

                    info = MessageInfoEnum.NoMessage;
                    foreach (BaseTreeNode subNode in Nodes)
                    {
                        info = info | subNode.Model.MessagePathInfo;
                    }
                }

                color = ColorBasedOnInfo(info);
            }

            // ReSharper disable once RedundantCheckBeforeAssignment
            if (color != ForeColor)
            {
                retVal = true;
                ForeColor = color;
            }

            return retVal;
        }

        /// <summary>
        /// Recursively updates the node colors
        /// </summary>
        public void RecursiveUpdateNodeColor()
        {
            UpdateColor();
            foreach (BaseTreeNode subNode in Nodes)
            {
                subNode.RecursiveUpdateNodeColor();
            }
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
            if ((parent != null))
            {
                parent.Nodes.Remove(this);
                Model.Delete();

                DataDictionary.ReqRelated reqRelated = Model as DataDictionary.ReqRelated;
                if (reqRelated != null)
                {
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
                ModelElement modelElement = element as ModelElement;
                if (modelElement != null)
                {
                    visit(modelElement);
                    dispatch(modelElement);
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
            MarkingHistory.PerformMark(() =>
            {
                ModelElement modelElement = Model as ModelElement;
                if (modelElement != null)
                {
                    RuleCheckerVisitor visitor = new RuleCheckerVisitor(modelElement.Dictionary);

                    visitor.visit(modelElement, true);
                }
            });
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
                ModelElement modelElement = element as ModelElement;
                if (modelElement != null)
                {
                    visit(modelElement);
                    dispatch(modelElement);
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
        ///     Provides the menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected virtual List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Rename", LabelEditHandler), new MenuItem("-")};

            MenuItem newItem = new MenuItem("Recursive actions...");
            newItem.MenuItems.Add(new MenuItem("Mark as implemented", MarkAsImplemented));
            newItem.MenuItems.Add(new MenuItem("Mark as verified", MarkAsVerified));
            retVal.Add(newItem);
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Check", Check));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Refresh", RefreshNodeHandler));

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
        public BaseTreeNode FindSubNode(string text)
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
            EFSSystem.INSTANCE.ClearMessages(false);
        }

        /// <summary>
        ///     Accepts the drop of a base tree node on this node
        /// </summary>
        /// <param name="sourceNode"></param>
        public virtual void AcceptDrop(BaseTreeNode sourceNode)
        {
        }

        /// <summary>
        ///     Generates new GUID for the element
        /// </summary>
        private class RegenerateGuidVisitor : Visitor
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

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        /// Provides the sub node of the corresponding type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T SubNode<T>()
            where T : class
        {
            T retVal = null;

            foreach (TreeNode node in Nodes)
            {
                retVal = node as T;
                if (retVal != null)
                {
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Accepts the drop of a base tree node on this node
        /// </summary>
        /// <param name="sourceNode"></param>
        public virtual void AcceptCopy(BaseTreeNode sourceNode)
        {
            XmlBBase xmlBBase = sourceNode.Model as XmlBBase;
            if (xmlBBase != null)
            {
                string data = xmlBBase.ToXMLString();
                XmlBStringContext ctxt = new XmlBStringContext(data);
                try
                {
                    ModelElement copy = acceptor.accept(ctxt) as ModelElement;
                    if (copy != null)
                    {
                        RegenerateGuidVisitor visitor = new RegenerateGuidVisitor();
                        visitor.visit(copy, true);

                        Model.AddModelElement(copy);
                        ArrayList targetCollection = copy.EnclosingCollection;
                        copy.Delete();
                        if (targetCollection != null)
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
                                        if (other.Name.Equals(copy.Name + "_" + index))
                                        {
                                            index += 1;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (other.Name.Equals(copy.Name))
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
                                copy.Name = copy.Name + "_" + index;
                            }
                        }

                        Model.AddModelElement(copy);
                    }
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
        /// <param name="sourceNode"></param>
        public virtual void AcceptMove(BaseTreeNode sourceNode)
        {
            ArrayList sourceCollection = sourceNode.Model.EnclosingCollection;
            ArrayList thisCollection = Model.EnclosingCollection;

            if (thisCollection != null && sourceCollection == thisCollection)
            {
                // This is a reordering
                int sourceIndex = thisCollection.IndexOf(sourceNode.Model);
                int thisIndex = thisCollection.IndexOf(Model);
                if (thisIndex >= 0 && thisIndex >= 0 && thisIndex != sourceIndex)
                {
                    thisCollection.Remove(sourceNode.Model);
                    thisIndex = thisCollection.IndexOf(Model);
                    thisCollection.Insert(thisIndex, sourceNode.Model);
                    EFSSystem.INSTANCE.Context.HandleChangeEvent(Model as BaseModelElement);
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
                        RefactorOperation refactorOperation = new RefactorOperation(Model as ModelElement, newLabel);
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
        public abstract class Editor : BaseEditor
        {
            /// <summary>
            ///     The item that is edited.
            /// </summary>
            private T _item;

            [Browsable(false)]
            public T Item
            {
                get { return _item; }
                set
                {
                    _item = value;
                    Model = value;
                    UpdateActivation();
                }
            }

            /// <summary>
            /// Indicates that the element has been removed
            /// </summary>
            public string Removed
            {
                get
                {
                    string retVal = "False";

                    ModelElement model = Item as ModelElement;

                    if (model != null && model.IsRemoved)
                    {
                        if (model.getIsRemoved())
                        {
                            retVal = "True";
                        }
                        else
                        {
                            retVal = "In update";
                        }
                    }

                    return retVal;
                }
            }

            /// <summary>
            ///     The node that holds the item.
            /// </summary>
            internal ModelElementTreeNode<T> Node { get; set; }

            public void RefreshNode()
            {
                Node.RefreshNode();
            }

            public void RefreshTree()
            {
                Node.BaseTreeView.Refresh();
            }

            /// <summary>
            ///     Updates the field activation according to the displayed data
            /// </summary>
            protected virtual void UpdateActivation()
            {
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
            if (buildSubNodes)
            {
                BuildOrRefreshSubNodes(null);                
            }
        }

        /// <summary>
        ///     Provides the dictionary on which operation should be performed
        /// </summary>
        /// <returns></returns>
        public Dictionary GetPatchDictionary()
        {
            Dictionary retVal = null;

            MainWindow mainWindow = GuiUtils.MdiWindow;
            EFSSystem efsSystem = mainWindow.EfsSystem;
            if (efsSystem != null)
            {
                ModelElement modelElement = Item as ModelElement;
                if (modelElement != null)
                {
                    int updates = 0;
                    foreach (Dictionary dict in efsSystem.Dictionaries)
                    {
                        if (modelElement.Dictionary.IsUpdatedBy(dict))
                        {
                            // Set retVal to the update in case it is the only one for the base dictionary
                            retVal = dict;
                            updates++;
                        }
                    }

                    if (updates == 0)
                    {
                        MessageBox.Show("No updates loaded for the current dictionary.");
                    }

                    if (updates > 1)
                    {
                        // if there are 0 or 1 possible updates, it will already have the correct value
                        // if there are more, choose the update from a list of possibilities
                        DictionarySelector.DictionarySelector dictionarySelector =
                            new DictionarySelector.DictionarySelector(efsSystem, FilterOptions.Updates,
                                modelElement.Dictionary);
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
        ///     Adds a copy of the current model element to the selected dictionary, if a copy does not already exist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddUpdate(object sender, EventArgs args)
        {
            FindOrCreateUpdate();
        }

        /// <summary>
        /// Find or creates an update for the current element
        /// </summary>
        /// <returns></returns>
        protected virtual ModelElement FindOrCreateUpdate()
        {
            return null;
        }

        /// <summary>
        ///     Marks the item as removed from the model. The tool will treat it as if it was deleted.
        /// </summary>
        protected void RemoveInUpdate(object sender, EventArgs args)
        {
            ModelElement model = Item as ModelElement;
            if (model != null && model.Updates == null)
            {
                model = FindOrCreateUpdate();
            }

            if (model != null)
            {
                model.setIsRemoved(true);
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
                    node.BuildOrRefreshSubNodes(null);
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
                get
                {
                    string retVal = "";

                    if (Item != null)
                    {
                        retVal = Item.Name;
                    }

                    return retVal;
                }
                set
                {
                    if (Item.EnclosingCollection != null)
                    {
                        bool unique = true;
                        foreach (IModelElement model in Item.EnclosingCollection)
                        {
                            if (model != null && model != Item && model.Name.CompareTo(value) == 0)
                            {
                                unique = false;
                                break;
                            }
                        }

                        if (unique)
                        {
                            if (Settings.Default.AllowRefactor)
                            {
                                RefactorOperation refactorOperation = new RefactorOperation(Model as ModelElement, value);
                                refactorOperation.ExecuteUsingProgressDialog("Refactoring", false);
                            }
                            else
                            {
                                Item.Name = value;
                            }
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
                get
                {
                    string retVal = "";

                    if (Item != null)
                    {
                        retVal = Item.FullName;
                    }

                    return retVal;
                }
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
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected abstract Editor CreateEditor();

        /// <summary>
        ///     Handles a selection change event
        /// </summary>
        public override void SelectionHandler()
        {
            NodeEditor = GetEditor();

            base.SelectionHandler();
        }

        /// <summary>
        /// Provides the editor for this node, and link the editor with the edited model element
        /// </summary>
        /// <returns></returns>
        public override BaseEditor GetEditor()
        {
            BaseEditor retVal = NodeEditor;

            if (retVal == null)
            {
                Editor editor = CreateEditor();
                editor.Item = Item;
                editor.Node = this;


                retVal = editor;
            }

            return retVal;
        }
    }
}