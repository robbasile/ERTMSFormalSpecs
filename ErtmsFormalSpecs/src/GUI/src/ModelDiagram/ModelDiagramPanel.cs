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
using GUIUtils.Editor.Patterns;
using Utils;
using Action = DataDictionary.Rules.Action;
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
        ///     The procedure for which this panel is built
        /// </summary>
        public Procedure Procedure
        {
            get { return Model as Procedure; }
            set { Model = value; }
        }

        /// <summary>
        ///     The rule for which this panel is built
        /// </summary>
        public Rule Rule
        {
            get { return Model as Rule; }
            set { Model = value; }
        }

        /// <summary>
        ///     The rule condition for which this panel is built
        /// </summary>
        public RuleCondition RuleCondition
        {
            get { return Model as RuleCondition; }
            set { Model = value; }
        }

        /// <summary>
        /// Tokenizes EFS text
        /// </summary>
        public EfsRecognizer Recognizer { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ModelDiagramPanel()
        {
            Recognizer = new EfsRecognizer(Font);
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

            RuleCondition ruleCondition = model as RuleCondition;
            if (ruleCondition != null)
            {
                retVal = new RuleConditionModelControl(this, ruleCondition);
            }

            Action action = model as Action;
            if (action != null)
            {
                retVal = new ActionModelControl(this, action);
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

            if (Procedure != null)
            {
                BuildProcedureBoxes(retVal);
            }

            if (RuleCondition != null)
            {
                BuildRuleConditionBoxes(RuleCondition, retVal);                
            }

            if (Rule != null)
            {
                BuildRuleBoxes(Rule, retVal);
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
        /// Builds the boxes representing the contents of a procedure
        /// </summary>
        /// <param name="retVal"></param>
        private void BuildProcedureBoxes(ICollection<IGraphicalDisplay> retVal)
        {
            retVal.Add(Procedure);
            foreach (Parameter parameter in Procedure.FormalParameters)
            {
                retVal.Add(parameter);
            }
            foreach (Rule rule in Procedure.Rules)
            {
                BuildRuleBoxes(rule, retVal);
            }
        }

        /// <summary>
        /// Builds the boxes representing the contents of a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="retVal"></param>
        private void BuildRuleBoxes(Rule rule, ICollection<IGraphicalDisplay> retVal)
        {
            retVal.Add(rule);
            foreach (RuleCondition condition in rule.RuleConditions)
            {
                BuildRuleConditionBoxes(condition, retVal);
            }
        }

        /// <summary>
        /// Builds the boxes representing the contents of a rule condition
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="retVal"></param>
        private void BuildRuleConditionBoxes(RuleCondition condition, ICollection<IGraphicalDisplay> retVal)
        {
            retVal.Add(condition);
            foreach (PreCondition preCondition in condition.PreConditions)
            {
                retVal.Add(preCondition);
            }
            foreach (Action action in condition.Actions)
            {
                retVal.Add(action);
            }
            foreach (Rule subRule in condition.SubRules)
            {
                BuildRuleBoxes(subRule, retVal);
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
                CreateFunctionLayout(Function);
            }
            else if (Procedure != null)
            {
                PanelSize = new Size(0, 0);
                CreateProcedureLayout(Procedure);
            }
            else if (Rule != null)
            {
                PanelSize = new Size(0, 0);
                CreateRuleLayout(Rule, new Point(0, 0));
            }
            else if (RuleCondition != null)
            {
                PanelSize = new Size(0, 0);
                CreateRuleConditionLayout(RuleCondition, new Point(0, 0));
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
                Math.Max(control.Size.Width, (int) stringSize.Width + 20),
                Math.Max ( control.Size.Height, location.Y - control.Location.Y + (int) stringSize.Height + 5)
            );

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
            if (commentable != null)
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
        /// <param name="tab"></param>
        /// <returns>return>The location where filling should be continued</returns>
        private Point InbedTopDown(ModelControl enclosingControl, ModelControl subControl, Point location, bool tab = true)
        {
            int delta = tab ? 10 : 0;

            // Increase the enclosing control size 
            enclosingControl.Size = new Size(
                Math.Max(enclosingControl.Width, location.X - enclosingControl.Location.X + subControl.Width + 20),
                Math.Max(enclosingControl.Size.Height, subControl.Size.Height + location.Y - enclosingControl.Location.Y + 10)
            );
            RegisterControl(enclosingControl);

            return new Point(
                subControl.Location.X,
                subControl.Location.Y + subControl.Size.Height + delta);
        }

        /// <summary>
        /// Inbeds a control (the subControl) in a control
        /// </summary>
        /// <param name="enclosingControl">The parent control</param>
        /// <param name="subControl">The control to inbed</param>
        /// <param name="location">The location where to inbed the sub control</param>
        /// <param name="tab"></param>
        /// <returns>return>The location where filling should be continued</returns>
        private Point InbedLeftRight(ModelControl enclosingControl, ModelControl subControl, Point location, bool tab = true)
        {
            int delta = tab ? 10 : 0;

            // Increase the enclosing control size 
            enclosingControl.Size = new Size(
                Math.Max(enclosingControl.Size.Width, subControl.Width + subControl.Location.X - enclosingControl.Location.X + 10),
                Math.Max(enclosingControl.Size.Height, subControl.Size.Height + location.Y - enclosingControl.Location.Y + 10)
            );
            RegisterControl(enclosingControl);

            return new Point(
                subControl.Location.X + subControl.Width + delta,
                subControl.Location.Y);
        }

        /// <summary>
        /// Creates the layout for a function contents
        /// </summary>
        /// <param name="function"></param>
        private void CreateFunctionLayout(Function function)
        {
            PanelSize = new Size(0, 0);

            // Setup the function control location and size
            FunctionModelControl functionControl = (FunctionModelControl) GetBoxControl(function);
            Point location = SetSizeAndLocation(functionControl, new Point(10, 10));

            // Compute the position automatically
            location = new Point(20, location.Y);
            foreach (Parameter parameter in function.FormalParameters)
            {
                // Compute the parameter box size
                ParameterModelControl parameterControl = (ParameterModelControl) GetBoxControl(parameter);
                SetSizeAndLocation(parameterControl, location);
                location = InbedTopDown(functionControl, parameterControl, location);
            }

            location = new Point(30, functionControl.Location.Y + functionControl.Size.Height + 10);
            foreach (Case cas in function.Cases)
            {
                CaseModelControl caseControl = (CaseModelControl) GetBoxControl(cas);
                location = SetSizeAndLocation(caseControl, location);

                location = new Point(50, location.Y);
                foreach (PreCondition preCondition in cas.PreConditions)
                {
                    PreConditionModelControl preConditionControl =
                        (PreConditionModelControl) GetBoxControl(preCondition);

                    location = SetSizeAndLocation(preConditionControl, location);
                    SetText(preConditionControl, preCondition.ExpressionText, preConditionControl.Font,
                        Color.Transparent, location);
                    location = InbedTopDown(caseControl, preConditionControl, location);
                }

                // Sets the control contents
                location = new Point(30, location.Y);
                location = SetText(caseControl, caseControl.TypedModel.GraphicalName, caseControl.Font,
                    Color.Transparent, location);

                location = InbedTopDown(functionControl, caseControl, location);

                // Prepare for next loop iteration
                location = new Point(30, location.Y + 10);
            }

            pictureBox.Size = MaxSize(PanelSize, Size);
        }

        /// <summary>
        /// Creates the layout for a procedure contents
        /// </summary>
        /// <param name="procedure"></param>
        private void CreateProcedureLayout(Procedure procedure)
        {
            PanelSize = new Size(0, 0);

            // Setup the function control location and size
            ProcedureModelControl procedureControl = (ProcedureModelControl)GetBoxControl(procedure);
            Point location = SetSizeAndLocation(procedureControl, new Point(10, 10));

            // Compute the position automatically
            location = new Point(20, location.Y);
            foreach (Parameter parameter in procedure.FormalParameters)
            {
                // Compute the parameter box size
                ParameterModelControl parameterControl = (ParameterModelControl)GetBoxControl(parameter);
                SetSizeAndLocation(parameterControl, location);
                location = InbedTopDown(procedureControl, parameterControl, location);
            }

            location = new Point(30, procedureControl.Location.Y + procedureControl.Size.Height + 10);
            foreach (Rule rule in procedure.Rules)
            {
                RuleModelControl ruleControl = CreateRuleLayout(rule, location);
                location = InbedTopDown(procedureControl, ruleControl, location, false);
                location = new Point(location.X - 10, location.Y + 10);
            }

            pictureBox.Size = MaxSize(PanelSize, Size);
        }

        /// <summary>
        /// Creates the layout for a rule contents
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="location"></param>
        private RuleModelControl CreateRuleLayout(Rule rule, Point location)
        {
            // Setup the rule control location and size
            RuleModelControl ruleControl = (RuleModelControl) GetBoxControl(rule);
            if (ruleControl != null)
            {
                location = new Point(location.X + 10, location.Y);
                location = SetSizeAndLocation(ruleControl, location);

                // Compute the position automatically
                location = new Point(location.X + 10, location.Y);
                foreach (RuleCondition ruleCondition in rule.RuleConditions)
                {
                    RuleConditionModelControl ruleConditionControl = CreateRuleConditionLayout(ruleCondition, location);
                    if (ruleConditionControl != null)
                    {
                        location = InbedTopDown(ruleControl, ruleConditionControl, location, false);
                    }
                    location = new Point(location.X - 10, location.Y + 10);
                    ruleControl.Size = new Size(ruleControl.Size.Width, ruleControl.Size.Height + 20);
                }

                pictureBox.Size = MaxSize(PanelSize, Size);
            }

            return ruleControl;
        }

        /// <summary>
        /// Creates the layout for a rule condition contents
        /// </summary>
        /// <param name="ruleCondition"></param>
        /// <param name="location"></param>
        private RuleConditionModelControl CreateRuleConditionLayout(RuleCondition ruleCondition, Point location)
        {
            PanelSize = new Size(0, 0);

            // Setup the function control location and size
            RuleConditionModelControl ruleConditionControl = (RuleConditionModelControl) GetBoxControl(ruleCondition);
            if (ruleConditionControl != null)
            {
                location = new Point(location.X + 10, location.Y);
                location = SetSizeAndLocation(ruleConditionControl, location);

                // Compute the position automatically
                location = new Point(location.X + 30,
                    ruleConditionControl.Location.Y + ruleConditionControl.Size.Height + 10);
                foreach (PreCondition preCondition in ruleCondition.PreConditions)
                {
                    PreConditionModelControl preConditionControl =
                        (PreConditionModelControl)GetBoxControl(preCondition);

                    location = SetSizeAndLocation(preConditionControl, location);
                    SetText(preConditionControl, preCondition.ExpressionText, preConditionControl.Font, Color.Transparent,
                        location);
                    location = InbedTopDown(ruleConditionControl, preConditionControl, location);
                }

                location = new Point(location.X - 20, location.Y);
                foreach (Action action in ruleCondition.Actions)
                {
                    ActionModelControl actionControl = (ActionModelControl)GetBoxControl(action);

                    location = SetSizeAndLocation(actionControl, location);
                    SetText(actionControl, action.ExpressionText, actionControl.Font, Color.Transparent, location);
                    location = InbedTopDown(ruleConditionControl, actionControl, location);
                }

                foreach (Rule subRule in ruleCondition.SubRules)
                {
                    RuleModelControl ruleControl = CreateRuleLayout(subRule, location);
                    if (ruleControl != null)
                    {
                        location = InbedLeftRight(ruleConditionControl, ruleControl, location, false);
                    }
                }

                pictureBox.Size = MaxSize(PanelSize, Size);
            }

            return ruleConditionControl;
        }
    }
}
