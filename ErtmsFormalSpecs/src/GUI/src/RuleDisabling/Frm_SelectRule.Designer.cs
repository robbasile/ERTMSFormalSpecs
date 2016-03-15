namespace GUI.RuleDisabling
{
    partial class Frm_SelectRule
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
            this.Btn_Import = new System.Windows.Forms.Button();
            this.L_SpeedInterval = new System.Windows.Forms.Label();
            this.CBB_DisableableRuleChecks = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // Btn_Import
            // 
            this.Btn_Import.Location = new System.Drawing.Point(233, 70);
            this.Btn_Import.Margin = new System.Windows.Forms.Padding(4);
            this.Btn_Import.Name = "Btn_Import";
            this.Btn_Import.Size = new System.Drawing.Size(100, 28);
            this.Btn_Import.TabIndex = 3;
            this.Btn_Import.Text = "Select";
            this.Btn_Import.UseVisualStyleBackColor = true;
            this.Btn_Import.Click += new System.EventHandler(this.Btn_Select_Click);
            // 
            // L_SpeedInterval
            // 
            this.L_SpeedInterval.AutoSize = true;
            this.L_SpeedInterval.Location = new System.Drawing.Point(22, 28);
            this.L_SpeedInterval.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.L_SpeedInterval.Name = "L_SpeedInterval";
            this.L_SpeedInterval.Size = new System.Drawing.Size(45, 17);
            this.L_SpeedInterval.TabIndex = 18;
            this.L_SpeedInterval.Text = "Rule :";
            // 
            // CBB_DisableableRuleChecks
            // 
            this.CBB_DisableableRuleChecks.FormattingEnabled = true;
            this.CBB_DisableableRuleChecks.Location = new System.Drawing.Point(75, 25);
            this.CBB_DisableableRuleChecks.Margin = new System.Windows.Forms.Padding(4);
            this.CBB_DisableableRuleChecks.Name = "CBB_DisableableRuleChecks";
            this.CBB_DisableableRuleChecks.Size = new System.Drawing.Size(228, 24);
            this.CBB_DisableableRuleChecks.TabIndex = 17;
            // 
            // Frm_SelectRule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 111);
            this.Controls.Add(this.L_SpeedInterval);
            this.Controls.Add(this.CBB_DisableableRuleChecks);
            this.Controls.Add(this.Btn_Import);
            this.Name = "Frm_SelectRule";
            this.Text = "Frm_SelectRule";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_Import;
        private System.Windows.Forms.Label L_SpeedInterval;
        private System.Windows.Forms.ComboBox CBB_DisableableRuleChecks;

    }
}