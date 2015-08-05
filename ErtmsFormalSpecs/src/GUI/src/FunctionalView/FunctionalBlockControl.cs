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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Types;
using DataDictionary.Types.AccessMode;
using GUI.BoxArrowDiagram;
using GUI.Properties;

namespace GUI.FunctionalView
{
    public class FunctionalBlockControl : BoxControl<IEnclosesNameSpaces, NameSpace, AccessMode>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public FunctionalBlockControl()
        {
            BoxMode = BoxModeEnum.RoundedCorners;
            BackColor = Color.Transparent;
            MouseDoubleClick += HandleMouseDoubleClick;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public FunctionalBlockControl(IContainer container)
            : base(container)
        {
            BoxMode = BoxModeEnum.RoundedCorners;
            BackColor = Color.Transparent;
            MouseDoubleClick += HandleMouseDoubleClick;
        }

        /// <summary>
        ///     Handles a double click event on the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDoubleClick(object sender, MouseEventArgs e)
        {
            FunctionalAnalysisPanel panel = (FunctionalAnalysisPanel) Panel;
            if (panel != null)
            {
                FunctionalAnalysisWindow window = new FunctionalAnalysisWindow();
                GuiUtils.MdiWindow.AddChildWindow(window);
                window.SetNameSpaceContainer(Model);
                window.Text = Model.Name + @" " +
                              Resources.FunctionalBlockControl_HandleMouseDoubleClick_functional_analysis;
            }
        }
    }
}