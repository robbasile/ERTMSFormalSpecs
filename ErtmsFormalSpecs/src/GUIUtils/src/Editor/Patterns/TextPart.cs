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
        public int Start { get; set; }
        public int Length { get; set; }
        public Color Color { get; set; }
        public Font Font { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="color"></param>
        /// <param name="font"></param>
        public TextPart(int start, int length, Color color, Font font)
        {
            Start = start;
            Length = length;
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
