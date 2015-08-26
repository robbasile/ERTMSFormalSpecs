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
using System.IO;
using System.Threading;

namespace DataDictionary
{
    public class DictionaryWatcher
    {
        /// <summary>
        ///     The system for which this watcher is built
        /// </summary>
        public EfsSystem System { get; private set; }

        /// <summary>
        ///     The dictionary to watch
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        ///     The file system watcher
        /// </summary>
        private FileSystemWatcher Watcher { get; set; }

        /// <summary>
        ///     Provides the last time a change occured
        /// </summary>
        private DateTime LastChange { get; set; }

        /// <summary>
        ///     The time between which changes are not taken into consideration
        /// </summary>
        private TimeSpan DeltaTime { get; set; }

        /// <summary>
        ///     The thread which waits for the end of the burst
        /// </summary>
        private Thread WaitEndOfBurst { get; set; }

        /// <summary>
        ///     The time from witch watching is allowed.
        /// </summary>
        private DateTime? WatchTime { get; set; }

        /// <summary>
        ///     A mutex to enter the critical region
        /// </summary>
        private Mutex CriticalRegion { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public DictionaryWatcher(EfsSystem system, Dictionary dictionary)
        {
            System = system;
            Dictionary = dictionary;
            LastChange = DateTime.Now;
            DeltaTime = new TimeSpan(0, 0, 2);

            CriticalRegion = new Mutex(false, "Critical region");

            string path = Path.GetDirectoryName(dictionary.FilePath) + Path.DirectorySeparatorChar +
                          Path.GetFileNameWithoutExtension(dictionary.FilePath);
            path = Path.GetFullPath(path);
            Directory.CreateDirectory(path);
            Watcher = new FileSystemWatcher(path, "*.*")
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite
            };
            Watcher.Changed += Watcher_Changed;
            Watcher.Created += Watcher_Changed;
            Watcher.Deleted += Watcher_Changed;

            StartWatching();
        }

        /// <summary>
        ///     Handles a change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            CriticalRegion.WaitOne();

            LastChange = DateTime.Now;

            if (WaitEndOfBurst == null || !WaitEndOfBurst.IsAlive)
            {
                WaitEndOfBurst = new Thread(SendChangeEvent);
                WaitEndOfBurst.Start();
            }

            CriticalRegion.ReleaseMutex();
        }

        /// <summary>
        ///     Waits before the end of the change burst before sending the change event
        /// </summary>
        private void SendChangeEvent()
        {
            DateTime now;

            // Wait for the end of the burst
            do
            {
                Thread.Sleep(100);
                now = DateTime.Now;
            } while (now - LastChange > DeltaTime);

            if (WatchTime != null && now >= WatchTime)
            {
                System.OnDictionaryChangesOnFileSystem(Dictionary);
            }
        }

        /// <summary>
        ///     Stops alerting when file changes
        /// </summary>
        public void StopWatching()
        {
            Watcher.EnableRaisingEvents = false;
            WatchTime = null;
        }

        /// <summary>
        ///     Start alerting when file changes
        /// </summary>
        public void StartWatching()
        {
            // Wait 10 seconds before actually handling change events
            WatchTime = DateTime.Now + new TimeSpan(0, 0, 10);
            Watcher.EnableRaisingEvents = true;
        }
    }
}