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

using System.Collections.Generic;
using DataDictionary.Interpreter;
using DataDictionary.Rules;
using DataDictionary.Variables;
using GUI.BoxArrowDiagram;
using Utils;
using Dictionary = DataDictionary.Dictionary;
using State = DataDictionary.Constants.State;
using StateMachine = DataDictionary.Types.StateMachine;

namespace GUI.StateDiagram
{
    public class StatePanel : BoxArrowPanel<StateMachine, State, Transition>
    {
        /// <summary>
        ///     Method used to create a box
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override BoxControl<StateMachine, State, Transition> CreateBox(State model)
        {
            return new StateControl(this, model);
        }

        /// <summary>
        ///     Method used to create an arrow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ArrowControl<StateMachine, State, Transition> CreateArrow(Transition model)
        {
            return new TransitionControl(this, model);
        }


        /// <summary>
        ///     Sets the state machine type
        /// </summary>
        /// <param name="stateMachine"></param>
        public void SetStateMachine(StateMachine stateMachine)
        {
            Model = stateMachine;

            Model = stateMachine;
            RefreshControl();
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

            Model = Model;
            if (stateMachine != null)
            {
                StateMachineVariableExpression =
                    new Parser().Expression(EnclosingFinder<Dictionary>.find(stateMachine),
                        stateMachine.FullName);
            }
            else
            {
                StateMachineVariableExpression = null;
            }
            RefreshControl();
        }

        /// <summary>
        ///     The expressiong required to get the state machine variable (if any) displayed by this panel
        /// </summary>
        private Expression StateMachineVariableExpression { get; set; }

        /// <summary>
        ///     Provides the state machine variable (if any)
        /// </summary>
        public IVariable StateMachineVariable
        {
            get
            {
                IVariable retVal = null;

                if (StateMachineVariableExpression != null)
                {
                    retVal = StateMachineVariableExpression.GetVariable(new InterpretationContext());
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the boxes that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<State> GetBoxes()
        {
            List<State> retVal = new List<State>();

            if (Model != null)
            {
                foreach (State state in Model.States)
                {
                    retVal.Add(state);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the arrows that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<Transition> GetArrows()
        {
            List<Transition> retVal = new List<Transition>();

            if (Model != null)
            {
                retVal = Model.Transitions;
            }

            return retVal;
        }

        /// <summary>
        ///     Factory for BoxEditor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override BoxEditor<StateMachine, State, Transition> CreateBoxEditor(
            BoxControl<StateMachine, State, Transition> control)
        {
            return new StateEditor(control);
        }


        /// <summary>
        ///     Factory for arrow editor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override ArrowEditor<StateMachine, State, Transition> CreateArrowEditor(
            ArrowControl<StateMachine, State, Transition> control)
        {
            return new TransitionEditor(control);
        }

        /// <summary>
        /// Selects the graphical element for the model provided
        /// </summary>
        /// <param name="model"></param>
        public override void SelectModel(object model)
        {
            RuleCondition ruleCondition = EnclosingFinder<RuleCondition>.find(model as IModelElement, true);
            if (ruleCondition != null)
            {
                base.SelectModel(ruleCondition);
            }
            else
            {
                State state = EnclosingFinder<State>.find(model as IModelElement, true);

                if (state != null)
                {
                    base.SelectModel(state);
                }                
            }
        }
    }
}