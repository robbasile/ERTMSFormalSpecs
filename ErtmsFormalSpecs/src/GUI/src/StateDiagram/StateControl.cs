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
using System.Windows.Forms;
using DataDictionary.Constants;
using DataDictionary.Rules;
using DataDictionary.Variables;
using GUI.BoxArrowDiagram;

namespace GUI.StateDiagram
{
    public partial class StateControl : BoxControl<State, Transition>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public StateControl()
            : base()
        {
            MouseDoubleClick += new MouseEventHandler(HandleMouseDoubleClick);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public StateControl(IContainer container)
            : base(container)
        {
            MouseDoubleClick += new MouseEventHandler(HandleMouseDoubleClick);
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
                if (variable != null && panel.StateMachine.Contains(Model, variable.Value))
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Handles a double click event on the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDoubleClick(object sender, MouseEventArgs e)
        {
            SelectBox();

            StatePanel panel = (StatePanel) Panel;
            if (panel != null)
            {
                StateDiagramWindow window = new StateDiagramWindow();
                GUIUtils.MDIWindow.AddChildWindow(window);
                window.SetStateMachine(panel.StateMachineVariable, Model.StateMachine);
                window.Text = Model.Name + " state diagram";
            }
        }
    }
}