// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpecs software and documentation
// --
// --  ERTMSFormalSpecs is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpecs is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;
using DataDictionary;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Collection = DataDictionary.Types.Collection;
using Enum = DataDictionary.Types.Enum;
using EnumValue = DataDictionary.Constants.EnumValue;
using Range = DataDictionary.Types.Range;
using Structure = DataDictionary.Types.Structure;
using StructureElement = DataDictionary.Types.StructureElement;
using Variable = DataDictionary.Variables.Variable;

namespace GUIUtils.StructureEditor
{
    /// <summary>
    ///     Customize the tree view according to the model element to display
    /// </summary>
    public static class CustomizeTreeView
    {
        /// <summary>
        ///     If set to true, indicates that even empty variables should be displayed
        /// </summary>
        public static bool DisplayAllVariables { get; set; }

        /// <summary>
        ///     The tooltip allowing to display the full name of elements in comboboxes
        /// </summary>
        public static ToolTip ToolTip = new ToolTip();

        /// <summary>
        ///     Dereferences the variables
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static IValue DerefVariable(object obj)
        {
            IValue retVal;

            IVariable variable = obj as IVariable;
            if (variable != null)
            {
                retVal = variable.Value;
            }
            else
            {
                retVal = obj as IValue;
            }

            return retVal;
        }

        public delegate void HandlePostClickAction();

        /// <summary>
        /// The action to be executed after the click handling
        /// </summary>
        public static HandlePostClickAction PostClickAction { get; set; }

        #region Stringonizer

        /// <summary>
        ///     Provides the field column string value for the object provided
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string FieldColumnStringonizer(object obj)
        {
            string retVal = "";

            StructureValue structureValue = obj as StructureValue;
            if (structureValue == null)
            {
                if (obj is Field)
                {
                    structureValue = (obj as Field).Value as StructureValue;
                }
            }
            if (structureValue != null)
            {
                ListValue enclosingListValue = structureValue.Enclosing as ListValue;
                if (enclosingListValue != null)
                {
                    int index = enclosingListValue.Val.IndexOf(structureValue) + 1;
                    IVariable enclosingListVariable = enclosingListValue.Enclosing as Variable;
                    if (enclosingListVariable == null)
                    {
                        enclosingListVariable = enclosingListValue.Enclosing as Field;
                    }
                    if (enclosingListVariable != null)
                    {
                        retVal = enclosingListVariable.Name + "[" + index + "]";
                    }
                }
                else
                {
                    retVal = structureValue.Type.Name;

                    IVariable name = structureValue.GetVariable("Name");
                    if (name == null)
                    {
                        name = structureValue.GetVariable("Id");
                    }

                    if (name != null)
                    {
                        retVal = retVal + " " + name.Value.Name;
                    }
                }
            }

            IVariable variable = obj as IVariable;
            if (variable != null)
            {
                retVal = variable.Name;
            }

            return retVal;
        }

        /// <summary>
        ///     The default indicator
        /// </summary>
        private const string DefaultConst = "<default>";

        /// <summary>
        ///     Provides the value column string value for the object provided
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ValueColumnStringonizer(object obj)
        {
            return ValueStringonizer(obj, false);
        }

        /// <summary>
        ///     Provides the value column string value for the object provided
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ActualValueColumnStringonizer(object obj)
        {
            return ValueStringonizer(obj, true);
        }

        /// <summary>
        ///     Converts a value into a string
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="displayActualDefaultValue">
        ///     Indicates that the actual default value should be displayed instead of "
        ///     <default>"
        /// </param>
        /// <returns></returns>
        private static string ValueStringonizer(object obj, bool displayActualDefaultValue)
        {
            string retVal;

            IVariable variable = obj as IVariable;
            IValue value = DerefVariable(obj);

            StructureValue structureValue = value as StructureValue;
            ListValue listValue = value as ListValue;
            if (structureValue != null)
            {
                retVal = "";
            }
            else if (listValue != null)
            {
                retVal = "";
            }
            else
            {
                if (displayActualDefaultValue)
                {
                    // Dereference enumeration constants to get the actual value
                    EnumValue enumValue = value as EnumValue;
                    while (enumValue != null)
                    {
                        value = enumValue.Value;
                        if (value != enumValue.Value)
                        {
                            enumValue = value as EnumValue;
                        }
                        else
                        {
                            enumValue = null;
                        }
                    }

                    retVal = value.Name;
                }
                else
                {
                    if (IsDefaultValue(variable, value))
                    {
                        retVal = DefaultConst;
                    }
                    else
                    {
                        retVal = value.Name;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates whether the value provided is the default value for that variable
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsDefaultValue(IVariable variable, IValue value)
        {
            bool retVal = value is DefaultValue;

            return retVal;
        }

        /// <summary>
        ///     Provides the string value for the description columb according to the object provided
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string DescriptionColumnStringonizer(object obj)
        {
            string retVal = "";

            IVariable variable = obj as IVariable;
            if (variable != null)
            {
                Variable modelVariable = variable as Variable;
                if (modelVariable != null)
                {
                    if (string.IsNullOrEmpty(modelVariable.Comment))
                    {
                        retVal = modelVariable.Type.Comment;
                    }
                    else
                    {
                        retVal = modelVariable.Comment;
                    }
                }
                else
                {
                    retVal = variable.Type.Comment;
                }
            }
            else
            {
                Value value = obj as Value;
                if (value != null)
                {
                    retVal = value.Type.Comment;
                }
            }

            return retVal;
        }

        public static void FormatCell(object sender, FormatCellEventArgs e)
        {
            if (e.Column.Index == 1)
            {
                IValue value = DerefVariable(e.Model);
                if (value != null)
                {
                    if (value is DefaultValue)
                    {
                        e.SubItem.ForeColor = Color.Blue;
                    }
                }
            }
        }

        #endregion

        #region Tree Structure

        public static bool HasChildren(object obj)
        {
            bool retVal = false;

            IValue value = DerefVariable(obj);

            StructureValue structureValue = value as StructureValue;
            if (structureValue != null)
            {
                foreach (IVariable subVariable in structureValue.Val.Values)
                {
                    if (subVariable.Type is Structure)
                    {
                        if (!(subVariable.Value is DefaultValue))
                        {
                            if (subVariable.Value != EfsSystem.Instance.EmptyValue || DisplayAllVariables)
                            {
                                retVal = true;
                                break;
                            }
                        }
                        else
                        {
                            retVal = DisplayAllVariables;
                        }
                    }
                    else
                    {
                        retVal = true;
                        break;
                    }
                }
            }

            ListValue listValue = value as ListValue;
            if (listValue != null)
            {
                retVal = listValue.Val.Count > 0;
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the children of a specific object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable GetChildren(object obj)
        {
            IEnumerable retVal = null;

            IValue value = DerefVariable(obj);

            StructureValue structureValue = value as StructureValue;
            if (structureValue != null)
            {
                ArrayList list = new ArrayList();
                foreach (IVariable subVariable in structureValue.Val.Values)
                {
                    if (subVariable.Value is DefaultValue)
                    {
                        if (subVariable.Type is Structure && ! DisplayAllVariables)
                        {
                            // Don't add it, it shall be handled by the contextual menus
                        }
                        else
                        {
                            list.Add(subVariable);
                        }
                    }
                    else
                    {
                        if (subVariable.Value != EfsSystem.Instance.EmptyValue || DisplayAllVariables)
                        {
                            list.Add(subVariable);
                        }
                    }
                }
                retVal = list;
            }

            ListValue listValue = value as ListValue;
            if (listValue != null)
            {
                ArrayList list = new ArrayList();
                foreach (Value element in listValue.Val)
                {
                    if (element != EfsSystem.Instance.EmptyValue)
                    {
                        list.Add(element);
                    }
                }
                retVal = list;
            }

            return retVal;
        }

        #endregion

        #region Contextual menu

        /// <summary>
        ///     The base toolstrip button. Handles synchronization with the object list view
        /// </summary>
        public class BaseToolStripButton : ToolStripButton
        {
            /// <summary>
            ///     The arguments used to launch the tool strip
            /// </summary>
            protected CellRightClickEventArgs Args { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="args"></param>
            /// <param name="text"></param>
            protected BaseToolStripButton(CellRightClickEventArgs args, string text)
                : base(text)
            {
                Args = args;
                Width = Text.Length*8;
            }

            /// <summary>
            ///     Synchronises the list view
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                try
                {
                    Args.ListView.RefreshObject(Args.Model);
                }
                catch (Exception)
                {
                }

                base.OnClick(e);

                if (PostClickAction != null)
                {
                    PostClickAction();
                }
            }

            /// <summary>
            ///     Provides the parent model
            /// </summary>
            public object ParentModel
            {
                get
                {
                    object retVal = null;

                    TreeListView treeListView = (TreeListView) Args.ListView;
                    retVal = treeListView.GetParent(Args.Model);

                    return retVal;
                }
            }
        }

        /// <summary>
        ///     Adds a value in a list
        /// </summary>
        private class ToolStripAddValueInList : BaseToolStripButton
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
            public ToolStripAddValueInList(CellRightClickEventArgs args, IVariable variable)
                : base(args, "Add entry")
            {
                Variable = variable;
            }

            /// <summary>
            ///     Executes the action requested by this tool strip button
            /// </summary>
            protected override void OnClick(EventArgs e)
            {
                Collection collectionType = (Collection) Variable.Type;
                Structure structureType = (Structure) collectionType.Type;
                StructureValue element = new StructureValue(structureType, false);

                if (structureType.Elements.Count == 1)
                {
                    StructureElement subElement = (StructureElement) structureType.Elements[0];
                    Structure subElementStructureType = subElement.Type as Structure;
                    if (subElementStructureType != null)
                    {
                        element.CreateField(subElement, structureType);
                    }
                }

                Variable.Value = Variable.Value.RightSide(Variable, false, true) as ListValue;
                ListValue value = Variable.Value as ListValue;
                if (value != null)
                {
                    for (int i = 0; i < value.Val.Count; i++)
                    {
                        if (value.Val[i] == EfsSystem.Instance.EmptyValue)
                        {
                            value.Val[i] = element;
                            element.Enclosing = Variable.Value;
                            break;
                        }
                    }
                }

                base.OnClick(e);
            }
        }

        /// <summary>
        ///     Removes a value from a list
        /// </summary>
        private class ToolStripRemoveListEntry : BaseToolStripButton
        {
            /// <summary>
            ///     The value that should be updated
            /// </summary>
            private ListValue Value { get; set; }

            /// <summary>
            ///     The entry to remove from the list value
            /// </summary>
            private Value Entry { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="args"></param>
            /// <param name="value"></param>
            /// <param name="entry"></param>
            public ToolStripRemoveListEntry(CellRightClickEventArgs args, ListValue value, Value entry)
                : base(args, "Remove")
            {
                Value = value;
                Entry = entry;
            }

            /// <summary>
            ///     Executes the action requested by this tool strip button
            /// </summary>
            protected override void OnClick(EventArgs e)
            {
                for (int i = 0; i < Value.Val.Count; i++)
                {
                    if (Value.Val[i] == Entry)
                    {
                        Value.Val[i] = EfsSystem.Instance.EmptyValue;
                    }
                }
                Args.ListView.RefreshObject(ParentModel);

                // Not needed because the element has been removed
                // base.OnClick(e);
            }
        }

        /// <summary>
        ///     Adds an element in a structure value
        /// </summary>
        private class ToolStripAddStructureMember : BaseToolStripButton
        {
            /// <summary>
            ///     The value that should be updated
            /// </summary>
            private StructureValue Value { get; set; }

            /// <summary>
            ///     The element on which the action should be performed
            /// </summary>
            private StructureElement Element { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="args"></param>
            /// <param name="value"></param>
            /// <param name="element"></param>
            public ToolStripAddStructureMember(CellRightClickEventArgs args, StructureValue value,
                StructureElement element)
                : base(args, "Add " + element.Name)
            {
                Value = value;
                Element = element;
            }

            /// <summary>
            ///     Executes the action requested by this tool strip button
            /// </summary>
            protected override void OnClick(EventArgs e)
            {
                Structure elementStructureType = (Structure) Element.Type;
                StructureValue subValue = new StructureValue(elementStructureType, false);
                Field field = Value.CreateField(Element, elementStructureType);
                field.Value = subValue;
                base.OnClick(e);
            }
        }

        /// <summary>
        ///     Removes an element from a structure value
        /// </summary>
        private class ToolStripRemoveStructureMember : BaseToolStripButton
        {
            /// <summary>
            ///     The variable to remove
            /// </summary>
            private IVariable Variable { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="args"></param>
            /// <param name="variable"></param>
            public ToolStripRemoveStructureMember(CellRightClickEventArgs args, IVariable variable)
                : base(args, "Remove")
            {
                Variable = variable;
            }

            /// <summary>
            ///     Executes the action requested by this tool strip button
            /// </summary>
            protected override void OnClick(EventArgs e)
            {
                Variable.Value = EfsSystem.Instance.EmptyValue;

                Args.ListView.RefreshObject(ParentModel);
            }
        }

        public static void CreateContextualMenu(object obj, CellRightClickEventArgs args)
        {
            ContextMenuStrip menuStrip = new ContextMenuStrip();
            List<BaseToolStripButton> items = new List<BaseToolStripButton>();

            IVariable enclosingVariable = args.Model as IVariable;
            IValue value = DerefVariable(args.Model);

            StructureValue structureValue = value as StructureValue;
            if (structureValue != null)
            {
                Structure structureType = (Structure) structureValue.Type;
                foreach (StructureElement element in structureType.Elements)
                {
                    if (element.Type is Structure)
                    {
                        IVariable subVariable = null;
                        INamable tmp;
                        if (structureValue.Val.TryGetValue(element.Name, out tmp))
                        {
                            subVariable = tmp as IVariable;
                        }

                        if (subVariable == null || subVariable.Value == EfsSystem.Instance.EmptyValue ||
                            subVariable.Value is DefaultValue)
                        {
                            items.Add(new ToolStripAddStructureMember(args, structureValue, element));
                        }
                    }
                }

                if (enclosingVariable != null)
                {
                    StructureValue enclosingStructureValue = enclosingVariable.Enclosing as StructureValue;
                    if (enclosingStructureValue != null)
                    {
                        items.Add(new ToolStripRemoveStructureMember(args, enclosingVariable));
                    }
                }

                TreeListView treeListView = (TreeListView) obj;
                object parent = treeListView.GetParent(args.Model);
                ListValue enclosingListValue = DerefVariable(parent) as ListValue;
                if (enclosingListValue != null)
                {
                    items.Add(new ToolStripRemoveListEntry(args, enclosingListValue, structureValue));
                }
            }

            ListValue listValue = value as ListValue;
            if (listValue != null)
            {
                if (enclosingVariable != null)
                {
                    Collection collection = (Collection) enclosingVariable.Type;
                    if (listValue.Val.Count < collection.getMaxSize())
                    {
                        items.Add(new ToolStripAddValueInList(args, enclosingVariable));
                    }
                }
            }

            items.Sort(delegate(BaseToolStripButton b1, BaseToolStripButton b2) { return b1.Text.CompareTo(b2.Text); });
            foreach (BaseToolStripButton menuItem in items)
            {
                menuStrip.Items.Add(menuItem);
            }

            args.MenuStrip = menuStrip;
        }

        #endregion

        #region Edition

        public static void HandleCellEditStarting(object sender, CellEditEventArgs e)
        {
            IVariable variable = e.RowObject as IVariable;
            if (variable != null)
            {
                Enum enumType = variable.Type as Enum;
                if (enumType != null)
                {
                    ComboBox control = new ComboBox
                    {
                        Bounds = e.CellBounds,
                        Font = ((ObjectListView) sender).Font,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Text = (string) e.Value,
                        DrawMode = DrawMode.OwnerDrawFixed
                    };
                    foreach (EnumValue enumValue in enumType.Values)
                    {
                        control.Items.Add(enumValue.Name);
                    }
                    control.Items.Add(DefaultConst);
                    control.Text = variable.Value.Name;
                    control.DrawItem += cbb_DrawItem;
                    e.Control = control;
                }

                Range rangeType = variable.Type as Range;
                if (rangeType != null)
                {
                    ComboBox control = new ComboBox
                    {
                        Bounds = e.CellBounds,
                        Font = ((ObjectListView) sender).Font,
                        DropDownStyle = ComboBoxStyle.DropDown,
                        Text = (string) e.Value,
                        DrawMode = DrawMode.OwnerDrawFixed
                    };
                    foreach (EnumValue enumValue in rangeType.SpecialValues)
                    {
                        control.Items.Add(enumValue.Name);
                    }
                    control.Items.Add(DefaultConst);
                    control.Text = variable.Value.Name;
                    control.DrawItem += cbb_DrawItem;
                    e.Control = control;
                }

                Structure structure = variable.Type as Structure;
                if (structure != null)
                {
                    e.Cancel = true;
                }

                Collection collection = variable.Type as Collection;
                if (collection != null)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        public static void HandleCellEditValidating(object sender, CellEditEventArgs e)
        {
            string text = e.Control.Text;

            IVariable variable = e.RowObject as IVariable;
            if (variable != null)
            {
                if (DefaultConst != text)
                {
                    if (variable.Type.getValue(text) == null)
                    {
                        e.Cancel = true;
                    }
                }
            }
            ToolTip.RemoveAll();
        }

        public static void HandleCellEditFinishing(object sender, CellEditEventArgs e)
        {
            if (!e.Cancel)
            {
                IVariable variable = e.RowObject as IVariable;

                string text = e.Control.Text;
                if (DefaultConst == text)
                {
                    variable.Value = variable.Type.DefaultValue;
                }
                else
                {
                    if (variable != null)
                    {
                        variable.Value = variable.Type.getValue(text);
                    }
                }
            }
            ToolTip.RemoveAll();
        }

        #endregion

        #region ToolTips

        /// <summary>
        ///     Used to redefine the drawing method of comboboxes and add a tooltip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void cbb_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                Point p = new Point(comboBox.Location.X + 100, comboBox.Location.Y + comboBox.Height + (20 + e.Index*10));

                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    ToolTip.Show(comboBox.Items[e.Index].ToString(), comboBox.FindForm(), p);
                }
                e.DrawBackground();
                e.Graphics.DrawString(comboBox.Items[e.Index].ToString(), e.Font, Brushes.Black,
                    new Point(e.Bounds.X, e.Bounds.Y));
            }
        }

        #endregion
    }
}