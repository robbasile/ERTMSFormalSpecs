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
using DataDictionary.Generated;
using GUI.LongOperations;
using GUI.Properties;
using Utils;
using ModelElement = DataDictionary.ModelElement;
using Util = DataDictionary.Util;

namespace GUI.BoxArrowDiagram
{
    public abstract partial class BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel> : BaseBoxArrowPanel
        where TEnclosing : class
        where TBoxModel : class, IGraphicalDisplay
        where TArrowModel : class, IGraphicalArrow<TBoxModel>
    {
        /// <summary>
        ///     The size of an box control button
        /// </summary>
        public Size DefaultBoxSize = new Size(100, 50);

        /// <summary>
        ///     The model element for which this panel is built
        /// </summary>
        public virtual TEnclosing Model { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected BoxArrowPanel()
        {
            InitializeComponent();

            pictureBox.Location = new Point(0, 0);

            pictureBox.MouseDown += HandleMouseDown;
            pictureBox.MouseMove += HandleMouseMove;
            pictureBox.MouseUp += HandleMouseUp;
            pictureBox.Click += HandleClick;
            pictureBox.DoubleClick += HandleDoubleClick;
            pictureBox.Paint += PaintContent;

            DragEnter += DragEnterHandler;
            DragDrop += DragDropHandler;
            AllowDrop = true;
            DoubleBuffered = true;
        }

        /// <summary>
        /// Handles a click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void HandleClick(object sender, EventArgs e)
        {
            MouseEventArgs mouseEventArgs = e as MouseEventArgs;
            if (mouseEventArgs != null)
            {
                GraphicElement element = ElementForLocation(mouseEventArgs.Location);
                if (element != null)
                {
                    Selected = element;
                    Refresh();
                    element.HandleClick(sender, mouseEventArgs);
                }

                if (mouseEventArgs.Button == MouseButtons.Right)
                {
                    // Build the contextual menu according to the enclosing panel tree view
                    ContextMenu menu = BuildContextMenu(element);
                    if (menu != null)
                    {
                        if (element != null)
                        {
                            Selected = element;
                        }

                        Point location = new Point(mouseEventArgs.Location.X - HorizontalScroll.Value, mouseEventArgs.Y - VerticalScroll.Value);
                        menu.Show(this, location);
                    }
                }
            }
        }

        /// <summary>
        /// Builds teh context menu associated to either the selected element or the panel
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected virtual ContextMenu BuildContextMenu(GraphicElement element)
        {
            ContextMenu retVal = null;

            // Build the contextual menu according to the enclosing panel tree view
            IModelElement model = null;
            if (element != null)
            {
                model = element.Model as IModelElement;
            }
            if (model == null)
            {
                model = Model as IModelElement;
            }

            BaseTreeNode node = CorrespondingNode(model);
            if (node == null)
            {
                node = CorrespondingNode(Model as IModelElement);
            }
            if (node != null)
            {
                retVal = node.ContextMenu;
            }
            return retVal;
        }

        /// <summary>
        /// Provides the base tree node associated to a model element
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected BaseTreeNode CorrespondingNode(IModelElement model)
        {
            BaseTreeNode retVal = null;

            IBaseForm baseForm = GuiUtils.EnclosingFinder<IBaseForm>.Find(this);
            if (baseForm != null && baseForm.TreeView != null)
            {
                retVal = baseForm.TreeView.FindNode(model, true);
            }

            return retVal;
        }

        /// <summary>
        /// Handles a double click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void HandleDoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs mouseEventArgs = e as MouseEventArgs;
            if (mouseEventArgs != null)
            {
                GraphicElement element = ElementForLocation(mouseEventArgs.Location);
                if (element != null)
                {
                    element.HandleDoubleClick(sender, mouseEventArgs);
                }
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
                    // The location where the element has been dropped
                    Point location = PointToClient(new Point(e.X, e.Y));
                    BoxControl<TEnclosing, TBoxModel, TArrowModel> target = BoxForLocation(location);
                    if (target != null)
                    {
                        target.AcceptDrop(sourceNode.Model as ModelElement);
                    }
                }
            }
        }

        /// <summary>
        ///     The selected object
        /// </summary>
        public GraphicElement Selected { get; set; }

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
        /// Provides the element for the given location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected
            GraphicElement ElementForLocation(Point location)
        {
            GraphicElement retVal = BoxForLocation(location);

            if (retVal == null)
            {
                retVal = ArrowForLocation(location);
            }

            return retVal;
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
        /// The box that is currently being moved
        /// </summary>
        private BoxControl<TEnclosing, TBoxModel, TArrowModel> _movingBox;

        /// <summary>
        /// Indicates that the moving box moved sufficiently to be actually considered as moving
        /// </summary>
        private bool _movingBoxHasMoved;

        /// <summary>
        ///     The location where the mouse down occured
        /// </summary>
        private Point _moveStartLocation;

        /// <summary>
        ///     The control location where the mouse down occured
        /// </summary>
        private Point _positionBeforeMove;

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

        /// <summary>
        /// Handles a mouse down event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        public void HandleMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            GraphicElement element = ElementForLocation(mouseEventArgs.Location);
            if (element != null)
            {
                element.HandleMouseDown(sender, mouseEventArgs);
            }

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                Point clickPoint = new Point(mouseEventArgs.X, mouseEventArgs.Y);
                foreach (var arrow in _arrows.Values)
                {
                    if (Around(arrow.StartLocation, clickPoint))
                    {
                        _changingArrow = arrow;
                        _chaningArrowAction = ChangeAction.InitialBox;
                        Selected = arrow;
                        break;
                    }
                    if (Around(arrow.TargetLocation, clickPoint))
                    {
                        _changingArrow = arrow;
                        _chaningArrowAction = ChangeAction.TargetBox;
                        Selected = arrow;
                        break;
                    }
                }

                if (_changingArrow == null)
                {
                    var box = BoxForLocation(clickPoint);
                    if (box != null)
                    {
                        _movingBox = box;
                        _movingBoxHasMoved = false;
                        _moveStartLocation = mouseEventArgs.Location;
                        _positionBeforeMove = box.Location;
                    }
                }
            }
        }

        /// <summary>
        ///     Handles the move event, which, in case of an arrow is selected to be modified,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        private void HandleMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            GraphicElement element = ElementForLocation(mouseEventArgs.Location);
            if (element != null)
            {
                element.HandleMouseMove(sender, mouseEventArgs);
            } 
            
            if (_changingArrow != null && _chaningArrowAction != ChangeAction.None)
            {
                BoxControl<TEnclosing, TBoxModel, TArrowModel> box = BoxForLocation(mouseEventArgs.Location);
                if (box != null)
                {
                    switch (_chaningArrowAction)
                    {
                        case ChangeAction.InitialBox:
                            if (_changingArrow.TypedModel.Source != box.Model)
                            {
                                _changingArrow.SetInitialBox(box.TypedModel);
                            }
                            break;
                        case ChangeAction.TargetBox:
                            if (_changingArrow.TypedModel.Target != box.Model)
                            {
                                if (_changingArrow.TypedModel.Source != null)
                                {
                                    _changingArrow.SetTargetBox(box.TypedModel);
                                }
                            }
                            break;
                    }
                }
            }

            if (_movingBox != null)
            {
                Point mouseMoveLocation = mouseEventArgs.Location;

                int deltaX = mouseMoveLocation.X - _moveStartLocation.X;
                int deltaY = mouseMoveLocation.Y - _moveStartLocation.Y;

                if (Math.Abs(deltaX) > 5 || Math.Abs(deltaY) > 5)
                {
                    IModelElement model = _movingBox.TypedModel;
                    if (model != null && !_movingBoxHasMoved)
                    {
                        Context.SelectionCriteria criteria = GuiUtils.SelectionCriteriaBasedOnMouseEvent(mouseEventArgs);
                        EFSSystem.INSTANCE.Context.SelectElement(model, this, criteria);
                        _movingBoxHasMoved = true;
                    } 
                    
                    Util.DontNotify(() =>
                    {
                        int newX = _positionBeforeMove.X + deltaX;
                        int newY = _positionBeforeMove.Y + deltaY;
                        SetBoxPosition(_movingBox, newX, newY);                        
                        UpdatePositions();
                    });
                }
            }
        }

        /// <summary>
        ///     The grid size used to place boxes
        /// </summary>
        public int GridSize = 10;

        /// <summary>
        ///     Sets the position of the control, according to the X & Y provided
        /// </summary>
        /// <param name="box"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetBoxPosition(BoxControl<TEnclosing, TBoxModel, TArrowModel> box, int x, int y)
        {
            int posX = (x/GridSize)*GridSize;
            int posY = (y/GridSize)*GridSize;

            box.Location = new Point(posX, posY);
        }

        /// <summary>
        /// Handles a mouse up event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        private void HandleMouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
            GraphicElement element = ElementForLocation(mouseEventArgs.Location);
            if (element != null)
            {
                element.HandleMouseUp(sender, mouseEventArgs);
            }

            if (_changingArrow != null)
            {
                _changingArrow = null;
                _chaningArrowAction = ChangeAction.None;
                RefreshControl();                
            }

            if (_movingBox != null)
            {
                if (_movingBoxHasMoved)
                {
                    if (element != null)
                    {
                        BaseTreeNode targetNode = CorrespondingNode(element.Model as IModelElement);
                        BaseTreeNode sourceNode = CorrespondingNode(_movingBox.Model as IModelElement);

                        if (targetNode != null && sourceNode != null && sourceNode != targetNode)
                        {
                            targetNode.AcceptDrop(sourceNode);
                            _movingBox.Location = new Point(0, 0);

                            if (Settings.Default.AllowRefactor)
                            {
                                RefactorAndRelocateOperation refactorAndRelocate =
                                    new RefactorAndRelocateOperation(sourceNode.Model as ModelElement);
                                refactorAndRelocate.ExecuteUsingProgressDialog("Refactoring", false);
                            }
                        }
                    }

                    // Register the fact that the element has moved
                    // because 
                    if (_movingBox.TypedModel.X != _positionBeforeMove.X ||
                        _movingBox.TypedModel.Y != _positionBeforeMove.Y)
                    {
                        EFSSystem.INSTANCE.Context.HandleChangeEvent(_movingBox.Model as BaseModelElement,
                            Context.ChangeKind.ModelChange);
                    }
                }

                _movingBox = null;
                _movingBoxHasMoved = false;
            }
        }

        /// <summary>
        ///     The maximum delta when considering if two points are near one from the other
        /// </summary>
        private const int MaxDelta = 10;

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

            if (model != null)
            {
                if (_arrows.ContainsKey(model))
                {
                    retVal = _arrows[model];
                }
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
                if (control.TypedModel.ReferencedModel == referencedModel)
                {
                    retVal = control;
                    break;
                }
            }

            return retVal;
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
                SuspendLayout();

                // Clear all
                _boxes.Clear();
                _arrows.Clear();

                // Consider all boxes in this panel
                foreach (TBoxModel model in GetBoxes())
                {
                    BoxControl<TEnclosing, TBoxModel, TArrowModel> boxControl = CreateBox(model);
                    _boxes[model] = boxControl;
                }

                // Consider all arrows in this panel
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
                        _arrows[model] = arrowControl;
                    }
                }

                UpdatePositions();
            }
            finally
            {
                ResumeLayout(true);
            }
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
        /// Updates the position of both boxes and arrows
        /// </summary>
        private void UpdatePositions()
        {
            UpdateBoxPosition();
            UpdateArrowPosition();
            Refresh();
        }

        /// <summary>
        /// Update the box location and compute the panel size
        /// </summary>
        private void UpdateBoxPosition()
        {
            Size size = new Size(0, 0);
            const int deltaHeight = 20;
            const int deltaWidth = 20;
            foreach (var box in _boxes.Values)
            {
                if (box.Width == 0 || box.Height == 0)
                {
                    // Setup default size
                    box.Size = DefaultBoxSize;
                }

                if( box.Location.IsEmpty )
                {
                    // Setup next location
                    box.Location = GetNextPosition();
                }

                Rectangle rectangle = box.Rectangle;
                int height = rectangle.Y + rectangle.Height;
                if (height > size.Height)
                {
                    size.Height = height + deltaHeight;
                }

                int width = rectangle.X + rectangle.Width;
                if (width > size.Width)
                {
                    size.Width = width + deltaWidth;
                }
            }

            if (size.Width < Size.Width)
            {
                size.Width = Size.Width;
            }
            if (size.Height < Size.Height)
            {
                size.Height = Size.Height;
            }
            pictureBox.Size = size;
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
                    if (t.TypedModel.Source == t1.TypedModel.Source &&
                        t.TypedModel.Target == t1.TypedModel.Target)
                    {
                        overlap.Add(t);
                    }
                    else if ((t.TypedModel.Source == t1.TypedModel.Target &&
                              t.TypedModel.Target == t1.TypedModel.Source))
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
                Rectangle rectangle = box.Rectangle;
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
        ///     Provides the next available position in the box-arrow diagram
        /// </summary>
        /// <returns></returns>
        public Point GetNextPosition()
        {
            Point retVal;

            Point currentPosition = new Point(1, 1);
            do
            {
                // Check the current position
                retVal = currentPosition;

                // Ensure there is no clash
                Rectangle rectangle = new Rectangle(retVal, DefaultBoxSize);
                foreach (var box in _boxes.Values)
                {
                    if (box.Rectangle.IntersectsWith(rectangle))
                    {
                        retVal = Point.Empty;
                        break;
                    }
                }

                // Prepare the next position
                currentPosition.Offset(DefaultBoxSize.Width + 10, 0);
                if (currentPosition.X + DefaultBoxSize.Width > Size.Width)
                {
                    currentPosition = new Point(1, currentPosition.Y + DefaultBoxSize.Height + 10);
                }
            } while (retVal == Point.Empty);

            return retVal;
        }


        /// <summary>
        /// Paints the pannel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaintContent(object sender, PaintEventArgs e)
        {
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
        ///     Factory for BoxEditor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected virtual BoxEditor<TEnclosing, TBoxModel, TArrowModel> CreateBoxEditor(BoxControl<TEnclosing, TBoxModel, TArrowModel> control)
        {
            return new BoxEditor<TEnclosing, TBoxModel, TArrowModel>(control);
        }

        /// <summary>
        ///     Factory for arrow editor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected virtual ArrowEditor<TEnclosing, TBoxModel, TArrowModel> CreateArrowEditor(ArrowControl<TEnclosing, TBoxModel, TArrowModel> control)
        {
            return new ArrowEditor<TEnclosing, TBoxModel, TArrowModel>(control);
        }

        /// <summary>
        /// Creates the editor for the selected object
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override object CreateEditor(IModelElement model)
        {
            object retVal= null;

            var boxControl = GetBoxControl(model as TBoxModel);
            if (boxControl != null)
            {
                retVal = CreateBoxEditor(boxControl);
            }

            var arrowControl = GetArrowControl(model as TArrowModel);
            if (arrowControl != null)
            {
                retVal = CreateArrowEditor(arrowControl);
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates whether the control is selected
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsSelected(GraphicElement element)
        {
            bool retVal = element == Selected;

            return retVal;
        }
    }
}
