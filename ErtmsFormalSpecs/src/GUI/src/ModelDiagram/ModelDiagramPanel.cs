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
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Generated;
using GUI.BoxArrowDiagram;
using Utils;
using Collection = DataDictionary.Types.Collection;
using Dictionary = DataDictionary.Dictionary;
using Enum = DataDictionary.Types.Enum;
using Function = DataDictionary.Functions.Function;
using ModelElement = DataDictionary.ModelElement;
using NameSpace = DataDictionary.Types.NameSpace;
using Procedure = DataDictionary.Functions.Procedure;
using Range = DataDictionary.Types.Range;
using Rule = DataDictionary.Rules.Rule;
using StateMachine = DataDictionary.Types.StateMachine;
using Structure = DataDictionary.Types.Structure;
using StructureElement = DataDictionary.Types.StructureElement;
using Type = DataDictionary.Types.Type;
using Variable = DataDictionary.Variables.Variable;

namespace GUI.ModelDiagram
{
    /// <summary>
    ///     The panel used to display model elements (types, variables, rules, ...)
    /// </summary>
    public class ModelDiagramPanel : BoxArrowPanel<IModelElement, IGraphicalDisplay, ModelArrow>
    {
        private ToolStripMenuItem _addRangeMenuItem;

        /// <summary>
        ///     Adds the menu items related to this model element
        /// </summary>
        public override void InitializeStartMenu()
        {
            if ((NameSpace != null))
            {
                // 
                // Add range
                // 
                _addRangeMenuItem = new ToolStripMenuItem();
                _addRangeMenuItem.Text = "Add range";
                _addRangeMenuItem.Click += addRangeMenuItem_Click;
            }
        }

        /// <summary>
        ///     Adds a new range
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addRangeMenuItem_Click(object sender, EventArgs e)
        {
            Range range = (Range) acceptor.getFactory().createRange();
            range.Name = "Range";
            NameSpace.appendRanges(range);
            RefreshControl();
        }

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
        ///     Method used to create a box
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override BoxControl<IModelElement, IGraphicalDisplay, ModelArrow> CreateBox(IGraphicalDisplay model)
        {
            ModelControl retVal = null;

            NameSpace nameSpace = model as NameSpace;
            if (retVal == null && nameSpace != null)
            {
                retVal = new NameSpaceModelControl(nameSpace);
            }

            Variable variable = model as Variable;
            if (retVal == null && variable != null)
            {
                retVal = new VariableModelControl(variable);
            }

            Function function = model as Function;
            if (retVal == null && function != null)
            {
                retVal = new FunctionModelControl(function);
            }

            Procedure procedure = model as Procedure;
            if (procedure != null)
            {
                retVal = new ProcedureModelControl(procedure);
            }

            Range range = model as Range;
            if (range != null)
            {
                retVal = new RangeModelControl(range);
            }

            Enum enumeration = model as Enum;
            if (enumeration != null)
            {
                retVal = new EnumModelControl(enumeration);
            }

            Collection collection = model as Collection;
            if (collection != null)
            {
                retVal = new CollectionModelControl(collection);
            }

            StateMachine stateMachine = model as StateMachine;
            if (stateMachine != null)
            {
                retVal = new StateMachineModelControl(stateMachine);
            }

            Structure structure = model as Structure;
            if (structure != null)
            {
                if (structure.IsAbstract)
                {
                    retVal = new InterfaceModelControl(structure);
                }
                else
                {
                    retVal = new StructureModelControl(structure);
                }
            }

            Rule rule = model as Rule;
            if (rule != null)
            {
                retVal = new RuleModelControl(rule);
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
            ModelArrowControl retVal = new ModelArrowControl(model);

            return retVal;
        }

        /// <summary>
        ///     Provides the boxes representing the models displayed in this panel
        /// </summary>
        /// <returns></returns>
        public override List<IGraphicalDisplay> GetBoxes()
        {
            List<IGraphicalDisplay> retVal = new List<IGraphicalDisplay>();

            if (NameSpace != null)
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

            if (Dictionary != null)
            {
                foreach (NameSpace nameSpace in Dictionary.NameSpaces)
                {
                    retVal.Add(nameSpace);
                }
            }

            return retVal;
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
                    retVal.Add(new ModelArrow(variable, variable.Type, "type", variable));
                }

                Collection collection = item as Collection;
                if (collection != null && collection.Type != null)
                {
                    retVal.Add(new ModelArrow(collection, collection.Type, "of " + collection.getMaxSize(), collection));
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
                                retVal.Add(new ModelArrow(structure, element.Type, element.Name, element));
                            }
                        }
                    }

                    foreach (Structure inheritedStructure in structure.Interfaces)
                    {
                        if (boxes.Contains(inheritedStructure))
                        {
                            retVal.Add(new ModelArrow(structure, inheritedStructure, "implements", inheritedStructure));
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The last model for wich a displayed position has been computed
        /// </summary>
        private IGraphicalDisplay LastComputedPositionModel = null;

        /// <summary>
        ///     Reinitialises the automatic position handling
        /// </summary>
        protected override void InitPositionHandling()
        {
            LastComputedPositionModel = null;
            base.InitPositionHandling();
        }

        /// <summary>
        ///     Provides the next control position.
        ///     Align controls according to their type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override Point GetNextPosition(IGraphicalDisplay model)
        {
            if (LastComputedPositionModel != null && model.GetType() != LastComputedPositionModel.GetType())
            {
                int Y_OFFSET = model.Height + 10;
                CurrentPosition = new Point(1, CurrentPosition.Y + Y_OFFSET);
            }
            LastComputedPositionModel = model;

            return base.GetNextPosition(model);
        }
    }
}