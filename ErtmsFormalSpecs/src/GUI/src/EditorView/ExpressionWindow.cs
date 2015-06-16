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

namespace GUI.EditorView
{
    /// <summary>
    ///     An expression editor window
    /// </summary>
    public class ExpressionWindow : Window
    {
        protected override string EditorName
        {
            get { return "Expression editor"; }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof (ExpressionWindow));
            this.SuspendLayout();
            // 
            // ExpressionWindow
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.ClientSize = new Size(699, 218);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Icon = ((Icon) (resources.GetObject("$this.Icon")));
            this.Name = "ExpressionWindow";
            this.ResumeLayout(false);
        }
    }
}