namespace GUI.Report
{
    partial class ERTMSAcademyReport
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
            this.Lbl_UserName = new System.Windows.Forms.Label();
            this.Cbb_UserNames = new System.Windows.Forms.ComboBox();
            this.TxtB_Path = new System.Windows.Forms.TextBox();
            this.Btn_CreateReport = new System.Windows.Forms.Button();
            this.Btn_Browse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.sinceUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.sinceUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // Lbl_UserName
            // 
            this.Lbl_UserName.AutoSize = true;
            this.Lbl_UserName.Location = new System.Drawing.Point(13, 13);
            this.Lbl_UserName.Name = "Lbl_UserName";
            this.Lbl_UserName.Size = new System.Drawing.Size(61, 13);
            this.Lbl_UserName.TabIndex = 0;
            this.Lbl_UserName.Text = "User name:";
            // 
            // Cbb_UserNames
            // 
            this.Cbb_UserNames.FormattingEnabled = true;
            this.Cbb_UserNames.Location = new System.Drawing.Point(98, 10);
            this.Cbb_UserNames.Name = "Cbb_UserNames";
            this.Cbb_UserNames.Size = new System.Drawing.Size(216, 21);
            this.Cbb_UserNames.TabIndex = 1;
            // 
            // TxtB_Path
            // 
            this.TxtB_Path.Location = new System.Drawing.Point(12, 77);
            this.TxtB_Path.Name = "TxtB_Path";
            this.TxtB_Path.Size = new System.Drawing.Size(245, 20);
            this.TxtB_Path.TabIndex = 2;
            // 
            // Btn_CreateReport
            // 
            this.Btn_CreateReport.Location = new System.Drawing.Point(344, 75);
            this.Btn_CreateReport.Name = "Btn_CreateReport";
            this.Btn_CreateReport.Size = new System.Drawing.Size(79, 23);
            this.Btn_CreateReport.TabIndex = 3;
            this.Btn_CreateReport.Text = "Create report";
            this.Btn_CreateReport.UseVisualStyleBackColor = true;
            this.Btn_CreateReport.Click += new System.EventHandler(this.Btn_CreateReport_Click);
            // 
            // Btn_Browse
            // 
            this.Btn_Browse.Location = new System.Drawing.Point(263, 75);
            this.Btn_Browse.Name = "Btn_Browse";
            this.Btn_Browse.Size = new System.Drawing.Size(75, 23);
            this.Btn_Browse.TabIndex = 4;
            this.Btn_Browse.Text = "Browse...";
            this.Btn_Browse.UseVisualStyleBackColor = true;
            this.Btn_Browse.Click += new System.EventHandler(this.Btn_Browse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Since ";
            // 
            // sinceUpDown
            // 
            this.sinceUpDown.Location = new System.Drawing.Point(98, 42);
            this.sinceUpDown.Name = "sinceUpDown";
            this.sinceUpDown.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.sinceUpDown.Size = new System.Drawing.Size(71, 20);
            this.sinceUpDown.TabIndex = 6;
            this.sinceUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.sinceUpDown.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(175, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "days";
            // 
            // ERTMSAcademyReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 109);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sinceUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Btn_Browse);
            this.Controls.Add(this.Btn_CreateReport);
            this.Controls.Add(this.TxtB_Path);
            this.Controls.Add(this.Cbb_UserNames);
            this.Controls.Add(this.Lbl_UserName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ERTMSAcademyReport";
            this.Text = "ERTMS Academy report";
            ((System.ComponentModel.ISupportInitialize)(this.sinceUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Lbl_UserName;
        private System.Windows.Forms.ComboBox Cbb_UserNames;
        private System.Windows.Forms.TextBox TxtB_Path;
        private System.Windows.Forms.Button Btn_CreateReport;
        private System.Windows.Forms.Button Btn_Browse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown sinceUpDown;
        private System.Windows.Forms.Label label2;
    }
}