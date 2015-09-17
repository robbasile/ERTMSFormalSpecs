using System;
using System.Windows.Forms;
using DataDictionary.Constants;
using StateMachine = DataDictionary.Types.StateMachine;

namespace GUI.DataDictionaryView
{
    public partial class SelectStartAndTargetStateForTransition : Form
    {
        /// <summary>
        /// Indicates that the OK button has been clicked
        /// </summary>
        public bool OkCkicked { get; private set; }

        public SelectStartAndTargetStateForTransition()
        {
            InitializeComponent();
            OkCkicked = false;
        }

        /// <summary>
        /// Setups the combo boxes according to the state machine and (optionally start and end states)
        /// </summary>
        /// <param name="stateMachine"></param>
        /// <param name="initialState"></param>
        /// <param name="endState"></param>
        public void SetStateMachine(StateMachine stateMachine, State initialState = null, State endState = null)
        {
            startStatesComboBox.Items.Clear();
            endStatesComboBox.Items.Clear();
            foreach (State state in stateMachine.States)
            {
                startStatesComboBox.Items.Add(state.Name);
                endStatesComboBox.Items.Add(state.Name);
            }

            if (initialState != null)
            {
                startStatesComboBox.Text = initialState.Name;
            }

            if (endState != null)
            {
                endStatesComboBox.Text = endState.Name;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            OkCkicked = true;
            Close();
        }

        /// <summary>
        /// The start state name
        /// </summary>
        public string StartStateName { get { return startStatesComboBox.Text; } }

        /// <summary>
        /// The end state name
        /// </summary>
        public string EndStateName { get { return endStatesComboBox.Text; } }
    }
}
