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
using Utils;

namespace GUI.ModelDiagram
{
    /// <summary>
    ///     The boxes that represent a model element
    /// </summary>
    public abstract class ModelControl : BoxControl<IModelElement, IGraphicalDisplay, ModelArrow>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        protected ModelControl(ModelDiagramPanel panel, IGraphicalDisplay model)
            : base(panel, model)
        {
            BoxMode = BoxModeEnum.Custom;
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

            // Center the element name
            string name = GuiUtils.AdjustForDisplay(TypedModel.GraphicalName, Width, Font);
            SizeF textSize = graphics.MeasureString(name, Font);
            int boxHeight = Height - bold.Height - 4;
            graphics.DrawString(name, Font, textBrush, Location.X + Width/2 - textSize.Width/2,
                Location.Y + bold.Height + 4 + boxHeight/2 - Font.Height/2);
        }
    }
}