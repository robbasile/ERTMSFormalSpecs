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

using DataDictionary;
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.UsageView
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            usageTreeView.LabelEdit = false;
            DockAreas = DockAreas.DockBottom;
        }

        /// <summary>
        ///     Indicates that the model element should be displayed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <returns></returns>
        protected override bool ShouldTrackSelectionChange(IModelElement modelElement)
        {
            return modelElement == null || DisplayedModel != modelElement;
        }

        /// <summary>
        ///     Allows to refresh the view, when the selected model changed
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if refresh should be performed</returns>
        public override bool HandleSelectionChange(Context.SelectionContext context)
        {
            bool retVal = base.HandleSelectionChange(context);

            if (retVal)
            {
                usageTreeView.Root = DisplayedModel;
                if (usageTreeView.Nodes.Count > 0)
                {
                    usageTreeView.Nodes[0].EnsureVisible();
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates that a change event should be displayed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns></returns>
        protected override bool ShouldDisplayChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            // All changes in the model should be tracked
            return true;
        }

        /// <summary>
        ///     Allows to refresh the view, when the value of a model changed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns>True if the view should be refreshed</returns>
        public override bool HandleValueChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            bool retVal = base.HandleValueChange(modelElement, changeKind);

            if (retVal)
            {
                usageTreeView.Root = DisplayedModel;
                if (usageTreeView.Nodes.Count > 0)
                {
                    usageTreeView.Nodes[0].EnsureVisible();
                }
            }

            return retVal;
        }

        /// <summary>
        /// Indicates that coloring should be taken into consideration
        /// </summary>
        /// <param name="modelElement"></param>
        /// <returns></returns>
        public override bool ShouldUpdateColoring(IModelElement modelElement)
        {
            // This view do not use coloring
            return false;
        }
    }
}