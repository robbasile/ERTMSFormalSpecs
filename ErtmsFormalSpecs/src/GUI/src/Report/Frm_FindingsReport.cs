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

using System;
using System.Windows.Forms;
using DataDictionary;
using GUIUtils;
using Reports.Specs.SubSet76;

namespace GUI.Report
{
    public partial class FindingsReport : Form
    {
        /// <summary>
        /// Th report handled
        /// </summary>
        private Subseet76ReportHandler ReportHandler { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public FindingsReport(Dictionary dictionary)
        {
            InitializeComponent();
            ReportHandler = new Subseet76ReportHandler(dictionary);
            TxtB_Path.Text = ReportHandler.FileName;
        }

        /// <summary>
        ///     Creates a report config with user's choices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_CreateReport_Click(object sender, EventArgs e)
        {
            ReportHandler.Name = "Findings report";
            ReportHandler.IncludeDetails = detailCheckBox.Checked;
            ReportHandler.IncludeTestSequencesDetails = subSequenceCheckBox.Checked;

            Hide();

            ProgressDialog dialog = new ProgressDialog("Generating report", ReportHandler);
            dialog.ShowDialog(Owner);
        }

        private void Btn_SelectFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
            };
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                ReportHandler.FileName = saveFileDialog.FileName;
                TxtB_Path.Text = ReportHandler.FileName;
            }
        }
    }
}