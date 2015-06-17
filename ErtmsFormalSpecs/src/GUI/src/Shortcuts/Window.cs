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
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.Shortcuts
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            FormClosed += new FormClosedEventHandler(Window_FormClosed);

            Visible = false;
            shortcutTreeView.Root = EFSSystem.INSTANCE;
            Text = "Shortcuts view";

            DockAreas = DockAreas.DockRight;
            Refresh();
        }

        /// <summary>
        ///     Handles the close event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            GUIUtils.MDIWindow.HandleSubWindowClosed(this);
        }

        /// <summary>
        ///     Refreshed the model of the window
        /// </summary>
        public override void RefreshModel()
        {
            shortcutTreeView.RefreshModel();
            Refresh();
        }

        public override BaseTreeView TreeView
        {
            get { return shortcutTreeView; }
        }

        /// <summary>
        ///     Provides the model element currently selected in this IBaseForm
        /// </summary>
        public override IModelElement Selected
        {
            get
            {
                IModelElement retVal = null;

                if (TreeView != null && TreeView.Selected != null)
                {
                    retVal = TreeView.Selected.Model;
                }

                return retVal;
            }
        }
    }
}