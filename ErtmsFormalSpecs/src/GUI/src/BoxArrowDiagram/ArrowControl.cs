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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DataDictionary;
using Utils;

namespace GUI.BoxArrowDiagram
{
    public abstract partial class ArrowControl<TEnclosing, TBoxModel, TArrowModel> : Label
        where TEnclosing : class
        where TBoxModel : class, IGraphicalDisplay
        where TArrowModel : class, IGraphicalArrow<TBoxModel>
    {
        /// <summary>
        ///     The display mode the each arrow
        /// </summary>
        protected enum ArrowModeEnum
        {
            Full,
            Half,
            None
        };

        /// <summary>
        ///     The display mode the each arrow
        /// </summary>
        protected ArrowModeEnum ArrowMode { get; set; }

        /// <summary>
        ///     The way the tip of the arrow is displayed
        /// </summary>
        protected enum ArrowFillEnum
        {
            Fill,
            Line
        };

        /// <summary>
        ///     The way the tip of the arrow is displayed
        /// </summary>
        protected ArrowFillEnum ArrowFill { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected ArrowControl()
        {
            InitializeComponent();
            InitializeColors();

            ArrowMode = ArrowModeEnum.Full;
            ArrowFill = ArrowFillEnum.Line;
            MouseClick += MouseClickHandler;
            MouseDoubleClick += MouseDoubleClickHandler;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        protected ArrowControl(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            InitializeColors();

            ArrowMode = ArrowModeEnum.Full;
            ArrowFill = ArrowFillEnum.Line;
            MouseClick += MouseClickHandler;
            MouseDoubleClick += MouseDoubleClickHandler;
        }

        /// <summary>
        ///     Initializes the colors of the pens
        /// </summary>
        private void InitializeColors()
        {
            NormalColor = Color.Black;
            NormalPen = new Pen(NormalColor);

            DeducedCaseColor = Color.MediumPurple;
            DeducedCasePen = new Pen(DeducedCaseColor);

            DisabledColor = Color.Red;
            DisabledPen = new Pen(DisabledColor);

            ActivatedColor = Color.Blue;
            ActivatedPen = new Pen(ActivatedColor, 4);

            ExternalBoxColor = Color.Green;
            ExternalBoxPen = new Pen(ExternalBoxColor, 2);
        }

        /// <summary>
        ///     Handles a mouse click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        private void MouseClickHandler(object sender, MouseEventArgs mouseEventArgs)
        {
            IModelElement modelElement = Model as IModelElement;
            if (modelElement != null)
            {
                Context.SelectionCriteria criteria = GuiUtils.SelectionCriteriaBasedOnMouseEvent(mouseEventArgs);
                EFSSystem.INSTANCE.Context.SelectElement(modelElement, this, criteria);
            }
        }

        /// <summary>
        ///     Handles a mouse click event on an arrow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        private void MouseDoubleClickHandler(object sender, MouseEventArgs mouseEventArgs)
        {
            IModelElement modelElement = Model as IModelElement;
            if (modelElement != null)
            {
                Context.SelectionCriteria criteria = GuiUtils.SelectionCriteriaBasedOnMouseEvent(mouseEventArgs);
                EFSSystem.INSTANCE.Context.SelectElement(modelElement, this, criteria);
            }
        }

        /// <summary>
        ///     The parent box-arrow panel
        /// </summary>
        public BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel> BoxArrowPanel
        {
            get { return GuiUtils.EnclosingFinder<BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel>>.Find(this); }
        }

        /// <summary>
        ///     Provides the enclosing form
        /// </summary>
        public Form EnclosingForm
        {
            get { return GuiUtils.EnclosingFinder<Form>.Find(this); }
        }

        /// <summary>
        ///     Provides the enclosing box-arrow diagram panel
        /// </summary>
        public BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel> Panel
        {
            get { return GuiUtils.EnclosingFinder<BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel>>.Find(this); }
        }

        /// <summary>
        ///     The Model
        /// </summary>
        private TArrowModel _model;

        public virtual TArrowModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                RefreshControl();
            }
        }

        /// <summary>
        ///     Refreshes the control contents, according to the modeled arrow
        /// </summary>
        public void RefreshControl()
        {
            Text = Model.GraphicalName;

            if (Panel != null)
            {
                Panel.UpdateArrowPosition();
                Panel.Refresh();
            }
        }

        /// <summary>
        ///     Provides the box control which corresponds to the initial state
        /// </summary>
        public BoxControl<TEnclosing, TBoxModel, TArrowModel> SourceBoxControl
        {
            get
            {
                BoxControl<TEnclosing, TBoxModel, TArrowModel> retVal = null;

                if (Model.Source != null)
                {
                    retVal = Panel.GetBoxControl(Model.Source);
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the box control which corresponds to the target state
        /// </summary>
        public BoxControl<TEnclosing, TBoxModel, TArrowModel> TargetBoxControl
        {
            get
            {
                BoxControl<TEnclosing, TBoxModel, TArrowModel> retVal = Panel.GetBoxControl(Model.Target);

                return retVal;
            }
        }

        /// <summary>
        ///     The default size of a arrow. This is used when one of the control ending the arrow does not belong to the diagram
        /// </summary>
        public int DefaultArrowLength = 40;

        private const double ArrowLength = 10.0;
        private const double ArrowAngle = Math.PI/6;

        /// <summary>
        ///     Provides the angle the arrow performs
        /// </summary>
        public double Angle
        {
            get
            {
                double retVal = Math.PI/2;

                if (SourceBoxControl != null && TargetBoxControl != null)
                {
                    double deltaX = TargetBoxControl.Center.X - SourceBoxControl.Center.X;
                    double deltaY = TargetBoxControl.Center.Y - SourceBoxControl.Center.Y;
                    retVal = Math.Atan2(deltaY, deltaX);

                    // Make horizontal or vertical arrows, when possible
                    if (Span.Intersection(SourceBoxControl.XSpan, TargetBoxControl.XSpan) != null)
                    {
                        if (retVal >= 0)
                        {
                            // Quadrant 1 & 2
                            retVal = Math.PI/2;
                        }
                        else
                        {
                            // Quadrant 3 & 4
                            retVal = -Math.PI/2;
                        }
                    }
                    else
                    {
                        if (Span.Intersection(SourceBoxControl.YSpan, TargetBoxControl.YSpan) != null)
                        {
                            if (Math.Abs(retVal) >= Math.PI/2)
                            {
                                // Quadrant 2 & 3
                                retVal = Math.PI;
                            }
                            else
                            {
                                // Quadrant 1 & 4
                                retVal = 0;
                            }
                        }
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the start location of the arrow
        /// </summary>
        public Point StartLocation
        {
            get
            {
                Point retVal;

                BoxControl<TEnclosing, TBoxModel, TArrowModel> initialBoxControl = SourceBoxControl;
                BoxControl<TEnclosing, TBoxModel, TArrowModel> targetBoxControl = TargetBoxControl;

                if (initialBoxControl != null)
                {
                    Point center = initialBoxControl.Center;
                    double angle = Angle;

                    int x = center.X + (int) (Math.Cos(angle)*initialBoxControl.Width/2);
                    int y = center.Y + (int) (Math.Sin(angle)*initialBoxControl.Height/2);

                    if (targetBoxControl != null)
                    {
                        Span xIntersection = Span.Intersection(initialBoxControl.XSpan, targetBoxControl.XSpan);
                        if (xIntersection != null)
                        {
                            x = xIntersection.Center +
                                Math.Max(initialBoxControl.Location.X, targetBoxControl.Location.X);
                        }

                        Span yIntersection = Span.Intersection(initialBoxControl.YSpan, targetBoxControl.YSpan);
                        if (yIntersection != null)
                        {
                            y = yIntersection.Center +
                                Math.Max(initialBoxControl.Location.Y, targetBoxControl.Location.Y);
                        }
                    }

                    retVal = new Point(x, y);
                }
                else if (targetBoxControl != null)
                {
                    retVal = new Point(targetBoxControl.Center.X, targetBoxControl.Location.Y - DefaultArrowLength);
                }
                else
                {
                    retVal = new Point(50, 50);
                }

                retVal.Offset(Offset); // This offset is used to avoid overlapping of similar arrows
                return retVal;
            }
        }

        /// <summary>
        ///     Provides the target location of the arrow
        /// </summary>
        public Point TargetLocation
        {
            get
            {
                Point retVal;

                BoxControl<TEnclosing, TBoxModel, TArrowModel> initialBoxControl = SourceBoxControl;
                BoxControl<TEnclosing, TBoxModel, TArrowModel> targetBoxControl = TargetBoxControl;

                if (targetBoxControl != null)
                {
                    Point center = targetBoxControl.Center;
                    double angle = Math.PI + Angle;

                    int x = center.X + (int) (Math.Cos(angle)*targetBoxControl.Width/2);
                    int y = center.Y + (int) (Math.Sin(angle)*targetBoxControl.Height/2);

                    if (initialBoxControl != null)
                    {
                        Span xIntersection = Span.Intersection(initialBoxControl.XSpan, targetBoxControl.XSpan);
                        if (xIntersection != null)
                        {
                            x = xIntersection.Center +
                                Math.Max(initialBoxControl.Location.X, targetBoxControl.Location.X);
                        }

                        Span yIntersection = Span.Intersection(initialBoxControl.YSpan, targetBoxControl.YSpan);
                        if (yIntersection != null)
                        {
                            y = yIntersection.Center +
                                Math.Max(initialBoxControl.Location.Y, targetBoxControl.Location.Y);
                        }
                    }

                    retVal = new Point(x, y);
                }
                else if (initialBoxControl != null)
                {
                    retVal = new Point(initialBoxControl.Center.X,
                        initialBoxControl.Location.Y + initialBoxControl.Height + DefaultArrowLength);
                }
                else
                {
                    retVal = new Point(50, 50 + DefaultArrowLength);
                }

                retVal.Offset(EndOffset); // This offset is used to have final arrows unaligned
                retVal.Offset(Offset); // This offset is used to avoid overlapping of similar arrows
                return retVal;
            }
        }

        /// <summary>
        ///     Sets the label color
        /// </summary>
        /// <param name="color"></param>
        private void SetColor(Color color)
        {
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (ForeColor != color)
            {
                ForeColor = color;
            }
        }

        /// <summary>
        ///     A normal pen
        /// </summary>
        public Color NormalColor;

        public Pen NormalPen;

        /// <summary>
        ///     A degraded case pen
        /// </summary>
        public Color DeducedCaseColor;

        public Pen DeducedCasePen;

        /// <summary>
        ///     A pen indicating that the arrow is disabled
        /// </summary>
        public Color DisabledColor;

        public Pen DisabledPen;

        /// <summary>
        ///     A activated pen
        /// </summary>
        public Color ActivatedColor;

        public Pen ActivatedPen;

        /// <summary>
        ///     An external box
        /// </summary>
        public Color ExternalBoxColor;

        public Pen ExternalBoxPen;

        /// <summary>
        ///     Indicates that the arrow should be displayed in the DISABLED color
        /// </summary>
        /// <returns></returns>
        public virtual bool IsDisabled()
        {
            return false;
        }

        /// <summary>
        ///     Indicates that the arrow should be displayed in the DEDUCED color
        /// </summary>
        /// <returns></returns>
        public virtual bool IsDeduced()
        {
            return false;
        }

        /// <summary>
        ///     Indicates that the arrow should be displayed in the ACTIVE color
        /// </summary>
        /// <returns></returns>
        public virtual bool IsActive()
        {
            return false;
        }

        /// <summary>
        ///     Draws the arrow within the box-arrow panel
        /// </summary>
        /// <param name="g"></param>
        public void PaintInBoxArrowPanel(Graphics g)
        {
            if (Visible)
            {
                double angle = Angle;
                Point start = StartLocation;
                Point target = TargetLocation;

                // Select the pen used to draw the arrow
                Pen pen;
                if (IsDisabled())
                {
                    pen = DisabledPen;
                    SetColor(DisabledColor);
                }
                else if (IsActive())
                {
                    pen = ActivatedPen;
                    SetColor(ActivatedColor);
                }
                else if (IsDeduced())
                {
                    // A degraded case is a arrow that is not defined in any state machine
                    pen = DeducedCasePen;
                    SetColor(DeducedCaseColor);
                }
                else
                {
                    pen = NormalPen;
                    SetColor(NormalColor);
                }

                if (Panel.IsSelected(this))
                {
                    // Change the pen when the arrow is selected
                    pen = new Pen(pen.Color, 4);
                }

                // Draw the arrow
                g.DrawLine(pen, start, target);

                // Draw the arrow tip
                switch (ArrowFill)
                {
                    case ArrowFillEnum.Line:
                        if (ArrowMode == ArrowModeEnum.Full || ArrowMode == ArrowModeEnum.Half)
                        {
                            int x = target.X - (int) (Math.Cos(angle + ArrowAngle)*ArrowLength);
                            int y = target.Y - (int) (Math.Sin(angle + ArrowAngle)*ArrowLength);
                            g.DrawLine(pen, target, new Point(x, y));
                        }
                        if (ArrowMode == ArrowModeEnum.Full)
                        {
                            int x = target.X - (int) (Math.Cos(angle - ArrowAngle)*ArrowLength);
                            int y = target.Y - (int) (Math.Sin(angle - ArrowAngle)*ArrowLength);
                            g.DrawLine(pen, target, new Point(x, y));
                        }
                        break;

                    case ArrowFillEnum.Fill:
                        Brush brush = new SolidBrush(pen.Color);
                        int x1 = target.X - (int) (Math.Cos(angle)*ArrowLength);
                        int y1 = target.Y - (int) (Math.Sin(angle)*ArrowLength);

                        if (ArrowMode == ArrowModeEnum.Full || ArrowMode == ArrowModeEnum.Half)
                        {
                            int x2 = target.X - (int) (Math.Cos(angle + ArrowAngle)*ArrowLength);
                            int y2 = target.Y - (int) (Math.Sin(angle + ArrowAngle)*ArrowLength);

                            Point[] points = {target, new Point(x1, y1), new Point(x2, y2)};
                            g.FillPolygon(brush, points);
                        }
                        if (ArrowMode == ArrowModeEnum.Full)
                        {
                            int x2 = target.X - (int) (Math.Cos(angle - ArrowAngle)*ArrowLength);
                            int y2 = target.Y - (int) (Math.Sin(angle - ArrowAngle)*ArrowLength);

                            Point[] points = {target, new Point(x1, y1), new Point(x2, y2)};
                            g.FillPolygon(brush, points);
                        }
                        break;
                }

                if (TargetBoxControl == null)
                {
                    Font boldFont = new Font(Font, FontStyle.Bold);
                    string targetStateName = GetTargetName();

                    SizeF size = g.MeasureString(targetStateName, boldFont);
                    int x = target.X - (int) (size.Width/2);
                    int y = target.Y + 10;
                    g.DrawString(targetStateName, boldFont, ExternalBoxPen.Brush, new Point(x, y));
                }
            }
        }

        /// <summary>
        ///     Provides the name of the target state
        /// </summary>
        /// <returns></returns>
        public virtual string GetTargetName()
        {
            string retVal = "<unknown>";

            if (Model.Target != null)
            {
                retVal = Model.Target.Name;
            }

            return retVal;
        }

        /// <summary>
        ///     Sets the initial box of the arrow controlled by this arrow control
        /// </summary>
        /// <param name="box"></param>
        public void SetInitialBox(TBoxModel box)
        {
            Model.SetInitialBox(box);
            RefreshControl();
        }

        /// <summary>
        ///     Sets the target box of the arrow controlled by this arrow control
        /// </summary>
        /// <param name="box"></param>
        public void SetTargetBox(TBoxModel box)
        {
            Model.SetTargetBox(box);
            RefreshControl();
        }

        /// <summary>
        ///     The offset to apply to the start location & end location before painting the arrow
        /// </summary>
        public Point Offset { get; set; }

        /// <summary>
        ///     The offset to be applied to the end arrow
        /// </summary>
        public Point EndOffset { get; set; }

        /// <summary>
        ///     Provides the center of the arrow
        /// </summary>
        /// <returns></returns>
        public Point GetCenter()
        {
            // Set the start & end location of the arrow
            Point startLocation = StartLocation;
            Point targetLocation = TargetLocation;

            // Set the location of the text
            Span xSpan = new Span(startLocation.X, targetLocation.X);
            Span ySpan = new Span(startLocation.Y, targetLocation.Y);

            int x = Math.Min(startLocation.X, targetLocation.X) + xSpan.Center;
            int y = Math.Min(startLocation.Y, targetLocation.Y) + ySpan.Center;

            return new Point(x, y);
        }

        /// <summary>
        ///     Provides the text bounding box, according to the center point provided.
        ///     The text bounding box for initial arrows is above that arrow
        /// </summary>
        /// <param name="center">The center of the box</param>
        /// <returns></returns>
        public Rectangle GetTextBoundingBox(Point center)
        {
            int x = center.X - Width/2;
            int y = center.Y - Height/2;

            // Position of the text box for initial arrows
            if (SourceBoxControl == null)
            {
                y = y - DefaultArrowLength/2;
            }

            return new Rectangle(x, y, Width, Height);
        }

        /// <summary>
        ///     The delta applied when sliding the arrow
        /// </summary>
        private const int Delta = 5;

        /// <summary>
        ///     Direction of the slide
        /// </summary>
        public enum SlideDirection
        {
            Up,
            Down
        };

        /// <summary>
        ///     Slides the arrow following the arrow
        ///     to avoid colliding with the colliding rectangle
        /// </summary>
        /// <param name="center">The current center of the text box</param>
        /// <param name="colliding">The colliding rectangle</param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Point Slide(Point center, Rectangle colliding, SlideDirection direction)
        {
            Point retVal;

            double angle = Angle;
            if (direction == SlideDirection.Up)
            {
                retVal = new Point((int) (center.X + Math.Cos(angle)*Delta), (int) (center.Y + Math.Sin(angle)*Delta));
            }
            else
            {
                retVal = new Point((int) (center.X - Math.Cos(angle)*Delta), (int) (center.Y - Math.Sin(angle)*Delta));
            }

            return retVal;
        }
    }
}