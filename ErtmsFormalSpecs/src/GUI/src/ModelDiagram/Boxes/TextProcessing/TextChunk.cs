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

using System.Drawing;
using GUIUtils.Editor.Patterns;

namespace GUI.ModelDiagram.Boxes.TextProcessing
{
    /// <summary>
    /// Registers a text to display
    /// </summary>
    public class TextChunk
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="part"></param>
        /// <param name="location">The initialLocation of the text chunk in the display</param>
        public TextChunk(TextPart part, Point location)
        {
            Text = part;
            Location = location;
        }

        /// <summary>
        /// The initialLocation of the text chunk on the display panel
        /// </summary>
        public Point Location { get; set; }

        /// <summary>
        /// The size of the text chunk
        /// </summary>
        public SizeF Size
        {
            get
                {
                    return GuiUtils.MeasureDisplayedString(Text.Text, Text.Font);
                }
        }

        /// <summary>
        /// The text to be displayed
        /// </summary>
        public TextPart Text { get; set; }

        /// <summary>
        /// Displays the text chunk using the position, color, ... in the graphics
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="initialLocation">The relative position where the text should be displayed</param>
        public void Display(Graphics graphics, PointF initialLocation)
        {
            graphics.DrawString(Text.Text, Text.Font, new SolidBrush(Text.Color), initialLocation.X + Location.X, initialLocation.Y + Location.Y);
        }
    }
}
