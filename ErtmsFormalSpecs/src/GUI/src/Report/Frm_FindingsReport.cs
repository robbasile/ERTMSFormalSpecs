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
    public partial class FindingsReport : Form
    {
        private readonly FindingsReportHandler _reportHandler;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public FindingsReport(Dictionary dictionary)
        {
            InitializeComponent();
            _reportHandler = new FindingsReportHandler(dictionary);
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
            _reportHandler.Name = "Findings report";

            _reportHandler.addQuestions = CB_ShowQuestions.Checked;
            _reportHandler.addComments = CB_ShowComments.Checked;
            _reportHandler.addBugs = CB_ShowBugs.Checked;
            _reportHandler.addReviewed = CB_Reviewed.Checked;
            _reportHandler.addNotReviewed = CB_NotReviewed.Checked;

            Hide();

            ProgressDialog dialog = new ProgressDialog("Generating report", _reportHandler);
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
                _reportHandler.FileName = saveFileDialog.FileName;
                TxtB_Path.Text = _reportHandler.FileName;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}