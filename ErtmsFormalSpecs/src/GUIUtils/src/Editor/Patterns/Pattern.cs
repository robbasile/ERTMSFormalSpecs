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

using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using DataDictionary;

namespace GUIUtils.Editor.Patterns
{
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
            set { HighlightFont = new Font(HighlightFont, value); }
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

        /// <summary>
        /// Tokenizes a single line
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="line"></param>
        /// <param name="textParts"></param>
        /// <param name="colorizedLocations"></param>
        public void TokenizeLine(ModelElement instance, string line, List<TextPart> textParts, List<ColorizedLocation> colorizedLocations)
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
                        textParts.Add(new TextPart(match.Index, match.Length, Color, HighlightFont));
                    }
                }
            }
        }
    }
}
