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
using System.Reflection;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Variables;
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI
{
    public interface IBaseForm
    {
        /// <summary>
        ///     Provides the model element currently selected in this IBaseForm
        /// </summary>
        IModelElement DisplayedModel { get; }

        /// <summary>
        ///     The main tree view of the form
        /// </summary>
        BaseTreeView TreeView { get; }

        /// <summary>
        ///     Allows to refresh the view, when the selected model changed
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if refresh should be performed</returns>
        bool HandleSelectionChange(Context.SelectionContext context);

        /// <summary>
        ///     Allows to refresh the view, when the value of a model changed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns>True if the view should be refreshed</returns>
        bool HandleValueChange(IModelElement modelElement, Context.ChangeKind changeKind);
    }

    /// <summary>
    ///     The base class for all forms displayed in the GUI
    /// </summary>
    public class BaseForm : DockContent, IBaseForm
    {
        /// <summary>
        ///     The main tree view of the form
        /// </summary>
        public virtual BaseTreeView TreeView
        {
            get { return null; }
        }

        /// <summary>
        ///     Allows to refresh the view, when the selected model changed
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if refresh should be performed</returns>
        public virtual bool HandleSelectionChange(Context.SelectionContext context)
        {
            bool retVal = ShouldDisplay(context.Element);

            if (retVal)
            {
                retVal = DisplayedModel != context.Element;
                DisplayedModel = context.Element;

                if (TreeView != null)
                {
                    BaseTreeNode node = context.Sender as BaseTreeNode;
                    if (node == null || node.TreeView != TreeView)
                    {
                        node = TreeView.FindNode(context.Element, true);
                    }

                    if (node != null)
                    {
                        TreeView.SilentSelect = true;
                        TreeView.Selected = node;
                        TreeView.SilentSelect = false;
                        if ((context.Criteria & Context.SelectionCriteria.DoubleClick) != 0)
                        {
                            Focus();
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates that the model element should be displayed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <returns></returns>
        protected virtual bool ShouldDisplay(IModelElement modelElement)
        {
            bool retVal = !IsActivated;

            if (modelElement != null)
            {
                if (TreeView != null)
                {
                    BaseTreeNode correspondingNode = TreeView.FindNode(modelElement, true);
                    retVal = correspondingNode != null;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Allows to refresh the view, when the value of a model changed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns>True if the view should be refreshed</returns>
        public virtual bool HandleValueChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            return ShouldDisplayChange(modelElement, changeKind);
        }

        /// <summary>
        ///     Indicates that a change event should be displayed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns></returns>
        protected virtual bool ShouldDisplayChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            bool retVal = modelElement == null || DisplayedModel == null || DisplayedModel.IsParent(modelElement);

            // When end of cycle, only redisplay when the displayed element related to a variable
            if (retVal && changeKind == Context.ChangeKind.EndOfCycle)
            {
                IVariable variable = EnclosingFinder<IVariable>.find(DisplayedModel, true);
                retVal = (variable != null);
            }

            return retVal;
        }

        /// <summary>
        ///     Allows to refresh the view, when the information message changed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <returns>True if the view should be refreshed</returns>
        public virtual bool HandleInfoMessageChange(IModelElement modelElement)
        {
            bool retVal = modelElement == null || DisplayedModel == null || DisplayedModel.IsParent(modelElement);

            if (retVal)
            {
                if (TreeView != null)
                {
                    if (modelElement != null)
                    {
                        // Find the first displayed tree node to be colorized
                        BaseTreeNode displayedNode = null;
                        while (displayedNode == null && modelElement != null)
                        {
                            displayedNode = TreeView.FindNode(modelElement, false);
                            modelElement = modelElement.Enclosing as IModelElement;
                        }

                        if (displayedNode != null)
                        {
                            while (displayedNode != null)
                            {
                                bool changed = displayedNode.UpdateColor();
                                if (changed)
                                {
                                    displayedNode = displayedNode.Parent as BaseTreeNode;
                                }
                                else
                                {
                                    displayedNode = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        // When no model element is provided, the complete tree should be recolored
                        foreach (BaseTreeNode node in TreeView.Nodes)
                        {
                            node.RecursiveUpdateNodeColor();
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the model element currently selected in this IBaseForm
        /// </summary>
        public virtual IModelElement DisplayedModel { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected BaseForm()
        {
            // By default, a form is displayed in the document part of the window
            DockAreas = DockAreas.Document;
            Visible = false;

            // Handles a selection change in the system
            EFSSystem.INSTANCE.Context.SelectionChange += Context_SelectionChange;
            EFSSystem.INSTANCE.Context.ValueChange += Context_ValueChange;
            EFSSystem.INSTANCE.Context.InfoMessageChange += Context_InfoMessageChange;

            // Allow to dock back the form
            ParentChanged += BaseForm_ParentChanged;
            FormClosed += Window_FormClosed;
        }

        /// <summary>
        ///     Handles the close event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            EFSSystem.INSTANCE.Context.SelectionChange -= Context_SelectionChange;
            EFSSystem.INSTANCE.Context.ValueChange -= Context_ValueChange;
            EFSSystem.INSTANCE.Context.InfoMessageChange -= Context_InfoMessageChange;

            GuiUtils.MdiWindow.HandleSubWindowClosed(this);
        }

        /// <summary>
        ///     Tries the find the corresponding node when the selection occurs
        /// </summary>
        /// <param name="context"></param>
        protected virtual void Context_SelectionChange(Context.SelectionContext context)
        {
            HandleSelectionChange(context);
        }

        /// <summary>
        ///     The delegate used to handle the change of the value of a model element
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind">Indicates the reason why the change occured</param>
        protected virtual void Context_ValueChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            try
            {
                BeginInvoke((MethodInvoker) (() => HandleValueChange(modelElement, changeKind)));
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     Handles the change of the information message for a model element
        /// </summary>
        /// <param name="modelElement"></param>
        protected virtual void Context_InfoMessageChange(IModelElement modelElement)
        {
            try
            {
                BeginInvoke((MethodInvoker) (() => HandleInfoMessageChange(modelElement)));
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     Allows to dock back a window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseForm_ParentChanged(object sender, EventArgs e)
        {
            FloatWindow window = ParentForm as FloatWindow;
            if (window != null)
            {
                ParentForm.Move += ParentForm_Move;
            }
        }

        /// <summary>
        ///     When the parent of the current form changes, dock back this form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParentForm_Move(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                Hide();
                DockAreas = DockAreas.Document;
                DockState = DockState.Document;
                Show(GuiUtils.MdiWindow.dockPanel, DockState.Document);
            }
        }

        /// <summary>
        ///     Resizes the description area of the property grid to be as small as possible
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="height"></param>
        protected static void ResizeDescriptionArea(PropertyGrid grid, int height)
        {
            if (grid == null) throw new ArgumentNullException("grid");

            foreach (Control control in grid.Controls)
            {
                if (control.GetType().Name == "DocComment")
                {
                    Type baseType = control.GetType().BaseType;
                    if (baseType != null)
                    {
                        FieldInfo fieldInfo = baseType.GetField("userSized",
                            BindingFlags.Instance |
                            BindingFlags.NonPublic);
                        if (fieldInfo != null)
                        {
                            fieldInfo.SetValue(control, true);
                            control.Height = height;
                        }
                    }
                    break;
                }
            }
        }
    }
}