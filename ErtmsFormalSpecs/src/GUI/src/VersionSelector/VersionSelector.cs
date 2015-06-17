using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DataDictionary;
using LibGit2Sharp;

namespace GUI.VersionSelector
{
    public partial class VersionSelector : Form
    {
        /// <summary>
        ///     The dictionary for which this selection is built
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        ///     The selected commit
        /// </summary>
        public Commit Selected { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public VersionSelector()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public VersionSelector(Dictionary dictionary)
        {
            InitializeComponent();
            Dictionary = dictionary;
            RefreshModel();

            dataGridView.DoubleClick += new EventHandler(dataGridView_DoubleClick);
        }

        /// <summary>
        ///     When double clicking on an element of the version selector,
        ///     select the corresponding Commit and close the window to continue work
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_DoubleClick(object sender, EventArgs e)
        {
            Selected = null;

            if (dataGridView.SelectedCells.Count == 1)
            {
                Selected =
                    ((List<DisplayObject>) dataGridView.DataSource)[dataGridView.SelectedCells[0].OwningRow.Index]
                        .Commit;
            }

            if (Selected != null)
            {
                Close();
            }
        }

        private class DisplayObject
        {
            /// <summary>
            ///     The date of the commit
            /// </summary>
            public DateTimeOffset Date
            {
                get { return Commit.Committer.When; }
            }

            /// <summary>
            ///     The author of the commit
            /// </summary>
            public String Author
            {
                get { return Commit.Committer.Name; }
            }

            /// <summary>
            ///     The message of the commit
            /// </summary>
            public String Message
            {
                get { return Commit.Message; }
            }

            /// <summary>
            ///     The commit to be displayed
            /// </summary>
            public Commit Commit { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="commit"></param>
            public DisplayObject(Commit commit)
            {
                Commit = commit;
            }
        }

        /// <summary>
        ///     Rebuilds the model according to the dictionary to be considered
        /// </summary>
        public void RefreshModel()
        {
            if (Dictionary != null)
            {
                Repository repository = Repository;
                if (repository != null)
                {
                    string filter = filterTextBox.Text;

                    Regex regEx = null;
                    bool validFilter = true;
                    if (!String.IsNullOrEmpty(filter))
                    {
                        try
                        {
                            regEx = new Regex(filter);
                        }
                        catch (Exception)
                        {
                            validFilter = false;
                        }
                    }

                    List<DisplayObject> source = new List<DisplayObject>();
                    if (validFilter)
                    {
                        foreach (Commit commit in repository.Commits)
                        {
                            if (regEx == null)
                            {
                                source.Add(new DisplayObject(commit));
                            }
                            else if (regEx.Match(commit.Message).Success)
                            {
                                source.Add(new DisplayObject(commit));
                            }
                        }
                    }
                    dataGridView.DataSource = source;
                }
                else
                {
                    MessageBox.Show("Document does not belong to a GIT repository", "GIT information not found",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            Refresh();
        }

        /// <summary>
        ///     Provides the repository related to the VersionSelector directory
        /// </summary>
        /// <returns></returns>
        public Repository Repository
        {
            get
            {
                Repository retVal = null;

                string directory = Path.GetDirectoryName(Dictionary.FilePath);
                while (retVal == null && !String.IsNullOrEmpty(directory))
                {
                    try
                    {
                        retVal = new Repository(directory);
                    }
                    catch (Exception)
                    {
                        directory = Path.GetDirectoryName(directory);
                    }
                }

                return retVal;
            }
        }

        private void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            RefreshModel();
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}