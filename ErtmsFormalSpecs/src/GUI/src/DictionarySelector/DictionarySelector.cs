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
using System.Windows.Forms;
using DataDictionary;

namespace GUI.DictionarySelector
{
    public enum FilterOptions
    {
        None,
        Updates,
        HideUpdates
    };

    public partial class DictionarySelector : Form
    {
        /// <summary>
        ///     An entry in the list box
        /// </summary>
        private class ListBoxEntry
        {
            /// <summary>
            ///     The reference entry
            /// </summary>
            public Dictionary Dictionary { get; private set; }

            /// <summary>
            ///     The display name of the entry
            /// </summary>
            // ReSharper disable once UnusedMember.Local
            public string Name
            {
                get { return Dictionary.Name; }
            }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="dictionary"></param>
            public ListBoxEntry(Dictionary dictionary)
            {
                Dictionary = dictionary;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="options">The options used to filter the selection</param>
        /// <param name="updatedDictionary"></param>
        public DictionarySelector(FilterOptions options = FilterOptions.None,
            Dictionary updatedDictionary = null)
        {
            InitializeComponent();

            Options = options;
            UpdatedDictionary = updatedDictionary;

            ArrayList entries = new ArrayList();
            foreach (Dictionary dictionary in EfsSystem.Instance.Dictionaries)
            {
                if (AddToEntries(dictionary))
                {
                    entries.Add(new ListBoxEntry(dictionary));
                }
            }
            dataDictionaryListBox.DataSource = entries;
            dataDictionaryListBox.DisplayMember = "Name";

            if (entries.Count > 0)
            {
                dataDictionaryListBox.ValueMember = "Dictionary";
            }
        }

        /// <summary>
        ///     The filtering options applied to the dictionaries
        /// </summary>
        private FilterOptions Options { get; set; }

        /// <summary>
        ///     If the selection is limited to updates of a particular dictionary, the Guid of the updated dictionary
        /// </summary>
        private Dictionary UpdatedDictionary { get; set; }
        
        /// <summary>
        ///     Filters the dictionaries displayed for selection, if required
        /// </summary>
        /// <param name="dictionary">The dictionary under consideration</param>
        /// <returns>Whether the dictionary should be displayed</returns>
        private bool AddToEntries(Dictionary dictionary)
        {
            bool retVal = true;

            if (Options == FilterOptions.HideUpdates &&
                dictionary.Updates != null)
            {
                // The options restrict the selection to non-update dictionaries
                retVal = false;
            }
            else if (Options == FilterOptions.Updates &&
                     UpdatedDictionary == null)
            {
                retVal = dictionary.Updates != null;
            }
            else if (Options == FilterOptions.Updates &&
                     !UpdatedDictionary.IsUpdatedBy(dictionary))
            {
                // The options restrict the selection to updates of a particular dictionary
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        ///     The selected dictionary
        /// </summary>
        public Dictionary Selected { get; private set; }


        public void ShowDictionaries(MainWindow mainWindow)
        {
            // test that the data source of the selector is not empty
            // to do this, we must check whether the DisplayMember is an empty string
            if (dataDictionaryListBox.DisplayMember != "")
            {
                ShowDialog(mainWindow);
            }
            else
            {
                string noRelevantDictionaries = "";
                if (Options == FilterOptions.None)
                {
                    noRelevantDictionaries = "There are no dictionaries loaded in ERTMSFormalSpecs.";
                }
                else if (Options == FilterOptions.Updates)
                {
                    noRelevantDictionaries = "There are no updates";
                    if (UpdatedDictionary != null)
                    {
                        noRelevantDictionaries += " for " + UpdatedDictionary.Name;
                    }
                    noRelevantDictionaries +=
                        " currently loaded in ERTMSFormalSpecs.\nTo create one, select File-> New-> Update.";
                }
                else if (Options == FilterOptions.HideUpdates)
                {
                    noRelevantDictionaries =
                        "There are no dictionaries that are not updates currently loaded in ERTMSFormalSpecs.";
                }
                MessageBox.Show(noRelevantDictionaries);
            }
        }

        /// <summary>
        ///     Handles the click event on the select button :
        ///     - stores the selected dictionary
        ///     - close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectButton_Click(object sender, EventArgs e)
        {
            Selected = null;

            ListBoxEntry selected = dataDictionaryListBox.SelectedItem as ListBoxEntry;
            if (selected != null)
            {
                Selected = selected.Dictionary;
            }

            Close();
        }
    }
}