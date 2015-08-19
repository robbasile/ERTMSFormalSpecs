using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace GUI
{
    public partial class AboutDialogBox : Form
    {
        private class AboutData
        {
            // ReSharper disable once UnusedMember.Local
            public string Version
            {
                get
                {
                    string retVal;

                    Assembly assembly = Assembly.GetExecutingAssembly();
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                    retVal = fvi.FileVersion;

                    return retVal;
                }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public AboutDialogBox()
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = new AboutData();
        }
    }
}