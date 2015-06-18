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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using DataDictionary;
using GUI;
using GUI.IPCInterface;
using GUI.Options;
using log4net;
using log4net.Config;
using Utils;

namespace ERTMSFormalSpecs
{
    public static class ErtmsFormalSpecGui
    {
        /// <summary>
        ///     The EFS IPC service
        /// </summary>
        private static ServiceHost host = null;

        /// <summary>
        ///     Hosts the EFS IPC service
        /// </summary>
        /// <returns>The hosting service</returns>
        private static void HostEFSService()
        {
            host = new ServiceHost(EFSService.INSTANCE);
            try
            {
                host.Open();
            }
            catch (CommunicationException exception)
            {
                Console.WriteLine("An exception occurred: {0}", exception.Message);
                host.Abort();
            }
        }

        /// <summary>
        ///     Closes the EFS IPC host service
        /// </summary>
        private static void CloseEFSService()
        {
            if (host != null)
            {
                // Close the ServiceHostBase to shutdown the service.
                host.Close();
            }
        }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                XmlConfigurator.Configure(new FileInfo("logconfig.xml"));

                Options.setSettings(EFSSystem.INSTANCE);

                MainWindow window = new MainWindow();
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                {
                    // TRICKY SECTION
                    // This thread is mandatory otherwise WCF does not create a new thread to handle the service requests. 
                    // Since the call to Cycle is blocking, creating such threads is mandatory
                    Thread thread = ThreadUtil.CreateThread("EFS Service", HostEFSService);
                    thread.Start();
                }

                foreach (string file in args)
                {
                    window.OpenFile(file);
                }
                 Application.Run(window);
                CloseEFSService();
            }
            finally
            {
                Util.UnlockAllFiles();
            }


            EFSSystem.INSTANCE.Stop();
            SynchronizerList.Stop();
        }
    }
}