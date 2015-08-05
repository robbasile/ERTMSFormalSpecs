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
namespace GUI.BoxArrowDiagram
{
    partial class BoxArrowWindow<TEnclosing, TBoxModel, TArrowModel>
        where TEnclosing : class
        where TBoxModel : class, DataDictionary.IGraphicalDisplay
        where TArrowModel : class, DataDictionary.IGraphicalArrow<TBoxModel>
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
            this.BoxArrowContainerPanel = CreatePanel();
            this.SuspendLayout();
            // 
            // StateContainerPanel
            // 
            this.Controls.Add(this.BoxArrowContainerPanel); 
            this.BoxArrowContainerPanel.AutoScroll = true;
            this.BoxArrowContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BoxArrowContainerPanel.Location = new System.Drawing.Point(0, 0);
            this.BoxArrowContainerPanel.Margin = new System.Windows.Forms.Padding(2);
            this.BoxArrowContainerPanel.Name = "StateContainerPanel";
            this.BoxArrowContainerPanel.Size = new System.Drawing.Size(403, 341);
            this.BoxArrowContainerPanel.TabIndex = 0;
            // 
            // StateDiagramWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 341);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "StateDiagramWindow";
            this.Text = "StateDiagramWindow";
            this.ResumeLayout(false);
        }

        #endregion

        protected BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel> BoxArrowContainerPanel;
    }
}