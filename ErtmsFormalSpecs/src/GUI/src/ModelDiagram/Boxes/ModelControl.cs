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
using DataDictionary;
using GUI.BoxArrowDiagram;
using GUI.ModelDiagram.Arrows;
using Utils;

namespace GUI.ModelDiagram.Boxes
{
    /// <summary>
    ///     The boxes that represent a model element
    /// </summary>
    public abstract class ModelControl : BoxControl<IModelElement, IGraphicalDisplay, ModelArrow>
    {
        /// <summary>
        /// The bold font
        /// </summary>
        public Font Bold { get; set; }

        /// <summary>
        /// The italic font
        /// </summary>
        public Font Italic { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected ModelControl(ModelDiagramPanel panel, IGraphicalDisplay model)
            : base(panel, model)
        {
            Bold = new Font(Font, FontStyle.Bold);
            Italic = new Font(Font, FontStyle.Italic);
            BoxMode = BoxModeEnum.Custom;
            Texts = new List<TextPosition>();
        }

        /// <summary>
        /// Registers a text to display
        /// </summary>
        public class TextPosition
        {
            public Point Location { get; set; }
            public string Text { get; set; }
            public Font Font { get; set; }
            public Color Color { get; set; }
        }

        public List<TextPosition> Texts { get; set; }

        /// <summary>
        /// Provides the computed position and size
        /// </summary>
        public bool ComputedPositionAndSize { get; set; }

        public Point ComputedLocation { get; set; }
        public Size ComputedSize { get; set; }

        /// <summary>
        ///     The location of the box
        /// </summary>
        public override Point Location
        {
            get
            {
                Point retVal;

                if (ComputedPositionAndSize)
                {
                    retVal = ComputedLocation;
                }
                else
                {
                    retVal = new Point(TypedModel.X, TypedModel.Y);
                }

                return retVal;
            }
            set
            {
                if (ComputedPositionAndSize)
                {
                    ComputedLocation = value;
                }
                else
                {
                    TypedModel.X = value.X;
                    TypedModel.Y = value.Y;
                }
            }
        }

        /// <summary>
        ///     The size of the box
        /// </summary>
        public override Size Size
        {
            get
            {
                Size retVal;

                if (ComputedPositionAndSize)
                {
                    retVal = ComputedSize;
                }
                else
                {
                    retVal = new Size(TypedModel.Width, TypedModel.Height);
                }

                return retVal;
            }
            set
            {
                if (ComputedPositionAndSize)
                {
                    ComputedSize = value;
                }
                else
                {
                    TypedModel.Width = value.Width;
                    TypedModel.Height = value.Height;
                }
            }
        }

        /// <summary>
        ///     The name of the kind of model
        /// </summary>
        public abstract string ModelName { get; }

        public override void PaintInBoxArrowPanel(Graphics graphics)
        {
            base.PaintInBoxArrowPanel(graphics);

            if (BoxMode == BoxModeEnum.Custom)
            {
                Pen pen = SelectPen();

                // Create the box
                Brush innerBrush = new SolidBrush(NormalColor);
                graphics.FillRectangle(innerBrush, Location.X, Location.Y, Width, Height);
                graphics.DrawRectangle(pen, Location.X, Location.Y, Width, Height);
            }

            if (Texts.Count == 0)
            {
                // Write the title
                string typeName = GuiUtils.AdjustForDisplay(ModelName, Width - 4, Bold);
                Brush textBrush = new SolidBrush(Color.Black);
                graphics.DrawString(typeName, Bold, textBrush, Location.X + 2, Location.Y + 2);
                graphics.DrawLine(NormalPen, new Point(Location.X, Location.Y + Font.Height + 2),
                    new Point(Location.X + Width, Location.Y + Font.Height + 2));

                // Write the text in the box
                // Center the element name
                string name = GuiUtils.AdjustForDisplay(TypedModel.GraphicalName, Width, Font);
                SizeF textSize = graphics.MeasureString(name, Font);
                int boxHeight = Height - Bold.Height - 4;
                graphics.DrawString(name, Font, textBrush, Location.X + Width/2 - textSize.Width/2,
                    Location.Y + Bold.Height + 4 + boxHeight/2 - Font.Height/2);
            }
            else
            {
                // Draw the line between the title and the rest of the box
                graphics.DrawLine(
                    NormalPen,
                    new Point(Location.X, Location.Y + Font.Height + 2),
                    new Point(Location.X + Width, Location.Y + Font.Height + 2));

                // Display the pre computed text at their corresponding locaations
                foreach (TextPosition textPosition in Texts)
                {
                    graphics.DrawString(
                        textPosition.Text,
                        textPosition.Font,
                        new SolidBrush(textPosition.Color),
                        textPosition.Location.X,
                        textPosition.Location.Y);
                }
            }
        }
    }
}