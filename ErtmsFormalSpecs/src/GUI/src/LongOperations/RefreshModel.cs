using System.Windows.Forms;

namespace GUI.LongOperations
{
    /// <summary>
    ///     Refreshes the model
    /// </summary>
    public class RefreshModel : BaseLongOperation
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public RefreshModel()
        {
        }

        public override void ExecuteWork()
        {
            GUIUtils.MDIWindow.Invoke((MethodInvoker)GUIUtils.MDIWindow.RefreshModel);            
        }
    }
}