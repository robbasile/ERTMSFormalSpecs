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
using System.Windows.Forms;
using DataDictionary;
using GUI.BoxArrowDiagram;

namespace GUI.ModelDiagram
{
    /// <summary>
    ///     The boxes that represent a model element
    /// </summary>
    public abstract class ModelControl : BoxControl<IGraphicalDisplay, ModelArrow>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public ModelControl(IGraphicalDisplay model)
            : base()
        {
            Model = model;
            BoxMode = BoxModeEnum.Rectangle;

            MouseUp += new MouseEventHandler(ModelControl_MouseUp);
        }

        private void ModelControl_MouseUp(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        ///     Avoid the control displaying the graphical name itself.
        ///     Tt shall be done during the PaintInBoxArrowPanel method
        /// </summary>
        public override string Text
        {
            get { return ""; }
        }

        /// <summary>
        ///     The name of the kind of model
        /// </summary>
        public abstract string ModelName { get; }

        public override void PaintInBoxArrowPanel(Graphics graphics)
        {
            base.PaintInBoxArrowPanel(graphics);

            Font bold = new Font(Font, FontStyle.Bold);

            string typeName = GUIUtils.AdjustForDisplay(graphics, ModelName, Width - 4, bold);
            Brush textBrush = new SolidBrush(Color.Black);
            graphics.DrawString(typeName, bold, textBrush, Location.X + 2, Location.Y + 2);
            Pen border = new Pen(Color.Black);
            graphics.DrawLine(border, new Point(Location.X, Location.Y + Font.Height + 2),
                new Point(Location.X + Width, Location.Y + Font.Height + 2));

            // Center the element name
            string name = GUIUtils.AdjustForDisplay(graphics, Model.GraphicalName, Width, Font);
            SizeF textSize = graphics.MeasureString(name, Font);
            int boxHeight = Height - bold.Height - 4;
            graphics.DrawString(name, Font, textBrush, Location.X + Width/2 - textSize.Width/2,
                Location.Y + bold.Height + 4 + boxHeight/2 - Font.Height/2);
        }
    }
}