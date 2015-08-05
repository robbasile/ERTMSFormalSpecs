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
    public class StateDiagramWindow : BoxArrowWindow<StateMachine, State, Transition>
    {
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
            Model = stateMachine;

            StatePanel.Model = stateMachine;
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
                Model = stateMachineType;
            }

            StatePanel.Model = Model;
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

        public override BoxArrowPanel<StateMachine, State, Transition> CreatePanel()
        {
            BoxArrowPanel<StateMachine, State, Transition> retVal = new StatePanel();

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
            public StateEditor(BoxControl<StateMachine, State, Transition> control)
                : base(control)
            {
            }
        }

        /// <summary>
        ///     Factory for BoxEditor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override BoxEditor CreateBoxEditor(BoxControl<StateMachine, State, Transition> control)
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
                StatePanel panel = (StatePanel) instance.Control.BoxArrowPanel;
                return GetValues(panel.Model);
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
            public TransitionEditor(ArrowControl<StateMachine, State, Transition> control)
                : base(control)
            {
            }

            [Category("Description"), TypeConverter(typeof (InternalStateTypeConverter))]
            public string Source
            {
                get
                {
                    string retVal = "";

                    if (Control.Model.Source != null)
                    {
                        retVal = Control.Model.Source.Name;
                    }
                    return retVal;
                }
                set
                {
                    TransitionControl transitionControl = (TransitionControl) Control;
                    StatePanel statePanel = (StatePanel) transitionControl.Panel;
                    State state = OverallStateFinder.INSTANCE.findByName(statePanel.Model, value);
                    if (state != null)
                    {
                        Control.SetInitialBox(state);
                        Control.RefreshControl();
                    }
                }
            }

            [Category("Description"), TypeConverter(typeof (InternalStateTypeConverter))]
            public string Target
            {
                get
                {
                    string retVal = "";

                    if (Control.Model != null && Control.Model.Target != null)
                    {
                        retVal = Control.Model.Target.Name;
                    }

                    return retVal;
                }
                set
                {
                    TransitionControl transitionControl = (TransitionControl) Control;
                    StatePanel statePanel = (StatePanel) transitionControl.Panel;
                    State state = OverallStateFinder.INSTANCE.findByName(statePanel.Model, value);
                    if (state != null)
                    {
                        Control.SetTargetBox(state);
                        Control.RefreshControl();
                    }
                }
            }
        }

        /// <summary>
        ///     Factory for arrow editor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override ArrowEditor CreateArrowEditor(ArrowControl<StateMachine, State, Transition> control)
        {
            ArrowEditor retVal = new TransitionEditor(control);

            return retVal;
        }
    }
}