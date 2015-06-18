namespace GUI.MessagesView
{
    partial class MessageDetail
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
            this.MessageDetailTextBox = new System.Windows.Forms.RichTextBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.CopyToClipBoardButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MessageDetailTextBox
            // 
            this.MessageDetailTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.MessageDetailTextBox.Location = new System.Drawing.Point(13, 13);
            this.MessageDetailTextBox.Name = "MessageDetailTextBox";
            this.MessageDetailTextBox.ReadOnly = true;
            this.MessageDetailTextBox.Size = new System.Drawing.Size(544, 153);
            this.MessageDetailTextBox.TabIndex = 0;
            this.MessageDetailTextBox.Text = "";
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(462, 172);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(95, 38);
            this.OKButton.TabIndex = 1;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CopyToClipBoardButton
            // 
            this.CopyToClipBoardButton.Location = new System.Drawing.Point(361, 172);
            this.CopyToClipBoardButton.Name = "CopyToClipBoardButton";
            this.CopyToClipBoardButton.Size = new System.Drawing.Size(95, 38);
            this.CopyToClipBoardButton.TabIndex = 2;
            this.CopyToClipBoardButton.Text = "Copy to clipboard";
            this.CopyToClipBoardButton.UseVisualStyleBackColor = true;
            this.CopyToClipBoardButton.Click += new System.EventHandler(this.CopyToClipBoardButton_Click);
            // 
            // MessageDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 222);
            this.Controls.Add(this.CopyToClipBoardButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.MessageDetailTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MessageDetail";
            this.Text = "MessageDetail";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox MessageDetailTextBox;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CopyToClipBoardButton;
    }
}