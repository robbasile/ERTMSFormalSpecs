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
using System.Threading;
using System.Windows.Forms;
using Utils;
using Util = DataDictionary.Util;

namespace GUIUtils.LongOperations
{
    /// <summary>
    ///     The base class used to handle long operations
    /// </summary>
    public abstract class BaseLongOperation : ProgressHandler
    {
        /// <summary>
        ///     Execution time span
        /// </summary>
        public TimeSpan Span { get; set; }

        /// <summary>
        ///     Indicates that the dialog should be displayed
        /// </summary>
        public bool ShowDialog { get; set; }

        /// <summary>
        ///     The dialog used to display progress to the user
        /// </summary>
        public ProgressDialog Dialog { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected BaseLongOperation()
        {
            if (Environment.UserInteractive)
            {
                ShowDialog = true;
            }
            else
            {
                ShowDialog = false;
            }
        }

        /// <summary>
        ///     Executes the operation in background using a progress handler
        /// </summary>
        /// <param name="message">The message to display on the dialog window</param>
        /// <param name="allowCancel">Indicates that the opeation can be canceled</param>
        public virtual void ExecuteUsingProgressDialog(string message, bool allowCancel = true)
        {
            DateTime start = DateTime.Now;

            Util.DontNotify(() =>
            {
                try
                {
                    SynchronizerList.SuspendSynchronization();

                    if (ShowDialog)
                    {
                        Dialog = new ProgressDialog(message, this, allowCancel);
                        Dialog.ShowDialog ();
                    }
                    else
                    {
                        ExecuteWork();
                    }
                }
                catch (ThreadAbortException)
                {
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + @"\n" + e.StackTrace, @"Exception raised");
                    // DefaultDesktopOnly option is added in order to avoid exceptions during nightbuild execution
                    MessageBox.Show (e.Message + @"\n" + e.StackTrace, @"Exception raised", MessageBoxButtons.OK,
                                     MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                     MessageBoxOptions.DefaultDesktopOnly);
                }
                finally
                {
                    Span = DateTime.Now.Subtract(start);
                    SynchronizerList.ResumeSynchronization();
                }
            });
        }
    }
}