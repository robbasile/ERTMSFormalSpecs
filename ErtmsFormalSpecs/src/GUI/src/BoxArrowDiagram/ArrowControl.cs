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

namespace GUI.BoxArrowDiagram
{
    public abstract partial class ArrowControl<BoxModel, ArrowModel> : Label
        where BoxModel : class, IGraphicalDisplay
        where ArrowModel : class, IGraphicalArrow<BoxModel>
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
        public ArrowControl()
        {
            InitializeComponent();
            InitializeColors();

            ArrowMode = ArrowModeEnum.Full;
            ArrowFill = ArrowFillEnum.Line;
            MouseClick += new MouseEventHandler(MouseClickHandler);
            MouseDoubleClick += new MouseEventHandler(MouseDoubleClickHandler);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public ArrowControl(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            InitializeColors();

            ArrowMode = ArrowModeEnum.Full;
            ArrowFill = ArrowFillEnum.Line;
            MouseClick += new MouseEventHandler(MouseClickHandler);
            MouseDoubleClick += new MouseEventHandler(MouseDoubleClickHandler);
        }

        /// <summary>
        ///     Initializes the colors of the pens
        /// </summary>
        private void InitializeColors()
        {
            NORMAL_COLOR = Color.Black;
            NORMAL_PEN = new Pen(NORMAL_COLOR);

            DEDUCED_CASE_COLOR = Color.MediumPurple;
            DEDUCED_CASE_PEN = new Pen(DEDUCED_CASE_COLOR);

            DISABLED_COLOR = Color.Red;
            DISABLED_PEN = new Pen(DISABLED_COLOR);

            ACTIVATED_COLOR = Color.Blue;
            ACTIVATED_PEN = new Pen(ACTIVATED_COLOR, 4);

            EXTERNAL_BOX_COLOR = Color.Green;
            EXTERNAL_BOX_PEN = new Pen(EXTERNAL_BOX_COLOR, 2);
        }

        /// <summary>
        ///     Handles a mouse click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseClickHandler(object sender, MouseEventArgs e)
        {
            SelectArrow();
        }

        /// <summary>
        ///     Handles a mouse click event on an arrow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseDoubleClickHandler(object sender, MouseEventArgs e)
        {
            SelectArrow();
        }

        /// <summary>
        ///     The parent box-arrow panel
        /// </summary>
        public BoxArrowPanel<BoxModel, ArrowModel> BoxArrowPanel
        {
            get { return GUIUtils.EnclosingFinder<BoxArrowPanel<BoxModel, ArrowModel>>.find(this); }
        }

        /// <summary>
        ///     Provides the enclosing form
        /// </summary>
        public Form EnclosingForm
        {
            get { return GUIUtils.EnclosingFinder<Form>.find(this); }
        }

        /// <summary>
        ///     Selects the current arrow
        /// </summary>
        public void SelectArrow()
        {
            BoxArrowPanel.Select(this, ModifierKeys == Keys.Control);
        }

        /// <summary>
        ///     Provides the enclosing box-arrow diagram panel
        /// </summary>
        public BoxArrowPanel<BoxModel, ArrowModel> Panel
        {
            get { return GUIUtils.EnclosingFinder<BoxArrowPanel<BoxModel, ArrowModel>>.find(this); }
        }

        /// <summary>
        ///     The Model
        /// </summary>
        private ArrowModel __model;

        public virtual ArrowModel Model
        {
            get { return __model; }
            set
            {
                __model = value;
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
        public BoxControl<BoxModel, ArrowModel> SourceBoxControl
        {
            get
            {
                BoxControl<BoxModel, ArrowModel> retVal = null;

                if (Model.Source != null)
                {
                    retVal = Panel.getBoxControl(Model.Source);
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the box control which corresponds to the target state
        /// </summary>
        public BoxControl<BoxModel, ArrowModel> TargetBoxControl
        {
            get
            {
                BoxControl<BoxModel, ArrowModel> retVal = null;

                retVal = Panel.getBoxControl(Model.Target);

                return retVal;
            }
        }

        /// <summary>
        ///     The default size of a arrow. This is used when one of the control ending the arrow does not belong to the diagram
        /// </summary>
        public int DEFAULT_ARROW_LENGTH = 40;

        private static double ARROW_LENGTH = 10.0;
        private static double ARROW_ANGLE = Math.PI/6;

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
        ///     Indicates whether the angle is nearly vertical
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private bool aroundVertical(double angle)
        {
            // Ensure the angle is in the first or second quadrant 
            while (angle < 0)
            {
                angle = angle + 2*Math.PI;
            }
            while (angle > Math.PI)
            {
                angle = angle - Math.PI;
            }

            return (angle > 3*Math.PI/8) && (angle < 5*Math.PI/8);
        }

        /// <summary>
        ///     Provides the start location of the arrow
        /// </summary>
        public Point StartLocation
        {
            get
            {
                Point retVal;

                int x;
                int y;

                BoxControl<BoxModel, ArrowModel> initialBoxControl = SourceBoxControl;
                BoxControl<BoxModel, ArrowModel> targetBoxControl = TargetBoxControl;

                if (initialBoxControl != null)
                {
                    Point center = initialBoxControl.Center;
                    double angle = Angle;

                    x = center.X + (int) (Math.Cos(angle)*initialBoxControl.Width/2);
                    y = center.Y + (int) (Math.Sin(angle)*initialBoxControl.Height/2);

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
                    retVal = new Point(targetBoxControl.Center.X, targetBoxControl.Location.Y - DEFAULT_ARROW_LENGTH);
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

                int x;
                int y;

                BoxControl<BoxModel, ArrowModel> initialBoxControl = SourceBoxControl;
                BoxControl<BoxModel, ArrowModel> targetBoxControl = TargetBoxControl;

                if (targetBoxControl != null)
                {
                    Point center = targetBoxControl.Center;
                    double angle = Math.PI + Angle;

                    x = center.X + (int) (Math.Cos(angle)*targetBoxControl.Width/2);
                    y = center.Y + (int) (Math.Sin(angle)*targetBoxControl.Height/2);

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
                        initialBoxControl.Location.Y + initialBoxControl.Height + DEFAULT_ARROW_LENGTH);
                }
                else
                {
                    retVal = new Point(50, 50 + DEFAULT_ARROW_LENGTH);
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
            if (ForeColor != color)
            {
                ForeColor = color;
            }
        }

        /// <summary>
        ///     A normal pen
        /// </summary>
        public Color NORMAL_COLOR;

        public Pen NORMAL_PEN;

        /// <summary>
        ///     A degraded case pen
        /// </summary>
        public Color DEDUCED_CASE_COLOR;

        public Pen DEDUCED_CASE_PEN;

        /// <summary>
        ///     A pen indicating that the arrow is disabled
        /// </summary>
        public Color DISABLED_COLOR;

        public Pen DISABLED_PEN;

        /// <summary>
        ///     A activated pen
        /// </summary>
        public Color ACTIVATED_COLOR;

        public Pen ACTIVATED_PEN;

        /// <summary>
        ///     An external box
        /// </summary>
        public Color EXTERNAL_BOX_COLOR;

        public Pen EXTERNAL_BOX_PEN;

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
        /// <param name="e"></param>
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
                    pen = DISABLED_PEN;
                    SetColor(DISABLED_COLOR);
                }
                else if (IsActive())
                {
                    pen = ACTIVATED_PEN;
                    SetColor(ACTIVATED_COLOR);
                }
                else if (IsDeduced())
                {
                    // A degraded case is a arrow that is not defined in any state machine
                    pen = DEDUCED_CASE_PEN;
                    SetColor(DEDUCED_CASE_COLOR);
                }
                else
                {
                    pen = NORMAL_PEN;
                    SetColor(NORMAL_COLOR);
                }

                if (Panel.isSelected(this))
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
                            int x = target.X - (int) (Math.Cos(angle + ARROW_ANGLE)*ARROW_LENGTH);
                            int y = target.Y - (int) (Math.Sin(angle + ARROW_ANGLE)*ARROW_LENGTH);
                            g.DrawLine(pen, target, new Point(x, y));
                        }
                        if (ArrowMode == ArrowModeEnum.Full)
                        {
                            int x = target.X - (int) (Math.Cos(angle - ARROW_ANGLE)*ARROW_LENGTH);
                            int y = target.Y - (int) (Math.Sin(angle - ARROW_ANGLE)*ARROW_LENGTH);
                            g.DrawLine(pen, target, new Point(x, y));
                        }
                        break;

                    case ArrowFillEnum.Fill:
                        Brush brush = new SolidBrush(pen.Color);
                        int x1 = target.X - (int) (Math.Cos(angle)*ARROW_LENGTH);
                        int y1 = target.Y - (int) (Math.Sin(angle)*ARROW_LENGTH);

                        if (ArrowMode == ArrowModeEnum.Full || ArrowMode == ArrowModeEnum.Half)
                        {
                            int x2 = target.X - (int) (Math.Cos(angle + ARROW_ANGLE)*ARROW_LENGTH);
                            int y2 = target.Y - (int) (Math.Sin(angle + ARROW_ANGLE)*ARROW_LENGTH);

                            Point[] points = new Point[] {target, new Point(x1, y1), new Point(x2, y2)};
                            g.FillPolygon(brush, points);
                        }
                        if (ArrowMode == ArrowModeEnum.Full)
                        {
                            int x2 = target.X - (int) (Math.Cos(angle - ARROW_ANGLE)*ARROW_LENGTH);
                            int y2 = target.Y - (int) (Math.Sin(angle - ARROW_ANGLE)*ARROW_LENGTH);

                            Point[] points = new Point[] {target, new Point(x1, y1), new Point(x2, y2)};
                            g.FillPolygon(brush, points);
                        }
                        break;
                }

                if (TargetBoxControl == null)
                {
                    Font boldFont = new Font(Font, FontStyle.Bold);
                    string targetStateName = getTargetName();

                    SizeF size = g.MeasureString(targetStateName, boldFont);
                    int x = target.X - (int) (size.Width/2);
                    int y = target.Y + 10;
                    g.DrawString(targetStateName, boldFont, EXTERNAL_BOX_PEN.Brush, new Point(x, y));
                }
            }
        }

        /// <summary>
        ///     Provides the name of the target state
        /// </summary>
        /// <returns></returns>
        public virtual string getTargetName()
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
        public void SetInitialBox(BoxModel box)
        {
            Model.SetInitialBox(box);
            RefreshControl();
        }

        /// <summary>
        ///     Sets the target box of the arrow controlled by this arrow control
        /// </summary>
        /// <param name="box"></param>
        public void SetTargetBox(BoxModel box)
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
        public Point getCenter()
        {
            // Set the start & end location of the arrow
            Point startLocation = StartLocation;
            Point targetLocation = TargetLocation;

            // Set the location of the text
            Span Xspan = new Span(startLocation.X, targetLocation.X);
            Span Yspan = new Span(startLocation.Y, targetLocation.Y);

            int x = Math.Min(startLocation.X, targetLocation.X) + Xspan.Center;
            int y = Math.Min(startLocation.Y, targetLocation.Y) + Yspan.Center;

            return new Point(x, y);
        }

        /// <summary>
        ///     Provides the text bounding box, according to the center point provided.
        ///     The text bounding box for initial arrows is above that arrow
        /// </summary>
        /// <param name="center">The center of the box</param>
        /// <returns></returns>
        public Rectangle getTextBoundingBox(Point center)
        {
            int x = center.X - Width/2;
            int y = center.Y - Height/2;

            // Position of the text box for initial arrows
            if (SourceBoxControl == null)
            {
                y = y - DEFAULT_ARROW_LENGTH/2;
            }

            return new Rectangle(x, y, Width, Height);
        }

        /// <summary>
        ///     The delta applied when sliding the arrow
        /// </summary>
        private const int DELTA = 5;

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
        /// <returns></returns>
        public Point Slide(Point center, Rectangle colliding, SlideDirection direction)
        {
            Point retVal;

            double angle = Angle;
            if (direction == SlideDirection.Up)
            {
                retVal = new Point((int) (center.X + Math.Cos(angle)*DELTA), (int) (center.Y + Math.Sin(angle)*DELTA));
            }
            else
            {
                retVal = new Point((int) (center.X - Math.Cos(angle)*DELTA), (int) (center.Y - Math.Sin(angle)*DELTA));
            }

            return retVal;
        }
    }
}