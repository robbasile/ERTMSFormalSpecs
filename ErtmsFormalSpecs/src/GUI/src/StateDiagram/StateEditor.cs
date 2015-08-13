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
    /// <summary>
    ///     A box editor
    /// </summary>
    public class StateEditor : BoxEditor<StateMachine, State, Transition>
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
}
