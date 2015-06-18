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
namespace GUI.HistoryView
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
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.historyTreeView = new GUI.HistoryView.HistoryTreeView();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid = new GUI.MyPropertyGrid();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.beforeGroupBox = new System.Windows.Forms.GroupBox();
            this.beforeRichTextBox = new GUI.EditorTextBox();
            this.afterGroupBox = new System.Windows.Forms.GroupBox();
            this.afterRichTextBox = new GUI.EditorTextBox();
            this.toolStrip3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.beforeGroupBox.SuspendLayout();
            this.afterGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(352, 25);
            this.toolStrip3.TabIndex = 1;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(45, 22);
            this.toolStripLabel1.Text = "History";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.historyTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(352, 733);
            this.splitContainer1.SplitterDistance = 163;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 5;
            // 
            // historyTreeView
            // 
            this.historyTreeView.AllowDrop = true;
            this.historyTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyTreeView.HideSelection = false;
            this.historyTreeView.ImageIndex = 0;
            this.historyTreeView.LabelEdit = true;
            this.historyTreeView.Location = new System.Drawing.Point(0, 0);
            this.historyTreeView.Name = "historyTreeView";
            this.historyTreeView.Root = null;
            this.historyTreeView.Selected = null;
            this.historyTreeView.SelectedImageIndex = 0;
            this.historyTreeView.Size = new System.Drawing.Size(352, 163);
            this.historyTreeView.TabIndex = 2;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.propertyGrid);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer3.Size = new System.Drawing.Size(352, 567);
            this.splitContainer3.SplitterDistance = 256;
            this.splitContainer3.TabIndex = 1;
            // 
            // propertyGrid
            // 
            this.propertyGrid.AllowDrop = true;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Margin = new System.Windows.Forms.Padding(2);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid.Size = new System.Drawing.Size(352, 256);
            this.propertyGrid.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.beforeGroupBox);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.afterGroupBox);
            this.splitContainer2.Size = new System.Drawing.Size(352, 307);
            this.splitContainer2.SplitterDistance = 153;
            this.splitContainer2.SplitterWidth = 3;
            this.splitContainer2.TabIndex = 0;
            // 
            // beforeGroupBox
            // 
            this.beforeGroupBox.Controls.Add(this.beforeRichTextBox);
            this.beforeGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.beforeGroupBox.Location = new System.Drawing.Point(0, 0);
            this.beforeGroupBox.Name = "beforeGroupBox";
            this.beforeGroupBox.Size = new System.Drawing.Size(352, 153);
            this.beforeGroupBox.TabIndex = 1;
            this.beforeGroupBox.TabStop = false;
            this.beforeGroupBox.Text = "Before";
            // 
            // beforeRichTextBox
            // 
            this.beforeRichTextBox.AutoComplete = true;
            this.beforeRichTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.beforeRichTextBox.ConsiderOnlyTypes = false;
            this.beforeRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.beforeRichTextBox.Instance = null;
            this.beforeRichTextBox.Location = new System.Drawing.Point(3, 16);
            this.beforeRichTextBox.Name = "beforeRichTextBox";
            this.beforeRichTextBox.ReadOnly = true;
            this.beforeRichTextBox.Size = new System.Drawing.Size(346, 134);
            this.beforeRichTextBox.TabIndex = 0;
            // 
            // afterGroupBox
            // 
            this.afterGroupBox.Controls.Add(this.afterRichTextBox);
            this.afterGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.afterGroupBox.Location = new System.Drawing.Point(0, 0);
            this.afterGroupBox.Name = "afterGroupBox";
            this.afterGroupBox.Size = new System.Drawing.Size(352, 151);
            this.afterGroupBox.TabIndex = 1;
            this.afterGroupBox.TabStop = false;
            this.afterGroupBox.Text = "After";
            // 
            // afterRichTextBox
            // 
            this.afterRichTextBox.AutoComplete = true;
            this.afterRichTextBox.ConsiderOnlyTypes = false;
            this.afterRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.afterRichTextBox.Instance = null;
            this.afterRichTextBox.Location = new System.Drawing.Point(3, 16);
            this.afterRichTextBox.Name = "afterRichTextBox";
            this.afterRichTextBox.ReadOnly = true;
            this.afterRichTextBox.Size = new System.Drawing.Size(346, 132);
            this.afterRichTextBox.TabIndex = 0;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 758);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip3);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Window";
            this.ShowInTaskbar = false;
            this.Text = "History";
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.beforeGroupBox.ResumeLayout(false);
            this.afterGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip3;
        private HistoryTreeView historyTreeView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private MyPropertyGrid propertyGrid;
        private EditorTextBox beforeRichTextBox;
        private EditorTextBox afterRichTextBox;
        private System.Windows.Forms.GroupBox beforeGroupBox;
        private System.Windows.Forms.GroupBox afterGroupBox;

    }
}