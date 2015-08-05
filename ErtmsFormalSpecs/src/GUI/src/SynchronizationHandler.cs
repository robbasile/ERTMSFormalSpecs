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
using System.Threading;
using Utils;

namespace GUI
{
    /// <summary>
    ///     Performs synchronization between model and view
    /// </summary>
    public abstract class SynchronizationHandler
    {
        /// <summary>
        ///     Indicates that synchronization is required to continue.
        ///     This flag is used to stop the thread
        /// </summary>
        public bool Synchronize { get; set; }

        /// <summary>
        ///     Indicates that synchronization should be suspended
        /// </summary>
        public bool Suspend { get; set; }

        /// <summary>
        ///     The cycle time used
        /// </summary>
        protected int CycleTime { get; private set; }

        /// <summary>
        ///     Performs the background job
        /// </summary>
        public abstract void DoSynchronize(object obj);

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="cycleTime"></param>
        protected SynchronizationHandler(int cycleTime)
        {
            Synchronize = true;
            Suspend = false;
            CycleTime = cycleTime;

            SynchronizerList.RegisterSynchronizer(this);
        }

        /// <summary>
        ///     Destructor
        /// </summary>
        ~SynchronizationHandler()
        {
            SynchronizerList.UnregisterSynchronizer(this);
        }

        /// <summary>
        ///     Stops the underlying task
        /// </summary>
        public abstract void Stop();
    }

    /// <summary>
    ///     Performs synchronization between model and view
    /// </summary>
    public abstract class GenericSynchronizationHandler<T> : SynchronizationHandler
    {
        /// <summary>
        ///     The instance that is currently synchronized
        /// </summary>
        protected T Instance { get; set; }

        /// <summary>
        ///     The thread handling the synchronization
        /// </summary>
        private Thread Thread { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="cycleTime"></param>
        protected GenericSynchronizationHandler(T instance, int cycleTime)
            : base(cycleTime)
        {
            Instance = instance;
            Thread = ThreadUtil.CreateThread("UI Synchronization", DoSynchronize);
            Thread.Start(Instance);
        }

        /// <summary>
        ///     Destructor
        /// </summary>
        ~GenericSynchronizationHandler()
        {
            Stop();
        }

        /// <summary>
        ///     Performs the initialization of the task
        /// </summary>
        public virtual void Initialize(T instance)
        {
        }

        /// <summary>
        ///     Handles the synchronization
        /// </summary>
        public abstract void HandleSynchronization(T instance);

        /// <summary>
        ///     Performs the background job
        /// </summary>
        /// <param name="obj"></param>
        public override void DoSynchronize(object obj)
        {
            T instance = (T) obj;

            Initialize(instance);
            while (Synchronize)
            {
                Thread.Sleep(CycleTime);

                if (!Suspend)
                {
                    try
                    {
                        HandleSynchronization(instance);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        /// <summary>
        ///     Stops the underlying task
        /// </summary>
        public override void Stop()
        {
            Synchronize = false;
            if (Thread != null)
            {
                Thread.Abort();
                Thread = null;
            }
        }
    }

    public static class SynchronizerList
    {
        /// <summary>
        ///     The registered handlers
        /// </summary>
        private static readonly List<SynchronizationHandler> Handlers = new List<SynchronizationHandler>();

        /// <summary>
        ///     Registers a new handler
        /// </summary>
        /// <param name="handler"></param>
        public static void RegisterSynchronizer(SynchronizationHandler handler)
        {
            Handlers.Add(handler);
        }

        /// <summary>
        ///     Unregisters the handler
        /// </summary>
        /// <param name="handler"></param>
        public static void UnregisterSynchronizer(SynchronizationHandler handler)
        {
            Handlers.Remove(handler);
        }

        /// <summary>
        ///     Suspends the synchronization for all handlers
        /// </summary>
        public static void SuspendSynchronization()
        {
            foreach (SynchronizationHandler handler in Handlers)
            {
                handler.Suspend = true;
            }
        }

        /// <summary>
        ///     Resumes the synchronization performed by the handlers
        /// </summary>
        public static void ResumeSynchronization()
        {
            foreach (SynchronizationHandler handler in Handlers)
            {
                handler.Suspend = false;
            }
        }

        /// <summary>
        ///     Stops all synchronization handlers
        /// </summary>
        public static void Stop()
        {
            // Stops all handlers at the same time
            foreach (SynchronizationHandler handler in Handlers)
            {
                handler.Synchronize = false;
            }

            // And join them
            foreach (SynchronizationHandler handler in Handlers)
            {
                handler.Stop();
            }
        }
    }
}