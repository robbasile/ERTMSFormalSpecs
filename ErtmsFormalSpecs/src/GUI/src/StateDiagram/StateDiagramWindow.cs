// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System.ComponentModel;
using DataDictionary;
using DataDictionary.Constants;
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Variables;
using GUI.BoxArrowDiagram;
using GUI.Converters;
using Utils;

namespace GUI.StateDiagram
{
    public partial class StateDiagramWindow : BoxArrowWindow<State, Transition>
    {
        /// <summary>
        ///     The state machine currently displayed
        /// </summary>
        public StateMachine StateMachine { get; private set; }

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public StateDiagramWindow()
            : base()
        {
        }

        /// <summary>
        ///     The panel used to display the state diagram
        /// </summary>
        private StatePanel StatePanel
        {
            get { return (StatePanel) BoxArrowContainerPanel; }
        }

        /// <summary>
        ///     Sets the state machine type
        /// </summary>
        /// <param name="stateMachine"></param>
        public void SetStateMachine(StateMachine stateMachine)
        {
            StateMachine = stateMachine;

            StatePanel.StateMachine = StateMachine;
            StatePanel.RefreshControl();
        }

        /// <summary>
        ///     Sets the state machine variable (and type)
        /// </summary>
        /// <param name="stateMachine">The state machine variable to display</param>
        /// <param name="stateMachineType">
        ///     The state machine type which should be displayed. If null, the default state machine is
        ///     displayed
        /// </param>
        public void SetStateMachine(IVariable stateMachine, StateMachine stateMachineType = null)
        {
            if (stateMachineType == null)
            {
                stateMachineType = stateMachine.Type as StateMachine;
            }

            if (stateMachineType != null)
            {
                StateMachine = stateMachineType;
            }

            StatePanel.StateMachine = StateMachine;
            if (stateMachine != null)
            {
                StatePanel.StateMachineVariableExpression =
                    EFSSystem.INSTANCE.Parser.Expression(EnclosingFinder<Dictionary>.find(stateMachine),
                        stateMachine.FullName);
            }
            else
            {
                StatePanel.StateMachineVariableExpression = null;
            }
            StatePanel.RefreshControl();
        }

        public override BoxArrowPanel<State, Transition> createPanel()
        {
            BoxArrowPanel<State, Transition> retVal = new StatePanel();

            return retVal;
        }

        /// <summary>
        ///     A box editor
        /// </summary>
        protected class StateEditor : BoxEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="control"></param>
            public StateEditor(BoxControl<State, Transition> control)
                : base(control)
            {
            }
        }

        /// <summary>
        ///     Factory for BoxEditor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override BoxEditor createBoxEditor(BoxControl<State, Transition> control)
        {
            BoxEditor retVal = new StateEditor(control);

            return retVal;
        }

        protected class InternalStateTypeConverter : StateTypeConverter
        {
            public override StandardValuesCollection
                GetStandardValues(ITypeDescriptorContext context)
            {
                TransitionEditor instance = (TransitionEditor) context.Instance;
                StatePanel panel = (StatePanel) instance.control.BoxArrowPanel;
                return GetValues(panel.StateMachine);
            }
        }

        /// <summary>
        ///     An arrow editor
        /// </summary>
        protected class TransitionEditor : ArrowEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="control"></param>
            public TransitionEditor(ArrowControl<State, Transition> control)
                : base(control)
            {
            }

            [Category("Description"), TypeConverter(typeof (InternalStateTypeConverter))]
            public string Source
            {
                get
                {
                    string retVal = "";

                    if (control.Model.Source != null)
                    {
                        retVal = control.Model.Source.Name;
                    }
                    return retVal;
                }
                set
                {
                    TransitionControl transitionControl = (TransitionControl) control;
                    StatePanel statePanel = (StatePanel) transitionControl.Panel;
                    State state = OverallStateFinder.INSTANCE.findByName(statePanel.StateMachine, value);
                    if (state != null)
                    {
                        control.SetInitialBox(state);
                        control.RefreshControl();
                    }
                }
            }

            [Category("Description"), TypeConverter(typeof (InternalStateTypeConverter))]
            public string Target
            {
                get
                {
                    string retVal = "";

                    if (control.Model != null && control.Model.Target != null)
                    {
                        retVal = control.Model.Target.Name;
                    }

                    return retVal;
                }
                set
                {
                    TransitionControl transitionControl = (TransitionControl) control;
                    StatePanel statePanel = (StatePanel) transitionControl.Panel;
                    State state = OverallStateFinder.INSTANCE.findByName(statePanel.StateMachine, value);
                    if (state != null)
                    {
                        control.SetTargetBox(state);
                        control.RefreshControl();
                    }
                }
            }
        }

        /// <summary>
        ///     Factory for arrow editor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override ArrowEditor createArrowEditor(ArrowControl<State, Transition> control)
        {
            ArrowEditor retVal = new TransitionEditor(control);

            return retVal;
        }
    }
}