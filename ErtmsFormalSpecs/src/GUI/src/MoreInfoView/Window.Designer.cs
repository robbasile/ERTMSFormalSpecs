namespace GUI.MoreInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.moreInfoRichTextBox = new GUI.EditorTextBox();
            this.SuspendLayout();
            // 
            // moreInfoRichTextBox
            // 
            this.moreInfoRichTextBox.AutoComplete = true;
            this.moreInfoRichTextBox.ConsiderOnlyTypes = false;
            this.moreInfoRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.moreInfoRichTextBox.Instance = null;
            this.moreInfoRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.moreInfoRichTextBox.Name = "moreInfoRichTextBox";
            this.moreInfoRichTextBox.ReadOnly = true;
            this.moreInfoRichTextBox.Rtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang2060{\\fonttbl{\\f0\\fnil\\fcharset0 Microsoft S" +
    "ans Serif;}}\r\n\\viewkind4\\uc1\\pard\\f0\\fs17\\par\r\n}\r\n";
            this.moreInfoRichTextBox.Size = new System.Drawing.Size(1004, 165);
            this.moreInfoRichTextBox.TabIndex = 0;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 165);
            this.Controls.Add(this.moreInfoRichTextBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Window";
            this.Text = "More info";
            this.ResumeLayout(false);

        }

        #endregion

        private EditorTextBox moreInfoRichTextBox;

    }
}