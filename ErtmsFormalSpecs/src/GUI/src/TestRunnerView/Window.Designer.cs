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
namespace GUI.TestRunnerView
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
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.nextErrortoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.nextWarningToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.nextInfoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.frameToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.subSequenceSelectorComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.rewindButton = new System.Windows.Forms.ToolStripButton();
            this.RestartButton = new System.Windows.Forms.ToolStripButton();
            this.StepOnceButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTimeTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripCurrentStepTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.testBrowserTreeView = new GUI.TestRunnerView.TestTreeView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.testDescriptionTabPage = new System.Windows.Forms.TabPage();
            this.testDescriptionTimeLineControl = new GUI.TestRunnerView.TimeLineControl.StaticTimeLineControl();
            this.testExecutionTabPage = new System.Windows.Forms.TabPage();
            this.testExecutionTimeLineControl = new GUI.TestRunnerView.TimeLineControl.DynamicTimeLineControl();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.testDescriptionTabPage.SuspendLayout();
            this.testExecutionTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel3,
            this.toolStripSeparator1,
            this.toolStripButton1,
            this.toolStripButton2,
            this.nextErrortoolStripButton,
            this.nextWarningToolStripButton,
            this.nextInfoToolStripButton,
            this.toolStripSeparator4,
            this.toolStripLabel5,
            this.frameToolStripComboBox,
            this.toolStripLabel2,
            this.subSequenceSelectorComboBox,
            this.toolStripSeparator5,
            this.rewindButton,
            this.RestartButton,
            this.StepOnceButton,
            this.toolStripButton4,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.toolStripTimeTextBox,
            this.toolStripSeparator3,
            this.toolStripLabel4,
            this.toolStripCurrentStepTextBox});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(926, 25);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(34, 22);
            this.toolStripLabel3.Text = "Tests";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.ToolTipText = "Select previous marking";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            this.toolStripButton2.ToolTipText = "Select next marking";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
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
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(40, 22);
            this.toolStripLabel5.Text = "Frame";
            // 
            // frameToolStripComboBox
            // 
            this.frameToolStripComboBox.DropDownWidth = 400;
            this.frameToolStripComboBox.Name = "frameToolStripComboBox";
            this.frameToolStripComboBox.Size = new System.Drawing.Size(121, 25);
            this.frameToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.frameSelectorComboBox_SelectionChanged);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(80, 22);
            this.toolStripLabel2.Text = "Sub sequence";
            this.toolStripLabel2.Click += new System.EventHandler(this.toolStripLabel2_Click);
            // 
            // subSequenceSelectorComboBox
            // 
            this.subSequenceSelectorComboBox.DropDownWidth = 400;
            this.subSequenceSelectorComboBox.Name = "subSequenceSelectorComboBox";
            this.subSequenceSelectorComboBox.Size = new System.Drawing.Size(201, 25);
            this.subSequenceSelectorComboBox.SelectedIndexChanged += new System.EventHandler(this.testCaseSelectorComboBox_SelectionChanged);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // rewindButton
            // 
            this.rewindButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rewindButton.Image = ((System.Drawing.Image)(resources.GetObject("rewindButton.Image")));
            this.rewindButton.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.rewindButton.Name = "rewindButton";
            this.rewindButton.Size = new System.Drawing.Size(23, 22);
            this.rewindButton.Text = "Step back";
            this.rewindButton.Click += new System.EventHandler(this.rewindButton_Click);
            // 
            // RestartButton
            // 
            this.RestartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RestartButton.Image = ((System.Drawing.Image)(resources.GetObject("RestartButton.Image")));
            this.RestartButton.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.RestartButton.Name = "RestartButton";
            this.RestartButton.Size = new System.Drawing.Size(23, 22);
            this.RestartButton.Text = "Restart";
            this.RestartButton.Click += new System.EventHandler(this.restart_Click);
            // 
            // StepOnceButton
            // 
            this.StepOnceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.StepOnceButton.Image = ((System.Drawing.Image)(resources.GetObject("StepOnceButton.Image")));
            this.StepOnceButton.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.StepOnceButton.Name = "StepOnceButton";
            this.StepOnceButton.Size = new System.Drawing.Size(23, 22);
            this.StepOnceButton.Text = "Step once";
            this.StepOnceButton.Click += new System.EventHandler(this.stepOnce_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton4.Text = "toolStripButton4";
            this.toolStripButton4.ToolTipText = "Pause";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(34, 22);
            this.toolStripLabel1.Text = "Time";
            // 
            // toolStripTimeTextBox
            // 
            this.toolStripTimeTextBox.Name = "toolStripTimeTextBox";
            this.toolStripTimeTextBox.ReadOnly = true;
            this.toolStripTimeTextBox.Size = new System.Drawing.Size(68, 25);
            this.toolStripTimeTextBox.Text = "0";
            this.toolStripTimeTextBox.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(73, 22);
            this.toolStripLabel4.Text = "Current Step";
            this.toolStripLabel4.Click += new System.EventHandler(this.toolStripLabel4_Click);
            // 
            // toolStripCurrentStepTextBox
            // 
            this.toolStripCurrentStepTextBox.AutoToolTip = true;
            this.toolStripCurrentStepTextBox.Name = "toolStripCurrentStepTextBox";
            this.toolStripCurrentStepTextBox.ReadOnly = true;
            this.toolStripCurrentStepTextBox.Size = new System.Drawing.Size(501, 23);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.testBrowserTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(926, 548);
            this.splitContainer1.SplitterDistance = 306;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 5;
            // 
            // testBrowserTreeView
            // 
            this.testBrowserTreeView.AllowDrop = true;
            this.testBrowserTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testBrowserTreeView.HideSelection = false;
            this.testBrowserTreeView.ImageIndex = 0;
            this.testBrowserTreeView.LabelEdit = true;
            this.testBrowserTreeView.Location = new System.Drawing.Point(0, 0);
            this.testBrowserTreeView.Name = "testBrowserTreeView";
            this.testBrowserTreeView.Root = null;
            this.testBrowserTreeView.Selected = null;
            this.testBrowserTreeView.SelectedImageIndex = 0;
            this.testBrowserTreeView.Size = new System.Drawing.Size(306, 548);
            this.testBrowserTreeView.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.testDescriptionTabPage);
            this.tabControl1.Controls.Add(this.testExecutionTabPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(617, 548);
            this.tabControl1.TabIndex = 1;
            // 
            // testDescriptionTabPage
            // 
            this.testDescriptionTabPage.Controls.Add(this.testDescriptionTimeLineControl);
            this.testDescriptionTabPage.Location = new System.Drawing.Point(4, 22);
            this.testDescriptionTabPage.Name = "testDescriptionTabPage";
            this.testDescriptionTabPage.Size = new System.Drawing.Size(609, 522);
            this.testDescriptionTabPage.TabIndex = 4;
            this.testDescriptionTabPage.Text = "Test description";
            this.testDescriptionTabPage.UseVisualStyleBackColor = true;
            // 
            // testDescriptionTimeLineControl
            // 
            this.testDescriptionTimeLineControl.AllowDrop = true;
            this.testDescriptionTimeLineControl.AutoScroll = true;
            this.testDescriptionTimeLineControl.AutoSize = true;
            this.testDescriptionTimeLineControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.testDescriptionTimeLineControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.testDescriptionTimeLineControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testDescriptionTimeLineControl.Location = new System.Drawing.Point(0, 0);
            this.testDescriptionTimeLineControl.Name = "testDescriptionTimeLineControl";
            this.testDescriptionTimeLineControl.Size = new System.Drawing.Size(609, 522);
            this.testDescriptionTimeLineControl.SubSequence = null;
            this.testDescriptionTimeLineControl.TabIndex = 0;
            this.testDescriptionTimeLineControl.TestCase = null;
            this.testDescriptionTimeLineControl.Translation = null;
            // 
            // testExecutionTabPage
            // 
            this.testExecutionTabPage.Controls.Add(this.testExecutionTimeLineControl);
            this.testExecutionTabPage.Location = new System.Drawing.Point(4, 22);
            this.testExecutionTabPage.Margin = new System.Windows.Forms.Padding(2);
            this.testExecutionTabPage.Name = "testExecutionTabPage";
            this.testExecutionTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.testExecutionTabPage.Size = new System.Drawing.Size(609, 522);
            this.testExecutionTabPage.TabIndex = 0;
            this.testExecutionTabPage.Text = "Test execution";
            this.testExecutionTabPage.UseVisualStyleBackColor = true;
            // 
            // testExecutionTimeLineControl
            // 
            this.testExecutionTimeLineControl.AutoScroll = true;
            this.testExecutionTimeLineControl.AutoSize = true;
            this.testExecutionTimeLineControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.testExecutionTimeLineControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testExecutionTimeLineControl.Location = new System.Drawing.Point(2, 2);
            this.testExecutionTimeLineControl.Name = "testExecutionTimeLineControl";
            this.testExecutionTimeLineControl.Size = new System.Drawing.Size(605, 518);
            this.testExecutionTimeLineControl.TabIndex = 1;
            this.testExecutionTimeLineControl.Text = "evcTimeLineControl1";
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 573);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip2);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Window";
            this.ShowInTaskbar = false;
            this.Text = "Test";
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.testDescriptionTabPage.ResumeLayout(false);
            this.testDescriptionTabPage.PerformLayout();
            this.testExecutionTabPage.ResumeLayout(false);
            this.testExecutionTabPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private TimeLineControl.DynamicTimeLineControl testExecutionTimeLineControl;
        private TestTreeView testBrowserTreeView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripTextBox toolStripTimeTextBox;
        private System.Windows.Forms.ToolStripButton StepOnceButton;
        private System.Windows.Forms.ToolStripButton RestartButton;
        private System.Windows.Forms.ToolStripButton rewindButton;
        private System.Windows.Forms.ToolStripComboBox subSequenceSelectorComboBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripTextBox toolStripCurrentStepTextBox;
        private System.Windows.Forms.ToolStripButton nextInfoToolStripButton;
        private System.Windows.Forms.ToolStripButton nextWarningToolStripButton;
        private System.Windows.Forms.ToolStripButton nextErrortoolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripComboBox frameToolStripComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.TabPage testExecutionTabPage;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.TabPage testDescriptionTabPage;
        public TimeLineControl.StaticTimeLineControl testDescriptionTimeLineControl;
        private System.Windows.Forms.ToolStripButton toolStripButton4;

    }
}