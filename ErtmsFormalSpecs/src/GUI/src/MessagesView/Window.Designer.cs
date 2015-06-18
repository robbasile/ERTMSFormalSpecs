namespace GUI.MessagesView
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
            this.messagesDataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.messagesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // messagesDataGridView
            // 
            this.messagesDataGridView.AllowUserToAddRows = false;
            this.messagesDataGridView.AllowUserToDeleteRows = false;
            this.messagesDataGridView.AllowUserToOrderColumns = true;
            this.messagesDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.messagesDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.messagesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.messagesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messagesDataGridView.Location = new System.Drawing.Point(0, 0);
            this.messagesDataGridView.Name = "messagesDataGridView";
            this.messagesDataGridView.ReadOnly = true;
            this.messagesDataGridView.RowHeadersVisible = false;
            this.messagesDataGridView.Size = new System.Drawing.Size(1004, 165);
            this.messagesDataGridView.TabIndex = 0;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 165);
            this.Controls.Add(this.messagesDataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Window";
            this.Text = "Messages";
            ((System.ComponentModel.ISupportInitialize)(this.messagesDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView messagesDataGridView;
    }
}