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
namespace GUI.BoxArrowDiagram
{
    partial class BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel>
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pleaseWaitLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // pleaseWaitLabel
            // 
            this.pleaseWaitLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pleaseWaitLabel.Location = new System.Drawing.Point(0, 0);
            this.pleaseWaitLabel.Name = "pleaseWaitLabel";
            this.pleaseWaitLabel.Size = new System.Drawing.Size(100, 23);
            this.pleaseWaitLabel.TabIndex = 0;
            this.pleaseWaitLabel.Text = "Building model, please wait...";
            this.pleaseWaitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BoxArrowPanel
            // 
            this.AutoScroll = true;
            this.ContextMenuStrip = this.contextMenu;
            this.ResumeLayout(false);

        }

        protected System.Windows.Forms.ContextMenuStrip contextMenu;
        #endregion
        private System.Windows.Forms.Label pleaseWaitLabel;
    }
}
