using System.Windows.Forms;
using DataDictionary;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.MoreInfoView
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     The element for which this message window is built
        /// </summary>
        private TextualExplain Model { get; set; }

        private string EmptyRTF { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();
            EmptyRTF = moreInfoRichTextBox.Rtf;

            moreInfoRichTextBox.ReadOnly = true;
            moreInfoRichTextBox.Enabled = true;

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
        /// <param name="model"></param>
        public void SetModel(TextualExplain model)
        {
            Model = model;
            RefreshModel();
        }

        /// <summary>
        ///     Refreshes the displayed messages according to the window model
        /// </summary>
        public override void RefreshModel()
        {
            ModelElement.DontRaiseError(()=>{
                moreInfoRichTextBox.Rtf = EmptyRTF;
                if (Model != null)
                {
                    moreInfoRichTextBox.Instance = Model as ModelElement;
                    moreInfoRichTextBox.Rtf = TextualExplainUtilities.Encapsule(Model.getExplain(true));
                }
                Refresh();
            });
        }
    }
}