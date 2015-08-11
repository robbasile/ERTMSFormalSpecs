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
using System.Reflection;
using System.Windows.Forms;
using DataDictionary;
using GUIUtils;
using log4net;
using Reports.Model;

namespace GUI.Report
{
    public partial class ModelReport : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ModelReportHandler reportHandler;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public ModelReport(Dictionary dictionary)
        {
            InitializeComponent();
            reportHandler = new ModelReportHandler(dictionary);
            TxtB_Path.Text = reportHandler.FileName;
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
                retVal.AddRange(this.Controls);
                retVal.AddRange(this.GrB_Options.Controls);
                return retVal;
            }
        }


        /// <summary>
        ///     Method called in case of check event of one of the check boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            string[] tags = cb.Tag.ToString().Split('.');
            string cbProperty = tags[0];
            int cbLevel;
            Int32.TryParse(tags[1], out cbLevel);
            if (cbProperty.Equals("FILTER"))
            {
                if (cb.Checked)
                {
                    SelectCheckBoxes(cbLevel); /* we enable all the check boxes of the selected level */
                }
                else
                {
                    DeselectCheckBoxes(cbLevel); /* we disable the statistics check boxes of the selected level */
                }
            }
            if (cbLevel == 10)
            {
                if (cb.Checked)
                {
                    SelectCheckBoxes(cbProperty);
                    cb.Text = "Deselect all";
                }
                else
                {
                    DeselectCheckBoxes(cbProperty);
                    cb.Text = "Select all";
                }
            }
        }


        /// <summary>
        ///     Enables all the check boxes of the selected level
        ///     and the check box corresponding to the filter of selected level + 1
        /// </summary>
        /// <param name="level">Level of the checked check box</param>
        private void SelectCheckBoxes(int level)
        {
            foreach (Control control in AllControls)
            {
                if (control is CheckBox)
                {
                    CheckBox cb = control as CheckBox;
                    string[] tags = cb.Tag.ToString().Split('.');
                    string cbProperty = tags[0];
                    int cbLevel;
                    Int32.TryParse(tags[1], out cbLevel);
                    if (cbLevel == level && !cbProperty.Equals("FILTER"))
                    {
                        cb.Enabled = true;
                    }
                }
            }
        }


        /// <summary>
        ///     Checks all the check boxes of the selected property
        /// </summary>
        /// <param name="level">Property of the checked check box</param>
        private void SelectCheckBoxes(string property)
        {
            foreach (Control control in AllControls)
            {
                if (control is CheckBox)
                {
                    CheckBox cb = control as CheckBox;
                    string[] tags = cb.Tag.ToString().Split('.');
                    string cbProperty = tags[0];
                    int cbLevel;
                    Int32.TryParse(tags[1], out cbLevel);
                    if (cbProperty.Equals(property) && cbLevel != 10 && cb.Enabled == true)
                    {
                        cb.Checked = true;
                    }
                }
            }
        }


        /// <summary>
        ///     Disables the check boxes corresponding to the statistics of the selected level
        /// </summary>
        /// <param name="level">Level of the unckecked check box</param>
        private void DeselectCheckBoxes(int level)
        {
            foreach (Control control in AllControls)
            {
                if (control is CheckBox)
                {
                    CheckBox cb = control as CheckBox;
                    string[] tags = cb.Tag.ToString().Split('.');
                    string cbProperty = tags[0];
                    int cbLevel;
                    Int32.TryParse(tags[1], out cbLevel);
                    if (cbLevel == level && !cbProperty.Equals("FILTER"))
                    {
                        cb.Checked = false;
                        cb.Enabled = false;
                    }
                }
            }
        }


        /// <summary>
        ///     Disables the check boxes corresponding to the statistics of the selected level
        /// </summary>
        /// <param name="level">Level of the unckecked check box</param>
        private void DeselectCheckBoxes(string property)
        {
            foreach (Control control in AllControls)
            {
                if (control is CheckBox)
                {
                    CheckBox cb = control as CheckBox;
                    string[] tags = cb.Tag.ToString().Split('.');
                    string cbProperty = tags[0];
                    int cbLevel;
                    Int32.TryParse(tags[1], out cbLevel);
                    if (cbProperty.Equals(property) && cbLevel != 10)
                    {
                        cb.Checked = false;
                    }
                }
            }
        }


        /// <summary>
        ///     Creates a report config with user's choices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_CreateReport_Click(object sender, EventArgs e)
        {
            reportHandler.Name = "Model report";

            reportHandler.AddRanges = CB_AddRanges.Checked;
            reportHandler.AddRangesDetails = CB_AddRangesDetails.Checked;

            reportHandler.AddEnumerations = CB_AddEnumerations.Checked;
            reportHandler.AddEnumerationsDetails = CB_AddEnumerationsDetails.Checked;

            reportHandler.AddStructures = CB_AddStructures.Checked;
            reportHandler.AddStructuresDetails = CB_AddStructuresDetails.Checked;

            reportHandler.AddCollections = CB_AddCollections.Checked;
            reportHandler.AddCollectionsDetails = CB_AddCollectionsDetails.Checked;

            reportHandler.AddStateMachines = CB_AddStateMachines.Checked;
            reportHandler.AddStateMachinesDetails = CB_AddStateMachinesDetails.Checked;

            reportHandler.AddFunctions = CB_AddFunctions.Checked;
            reportHandler.AddFunctionsDetails = CB_AddFunctionsDetails.Checked;

            reportHandler.AddProcedures = CB_AddProcedures.Checked;
            reportHandler.AddProceduresDetails = CB_AddProceduresDetails.Checked;

            reportHandler.AddVariables = CB_AddVariables.Checked;
            reportHandler.AddVariablesDetails = CB_AddVariablesDetails.Checked;
            reportHandler.InOutOnly = CB_InOutFilter.Checked;

            reportHandler.AddRules = CB_AddRules.Checked;
            reportHandler.AddRulesDetails = CB_AddRulesDetails.Checked;

            reportHandler.ImplementedOnly = CB_ImplementedFilter.Checked;

            Hide();

            ProgressDialog dialog = new ProgressDialog("Generating report", reportHandler);
            dialog.ShowDialog(Owner);
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
                reportHandler.FileName = saveFileDialog.FileName;
                TxtB_Path.Text = reportHandler.FileName;
            }
        }
    }
}