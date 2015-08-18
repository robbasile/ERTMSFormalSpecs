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
using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;
using DataDictionary;
using DataDictionary.Interpreter;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using WeifenLuo.WinFormsUI.Docking;
using ModelElement = DataDictionary.ModelElement;
using Shortcut = DataDictionary.Shortcuts.Shortcut;
using Type = DataDictionary.Types.Type;

namespace GUI.TestRunnerView.Watch
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            DockAreas = DockAreas.DockBottom;

            watchDataGridView.AllowDrop = true;
            watchDataGridView.DragEnter += watchDataGridView_DragEnter;
            watchDataGridView.DragDrop += watchDataGridView_DragDrop;
            watchDataGridView.CellEndEdit += watchDataGridView_CellEndEdit;
            watchDataGridView.KeyUp += watchDataGridView_KeyUp;
            watchDataGridView.DoubleClick += watchDataGridView_DoubleClick;
            List<WatchedExpression> watches = new List<WatchedExpression> {new WatchedExpression(Instance, "")};
            watchDataGridView.DataSource = watches;
        }

        /// <summary>
        ///     The instance on which expressions should be evaluated
        /// </summary>
        private ModelElement Instance
        {
            get
            {
                Dictionary retVal = null;

                if (EFSSystem.INSTANCE.Dictionaries.Count > 0)
                {
                    retVal = EFSSystem.INSTANCE.Dictionaries[0];
                }

                return retVal;
            }
        }

        private class TextChangeHandler : EditorView.Window.HandleTextChange
        {
            /// <summary>
            ///     The expression supervised by this change handler
            /// </summary>
            private WatchedExpression Watch { get; set; }

            /// <summary>
            ///     The column that is edited
            /// </summary>
            private string ColumnName { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="instance"></param>
            /// <param name="watch"></param>
            /// <param name="columnName"></param>
            public TextChangeHandler(ModelElement instance, WatchedExpression watch, string columnName)
                : base(instance, "Watch")
            {
                Watch = watch;
                ColumnName = columnName;
            }

            /// <summary>
            ///     The way text is retrieved from the instance
            /// </summary>
            /// <returns></returns>
            public override string GetText()
            {
                string retVal;

                if (ColumnName == "Expression")
                {
                    retVal = Watch.Expression;
                }
                else
                {
                    retVal = Watch.Value;
                }

                return retVal;
            }

            /// <summary>
            ///     The way text is set back in the instance
            /// </summary>
            /// <returns></returns>
            public override void SetText(string text)
            {
                if (ColumnName == "Expression")
                {
                    Watch.Expression = text;
                }
                else
                {
                    Watch.Value = text;
                }
            }
        }

        /// <summary>
        ///     Indicates that a double click event is being handled
        /// </summary>
        private bool HandlingDoubleClick { get; set; }

        /// <summary>
        ///     Handles a double click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watchDataGridView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                HandlingDoubleClick = true;

                List<WatchedExpression> watches = (List<WatchedExpression>) watchDataGridView.DataSource;

                // Open a editor to edit the cell contents
                WatchedExpression selected = SelectedWatch;
                if (selected != null)
                {
                    DataGridViewCell selectedCell = watchDataGridView.SelectedCells[0];
                    if (selectedCell.ColumnIndex == 0)
                    {
                        EditorView.Window form = new EditorView.Window {AutoComplete = true};
                        TextChangeHandler handler = new TextChangeHandler(Instance, selected,
                            selectedCell.OwningColumn.Name);
                        form.setChangeHandler(handler);
                        form.ShowDialog();

                        watchDataGridView.DataSource = null;
                        watchDataGridView.DataSource = watches;
                        EnsureEmptyRoom();
                        Refresh();
                    }
                    else if (selectedCell.ColumnIndex == 1)
                    {
                        ExplainBox explainTextBox = new ExplainBox();
                        explainTextBox.SetExplanation(selected.ExpressionTree.Explain());
                        GuiUtils.MdiWindow.AddChildWindow(explainTextBox);
                    }
                }
            }
            finally
            {
                HandlingDoubleClick = false;
            }
        }

        /// <summary>
        ///     Provides the watch expression selected by the grid view
        /// </summary>
        private WatchedExpression SelectedWatch
        {
            get
            {
                WatchedExpression retVal = null;

                if (watchDataGridView.SelectedCells.Count == 1)
                {
                    retVal =
                        ((List<WatchedExpression>) watchDataGridView.DataSource)[
                            watchDataGridView.SelectedCells[0].OwningRow.Index];
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Handles the key up event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watchDataGridView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                // Delete current watch
                WatchedExpression selected = SelectedWatch;
                if (selected != null)
                {
                    List<WatchedExpression> watches = (List<WatchedExpression>) watchDataGridView.DataSource;
                    watches.Remove(selected);

                    watchDataGridView.DataSource = null;
                    watchDataGridView.DataSource = watches;
                    EnsureEmptyRoom();
                    Refresh();
                }
            }
        }

        /// <summary>
        ///     Ensures that there is an empty room available in the data
        /// </summary>
        private void EnsureEmptyRoom()
        {
            List<WatchedExpression> watches = (List<WatchedExpression>) watchDataGridView.DataSource;

            bool emptyFound = false;
            foreach (WatchedExpression watch in watches)
            {
                if (string.IsNullOrEmpty(watch.Expression))
                {
                    emptyFound = true;
                    break;
                }
            }

            if (!emptyFound)
            {
                watches.Add(new WatchedExpression(Instance, ""));
                watchDataGridView.DataSource = null;
                watchDataGridView.DataSource = watches;
            }
        }

        /// <summary>
        ///     Handles the end of edition event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watchDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (!HandlingDoubleClick)
            {
                EnsureEmptyRoom();
                Refresh();
            }
        }

        /// <summary>
        ///     Changes the drag & drop pointer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watchDataGridView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        ///     Handles a drop event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watchDataGridView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("WindowsForms10PersistentObject", false))
            {
                object data = e.Data.GetData("WindowsForms10PersistentObject");
                BaseTreeNode sourceNode = data as BaseTreeNode;
                if (sourceNode != null)
                {
                    Variable variable = sourceNode.Model as Variable;
                    if (variable == null)
                    {
                        Shortcut shortCut = sourceNode.Model as Shortcut;
                        if (shortCut != null)
                        {
                            variable = shortCut.GetReference() as Variable;
                        }
                    }

                    if (variable != null)
                    {
                        AddVariable(variable);
                    }
                }

                OLVListItem item = data as OLVListItem;
                if (item != null)
                {
                    Variable variable = item.RowObject as Variable;
                    if (variable != null)
                    {
                        AddVariable(variable);
                    }
                }
            }
        }

        private void AddVariable(Variable variable)
        {
            List<WatchedExpression> watches = (List<WatchedExpression>) watchDataGridView.DataSource;
            watches.Insert(watches.Count - 1, new WatchedExpression(Instance, variable.FullName));
            watchDataGridView.DataSource = null;
            watchDataGridView.DataSource = watches;
            Refresh();
        }

        /// <summary>
        ///     Allows to refresh the view, when the selected model changed
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if refresh should be performed</returns>
        public override bool HandleSelectionChange(Context.SelectionContext context)
        {
            bool retVal = base.HandleSelectionChange(context);

            if (retVal)
            {
                Refresh();
            }

            return retVal;
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
                Refresh();
            }

            return retVal;
        }

        private class WatchedExpression
        {
            /// <summary>
            ///     The instance on which expressions should be evaluated
            /// </summary>
            private ModelElement Instance { get; set; }

            /// <summary>
            ///     The identification of the element
            /// </summary>
            public string Expression { get; set; }

            /// <summary>
            ///     Provides the expression which corresponds to the Expression text.
            ///     Returns null if the expression could not be parsed
            /// </summary>
            [Browsable(false)]
            public Expression ExpressionTree
            {
                get
                {
                    Expression retVal = null;

                    if (!string.IsNullOrEmpty(Expression))
                    {
                        ModelElement.DontRaiseError(() =>
                        {
                            try
                            {
                                retVal = Instance.EFSSystem.Parser.Expression(Instance, Expression);
                            }
                            catch (Exception)
                            {
                            }
                        });
                    }

                    return retVal;
                }
            }

            /// <summary>
            ///     The value of the corresponding expression
            /// </summary>
            public string Value
            {
                get
                {
                    string retVal = "";

                    if (!string.IsNullOrEmpty(Expression))
                    {
                        retVal = "<cannot evaluate expression>";
                        Expression expression = ExpressionTree;
                        if (expression != null)
                        {
                            try
                            {
                                IValue value = expression.GetExpressionValue(new InterpretationContext(), null);
                                if (value != null)
                                {
                                    retVal = value.LiteralName;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }

                    return retVal;
                }
                set
                {
                    if (!string.IsNullOrEmpty(Expression))
                    {
                        Expression expression = ExpressionTree;
                        if (expression != null)
                        {
                            Variable variable = expression.Ref as Variable;
                            if (variable != null)
                            {
                                Type type = variable.Type;
                                if (type != null)
                                {
                                    IValue val = type.getValue(value);
                                    if (val != null)
                                    {
                                        variable.Value = val;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="instance"></param>
            /// <param name="expression"></param>
            public WatchedExpression(ModelElement instance, string expression)
            {
                Instance = instance;
                Expression = expression;
            }
        }
    }
}