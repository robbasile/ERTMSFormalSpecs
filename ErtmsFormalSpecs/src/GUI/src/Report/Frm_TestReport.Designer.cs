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
namespace GUI.Report
{
    partial class TestReport
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
            this.Btn_CreateReport = new System.Windows.Forms.Button();
            this.Btn_SelectFile = new System.Windows.Forms.Button();
            this.CB_ActivatedRulesInFrames = new System.Windows.Forms.CheckBox();
            this.CB_Frames = new System.Windows.Forms.CheckBox();
            this.CB_SubSequences = new System.Windows.Forms.CheckBox();
            this.CB_TestCases = new System.Windows.Forms.CheckBox();
            this.CB_Steps = new System.Windows.Forms.CheckBox();
            this.GrB_Filters = new System.Windows.Forms.GroupBox();
            this.Lbl_Element = new System.Windows.Forms.Label();
            this.CB_ActivatedRulesInSteps = new System.Windows.Forms.CheckBox();
            this.CB_ActivatedRulesInTestCases = new System.Windows.Forms.CheckBox();
            this.CB_ActivatedRulesInSubSequences = new System.Windows.Forms.CheckBox();
            this.Lbl_CoveredRules = new System.Windows.Forms.Label();
            this.TxtB_Path = new System.Windows.Forms.TextBox();
            this.GrB_Filters.SuspendLayout();
            this.SuspendLayout();
            // 
            // Btn_CreateReport
            // 
            this.Btn_CreateReport.Location = new System.Drawing.Point(296, 173);
            this.Btn_CreateReport.Name = "Btn_CreateReport";
            this.Btn_CreateReport.Size = new System.Drawing.Size(87, 23);
            this.Btn_CreateReport.TabIndex = 1;
            this.Btn_CreateReport.Text = "Create report";
            this.Btn_CreateReport.UseVisualStyleBackColor = true;
            this.Btn_CreateReport.Click += new System.EventHandler(this.Btn_CreateReport_Click);
            // 
            // Btn_SelectFile
            // 
            this.Btn_SelectFile.Location = new System.Drawing.Point(203, 173);
            this.Btn_SelectFile.Name = "Btn_SelectFile";
            this.Btn_SelectFile.Size = new System.Drawing.Size(87, 23);
            this.Btn_SelectFile.TabIndex = 2;
            this.Btn_SelectFile.Text = "Browse...";
            this.Btn_SelectFile.UseVisualStyleBackColor = true;
            this.Btn_SelectFile.Click += new System.EventHandler(this.Btn_SelectFile_Click);
            // 
            // CB_ActivatedRulesInFrames
            // 
            this.CB_ActivatedRulesInFrames.AutoSize = true;
            this.CB_ActivatedRulesInFrames.Location = new System.Drawing.Point(179, 59);
            this.CB_ActivatedRulesInFrames.Name = "CB_ActivatedRulesInFrames";
            this.CB_ActivatedRulesInFrames.Size = new System.Drawing.Size(15, 14);
            this.CB_ActivatedRulesInFrames.TabIndex = 1;
            this.CB_ActivatedRulesInFrames.Tag = "STAT.1";
            this.CB_ActivatedRulesInFrames.UseVisualStyleBackColor = true;
            this.CB_ActivatedRulesInFrames.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // CB_Frames
            // 
            this.CB_Frames.AutoSize = true;
            this.CB_Frames.Enabled = false;
            this.CB_Frames.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_Frames.Location = new System.Drawing.Point(9, 56);
            this.CB_Frames.Name = "CB_Frames";
            this.CB_Frames.Size = new System.Drawing.Size(60, 17);
            this.CB_Frames.TabIndex = 0;
            this.CB_Frames.Tag = "FILTER.1";
            this.CB_Frames.Text = "Frames";
            this.CB_Frames.UseVisualStyleBackColor = true;
            this.CB_Frames.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // CB_SubSequences
            // 
            this.CB_SubSequences.AutoSize = true;
            this.CB_SubSequences.Enabled = false;
            this.CB_SubSequences.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_SubSequences.Location = new System.Drawing.Point(9, 79);
            this.CB_SubSequences.Name = "CB_SubSequences";
            this.CB_SubSequences.Size = new System.Drawing.Size(100, 17);
            this.CB_SubSequences.TabIndex = 1;
            this.CB_SubSequences.Tag = "FILTER.2";
            this.CB_SubSequences.Text = "Sub sequences";
            this.CB_SubSequences.UseVisualStyleBackColor = true;
            this.CB_SubSequences.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // CB_TestCases
            // 
            this.CB_TestCases.AutoSize = true;
            this.CB_TestCases.Enabled = false;
            this.CB_TestCases.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_TestCases.Location = new System.Drawing.Point(9, 102);
            this.CB_TestCases.Name = "CB_TestCases";
            this.CB_TestCases.Size = new System.Drawing.Size(78, 17);
            this.CB_TestCases.TabIndex = 2;
            this.CB_TestCases.Tag = "FILTER.3";
            this.CB_TestCases.Text = "Test cases";
            this.CB_TestCases.UseVisualStyleBackColor = true;
            this.CB_TestCases.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // CB_Steps
            // 
            this.CB_Steps.AutoSize = true;
            this.CB_Steps.Enabled = false;
            this.CB_Steps.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_Steps.Location = new System.Drawing.Point(9, 125);
            this.CB_Steps.Name = "CB_Steps";
            this.CB_Steps.Size = new System.Drawing.Size(53, 17);
            this.CB_Steps.TabIndex = 3;
            this.CB_Steps.Tag = "FILTER.4";
            this.CB_Steps.Text = "Steps";
            this.CB_Steps.UseVisualStyleBackColor = true;
            this.CB_Steps.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // GrB_Filters
            // 
            this.GrB_Filters.Controls.Add(this.Lbl_Element);
            this.GrB_Filters.Controls.Add(this.CB_ActivatedRulesInSteps);
            this.GrB_Filters.Controls.Add(this.CB_ActivatedRulesInTestCases);
            this.GrB_Filters.Controls.Add(this.CB_Steps);
            this.GrB_Filters.Controls.Add(this.CB_ActivatedRulesInSubSequences);
            this.GrB_Filters.Controls.Add(this.CB_Frames);
            this.GrB_Filters.Controls.Add(this.CB_ActivatedRulesInFrames);
            this.GrB_Filters.Controls.Add(this.CB_TestCases);
            this.GrB_Filters.Controls.Add(this.Lbl_CoveredRules);
            this.GrB_Filters.Controls.Add(this.CB_SubSequences);
            this.GrB_Filters.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GrB_Filters.Location = new System.Drawing.Point(12, 12);
            this.GrB_Filters.Name = "GrB_Filters";
            this.GrB_Filters.Size = new System.Drawing.Size(371, 155);
            this.GrB_Filters.TabIndex = 3;
            this.GrB_Filters.TabStop = false;
            this.GrB_Filters.Text = "Filters";
            // 
            // Lbl_Element
            // 
            this.Lbl_Element.AutoSize = true;
            this.Lbl_Element.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Element.Location = new System.Drawing.Point(6, 31);
            this.Lbl_Element.Name = "Lbl_Element";
            this.Lbl_Element.Size = new System.Drawing.Size(97, 13);
            this.Lbl_Element.TabIndex = 7;
            this.Lbl_Element.Text = "Elements to display";
            // 
            // CB_ActivatedRulesInSteps
            // 
            this.CB_ActivatedRulesInSteps.AutoSize = true;
            this.CB_ActivatedRulesInSteps.Location = new System.Drawing.Point(179, 128);
            this.CB_ActivatedRulesInSteps.Name = "CB_ActivatedRulesInSteps";
            this.CB_ActivatedRulesInSteps.Size = new System.Drawing.Size(15, 14);
            this.CB_ActivatedRulesInSteps.TabIndex = 6;
            this.CB_ActivatedRulesInSteps.Tag = "STAT.4";
            this.CB_ActivatedRulesInSteps.UseVisualStyleBackColor = true;
            // 
            // CB_ActivatedRulesInTestCases
            // 
            this.CB_ActivatedRulesInTestCases.AutoSize = true;
            this.CB_ActivatedRulesInTestCases.Location = new System.Drawing.Point(179, 105);
            this.CB_ActivatedRulesInTestCases.Name = "CB_ActivatedRulesInTestCases";
            this.CB_ActivatedRulesInTestCases.Size = new System.Drawing.Size(15, 14);
            this.CB_ActivatedRulesInTestCases.TabIndex = 4;
            this.CB_ActivatedRulesInTestCases.Tag = "STAT.3";
            this.CB_ActivatedRulesInTestCases.UseVisualStyleBackColor = true;
            // 
            // CB_ActivatedRulesInSubSequences
            // 
            this.CB_ActivatedRulesInSubSequences.AutoSize = true;
            this.CB_ActivatedRulesInSubSequences.Location = new System.Drawing.Point(179, 82);
            this.CB_ActivatedRulesInSubSequences.Name = "CB_ActivatedRulesInSubSequences";
            this.CB_ActivatedRulesInSubSequences.Size = new System.Drawing.Size(15, 14);
            this.CB_ActivatedRulesInSubSequences.TabIndex = 2;
            this.CB_ActivatedRulesInSubSequences.Tag = "STAT.2";
            this.CB_ActivatedRulesInSubSequences.UseVisualStyleBackColor = true;
            // 
            // Lbl_CoveredRules
            // 
            this.Lbl_CoveredRules.AutoSize = true;
            this.Lbl_CoveredRules.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_CoveredRules.Location = new System.Drawing.Point(176, 31);
            this.Lbl_CoveredRules.Name = "Lbl_CoveredRules";
            this.Lbl_CoveredRules.Size = new System.Drawing.Size(113, 13);
            this.Lbl_CoveredRules.TabIndex = 0;
            this.Lbl_CoveredRules.Text = "Display activated rules";
            // 
            // TxtB_Path
            // 
            this.TxtB_Path.Location = new System.Drawing.Point(10, 176);
            this.TxtB_Path.Name = "TxtB_Path";
            this.TxtB_Path.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.TxtB_Path.Size = new System.Drawing.Size(186, 20);
            this.TxtB_Path.TabIndex = 4;
            this.TxtB_Path.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TestReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 207);
            this.Controls.Add(this.TxtB_Path);
            this.Controls.Add(this.GrB_Filters);
            this.Controls.Add(this.Btn_SelectFile);
            this.Controls.Add(this.Btn_CreateReport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TestReport";
            this.ShowInTaskbar = false;
            this.Text = "Test report options";
            this.TopMost = true;
            this.GrB_Filters.ResumeLayout(false);
            this.GrB_Filters.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_CreateReport;
        private System.Windows.Forms.Button Btn_SelectFile;
        private System.Windows.Forms.CheckBox CB_ActivatedRulesInFrames;
        private System.Windows.Forms.CheckBox CB_Frames;
        private System.Windows.Forms.CheckBox CB_SubSequences;
        private System.Windows.Forms.CheckBox CB_TestCases;
        private System.Windows.Forms.CheckBox CB_Steps;
        private System.Windows.Forms.GroupBox GrB_Filters;
        private System.Windows.Forms.Label Lbl_CoveredRules;
        private System.Windows.Forms.TextBox TxtB_Path;
        private System.Windows.Forms.CheckBox CB_ActivatedRulesInSteps;
        private System.Windows.Forms.CheckBox CB_ActivatedRulesInTestCases;
        private System.Windows.Forms.CheckBox CB_ActivatedRulesInSubSequences;
        private System.Windows.Forms.Label Lbl_Element;


    }
}