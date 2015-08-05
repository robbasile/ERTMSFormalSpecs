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
            this.GraphVisualiser = new GraphVisualizer();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.maximumValueTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.minimumValueTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GraphVisualiser)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(764, 450);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Properties";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.maximumValueTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.minimumValueTextBox);
            this.groupBox1.Location = new System.Drawing.Point(9, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(245, 83);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "X axis";
            // 
            // maximumValueTextBox
            // 
            this.maximumValueTextBox.Location = new System.Drawing.Point(94, 45);
            this.maximumValueTextBox.Name = "maximumValueTextBox";
            this.maximumValueTextBox.Size = new System.Drawing.Size(138, 20);
            this.maximumValueTextBox.TabIndex = 5;
            this.maximumValueTextBox.Text = "5000";
            this.maximumValueTextBox.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Minimum value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Maximum value";
            // 
            // minimumValueTextBox
            // 
            this.minimumValueTextBox.Location = new System.Drawing.Point(94, 19);
            this.minimumValueTextBox.Name = "minimumValueTextBox";
            this.minimumValueTextBox.Size = new System.Drawing.Size(138, 20);
            this.minimumValueTextBox.TabIndex = 2;
            this.minimumValueTextBox.Text = "0";
            this.minimumValueTextBox.TextChanged += new System.EventHandler(this.ValueChanged);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox maximumValueTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox minimumValueTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private global::GUIUtils.GraphVisualization.GraphVisualizer GraphVisualiser;
    }
}