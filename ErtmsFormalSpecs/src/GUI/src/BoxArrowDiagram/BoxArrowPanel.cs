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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DataDictionary;

namespace GUI.BoxArrowDiagram
{
    public abstract partial class BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel> : Panel
        where TEnclosing : class
        where TBoxModel : class, IGraphicalDisplay
        where TArrowModel : class, IGraphicalArrow<TBoxModel>
    {
        private ToolStripMenuItem _refreshMenuItem;

        /// <summary>
        ///     Initializes the context menu items
        /// </summary>
        public virtual void InitializeStartMenu()
        {
            // 
            // Refresh
            // 
            _refreshMenuItem = new ToolStripMenuItem
            {
                Name = "refreshMenuItem",
                Size = new Size(161, 22),
                Text = @"Refresh"
            };
            _refreshMenuItem.Click += refreshMenuItem_Click;

            contextMenu.Items.Clear();
            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                _refreshMenuItem
            });
            contextMenu.Opening += contextMenu_Opening;
        }

        private void contextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Point location = PointToClient(Cursor.Position);

            object target = BoxForLocation(location);
            if (target == null)
            {
                target = ArrowForLocation(location);
            }

            if (target != null)
            {
                Selected = target;
            }
        }

        /// <summary>
        ///     The images used by this time line control
        /// </summary>
        public ImageList Images { get; set; }

        /// <summary>
        ///     The image indexes used to retrieve images
        /// </summary>
        public const int PinnedImageIndex = 0;

        public const int UnPinnedImageIndex = 1;

        /// <summary>
        ///     The size of an box control button
        /// </summary>
        public Size DefaultBoxSize = new Size(100, 50);

        /// <summary>
        ///     The model
        /// </summary>
        private TEnclosing _model;

        /// <summary>
        ///     The model element for which this panel is built
        /// </summary>
        public virtual TEnclosing Model
        {
            get { return _model; }
            set
            {
                _model = value;
                InitPositionHandling();
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected BoxArrowPanel()
        {
            InitializeComponent();
            InitializeStartMenu();

            MouseDown += BoxArrowPanel_MouseDown;
            MouseMove += BoxArrowPanel_MouseMove;
            MouseUp += BoxArrowPanel_MouseUp;
            Click += BoxArrowPanel_Click;

            DragEnter += DragEnterHandler;
            DragDrop += DragDropHandler;
            AllowDrop = true;
            DoubleBuffered = true;

            Images = new ImageList();
            Images.Images.Add(Properties.Resources.pin);
            Images.Images.Add(Properties.Resources.unpin);
        }

        private void BoxArrowPanel_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouseEvent = (MouseEventArgs) e;

            if (mouseEvent.Button == MouseButtons.Right)
            {
                BaseForm baseForm = EnclosingForm as BaseForm;
                if (baseForm != null && baseForm.TreeView != null)
                {
                    BaseTreeNode node = baseForm.TreeView.FindNode(Model as ModelElement, true);
                    ContextMenu menu = node.ContextMenu;
                    menu.Show(this, mouseEvent.Location);
                }
            }
        }

        /// <summary>
        ///     Provides the enclosing form
        /// </summary>
        public Form EnclosingForm
        {
            get
            {
                Form retVal = null;

                Control current = this;
                while (current != null && retVal == null)
                {
                    retVal = current as Form;
                    current = current.Parent;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Called to initiate a drag & drop operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragEnterHandler(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        ///     Called when the drop operation is performed on a node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragDropHandler(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("WindowsForms10PersistentObject", false))
            {
                object data = e.Data.GetData("WindowsForms10PersistentObject");
                BaseTreeNode sourceNode = data as BaseTreeNode;
                if (sourceNode != null)
                {
                    BoxControl<TEnclosing, TBoxModel, TArrowModel> target = null;

                    foreach (BoxControl<TEnclosing, TBoxModel, TArrowModel> box in _boxes.Values)
                    {
                        Rectangle rectangle = box.DisplayRectangle;
                        rectangle.Offset(box.PointToScreen(Location));
                        if (rectangle.Contains(e.X, e.Y))
                        {
                            target = box;
                            break;
                        }
                    }

                    if (target != null)
                    {
                        target.AcceptDrop(sourceNode.Model as ModelElement);
                    }
                }
            }
        }

        /// <summary>
        ///     Refreshes the panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshMenuItem_Click(object sender, EventArgs e)
        {
            RefreshControl();
        }

        /// <summary>
        ///     The selected object
        /// </summary>
        public object Selected { get; set; }

        /// <summary>
        ///     Method used to create a box
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract BoxControl<TEnclosing, TBoxModel, TArrowModel> CreateBox(TBoxModel model);

        /// <summary>
        ///     Method used to create an arrow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract ArrowControl<TEnclosing, TBoxModel, TArrowModel> CreateArrow(TArrowModel model);

        /// <summary>
        ///     The arrow that is currently being changed
        /// </summary>
        private ArrowControl<TEnclosing, TBoxModel, TArrowModel> _changingArrow;

        /// <summary>
        ///     The action that is applied on the arrow
        /// </summary>
        private enum ChangeAction
        {
            None,
            InitialBox,
            TargetBox
        };

        private ChangeAction _chaningArrowAction = ChangeAction.None;

        private void BoxArrowPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clickPoint = new Point(e.X, e.Y);
                foreach (ArrowControl<TEnclosing, TBoxModel, TArrowModel> arrow in _arrows.Values)
                {
                    if (Around(arrow.StartLocation, clickPoint))
                    {
                        _changingArrow = arrow;
                        _changingArrow.Parent = this; // I do not know why...
                        _chaningArrowAction = ChangeAction.InitialBox;
                        break;
                    }
                    if (Around(arrow.TargetLocation, clickPoint))
                    {
                        _changingArrow = arrow;
                        _changingArrow.Parent = this; // I do not know why...
                        _chaningArrowAction = ChangeAction.TargetBox;
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Provides the box at a given location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected BoxControl<TEnclosing, TBoxModel, TArrowModel> BoxForLocation(Point location)
        {
            BoxControl<TEnclosing, TBoxModel, TArrowModel> retVal = null;

            foreach (BoxControl<TEnclosing, TBoxModel, TArrowModel> box in _boxes.Values)
            {
                if ((location.X > box.Location.X && location.X < box.Location.X + box.Width) &&
                    (location.Y > box.Location.Y && location.Y < box.Location.Y + box.Height))
                {
                    retVal = box;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the arrow at a given location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected ArrowControl<TEnclosing, TBoxModel, TArrowModel> ArrowForLocation(Point location)
        {
            ArrowControl<TEnclosing, TBoxModel, TArrowModel> retVal = null;

            foreach (ArrowControl<TEnclosing, TBoxModel, TArrowModel> arrow in _arrows.Values)
            {
                if ((location.X > arrow.Location.X && location.X < arrow.Location.X + arrow.Width) &&
                    (location.Y > arrow.Location.Y && location.Y < arrow.Location.Y + arrow.Height))
                {
                    retVal = arrow;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Handles the move event, which, in case of an arrow is selected to be modified,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoxArrowPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_changingArrow != null && _chaningArrowAction != ChangeAction.None)
            {
                BoxControl<TEnclosing, TBoxModel, TArrowModel> box = BoxForLocation(e.Location);
                if (box != null)
                {
                    switch (_chaningArrowAction)
                    {
                        case ChangeAction.InitialBox:
                            if (_changingArrow.Model.Source != box.Model)
                            {
                                _changingArrow.SetInitialBox(box.Model);
                            }
                            break;
                        case ChangeAction.TargetBox:
                            if (_changingArrow.Model.Target != box.Model)
                            {
                                if (_changingArrow.Model.Source != null)
                                {
                                    _changingArrow.SetTargetBox(box.Model);
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void BoxArrowPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _changingArrow = null;
            _chaningArrowAction = ChangeAction.None;
        }

        /// <summary>
        ///     The maximum delta when considering if two points are near one from the other
        /// </summary>
        private const int MaxDelta = 5;

        /// <summary>
        ///     Indicates whether two points are near one from the other
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private bool Around(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) < MaxDelta && Math.Abs(p1.Y - p2.Y) < MaxDelta;
        }
        
        /// <summary>
        ///     The dictionary used to keep the relation between boxe controls and their model
        /// </summary>
        private readonly Dictionary<TBoxModel, BoxControl<TEnclosing, TBoxModel, TArrowModel>> _boxes =
            new Dictionary<TBoxModel, BoxControl<TEnclosing, TBoxModel, TArrowModel>>();

        /// <summary>
        ///     Provides the box control which corresponds to the model provided
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BoxControl<TEnclosing, TBoxModel, TArrowModel> GetBoxControl(TBoxModel model)
        {
            BoxControl<TEnclosing, TBoxModel, TArrowModel> retVal = null;

            if (model != null)
            {
                if (_boxes.ContainsKey(model))
                {
                    retVal = _boxes[model];
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The dictionary used to keep the relation between arrows and their model
        /// </summary>
        private readonly Dictionary<TArrowModel, ArrowControl<TEnclosing, TBoxModel, TArrowModel>> _arrows =
            new Dictionary<TArrowModel, ArrowControl<TEnclosing, TBoxModel, TArrowModel>>();

        /// <summary>
        ///     Provides the arrow control which corresponds to the model provided
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ArrowControl<TEnclosing, TBoxModel, TArrowModel> GetArrowControl(TArrowModel model)
        {
            ArrowControl<TEnclosing, TBoxModel, TArrowModel> retVal = null;

            if (_arrows.ContainsKey(model))
            {
                retVal = _arrows[model];
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the arrow control which corresponds to the rule
        /// </summary>
        /// <param name="referencedModel"></param>
        /// <returns></returns>
        public ArrowControl<TEnclosing, TBoxModel, TArrowModel> GetArrowControl(ModelElement referencedModel)
        {
            ArrowControl<TEnclosing, TBoxModel, TArrowModel> retVal = null;

            foreach (ArrowControl<TEnclosing, TBoxModel, TArrowModel> control in _arrows.Values)
            {
                if (control.Model.ReferencedModel == referencedModel)
                {
                    retVal = control;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates whether the layout should be suspended
        /// </summary>
        private bool _refreshingControl;

        /// <summary>
        ///     Refreshes the layout, if it is not suspended
        /// </summary>
        public override void Refresh()
        {
            if (!_refreshingControl)
            {
                base.Refresh();
            }
        }

        /// <summary>
        ///     Provides the boxes that need be displayed
        /// </summary>
        /// <returns></returns>
        public abstract List<TBoxModel> GetBoxes();

        /// <summary>
        ///     Provides the arrows that need be displayed
        /// </summary>
        /// <returns></returns>
        public abstract List<TArrowModel> GetArrows();

        /// <summary>
        ///     Refreshes the control according to the model
        /// </summary>
        public void RefreshControl()
        {
            try
            {
                _refreshingControl = true;
                pleaseWaitLabel.Visible = true;
                SuspendLayout();

                foreach (BoxControl<TEnclosing, TBoxModel, TArrowModel> control in _boxes.Values)
                {
                    control.Parent = null;
                }
                _boxes.Clear();

                foreach (ArrowControl<TEnclosing, TBoxModel, TArrowModel> control in _arrows.Values)
                {
                    control.Parent = null;
                }
                _arrows.Clear();

                List<TBoxModel> theBoxes = GetBoxes();
                foreach (TBoxModel model in theBoxes)
                {
                    BoxControl<TEnclosing, TBoxModel, TArrowModel> boxControl = CreateBox(model);
                    boxControl.Parent = this;
                    boxControl.RefreshControl();
                    _boxes[model] = boxControl;
                }

                List<TArrowModel> theArrows = GetArrows();
                foreach (TArrowModel model in theArrows)
                {
                    bool showArrow = true;
                    if (model.Source != null)
                    {
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        showArrow = showArrow && !model.Source.Hidden;
                    }

                    if (model.Target != null)
                    {
                        showArrow = showArrow && !model.Target.Hidden;
                    }

                    if (showArrow)
                    {
                        ArrowControl<TEnclosing, TBoxModel, TArrowModel> arrowControl = CreateArrow(model);
                        arrowControl.Parent = this;
                        _arrows[model] = arrowControl;
                    }
                }

                UpdateArrowPosition();
            }
            finally
            {
                _refreshingControl = false;
                pleaseWaitLabel.Visible = false;
                ResumeLayout(true);
            }

            Refresh();
        }

        /// <summary>
        ///     Handles the rectangles that are already allocated in the diagram
        /// </summary>
        private class BoxAllocation
        {
            /// <summary>
            ///     The allocated rectangles
            /// </summary>
            private readonly List<Rectangle> _allocatedBoxes = new List<Rectangle>();
            
            /// <summary>
            ///     Finds a rectangle which intersects with the current rectangle
            /// </summary>
            /// <param name="rectangle"></param>
            /// <returns></returns>
            public Rectangle Intersects(Rectangle rectangle)
            {
                Rectangle retVal = Rectangle.Empty;

                foreach (Rectangle current in _allocatedBoxes)
                {
                    if (current.IntersectsWith(rectangle))
                    {
                        retVal = current;
                        break;
                    }
                }

                return retVal;
            }

            /// <summary>
            ///     Allocates a new rectangle
            /// </summary>
            /// <param name="rectangle"></param>
            public void Allocate(Rectangle rectangle)
            {
                _allocatedBoxes.Add(rectangle);
            }
        }

        /// <summary>
        ///     The allocated boxes
        /// </summary>
        private BoxAllocation _allocatedBoxes;

        /// <summary>
        ///     Provides a distance between two points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private int Distance(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }

        /// <summary>
        ///     Updates the arrows position to ensure that no overlap exists
        ///     - on the arrows
        ///     - on their text
        /// </summary>
        public void UpdateArrowPosition()
        {
            ComputeArrowPosition();
            ComputeArrowTextPosition();
        }

        /// <summary>
        ///     The size of the shift between arrows to be used when overlap occurs (more or less horizontally)
        /// </summary>
        private const int HorizontalShiftSize = 40;

        /// <summary>
        ///     The size of the shift between arrows to be used when overlap occurs (more or less horizontally)
        /// </summary>
        private const int VerticalShiftSize = 20;

        /// <summary>
        ///     Ensures that two arrowss do not overlap by computing an offset between the arrows
        /// </summary>
        private void ComputeArrowPosition()
        {
            List<ArrowControl<TEnclosing, TBoxModel, TArrowModel>> workingSet = new List<ArrowControl<TEnclosing, TBoxModel, TArrowModel>>();
            workingSet.AddRange(_arrows.Values);

            while (workingSet.Count > 1)
            {
                ArrowControl<TEnclosing, TBoxModel, TArrowModel> t1 = workingSet[0];
                workingSet.Remove(t1);

                // Compute the set of arrows overlapping with t1
                List<ArrowControl<TEnclosing, TBoxModel, TArrowModel>> overlap = new List<ArrowControl<TEnclosing, TBoxModel, TArrowModel>> { t1 };
                foreach (ArrowControl<TEnclosing, TBoxModel, TArrowModel> t in workingSet)
                {
                    if (t.Model.Source == t1.Model.Source &&
                        t.Model.Target == t1.Model.Target)
                    {
                        overlap.Add(t);
                    }
                    else if ((t.Model.Source == t1.Model.Target &&
                              t.Model.Target == t1.Model.Source))
                    {
                        overlap.Add(t);
                    }
                }

                // Remove all arrows of this overlap class from the working set
                foreach (ArrowControl<TEnclosing, TBoxModel, TArrowModel> t in overlap)
                {
                    workingSet.Remove(t);
                }

                // Shift arrows of this overlap set if they are overlapping (that is, if the set size > 1)
                if (overlap.Count > 1)
                {
                    Point shift; // the shift to be applied to the current arrow
                    Point offset; // the offset to apply on all arrows of this overlap set

                    double angle = overlap[0].Angle;
                    if ((angle > Math.PI/4 && angle < 3*Math.PI/4) ||
                        (angle < -Math.PI/4 && angle > -3*Math.PI/4))
                    {
                        // Horizontal shift
                        shift = new Point(-(overlap.Count - 1)*HorizontalShiftSize/2, 0);
                        offset = new Point(HorizontalShiftSize, 0);
                    }
                    else
                    {
                        // Vertical shift
                        shift = new Point(0, -(overlap.Count - 1)*VerticalShiftSize/2);
                        offset = new Point(0, VerticalShiftSize);
                    }

                    int i = 0;
                    foreach (ArrowControl<TEnclosing, TBoxModel, TArrowModel> arrow in overlap)
                    {
                        arrow.Offset = shift;
                        shift.Offset(offset);

                        if (arrow.TargetBoxControl == null)
                        {
                            arrow.EndOffset = new Point(0, VerticalShiftSize*i/2);
                        }
                        i = i + 1;
                    }
                }
            }
        }

        /// <summary>
        ///     Computes the position of the arrow texts, following the arrow, to avoid text overlap
        /// </summary>
        private void ComputeArrowTextPosition()
        {
            _allocatedBoxes = new BoxAllocation();

            // Allocate all boxes as non available
            foreach (BoxControl<TEnclosing, TBoxModel, TArrowModel> box in _boxes.Values)
            {
                Rectangle rectangle = box.DisplayRectangle;
                rectangle.Offset(box.Location);
                _allocatedBoxes.Allocate(rectangle);
            }

            foreach (ArrowControl<TEnclosing, TBoxModel, TArrowModel> arrow in _arrows.Values)
            {
                Point center = arrow.GetCenter();
                Point upSlide = Slide(arrow, center, ArrowControl<TEnclosing, TBoxModel, TArrowModel>.SlideDirection.Up);
                Point downSlide = Slide(arrow, center, ArrowControl<TEnclosing, TBoxModel, TArrowModel>.SlideDirection.Down);

                Rectangle boundingBox;
                if (Distance(center, upSlide) <= Distance(center, downSlide))
                {
                    boundingBox = arrow.GetTextBoundingBox(upSlide);
                }
                else
                {
                    boundingBox = arrow.GetTextBoundingBox(downSlide);
                }

                arrow.Location = new Point(boundingBox.X, boundingBox.Y);
                _allocatedBoxes.Allocate(boundingBox);
            }
        }

        /// <summary>
        ///     Tries to slide the arrow up following the arrow to avoid any collision
        ///     with the already allocated bounding boxes
        /// </summary>
        /// <param name="arrow"></param>
        /// <param name="center"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Point Slide(ArrowControl<TEnclosing, TBoxModel, TArrowModel> arrow, Point center,
            ArrowControl<TEnclosing, TBoxModel, TArrowModel>.SlideDirection direction)
        {
            Point retVal = center;
            Rectangle colliding = _allocatedBoxes.Intersects(arrow.GetTextBoundingBox(retVal));

            while (colliding != Rectangle.Empty)
            {
                retVal = arrow.Slide(retVal, colliding, direction);
                colliding = _allocatedBoxes.Intersects(arrow.GetTextBoundingBox(retVal));
            }

            return retVal;
        }

        /// <summary>
        ///     The next position available for a computed box position
        /// </summary>
        protected Point CurrentPosition = new Point(1, 1);

        /// <summary>
        ///     Reinitialises the automatic position handling
        /// </summary>
        protected virtual void InitPositionHandling()
        {
            CurrentPosition = new Point(1, 1);
        }

        /// <summary>
        ///     Provides the next available position in the box-arrow diagram
        /// </summary>
        /// <returns></returns>
        public virtual Point GetNextPosition(TBoxModel model)
        {
            Point retVal = new Point(CurrentPosition.X, CurrentPosition.Y);

            // Prepare the next call for GetNextPosition
            int xOffset = model.Width + 10;
            int yOffset = model.Height + 10;

            CurrentPosition.Offset(xOffset, 0);
            if (CurrentPosition.X > Size.Width - model.Width)
            {
                CurrentPosition = new Point(1, CurrentPosition.Y + yOffset);
            }

            return retVal;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            try
            {
                Graphics g = e.Graphics;
                foreach (BoxControl<TEnclosing, TBoxModel, TArrowModel> control in _boxes.Values)
                {
                    control.PaintInBoxArrowPanel(g);
                }

                foreach (ArrowControl<TEnclosing, TBoxModel, TArrowModel> control in _arrows.Values)
                {
                    control.PaintInBoxArrowPanel(g);
                }
            }
            catch (Exception)
            {
            }
        }

        public void ControlHasMoved()
        {
            UpdateArrowPosition();
        }

        /// <summary>
        ///     Provides the enclosing form
        /// </summary>
        protected BoxArrowWindow<TEnclosing, TBoxModel, TArrowModel> EnclosingWindow
        {
            get { return GuiUtils.EnclosingFinder<BoxArrowWindow<TEnclosing, TBoxModel, TArrowModel>>.Find(this); }
        }

        /// <summary>
        ///     Indicates whether the control is selected
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        internal bool IsSelected(Control control)
        {
            bool retVal = false;

            if (EnclosingWindow != null)
            {
                retVal = EnclosingWindow.IsSelected(control);
            }

            return retVal;
        }
    }
}
