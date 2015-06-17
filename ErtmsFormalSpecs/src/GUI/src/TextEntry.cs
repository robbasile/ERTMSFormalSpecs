using System;
using System.Windows.Forms;

namespace GUI
{
    public partial class TextEntry : Form
    {
        public TextEntry()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        public String Value
        {
            get { return valueTextBox.Text; }
        }
    }
}