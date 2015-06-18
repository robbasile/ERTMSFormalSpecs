namespace GUI.UsageView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.usageTreeView = new GUI.DataDictionaryView.UsageTreeView.UsageTreeView();
            this.SuspendLayout();
            // 
            // usageTreeView
            // 
            this.usageTreeView.AllowDrop = true;
            this.usageTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usageTreeView.HideSelection = false;
            this.usageTreeView.ImageIndex = 0;
            this.usageTreeView.LabelEdit = true;
            this.usageTreeView.Location = new System.Drawing.Point(0, 0);
            this.usageTreeView.Name = "usageTreeView";
            this.usageTreeView.Root = null;
            this.usageTreeView.Selected = null;
            this.usageTreeView.SelectedImageIndex = 0;
            this.usageTreeView.Size = new System.Drawing.Size(767, 240);
            this.usageTreeView.TabIndex = 0;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 240);
            this.Controls.Add(this.usageTreeView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Window";
            this.Text = "Usages";
            this.ResumeLayout(false);

        }

        #endregion

        private DataDictionaryView.UsageTreeView.UsageTreeView usageTreeView;



    }
}