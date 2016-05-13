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
using GUIUtils;
using Reports;

namespace GUI.Report
{
    public static class ReportUtil
    {
        /// <summary>
        /// Displays the progress dialog and handle exceptions, if any
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="handler"></param>
        public static void CreateReport(Form owner, ReportHandler handler)
        {
            Exception exception = null;

            try
            {
                SynchronizerList.SuspendSynchronization();

                ProgressDialog dialog = new ProgressDialog("Generating report", handler);
                dialog.ShowDialog(owner);
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                SynchronizerList.ResumeSynchronization();
            }

            if (handler.Error != null)
            {
                exception = handler.Error;
            }

            if (exception != null)
            {
                MessageBox.Show(owner, exception.Message, "An error has occured", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
