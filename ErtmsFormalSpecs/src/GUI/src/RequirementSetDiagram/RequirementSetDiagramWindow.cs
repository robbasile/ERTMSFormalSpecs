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
    public partial class RequirementSetDiagramWindow : BoxArrowWindow<RequirementSet, RequirementSetDependancy>
    {
        /// <summary>
        ///     The enclosing for which the requirement set diagram is built
        /// </summary>
        public IHoldsRequirementSets Enclosing { get; private set; }

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="system"></param>
        public RequirementSetDiagramWindow()
            : base()
        {
        }

        /// <summary>
        ///     The panel used to display the state diagram
        /// </summary>
        private RequirementSetPanel Panel
        {
            get { return (RequirementSetPanel) BoxArrowContainerPanel; }
        }

        /// <summary>
        ///     Sets the system for this diagram
        /// </summary>
        /// <param name="enclosing"></param>
        public void SetEnclosing(IHoldsRequirementSets enclosing)
        {
            Enclosing = enclosing;

            Panel.Enclosing = enclosing;
            Panel.RefreshControl();
        }

        public override BoxArrowPanel<RequirementSet, RequirementSetDependancy> createPanel()
        {
            BoxArrowPanel<RequirementSet, RequirementSetDependancy> retVal = new RequirementSetPanel();

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
            public RequirementSetEditor(BoxControl<RequirementSet, RequirementSetDependancy> control)
                : base(control)
            {
            }

            [Category("Related Requirements behaviour")]
            public bool Recursive
            {
                get { return control.Model.getRecursiveSelection(); }
                set { control.Model.setRecursiveSelection(value); }
            }

            [Category("Related Requirements behaviour")]
            public bool Default
            {
                get { return control.Model.getDefault(); }
                set { control.Model.setDefault(value); }
            }
        }

        /// <summary>
        ///     Factory for BoxEditor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override BoxEditor createBoxEditor(BoxControl<RequirementSet, RequirementSetDependancy> control)
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
                RequirementSetPanel panel = (RequirementSetPanel) instance.control.BoxArrowPanel;
                return GetValues(panel.Enclosing);
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
            public TransitionEditor(ArrowControl<RequirementSet, RequirementSetDependancy> control)
                : base(control)
            {
            }

            [Category("Description"), TypeConverter(typeof (InternalRequirementSetTypeConverter))]
            public string Source
            {
                get
                {
                    string retVal = "";

                    if (control.Model.Source != null)
                    {
                        retVal = control.Model.Source.Name;
                    }
                    return retVal;
                }
                set
                {
                    RequirementSetDependancyControl transitionControl = (RequirementSetDependancyControl) control;
                    IHoldsRequirementSets enclosing = transitionControl.Model.Source.Enclosing as IHoldsRequirementSets;
                    if (enclosing != null)
                    {
                        RequirementSet newSource = enclosing.findRequirementSet(value, false);
                        if (newSource != null)
                        {
                            control.SetInitialBox(newSource);
                            control.RefreshControl();
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

                    if (control.Model != null && control.Model.Target != null)
                    {
                        retVal = control.Model.Target.Name;
                    }

                    return retVal;
                }
                set
                {
                    RequirementSetDependancyControl transitionControl = (RequirementSetDependancyControl) control;
                    IHoldsRequirementSets enclosing = transitionControl.Model.Source.Enclosing as IHoldsRequirementSets;
                    if (enclosing != null)
                    {
                        RequirementSet newTarget = enclosing.findRequirementSet(value, false);
                        if (newTarget != null)
                        {
                            control.SetTargetBox(newTarget);
                            control.RefreshControl();
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
        protected override ArrowEditor createArrowEditor(ArrowControl<RequirementSet, RequirementSetDependancy> control)
        {
            ArrowEditor retVal = new TransitionEditor(control);

            return retVal;
        }
    }
}