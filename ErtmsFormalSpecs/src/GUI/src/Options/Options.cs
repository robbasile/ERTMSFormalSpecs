using System.ComponentModel;
using System.Windows.Forms;
using DataDictionary;
using GUI.Properties;

namespace GUI.Options
{
    public partial class Options : Form
    {
        private class SettingsEditor
        {
            [Category("Display")]
            [DisplayName("Enclosing messages")]
            [Description("Indicates that the enclosing messages should be displayed when selecting a model element")]
            // ReSharper disable once UnusedMember.Local
            public bool DisplayEnclosingMessages
            {
                get { return Settings.Default.DisplayEnclosingMessages; }
                set { Settings.Default.DisplayEnclosingMessages = value; }
            }

            [Category("Display")]
            [DisplayName("Requirements as list")]
            [Description(
                "When set to true, indicates that the requirements should only be displayed as a list of number, instead of the requirement number followed by the requirement text"
                )]
            // ReSharper disable once UnusedMember.Local
            public bool DisplayRequirementsAsList
            {
                get { return Settings.Default.DisplayRequirementsAsList; }
                set { Settings.Default.DisplayRequirementsAsList = value; }
            }

            [Category("Files")]
            [DisplayName("Lock opened files")]
            [Description(
                "When set to true, indicates that the files opened by EFS should be locked, which forbid other processes to access them"
                )]
            // ReSharper disable once UnusedMember.Local
            public bool LockOpenedFiles
            {
                get { return Settings.Default.LockOpenedFiles; }
                set { Settings.Default.LockOpenedFiles = value; }
            }

            [Category("Files")]
            [DisplayName("Convert obsote version of model file")]
            [Description(
                "When set to true, indicates that EFS should convert the format of obsolete version of EFS files. For instance, it will replace USING X with USING X IN Coll in all expressions."
                )]
            // ReSharper disable once UnusedMember.Local
            public bool ConvertObsoleteFiles
            {
                get { return Settings.Default.ConvertObsoleteVersionOfModelFile; }
                set { Settings.Default.ConvertObsoleteVersionOfModelFile = value; }
            }

            [Category("Display")]
            [DisplayName("Display all variables in structure editor")]
            [Description(
                "When set to true, indicates that all the variables should be displayed in the structure editor, even those which are empty"
                )]
            // ReSharper disable once UnusedMember.Local
            public bool DisplayAllVariablesInStructureEditor
            {
                get { return Settings.Default.DisplayAllVariablesInStructureEditor; }
                set { Settings.Default.DisplayAllVariablesInStructureEditor = value; }
            }

            [Category("Behaviour")]
            [DisplayName("Check parent relationship")]
            [Description(
                "When animating the model, verify the correctness of the 'parent' relation for each model element")]
            // ReSharper disable once UnusedMember.Local
            public bool CheckParentRelationship
            {
                get { return Settings.Default.CheckParentRelationship; }
                set { Settings.Default.CheckParentRelationship = value; }
            }

            [Category("Behaviour")]
            [DisplayName("Cache function")]
            [Description("When animating the model, cache the values computed by functions")]
            // ReSharper disable once UnusedMember.Local
            public bool CacheFunctions
            {
                get { return Settings.Default.CacheFunctions; }
                set { Settings.Default.CacheFunctions = value; }
            }

            [Category("Behaviour")]
            [DisplayName("Allow refactor")]
            [Description("Allow refactoring when moving/renaming elements")]
            // ReSharper disable once UnusedMember.Local
            public bool AllowRefactor
            {
                get { return Settings.Default.AllowRefactor; }
                set { Settings.Default.AllowRefactor = value; }
            }
        }

        public Options()
        {
            InitializeComponent();
            propertyGrid.SelectedObject = new SettingsEditor();
        }

        /// <summary>
        ///     Sets the settings according to the application data
        /// </summary>
        public static void SetSettings()
        {
            Settings settings = Settings.Default;

            EfsSystem.Instance.DisplayEnclosingMessages = settings.DisplayEnclosingMessages;
            EfsSystem.Instance.DisplayRequirementsAsList = settings.DisplayRequirementsAsList;
            EfsSystem.Instance.CheckParentRelationship = settings.CheckParentRelationship;
            EfsSystem.Instance.CacheFunctions = settings.CacheFunctions;
            Util.PleaseLockFiles = settings.LockOpenedFiles;

            settings.Save();
        }
    }
}