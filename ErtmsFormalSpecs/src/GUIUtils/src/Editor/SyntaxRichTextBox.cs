using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Constants;
using DataDictionary.Functions;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;

namespace GUIUtils.Editor
{
    public class SyntaxRichTextBox : RichTextBox
    {
        /// <summary>
        ///     The patterns to apply
        /// </summary>
        public List<Pattern> Patterns { get; private set; }

        /// <summary>
        ///     The keywords
        /// </summary>
        private static readonly string[] Keywords =
        {
            "FORALL", "THERE_IS", "FIRST", "LAST", "IN", "MAP", "REDUCE", "SUM", "\\|", "USING", "=>", "<-", "NOT",
            "AND", "OR", "LET", "STABILIZE", "INITIAL_VALUE", "STOP_CONDITION",
            "==", "!=", "in", "not in", "<=", ">=", "<", ">", "is", "as", "\\+", "\\-", "\\*", "/", "\\^", "\\.",
            "APPLY", "ON", "INSERT", "WHEN", "FULL", "REPLACE", "REMOVE", "FIRST", "LAST", "ALL", "BY"
        };

        /// <summary>
        ///     The keywords
        /// </summary>
        private static readonly string[] ModelElements =
        {
            "NAMESPACE", "RANGE", "FROM", "TO", "ENUMERATION", "COLLECTION", "OF", "STRUCTURE", "FIELD", "INTERFACE",
            "IMPLEMENTS",
            "STATE MACHINE", "STATE",
            "FUNCTION", "RETURNS", "PROCEDURE", "PARAMETER", "RULE", "IF", "ELSIF", "ELSE", "THEN", "END",
            "FOLDER", "TRANSLATION", "SOURCE TEXT", "IMPLIES",
            "REQUIREMENT SET", "MODEL EVENT"
        };

        /// <summary>
        ///     Regular expression for a qualified name
        /// </summary>
        private const string QualifiedName = "\\b[a-zA-Z][\\.0-9a-zA-Z_]*\\b";

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
        ///     Constructor
        /// </summary>
        public SyntaxRichTextBox()
        {
            RegularFont = new Font(Font, FontStyle.Regular);

            Pattern commentPattern = new Pattern(RegularFont, "//.*$")
            {
                Color = Color.Green
            };
            Pattern holeInTemplatePattern = new Pattern(RegularFont, "<[a-zA-Z\\b]+>")
            {
                Color = Color.OrangeRed
            };
            Pattern integerPattern = new Pattern(RegularFont, "\\b(?:[0-9]*\\.)?[0-9]+\\b")
            {
                Color = Color.BlueViolet
            };
            Pattern stringPattern = new Pattern(RegularFont, "\'[^\']*\'")
            {
                Color = Color.BlueViolet
            };
            ConstantPattern constantPattern = new ConstantPattern(RegularFont, QualifiedName)
            {
                Color = Color.BlueViolet
            };
            Pattern keywordPattern = new ListPattern(RegularFont, Keywords)
            {
                FontStyle = FontStyle.Bold
            };
            Pattern modelElementPattern = new ListPattern(RegularFont, ModelElements)
            {
                FontStyle = FontStyle.Bold | FontStyle.Underline
            };
            TypePattern typePattern = new TypePattern(RegularFont, QualifiedName)
            {
                Color = Color.Blue
            };

            Patterns = new List<Pattern>
            {
                commentPattern,
                holeInTemplatePattern,
                keywordPattern,
                modelElementPattern,
                typePattern,
                integerPattern,
                stringPattern,
                constantPattern,
            };

            TextChanged += SyntaxRichTextBox_TextChanged;
            CanPaint = true;
            ApplyPatterns = true;
        }

        /// <summary>
        ///     Captures WndProc event and filter out paint events when CanPaint is set to false to avoid flickering
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x00f)
            {
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

                List<ColorizedLocation> colored = new List<ColorizedLocation>();
                foreach (Pattern pattern in Patterns)
                {
                    pattern.Colorize(this, Instance, start, line, colored);
                }

                SelectionStart = savedSelectionStart;
                SelectionLength = savedSelectionLength;
                SelectionColor = savedSelectionColor;
            }
        }

        /// <summary>
        ///     Processes all lines in the text box
        /// </summary>
        public void ProcessAllLines()
        {
            if (ApplyPatterns)
            {
                CanPaint = false;

                int start = 0;
                foreach (string line in Lines)
                {
                    ProcessLine(start, line);

                    start = start + line.Length + 1;
                }

                CanPaint = true;
            }
        }
    }

    /// <summary>
    ///     The location where coloring has already taken place
    /// </summary>
    public class ColorizedLocation
    {
        public int Start { get; set; }
        public int End { get; set; }
    }

    /// <summary>
    ///     A pattern to be found in the text, associated to a color
    /// </summary>
    public class Pattern
    {
        /// <summary>
        ///     The color to be applied when the pattern is found
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     The regular expression to find the pattern
        /// </summary>
        public Regex RegExp { get; set; }

        /// <summary>
        ///     The font style to use when applying this pattern
        /// </summary>
        public FontStyle FontStyle
        {
            set { HighlightFont = new Font(HighlightFont, value);}            
        }

        /// <summary>
        /// The font used to colorize
        /// </summary>
        public Font HighlightFont { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="baseFont"></param>
        /// <param name="regExp"></param>
        public Pattern(Font baseFont, string regExp)
        {
            RegExp = new Regex(regExp, RegexOptions.Compiled);
            Color = Color.Black;
            HighlightFont = new Font(baseFont, FontStyle.Regular);
        }

        /// <summary>
        ///     Process a regular expression.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="instance"></param>
        /// <param name="start">The start index of the line</param>
        /// <param name="line">The line to be processed</param>
        /// <param name="colorizedLocations">The location where coloring already occured</param>
        public void Colorize(SyntaxRichTextBox textBox, ModelElement instance, int start, string line,
            List<ColorizedLocation> colorizedLocations)
        {
            foreach (Match match in RegExp.Matches(line))
            {
                if (CheckLocation(match, colorizedLocations))
                {
                    if (AdditionalCheck(match.Value, instance))
                    {
                        colorizedLocations.Add(new ColorizedLocation
                        {
                            Start = match.Index,
                            End = match.Index + match.Length
                        });
                        textBox.SelectionStart = start + match.Index;
                        textBox.SelectionLength = match.Length;
                        textBox.SelectionColor = Color;
                        textBox.SelectionFont = HighlightFont;
                    }
                }
            }
        }

        /// <summary>
        ///     Checks that the matching does not occur in an already colored location
        /// </summary>
        /// <param name="match"></param>
        /// <param name="colorizedLocations"></param>
        /// <returns></returns>
        private bool CheckLocation(Match match, IEnumerable<ColorizedLocation> colorizedLocations)
        {
            bool retVal = true;

            foreach (ColorizedLocation location in colorizedLocations)
            {
                if (match.Index >= location.Start && match.Index <= location.End)
                {
                    retVal = false;
                    break;
                }

                if (match.Index + match.Length >= location.Start && match.Index + match.Length <= location.End)
                {
                    retVal = false;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Performs additional checks on the string provided
        /// </summary>
        /// <param name="text"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual bool AdditionalCheck(string text, ModelElement instance)
        {
            return true;
        }
    }

    /// <summary>
    ///     A pattern used to identify types
    /// </summary>
    public class TypePattern : Pattern
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="regExp"></param>
        /// <param name="baseFont"></param>
        public TypePattern(Font baseFont, string regExp)
            : base(baseFont, regExp)
        {
        }

        /// <summary>
        ///     Ensures that the string provided is a type
        /// </summary>
        /// <param name="text"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override bool AdditionalCheck(string text, ModelElement instance) 
        {
            bool retVal = base.AdditionalCheck(text, instance);

            if (retVal && instance != null)
            {
                Expression expression = instance.EFSSystem.Parser.Expression(instance, text, IsType.INSTANCE, true, null,
                    true);
                retVal = (expression != null && expression.Ref != null) && !(expression.Ref is Function);
            }

            return retVal;
        }
    }

    /// <summary>
    ///     A pattern used to identify types
    /// </summary>
    public class ConstantPattern : Pattern
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="regExp"></param>
        /// <param name="baseFont"></param>
        public ConstantPattern(Font baseFont, string regExp)
            : base(baseFont, regExp)
        {
        }

        /// <summary>
        ///     Ensures that the string provided is a type
        /// </summary>
        /// <param name="text"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override bool AdditionalCheck(string text, ModelElement instance)
        {
            bool retVal = base.AdditionalCheck(text, instance);

            if (retVal && instance != null)
            {
                Expression expression = instance.EFSSystem.Parser.Expression(instance, text, IsValue.INSTANCE, true,
                    null, true);
                retVal = (expression != null && expression.Ref is EnumValue);
            }

            return retVal;
        }
    }

    /// <summary>
    ///     A pattern consisting of a list of elements
    /// </summary>
    public class ListPattern : Pattern
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="baseFont"></param>
        public ListPattern(Font baseFont, params string[] elements)
            : base(baseFont, ComputeRegExp(elements))
        {
        }

        /// <summary>
        ///     Compiles the keywords as a regular expression.
        /// </summary>
        public static string ComputeRegExp(params string[] elements)
        {
            string retVal = "";

            foreach (string element in elements)
            {
                if (!string.IsNullOrEmpty(retVal))
                {
                    retVal += "|";
                }
                if (Char.IsLetterOrDigit(element[0]))
                {
                    retVal += "\\b" + element + "\\b";
                }
                else
                {
                    retVal += element;
                }
            }

            return retVal;
        }
    }
}