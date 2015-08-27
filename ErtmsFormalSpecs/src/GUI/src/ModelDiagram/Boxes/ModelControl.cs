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
using Utils;

namespace GUI.ModelDiagram.Boxes
{
    /// <summary>
    ///     The boxes that represent a model element
    /// </summary>
    public abstract class ModelControl : BoxControl<IModelElement, IGraphicalDisplay, ModelArrow>
    {
        public enum PositionEnum
        {
            Top,
            Center,
            Bottom,
            None
        };

        /// <summary>
        /// The position of the graphical name
        /// </summary>
        public PositionEnum GraphicalNamePosition { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected ModelControl(ModelDiagramPanel panel, IGraphicalDisplay model)
            : base(panel, model)
        {
            BoxMode = BoxModeEnum.Custom;
            GraphicalNamePosition = PositionEnum.Center;
        }

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

            // Write the text
            Font bold = new Font(Font, FontStyle.Bold);

            string typeName = GuiUtils.AdjustForDisplay(ModelName, Width - 4, bold);
            Brush textBrush = new SolidBrush(Color.Black);
            graphics.DrawString(typeName, bold, textBrush, Location.X + 2, Location.Y + 2);
            graphics.DrawLine(NormalPen, new Point(Location.X, Location.Y + Font.Height + 2),
                new Point(Location.X + Width, Location.Y + Font.Height + 2));

            string name = GuiUtils.AdjustForDisplay(TypedModel.GraphicalName, Width, Font);
            SizeF textSize = graphics.MeasureString(name, Font);
            int boxHeight = Height - bold.Height - 4;
            switch (GraphicalNamePosition)
            {
                case PositionEnum.Center:
                {
                    // Center the element name
                    graphics.DrawString(name, Font, textBrush, Location.X + Width/2 - textSize.Width/2,
                        Location.Y + bold.Height + 4 + boxHeight/2 - Font.Height/2);
                    break;
                }
                case PositionEnum.Top:
                {
                    // Place the element name at the top
                    graphics.DrawString(name, Font, textBrush, Location.X + 5, Location.Y + 20);
                    break;
                }
                case PositionEnum.Bottom:
                {
                    // Place the element name at the top
                    graphics.DrawString(name, Font, textBrush, Location.X + 5, Location.Y + Height - textSize.Height - 5);
                    break;
                }
                case PositionEnum.None:
                {
                    break;
                }
            }
        }
    }
}