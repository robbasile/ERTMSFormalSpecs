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
        /// The required size of the panel
        /// </summary>
        private Size PanelSize { get; set; }

        /// <summary>
        /// Updates the panel size according to the control
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private void RegisterControl(ModelControl control)
        {
            PanelSize = new Size(
                Math.Max(PanelSize.Width, control.Location.X + control.Size.Width + 20),
                Math.Max(PanelSize.Height, control.Location.Y + control.Size.Height + 20)
            );
        }

        /// <summary>
        /// Measures a string and updates the control according to it
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="color"></param>
        /// <param name="location"></param>
        /// <returns>the next location where data can be added</returns>
        private static Point SetText(ModelControl control, string text, Font font, Color color, Point location)
        {
            // Compute the size of the displayed text
            SizeF stringSize = GuiUtils.Graphics.MeasureString(text, font);
            control.Size = new Size(
                Math.Max(control.Size.Width, (int)stringSize.Width + 20),
                control.Size.Height + (int) stringSize.Height + 5);

            // Position the text
            control.Texts.Add(new ModelControl.TextPosition
            {
                Text = text,
                Font = font,
                Color = color,
                Location = location
            });

            // Returns the next location where data can be added
            return new Point(location.X, location.Y + (int) stringSize.Height + 5);
        }

        /// <summary>
        /// Sets the default control size, according to its ocntents
        /// </summary>
        /// <param name="control"></param>
        /// <param name="location"></param>
        /// <returns>return>The location where filling should be continued</returns>
        private Point SetSizeAndLocation(ModelControl control, Point location)
        {
            // Set the control location
            control.ComputedPositionAndSize = true;
            control.Location = location;
            Point retVal = control.Location;

            // Increase control size according to title
            retVal = SetText(
                control, 
                control.ModelName, 
                control.Bold, 
                Color.Black, 
                retVal);

            // Increase control size according to comment
            ICommentable commentable = control.TypedModel as ICommentable;
            if ( commentable != null )
            {
                retVal = SetText(
                    control, 
                    commentable.Comment, 
                    control.Italic, 
                    Color.Green, 
                    retVal);
            }

            // Registers the control to update the panel size
            RegisterControl(control);

            return retVal;
        }

        /// <summary>
        /// Inbeds a control (the subControl) in a control
        /// </summary>
        /// <param name="enclosingControl">The parent control</param>
        /// <param name="subControl">The control to inbed</param>
        /// <param name="location">The location where to inbed the sub control</param>
        /// <returns>return>The location where filling should be continued</returns>
        private Point Inbed(ModelControl enclosingControl, ModelControl subControl, Point location)
        {
            // Increase the enclosing control size 
            enclosingControl.Size = new Size(
                Math.Max(enclosingControl.Width, subControl.Width + location.X + 10), 
                enclosingControl.Size.Height + subControl.Size.Height + 10
            );
            RegisterControl(enclosingControl);

            return new Point(
                subControl.Location.X,
                subControl.Location.Y + subControl.Size.Height + 10); 
        }

        /// <summary>
        /// Creates the layout for a function contents
        /// </summary>
        private void CreateFunctionLayout()
        {
            PanelSize = new Size(0, 0);

            // Setup the function control location and size
            FunctionModelControl functionControl = (FunctionModelControl) GetBoxControl(Function);
            Point location = SetSizeAndLocation(functionControl, new Point(10, 10));

            // Compute the position automatically
            location = new Point(20, location.Y);
            foreach (Parameter parameter in Function.FormalParameters)
            {
                // Compute the parameter box size
                ParameterModelControl parameterControl = (ParameterModelControl) GetBoxControl(parameter);
                SetSizeAndLocation(parameterControl, location);
                location = Inbed(functionControl, parameterControl, location);
            }

            location = new Point(30, functionControl.Location.Y + functionControl.Size.Height + 10);
            foreach (Case cas in Function.Cases)
            {
                CaseModelControl caseControl = (CaseModelControl) GetBoxControl(cas);
                location = SetSizeAndLocation(caseControl, location);

                location = new Point(50, location.Y);
                foreach (PreCondition preCondition in cas.PreConditions)
                {
                    PreConditionModelControl preConditionControl =
                        (PreConditionModelControl) GetBoxControl(preCondition);

                    location = SetSizeAndLocation(preConditionControl, location);
                    SetText(preConditionControl, preCondition.ExpressionText, preConditionControl.Font, Color.Black, location);
                    location = Inbed(caseControl, preConditionControl, location);
                }

                // Sets the control contents
                location = new Point(30, location.Y);
                location = SetText(caseControl, caseControl.TypedModel.GraphicalName, caseControl.Font, Color.Black, location);

                // Prepare for next loop iteration
                location = new Point(30, location.Y + 10);
            }

            pictureBox.Size = MaxSize(PanelSize, Size);
        }
    }
}