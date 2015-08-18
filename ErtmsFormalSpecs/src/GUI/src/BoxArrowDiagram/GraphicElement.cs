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
using Utils;

namespace GUI.BoxArrowDiagram
{
    /// <summary>
    ///     A thing that is displayed
    /// </summary>
    public abstract class GraphicElement
    {
        /// <summary>
        ///     The base class for graphic elements
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="model"></param>
        protected GraphicElement(Panel panel, object model)
        {
            Panel = panel;
            Model = model;

            Visible = true;
            Font = new Font(new FontFamily("Microsoft Sans Serif"), 8.25F);
            BackColor = Color.Transparent;
            ForeColor = Color.Black;
        }

        /// <summary>
        ///     The panel in which this graphic element lies
        /// </summary>
        private Panel Panel { get; set; }

        /// <summary>
        ///     The model on which this element is related
        /// </summary>
        public virtual object Model { get; set; }

        /// <summary>
        ///     The location of the element
        /// </summary>
        public abstract Point Location { get; set; }

        /// <summary>
        ///     The size of the element
        /// </summary>
        public abstract Size Size { get; set; }

        /// <summary>
        ///     The rectangle where the graphical element is displayed
        /// </summary>
        public Rectangle Rectangle
        {
            get { return new Rectangle(Location, Size); }
        }

        /// <summary>
        ///     The width of the arrow
        /// </summary>
        public int Width
        {
            get { return Size.Width; }
        }

        /// <summary>
        ///     The Height of the arrow
        /// </summary>
        public int Height
        {
            get { return Size.Height; }
        }

        /// <summary>
        ///     Indicates that the element is visible
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        ///     Handles a mouse click event on a graphical element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        public virtual void HandleClick(object sender, MouseEventArgs mouseEventArgs)
        {
            IModelElement model = Model as IModelElement;
            if (model != null)
            {
                Context.SelectionCriteria criteria = GuiUtils.SelectionCriteriaBasedOnMouseEvent(mouseEventArgs);
                EFSSystem.INSTANCE.Context.SelectElement(model, Panel, criteria);
            }
        }

        /// <summary>
        ///     Handles a mouse double click event on a graphical element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        public virtual void HandleDoubleClick(object sender, MouseEventArgs mouseEventArgs)
        {
            IModelElement model = Model as IModelElement;
            if (model != null)
            {
                Context.SelectionCriteria criteria = GuiUtils.SelectionCriteriaBasedOnMouseEvent(mouseEventArgs);
                EFSSystem.INSTANCE.Context.SelectElement(model, Panel, criteria);
            }
        }

        /// <summary>
        ///     Handles a mouse down event on a graphical element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        public virtual void HandleMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
        }

        /// <summary>
        ///     Handles a mouse down event on a graphical element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        public virtual void HandleMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
        }

        /// <summary>
        ///     Handles a mouse down event on a graphical element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        public virtual void HandleMouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
        }

        /// <summary>
        ///     The font used to display text
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        ///     The color used for the text and foreground
        /// </summary>
        public Color ForeColor { get; set; }

        /// <summary>
        ///     The color used for the background
        /// </summary>
        public Color BackColor { get; set; }

        /// <summary>
        ///     Selects the pen according to the graphic element status
        /// </summary>
        /// <returns></returns>
        public abstract Pen SelectPen();
    }
}