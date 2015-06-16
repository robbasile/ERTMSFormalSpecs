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
using System.Reflection;
using System.Windows.Forms;
using DataDictionary;
using System.IO;
using System.Globalization;
using ErtmsSolutions.Utils.RunProcessExec;

namespace GUI.BoxArrowDiagram
{
    public abstract partial class BoxArrowPanel<BoxModel, ArrowModel> : Panel
        where BoxModel : class, IGraphicalDisplay
        where ArrowModel : class, IGraphicalArrow<BoxModel>
    {
        private ToolStripMenuItem refreshMenuItem;
        private ToolStripMenuItem reDisplayMenuItem;

        /// <summary>
        ///     Initializes the context menu items
        /// </summary>
        public virtual void InitializeStartMenu()
        {
            // 
            // Refresh
            // 
            refreshMenuItem = new ToolStripMenuItem();
            refreshMenuItem.Name = "refreshMenuItem";
            refreshMenuItem.Size = new Size(161, 22);
            refreshMenuItem.Text = "Refresh";
            refreshMenuItem.Click += refreshMenuItem_Click;
            // 
            // Redisplay
            // 
            reDisplayMenuItem = new ToolStripMenuItem();
            reDisplayMenuItem.Name = "reDisplayMenuItem";
            reDisplayMenuItem.Size = new Size(250, 22);
            reDisplayMenuItem.Text = "Redisplay items";
            reDisplayMenuItem.Click += ReDisplayMenuItem_Click;

            contextMenu.Items.Clear();
            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                reDisplayMenuItem,
                refreshMenuItem
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
        private object _model = null;

        /// <summary>
        ///     The model element for which this panel is built
        /// </summary>
        public object Model
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
        public BoxArrowPanel()
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

            Paint += BoxArrowPanel_Paint;
        }

        private void BoxArrowPanel_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouseEvent = (MouseEventArgs) e;

            if (mouseEvent.Button == MouseButtons.Right)
            {
                BaseForm baseForm = EnclosingForm as BaseForm;
                if (baseForm != null && baseForm.TreeView != null)
                {
                    BaseTreeNode node = baseForm.TreeView.FindNode(Model as ModelElement);
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

        private const int CTRL = 8;
        private const int ALT = 32;

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
                    BoxControl<BoxModel, ArrowModel> target = null;

                    foreach (BoxControl<BoxModel, ArrowModel> box in boxes.Values)
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
        ///     Refreshes the panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReDisplayMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = System.IO.Path.GetTempFileName();
            try
            {
                // Generate the .dot file
                StreamWriter writer = new StreamWriter(filePath);
                writer.WriteLine("digraph Test {");
                Dictionary<string, BoxModel> theBoxes = new Dictionary<string, BoxModel>();
                Dictionary<string, List<BoxModel>> theBoxesByType = new Dictionary<string, List<BoxModel>>();
                foreach (BoxModel box in getBoxes())
                {
                    // Keep track of the boxes
                    theBoxes[box.Guid] = box;
                    string boxType = box.GetType().ToString();
                    if (!theBoxesByType.ContainsKey(boxType))
                    {
                        theBoxesByType[boxType] = new List<BoxModel>();
                    }
                    theBoxesByType[boxType].Add(box);

                    string name = box.GraphicalName;
                    if (name.Length > 12)
                    {
                        name = name.Substring(0, 12) + "...";
                    }

                    string pinned = "";
                    if (box.Pinned)
                    {
                        pinned = "!";
                    }
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                        "  \"{0}\"[shape=record,width={1},height={2},label={3},pos=\"{4},{5}{6}\"];", box.Guid,
                        DefaultBoxSize.Width/100.0, DefaultBoxSize.Height/100.0, box.GraphicalName, box.X/100.0,
                        box.Y/100.0, pinned));
                }

                // Generate the arrows
                foreach (ArrowModel arrow in getArrows())
                {
                    string sourceGuid = "";
                    if (arrow.Source != null)
                    {
                        sourceGuid = arrow.Source.Guid;
                    }
                    string targetGuid = "";
                    if (arrow.Target != null)
                    {
                        targetGuid = arrow.Target.Guid;
                    }
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "  \"{0}\" -> \"{1}\";", sourceGuid,
                        targetGuid));
                }

                // Generate dependancies betweens boxes
                foreach (List<BoxModel> boxesOfAKind in theBoxesByType.Values)
                {
                    for (int i = 0; i < boxesOfAKind.Count - 1; i++)
                    {
                        for (int j = i + 1; j < boxesOfAKind.Count; j++)
                        {
                            writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                "  \"{0}\" -> \"{1}\"[penwidth=0,arrowhead=none,len={2}];", boxesOfAKind[i].Guid,
                                boxesOfAKind[j].Guid, ((boxesOfAKind.Count + 5)/5)*DefaultBoxSize.Width/100.0));
                        }
                    }
                }

                writer.WriteLine("}");
                writer.Close();

                // Run graphviz
                RunProcessExec pe = new RunProcessExec(
                    /* WorkingDirectory,    */ @".",
                        /* Path,                */ @"c:\Program Files (x86)\Graphviz 2.28\bin",
                        /* Program,             */ "neato.exe",
                        /* Arguments,           */ "-O -Tplain " + filePath,
                        /* FName_StandardInput, */ null,
                        /* FName_StandardOutput,*/ null,
                        /* FName_StandardError, */ null,
                        /* TimeSpan TimeOut)    */ new TimeSpan(0, 0, 0, 1 /*second*/));

                // Retrieve results
                RunProcessExec.ProcessExecResult_Struct result = pe.StartAndWait();
                if (result.ExecResult == RunProcessExec.ProcessExecResult_Enum.OK)
                {
                    StreamReader reader = new StreamReader(filePath + ".plain");
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] words = line.Split(' ');
                        if (words.Length > 0)
                        {
                            if (words[0] == "node" && words.Length >= 6)
                            {
                                string guid = words[1].Substring(1, words[1].Length - 2); // Remove the quotes
                                if (theBoxes.ContainsKey(guid))
                                {
                                    BoxModel box = theBoxes[guid];
                                    if (!box.Pinned)
                                    {
                                        box.X = (int) (Decimal.Parse(words[2], CultureInfo.InvariantCulture)*100);
                                        box.Y = (int) (Decimal.Parse(words[3], CultureInfo.InvariantCulture)*100);
                                        box.Width = DefaultBoxSize.Width;
                                        box.Height = DefaultBoxSize.Height;
                                    }
                                }
                            }
                        }
                    }
                }

                RefreshControl();
            }
            catch (Exception excp)
            {
                MessageBox.Show("Cannot perfom operation due to error " + excp.Message, "Cannot perform operation",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    if (filePath != null)
                    {
                        File.Delete(filePath);
                        File.Delete(filePath + ".plain");
                    }
                }
                catch (Exception)
                {
                }
            }
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
        public abstract BoxControl<BoxModel, ArrowModel> createBox(BoxModel model);

        /// <summary>
        ///     Method used to create an arrow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract ArrowControl<BoxModel, ArrowModel> createArrow(ArrowModel model);

        /// <summary>
        ///     The arrow that is currently being changed
        /// </summary>
        private ArrowControl<BoxModel, ArrowModel> changingArrow = null;

        /// <summary>
        ///     The action that is applied on the arrow
        /// </summary>
        private enum ChangeAction
        {
            None,
            InitialBox,
            TargetBox
        };

        private ChangeAction chaningArrowAction = ChangeAction.None;

        private void BoxArrowPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Point clickPoint = new Point(e.X, e.Y);
                foreach (ArrowControl<BoxModel, ArrowModel> arrow in arrows.Values)
                {
                    if (around(arrow.StartLocation, clickPoint))
                    {
                        changingArrow = arrow;
                        changingArrow.Parent = this; // I do not know why...
                        chaningArrowAction = ChangeAction.InitialBox;
                        break;
                    }
                    if (around(arrow.TargetLocation, clickPoint))
                    {
                        changingArrow = arrow;
                        changingArrow.Parent = this; // I do not know why...
                        chaningArrowAction = ChangeAction.TargetBox;
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
        protected BoxControl<BoxModel, ArrowModel> BoxForLocation(Point location)
        {
            BoxControl<BoxModel, ArrowModel> retVal = null;

            foreach (BoxControl<BoxModel, ArrowModel> box in boxes.Values)
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
        protected ArrowControl<BoxModel, ArrowModel> ArrowForLocation(Point location)
        {
            ArrowControl<BoxModel, ArrowModel> retVal = null;

            foreach (ArrowControl<BoxModel, ArrowModel> arrow in arrows.Values)
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
            if (changingArrow != null && chaningArrowAction != ChangeAction.None)
            {
                BoxControl<BoxModel, ArrowModel> box = BoxForLocation(e.Location);
                if (box != null)
                {
                    switch (chaningArrowAction)
                    {
                        case ChangeAction.InitialBox:
                            if (changingArrow.Model.Source != box.Model)
                            {
                                changingArrow.SetInitialBox(box.Model);
                            }
                            break;
                        case ChangeAction.TargetBox:
                            if (changingArrow.Model.Target != box.Model)
                            {
                                if (changingArrow.Model.Source != null)
                                {
                                    changingArrow.SetTargetBox(box.Model);
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void BoxArrowPanel_MouseUp(object sender, MouseEventArgs e)
        {
            changingArrow = null;
            chaningArrowAction = ChangeAction.None;
        }

        /// <summary>
        ///     The maximum delta when considering if two points are near one from the other
        /// </summary>
        private const int MAX_DELTA = 5;

        /// <summary>
        ///     Indicates whether two points are near one from the other
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private bool around(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) < MAX_DELTA && Math.Abs(p1.Y - p2.Y) < MAX_DELTA;
        }

        /// <summary>
        ///     Handles the add box event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBoxHandler(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Handles the add arrow event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddArrowHandler(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     The dictionary used to keep the relation between boxe controls and their model
        /// </summary>
        private Dictionary<BoxModel, BoxControl<BoxModel, ArrowModel>> boxes =
            new Dictionary<BoxModel, BoxControl<BoxModel, ArrowModel>>();

        /// <summary>
        ///     Provides the box control which corresponds to the model provided
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BoxControl<BoxModel, ArrowModel> getBoxControl(BoxModel model)
        {
            BoxControl<BoxModel, ArrowModel> retVal = null;

            if (model != null)
            {
                if (boxes.ContainsKey(model))
                {
                    retVal = boxes[model];
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The dictionary used to keep the relation between arrows and their model
        /// </summary>
        private Dictionary<ArrowModel, ArrowControl<BoxModel, ArrowModel>> arrows =
            new Dictionary<ArrowModel, ArrowControl<BoxModel, ArrowModel>>();

        /// <summary>
        ///     Provides the arrow control which corresponds to the model provided
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ArrowControl<BoxModel, ArrowModel> getArrowControl(ArrowModel model)
        {
            ArrowControl<BoxModel, ArrowModel> retVal = null;

            if (arrows.ContainsKey(model))
            {
                retVal = arrows[model];
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the arrow control which corresponds to the rule
        /// </summary>
        /// <param name="referencedModel"></param>
        /// <returns></returns>
        public ArrowControl<BoxModel, ArrowModel> getArrowControl(DataDictionary.ModelElement referencedModel)
        {
            ArrowControl<BoxModel, ArrowModel> retVal = null;

            foreach (ArrowControl<BoxModel, ArrowModel> control in arrows.Values)
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
        private bool refreshingControl = false;

        /// <summary>
        ///     Refreshes the layout, if it is not suspended
        /// </summary>
        public override void Refresh()
        {
            if (!refreshingControl)
            {
                base.Refresh();
            }
        }

        /// <summary>
        ///     Provides the boxes that need be displayed
        /// </summary>
        /// <returns></returns>
        public abstract List<BoxModel> getBoxes();

        /// <summary>
        ///     Provides the arrows that need be displayed
        /// </summary>
        /// <returns></returns>
        public abstract List<ArrowModel> getArrows();

        /// <summary>
        ///     Refreshes the control according to the model
        /// </summary>
        public void RefreshControl()
        {
            try
            {
                refreshingControl = true;
                pleaseWaitLabel.Visible = true;
                SuspendLayout();

                foreach (BoxControl<BoxModel, ArrowModel> control in boxes.Values)
                {
                    control.Parent = null;
                }
                boxes.Clear();

                foreach (ArrowControl<BoxModel, ArrowModel> control in arrows.Values)
                {
                    control.Parent = null;
                }
                arrows.Clear();

                List<BoxModel> theBoxes = getBoxes();
                foreach (BoxModel model in theBoxes)
                {
                    BoxControl<BoxModel, ArrowModel> boxControl = createBox(model);
                    boxControl.Parent = this;
                    boxControl.RefreshControl();
                    boxes[model] = boxControl;
                }

                List<ArrowModel> theArrows = getArrows();
                foreach (ArrowModel model in theArrows)
                {
                    bool showArrow = true;
                    if (model.Source != null)
                    {
                        showArrow = showArrow && !model.Source.Hidden;
                    }

                    if (model.Target != null)
                    {
                        showArrow = showArrow && !model.Target.Hidden;
                    }

                    if (showArrow)
                    {
                        ArrowControl<BoxModel, ArrowModel> arrowControl = createArrow(model);
                        arrowControl.Parent = this;
                        arrows[model] = arrowControl;
                    }
                }

                UpdateArrowPosition();
            }
            finally
            {
                refreshingControl = false;
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
            private List<Rectangle> AllocatedBoxes = new List<Rectangle>();

            /// <summary>
            ///     Constructor
            /// </summary>
            public BoxAllocation()
            {
            }

            /// <summary>
            ///     Finds a rectangle which intersects with the current rectangle
            /// </summary>
            /// <param name="rectangle"></param>
            /// <returns></returns>
            public Rectangle Intersects(Rectangle rectangle)
            {
                Rectangle retVal = Rectangle.Empty;

                foreach (Rectangle current in AllocatedBoxes)
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
                AllocatedBoxes.Add(rectangle);
            }
        }

        /// <summary>
        ///     The allocated boxes
        /// </summary>
        private BoxAllocation AllocatedBoxes;

        /// <summary>
        ///     Provides a distance between two points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private int distance(Point p1, Point p2)
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
        private static int HORIZONTAL_SHIFT_SIZE = 40;

        /// <summary>
        ///     The size of the shift between arrows to be used when overlap occurs (more or less horizontally)
        /// </summary>
        private static int VERTICAL_SHIFT_SIZE = 20;

        /// <summary>
        ///     Ensures that two arrowss do not overlap by computing an offset between the arrows
        /// </summary>
        private void ComputeArrowPosition()
        {
            List<ArrowControl<BoxModel, ArrowModel>> workingSet = new List<ArrowControl<BoxModel, ArrowModel>>();
            workingSet.AddRange(arrows.Values);

            while (workingSet.Count > 1)
            {
                ArrowControl<BoxModel, ArrowModel> t1 = workingSet[0];
                workingSet.Remove(t1);

                // Compute the set of arrows overlapping with t1
                List<ArrowControl<BoxModel, ArrowModel>> overlap = new List<ArrowControl<BoxModel, ArrowModel>>();
                overlap.Add(t1);
                foreach (ArrowControl<BoxModel, ArrowModel> t in workingSet)
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
                foreach (ArrowControl<BoxModel, ArrowModel> t in overlap)
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
                        shift = new Point(-(overlap.Count - 1)*HORIZONTAL_SHIFT_SIZE/2, 0);
                        offset = new Point(HORIZONTAL_SHIFT_SIZE, 0);
                    }
                    else
                    {
                        // Vertical shift
                        shift = new Point(0, -(overlap.Count - 1)*VERTICAL_SHIFT_SIZE/2);
                        offset = new Point(0, VERTICAL_SHIFT_SIZE);
                    }

                    int i = 0;
                    foreach (ArrowControl<BoxModel, ArrowModel> arrow in overlap)
                    {
                        arrow.Offset = shift;
                        shift.Offset(offset);

                        if (arrow.TargetBoxControl == null)
                        {
                            arrow.EndOffset = new Point(0, VERTICAL_SHIFT_SIZE*i/2);
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
            AllocatedBoxes = new BoxAllocation();

            // Allocate all boxes as non available
            foreach (BoxControl<BoxModel, ArrowModel> box in boxes.Values)
            {
                Rectangle rectangle = box.DisplayRectangle;
                rectangle.Offset(box.Location);
                AllocatedBoxes.Allocate(rectangle);
            }

            foreach (ArrowControl<BoxModel, ArrowModel> arrow in arrows.Values)
            {
                Point center = arrow.getCenter();
                Point upSlide = Slide(arrow, center, ArrowControl<BoxModel, ArrowModel>.SlideDirection.Up);
                Point downSlide = Slide(arrow, center, ArrowControl<BoxModel, ArrowModel>.SlideDirection.Down);

                Rectangle boundingBox;
                if (distance(center, upSlide) <= distance(center, downSlide))
                {
                    boundingBox = arrow.getTextBoundingBox(upSlide);
                }
                else
                {
                    boundingBox = arrow.getTextBoundingBox(downSlide);
                }

                arrow.Location = new Point(boundingBox.X, boundingBox.Y);
                AllocatedBoxes.Allocate(boundingBox);
            }
        }

        /// <summary>
        ///     Tries to slide the arrow up following the arrow to avoid any collision
        ///     with the already allocated bounding boxes
        /// </summary>
        /// <param name="arrow"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        private Point Slide(ArrowControl<BoxModel, ArrowModel> arrow, Point center,
            ArrowControl<BoxModel, ArrowModel>.SlideDirection direction)
        {
            Point retVal = center;
            Rectangle colliding = AllocatedBoxes.Intersects(arrow.getTextBoundingBox(retVal));

            while (colliding != Rectangle.Empty)
            {
                retVal = arrow.Slide(retVal, colliding, direction);
                colliding = AllocatedBoxes.Intersects(arrow.getTextBoundingBox(retVal));
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
        public virtual Point GetNextPosition(BoxModel model)
        {
            Point retVal = new Point(CurrentPosition.X, CurrentPosition.Y);

            // Prepare the next call for GetNextPosition
            int X_OFFSET = model.Width + 10;
            int Y_OFFSET = model.Height + 10;

            CurrentPosition.Offset(X_OFFSET, 0);
            if (CurrentPosition.X > Size.Width - model.Width)
            {
                CurrentPosition = new Point(1, CurrentPosition.Y + Y_OFFSET);
            }

            return retVal;
        }

        private void BoxArrowPanel_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                foreach (BoxControl<BoxModel, ArrowModel> control in boxes.Values)
                {
                    control.PaintInBoxArrowPanel(g);
                }

                foreach (ArrowControl<BoxModel, ArrowModel> control in arrows.Values)
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
        protected BoxArrowWindow<BoxModel, ArrowModel> EnclosingWindow
        {
            get { return GUIUtils.EnclosingFinder<BoxArrowWindow<BoxModel, ArrowModel>>.find(this); }
        }

        /// <summary>
        ///     Selects a model element
        /// </summary>
        /// <param name="model"></param>
        /// <param name="alsoSelectInOtherWindows">
        ///     Indicates that the element should also be selected in the other windows + get
        ///     focus
        /// </param>
        public void Select(object model, bool alsoSelectInOtherWindows)
        {
            if (EnclosingWindow != null)
            {
                EnclosingWindow.Select(model, alsoSelectInOtherWindows);
            }
            else if (alsoSelectInOtherWindows)
            {
                if (model is BoxControl<BoxModel, ArrowModel>)
                {
                    BoxControl<BoxModel, ArrowModel> control = model as BoxControl<BoxModel, ArrowModel>;
                    GUIUtils.MDIWindow.Select(control.Model);
                }
                else if (model is ArrowControl<BoxModel, ArrowModel>)
                {
                    ArrowControl<BoxModel, ArrowModel> control = model as ArrowControl<BoxModel, ArrowModel>;
                    GUIUtils.MDIWindow.Select(control.Model.ReferencedModel);
                }
            }

            Refresh();
        }

        /// <summary>
        ///     Indicates whether the control is selected
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        internal bool isSelected(Control control)
        {
            bool retVal = false;

            if (EnclosingWindow != null)
            {
                retVal = EnclosingWindow.isSelected(control);
            }

            return retVal;
        }
    }
}