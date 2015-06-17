using System;
using System.Windows.Forms;
using Utils;

namespace GUI.MessagesView
{
    public partial class MessageDetail : Form
    {
        public MessageDetail()
        {
            InitializeComponent();
        }

        public void SetMessage(ElementLog log)
        {
            MessageDetailTextBox.Text = log.Log;

            string caption = "";
            switch (log.Level)
            {
                case ElementLog.LevelEnum.Error:
                    caption = "Error found";
                    break;
                case ElementLog.LevelEnum.Warning:
                    caption = "Warning found";
                    break;
                case ElementLog.LevelEnum.Info:
                    caption = "Informative message found";
                    break;
            }
            Text = caption;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CopyToClipBoardButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(MessageDetailTextBox.Text);
            Close();
        }
    }
}