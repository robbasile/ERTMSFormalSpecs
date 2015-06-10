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
    partial class FindingsReport
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
            this.GrB_Options = new System.Windows.Forms.GroupBox();
            this.CB_NotReviewed = new System.Windows.Forms.CheckBox();
            this.CB_Reviewed = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CB_ShowBugs = new System.Windows.Forms.CheckBox();
            this.CB_ShowComments = new System.Windows.Forms.CheckBox();
            this.CB_ShowQuestions = new System.Windows.Forms.CheckBox();
            this.TxtB_Path = new System.Windows.Forms.TextBox();
            this.Btn_SelectFile = new System.Windows.Forms.Button();
            this.Btn_CreateReport = new System.Windows.Forms.Button();
            this.GrB_Options.SuspendLayout();
            this.SuspendLayout();
            // 
            // GrB_Options
            // 
            this.GrB_Options.Controls.Add(this.CB_NotReviewed);
            this.GrB_Options.Controls.Add(this.CB_Reviewed);
            this.GrB_Options.Controls.Add(this.label1);
            this.GrB_Options.Controls.Add(this.CB_ShowBugs);
            this.GrB_Options.Controls.Add(this.CB_ShowComments);
            this.GrB_Options.Controls.Add(this.CB_ShowQuestions);
            this.GrB_Options.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GrB_Options.Location = new System.Drawing.Point(13, 13);
            this.GrB_Options.Name = "GrB_Options";
            this.GrB_Options.Size = new System.Drawing.Size(388, 126);
            this.GrB_Options.TabIndex = 0;
            this.GrB_Options.TabStop = false;
            this.GrB_Options.Text = "Options";
            // 
            // CB_NotReviewed
            // 
            this.CB_NotReviewed.AutoSize = true;
            this.CB_NotReviewed.Checked = true;
            this.CB_NotReviewed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_NotReviewed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.CB_NotReviewed.Location = new System.Drawing.Point(208, 66);
            this.CB_NotReviewed.Name = "CB_NotReviewed";
            this.CB_NotReviewed.Size = new System.Drawing.Size(89, 17);
            this.CB_NotReviewed.TabIndex = 5;
            this.CB_NotReviewed.Text = "Not reviewed";
            this.CB_NotReviewed.UseVisualStyleBackColor = true;
            // 
            // CB_Reviewed
            // 
            this.CB_Reviewed.AutoSize = true;
            this.CB_Reviewed.Checked = true;
            this.CB_Reviewed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_Reviewed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.CB_Reviewed.Location = new System.Drawing.Point(208, 43);
            this.CB_Reviewed.Name = "CB_Reviewed";
            this.CB_Reviewed.Size = new System.Drawing.Size(74, 17);
            this.CB_Reviewed.TabIndex = 4;
            this.CB_Reviewed.Text = "Reviewed";
            this.CB_Reviewed.UseVisualStyleBackColor = true;
            this.CB_Reviewed.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(196, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Include Findings";
            // 
            // CB_ShowBugs
            // 
            this.CB_ShowBugs.AutoSize = true;
            this.CB_ShowBugs.Checked = true;
            this.CB_ShowBugs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_ShowBugs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_ShowBugs.Location = new System.Drawing.Point(6, 66);
            this.CB_ShowBugs.Name = "CB_ShowBugs";
            this.CB_ShowBugs.Size = new System.Drawing.Size(50, 17);
            this.CB_ShowBugs.TabIndex = 2;
            this.CB_ShowBugs.Text = "Bugs";
            this.CB_ShowBugs.UseVisualStyleBackColor = true;
            // 
            // CB_ShowComments
            // 
            this.CB_ShowComments.AutoSize = true;
            this.CB_ShowComments.Checked = true;
            this.CB_ShowComments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_ShowComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_ShowComments.Location = new System.Drawing.Point(6, 43);
            this.CB_ShowComments.Name = "CB_ShowComments";
            this.CB_ShowComments.Size = new System.Drawing.Size(75, 17);
            this.CB_ShowComments.TabIndex = 1;
            this.CB_ShowComments.Text = "Comments";
            this.CB_ShowComments.UseVisualStyleBackColor = true;
            // 
            // CB_ShowQuestions
            // 
            this.CB_ShowQuestions.AutoSize = true;
            this.CB_ShowQuestions.Checked = true;
            this.CB_ShowQuestions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_ShowQuestions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_ShowQuestions.Location = new System.Drawing.Point(6, 20);
            this.CB_ShowQuestions.Name = "CB_ShowQuestions";
            this.CB_ShowQuestions.Size = new System.Drawing.Size(73, 17);
            this.CB_ShowQuestions.TabIndex = 0;
            this.CB_ShowQuestions.Text = "Questions";
            this.CB_ShowQuestions.UseVisualStyleBackColor = true;
            // 
            // TxtB_Path
            // 
            this.TxtB_Path.Location = new System.Drawing.Point(12, 148);
            this.TxtB_Path.Name = "TxtB_Path";
            this.TxtB_Path.Size = new System.Drawing.Size(200, 20);
            this.TxtB_Path.TabIndex = 7;
            // 
            // Btn_SelectFile
            // 
            this.Btn_SelectFile.Location = new System.Drawing.Point(221, 145);
            this.Btn_SelectFile.Name = "Btn_SelectFile";
            this.Btn_SelectFile.Size = new System.Drawing.Size(87, 23);
            this.Btn_SelectFile.TabIndex = 6;
            this.Btn_SelectFile.Text = "Browse...";
            this.Btn_SelectFile.UseVisualStyleBackColor = true;
            this.Btn_SelectFile.Click += new System.EventHandler(this.Btn_SelectFile_Click);
            // 
            // Btn_CreateReport
            // 
            this.Btn_CreateReport.Location = new System.Drawing.Point(314, 145);
            this.Btn_CreateReport.Name = "Btn_CreateReport";
            this.Btn_CreateReport.Size = new System.Drawing.Size(87, 23);
            this.Btn_CreateReport.TabIndex = 5;
            this.Btn_CreateReport.Text = "Create report";
            this.Btn_CreateReport.UseVisualStyleBackColor = true;
            this.Btn_CreateReport.Click += new System.EventHandler(this.Btn_CreateReport_Click);
            // 
            // FindingsReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 180);
            this.Controls.Add(this.Btn_CreateReport);
            this.Controls.Add(this.Btn_SelectFile);
            this.Controls.Add(this.TxtB_Path);
            this.Controls.Add(this.GrB_Options);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FindingsReport";
            this.ShowInTaskbar = false;
            this.Text = "Findings report options";
            this.GrB_Options.ResumeLayout(false);
            this.GrB_Options.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox GrB_Options;
        private System.Windows.Forms.CheckBox CB_ShowBugs;
        private System.Windows.Forms.CheckBox CB_ShowComments;
        private System.Windows.Forms.CheckBox CB_ShowQuestions;
        private System.Windows.Forms.TextBox TxtB_Path;
        private System.Windows.Forms.Button Btn_SelectFile;
        private System.Windows.Forms.Button Btn_CreateReport;
        private System.Windows.Forms.CheckBox CB_NotReviewed;
        private System.Windows.Forms.CheckBox CB_Reviewed;
        private System.Windows.Forms.Label label1;


    }
}