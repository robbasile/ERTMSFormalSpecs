namespace EFSDriver
{
    partial class EFSDriver
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
            this.variableNameLabel = new System.Windows.Forms.Label();
            this.variableNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.variableValueTextBox = new System.Windows.Forms.TextBox();
            this.cycleCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // variableNameLabel
            // 
            this.variableNameLabel.AutoSize = true;
            this.variableNameLabel.Location = new System.Drawing.Point(12, 16);
            this.variableNameLabel.Name = "variableNameLabel";
            this.variableNameLabel.Size = new System.Drawing.Size(76, 13);
            this.variableNameLabel.TabIndex = 1;
            this.variableNameLabel.Text = "Variable Name";
            // 
            // variableNameTextBox
            // 
            this.variableNameTextBox.Location = new System.Drawing.Point(134, 13);
            this.variableNameTextBox.Name = "variableNameTextBox";
            this.variableNameTextBox.Size = new System.Drawing.Size(339, 20);
            this.variableNameTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Variable Value";
            // 
            // variableValueTextBox
            // 
            this.variableValueTextBox.Location = new System.Drawing.Point(134, 39);
            this.variableValueTextBox.Name = "variableValueTextBox";
            this.variableValueTextBox.Size = new System.Drawing.Size(339, 20);
            this.variableValueTextBox.TabIndex = 4;
            this.variableValueTextBox.TextChanged += new System.EventHandler(this.variableValueTextBox_TextChanged);
            // 
            // cycleCheckBox
            // 
            this.cycleCheckBox.AutoSize = true;
            this.cycleCheckBox.Location = new System.Drawing.Point(421, 65);
            this.cycleCheckBox.Name = "cycleCheckBox";
            this.cycleCheckBox.Size = new System.Drawing.Size(52, 17);
            this.cycleCheckBox.TabIndex = 5;
            this.cycleCheckBox.Text = "Cycle";
            this.cycleCheckBox.UseVisualStyleBackColor = true;
            // 
            // EFSDriver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 101);
            this.Controls.Add(this.cycleCheckBox);
            this.Controls.Add(this.variableValueTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.variableNameTextBox);
            this.Controls.Add(this.variableNameLabel);
            this.Name = "EFSDriver";
            this.Text = "EFSDriver";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label variableNameLabel;
        private System.Windows.Forms.TextBox variableNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox variableValueTextBox;
        private System.Windows.Forms.CheckBox cycleCheckBox;
    }
}

