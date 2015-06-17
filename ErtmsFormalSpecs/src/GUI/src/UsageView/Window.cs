using System.Windows.Forms;
using DataDictionary;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.UsageView
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     The editor used to edit these properties
        /// </summary>
        private ModelElement Model { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            DockAreas = DockAreas.DockBottom;
        }

        /// <summary>
        ///     Handles the close event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            GUIUtils.MDIWindow.HandleSubWindowClosed(this);
        }

        /// <summary>
        ///     Sets the model element for which messages should be displayed
        /// </summary>
        /// <param name="editor"></param>
        public void SetModel(ModelElement model)
        {
            Model = model;
            RefreshModel();
        }

        /// <summary>
        ///     Refreshes the displayed messages according to the window model
        /// </summary>
        public override void RefreshModel()
        {
            usageTreeView.Root = Model;
            Refresh();
        }
    }
}