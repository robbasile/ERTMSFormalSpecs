using System;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using EFSServiceClient.EFSService;

namespace EFSDriver
{
    public partial class EFSDriver : Form
    {
        /// <summary>
        ///     Access to EFS as a client
        /// </summary>
        private EFSServiceClient.EFSService.EFSServiceClient EFS { get; set; }

        /// <summary>
        ///     The client ID for EFS Service
        /// </summary>
        private int ClientId { get; set; }

        /// <summary>
        ///     Handles the background process
        /// </summary>
        private void HandleBackgroundProcess()
        {
            bool goOn = true;
            while (goOn)
            {
                if (cycleCheckBox.Checked)
                {
                    try
                    {
                        EstablishCommunication();
                        Invoke((MethodInvoker) delegate { variableValueTextBox.Enabled = true; });

                        EFS.Cycle(ClientId, Step.CleanUp);
                        Invoke((MethodInvoker) delegate { SetVariableValue(); });

                        EFS.Cycle(ClientId, Step.UpdateOutput);
                        Invoke((MethodInvoker) delegate { UpdateVariableValue(); });
                    }
                    catch (InvalidOperationException)
                    {
                        goOn = false;
                    }
                    catch (Exception)
                    {
                        CommunicationEstablished = false;
                    }
                }
                else
                {
                    Invoke((MethodInvoker) delegate { variableValueTextBox.Enabled = false; });
                    ;
                }
            }
        }

        /// <summary>
        ///     Indicates whether the communication has been established
        /// </summary>
        private bool CommunicationEstablished { get; set; }

        /// <summary>
        ///     Establishes the communication with the EFS Server
        /// </summary>
        private void EstablishCommunication()
        {
            while (!CommunicationEstablished)
            {
                EFS = new EFSServiceClient.EFSService.EFSServiceClient();
                try
                {
                    EFS.Open();
                    ClientId = EFS.ConnectUsingDefaultValues(false);

                    CommunicationEstablished = true;
                }
                catch (CommunicationException)
                {
                    Thread.Sleep(100);
                }
            }
        }

        private Thread BackgroundThread { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public EFSDriver()
        {
            InitializeComponent();
            TopMost = true;
            cycleCheckBox.Checked = false;

            BackgroundThread = new Thread((ThreadStart) HandleBackgroundProcess);
            BackgroundThread.CurrentCulture = CultureInfo.InvariantCulture;
            BackgroundThread.Start();
        }

        /// <summary>
        ///     Destructor
        /// </summary>
        ~EFSDriver()
        {
            BackgroundThread.Abort();
            EFS.Close();
        }

        /// <summary>
        ///     Updates the value of the variable referenced by the variable name text box
        /// </summary>
        private void UpdateVariableValue()
        {
            try
            {
                if (!string.IsNullOrEmpty(variableNameTextBox.Text))
                {
                    Value value = EFS.GetExpressionValue(variableNameTextBox.Text);
                    DisplayVariableValue(value);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     Indicates that changes in the UI should be propagated in the EFS system
        /// </summary>
        private bool Propagate = true;

        /// <summary>
        ///     Displays the value of the variable provided
        /// </summary>
        /// <param name="value"></param>
        private void DisplayVariableValue(Value value)
        {
            try
            {
                Propagate = false;
                if (value != null)
                {
                    variableValueTextBox.Text = value.DisplayValue();
                }
                else
                {
                    variableValueTextBox.Text = "<empty>";
                }
            }
            finally
            {
                Propagate = true;
            }
        }

        /// <summary>
        ///     The new value to be assigned to the variable
        /// </summary>
        private string NewVariableValue { get; set; }

        /// <summary>
        ///     Handles a change of the text box for the variable value.
        ///     Does not propagate the change if this change was due to DisplayVariableValue (propagate = false)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void variableValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Propagate)
            {
                // For now, do not change the variable value
                NewVariableValue = null;
            }
        }

        /// <summary>
        ///     Changes the EFS variable value if the new variable value is not null.
        ///     NewVariableValue is changed when the user changes the value of the textbox
        /// </summary>
        private void SetVariableValue()
        {
            if (NewVariableValue != null)
            {
                Value val = EFS.GetVariableValue(variableNameTextBox.Text);
                IntValue intValue = val as IntValue;
                if (intValue != null)
                {
                    intValue.Image = NewVariableValue;
                    EFS.SetVariableValue(variableNameTextBox.Text, intValue);
                }

                DoubleValue doubleValue = val as DoubleValue;
                if (doubleValue != null)
                {
                    doubleValue.Image = NewVariableValue;
                    EFS.SetVariableValue(variableNameTextBox.Text, doubleValue);
                }

                NewVariableValue = null;
            }
        }
    }
}