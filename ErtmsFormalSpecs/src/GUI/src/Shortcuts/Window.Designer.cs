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
namespace GUI.Shortcuts
{
    partial class Window
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.shortcutTreeView = new GUI.Shortcuts.ShortcutTreeView();
            this.SuspendLayout();
            // 
            // shortcutTreeView
            // 
            this.shortcutTreeView.AllowDrop = true;
            this.shortcutTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shortcutTreeView.HideSelection = false;
            this.shortcutTreeView.ImageIndex = 0;
            this.shortcutTreeView.LabelEdit = true;
            this.shortcutTreeView.Location = new System.Drawing.Point(0, 0);
            this.shortcutTreeView.Name = "shortcutTreeView";
            this.shortcutTreeView.Root = null;
            this.shortcutTreeView.Selected = null;
            this.shortcutTreeView.SelectedImageIndex = 0;
            this.shortcutTreeView.Size = new System.Drawing.Size(221, 303);
            this.shortcutTreeView.TabIndex = 1;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 303);
            this.Controls.Add(this.shortcutTreeView);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Window";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Shortcuts";
            this.ResumeLayout(false);

        }

        #endregion

        private ShortcutTreeView shortcutTreeView;
    }
}