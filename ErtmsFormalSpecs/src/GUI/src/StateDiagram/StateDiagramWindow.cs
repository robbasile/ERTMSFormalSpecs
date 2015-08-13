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

using DataDictionary.Constants;
using DataDictionary.Rules;
using DataDictionary.Types;
using GUI.BoxArrowDiagram;

namespace GUI.StateDiagram
{
    public class StateDiagramWindow : BoxArrowWindow<StateMachine, State, Transition>
    {
        /// <summary>
        ///     The panel used to display the state diagram
        /// </summary>
        public StatePanel StatePanel
        {
            get { return (StatePanel) BoxArrowContainerPanel; }
        }

        public override BoxArrowPanel<StateMachine, State, Transition> CreatePanel()
        {
            return new StatePanel();
        }
    }
}