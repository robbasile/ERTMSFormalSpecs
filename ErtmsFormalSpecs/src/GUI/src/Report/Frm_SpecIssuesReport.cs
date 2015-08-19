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
using System.Collections;
using System.Windows.Forms;
using DataDictionary;
using GUIUtils;
using Reports.Specs;

namespace GUI.Report
{
    public partial class SpecIssuesReport : Form
    {
        private readonly SpecIssuesReportHandler _reportHandler;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public SpecIssuesReport(Dictionary dictionary)
        {
            InitializeComponent();
            _reportHandler = new SpecIssuesReportHandler(dictionary);
            TxtB_Path.Text = _reportHandler.FileName;
        }


        /// <summary>
        ///     Gives the list of all the controls of the form
        ///     (situated on the main form or on its group box)
        /// </summary>
        public ArrayList AllControls
        {
            get
            {
                ArrayList retVal = new ArrayList();

                retVal.AddRange(Controls);
                retVal.AddRange(GrB_Options.Controls);

                return retVal;
            }
        }

        /// <summary>
        ///     Creates a report config with user's choices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_CreateReport_Click(object sender, EventArgs e)
        {
            _reportHandler.Name = "Specification issues report";

            _reportHandler.AddSpecIssues = CB_ShowIssues.Checked;
            _reportHandler.AddDesignChoices = CB_ShowDesignChoices.Checked;
            _reportHandler.AddInformationNeeded = moreInformationNeededCheckBox.Checked;

            Hide();

            ProgressDialog dialog = new ProgressDialog("Generating report", _reportHandler);
            dialog.ShowDialog(Owner);
        }


        /// <summary>
        ///     Permits to select the name and the path of the report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_SelectFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
            };
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                _reportHandler.FileName = saveFileDialog.FileName;
                TxtB_Path.Text = _reportHandler.FileName;
            }
        }
    }
}