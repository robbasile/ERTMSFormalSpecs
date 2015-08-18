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
using GUI.BoxArrowDiagram;
using GUI.Converters;

namespace GUI.StateDiagram
{
    /// <summary>
    ///     An arrow editor
    /// </summary>
    public class TransitionEditor : ArrowEditor<StateMachine, State, Transition>
    {
        protected class InternalStateTypeConverter : StateTypeConverter
        {
            public override StandardValuesCollection
                GetStandardValues(ITypeDescriptorContext context)
            {
                TransitionEditor instance = (TransitionEditor) context.Instance;
                StatePanel panel = (StatePanel) instance.Control.Panel;
                return GetValues(panel.Model);
            }
        }

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

                if (Control.TypedModel.Source != null)
                {
                    retVal = Control.TypedModel.Source.Name;
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
                }
            }
        }

        [Category("Description"), TypeConverter(typeof (InternalStateTypeConverter))]
        public string Target
        {
            get
            {
                string retVal = "";

                if (Control.Model != null && Control.TypedModel.Target != null)
                {
                    retVal = Control.TypedModel.Target.Name;
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
                }
            }
        }
    }
}