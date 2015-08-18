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

using System.Windows.Forms;
using DataDictionary.Constants;
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Variables;
using GUI.BoxArrowDiagram;
using GUI.Properties;

namespace GUI.StateDiagram
{
    public class StateControl : BoxControl<StateMachine, State, Transition>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="model"></param>
        public StateControl(StatePanel panel, State model)
            : base(panel, model)
        {
        }

        /// <summary>
        ///     Indicates that the box should be displayed in the ACTIVE color
        /// </summary>
        /// <returns></returns>
        public override bool IsActive()
        {
            bool retVal = base.IsActive();

            if (!retVal)
            {
                StatePanel panel = (StatePanel) Panel;
                IVariable variable = panel.StateMachineVariable;
                if (variable != null && panel.Model.Contains(TypedModel, variable.Value))
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        public override void HandleDoubleClick(object sender, MouseEventArgs mouseEventArgs)
        {
            base.HandleDoubleClick(sender, mouseEventArgs);

            StatePanel panel = (StatePanel) Panel;
            if (panel != null)
            {
                StateDiagramWindow window = new StateDiagramWindow();
                GuiUtils.MdiWindow.AddChildWindow(window);
                window.StatePanel.SetStateMachine(panel.StateMachineVariable, TypedModel.StateMachine);
                window.Text = TypedModel.Name + @" " + Resources.StateControl_HandleMouseDoubleClick_state_diagram;
            }
        }
    }
}