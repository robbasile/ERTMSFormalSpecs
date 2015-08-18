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
using DataDictionary;
using GUI.BoxArrowDiagram;
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.PropertyView
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();
            ResizeDescriptionArea(propertyGrid, 0);

            DockAreas = DockAreas.DockRight;
        }

        /// <summary>
        ///     Allows to refresh the view, when the selected model changed
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if refresh should be performed</returns>
        public override bool HandleSelectionChange(Context.SelectionContext context)
        {
            bool retVal = base.HandleSelectionChange(context);

            propertyGrid.SelectedObject = null;
            BaseTreeNode node = GuiUtils.SourceNode(context);
            if (node != null)
            {
                propertyGrid.SelectedObject = node.GetEditor();
            }
            else
            {
                BaseBoxArrowPanel panel = GuiUtils.EnclosingFinder<BaseBoxArrowPanel>.Find(context.Sender as Control);
                if (panel != null)
                {
                    propertyGrid.SelectedObject = panel.CreateEditor(context.Element);
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
        public override bool HandleValueChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            bool retVal = base.HandleValueChange(modelElement, changeKind);

            if (retVal)
            {
                propertyGrid.Refresh();
            }

            return retVal;
        }
    }
}