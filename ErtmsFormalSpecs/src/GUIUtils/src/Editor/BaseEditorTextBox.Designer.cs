namespace GUIUtils.Editor
{
    partial class BaseEditorTextBox 
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SelectionComboBox = new System.Windows.Forms.ComboBox();
            this.EditionTextBox = new GUIUtils.Editor.SyntaxRichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.explainRichTextBox = new GUIUtils.Editor.SyntaxRichTextBox();
            this.indentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SelectionComboBox
            // 
            this.SelectionComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.SelectionComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.SelectionComboBox.FormattingEnabled = true;
            this.SelectionComboBox.Location = new System.Drawing.Point(76, 3);
            this.SelectionComboBox.Name = "SelectionComboBox";
            this.SelectionComboBox.Size = new System.Drawing.Size(236, 21);
            this.SelectionComboBox.Sorted = true;
            this.SelectionComboBox.TabIndex = 1;
            this.SelectionComboBox.Visible = false;
            // 
            // EditionTextBox
            // 
            this.EditionTextBox.ApplyPatterns = true;
            this.EditionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditionTextBox.Instance = null;
            this.EditionTextBox.Location = new System.Drawing.Point(0, 0);
            this.EditionTextBox.Name = "EditionTextBox";
            this.EditionTextBox.Size = new System.Drawing.Size(330, 162);
            this.EditionTextBox.TabIndex = 0;
            this.EditionTextBox.Text = "";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator1,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator2,
            this.indentToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 170);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // explainRichTextBox
            // 
            this.explainRichTextBox.ApplyPatterns = true;
            this.explainRichTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.explainRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.explainRichTextBox.Instance = null;
            this.explainRichTextBox.Location = new System.Drawing.Point(76, 30);
            this.explainRichTextBox.Name = "explainRichTextBox";
            this.explainRichTextBox.ReadOnly = true;
            this.explainRichTextBox.Size = new System.Drawing.Size(400, 200);
            this.explainRichTextBox.TabIndex = 2;
            this.explainRichTextBox.Text = "";
            this.explainRichTextBox.Visible = false;
            // 
            // indentToolStripMenuItem
            // 
            this.indentToolStripMenuItem.Name = "indentToolStripMenuItem";
            this.indentToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.indentToolStripMenuItem.Text = "Indent";
            this.indentToolStripMenuItem.Click += new System.EventHandler(this.indentToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // BaseEditorTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.explainRichTextBox);
            this.Controls.Add(this.SelectionComboBox);
            this.Controls.Add(this.EditionTextBox);
            this.Name = "BaseEditorTextBox";
            this.Size = new System.Drawing.Size(330, 162);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox SelectionComboBox;
        protected System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private SyntaxRichTextBox explainRichTextBox;
        public SyntaxRichTextBox EditionTextBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem indentToolStripMenuItem;
    }
}
