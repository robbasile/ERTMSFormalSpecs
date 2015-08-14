namespace GUI.EditorView
{
    partial class EditorTextBox
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
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.openStructureEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator2,
            this.openStructureEditorToolStripMenuItem});
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(184, 6);
            // 
            // openStructureEditorToolStripMenuItem
            // 
            this.openStructureEditorToolStripMenuItem.Name = "openStructureEditorToolStripMenuItem";
            this.openStructureEditorToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.openStructureEditorToolStripMenuItem.Text = "Open structure editor";
            this.openStructureEditorToolStripMenuItem.Click += new System.EventHandler(this.openStructureEditorToolStripMenuItem_Click);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem openStructureEditorToolStripMenuItem;
    }
}
