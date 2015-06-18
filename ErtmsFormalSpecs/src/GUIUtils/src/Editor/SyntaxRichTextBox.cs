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

namespace GUI
{
    public class SyntaxRichTextBox : RichTextBox
    {
        /// <summary>
        /// The patterns to apply
        /// </summary>
        public List<Pattern> Patterns { get; private set; }

        /// <summary>
        /// The keywords
        /// </summary>
        private static readonly string[] Keywords =
        {
            "FORALL", "THERE_IS", "FIRST", "LAST", "IN", "MAP", "REDUCE", "SUM", "\\|", "USING", "=>", "<-", "NOT",
            "AND", "OR", "LET", "STABILIZE", "INITIAL_VALUE", "STOP_CONDITION",
            "==", "!=", "in", "not in", "<=", ">=", "<", ">", "is", "as", "\\+", "\\-", "\\*", "/", "\\^", "\\.",
            "APPLY", "ON", "INSERT", "WHEN", "FULL", "REPLACE", "REMOVE", "FIRST", "LAST", "ALL", "BY"
        };

        /// <summary>
        /// The keywords
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
        /// Regular expression for a qualified name
        /// </summary>
        private const string QualifiedName = "\\b[a-zA-Z][\\.0-9a-zA-Z_]*\\b";

        /// <summary>
        /// The instance according to which this text box presents the data
        /// </summary>
        public ModelElement Instance { get; set; }

        /// <summary>
        /// Indicates that the Paint event can be performed
        /// This is used to avoid blinking effect when refreshing a line
        /// </summary>
        private bool CanPaint { get; set; }

        /// <summary>
        /// Indicates that patterns should be applied to color the text box
        /// </summary>
        public bool ApplyPatterns { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SyntaxRichTextBox()
        {
            Pattern commentPattern = new Pattern(Color.Green, Font, "//.*$");
            Pattern holeInTemplatePattern = new Pattern(Color.OrangeRed, Font, "<[a-zA-Z\\b]+>");
            Pattern integerPattern = new Pattern(Color.BlueViolet, Font, "\\b(?:[0-9]*\\.)?[0-9]+\\b");
            Pattern stringPattern = new Pattern(Color.BlueViolet, Font, "\'[^\']*\'");
            ConstantPattern constantPattern = new ConstantPattern(Color.BlueViolet, Font, QualifiedName);
            Pattern keywordPattern = new ListPattern(Color.Black, new Font(Font, FontStyle.Bold), Keywords);
            Pattern modelElementPattern = new ListPattern(Color.Black,
                new Font(Font, FontStyle.Bold | FontStyle.Underline), ModelElements);
            TypePattern typePattern = new TypePattern(Color.Blue, Font, QualifiedName);

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
        /// Captures WndProc event and filter out paint events when CanPaint is set to false to avoid flickering
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
        /// Handles a text change to recolor it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyntaxRichTextBox_TextChanged(object sender, EventArgs e)
        {
            CanPaint = false;

            // Process the current line.
            int start = StartLine(SelectionStart);
            int end = EndLine(SelectionStart);
            ProcessLine(start, Text.Substring(start, end - start));

            CanPaint = true;
        }

        /// <summary>
        /// Provides the start of the line, according to the index provided as parameter
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
        /// Provides the end index of the line, according to the index provided as parameter
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
        /// Process a line.
        /// </summary>
        /// <param name="start">The index of the start of the line</param>
        /// <param name="line">The line</param>
        private void ProcessLine(int start, string line)
        {
            if (ApplyPatterns)
            {
                // Save the position and make the whole line black
                int nPosition = SelectionStart;
                SelectionStart = start;
                SelectionLength = line.Length;
                SelectionColor = Color.Black;
                SelectionFont = new Font(Font, FontStyle.Regular);

                List<ColorizedLocation> colored = new List<ColorizedLocation>();
                foreach (Pattern pattern in Patterns)
                {
                    pattern.Colorize(this, Instance, start, line, colored);
                }

                SelectionStart = nPosition;
                SelectionLength = 0;
                SelectionColor = Color.Black;
            }
        }

        /// <summary>
        /// Processes all lines in the text box
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

                    start = start + line.Length;
                }

                CanPaint = true;
            }
        }
    }

    /// <summary>
    /// The location where coloring has already taken place
    /// </summary>
    public class ColorizedLocation
    {
        public int Start { get; set; }
        public int End { get; set; }
    }

    /// <summary>
    /// A pattern to be found in the text, associated to a color
    /// </summary>
    public class Pattern
    {
        /// <summary>
        /// The color to be applied when the pattern is found
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The font to be applied when the pattern is found
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// The regular expression to find the pattern
        /// </summary>
        public Regex RegExp { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="color"></param>
        /// <param name="font"></param>
        /// <param name="regExp"></param>
        public Pattern(Color color, Font font, string regExp)
        {
            RegExp = new Regex(regExp, RegexOptions.Compiled);
            Color = color;
            Font = font;
        }

        /// <summary>
        /// Process a regular expression.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="instance"></param>
        /// <param name="start">The start index of the line</param>
        /// <param name="line">The line to be processed</param>
        /// <param name="colorizedLocations">The location where coloring already occured</param>
        public void Colorize(SyntaxRichTextBox textBox, ModelElement instance, int start, string line, List<ColorizedLocation> colorizedLocations)
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
                        textBox.SelectionFont = Font;
                    }
                }
            }
        }

        /// <summary>
        /// Checks that the matching does not occur in an already colored location
        /// </summary>
        /// <param name="match"></param>
        /// <param name="colorizedLocations"></param>
        /// <returns></returns>
        private bool CheckLocation(Match match, List<ColorizedLocation> colorizedLocations)
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
        /// Performs additional checks on the string provided
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
    /// A pattern used to identify types
    /// </summary>
    public class TypePattern : Pattern
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="color"></param>
        /// <param name="font"></param>
        /// <param name="regExp"></param>
        public TypePattern(Color color, Font font, string regExp)
            : base(color, font, regExp)
        {
        }

        /// <summary>
        /// Ensures that the string provided is a type
        /// </summary>
        /// <param name="text"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override bool AdditionalCheck(string text, ModelElement instance)
        {
            bool retVal = base.AdditionalCheck(text, instance);

            if (retVal && instance != null)
            {
                Expression expression = instance.EFSSystem.Parser.Expression(instance, text, IsType.INSTANCE, true, null, true);
                retVal = (expression != null && expression.Ref != null) && !(expression.Ref is Function);
            }

            return retVal;
        }
    }

    /// <summary>
    /// A pattern used to identify types
    /// </summary>
    public class ConstantPattern : Pattern
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="color"></param>
        /// <param name="font"></param>
        /// <param name="regExp"></param>
        public ConstantPattern(Color color, Font font, string regExp)
            : base(color, font, regExp)
        {
        }

        /// <summary>
        /// Ensures that the string provided is a type
        /// </summary>
        /// <param name="text"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override bool AdditionalCheck(string text, ModelElement instance)
        {
            bool retVal = base.AdditionalCheck(text, instance);

            if (retVal && instance != null)
            {
                Expression expression = instance.EFSSystem.Parser.Expression(instance, text, IsValue.INSTANCE, true, null, true);
                retVal = (expression != null && expression.Ref is EnumValue);
            }

            return retVal;
        }
    }

    /// <summary>
    /// A pattern consisting of a list of elements
    /// </summary>
    public class ListPattern : Pattern
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="color"></param>
        /// <param name="font"></param>
        /// <param name="elements"></param>
        public ListPattern(Color color, Font font, params string[] elements)
            : base(color, font, ComputeRegExp(elements))
        {
        }

        /// <summary>
        /// Compiles the keywords as a regular expression.
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