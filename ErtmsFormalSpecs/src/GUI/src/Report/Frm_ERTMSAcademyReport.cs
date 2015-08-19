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
using System.Collections.Generic;
using System.Windows.Forms;
using DataDictionary;
using GUIUtils;
using Reports.ERTMSAcademy;

namespace GUI.Report
{
    public partial class ErtmsAcademyReport : Form
    {
        private readonly ERTMSAcademyReportHandler _reportHandler;

        private readonly Dictionary<string, string> _usersAndLogin = new Dictionary<string, string>();


        public ErtmsAcademyReport(Dictionary dictionary)
        {
            InitializeComponent();
            _reportHandler = new ERTMSAcademyReportHandler(dictionary);
            TxtB_Path.Text = _reportHandler.FileName;

            _usersAndLogin.Add("James", "james@ertmssolutions.com");
            _usersAndLogin.Add("Moritz", "morido@web.de");
            _usersAndLogin.Add("Luis", "luis@ertmssolutions.com");
            _usersAndLogin.Add("Laurent", "laurent@ertmssolutions.com");
            _usersAndLogin.Add("Svitlana", "svitlana@ertmssolutions.com");

            List<string> userNames = new List<string>();
            userNames.AddRange(_usersAndLogin.Keys);
            userNames.Sort();
            Cbb_UserNames.DataSource = userNames;
        }

        private void Btn_Browse_Click(object sender, EventArgs e)
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

        private void Btn_CreateReport_Click(object sender, EventArgs e)
        {
            _reportHandler.Name = "ERTMS Academy report";

            _reportHandler.User = Cbb_UserNames.Text;
            _reportHandler.GitLogin = _usersAndLogin[_reportHandler.User];
            _reportHandler.SinceHowManyDays = (int) sinceUpDown.Value;

            Hide();

            ProgressDialog dialog = new ProgressDialog("Generating report", _reportHandler);
            dialog.ShowDialog(Owner);
        }
    }
}