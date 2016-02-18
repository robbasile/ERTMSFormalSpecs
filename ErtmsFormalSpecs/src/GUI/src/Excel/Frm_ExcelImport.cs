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
using System.Globalization;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Tests;
using GUIUtils;
using Importers.ExcelImporter;

namespace GUI.ExcelImport
{
    public partial class Frm_ExcelImport : Form
    {
        private readonly BrakingCurvesImporter _brakingCurvesImporter = new BrakingCurvesImporter();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aDictionary"></param>
        public Frm_ExcelImport(Dictionary aDictionary)
        {
            InitializeComponent();
            _brakingCurvesImporter.TheDictionary = aDictionary;

            TB_FrameName.Text = String.Format("Frame__{0}_{1}_{2}__{3}s_{4}m_{5}h", DateTime.Now.Year,
                DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Second, DateTime.Now.Minute, DateTime.Now.Hour);

            CBB_SpeedInterval.Items.Add("0.1");
            CBB_SpeedInterval.Items.Add("0.2");
            CBB_SpeedInterval.Items.Add("0.5");
            CBB_SpeedInterval.Items.Add("1.0");
            CBB_SpeedInterval.Items.Add("5.0");
            CBB_SpeedInterval.Items.Add("10.0");
        }


        public Frm_ExcelImport(Step aStep)
        {
            InitializeComponent();
            _brakingCurvesImporter.TheStep = aStep;

            CBB_SpeedInterval.Items.Add("0.1");
            CBB_SpeedInterval.Items.Add("0.2");
            CBB_SpeedInterval.Items.Add("0.5");
            CBB_SpeedInterval.Items.Add("1.0");
            CBB_SpeedInterval.Items.Add("5.0");
            CBB_SpeedInterval.Items.Add("10.0");
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
                return retVal;
            }
        }


        /// <summary>
        ///     Allows to select the excel file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_SelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select excel file...",
                Filter = "Microsof Excel (.xlsm)|*.xlsm"
            };
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                _brakingCurvesImporter.FileName = openFileDialog.FileName;
                TB_FileName.Text = openFileDialog.FileName;
            }
        }


        /// <summary>
        ///     Launches the command of importing excel file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Import_Click(object sender, EventArgs e)
        {
            Hide();
            Double.TryParse(CBB_SpeedInterval.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                out _brakingCurvesImporter.SpeedInterval);

            _brakingCurvesImporter.FrameName = TB_FrameName.Text;
            _brakingCurvesImporter.FileName = TB_FileName.Text;
            _brakingCurvesImporter.FillEBD = CB_EBD.Checked;
            _brakingCurvesImporter.FillSBD = CB_SBD.Checked;
            _brakingCurvesImporter.FillEBI = CB_EBI.Checked;
            _brakingCurvesImporter.FillSBI1 = CB_SBI1.Checked;
            _brakingCurvesImporter.FillSBI2 = CB_SBI2.Checked;
            _brakingCurvesImporter.FillWarning = CB_Warning.Checked;
            _brakingCurvesImporter.FillPermitted = CB_Permitted.Checked;
            _brakingCurvesImporter.FillIndication = CB_Indication.Checked;

            _brakingCurvesImporter.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Importing excel file....");
        }


        private void CB_Select_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Tag.Equals("GLOBAL_FILTER"))
            {
                if (cb.Checked)
                {
                    cb.Text = "Deselect all";
                    foreach (Control control in AllControls)
                    {
                        if (control is CheckBox)
                        {
                            CheckBox checkBox = control as CheckBox;
                            if (checkBox.Tag.Equals("FILTER"))
                            {
                                checkBox.Checked = true;
                            }
                        }
                    }
                }
                else
                {
                    cb.Text = "Select all";
                    foreach (Control control in AllControls)
                    {
                        if (control is CheckBox)
                        {
                            CheckBox checkBox = control as CheckBox;
                            if (checkBox.Tag.Equals("FILTER"))
                            {
                                checkBox.Checked = false;
                            }
                        }
                    }
                }
            }
        }
    }
}