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
using Reports.Model;

namespace GUI.Report
{
    public partial class FunctionalAnalysisReport : Form
    {
        /// <summary>
        ///     The report handler
        /// </summary>
        private readonly FunctionalAnalysisReportHandler _reportHandler;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public FunctionalAnalysisReport(Dictionary dictionary)
        {
            InitializeComponent();
            _reportHandler = new FunctionalAnalysisReportHandler(dictionary);
            TxtB_Path.Text = _reportHandler.FileName;
        }

        /// <summary>
        ///     Permits to select the name and the path of the report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_SelectFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                _reportHandler.FileName = saveFileDialog.FileName;
                TxtB_Path.Text = _reportHandler.FileName;
            }
        }

        private void Btn_CreateReport_Click(object sender, EventArgs e)
        {
            _reportHandler.Name = "Functional Analysis report";

            Hide();

            ReportUtil.CreateReport(Owner, _reportHandler);
        }
    }
}