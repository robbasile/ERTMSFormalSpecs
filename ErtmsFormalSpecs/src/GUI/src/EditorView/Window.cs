using System;
using System.Drawing;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Generated;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.EditorView
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     Indicates the actions to be performed to get the text from the instance and to set it into the instance
        /// </summary>
        public abstract class HandleTextChange
        {
            /// <summary>
            ///     The instance that is currently handled
            /// </summary>
            public ModelElement Instance { get; private set; }

            /// <summary>
            ///     The messages that identifies the action that is performed in the instance
            /// </summary>
            public string IdentifyingMessage { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="instance"></param>
            public HandleTextChange(ModelElement instance, string identifyingMessage)
            {
                Instance = instance;
                IdentifyingMessage = identifyingMessage;
            }

            /// <summary>
            ///     The way text is retrieved from the instance
            /// </summary>
            /// <returns></returns>
            public abstract string GetText();

            /// <summary>
            ///     The way text is set back in the instance
            /// </summary>
            /// <returns></returns>
            public abstract void SetText(string text);
        }

        /// <summary>
        ///     Indicates that only types should be considered
        /// </summary>
        public bool ConsiderOnlyTypes
        {
            set { editorTextBox.ConsiderOnlyTypes = value; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            editorTextBox.TextBox.TextChanged += new EventHandler(TextChangedHandler);
            editorTextBox.TextBox.KeyUp += new KeyEventHandler(TextBox_KeyUp);
            FormClosed += new FormClosedEventHandler(Window_FormClosed);
            Text = EditorName;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (DockState == DockState.Float || DockState == DockState.Unknown)
                    {
                        __textChangeHandler.SetText(editorTextBox.TextBox.Text);
                        Close();
                        e.Handled = true;
                    }
                    break;

                default:
                    break;
            }
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
        ///     Called when the text has been changed in the inner text box
        ///     This updates the instance according to the __textChangeHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextChangedHandler(object sender, EventArgs e)
        {
            if (__textChangeHandler != null)
            {
                __textChangeHandler.SetText(editorTextBox.TextBox.Text);
            }
        }

        /// <summary>
        ///     Indicates whether auto completion is available
        /// </summary>
        public bool AutoComplete
        {
            set { editorTextBox.AutoComplete = value; }
        }

        /// <summary>
        ///     Indicates whether auto completion is available
        /// </summary>
        public bool SyntaxHighlight
        {
            set { editorTextBox.EditionTextBox.ApplyPatterns = value; }
        }

        /// <summary>
        ///     The delegate method that need to be called when the text of the text box has been changed
        /// </summary>
        private HandleTextChange __textChangeHandler = null;

        /// <summary>
        ///     The name of the editor
        /// </summary>
        protected virtual string EditorName
        {
            get { return "Editor"; }
        }

        /// <summary>
        ///     The element on which this editor is built
        /// </summary>
        public void setChangeHandler(HandleTextChange handleTextChange)
        {
            __textChangeHandler = handleTextChange;
            if (__textChangeHandler != null && __textChangeHandler.Instance != null)
            {
                Text = __textChangeHandler.IdentifyingMessage + " " + __textChangeHandler.Instance.FullName;
                editorTextBox.Instance = __textChangeHandler.Instance;
                editorTextBox.Enabled = true;
            }
            else
            {
                __textChangeHandler = null;
                Text = EditorName;
                editorTextBox.Text = "";
                editorTextBox.Instance = null;
                editorTextBox.Enabled = false;
            }
            RefreshText();
        }

        /// <summary>
        ///     Refreshes the text of the text box
        /// </summary>
        public void RefreshText()
        {
            if (__textChangeHandler != null)
            {
                int start = editorTextBox.TextBox.SelectionStart;
                string newValue = __textChangeHandler.GetText();
                if (newValue != Value)
                {
                    Value = __textChangeHandler.GetText();
                }
                editorTextBox.TextBox.SelectionStart = start;
            }
        }

        /// <summary>
        ///     Indicates that the editor text box has the focus
        /// </summary>
        /// <returns></returns>
        public bool EditorTextBoxHasFocus()
        {
            return editorTextBox.TextBox.Focused;
        }

        /// <summary>
        ///     The textual value to edit
        /// </summary>
        public string Value
        {
            get { return editorTextBox.TextBox.Text; }
            set
            {
                editorTextBox.TextBox.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point,
                    ((byte) (0)));
                editorTextBox.TextBox.Text = value;
            }
        }

        /// <summary>
        ///     The instance currently edited by the editor
        /// </summary>
        public BaseModelElement Instance
        {
            get
            {
                BaseModelElement retVal = null;

                if (__textChangeHandler != null)
                {
                    retVal = __textChangeHandler.Instance;
                }

                return retVal;
            }
        }
    }
}