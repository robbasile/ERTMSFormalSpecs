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

namespace GUI.Shortcuts
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     The tree view used to display shortcuts
        /// </summary>
        public override BaseTreeView TreeView
        {
            get { return shortcutTreeView; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            shortcutTreeView.Root = EfsSystem.Instance;
            Text = "Shortcuts view";

            DockAreas = DockAreas.DockRight;
            Refresh();
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
                shortcutTreeView.RefreshModel(modelElement);                
            }

            return retVal;
        }
    }
}