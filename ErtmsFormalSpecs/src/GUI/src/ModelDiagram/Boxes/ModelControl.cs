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
using DataDictionary;
using GUI.BoxArrowDiagram;
using GUI.ModelDiagram.Arrows;
using GUI.ModelDiagram.Boxes.TextProcessing;
using Utils;
using ModelElement = DataDictionary.ModelElement;

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
        /// The text used to be displayed, as sequence of text chunks (text, position, font)
        /// </summary>
        private ProcessedText TokenizedText { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected ModelControl(ModelDiagramPanel panel, IGraphicalDisplay model)
            : base(panel, model)
        {
            Bold = new Font(Font, FontStyle.Bold);
            Italic = new Font(Font, FontStyle.Italic);
            BoxMode = BoxModeEnum.Custom;
            TokenizedText = new ProcessedText();
        }

        /// <summary>
        /// Add a text in the control
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="color"> If color is not transparent, then there is no conditional formating and these color and font are chosen</param>
        /// <param name="vOffset"></param>
        /// <returns>The size of the newly added text</returns>
        public SizeF AddText(ModelElement instance, string text, Font font, Color color, float vOffset)
        {
            float height;
            if (color == Color.Transparent)
            {
                height = TokenizedText.Tokenize(instance, text, font, vOffset);
            }
            else
            {
                height = TokenizedText.AddRawText(text, font, color, vOffset);                
            }

            return new SizeF(TokenizedText.Size.Width, height);
        }

        /// <summary>
        /// Provides the computed position and size
        /// </summary>
        public bool ComputedPositionAndSize { get; set; }

        public Size ComputedSize { get; set; }
        public Point ComputedLocation { get; set; }

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

            if (TokenizedText.IsEmpty())
            {
                // Write the title using the Model and TypedModel graphical name
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
                    new Point(Location.X, Location.Y + Font.Height + 7),
                    new Point(Location.X + Width, Location.Y + Font.Height + 7));

                // Syntax highlighting
                // Display the pre computed text at their corresponding locations
                TokenizedText.Display(graphics, new PointF(Location.X + 5, Location.Y + 5));
            }
        }
    }
}