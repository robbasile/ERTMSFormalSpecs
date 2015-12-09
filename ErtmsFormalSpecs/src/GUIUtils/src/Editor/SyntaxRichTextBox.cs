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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataDictionary;
using GUIUtils.Editor.Patterns;

namespace GUIUtils.Editor
{
    public class SyntaxRichTextBox : RichTextBox
    {
        /// <summary>
        ///     The instance according to which this text box presents the data
        /// </summary>
        public ModelElement Instance { get; set; }

        /// <summary>
        ///     Indicates that the Paint event can be performed
        ///     This is used to avoid blinking effect when refreshing a line
        /// </summary>
        private bool CanPaint { get; set; }

        /// <summary>
        ///     Indicates that patterns should be applied to color the text box
        /// </summary>
        public bool ApplyPatterns { get; set; }

        /// <summary>
        /// A regular font
        /// </summary>
        private Font RegularFont { get; set; }

        /// <summary>
        /// Recognises elements of EFS
        /// </summary>
        private EfsRecognizer Recognizer { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public SyntaxRichTextBox()
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            RegularFont = new Font(Font, FontStyle.Regular);
            Recognizer = new EfsRecognizer(RegularFont);

            TextChanged += SyntaxRichTextBox_TextChanged;
            SelectionChanged += SyntaxRichTextBox_SelectionChanged;
            
            CanPaint = true;
            ApplyPatterns = true;

            Clean();
        }

        /// <summary>
        /// Handles a text change
        /// </summary>
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                Clean();
            }
        }

        /// <summary>
        /// Recolor the displayed lines
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyntaxRichTextBox_SelectionChanged(object sender, EventArgs e)
        {
            if (CanPaint)
            {
                ProcessAllLines();
            }
        }

        /// <summary>
        /// Constants used in WndProc
        /// </summary>
        private const int WmPaint = 0x00f;

        /// <summary>
        ///     Captures WndProc event and filter out paint events when CanPaint is set to false to avoid flickering
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WmPaint)
            {
                // Capture the paint event to avoid painting when changes occur in the display
                if (CanPaint)
                {
                    base.WndProc(ref m);
                }
                else
                {
                    m.Result = IntPtr.Zero;
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        /// <summary>
        ///     Handles a text change to recolor it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyntaxRichTextBox_TextChanged(object sender, EventArgs e)
        {
            if (CanPaint)
            {
                CanPaint = false;

                // Process the current line.
                int start = StartLine(SelectionStart);
                int end = EndLine(SelectionStart);
                ProcessLine(start, Text.Substring(start, end - start));

                CanPaint = true;
            }
        }

        /// <summary>
        ///     Provides the start of the line, according to the index provided as parameter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int StartLine(int index)
        {
            int retVal = index;

            while ((retVal > 0) && (Text[retVal - 1] != '\n'))
            {
                retVal = retVal - 1;
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the end index of the line, according to the index provided as parameter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int EndLine(int index)
        {
            int retVal = index;

            while ((retVal < Text.Length) && (Text[retVal] != '\n'))
            {
                retVal++;
            }

            return retVal;
        }

        /// <summary>
        ///     Process a line.
        /// </summary>
        /// <param name="start">The index of the start of the line</param>
        /// <param name="line">The line</param>
        private void ProcessLine(int start, string line)
        {
            if (ApplyPatterns)
            {
                // Save SelectionStart and SelectionLength because syntax highlighting will change 
                int savedSelectionStart = SelectionStart;
                int savedSelectionLength = SelectionLength;
                Color savedSelectionColor = SelectionBackColor;

                SelectionStart = start;
                SelectionLength = line.Length;
                SelectionColor = Color.Black;
                SelectionFont = RegularFont;

                Recognizer.Colorize(this, Instance, start, line);

                SelectionStart = savedSelectionStart;
                SelectionLength = savedSelectionLength;
                SelectionColor = savedSelectionColor;
            }
        }

        // Only color the visible lines
        private HashSet<int> _processedLines;

        /// <summary>
        /// Cleans meta data about what is displayed
        /// </summary>
        private void Clean()
        {
            _processedLines = new HashSet<int>();
        }

        /// <summary>
        ///     Processes all lines in the text box
        /// </summary>
        public void ProcessAllLines()
        {
            if (ApplyPatterns)
            {
                CanPaint = false;

                if (Lines.Count() < 300)
                {
                    int lineNumber = 0;
                    int start = 0;
                    foreach (string line in Lines)
                    {
                        lineNumber = lineNumber + 1;
                        if (!_processedLines.Contains(lineNumber))
                        {
                            ProcessLine(start, line);
                            _processedLines.Add(lineNumber);
                        }

                        start = start + line.Length + 1;
                    }
                }
                else
                {
                    // For longer text, it takes too much time to apply to coloring algorithm. 
                    // Don't color anything in that case
                    int savedSelectionStart = SelectionStart;
                    int savedSelectionLength = SelectionLength;
                    Color savedSelectionColor = SelectionBackColor;

                    SelectionStart = 0;
                    SelectionLength = TextLength;
                    SelectionColor = Color.Black;
                    SelectionFont = RegularFont;

                    SelectionStart = savedSelectionStart;
                    SelectionLength = savedSelectionLength;
                    SelectionColor = savedSelectionColor; 
                }
                CanPaint = true;
            }
        }

        /// <summary>
        /// Indents the contents of the syntax text box
        /// </summary>
        public void Indent()
        {
            if (ApplyPatterns)
            {
                int indentLevel = 0;

                // Removes all spaces and carriage returns from the text.
                string text = Text.Replace('\n', ' ');
                while (text.IndexOf("  ", StringComparison.Ordinal) >= 0)
                {
                    text = text.Replace("  ", " ");                    
                }

                bool startOfLine = true;
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];

                    if (c == '(' || c == '{' || c == '[')
                    {
                        result.Append(c);
                        if (!Couple(text, i))
                        {
                            indentLevel += 1;
                            NewLine(result, indentLevel);
                            startOfLine = true;
                        }

                    }
                    else if (c == ')' || c == '}' || c == ']')
                    {
                        if (!Couple(text, i - 1))
                        {
                            indentLevel -= 1;
                            NewLine(result, indentLevel);
                        }
                        result.Append(c);
                        startOfLine = false;
                    }
                    else if (c == ',')
                    {
                        result.Append(c);
                        NewLine(result, indentLevel);
                        startOfLine = true;
                    }
                    else if (c == ' ')
                    {
                        if (!startOfLine)
                        {
                            result.Append(c);
                        }
                    }
                    else
                    {
                        result.Append(c);
                    }

                    startOfLine = startOfLine && c == ' ';
                }

                Text = result.ToString();
                ProcessAllLines();
            }
        }

        /// <summary>
        /// Indicates that two consecutive characters are a couple of parenthesis, brackets or curved brackets
        /// </summary>
        /// <param name="text"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool Couple(string text, int i)
        {
            bool retVal = false;

            if (i > 0 && i < text.Length - 1)
            {
                retVal = (text[i] == '(' && text[i + 1] == ')')
                         || (text[i] == '{' && text[i + 1] == '}')
                         || (text[i] == '[' && text[i + 1] == ']');
            }

            return retVal;
        }

        /// <summary>
        /// Adds a new line
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="level"></param>
        private void NewLine(StringBuilder builder, int level)
        {
            builder.Append('\n');
            AddIndent(builder, level);            
        }

        /// <summary>
        /// Adds an indent on the string
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="level"></param>
        private void AddIndent(StringBuilder builder, int level)
        {
            for (int i = 0; i < level; i++)
            {
                builder.Append("    ");
            }
        }
    }
}