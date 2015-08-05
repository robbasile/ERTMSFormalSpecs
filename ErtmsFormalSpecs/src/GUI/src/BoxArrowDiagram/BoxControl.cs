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
using DataDictionary.Generated;
using GUI.Properties;
using ModelElement = Utils.ModelElement;

namespace GUI.BoxArrowDiagram
{
    public partial class BoxControl<TEnclosing, TBoxModel, TArrowModel> : Label
        where TEnclosing : class
        where TBoxModel : class, IGraphicalDisplay
        where TArrowModel : class, IGraphicalArrow<TBoxModel>
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
        public int GridSize = 10;

        /// <summary>
        ///     Provides the enclosing box-arrow panel
        /// </summary>
        public BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel> Panel
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
        ///     The model for this control
        /// </summary>
        public virtual TBoxModel Model { get; set; }

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
                    Text = Model.GraphicalName + Resources.BoxControl_RefreshControl_;
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
            return Model.Hidden;
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
            Pen pen;
            if (IsActive())
            {
                pen = ActivatedPen;
                SetColor(ActivatedColor);
            }
            else if (IsHidden())
            {
                pen = HiddenPen;
                SetColor(HiddenColor);
            }
            else
            {
                pen = NormalPen;
                SetColor(NormalColor);
            }

            // Draw the box
            switch (BoxMode)
            {
                case BoxModeEnum.Rectangle3D:
                    g.DrawRectangle(pen, Location.X, Location.Y, Width, Height);
                    break;

                case BoxModeEnum.Rectangle:
                {
                    Brush innerBrush = new SolidBrush(NormalColor);
                    g.FillRectangle(innerBrush, Location.X, Location.Y, Width, Height);
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

            // Pinned or not
            Image image;
            if (Model.Pinned)
            {
                image = Panel.Images.Images[BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel>.PinnedImageIndex];
            }
            else
            {
                image = Panel.Images.Images[BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel>.UnPinnedImageIndex];
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
            MouseDown += HandleMouseDown;
            MouseUp += HandleMouseUp;
            MouseMove += HandleMouseMove;
            MouseClick += HandleMouseClick;
            DoubleClick += HandleMouseDoubleClick;
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
            MouseDown += HandleMouseDown;
            MouseUp += HandleMouseUp;
            MouseMove += HandleMouseMove;
            MouseClick += HandleMouseClick;
        }

        /// <summary>
        ///     Selects the current box
        /// </summary>
        /// <param name="mouseEventArgs"></param>
        public virtual void SelectBox(MouseEventArgs mouseEventArgs)
        {
            Context.SelectionCriteria criteria = GuiUtils.SelectionCriteriaBasedOnMouseEvent(mouseEventArgs);
            EFSSystem.INSTANCE.Context.SelectElement(Model, Panel, criteria);
        }

        /// <summary>
        ///     The location where the mouse down occured
        /// </summary>
        private Point _moveStartLocation;

        /// <summary>
        ///     The control location where the mouse down occured
        /// </summary>
        private Point _positionBeforeMove;

        /// <summary>
        ///     In a move operation ?
        /// </summary>
        private bool _moving;

        /// <summary>
        ///     Handles a mouse down event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            _moving = true;
            _moveStartLocation = e.Location;
            _positionBeforeMove = new Point(Model.X, Model.Y);
        }

        /// <summary>
        ///     Handles a mouse up event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            _moving = false;
            if (Model.X != _positionBeforeMove.X || Model.Y != _positionBeforeMove.Y)
            {
                EFSSystem.INSTANCE.Context.HandleChangeEvent(Model as BaseModelElement);
            }
        }

        /// <summary>
        ///     Handles a mouse move event, when
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (_moving)
            {
                Point mouseMoveLocation = e.Location;

                int deltaX = mouseMoveLocation.X - _moveStartLocation.X;
                int deltaY = mouseMoveLocation.Y - _moveStartLocation.Y;

                if (Math.Abs(deltaX) > 5 || Math.Abs(deltaY) > 5)
                {
                    int newX = Model.X + deltaX;
                    int newY = Model.Y + deltaY;
                    if (Panel != null)
                    {
                        if (Panel.Location.X <= newX && Panel.Location.Y <= newY)
                        {
                            SetPosition(newX, newY);
                        }
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
            Util.DontNotify(() =>
            {
                int posX = (x) / GridSize;
                posX = posX * GridSize;

                int posY = (y) / GridSize;
                posY = posY * GridSize;

                Model.X = posX;
                Model.Y = posY;

                Location = new Point(Model.X, Model.Y);                
            });
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
        /// <param name="mouseEventArgs"></param>
        private void HandleMouseClick(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                if (mouseEventArgs.X >= Width - 18 && mouseEventArgs.Y <= 18)
                {
                    Model.Pinned = !Model.Pinned;
                    Refresh();
                }
                else
                {
                    Context.SelectionCriteria criteria = GuiUtils.SelectionCriteriaBasedOnMouseEvent(mouseEventArgs);
                    EFSSystem.INSTANCE.Context.SelectElement(Model, Panel, criteria);
                }
            }
            else if ( mouseEventArgs.Button == MouseButtons.Right )            
            {
                // Show the context menu, according to the tree view of the base form
                BaseForm baseForm = EnclosingForm as BaseForm;
                if (baseForm != null && baseForm.TreeView != null)
                {
                    BaseTreeNode node = baseForm.TreeView.FindNode(Model, true);
                    if (node != null)
                    {
                        ContextMenu menu = node.ContextMenu;
                        menu.Show(this, mouseEventArgs.Location);
                    }
                }
            }
        }

        /// <summary>
        /// Handles a double click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDoubleClick(object sender, EventArgs e)
        {
            Context.SelectionCriteria criteria = GuiUtils.SelectionCriteriaBasedOnMouseEvent(e as MouseEventArgs);
            EFSSystem.INSTANCE.Context.SelectElement(Model, Panel, criteria);
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