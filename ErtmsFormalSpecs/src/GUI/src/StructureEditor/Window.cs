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
using System.Windows.Forms;
using BrightIdeasSoftware;
using DataDictionary;
using DataDictionary.Interpreter;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using GUI.Properties;
using GUI.StateDiagram;
using GUIUtils.StructureEditor;
using Utils;

namespace GUI.StructureEditor
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     The variable currently being displayed, if any
        /// </summary>
        private IVariable Variable { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            CustomizeTreeView.DisplayAllVariables = Settings.Default.DisplayAllVariablesInStructureEditor;

            // The text to get for each column
            structureTreeListView.GetColumn(0).AspectGetter = CustomizeTreeView.FieldColumnStringonizer;
            structureTreeListView.GetColumn(1).AspectGetter = CustomizeTreeView.ValueColumnStringonizer;
            structureTreeListView.GetColumn(2).AspectGetter = CustomizeTreeView.ActualValueColumnStringonizer;
            structureTreeListView.GetColumn(3).AspectGetter = CustomizeTreeView.DescriptionColumnStringonizer;
            structureTreeListView.FormatCell += CustomizeTreeView.FormatCell;

            // Tree structure
            structureTreeListView.CanExpandGetter = CustomizeTreeView.HasChildren;
            structureTreeListView.ChildrenGetter = CustomizeTreeView.GetChildren;

            // Contextual menu
            structureTreeListView.CellRightClick += CreateContextualMenu;

            // Edition
            structureTreeListView.CellEditStarting += CustomizeTreeView.HandleCellEditStarting;
            structureTreeListView.CellEditValidating += CustomizeTreeView.HandleCellEditValidating;
            structureTreeListView.CellEditFinishing += CustomizeTreeView.HandleCellEditFinishing;

            structureTreeListView.ItemDrag += structureTreeListView_ItemDrag;
        }

        private void structureTreeListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        /// <summary>
        ///     Sets the model for this tree view
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(IValue model)
        {
            DisplayedModel = model;

            List<IValue> objectModel = new List<IValue>();
            ListValue listValue = model as ListValue;
            if (listValue != null)
            {
                foreach (IValue value in listValue.Val)
                {
                    if (value != EfsSystem.Instance.EmptyValue)
                    {
                        objectModel.Add(value);
                    }
                }
            }
            else
            {
                objectModel.Add(model);
            }

            structureTreeListView.SetObjects(objectModel);
        }

        /// <summary>
        ///     Sets the variable as data source for this window
        /// </summary>
        /// <param name="variable"></param>
        public void SetVariable(IVariable variable)
        {
            DisplayedModel = variable;

            Variable = variable;
            Text = Variable.FullName;
            List<IVariable> objectModel = new List<IVariable> {variable};
            structureTreeListView.SetObjects(objectModel);
        }

        /// <summary>
        ///     Indicates that a change event should be displayed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns></returns>
        protected override bool ShouldDisplayChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            // There is no smart way to determine whether the change should be taken into account or not
            return true;
        }

        /// <summary>
        ///     Allows to refresh the view, when the value of a model changed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns>True if the view should be refreshed</returns>
        public override bool HandleValueChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            bool retVal = base.HandleValueChange(modelElement, changeKind);

            if (retVal)
            {
                if (Variable != null)
                {
                    Expression expression = new Parser().Expression(
                        EnclosingFinder<Dictionary>.find(Variable), Variable.FullName);
                    IVariable variable = expression.GetVariable(new InterpretationContext());
                    if (variable != Variable)
                    {
                        SetVariable(variable);
                    }
                    else
                    {
                        structureTreeListView.RefreshObject(Variable);
                        structureTreeListView.Refresh();
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates that the model element should be displayed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <returns></returns>
        protected override bool ShouldTrackSelectionChange(IModelElement modelElement)
        {
            // Once created, this view does not change displayed model
            return false;
        }

        /// <summary>
        /// Indicates that coloring should be taken into consideration
        /// </summary>
        /// <param name="modelElement"></param>
        /// <returns></returns>
        public override bool ShouldUpdateColoring(IModelElement modelElement)
        {
            // This view does not uses coloring
            return false;
        }

        /// <summary>
        ///     Shows the state machine which corresponds to the variable
        /// </summary>
        private class ToolStripShowStateMachine : CustomizeTreeView.BaseToolStripButton
        {
            /// <summary>
            ///     The variable that holds the list value
            /// </summary>
            private IVariable Variable { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="args"></param>
            /// <param name="variable"></param>
            public ToolStripShowStateMachine(CellRightClickEventArgs args, IVariable variable)
                : base(args, "Show state machine")
            {
                Variable = variable;
            }

            /// <summary>
            ///     Executes the action requested by this tool strip button
            /// </summary>
            protected override void OnClick(EventArgs e)
            {
                StateDiagramWindow window = new StateDiagramWindow();
                GuiUtils.MdiWindow.AddChildWindow(window);
                window.StatePanel.SetStateMachine(Variable);
                window.Text = Variable.Name + @" " + Resources.ToolStripShowStateMachine_OnClick_state_diagram;

                base.OnClick(e);
            }
        }

        public static void CreateContextualMenu(object obj, CellRightClickEventArgs args)
        {
            CustomizeTreeView.CreateContextualMenu(obj, args);

            IVariable enclosingVariable = args.Model as IVariable;

            if (enclosingVariable != null)
            {
                if (enclosingVariable.Type is StateMachine)
                {
                    args.MenuStrip.Items.Add(new ToolStripShowStateMachine(args, enclosingVariable));
                }
            }
        }
    }
}