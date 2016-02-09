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
using System.Globalization;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Interpreter.ListOperators;
using DataDictionary.Tests;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Action = DataDictionary.Rules.Action;
using ModelElement = DataDictionary.ModelElement;
using Util = Utils.Util;

namespace GUIUtils.Editor
{
    public partial class BaseEditorTextBox : UserControl
    {
        /// <summary>
        ///     Indicates that only types should be considered in the search
        /// </summary>
        public bool ConsiderOnlyTypes { get; set; }

        /// <summary>
        ///     Indicates whether there is a pending selection in the combo box
        /// </summary>
        private bool PendingSelection { get; set; }

        /// <summary>
        ///     Provides the instance on which this editor is based
        /// </summary>
        public virtual object Instance { get; set; }

        /// <summary>
        ///     The instance viewed as a model element
        /// </summary>
        public IModelElement Model
        {
            get { return Instance as IModelElement; }
        }

        /// <summary>
        ///     Indicates that autocompletion is active for the text box
        /// </summary>
        public bool AutoComplete { get; set; }

        /// <summary>
        ///     Indicates that a mouse mouve event hides the explanation
        /// </summary>
        private bool ConsiderMouseMoveToCloseExplanation { get; set; }

        /// <summary>
        ///     The location of the mouse
        /// </summary>
        private Point MouseLocation { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public BaseEditorTextBox()
        {
            InitializeComponent();

            AutoComplete = true;
            EditionTextBox.KeyUp += Editor_KeyUp;
            EditionTextBox.KeyPress += Editor_KeyPress;
            EditionTextBox.MouseUp += EditionTextBox_MouseClick;
            EditionTextBox.ShortcutsEnabled = true;
            EditionTextBox.MouseMove += EditionTextBox_MouseMove;

            SelectionComboBox.LostFocus += SelectionComboBox_LostFocus;
            SelectionComboBox.KeyUp += SelectionComboBox_KeyUp;
            SelectionComboBox.SelectedValueChanged += SelectionComboBox_SelectedValueChanged;
            SelectionComboBox.DropDownStyleChanged += SelectionComboBox_DropDownStyleChanged;
            SelectionComboBox.LocationChanged += SelectionComboBox_LocationChanged;
        }

        #region Explanation using the explain rich text box
        /// <summary>
        ///     Takes case of the mouse move to hide the explain text box, if needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditionTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            MouseLocation = e.Location;
            if (ConsiderMouseMoveToCloseExplanation && explainRichTextBox.Visible)
            {
                Rectangle rectangle = explainRichTextBox.DisplayRectangle;
                rectangle.Location = explainRichTextBox.Location;
                rectangle.Inflate(20, 20);
                if (!rectangle.Contains(e.Location))
                {
                    explainRichTextBox.Hide();
                    ConsiderMouseMoveToCloseExplanation = false;
                }
            }
        }

        /// <summary>
        ///     Displays the help associated to a location in the text box
        /// </summary>
        /// <param name="location"></param>
        private void DisplayHelp(Point location)
        {
            INamable instance = GetInstance(location);

            if (instance != null)
            {
                location.Offset(10, 10);
                const bool considerMouseMove = true;
                ExplainAndShow(instance, location, considerMouseMove);
            }
            else
            {
                contextMenuStrip1.Show(PointToScreen(MouseLocation));
            }
        }

        private void EditionTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                MouseLocation = e.Location;
                DisplayHelp(e.Location);
            }
        }

        private void SelectionComboBox_LocationChanged(object sender, EventArgs e)
        {
            ExplainAndShowReference((ObjectReference) SelectionComboBox.SelectedItem, Point.Empty);
        }

        private void SelectionComboBox_DropDownStyleChanged(object sender, EventArgs e)
        {
            ExplainAndShowReference((ObjectReference) SelectionComboBox.SelectedItem, Point.Empty);
        }

        private void SelectionComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ExplainAndShowReference((ObjectReference) SelectionComboBox.SelectedItem, Point.Empty);
        }

        /// <summary>
        ///     Explains a reference and shows the associated textbox
        /// </summary>
        /// <param name="reference">The object reference to explain</param>
        /// <param name="location">
        ///     The location where the explain box should be displayed. If empty is displayed, the location is
        ///     computed based on the combo box location
        /// </param>
        /// <param name="sensibleToMouseMove">Indicates that the explain box should be closed when the mouse moves</param>
        private void ExplainAndShowReference(ObjectReference reference, Point location, bool sensibleToMouseMove = false)
        {
            if (reference != null)
            {
                ExplainAndShow(reference.Model, location, sensibleToMouseMove);
            }
        }

        /// <summary>
        ///     Explains a list of namables and shows the associated textbox
        /// </summary>
        /// <param name="namable">The namable to explain</param>
        /// <param name="location">
        ///     The location where the explain box should be displayed. If empty is displayed, the location is
        ///     computed based on the combo box location
        /// </param>
        /// <param name="sensibleToMouseMove">Indicates that the explain box should be closed when the mouse moves</param>
        private void ExplainAndShow(INamable namable, Point location, bool sensibleToMouseMove)
        {
            explainRichTextBox.Text = "";
            if (namable != null)
            {
                TextualExplanation explanation = new TextualExplanation();
                ITextualExplain textualExplain = namable as ITextualExplain;
                if (textualExplain != null)
                {
                    textualExplain.GetExplain(explanation, false);
                }

                explainRichTextBox.Text = explanation.Text;
                explainRichTextBox.ProcessAllLines();

                if (location == Point.Empty)
                {
                    if (SelectionComboBox.DroppedDown)
                    {
                        explainRichTextBox.Location = new Point(
                            SelectionComboBox.Location.X + SelectionComboBox.Size.Width,
                            SelectionComboBox.Location.Y + SelectionComboBox.Size.Height
                            );
                    }
                    else
                    {
                        explainRichTextBox.Location = new Point(
                            SelectionComboBox.Location.X,
                            SelectionComboBox.Location.Y + SelectionComboBox.Size.Height
                            );
                    }
                }
                else
                {
                    explainRichTextBox.Location = new Point(
                        Math.Min(location.X, EditionTextBox.Size.Width - explainRichTextBox.Size.Width),
                        Math.Min(location.Y, EditionTextBox.Size.Height - explainRichTextBox.Size.Height));
                }

                ConsiderMouseMoveToCloseExplanation = sensibleToMouseMove;
                explainRichTextBox.Show();
                EditionTextBox.SendToBack();
            }
        }
        #endregion

        #region Reference to the textbox itself

        /// <summary>
        ///     The text box
        /// </summary>
        public SyntaxRichTextBox TextBox
        {
            get { return EditionTextBox; }
        }

        public void Copy()
        {
            EditionTextBox.Copy();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }

        public void Cut()
        {
            EditionTextBox.Cut();
            EditionTextBox.ProcessAllLines();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cut();
        }

        public void Paste()
        {
            EditionTextBox.Paste();
            EditionTextBox.ProcessAllLines();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste();
        }

        public void Undo()
        {
            EditionTextBox.Undo();
            EditionTextBox.ProcessAllLines();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        public void Redo()
        {
            EditionTextBox.Redo();
            EditionTextBox.ProcessAllLines();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Redo();
        }

        public bool ReadOnly
        {
            get { return EditionTextBox.ReadOnly; }
            set { EditionTextBox.ReadOnly = value; }
        }

        public override string Text
        {
            get { return EditionTextBox.Text; }
            set
            {
                if (value != EditionTextBox.Text)
                {
                    EditionTextBox.Text = value.Trim();
                    EditionTextBox.ProcessAllLines();
                }
            }
        }

        private void indentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditionTextBox.Indent();
        }

        #endregion

        /// <summary>
        ///     Provides the current prefix, according to the selection position
        /// </summary>
        /// <param name="end">The location where the prefix should end</param>
        /// <returns></returns>
        private string CurrentPrefix(int end)
        {
            string retVal = "";

            while (end >= EditionTextBox.Text.Length)
            {
                end = end - 1;
            }

            while (end >= 0 && Char.IsSeparator(EditionTextBox.Text[end]))
            {
                end = end - 1;
            }

            int start = end;
            while (start >= 0 && Char.IsLetterOrDigit(EditionTextBox.Text[start]))
            {
                start = start - 1;
            }
            start = start + 1;

            if (start <= end)
            {
                retVal = EditionTextBox.Text.Substring(start, end - start + 1);
            }
            return retVal;
        }

        /// <summary>
        ///     Code templates
        /// </summary>
        private static readonly string[] Templates =
        {
            ForAllExpression.Operator + " X IN <collection> | <condition> ",
            ThereIsExpression.Operator + " X IN <collection> | <condition> ",
            FirstExpression.Operator + " X IN <collection> | <condition>",
            LastExpression.Operator + " X IN <collection> | <condition>",
            CountExpression.Operator + " X IN <collection> | <condition>",
            MapExpression.Operator + " <collection> | <condition> USING X IN <map_expression>",
            FilterExpression.Operator + " <collection> | <condition> USING X",
            SumExpression.Operator + " <collection> | <condition> USING X IN <map_expression>",
            ReduceExpression.Operator +
            " <collection> | <condition> USING X IN <reduce_expression> INITIAL_VALUE <expression>",
            "LET <variable> <- <expression> IN <expression>",
            "STABILIZE <expression> INITIAL_VALUE <expression> STOP_CONDITION <condition>",
            "APPLY <statement> ON <collection> | <condition>",
            "INSERT <expression> IN <collection> WHEN FULL REPLACE <condition>",
            "REMOVE [FIRST|LAST|ALL] <condition> IN <collection>",
            "REPLACE <condition> IN <collection> BY <expression>",
            "%D_LRBG",
            "%Level",
            "%Mode",
            "%NID_LRBG",
            "%Q_DIRLRBG",
            "%Q_DIRTRAIN",
            "%Q_DLRBG",
            "%RBC_ID",
            "%RBCPhone",
            "%Step_Distance",
            "%Step_LevelIN",
            "%Step_LevelOUT",
            "%Step_ModeIN",
            "%Step_ModeOUT",
            "%Step_Messages_0",
            "%Step_Messages_1",
            "%Step_Messages_2",
            "%Step_Messages_3",
            "%Step_Messages_4",
            "%Step_Messages_5",
            "%Step_Messages_6",
            "%Step_Messages_7"
        };

        /// <summary>
        /// Parses the current statement or expression and returns the interpreter tree node
        /// </summary>
        /// <returns></returns>
        private InterpreterTreeNode Parse()
        {
            InterpreterTreeNode retVal = null;

            string text = EditionTextBox.Text;
            if (!String.IsNullOrEmpty(text))
            {
                Parser parser = new Parser();
                retVal = parser.Statement(Instance as ModelElement, EditionTextBox.Text, true, true);
                if (retVal == null)
                {
                    retVal = parser.Expression(Instance as ModelElement, text, AllMatches.INSTANCE, true, null, true, true);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the instance related to a location in the textbox
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected INamable GetInstance(Point location)
        {
            // ReSharper disable once RedundantAssignment
            INamable retVal = null;

            int index = EditionTextBox.GetCharIndexFromPosition(location);
            retVal = GetInstance(index);

            return retVal;
        }

        /// <summary>
        ///     Provides the instance related to a character index in the textbox
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected INamable GetInstance(int index)
        {
            INamable retVal = null;

            if (Model != null)
            {
                InterpreterTreeNode node = Parse();

                if (node != null)
                {
                    ContextGrabber grabber = new ContextGrabber();
                    retVal = grabber.GetContext(index, node);
                }
            }

            return retVal;
        }


        /// <summary>
        ///     Provides the enclosing sub declarator
        /// </summary>
        /// <param name="modelElement"></param>
        /// <returns></returns>
        private static ISubDeclarator EnclosingSubDeclarator(IModelElement modelElement)
        {
            ISubDeclarator retVal = null;

            while (modelElement != null && retVal == null)
            {
                retVal = modelElement as ISubDeclarator;
                modelElement = modelElement.Enclosing as IModelElement;
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the list of model elements which correspond to the prefix given
        /// </summary>
        /// <param name="index">The location of the cursor in the text box</param>
        /// <param name="prefix">The prefix used to reduce the choices</param>
        /// <returns></returns>
        public List<ObjectReference> AllChoices(int index, string prefix)
        {
            List<ObjectReference> retVal = new List<ObjectReference>();

            double val;
            bool isANumber = double.TryParse(CurrentPrefix(index - 1).Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out val);

            if (!isANumber)
            {
                ISubDeclarator subDeclarator = GetInstance(index) as ISubDeclarator;
                if (subDeclarator == null)
                {
                    if (Text[index] != '.')
                    {
                        // Out of context search, create the corresponding context according to the current instance
                        subDeclarator = EnclosingSubDeclarator(Instance as IModelElement);
                        while (subDeclarator != null)
                        {
                            ConsiderSubDeclarator(prefix, subDeclarator, retVal);
                            subDeclarator =
                                EnclosingSubDeclarator(((IModelElement) subDeclarator).Enclosing as IModelElement);
                        }

                        // Also add the templates
                        foreach (string template in Templates)
                        {
                            if (template.StartsWith(prefix))
                            {
                                retVal.Add(new ObjectReference(template, null));
                            }
                        }
                    }
                }

                if (subDeclarator != null)
                {
                    ConsiderSubDeclarator(prefix, subDeclarator, retVal);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Considers this sub declarator to create the list of choices
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="subDeclarator"></param>
        /// <param name="retVal"></param>
        private static void ConsiderSubDeclarator(string prefix, ISubDeclarator subDeclarator, ICollection<ObjectReference> retVal)
        {
            IVariable variable = subDeclarator as IVariable;
            if (variable != null)
            {
                subDeclarator = variable.Type as ISubDeclarator;
            }

            if (subDeclarator != null)
            {
                foreach (KeyValuePair<string, List<INamable>> pair in subDeclarator.DeclaredElements)
                {
                    if (pair.Key.StartsWith(prefix))
                    {
                        foreach (INamable namable in pair.Value)
                        {
                            retVal.Add(new ObjectReference(pair.Key, namable));
                        }
                    }
                }
            }
        }

        private int _selectionStart;
        private int _selectionLength;

        /// <summary>
        ///     Displays the combo box if required and updates the edotor's text
        /// </summary>
        private void DisplayComboBox()
        {
            int index = EditionTextBox.SelectionStart - 1;

            string prefix = CurrentPrefix(index).Trim();
            index = Math.Max(0, index - prefix.Length);
            List<ObjectReference> allChoices = AllChoices(index, prefix);

            if (prefix.Length <= EditionTextBox.SelectionStart)
            {
                // It seems that selection start and length may be lost when losing the focus. 
                // Store them to be able to reapply them 
                _selectionStart = EditionTextBox.SelectionStart - prefix.Length;
                _selectionLength = prefix.Length;
                EditionTextBox.Select(_selectionStart, _selectionLength);
                if (allChoices.Count == 1)
                {
                    EditionTextBox.SelectedText = allChoices[0].DisplayName;
                }
                else if (allChoices.Count > 1)
                {
                    SelectionComboBox.Items.Clear();
                    foreach (ObjectReference choice in allChoices)
                    {
                        SelectionComboBox.Items.Add(choice);
                    }
                    if (prefix.Length > 0)
                    {
                        SelectionComboBox.Text = prefix;
                    }
                    else
                    {
                        SelectionComboBox.Text = allChoices[0].DisplayName;
                    }

                    // Try to compute the combo box location
                    // TODO : Hypothesis. The first displayed line is the first line of the text
                    int line = 1;
                    string lineData = "";
                    for (int i = 0; i < EditionTextBox.SelectionStart; i++)
                    {
                        switch (EditionTextBox.Text[i])
                        {
                            case '\n':
                                line += 1;
                                lineData = "";
                                break;

                            default:
                                lineData += EditionTextBox.Text[i];
                                break;
                        }
                    }

                    SizeF size = CreateGraphics().MeasureString(lineData, EditionTextBox.Font);
                    int x = Math.Min((int) size.Width, Location.X + Size.Width - SelectionComboBox.Width);
                    int y = (line - 1)*EditionTextBox.Font.Height + 5;
                    Point comboBoxLocation = new Point(x, y);
                    SelectionComboBox.Location = comboBoxLocation;
                    PendingSelection = true;
                    EditionTextBox.SendToBack();
                    SelectionComboBox.Show();
                    SelectionComboBox.Focus();
                }
            }
        }

        private void Editor_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (AutoComplete)
                {
                    if (e.Control)
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Space:
                                // Remove the space that has just been added
                                EditionTextBox.Select(EditionTextBox.SelectionStart - 1, 1);
                                EditionTextBox.SelectedText = "";
                                DisplayComboBox();
                                e.Handled = true;
                                break;

                            case Keys.I:
                                // Remove the space that has just been added
                                EditionTextBox.Select(EditionTextBox.SelectionStart - 1, 1);
                                EditionTextBox.SelectedText = "";
                                EditionTextBox.Indent();
                                e.Handled = true;
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            if (!e.Handled)
            {
                if (e.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.A:
                            EditionTextBox.SelectAll();
                            e.Handled = true;
                            break;

                        case Keys.C:
                            EditionTextBox.Copy();
                            e.Handled = true;
                            break;

                        case Keys.V:
                            e.Handled = true;
                            break;
                    }
                }
            }
        }

        private void Editor_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (AutoComplete)
                {
                    string prefix;
                    switch (e.KeyChar)
                    {
                        case '.':
                            EditionTextBox.SelectedText = e.KeyChar.ToString(CultureInfo.InvariantCulture);
                            e.Handled = true;
                            DisplayComboBox();
                            break;

                        case '{':
                            prefix = CurrentPrefix(EditionTextBox.SelectionStart - 1).Trim();
                            Expression structureTypeExpression = new Parser().Expression(Instance as ModelElement,
                                prefix, IsStructure.INSTANCE, true, null, true);
                            if (structureTypeExpression != null)
                            {
                                Structure structure = structureTypeExpression.Ref as Structure;
                                if (structure != null)
                                {
                                    TextualExplanation text = new TextualExplanation();
                                    text.WriteLine("{");
                                    CreateDefaultStructureValue(text, structure, false);
                                    EditionTextBox.SelectedText = text.Text;
                                    EditionTextBox.ProcessAllLines();
                                    e.Handled = true;
                                }
                            }
                            break;

                        case '(':
                            ICallable callable = GetInstance(EditionTextBox.SelectionStart-1) as ICallable;
                            if (callable != null)
                            {
                                TextualExplanation text = new TextualExplanation();
                                CreateCallableParameters(text, callable);
                                EditionTextBox.SelectedText = text.Text;
                                EditionTextBox.ProcessAllLines();
                                e.Handled = true;
                            }
                            break;

                        case '>':
                        case '-':
                            prefix = CurrentPrefix(EditionTextBox.SelectionStart - 2).Trim();
                            char prev = EditionTextBox.Text[EditionTextBox.SelectionStart - 1];
                            if ((prev == '<' && e.KeyChar == '-') || (prev == '=' && e.KeyChar == '>'))
                            {
                                Expression variableExpression = new Parser().Expression(Instance as ModelElement,
                                    prefix, IsTypedElement.INSTANCE, true, null, true);
                                if (variableExpression != null)
                                {
                                    ITypedElement typedElement = variableExpression.Ref as ITypedElement;
                                    if (typedElement != null)
                                    {
                                        EditionTextBox.SelectedText = e.KeyChar + " " + typedElement.Type.FullName;
                                        e.Handled = true;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void ConfirmComboBoxSelection()
        {
            if (PendingSelection)
            {
                PendingSelection = false;
                EditionTextBox.Select(_selectionStart, _selectionLength);

                EditionTextBox.SelectedText = SelectionComboBox.Text;
                EditionTextBox.SelectionStart = EditionTextBox.SelectionStart;
                SelectionComboBox.Text = "";
                SelectionComboBox.Items.Clear();
                SelectionComboBox.Hide();
                explainRichTextBox.Hide();
            }
        }

        private void AbordComboBoxSelection()
        {
            if (PendingSelection)
            {
                PendingSelection = false;
                SelectionComboBox.Text = "";
                SelectionComboBox.Items.Clear();
                SelectionComboBox.Hide();
                explainRichTextBox.Hide();
            }
        }

        private void SelectionComboBox_LostFocus(object sender, EventArgs e)
        {
            ConfirmComboBoxSelection();
        }

        private void SelectionComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    ConfirmComboBoxSelection();
                    break;

                case Keys.Escape:
                    AbordComboBoxSelection();
                    break;
            }
        }

        /// <summary>
        ///     Sets the variable in the editor
        /// </summary>
        /// <param name="variable"></param>
        protected string SetVariable(IVariable variable)
        {
            TextualExplanation text = new TextualExplanation();

            text.Write(StripUseless(variable.FullName, WritingContext()) + " <- ");
            Structure structure = variable.Type as Structure;
            if (structure != null)
            {
                CreateDefaultStructureValue(text, structure);
            }
            else
            {
                text.Write(variable.DefaultValue.FullName);
            }

            return text.Text;
        }

        protected void CreateDefaultStructureValue(TextualExplanation text, Structure structure,
            bool displayStructureName = true)
        {
            if (displayStructureName)
            {
                text.WriteLine(StripUseless(structure.FullName, WritingContext()) + "{");
            }

            bool first = true;
            foreach (StructureElement element in structure.Elements)
            {
                if (!first)
                {
                    text.WriteLine(",");
                }
                InsertElement(element, text);
                first = false;
            }
            text.WriteLine();
            text.Write("}");
        }

        protected void CreateCallableParameters(TextualExplanation text, ICallable callable)
        {
            if (callable.FormalParameters.Count > 0)
            {
                if (callable.FormalParameters.Count == 1)
                {
                    Parameter formalParameter = callable.FormalParameters[0] as Parameter;
                    if(formalParameter != null)
                    {
                        text.Write("( " + formalParameter.Name + " => " + formalParameter.Type.Default + " )");
                    }
                }
                else
                {
                    text.WriteLine("(");
                    text.Indent(4, () =>
                    {
                        bool first = true;
                        foreach (Parameter parameter in callable.FormalParameters)
                        {
                            if (!first)
                            {
                                text.WriteLine(",");
                            }
                            text.Write(parameter.Name + " => " + parameter.Type.Default);
                            first = false;
                        }
                    });
                    text.WriteLine();
                    text.Write(")");
                }
            }
            else
            {
                text.Write("()");
            }
        }

        protected void InsertElement(ITypedElement element, TextualExplanation text)
        {
            text.Write(element.Name);
            text.Write(" => ");
            Structure structure = element.Type as Structure;
            if (structure != null)
            {
                text.WriteLine(StripUseless(structure.FullName, WritingContext()) + "{");
                text.Indent(4, () =>
                {
                    bool first = true;
                    foreach (StructureElement subElement in structure.Elements)
                    {
                        if (!first)
                        {
                            text.WriteLine(",");
                        }
                        InsertElement(subElement, text);
                        first = false;
                    }
                });
                text.WriteLine();
                text.Write("}");
            }
            else
            {
                IValue value = null;
                if (string.IsNullOrEmpty(element.Default))
                {
                    // No default value for element, get the one of the type
                    if (element.Type != null && element.Type.DefaultValue != null)
                    {
                        value = element.Type.DefaultValue;
                    }
                }
                else
                {
                    if (element.Type != null)
                    {
                        value = element.Type.getValue(element.Default);
                    }
                }

                if (value != null)
                {
                    text.Write(StripUseless(value.FullName, WritingContext()));
                }
            }
        }

        /// <summary>
        ///     Provides the writing context of this edition
        /// </summary>
        /// <returns></returns>
        protected IModelElement WritingContext()
        {
            IModelElement retVal = Model;

            if (retVal is Action || retVal is Expectation)
            {
                retVal = retVal.Enclosing as IModelElement;
            }

            return retVal;
        }

        /// <summary>
        ///     The prefix for the Default namespace
        /// </summary>
        private const string DefaultPrefix = "Default.";

        /// <summary>
        ///     Removes useless prefixes from the string provided
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected string StripUseless(string fullName, IModelElement model)
        {
            string retVal = fullName;

            if (model != null)
            {
                string[] words = fullName.Split('.');
                string[] context = model.FullName.Split('.');

                int i = 0;
                while (i < words.Length && i < context.Length)
                {
                    if (words[i] != context[i])
                    {
                        break;
                    }

                    i++;
                }

                // i is the first different word.
                retVal = "";
                while (i < words.Length)
                {
                    if (!String.IsNullOrEmpty(retVal))
                    {
                        retVal += ".";
                    }
                    retVal = retVal + words[i];
                    i++;
                }

                if (Util.isEmpty(retVal))
                {
                    retVal = model.Name;
                }
            }

            if (retVal.StartsWith(DefaultPrefix))
            {
                retVal = retVal.Substring(DefaultPrefix.Length);
            }

            return retVal;
        }
    }
}