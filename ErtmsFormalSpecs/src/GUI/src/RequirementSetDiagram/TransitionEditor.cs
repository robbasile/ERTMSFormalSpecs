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
using DataDictionary.Specification;
using GUI.BoxArrowDiagram;
using GUI.Converters;

namespace GUI.RequirementSetDiagram
{
    /// <summary>
    ///     An arrow editor
    /// </summary>
    public class TransitionEditor : ArrowEditor<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy>
    {
        protected class InternalRequirementSetTypeConverter : RequirementSetTypeConverter
        {
            public override StandardValuesCollection
                GetStandardValues(ITypeDescriptorContext context)
            {
                TransitionEditor instance = (TransitionEditor)context.Instance;
                RequirementSetPanel panel = (RequirementSetPanel)instance.Control.Panel;
                return GetValues(panel.Model);
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="control"></param>
        public TransitionEditor(ArrowControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> control)
            : base(control)
        {
        }

        [Category("Description"), TypeConverter(typeof(InternalRequirementSetTypeConverter))]
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
                RequirementSetDependancyControl transitionControl = (RequirementSetDependancyControl)Control;
                IHoldsRequirementSets enclosing = transitionControl.TypedModel.Source.Enclosing as IHoldsRequirementSets;
                if (enclosing != null)
                {
                    RequirementSet newSource = enclosing.findRequirementSet(value, false);
                    if (newSource != null)
                    {
                        Control.SetInitialBox(newSource);
                    }
                }
            }
        }

        [Category("Description"), TypeConverter(typeof(InternalRequirementSetTypeConverter))]
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
                RequirementSetDependancyControl transitionControl = (RequirementSetDependancyControl)Control;
                IHoldsRequirementSets enclosing = transitionControl.TypedModel.Source.Enclosing as IHoldsRequirementSets;
                if (enclosing != null)
                {
                    RequirementSet newTarget = enclosing.findRequirementSet(value, false);
                    if (newTarget != null)
                    {
                        Control.SetTargetBox(newTarget);
                    }
                }
            }
        }
    }
}
