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

using System.ComponentModel;
using System.Drawing;
using DataDictionary;

namespace GUI.BoxArrowDiagram
{
    /// <summary>
    ///     A box editor
    /// </summary>
    public class BoxEditor<TEnclosing, TBoxModel, TArrowModel>
        where TEnclosing : class
        where TBoxModel : class, IGraphicalDisplay
        where TArrowModel : class, IGraphicalArrow<TBoxModel>
    {
        public BoxControl<TEnclosing, TBoxModel, TArrowModel> Control;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="control"></param>
        public BoxEditor(BoxControl<TEnclosing, TBoxModel, TArrowModel> control)
        {
            Control = control;
        }

        [Category("Description")]
        public virtual string Name
        {
            get { return Control.TypedModel.GraphicalName; }
            set { Control.TypedModel.Name = value; }
        }

        [Category("Description")]
        public Point Position
        {
            get { return new Point(Control.TypedModel.X, Control.TypedModel.Y); }
            set
            {
                Control.TypedModel.X = value.X;
                Control.TypedModel.Y = value.Y;
                if (Control.Panel != null)
                {
                    Control.Panel.UpdateArrowPosition();
                }
            }
        }

        [Category("Description")]
        public Point Size
        {
            get { return new Point(Control.TypedModel.Width, Control.TypedModel.Height); }
            set
            {
                Control.TypedModel.Width = value.X;
                Control.TypedModel.Height = value.Y;
                if (Control.Panel != null)
                {
                    Control.Panel.UpdateArrowPosition();
                }
            }
        }

        [Category("Description")]
        public bool Hidden
        {
            get { return Control.TypedModel.Hidden; }
            set
            {
                Control.TypedModel.Hidden = value;
                if (Control.Panel != null)
                {
                    Control.Panel.RefreshControl();
                }
            }
        }
    }
}