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
    public class RequirementSetDiagramWindow : BoxArrowWindow<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy>
    {
        /// <summary>
        ///     The panel used to display the state diagram
        /// </summary>
        public RequirementSetPanel Panel
        {
            get { return (RequirementSetPanel) BoxArrowContainerPanel; }
        }

        /// <summary>
        ///     Sets the system for this diagram
        /// </summary>
        /// <param name="enclosing"></param>
        public void SetEnclosing(IHoldsRequirementSets enclosing)
        {
            Model = enclosing;

            Panel.Model = enclosing;
        }

        public override BoxArrowPanel<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> CreatePanel()
        {
            BoxArrowPanel<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> retVal = new RequirementSetPanel();

            return retVal;
        }

        /// <summary>
        ///     A box editor
        /// </summary>
        protected class RequirementSetEditor : BoxEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="control"></param>
            public RequirementSetEditor(BoxControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> control)
                : base(control)
            {
            }

            [Category("Related Requirements behaviour")]
            public bool Recursive
            {
                get { return Control.Model.getRecursiveSelection(); }
                set { Control.Model.setRecursiveSelection(value); }
            }

            [Category("Related Requirements behaviour")]
            public bool Default
            {
                get { return Control.Model.getDefault(); }
                set { Control.Model.setDefault(value); }
            }
        }

        /// <summary>
        ///     Factory for BoxEditor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override BoxEditor CreateBoxEditor(BoxControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> control)
        {
            BoxEditor retVal = new RequirementSetEditor(control);

            return retVal;
        }

        protected class InternalRequirementSetTypeConverter : RequirementSetTypeConverter
        {
            public override StandardValuesCollection
                GetStandardValues(ITypeDescriptorContext context)
            {
                TransitionEditor instance = (TransitionEditor) context.Instance;
                RequirementSetPanel panel = (RequirementSetPanel) instance.Control.BoxArrowPanel;
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
            public TransitionEditor(ArrowControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> control)
                : base(control)
            {
            }

            [Category("Description"), TypeConverter(typeof (InternalRequirementSetTypeConverter))]
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
                    RequirementSetDependancyControl transitionControl = (RequirementSetDependancyControl) Control;
                    IHoldsRequirementSets enclosing = transitionControl.Model.Source.Enclosing as IHoldsRequirementSets;
                    if (enclosing != null)
                    {
                        RequirementSet newSource = enclosing.findRequirementSet(value, false);
                        if (newSource != null)
                        {
                            Control.SetInitialBox(newSource);
                            Control.RefreshControl();
                        }
                    }
                }
            }

            [Category("Description"), TypeConverter(typeof (InternalRequirementSetTypeConverter))]
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
                    RequirementSetDependancyControl transitionControl = (RequirementSetDependancyControl) Control;
                    IHoldsRequirementSets enclosing = transitionControl.Model.Source.Enclosing as IHoldsRequirementSets;
                    if (enclosing != null)
                    {
                        RequirementSet newTarget = enclosing.findRequirementSet(value, false);
                        if (newTarget != null)
                        {
                            Control.SetTargetBox(newTarget);
                            Control.RefreshControl();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Factory for arrow editor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override ArrowEditor CreateArrowEditor(ArrowControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> control)
        {
            ArrowEditor retVal = new TransitionEditor(control);

            return retVal;
        }
    }
}