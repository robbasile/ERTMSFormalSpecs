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

using System.Drawing;
using System.Windows.Forms;
using DataDictionary.Types;

namespace GUI.ModelDiagram
{
    /// <summary>
    ///     The boxes that represent a name space
    /// </summary>
    public class NameSpaceModelControl : ModelControl
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public NameSpaceModelControl(NameSpace model)
            : base(model)
        {
            NORMAL_COLOR = Color.LightGreen;
            BoxMode = BoxModeEnum.RoundedCorners;
            MouseDoubleClick += new MouseEventHandler(HandleMouseDoubleClick);
        }

        /// <summary>
        ///     Handles a simple click event on the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Panel.Select(this, true);
            }
        }

        /// <summary>
        ///     The name of the kind of model
        /// </summary>
        public override string ModelName
        {
            get { return "Namespace"; }
        }
    }
}