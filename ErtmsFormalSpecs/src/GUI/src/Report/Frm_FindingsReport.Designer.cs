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
            this.TxtB_Path = new System.Windows.Forms.TextBox();
            this.Btn_SelectFile = new System.Windows.Forms.Button();
            this.Btn_CreateReport = new System.Windows.Forms.Button();
            this.detailCheckBox = new System.Windows.Forms.CheckBox();
            this.subSequenceCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // TxtB_Path
            // 
            this.TxtB_Path.Location = new System.Drawing.Point(12, 12);
            this.TxtB_Path.Name = "TxtB_Path";
            this.TxtB_Path.Size = new System.Drawing.Size(200, 20);
            this.TxtB_Path.TabIndex = 7;
            // 
            // Btn_SelectFile
            // 
            this.Btn_SelectFile.Location = new System.Drawing.Point(221, 9);
            this.Btn_SelectFile.Name = "Btn_SelectFile";
            this.Btn_SelectFile.Size = new System.Drawing.Size(87, 23);
            this.Btn_SelectFile.TabIndex = 6;
            this.Btn_SelectFile.Text = "Browse...";
            this.Btn_SelectFile.UseVisualStyleBackColor = true;
            this.Btn_SelectFile.Click += new System.EventHandler(this.Btn_SelectFile_Click);
            // 
            // Btn_CreateReport
            // 
            this.Btn_CreateReport.Location = new System.Drawing.Point(314, 9);
            this.Btn_CreateReport.Name = "Btn_CreateReport";
            this.Btn_CreateReport.Size = new System.Drawing.Size(87, 23);
            this.Btn_CreateReport.TabIndex = 5;
            this.Btn_CreateReport.Text = "Create report";
            this.Btn_CreateReport.UseVisualStyleBackColor = true;
            this.Btn_CreateReport.Click += new System.EventHandler(this.Btn_CreateReport_Click);
            // 
            // detailCheckBox
            // 
            this.detailCheckBox.AutoSize = true;
            this.detailCheckBox.Location = new System.Drawing.Point(13, 39);
            this.detailCheckBox.Name = "detailCheckBox";
            this.detailCheckBox.Size = new System.Drawing.Size(94, 17);
            this.detailCheckBox.TabIndex = 8;
            this.detailCheckBox.Text = "Include details";
            this.detailCheckBox.UseVisualStyleBackColor = true;
            // 
            // subSequenceCheckBox
            // 
            this.subSequenceCheckBox.AutoSize = true;
            this.subSequenceCheckBox.Location = new System.Drawing.Point(13, 62);
            this.subSequenceCheckBox.Name = "subSequenceCheckBox";
            this.subSequenceCheckBox.Size = new System.Drawing.Size(136, 17);
            this.subSequenceCheckBox.TabIndex = 9;
            this.subSequenceCheckBox.Text = "Include sub sequences";
            this.subSequenceCheckBox.UseVisualStyleBackColor = true;
            // 
            // FindingsReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 98);
            this.Controls.Add(this.subSequenceCheckBox);
            this.Controls.Add(this.detailCheckBox);
            this.Controls.Add(this.Btn_CreateReport);
            this.Controls.Add(this.Btn_SelectFile);
            this.Controls.Add(this.TxtB_Path);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FindingsReport";
            this.ShowInTaskbar = false;
            this.Text = "Findings report options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TxtB_Path;
        private System.Windows.Forms.Button Btn_SelectFile;
        private System.Windows.Forms.Button Btn_CreateReport;
        private System.Windows.Forms.CheckBox detailCheckBox;
        private System.Windows.Forms.CheckBox subSequenceCheckBox;


    }
}