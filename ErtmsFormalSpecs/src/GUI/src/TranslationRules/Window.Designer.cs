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
namespace GUI.TranslationRules
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
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.testBrowserStatusLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.nextErrortoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.nextWarningToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.nextInfoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.translationTreeView = new GUI.TranslationRules.TranslationTreeView();
            this.staticTimeLineControl = new GUI.TestRunnerView.TimeLineControl.StaticTimeLineControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.testBrowserStatusLabel,
            this.toolStripSeparator2,
            this.nextErrortoolStripButton,
            this.nextWarningToolStripButton,
            this.nextInfoToolStripButton});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(815, 25);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // testBrowserStatusLabel
            // 
            this.testBrowserStatusLabel.Name = "testBrowserStatusLabel";
            this.testBrowserStatusLabel.Size = new System.Drawing.Size(86, 22);
            this.testBrowserStatusLabel.Text = "toolStripLabel1";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // nextErrortoolStripButton
            // 
            this.nextErrortoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextErrortoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("nextErrortoolStripButton.Image")));
            this.nextErrortoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextErrortoolStripButton.Name = "nextErrortoolStripButton";
            this.nextErrortoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nextErrortoolStripButton.Text = "Next error";
            this.nextErrortoolStripButton.Click += new System.EventHandler(this.nextErrortoolStripButton_Click);
            // 
            // nextWarningToolStripButton
            // 
            this.nextWarningToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextWarningToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("nextWarningToolStripButton.Image")));
            this.nextWarningToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextWarningToolStripButton.Name = "nextWarningToolStripButton";
            this.nextWarningToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nextWarningToolStripButton.Text = "Next warning";
            this.nextWarningToolStripButton.Click += new System.EventHandler(this.nextWarningToolStripButton_Click);
            // 
            // nextInfoToolStripButton
            // 
            this.nextInfoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextInfoToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("nextInfoToolStripButton.Image")));
            this.nextInfoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextInfoToolStripButton.Name = "nextInfoToolStripButton";
            this.nextInfoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nextInfoToolStripButton.Text = "Next info";
            this.nextInfoToolStripButton.Click += new System.EventHandler(this.nextInfoToolStripButton_Click);
            // 
            // translationTreeView
            // 
            this.translationTreeView.AllowDrop = true;
            this.translationTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.translationTreeView.HideSelection = false;
            this.translationTreeView.ImageIndex = 0;
            this.translationTreeView.LabelEdit = true;
            this.translationTreeView.Location = new System.Drawing.Point(0, 0);
            this.translationTreeView.Name = "translationTreeView";
            this.translationTreeView.Root = null;
            this.translationTreeView.Selected = null;
            this.translationTreeView.SelectedImageIndex = 0;
            this.translationTreeView.Size = new System.Drawing.Size(271, 548);
            this.translationTreeView.TabIndex = 1;
            // 
            // staticTimeLineControl
            // 
            this.staticTimeLineControl.AllowDrop = true;
            this.staticTimeLineControl.AutoScroll = true;
            this.staticTimeLineControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.staticTimeLineControl.Location = new System.Drawing.Point(0, 0);
            this.staticTimeLineControl.Name = "staticTimeLineControl";
            this.staticTimeLineControl.Size = new System.Drawing.Size(540, 548);
            this.staticTimeLineControl.SubSequence = null;
            this.staticTimeLineControl.TabIndex = 3;
            this.staticTimeLineControl.TestCase = null;
            this.staticTimeLineControl.Translation = null;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.translationTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.staticTimeLineControl);
            this.splitContainer1.Size = new System.Drawing.Size(815, 548);
            this.splitContainer1.SplitterDistance = 271;
            this.splitContainer1.TabIndex = 4;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 573);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip2);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Window";
            this.ShowInTaskbar = false;
            this.Text = "Translation rules";
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel testBrowserStatusLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private TranslationTreeView translationTreeView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton nextErrortoolStripButton;
        private System.Windows.Forms.ToolStripButton nextWarningToolStripButton;
        private System.Windows.Forms.ToolStripButton nextInfoToolStripButton;
        private TestRunnerView.TimeLineControl.StaticTimeLineControl staticTimeLineControl;
        private System.Windows.Forms.SplitContainer splitContainer1;

    }
}