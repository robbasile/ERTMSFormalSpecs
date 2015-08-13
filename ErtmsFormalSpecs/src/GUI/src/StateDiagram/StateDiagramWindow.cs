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

using DataDictionary;
using DataDictionary.Constants;
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Variables;
using GUI.BoxArrowDiagram;
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
    }
}