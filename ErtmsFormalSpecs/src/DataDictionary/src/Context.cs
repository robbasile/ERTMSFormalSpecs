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
using DataDictionary.Generated;
using Utils;
using XmlBooster;

namespace DataDictionary
{
    /// <summary>
    /// The context used to display elements in the GUI
    /// </summary>
    public class Context: IListener<BaseModelElement>
    {
        /// <summary>
        /// Handling the critical section
        /// </summary>
        private Mutex CriticalSection { get; set; }

        /// <summary>
        /// The changes that occured in the system
        /// </summary>
        private Dictionary<IModelElement, HashSet<ChangeKind>> Changes { get; set; }

        /// <summary>
        /// Notifies of changes
        /// </summary>
        private Thread ChangeNotifier { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Context()
        {
            SelectionHistory = new List<SelectionContext>();
            ControllersManager.BaseModelElementController.Listeners.Add(this);

            CriticalSection = new Mutex(false, "Critical section");
            Changes = new Dictionary<IModelElement, HashSet<ChangeKind>>();
            ChangeNotifier = new Thread(NotifyChanges);
            ChangeNotifier.Start();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Context()
        {
            ControllersManager.BaseModelElementController.Listeners.Remove(this);
            ChangeNotifier.Abort();
        }

        /// <summary>
        ///     The maximum size of the history
        /// </summary>
        private const int MaxSelectionHistory = 100;

        /// <summary>
        ///     The selection history
        /// </summary>
        public List<SelectionContext> SelectionHistory { get; private set; }

        /// <summary>
        /// The flags related to this selection
        /// </summary>
        [Flags]
        public enum SelectionCriteria
        {
            None = 0,
            LeftClick = 1,
            RightClick = 2,
            DoubleClick = 4,
            Ctrl = 8,
            Alt = 16
        }

        /// <summary>
        /// Provides the context in which this selection occured
        /// </summary>
        public class SelectionContext
        {
            /// <summary>
            /// The element that has been selected
            /// </summary>
            public IModelElement Element { get; private set; }

            /// <summary>
            /// The sender of such event
            /// </summary>
            public object Sender { get; private set; }

            /// <summary>
            /// The selection criteria (left click, right click, ...)
            /// </summary>
            public SelectionCriteria Criteria { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="element"></param>
            /// <param name="sender"></param>
            /// <param name="criteria"></param>
            public SelectionContext(IModelElement element, object sender, SelectionCriteria criteria)
            {
                Element = element;
                Sender = sender;
                Criteria = criteria;
            }
        }

        /// <summary>
        /// Sets the currently selected element
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="source"></param>
        /// <param name="criteria"></param>
        public void SelectElement(IModelElement modelElement, object source, SelectionCriteria criteria)
        {
            SelectionContext selectionContext = new SelectionContext(modelElement, source, criteria);

            // Updates the selection history
            if (SelectionHistory.Count > 0)
            {
                // Only consider selection changes
                if (modelElement != SelectionHistory[0].Element)
                {
                    // Avoid overflow of the selection history
                    if (SelectionHistory.Count > MaxSelectionHistory)
                    {
                        SelectionHistory.RemoveAt(SelectionHistory.Count - 1);
                    }
                    SelectionHistory.Insert(0, selectionContext);
                }
            }
            else
            {
                SelectionHistory.Insert(0, selectionContext);
            }

            OnSelectionChange(selectionContext);
        }

        /// <summary>
        /// Clear the history until the element has been found
        /// </summary>
        public void ClearHistoryUntilElement(IModelElement element)
        {
            int index = -1;
            
            foreach (SelectionContext selectionContext in SelectionHistory)
            {
                index += 1;
                if (selectionContext.Element == element)
                {
                    break;
                }
            }

            if (index < SelectionHistory.Count)
            {
                while (index > 0)
                {
                    SelectionHistory.RemoveAt(0);
                    index -= 1;
                }
            }
        }

        /// <summary>
        /// The delegate used to handle the change of the currently selected model element
        /// </summary>
        /// <param name="context"></param>
        public delegate void HandleSelectionChange(SelectionContext context);

        /// <summary>
        /// The event raised when the selection changes
        /// </summary>
        public event HandleSelectionChange SelectionChange;

        /// <summary>
        /// Handles a change of the currently selected model element
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnSelectionChange(SelectionContext context)
        {
            if (SelectionChange != null)
            {
                SelectionChange(context);
            }
        }

        /// <summary>
        /// The reason why the change occured
        /// </summary>
        public enum ChangeKind
        {
            ModelChange,
            Translation,
            EndOfCycle,
            Load
        }

        /// <summary>
        /// The delegate used to handle the change of the value of a model element
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind">Indicates the reason why the change occured</param>
        public delegate void HandleValueChange(IModelElement modelElement, ChangeKind changeKind);

        /// <summary>
        /// The event raised when the value changes
        /// </summary>
        public event HandleValueChange ValueChange;

        /// <summary>
        /// Handles a change of the value of a model element
        /// </summary>
        /// <param name="value"></param>
        /// <param name="changeKind">Indicates the reason why the change occured</param>
        protected virtual void OnValueChange(IModelElement value, ChangeKind changeKind)
        {
            if (ValueChange != null)
            {
                if (value != null)
                {
                    CriticalSection.WaitOne();
                    HashSet<ChangeKind> changeKinds;
                    if (!Changes.TryGetValue(value, out changeKinds))
                    {
                        changeKinds = new HashSet<ChangeKind>();
                        Changes.Add(value, changeKinds);
                    }
                    changeKinds.Add(changeKind);
                    CriticalSection.ReleaseMutex();
                }
                else
                {
                    // ReSharper disable once ExpressionIsAlwaysNull
                    ValueChange(value, changeKind);
                }
            }
        }

        /// <summary>
        /// Background thread used to notify the changes in the system
        /// </summary>
        private void NotifyChanges()
        {
            while (true)
            {
                // Release the mutex asap
                CriticalSection.WaitOne();
                Dictionary<IModelElement, HashSet<ChangeKind>> changes = Changes;
                Changes = new Dictionary<IModelElement, HashSet<ChangeKind>>();
                CriticalSection.ReleaseMutex();

                // Notify of changes
                if (ValueChange != null)
                {
                    foreach (var pair in changes)
                    {
                        foreach (ChangeKind kind in pair.Value)
                        {
                            ValueChange(pair.Key, kind);
                        }
                    }
                }

                Thread.Sleep(250);
            }
        }

        /// <summary>
        /// Handles the change of a value on a model element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="changeKind">Indicates the reason why the change occured</param>
        public void HandleChangeEvent(BaseModelElement sender, ChangeKind changeKind)
        {
            OnValueChange(sender, changeKind);
        }

        /// <summary>
        /// Handles the change of a value on a model element
        /// </summary>
        /// <param name="sender"></param>
        public void HandleChangeEvent(BaseModelElement sender)
        {
            HandleChangeEvent(null, sender);
        }

        /// <summary>
        /// Handles the change of a value on a model element
        /// </summary>
        /// <param name="aLock"></param>
        /// <param name="sender"></param>
        public void HandleChangeEvent(Lock aLock, BaseModelElement sender)
        {
            if (aLock != null)
            {
                if (aLock.GetLock())
                {
                    OnValueChange(sender, ChangeKind.ModelChange);
                    aLock.UnLock();
                }
            }
            else
            {
                OnValueChange(sender, ChangeKind.ModelChange);
            }
        }

        /// <summary>
        /// The delegate used to handle the change of information message associaated to a model element
        /// </summary>
        /// <param name="modelElement"></param>
        public delegate void HandleInfoMessageChange(IModelElement modelElement);

        /// <summary>
        /// The event raised when the informative message changes
        /// </summary>
        public event HandleInfoMessageChange InfoMessageChange;

        /// <summary>
        /// Handles a change of the value of a model element
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnInformationMessageChange(IModelElement value)
        {
            if (InfoMessageChange != null)
            {
                InfoMessageChange(value);
            }
        }

        /// <summary>
        /// Handles the change of a value on a model element
        /// </summary>
        /// <param name="sender"></param>
        public void HandleInfoMessageChangeEvent(IModelElement sender)
        {
            OnInformationMessageChange(sender);
        }
    }
}
