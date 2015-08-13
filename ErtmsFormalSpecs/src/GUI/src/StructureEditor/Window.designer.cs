namespace GUI.StructureEditor
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
            this.structureTreeListView = new BrightIdeasSoftware.TreeListView();
            this.fieldColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.valueColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.actualValueColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.descriptionColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            ((System.ComponentModel.ISupportInitialize)(this.structureTreeListView)).BeginInit();
            this.SuspendLayout();
            // 
            // structureTreeListView
            // 
            this.structureTreeListView.AllColumns.Add(this.fieldColumn);
            this.structureTreeListView.AllColumns.Add(this.valueColumn);
            this.structureTreeListView.AllColumns.Add(this.actualValueColumn);
            this.structureTreeListView.AllColumns.Add(this.descriptionColumn);
            this.structureTreeListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.structureTreeListView.CellEditEnterChangesRows = true;
            this.structureTreeListView.CellEditTabChangesRows = true;
            this.structureTreeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fieldColumn,
            this.valueColumn,
            this.actualValueColumn,
            this.descriptionColumn});
            this.structureTreeListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.structureTreeListView.Location = new System.Drawing.Point(0, 0);
            this.structureTreeListView.Name = "structureTreeListView";
            this.structureTreeListView.OwnerDraw = true;
            this.structureTreeListView.ShowGroups = false;
            this.structureTreeListView.Size = new System.Drawing.Size(1100, 446);
            this.structureTreeListView.TabIndex = 0;
            this.structureTreeListView.UseCellFormatEvents = true;
            this.structureTreeListView.UseCompatibleStateImageBehavior = false;
            this.structureTreeListView.View = System.Windows.Forms.View.Details;
            this.structureTreeListView.VirtualMode = true;
            // 
            // fieldColumn
            // 
            this.fieldColumn.AspectName = "";
            this.fieldColumn.CellPadding = null;
            this.fieldColumn.IsEditable = false;
            this.fieldColumn.Text = "Field name";
            this.fieldColumn.ToolTipText = "The name of the field";
            this.fieldColumn.Width = 180;
            // 
            // valueColumn
            // 
            this.valueColumn.AspectName = "";
            this.valueColumn.CellPadding = null;
            this.valueColumn.Text = "Value";
            this.valueColumn.Width = 180;
            // 
            // actualValueColumn
            // 
            this.actualValueColumn.CellPadding = null;
            this.actualValueColumn.IsEditable = false;
            this.actualValueColumn.Text = "Actual value";
            this.actualValueColumn.Width = 180;
            // 
            // descriptionColumn
            // 
            this.descriptionColumn.AspectName = "";
            this.descriptionColumn.CellPadding = null;
            this.descriptionColumn.FillsFreeSpace = true;
            this.descriptionColumn.IsEditable = false;
            this.descriptionColumn.Text = "Description";
            this.descriptionColumn.Width = 400;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 446);
            this.Controls.Add(this.structureTreeListView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Window";
            this.Text = "Structure Editor";
            ((System.ComponentModel.ISupportInitialize)(this.structureTreeListView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.TreeListView structureTreeListView;
        private BrightIdeasSoftware.OLVColumn fieldColumn;
        private BrightIdeasSoftware.OLVColumn valueColumn;
        private BrightIdeasSoftware.OLVColumn descriptionColumn;
        private BrightIdeasSoftware.OLVColumn actualValueColumn;
    }
}