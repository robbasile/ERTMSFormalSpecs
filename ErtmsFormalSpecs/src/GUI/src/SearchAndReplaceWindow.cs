using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GUI
{
    public partial class SearchAndReplaceWindow : Form
    {
        /// <summary>
        /// Result of this dialog
        /// </summary>
        public DialogResult Result { get; set; }

        public SearchAndReplaceWindow()
        {
            InitializeComponent();
            Result = DialogResult.Abort;
        }

        public string SearchString
        {
            get {
                return searchTextBox.Text;
            }
        }

        public string ReplaceString
        {
            get
            {
                return replaceTextBox.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Result = DialogResult.OK;
            Close();
        }
    }
}
