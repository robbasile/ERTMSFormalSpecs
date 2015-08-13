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
using ModelElement = Utils.ModelElement;

namespace GUI.BoxArrowDiagram
{
    public class BoxControl<TEnclosing, TBoxModel, TArrowModel> : GraphicElement
        where TEnclosing : class
        where TBoxModel : class, IGraphicalDisplay
        where TArrowModel : class, IGraphicalArrow<TBoxModel>
    {

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="model"></param>
        public BoxControl(BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel> panel, TBoxModel model)
            : base(panel, model)
        {
            BoxMode = BoxModeEnum.Rectangle3D;
            Panel = panel;

            if (TypedModel.Hidden)
            {
                Font = new Font(Font, FontStyle.Italic);
                ForeColor = Color.Gray;
            }
            else
            {
                Font = new Font(Font, FontStyle.Regular);
                ForeColor = Color.Black;
            }
        }

        /// <summary>
        ///     The mode of displaying boxes
        /// </summary>
        protected enum BoxModeEnum
        {
            Custom,
            Rectangle3D,
            RoundedCorners
        };

        /// <summary>
        ///     The mode of displaying boxes
        /// </summary>
        protected BoxModeEnum BoxMode { get; set; }

        /// <summary>
        ///     Provides the enclosing box-arrow panel
        /// </summary>
        public BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel> Panel { get; private set; }

        /// <summary>
        ///     The model for this control
        /// </summary>
        public TBoxModel TypedModel
        {
            get { return Model as TBoxModel; }
        }

        /// <summary>
        /// The location of the box
        /// </summary>
        public override Point Location
        {
            get { return new Point(TypedModel.X, TypedModel.Y); }
            set
            {
                TypedModel.X = value.X;
                TypedModel.Y = value.Y;
            }
        }

        /// <summary>
        /// The size of the box
        /// </summary>
        public override Size Size
        {
            get { return new Size(TypedModel.Width, TypedModel.Height); }
            set
            {
                TypedModel.Width = value.Width;
                TypedModel.Height = value.Height;
            }
        }

        /// <summary>
        ///     Sets the color of the control
        /// </summary>
        /// <param name="color"></param>
        protected void SetColor(Color color)
        {
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (color != BackColor)
            {
                BackColor = color;
            }
        }

        /// <summary>
        ///     A normal pen
        /// </summary>
        public Color NormalColor = Color.LightGray;

        public Pen NormalPen = new Pen(Color.Black);

        /// <summary>
        ///     A normal pen
        /// </summary>
        public Color HiddenColor = Color.Transparent;

        public Pen HiddenPen = new Pen(Color.Gray);

        /// <summary>
        ///     A activated pen
        /// </summary>
        public Color ActivatedColor = Color.Blue;

        public Pen ActivatedPen = new Pen(Color.Black, 4);

        /// <summary>
        ///     Indicates that the box should be displayed in the ACTIVE color
        /// </summary>
        /// <returns></returns>
        public virtual bool IsActive()
        {
            return false;
        }

        /// <summary>
        ///     Indicates that the box should be displayed in the HIDDEN color
        /// </summary>
        /// <returns></returns>
        public virtual bool IsHidden()
        {
            return TypedModel.Hidden;
        }

        /// <summary>
        ///     The size of a round corner
        /// </summary>
        private const int RoundSize = 10;

        /// <summary>
        ///     Draws the box within the box-arrow panel
        /// </summary>
        /// <param name="g"></param>
        public virtual void PaintInBoxArrowPanel(Graphics g)
        {
            // Select the right pen, according to the model
            Pen pen = SelectPen();

            // Draw the box
            switch (BoxMode)
            {
                case BoxModeEnum.Rectangle3D:
                {
                    Brush innerBrush = new SolidBrush(NormalColor);
                    g.FillRectangle(innerBrush, Location.X, Location.Y, Width, Height);
                    g.DrawRectangle(pen, Location.X, Location.Y, Width, Height);

                    // Center the element name
                    string name = GuiUtils.AdjustForDisplay(TypedModel.GraphicalName, Width, Font);
                    SizeF textSize = g.MeasureString(name, Font);
                    g.DrawString(name, Font, new SolidBrush(NormalPen.Color), Location.X + Width / 2 - textSize.Width / 2,
                        Location.Y + Height / 2 - Font.Height / 2);
                    break;
                }

                case BoxModeEnum.RoundedCorners:
                {
                    Point[] points =
                    {
                        new Point(Location.X + RoundSize, Location.Y),
                        new Point(Location.X + Width - RoundSize, Location.Y),
                        new Point(Location.X + Width, Location.Y + RoundSize),
                        new Point(Location.X + Width, Location.Y + Height - RoundSize),
                        new Point(Location.X + Width - RoundSize, Location.Y + Height),
                        new Point(Location.X + RoundSize, Location.Y + Height),
                        new Point(Location.X, Location.Y + Height - RoundSize),
                        new Point(Location.X, Location.Y + RoundSize)
                    };

                    Brush innerBrush = new SolidBrush(NormalColor);
                    g.FillRectangle(innerBrush,
                        new Rectangle(points[0], new Size(points[4].X - points[0].X, points[4].Y - points[0].Y)));
                    g.FillRectangle(innerBrush,
                        new Rectangle(points[7], new Size(points[3].X - points[7].X, points[3].Y - points[7].Y)));

                    g.DrawLine(pen, points[0], points[1]);
                    g.DrawLine(pen, points[2], points[3]);
                    g.DrawLine(pen, points[4], points[5]);
                    g.DrawLine(pen, points[6], points[7]);

                    Size rectangleSize = new Size(2*RoundSize, 2*RoundSize);
                    Rectangle rectangle = new Rectangle(new Point(points[0].X - RoundSize, points[0].Y), rectangleSize);
                    g.FillPie(innerBrush, rectangle, 180.0f, 90.0f);
                    g.DrawArc(pen, rectangle, 180.0f, 90.0f);

                    rectangle = new Rectangle(new Point(points[2].X - 2*RoundSize, points[2].Y - RoundSize),
                        rectangleSize);
                    g.FillPie(innerBrush, rectangle, 270.0f, 90.0f);
                    g.DrawArc(pen, rectangle, 270.0f, 90.0f);

                    rectangle = new Rectangle(new Point(points[4].X - RoundSize, points[4].Y - 2*RoundSize),
                        rectangleSize);
                    g.FillPie(innerBrush, rectangle, 0.0f, 90.0f);
                    g.DrawArc(pen, rectangle, 0.0f, 90.0f);

                    rectangle = new Rectangle(new Point(points[6].X, points[6].Y - RoundSize), rectangleSize);
                    g.FillPie(innerBrush, rectangle, 90.0f, 90.0f);
                    g.DrawArc(pen, rectangle, 90.0f, 90.0f);
                    break;
                }
            }
        }

        /// <summary>
        /// Selects the pen, according to the control status
        /// </summary>
        /// <returns></returns>
        public override Pen SelectPen()
        {
            Pen retVal;

            if (IsActive())
            {
                retVal = ActivatedPen;
                SetColor(ActivatedColor);
            }
            else if (IsHidden())
            {
                retVal = HiddenPen;
                SetColor(HiddenColor);
            }
            else
            {
                retVal = NormalPen;
                SetColor(NormalColor);
            }

            // Bigger pen for selected elements
            if (Panel.IsSelected(this))
            {
                retVal = new Pen(retVal.Color, 4);
            }
            return retVal;
        }

        /// <summary>
        ///     Provides the center of the box control
        /// </summary>
        public Point Center
        {
            get
            {
                Point retVal = Location;

                retVal.X = retVal.X + Width/2;
                retVal.Y = retVal.Y + Height/2;

                return retVal;
            }
        }
        
        /// <summary>
        ///     Provides the span of this control, over the X axis
        /// </summary>
        public Span XSpan
        {
            get { return new Span(Location.X, Location.X + Width); }
        }

        /// <summary>
        ///     Provides the span of this control, over the Y axis
        /// </summary>
        public Span YSpan
        {
            get { return new Span(Location.Y, Location.Y + Height); }
        }

        /// <summary>
        ///     Accepts a drop event from a model element
        /// </summary>
        /// <param name="element"></param>
        public virtual void AcceptDrop(ModelElement element)
        {
        }
    }
}