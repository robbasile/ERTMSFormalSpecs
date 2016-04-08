using System;
using System.Drawing;
using System.Text;
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
            /// <param name="identifyingMessage"></param>
            protected HandleTextChange(ModelElement instance, string identifyingMessage)
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

            /// <summary>
            ///     Removes all useless characters from a source string
            /// </summary>
            /// <param name="source"></param>
            /// <returns></returns>
            public string RemoveUselessCharacters(string source)
            {
                StringBuilder retVal = new StringBuilder();

                if (source != null)
                {
                    foreach (char c in source)
                    {
                        if (c != '\r')
                        {
                            retVal.Append(c);
                        }
                    }
                }

                return retVal.ToString();
            }
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

            editorTextBox.TextBox.TextChanged += TextChangedHandler;
            editorTextBox.TextBox.KeyUp += TextBox_KeyUp;
            Text = EditorName;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (DockState == DockState.Float || DockState == DockState.Unknown)
                    {
                        _textChangeHandler.SetText(editorTextBox.TextBox.Text);
                        Close();
                        e.Handled = true;
                    }
                    break;
            }
        }

        /// <summary>
        ///     Called when the text has been changed in the inner text box
        ///     This updates the instance according to the __textChangeHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextChangedHandler(object sender, EventArgs e)
        {
            if (_textChangeHandler != null)
            {
                _textChangeHandler.SetText(editorTextBox.TextBox.Text);
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
        /// Indicates whether syntax highlighting is available
        /// </summary>
        public bool SyntaxHighlight
        {
            set { editorTextBox.EditionTextBox.ApplyPatterns = value; }
        }

        /// <summary>
        ///     The delegate method that need to be called when the text of the text box has been changed
        /// </summary>
        private HandleTextChange _textChangeHandler;

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
            _textChangeHandler = handleTextChange;
            if (_textChangeHandler != null)
            {
                editorTextBox.Enabled = true;
                if (_textChangeHandler.Instance != null)
                {
                    Text = _textChangeHandler.IdentifyingMessage + @" " + _textChangeHandler.Instance.FullName;
                    editorTextBox.Instance = _textChangeHandler.Instance;
                }
                else
                {
                    Text = _textChangeHandler.IdentifyingMessage;                    
                }
            }
            else
            {
                _textChangeHandler = null;
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
            if (_textChangeHandler != null)
            {
                string newValue = _textChangeHandler.GetText();
                if (newValue != Value && !(string.IsNullOrEmpty(newValue) && string.IsNullOrEmpty(Value)))
                {
                    int start = editorTextBox.TextBox.SelectionStart;
                    Value = newValue;
                    editorTextBox.TextBox.SelectionStart = start;
                }
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
                editorTextBox.TextBox.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
                editorTextBox.TextBox.Text = value;
                editorTextBox.TextBox.ProcessAllLines();
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

                if (_textChangeHandler != null)
                {
                    retVal = _textChangeHandler.Instance;
                }

                return retVal;
            }
        }
    }
}