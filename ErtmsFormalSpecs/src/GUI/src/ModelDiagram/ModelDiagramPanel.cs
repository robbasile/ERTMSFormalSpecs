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

using System;
using System.Collections.Generic;
using System.Drawing;
using DataDictionary;
using DataDictionary.Functions;
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Variables;
using GUI.BoxArrowDiagram;
using GUI.ModelDiagram.Arrows;
using GUI.ModelDiagram.Boxes;
using Utils;
using Enum = DataDictionary.Types.Enum;
using Type = DataDictionary.Types.Type;

namespace GUI.ModelDiagram
{
    /// <summary>
    ///     The panel used to display model elements (types, variables, rules, ...)
    /// </summary>
    public class ModelDiagramPanel : BoxArrowPanel<IModelElement, IGraphicalDisplay, ModelArrow>
    {
        /// <summary>
        ///     The namespace for which this panel is built
        /// </summary>
        public NameSpace NameSpace
        {
            get { return Model as NameSpace; }
            set { Model = value; }
        }

        /// <summary>
        ///     The dictionary for which this panel is built
        /// </summary>
        public Dictionary Dictionary
        {
            get { return Model as Dictionary; }
            set { Model = value; }
        }

        /// <summary>
        ///     The function for which this panel is built
        /// </summary>
        public Function Function
        {
            get { return Model as Function; }
            set { Model = value; }
        }

        /// <summary>
        ///     Method used to create a box
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override BoxControl<IModelElement, IGraphicalDisplay, ModelArrow> CreateBox(IGraphicalDisplay model)
        {
            ModelControl retVal = null;

            NameSpace nameSpace = model as NameSpace;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (retVal == null && nameSpace != null)
            {
                retVal = new NameSpaceModelControl(this, nameSpace);
            }

            Variable variable = model as Variable;
            if (retVal == null && variable != null)
            {
                retVal = new VariableModelControl(this, variable);
            }

            Function function = model as Function;
            if (retVal == null && function != null)
            {
                retVal = new FunctionModelControl(this, function);
            }

            Parameter parameter = model as Parameter;
            if (retVal == null && parameter != null)
            {
                retVal = new ParameterModelControl(this, parameter);
            }

            Case cas = model as Case;
            if (retVal == null && cas != null)
            {
                retVal = new CaseModelControl(this, cas);
            }

            PreCondition preCondition = model as PreCondition;
            if (retVal == null && preCondition != null)
            {
                retVal = new PreConditionModelControl(this, preCondition);
            }

            Procedure procedure = model as Procedure;
            if (procedure != null)
            {
                retVal = new ProcedureModelControl(this, procedure);
            }

            Range range = model as Range;
            if (range != null)
            {
                retVal = new RangeModelControl(this, range);
            }

            Enum enumeration = model as Enum;
            if (enumeration != null)
            {
                retVal = new EnumModelControl(this, enumeration);
            }

            Collection collection = model as Collection;
            if (collection != null)
            {
                retVal = new CollectionModelControl(this, collection);
            }

            StateMachine stateMachine = model as StateMachine;
            if (stateMachine != null)
            {
                retVal = new StateMachineModelControl(this, stateMachine);
            }

            Structure structure = model as Structure;
            if (structure != null)
            {
                if (structure.IsAbstract)
                {
                    retVal = new InterfaceModelControl(this, structure);
                }
                else
                {
                    retVal = new StructureModelControl(this, structure);
                }
            }

            Rule rule = model as Rule;
            if (rule != null)
            {
                retVal = new RuleModelControl(this, rule);
            }

            return retVal;
        }

        /// <summary>
        ///     Method used to create an arrow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ArrowControl<IModelElement, IGraphicalDisplay, ModelArrow> CreateArrow(ModelArrow model)
        {
            return new ModelArrowControl(this, model);
        }

        /// <summary>
        ///     Provides the boxes representing the models displayed in this panel
        /// </summary>
        /// <returns></returns>
        public override List<IGraphicalDisplay> GetBoxes()
        {
            List<IGraphicalDisplay> retVal = new List<IGraphicalDisplay>();

            if (Dictionary != null)
            {
                BuildDictionaryBoxes(retVal);
            }

            if (NameSpace != null)
            {
                BuildNameSpaceBoxes(retVal);
            }

            if (Function != null)
            {
                BuildFunctionBoxes(retVal);
            }

            return retVal;
        }

        /// <summary>
        /// Builds the boxes representing the contents of a dictionary
        /// </summary>
        /// <param name="retVal"></param>
        private void BuildDictionaryBoxes(ICollection<IGraphicalDisplay> retVal)
        {
            foreach (NameSpace nameSpace in Dictionary.NameSpaces)
            {
                retVal.Add(nameSpace);
            }
        }

        /// <summary>
        /// Builds the boxes representing the contents of a namespace
        /// </summary>
        /// <param name="retVal"></param>
        private void BuildNameSpaceBoxes(ICollection<IGraphicalDisplay> retVal)
        {
            foreach (NameSpace nameSpace in NameSpace.NameSpaces)
            {
                retVal.Add(nameSpace);
            }

            foreach (Type type in NameSpace.Types)
            {
                retVal.Add(type);
            }

            foreach (Variable variable in NameSpace.Variables)
            {
                retVal.Add(variable);
            }

            foreach (Function function in NameSpace.Functions)
            {
                retVal.Add(function);
            }

            foreach (Procedure procedure in NameSpace.Procedures)
            {
                retVal.Add(procedure);
            }

            foreach (Rule rule in NameSpace.Rules)
            {
                retVal.Add(rule);
            }
        }

        /// <summary>
        /// Builds the boxes representing the contents of a function
        /// </summary>
        /// <param name="retVal"></param>
        private void BuildFunctionBoxes(ICollection<IGraphicalDisplay> retVal)
        {
            retVal.Add(Function);
            foreach (Parameter parameter in Function.FormalParameters)
            {
                retVal.Add(parameter);
            }
            foreach (Case cas in Function.Cases)
            {
                retVal.Add(cas);
                foreach (PreCondition preCondition in cas.PreConditions)
                {
                    retVal.Add(preCondition);
                }
            }
        }

        /// <summary>
        ///     Provides the arrows between the models displayed in this panel
        /// </summary>
        /// <returns></returns>
        public override List<ModelArrow> GetArrows()
        {
            List<ModelArrow> retVal = new List<ModelArrow>();

            List<IGraphicalDisplay> boxes = GetBoxes();
            foreach (IGraphicalDisplay item in boxes)
            {
                Variable variable = item as Variable;
                if (variable != null && variable.Type != null)
                {
                    retVal.Add(new VariableTypeArrow(variable, variable.Type, variable));
                }

                Collection collection = item as Collection;
                if (collection != null && collection.Type != null)
                {
                    retVal.Add(new CollectionTypeArrow(collection, collection.Type, collection));
                }

                Structure structure = item as Structure;
                if (structure != null)
                {
                    foreach (StructureElement element in structure.Elements)
                    {
                        if (element.Type != null && !structure.StructureElementIsInherited(element))
                        {
                            if (boxes.Contains(element.Type))
                            {
                                retVal.Add(new ElementReferenceArrow(structure, element.Type, element));
                            }
                        }
                    }

                    foreach (Structure inheritedStructure in structure.Interfaces)
                    {
                        if (boxes.Contains(inheritedStructure))
                        {
                            retVal.Add(new InheritanceArrow(structure, inheritedStructure, inheritedStructure));
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Update the box location and compute the panel size
        /// </summary>
        protected override void UpdateBoxPosition()
        {
            if (Function != null)
            {
                CreateFunctionLayout();
            }
            else
            {
                base.UpdateBoxPosition();                
            }
        }

        /// <summary>
        /// Sets the default control size, according to its ocntents
        /// </summary>
        /// <param name="control"></param>
        private void SetDefaultSize(ModelControl control)
        {
            control.ComputedPositionAndSize = true;
            string text = control.TypedModel.GraphicalName;
            SizeF stringSize = GuiUtils.Graphics.MeasureString(text, control.Font);
            control.Size = new Size((int)stringSize.Width + 20, (int)stringSize.Height + 20);

            text = control.ModelName;
            stringSize = GuiUtils.Graphics.MeasureString(text, control.Font);
            control.Size = new Size(
                Math.Max(control.Size.Width, (int)stringSize.Width + 20),
                control.Size.Height);
        }

        /// <summary>
        /// Inbeds a control (the subControl) in a control
        /// </summary>
        /// <param name="control">The parent control</param>
        /// <param name="subControl">The control to inbed</param>
        /// <param name="location">The location where to inbed the sub control</param>
        private Point Inbed(ModelControl control, ModelControl subControl, Point location)
        {
            SetDefaultSize(subControl);
            subControl.Location = new Point(control.Location.X + location.X, control.Location.Y + location.Y);

            control.Size = new Size(
                Math.Max(control.Width, subControl.Width + location.X + 10), 
                subControl.Height + location.Y + 10
            );

            return new Point(location.X, location.Y + subControl.Size.Height + 5 ); 
        }

        /// <summary>
        /// Provides the bounding box considering the original bounding box and a control
        /// </summary>
        /// <param name="initialSize"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        private Size RegisterControl(Size initialSize, ModelControl control)
        {
            Size retVal = new Size(
                Math.Max(initialSize.Width, control.Location.X + control.Size.Width + 20),
                Math.Max(initialSize.Height, control.Location.Y + control.Size.Height + 20)
            );

            return retVal;
        }

        /// <summary>
        /// Creates the layout for a function contents
        /// </summary>
        private void CreateFunctionLayout()
        {
            Size panelSize = new Size(0,0);

            // Setup the function control location and size
            FunctionModelControl functionControl = (FunctionModelControl) GetBoxControl(Function);
            SetDefaultSize(functionControl);
            functionControl.Location = new Point(10, 10);
            functionControl.GraphicalNamePosition = ModelControl.PositionEnum.None;
            panelSize = RegisterControl(panelSize, functionControl);

            // Compute the position automatically
            Point location = new Point(20, 20);
            foreach (Parameter parameter in Function.FormalParameters)
            {
                // Compute the parameter box size
                ParameterModelControl parameterControl = (ParameterModelControl) GetBoxControl(parameter);
                location = Inbed(functionControl, parameterControl, location);
                parameterControl.GraphicalNamePosition = ModelControl.PositionEnum.Top;
            }

            location = new Point(30, functionControl.Location.Y + functionControl.Size.Height + 10);
            foreach (Case cas in Function.Cases)
            {
                CaseModelControl caseControl = (CaseModelControl) GetBoxControl(cas);
                SetDefaultSize(caseControl);
                caseControl.Location = location;
                caseControl.GraphicalNamePosition = ModelControl.PositionEnum.Bottom;

                Point preContidionsLocation = new Point(20, 20);
                foreach (PreCondition preCondition in cas.PreConditions)
                {
                    PreConditionModelControl preConditionControl =
                        (PreConditionModelControl) GetBoxControl(preCondition);
                    preContidionsLocation = Inbed(caseControl, preConditionControl, preContidionsLocation);
                    preConditionControl.GraphicalNamePosition = ModelControl.PositionEnum.Top;
                }

                SizeF stringSize = GuiUtils.Graphics.MeasureString(cas.ExpressionText, caseControl.Font);
                caseControl.Size = new Size(caseControl.Size.Width, caseControl.Size.Height + (int) stringSize.Height);
                panelSize = RegisterControl(panelSize, caseControl);

                // Prepare for next loop iteration
                location.Y = location.Y + caseControl.Size.Height + 10;
            }

            pictureBox.Size = MaxSize(panelSize, Size);
        }
    }
}