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
        private const int WmHscroll = 0x114;
        private const int WmVscroll = 0x115;
        private const int WmMousewheel = 0x20A;

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
            else if (m.Msg == WmVscroll || m.Msg == WmHscroll || m.Msg == WmMousewheel) 
            {
                // Capture scroll events
                if (CanPaint)
                {
                    ProcessAllLines();
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
        private int _firstVisibleIndex;
        private int _lastVisibleIndex;
        private HashSet<int> _processedLines;

        /// <summary>
        /// Cleans meta data about what is displayed
        /// </summary>
        private void Clean()
        {
            _firstVisibleIndex = -1;
            _lastVisibleIndex = -1;
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

                int firstVisibleIndex = GetCharIndexFromPosition(new Point(0, 0));
                int lastVisibleIndex = GetCharIndexFromPosition(new Point(Size.Width, Size.Height));
                if (firstVisibleIndex != _firstVisibleIndex || lastVisibleIndex != _lastVisibleIndex)
                {
                    _firstVisibleIndex = firstVisibleIndex;
                    _lastVisibleIndex = lastVisibleIndex;

                    int lineNumber = 0;
                    int start = 0;
                    foreach (string line in Lines)
                    {
                        lineNumber = lineNumber + 1;
                        if (start >= _firstVisibleIndex && start <= _lastVisibleIndex)
                        {
                            if (!_processedLines.Contains(lineNumber))
                            {
                                ProcessLine(start, line);
                                _processedLines.Add(lineNumber);
                            }
                        }

                        start = start + line.Length + 1;
                    }
                }

                CanPaint = true;
            }
        }
    }
}