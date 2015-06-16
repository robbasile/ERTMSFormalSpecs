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
using ModelElement = Utils.ModelElement;

namespace GUI.BoxArrowDiagram
{
    public partial class BoxControl<BoxModel, ArrowModel> : Label
        where BoxModel : class, IGraphicalDisplay
        where ArrowModel : class, IGraphicalArrow<BoxModel>
    {
        /// <summary>
        ///     The mode of displaying boxes
        /// </summary>
        protected enum BoxModeEnum
        {
            Custom,
            Rectangle3D,
            Rectangle,
            RoundedCorners
        };

        /// <summary>
        ///     The mode of displaying boxes
        /// </summary>
        protected BoxModeEnum BoxMode { get; set; }

        /// <summary>
        ///     The grid size used to place boxes
        /// </summary>
        public int GRID_SIZE = 10;

        /// <summary>
        ///     Provides the enclosing box-arrow panel
        /// </summary>
        public BoxArrowPanel<BoxModel, ArrowModel> Panel
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
        ///     The model for this control
        /// </summary>
        private BoxModel __model;

        public virtual BoxModel Model
        {
            get { return __model; }
            set { __model = value; }
        }

        /// <summary>
        ///     Refreshes the control according to the related model
        /// </summary>
        public void RefreshControl()
        {
            Util.DontNotify(() =>
            {
                if (Model.Width == 0 || Model.Height == 0)
                {
                    Size boxSize = Panel.DefaultBoxSize;
                    Model.Width = boxSize.Width;
                    Model.Height = boxSize.Height;

                    Point p = Panel.GetNextPosition(Model);
                    Model.X = p.X;
                    Model.Y = p.Y;
                }
                Size = new Size(Model.Width, Model.Height);
                SetPosition(Model.X, Model.Y);

                TextAlign = ContentAlignment.MiddleCenter;
                if (Model.Hidden)
                {
                    Text = Model.GraphicalName + "\n(Hidden)";
                    Font = new Font(Font, FontStyle.Italic);
                    ForeColor = Color.Gray;
                }
                else
                {
                    Text = Model.GraphicalName;
                    Font = new Font(Font, FontStyle.Regular);
                    ForeColor = Color.Black;
                }
            });
        }

        /// <summary>
        ///     Sets the color of the control
        /// </summary>
        /// <param name="color"></param>
        protected void SetColor(Color color)
        {
            if (BoxMode == BoxModeEnum.RoundedCorners
                || BoxMode == BoxModeEnum.Rectangle
                || BoxMode == BoxModeEnum.Custom)
            {
                // The background color is handled manually
                color = Color.Transparent;
            }

            if (color != BackColor)
            {
                BackColor = color;
            }
        }

        /// <summary>
        ///     A normal pen
        /// </summary>
        public Color NORMAL_COLOR = Color.LightGray;

        public Pen NORMAL_PEN = new Pen(Color.Black);

        /// <summary>
        ///     A normal pen
        /// </summary>
        public Color HIDDEN_COLOR = Color.Transparent;

        public Pen HIDDEN_PEN = new Pen(Color.Gray);

        /// <summary>
        ///     A activated pen
        /// </summary>
        public Color ACTIVATED_COLOR = Color.Blue;

        public Pen ACTIVATED_PEN = new Pen(Color.Black, 4);

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
            return Model.Hidden;
        }

        /// <summary>
        ///     The size of a round corner
        /// </summary>
        private int ROUND_SIZE = 10;

        /// <summary>
        ///     Draws the box within the box-arrow panel
        /// </summary>
        /// <param name="e"></param>
        public virtual void PaintInBoxArrowPanel(Graphics g)
        {
            // Select the right pen, according to the model
            Pen pen;
            if (IsActive())
            {
                pen = ACTIVATED_PEN;
                SetColor(ACTIVATED_COLOR);
            }
            else if (IsHidden())
            {
                pen = HIDDEN_PEN;
                SetColor(HIDDEN_COLOR);
            }
            else
            {
                pen = NORMAL_PEN;
                SetColor(NORMAL_COLOR);
            }

            // Draw the box
            switch (BoxMode)
            {
                case BoxModeEnum.Rectangle3D:
                    g.DrawRectangle(pen, Location.X, Location.Y, Width, Height);
                    break;

                case BoxModeEnum.Rectangle:
                {
                    Brush innerBrush = new SolidBrush(NORMAL_COLOR);
                    g.FillRectangle(innerBrush, Location.X, Location.Y, Width, Height);
                    break;
                }

                case BoxModeEnum.RoundedCorners:
                {
                    Point[] points = new Point[]
                    {
                        new Point(Location.X + ROUND_SIZE, Location.Y),
                        new Point(Location.X + Width - ROUND_SIZE, Location.Y),
                        new Point(Location.X + Width, Location.Y + ROUND_SIZE),
                        new Point(Location.X + Width, Location.Y + Height - ROUND_SIZE),
                        new Point(Location.X + Width - ROUND_SIZE, Location.Y + Height),
                        new Point(Location.X + ROUND_SIZE, Location.Y + Height),
                        new Point(Location.X, Location.Y + Height - ROUND_SIZE),
                        new Point(Location.X, Location.Y + ROUND_SIZE),
                    };

                    Brush innerBrush = new SolidBrush(NORMAL_COLOR);
                    g.FillRectangle(innerBrush,
                        new Rectangle(points[0], new Size(points[4].X - points[0].X, points[4].Y - points[0].Y)));
                    g.FillRectangle(innerBrush,
                        new Rectangle(points[7], new Size(points[3].X - points[7].X, points[3].Y - points[7].Y)));

                    g.DrawLine(pen, points[0], points[1]);
                    g.DrawLine(pen, points[2], points[3]);
                    g.DrawLine(pen, points[4], points[5]);
                    g.DrawLine(pen, points[6], points[7]);

                    Size rectangleSize = new Size(2*ROUND_SIZE, 2*ROUND_SIZE);
                    Rectangle rectangle;
                    rectangle = new Rectangle(new Point(points[0].X - ROUND_SIZE, points[0].Y), rectangleSize);
                    g.FillPie(innerBrush, rectangle, 180.0f, 90.0f);
                    g.DrawArc(pen, rectangle, 180.0f, 90.0f);

                    rectangle = new Rectangle(new Point(points[2].X - 2*ROUND_SIZE, points[2].Y - ROUND_SIZE),
                        rectangleSize);
                    g.FillPie(innerBrush, rectangle, 270.0f, 90.0f);
                    g.DrawArc(pen, rectangle, 270.0f, 90.0f);

                    rectangle = new Rectangle(new Point(points[4].X - ROUND_SIZE, points[4].Y - 2*ROUND_SIZE),
                        rectangleSize);
                    g.FillPie(innerBrush, rectangle, 0.0f, 90.0f);
                    g.DrawArc(pen, rectangle, 0.0f, 90.0f);

                    rectangle = new Rectangle(new Point(points[6].X, points[6].Y - ROUND_SIZE), rectangleSize);
                    g.FillPie(innerBrush, rectangle, 90.0f, 90.0f);
                    g.DrawArc(pen, rectangle, 90.0f, 90.0f);
                    break;
                }
            }

            // Pinned or not
            Image image;
            if (Model.Pinned)
            {
                image = Panel.Images.Images[BoxArrowPanel<BoxModel, ArrowModel>.PinnedImageIndex];
            }
            else
            {
                image = Panel.Images.Images[BoxArrowPanel<BoxModel, ArrowModel>.UnPinnedImageIndex];
            }
            g.DrawImage(image, Location.X + Width - 16, Location.Y, 16, 16);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public BoxControl()
        {
            InitializeComponent();

            BoxMode = BoxModeEnum.Rectangle3D;
            MouseDown += new MouseEventHandler(HandleMouseDown);
            MouseUp += new MouseEventHandler(HandleMouseUp);
            MouseMove += new MouseEventHandler(HandleMouseMove);
            MouseClick += new MouseEventHandler(HandleMouseClick);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public BoxControl(IContainer container)
        {
            container.Add(this);

            BoxMode = BoxModeEnum.Rectangle3D;
            InitializeComponent();
            MouseDown += new MouseEventHandler(HandleMouseDown);
            MouseUp += new MouseEventHandler(HandleMouseUp);
            MouseMove += new MouseEventHandler(HandleMouseMove);
            MouseClick += new MouseEventHandler(HandleMouseClick);
        }

        /// <summary>
        ///     Selects the current box
        /// </summary>
        public virtual void SelectBox()
        {
            Panel.Select(this, ModifierKeys == Keys.Control);
        }

        /// <summary>
        ///     The location where the mouse down occured
        /// </summary>
        private Point moveStartLocation;

        /// <summary>
        ///     The control location where the mouse down occured
        /// </summary>
        private Point positionBeforeMove;

        /// <summary>
        ///     In a move operation ?
        /// </summary>
        private bool moving = false;

        /// <summary>
        ///     Handles a mouse down event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            moving = true;
            moveStartLocation = e.Location;
            positionBeforeMove = new Point(Model.X, Model.Y);
        }

        /// <summary>
        ///     Handles a mouse up event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
            if (Model.X != positionBeforeMove.X || Model.Y != positionBeforeMove.Y)
            {
                Panel.ControlHasMoved();
                Panel.Refresh();
            }
        }

        /// <summary>
        ///     Handles a mouse move event, when
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                Point mouseMoveLocation = e.Location;

                int deltaX = mouseMoveLocation.X - moveStartLocation.X;
                int deltaY = mouseMoveLocation.Y - moveStartLocation.Y;

                if (Math.Abs(deltaX) > 5 || Math.Abs(deltaY) > 5)
                {
                    int newX = Model.X + deltaX;
                    int newY = Model.Y + deltaY;
                    if (Panel.Location.X <= newX && Panel.Location.Y <= newY)
                    {
                        SetPosition(newX, newY);
                    }
                }
            }
        }

        /// <summary>
        ///     Sets the position of the control, according to the X & Y provided
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetPosition(int x, int y)
        {
            int posX = (x)/GRID_SIZE;
            posX = posX*GRID_SIZE;

            int posY = (y)/GRID_SIZE;
            posY = posY*GRID_SIZE;

            Model.X = posX;
            Model.Y = posY;

            Location = new Point(Model.X, Model.Y);
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
        ///     Handles a mouse click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.X >= Width - 18 && e.Y <= 18)
                {
                    Model.Pinned = !Model.Pinned;
                    Refresh();
                }
                else
                {
                    SelectBox();
                }
            }
            else
            {
                BaseForm baseForm = EnclosingForm as BaseForm;
                if (baseForm != null && baseForm.TreeView != null)
                {
                    BaseTreeNode node = baseForm.TreeView.FindNode(Model);
                    if (node != null)
                    {
                        ContextMenu menu = node.ContextMenu;
                        menu.Show(this, e.Location);
                    }
                }
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