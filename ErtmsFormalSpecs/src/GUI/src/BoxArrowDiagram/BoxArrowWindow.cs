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
using System.Windows.Forms;
using DataDictionary;
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.BoxArrowDiagram
{
    public abstract partial class BoxArrowWindow<TEnclosing, TBoxModel, TArrowModel> : BaseBoxArrowWindow
        where TEnclosing : class
        where TBoxModel : class, IGraphicalDisplay
        where TArrowModel : class, IGraphicalArrow<TBoxModel>
    {
        /// <summary>
        /// The enclosing model
        /// </summary>
        protected TEnclosing Model { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected BoxArrowWindow()
        {
            InitializeComponent();

            DockAreas = DockAreas.Document;
        }

        /// <summary>
        ///     The selected control 
        /// </summary>
        protected Control Selected { get; set; }

        /// <summary>
        ///     Indicates whether the control is selected
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool IsSelected(Control control)
        {
            return control == Selected;
        }

        /// <summary>
        /// Provides the box control which corresponds to the model provided as parameter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected BoxControl<TEnclosing, TBoxModel, TArrowModel> BoxForModel(IModelElement model)
        {
            BoxControl<TEnclosing, TBoxModel, TArrowModel> retVal = null;

            TBoxModel boxModel = model as TBoxModel;
            if (boxModel != null)
            {
                retVal = BoxArrowContainerPanel.GetBoxControl(boxModel);
            }

            return retVal;
        }

        /// <summary>
        /// Provides the arrow control which corresponds to the model provided as parameter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected ArrowControl<TEnclosing, TBoxModel, TArrowModel> ArrowForModel(IModelElement model)
        {
            ArrowControl<TEnclosing, TBoxModel, TArrowModel> retVal = null;

            TArrowModel arrowModel = model as TArrowModel;
            if (arrowModel != null)
            {
                retVal = BoxArrowContainerPanel.GetArrowControl(arrowModel);
            }

            return retVal;
        }

        /// <summary>
        /// Creates the editor for the selected object
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override object CreateEditor(IModelElement model)
        {
            object retVal = base.CreateEditor(model);

            if (retVal == null)
            {
                BoxControl<TEnclosing, TBoxModel, TArrowModel> boxControl = BoxForModel(model);
                if (boxControl != null)
                {
                    Selected = boxControl;
                    retVal = CreateBoxEditor(boxControl);
                }

                ArrowControl<TEnclosing, TBoxModel, TArrowModel> arrowControl = ArrowForModel(model);
                if (arrowControl != null)
                {
                    Selected = arrowControl;
                    retVal = CreateArrowEditor(arrowControl);
                }
            }
            return retVal;
        }

        /// <summary>
        ///     Allows to refresh the view, when the selected model changed
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if refresh should be performed</returns>
        public override bool HandleSelectionChange(Context.SelectionContext context)
        {
            bool retVal = base.HandleSelectionChange(context);

            BoxControl<TEnclosing, TBoxModel, TArrowModel> boxControl = BoxForModel(context.Element);
            if (boxControl != null)
            {
                Selected = boxControl;
            }

            ArrowControl<TEnclosing, TBoxModel, TArrowModel> arrowControl = ArrowForModel(context.Element);
            if (arrowControl != null)
            {
                Selected = arrowControl;
            }

            return retVal;
        }

        /// <summary>
        ///     Method used to create a panel
        /// </summary>
        /// <returns></returns>
        public abstract BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel> CreatePanel();

        /// <summary>
        ///     A box editor
        /// </summary>
        protected class BoxEditor
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
                get { return Control.Model.GraphicalName; }
                set
                {
                    Control.Model.Name = value;
                    Control.RefreshControl();
                }
            }

            [Category("Description")]
            public Point Position
            {
                get { return new Point(Control.Model.X, Control.Model.Y); }
                set
                {
                    Control.Model.X = value.X;
                    Control.Model.Y = value.Y;
                    Control.RefreshControl();
                    if (Control.Panel != null)
                    {
                        Control.Panel.UpdateArrowPosition();
                    }
                }
            }

            [Category("Description")]
            public Point Size
            {
                get { return new Point(Control.Model.Width, Control.Model.Height); }
                set
                {
                    Control.Model.Width = value.X;
                    Control.Model.Height = value.Y;
                    Control.RefreshControl();
                    if (Control.Panel != null)
                    {
                        Control.Panel.UpdateArrowPosition();
                    }
                }
            }

            [Category("Description")]
            public bool Hidden
            {
                get { return Control.Model.Hidden; }
                set
                {
                    Control.Model.Hidden = value;
                    if (Control.Panel != null)
                    {
                        Control.Panel.RefreshControl();
                    }
                }
            }
        }

        /// <summary>
        ///     Factory for BoxEditor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected virtual BoxEditor CreateBoxEditor(BoxControl<TEnclosing, TBoxModel, TArrowModel> control)
        {
            BoxEditor retVal = new BoxEditor(control);

            return retVal;
        }

        /// <summary>
        ///     An arrow editor
        /// </summary>
        protected class ArrowEditor
        {
            public ArrowControl<TEnclosing, TBoxModel, TArrowModel> Control;

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="control"></param>
            public ArrowEditor(ArrowControl<TEnclosing, TBoxModel, TArrowModel> control)
            {
                Control = control;
            }

            [Category("Description")]
            public string Name
            {
                get { return Control.Model.GraphicalName; }
            }
        }

        /// <summary>
        ///     Factory for arrow editor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected virtual ArrowEditor CreateArrowEditor(ArrowControl<TEnclosing, TBoxModel, TArrowModel> control)
        {
            ArrowEditor retVal = new ArrowEditor(control);

            return retVal;
        }

        /// <summary>
        ///     Allows to refresh the view, when the value of a model changed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns>True if the view should be refreshed</returns>
        public override bool HandleValueChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            bool retVal = base.HandleValueChange(modelElement, changeKind);

            if (retVal)
            {
                if (modelElement == Model)
                {
                    BoxArrowContainerPanel.RefreshControl();
                }
            }

            return retVal;
        }
    }
}