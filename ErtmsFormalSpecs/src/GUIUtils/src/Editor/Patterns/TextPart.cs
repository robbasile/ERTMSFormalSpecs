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
using System.Drawing;

namespace GUIUtils.Editor.Patterns
{
    /// <summary>
    /// Part of a text associated to a color and a font
    /// </summary>
    public class TextPart : IComparable<TextPart>
    {
        /// <summary>
        /// The text to be displayed
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// The start position of the text in the line
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// The text length
        /// </summary>
        public int Length { get { return Text.Length; } }

        /// <summary>
        /// The color used to display the text
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The font used to display the text
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">The recognized text</param>
        /// <param name="start">The start location of the text in the line</param>
        /// <param name="color">The color used to display that text</param>
        /// <param name="font">The font used to display that text</param>
        public TextPart(string text, int start, Color color, Font font)
        {
            Text = text;
            Start = start;
            Color = color;
            Font = font;
        }

        public int CompareTo(TextPart other)
        {
            int retVal = 0;

            if (Start < other.Start)
            {
                retVal = -1;
            }
            else if (Start > other.Start)
            {
                retVal = 1;
            }
            else
            {
                if (Length > other.Length)
                {
                    retVal = -1;
                }
                else if (Length < other.Length)
                {
                    retVal = 1;
                }
            }

            return retVal;
        }
    }
}
