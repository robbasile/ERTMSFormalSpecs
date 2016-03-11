// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using GUIUtils.GraphVisualization;

namespace GUI.GraphView
{
    partial class GraphView
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphView));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.GraphVisualiser = new GUIUtils.GraphVisualization.GraphVisualizer();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.Gb_GradientAxis = new System.Windows.Forms.GroupBox();
            this.Lbl_MinGrad = new System.Windows.Forms.Label();
            this.Tb_MinGrad = new System.Windows.Forms.TextBox();
            this.Tb_MaxGrad = new System.Windows.Forms.TextBox();
            this.Lbl_MaxGrad = new System.Windows.Forms.Label();
            this.Gb_YAxis = new System.Windows.Forms.GroupBox();
            this.Cb_AutoYSize = new System.Windows.Forms.CheckBox();
            this.Tb_MaxY = new System.Windows.Forms.TextBox();
            this.Lbl_MaxY = new System.Windows.Forms.Label();
            this.Gb_XAsix = new System.Windows.Forms.GroupBox();
            this.Tb_MaxX = new System.Windows.Forms.TextBox();
            this.Lbl_MinX = new System.Windows.Forms.Label();
            this.Lbl_MaxX = new System.Windows.Forms.Label();
            this.Tb_MinX = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GraphVisualiser)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.Gb_GradientAxis.SuspendLayout();
            this.Gb_YAxis.SuspendLayout();
            this.Gb_XAsix.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(772, 476);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GraphVisualiser_MouseWheel);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.GraphVisualiser);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(764, 450);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Graph";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // GraphVisualiser
            // 
            chartArea1.Name = "ChartArea";
            this.GraphVisualiser.ChartAreas.Add(chartArea1);
            this.GraphVisualiser.DecelerationCurvePrecision = 51;
            this.GraphVisualiser.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend";
            this.GraphVisualiser.Legends.Add(legend1);
            this.GraphVisualiser.Location = new System.Drawing.Point(3, 3);
            this.GraphVisualiser.Name = "GraphVisualiser";
            this.GraphVisualiser.RecordPreviousValuesInTsm = false;
            this.GraphVisualiser.Size = new System.Drawing.Size(758, 444);
            this.GraphVisualiser.TabIndex = 0;
            this.GraphVisualiser.Text = "graphVisualizer1";
            this.GraphVisualiser.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GraphVisualiser_MouseWheel);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.Gb_GradientAxis);
            this.tabPage2.Controls.Add(this.Gb_YAxis);
            this.tabPage2.Controls.Add(this.Gb_XAsix);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(764, 450);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Properties";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // Gb_GradientAxis
            // 
            this.Gb_GradientAxis.Controls.Add(this.Lbl_MinGrad);
            this.Gb_GradientAxis.Controls.Add(this.Tb_MinGrad);
            this.Gb_GradientAxis.Controls.Add(this.Tb_MaxGrad);
            this.Gb_GradientAxis.Controls.Add(this.Lbl_MaxGrad);
            this.Gb_GradientAxis.Location = new System.Drawing.Point(3, 183);
            this.Gb_GradientAxis.Name = "Gb_GradientAxis";
            this.Gb_GradientAxis.Size = new System.Drawing.Size(245, 78);
            this.Gb_GradientAxis.TabIndex = 9;
            this.Gb_GradientAxis.TabStop = false;
            this.Gb_GradientAxis.Text = "Gradient axis";
            // 
            // Lbl_MinGrad
            // 
            this.Lbl_MinGrad.AutoSize = true;
            this.Lbl_MinGrad.Location = new System.Drawing.Point(6, 22);
            this.Lbl_MinGrad.Name = "Lbl_MinGrad";
            this.Lbl_MinGrad.Size = new System.Drawing.Size(77, 13);
            this.Lbl_MinGrad.TabIndex = 9;
            this.Lbl_MinGrad.Text = "Minimum value";
            // 
            // Tb_MinGrad
            // 
            this.Tb_MinGrad.Location = new System.Drawing.Point(92, 19);
            this.Tb_MinGrad.Name = "Tb_MinGrad";
            this.Tb_MinGrad.Size = new System.Drawing.Size(138, 20);
            this.Tb_MinGrad.TabIndex = 10;
            this.Tb_MinGrad.Text = "-2.5";
            this.Tb_MinGrad.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // Tb_MaxGrad
            // 
            this.Tb_MaxGrad.Location = new System.Drawing.Point(92, 45);
            this.Tb_MaxGrad.Name = "Tb_MaxGrad";
            this.Tb_MaxGrad.Size = new System.Drawing.Size(138, 20);
            this.Tb_MaxGrad.TabIndex = 6;
            this.Tb_MaxGrad.Text = "2.5";
            this.Tb_MaxGrad.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // Lbl_MaxGrad
            // 
            this.Lbl_MaxGrad.AutoSize = true;
            this.Lbl_MaxGrad.Location = new System.Drawing.Point(6, 48);
            this.Lbl_MaxGrad.Name = "Lbl_MaxGrad";
            this.Lbl_MaxGrad.Size = new System.Drawing.Size(80, 13);
            this.Lbl_MaxGrad.TabIndex = 6;
            this.Lbl_MaxGrad.Text = "Maximum value";
            // 
            // Gb_YAxis
            // 
            this.Gb_YAxis.Controls.Add(this.Cb_AutoYSize);
            this.Gb_YAxis.Controls.Add(this.Tb_MaxY);
            this.Gb_YAxis.Controls.Add(this.Lbl_MaxY);
            this.Gb_YAxis.Location = new System.Drawing.Point(3, 95);
            this.Gb_YAxis.Name = "Gb_YAxis";
            this.Gb_YAxis.Size = new System.Drawing.Size(245, 82);
            this.Gb_YAxis.TabIndex = 7;
            this.Gb_YAxis.TabStop = false;
            this.Gb_YAxis.Text = "Y axis";
            // 
            // Cb_AutoYSize
            // 
            this.Cb_AutoYSize.AutoSize = true;
            this.Cb_AutoYSize.Location = new System.Drawing.Point(9, 20);
            this.Cb_AutoYSize.Name = "Cb_AutoYSize";
            this.Cb_AutoYSize.Size = new System.Drawing.Size(175, 17);
            this.Cb_AutoYSize.TabIndex = 8;
            this.Cb_AutoYSize.Text = "Compute max size automatically";
            this.Cb_AutoYSize.UseVisualStyleBackColor = true;
            this.Cb_AutoYSize.CheckedChanged += new System.EventHandler(this.Cb_AutoYSize_CheckedChanged);
            // 
            // Tb_MaxY
            // 
            this.Tb_MaxY.Location = new System.Drawing.Point(92, 44);
            this.Tb_MaxY.Name = "Tb_MaxY";
            this.Tb_MaxY.Size = new System.Drawing.Size(138, 20);
            this.Tb_MaxY.TabIndex = 6;
            this.Tb_MaxY.Text = "200";
            this.Tb_MaxY.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // Lbl_MaxY
            // 
            this.Lbl_MaxY.AutoSize = true;
            this.Lbl_MaxY.Location = new System.Drawing.Point(6, 47);
            this.Lbl_MaxY.Name = "Lbl_MaxY";
            this.Lbl_MaxY.Size = new System.Drawing.Size(80, 13);
            this.Lbl_MaxY.TabIndex = 6;
            this.Lbl_MaxY.Text = "Maximum value";
            // 
            // Gb_XAsix
            // 
            this.Gb_XAsix.Controls.Add(this.Tb_MaxX);
            this.Gb_XAsix.Controls.Add(this.Lbl_MinX);
            this.Gb_XAsix.Controls.Add(this.Lbl_MaxX);
            this.Gb_XAsix.Controls.Add(this.Tb_MinX);
            this.Gb_XAsix.Location = new System.Drawing.Point(3, 6);
            this.Gb_XAsix.Name = "Gb_XAsix";
            this.Gb_XAsix.Size = new System.Drawing.Size(245, 83);
            this.Gb_XAsix.TabIndex = 6;
            this.Gb_XAsix.TabStop = false;
            this.Gb_XAsix.Text = "X axis";
            // 
            // Tb_MaxX
            // 
            this.Tb_MaxX.Location = new System.Drawing.Point(94, 45);
            this.Tb_MaxX.Name = "Tb_MaxX";
            this.Tb_MaxX.Size = new System.Drawing.Size(138, 20);
            this.Tb_MaxX.TabIndex = 5;
            this.Tb_MaxX.Text = "5000";
            this.Tb_MaxX.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // Lbl_MinX
            // 
            this.Lbl_MinX.AutoSize = true;
            this.Lbl_MinX.Location = new System.Drawing.Point(8, 22);
            this.Lbl_MinX.Name = "Lbl_MinX";
            this.Lbl_MinX.Size = new System.Drawing.Size(77, 13);
            this.Lbl_MinX.TabIndex = 1;
            this.Lbl_MinX.Text = "Minimum value";
            // 
            // Lbl_MaxX
            // 
            this.Lbl_MaxX.AutoSize = true;
            this.Lbl_MaxX.Location = new System.Drawing.Point(8, 48);
            this.Lbl_MaxX.Name = "Lbl_MaxX";
            this.Lbl_MaxX.Size = new System.Drawing.Size(80, 13);
            this.Lbl_MaxX.TabIndex = 4;
            this.Lbl_MaxX.Text = "Maximum value";
            // 
            // Tb_MinX
            // 
            this.Tb_MinX.Location = new System.Drawing.Point(94, 19);
            this.Tb_MinX.Name = "Tb_MinX";
            this.Tb_MinX.Size = new System.Drawing.Size(138, 20);
            this.Tb_MinX.TabIndex = 2;
            this.Tb_MinX.Text = "0";
            this.Tb_MinX.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Maximum value";
            // 
            // GraphView
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 476);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GraphView";
            this.ShowInTaskbar = false;
            this.Text = "Graph View";
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GraphVisualiser_MouseWheel);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GraphVisualiser)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.Gb_GradientAxis.ResumeLayout(false);
            this.Gb_GradientAxis.PerformLayout();
            this.Gb_YAxis.ResumeLayout(false);
            this.Gb_YAxis.PerformLayout();
            this.Gb_XAsix.ResumeLayout(false);
            this.Gb_XAsix.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox Tb_MaxX;
        private System.Windows.Forms.Label Lbl_MaxX;
        private System.Windows.Forms.TextBox Tb_MinX;
        private System.Windows.Forms.Label Lbl_MinX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox Gb_XAsix;
        private global::GUIUtils.GraphVisualization.GraphVisualizer GraphVisualiser;
        private System.Windows.Forms.GroupBox Gb_YAxis;
        private System.Windows.Forms.TextBox Tb_MaxY;
        private System.Windows.Forms.Label Lbl_MaxY;
        private System.Windows.Forms.CheckBox Cb_AutoYSize;
        private System.Windows.Forms.GroupBox Gb_GradientAxis;
        private System.Windows.Forms.Label Lbl_MinGrad;
        private System.Windows.Forms.TextBox Tb_MinGrad;
        private System.Windows.Forms.TextBox Tb_MaxGrad;
        private System.Windows.Forms.Label Lbl_MaxGrad;
    }
}