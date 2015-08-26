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

namespace DataDictionary
{
    /// <summary>
    ///     Keeps track of last MAX_MARKING markings
    /// </summary>
    public class MarkingHistory
    {
        /// <summary>
        ///     The maximum number of marking to keep track of
        /// </summary>
        private const int MaxMarking = 10;

        /// <summary>
        ///     The markings
        /// </summary>
        private List<Marking> Markings { get; set; }

        /// <summary>
        ///     The indice of the current marking in the history
        /// </summary>
        private Marking CurrentMarking { get; set; }

        /// <summary>
        ///     The marking history for a specific EFS System
        /// </summary>
        public MarkingHistory()
        {
            Markings = new List<Marking>();
        }

        /// <summary>
        ///     Registers the current marking
        /// </summary>
        public void RegisterCurrentMarking()
        {
            CurrentMarking = new Marking();

            if (Markings.Count >= MaxMarking)
            {
                Markings.RemoveAt(0);
            }

            Markings.Add(CurrentMarking);
        }

        /// <summary>
        ///     Selects the current marking an restores the marks
        /// </summary>
        /// <param name="marking"></param>
        /// <returns>true when a marking has been selected</returns>
        private bool SelectCurrentMarking(Marking marking)
        {
            bool retVal = marking != null;

            if (retVal)
            {
                EfsSystem.Instance.ClearMessages(false);
                EfsSystem.Instance.Context.HandleInfoMessageChangeEvent(null);
                CurrentMarking = marking;
                CurrentMarking.RestoreMarks();
            }

            return retVal;
        }

        /// <summary>
        ///     Selects the next marking in the history
        /// </summary>
        public bool SelectNextMarking()
        {
            Marking previous = null;
            Marking current = null;
            foreach (Marking marking in Markings)
            {
                if (marking == CurrentMarking)
                {
                    previous = marking;
                }
                else if (previous != null)
                {
                    current = marking;
                    break;
                }
            }

            // current is the marking after CurrentMarking
            return SelectCurrentMarking(current);
        }

        /// <summary>
        ///     Selects the previous marking in the history
        /// </summary>
        public bool SelectPreviousMarking()
        {
            Marking current = null;
            foreach (Marking marking in Markings)
            {
                if (marking != CurrentMarking)
                {
                    current = marking;
                }
                else
                {
                    break;
                }
            }

            // current is the marking before CurrentMarking
            return SelectCurrentMarking(current);
        }

        /// <summary>
        ///     The steps required to perform the marking
        /// </summary>
        public delegate void MarkAction();

        /// <summary>
        ///     Mark the model
        /// </summary>
        /// <param name="action"></param>
        public static void PerformMark(MarkAction action)
        {
            try
            {
                EfsSystem.Instance.ClearMessages(false);
                EfsSystem.Instance.Context.HandleInfoMessageChangeEvent(null);
                EfsSystem.Instance.Context.SuppressInfoMessage = true;
                action();
                EfsSystem.Instance.Markings.RegisterCurrentMarking();
            }
            catch (Exception)
            {
            }
            finally
            {
                EfsSystem.Instance.Context.SuppressInfoMessage = false;
                EfsSystem.Instance.Context.HandleInfoMessageChangeEvent(null);                
            }
        }
    }
}