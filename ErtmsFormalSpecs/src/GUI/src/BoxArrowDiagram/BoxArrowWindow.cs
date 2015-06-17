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
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.BoxArrowDiagram
{
    public abstract partial class BoxArrowWindow<BoxModel, ArrowModel> : DockContent
        where BoxModel : class, IGraphicalDisplay
        where ArrowModel : class, IGraphicalArrow<BoxModel>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public BoxArrowWindow()
        {
            descriptionRichTextBox = new EditorTextBox();
            InitializeComponent();

            FormClosed += new FormClosedEventHandler(BoxArrowDiagramWindow_FormClosed);
            splitContainer1.FixedPanel = FixedPanel.Panel2;

            DockAreas = DockAreas.Document;
        }

        /// <summary>
        ///     Method used to create a panel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract BoxArrowPanel<BoxModel, ArrowModel> createPanel();

        private void BoxArrowDiagramWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            GUIUtils.MDIWindow.HandleSubWindowClosed(this);
        }

        /// <summary>
        ///     A box editor
        /// </summary>
        protected class BoxEditor
        {
            public BoxControl<BoxModel, ArrowModel> control;

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="control"></param>
            public BoxEditor(BoxControl<BoxModel, ArrowModel> control)
            {
                this.control = control;
            }

            [Category("Description")]
            public virtual string Name
            {
                get { return control.Model.GraphicalName; }
                set
                {
                    control.Model.Name = value;
                    control.RefreshControl();
                }
            }

            [Category("Description")]
            public Point Position
            {
                get { return new Point(control.Model.X, control.Model.Y); }
                set
                {
                    control.Model.X = value.X;
                    control.Model.Y = value.Y;
                    control.RefreshControl();
                    if (control.Panel != null)
                    {
                        control.Panel.UpdateArrowPosition();
                    }
                }
            }

            [Category("Description")]
            public Point Size
            {
                get { return new Point(control.Model.Width, control.Model.Height); }
                set
                {
                    control.Model.Width = value.X;
                    control.Model.Height = value.Y;
                    control.RefreshControl();
                    if (control.Panel != null)
                    {
                        control.Panel.UpdateArrowPosition();
                    }
                }
            }

            [Category("Description")]
            public bool Hidden
            {
                get { return control.Model.Hidden; }
                set
                {
                    control.Model.Hidden = value;
                    if (control.Panel != null)
                    {
                        control.Panel.RefreshControl();
                    }
                }
            }
        }

        /// <summary>
        ///     Factory for BoxEditor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected virtual BoxEditor createBoxEditor(BoxControl<BoxModel, ArrowModel> control)
        {
            BoxEditor retVal = new BoxEditor(control);

            return retVal;
        }

        /// <summary>
        ///     An arrow editor
        /// </summary>
        protected class ArrowEditor
        {
            public ArrowControl<BoxModel, ArrowModel> control;

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="control"></param>
            public ArrowEditor(ArrowControl<BoxModel, ArrowModel> control)
            {
                this.control = control;
            }

            [Category("Description")]
            public string Name
            {
                get { return control.Model.GraphicalName; }
            }
        }

        /// <summary>
        ///     Factory for arrow editor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected virtual ArrowEditor createArrowEditor(ArrowControl<BoxModel, ArrowModel> control)
        {
            ArrowEditor retVal = new ArrowEditor(control);

            return retVal;
        }

        /// <summary>
        ///     The selected object
        /// </summary>
        protected object Selected { get; set; }

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
            Selected = model;
            BoxArrowContainerPanel.Selected = model;
            if (model is BoxControl<BoxModel, ArrowModel>)
            {
                BoxControl<BoxModel, ArrowModel> control = model as BoxControl<BoxModel, ArrowModel>;
                propertyGrid.SelectedObject = createBoxEditor(control);
                descriptionRichTextBox.ResetText();
                descriptionRichTextBox.Rtf = control.Model.getExplain(false);
                if (alsoSelectInOtherWindows)
                {
                    GUIUtils.MDIWindow.Select(control.Model);
                }
            }
            else if (model is ArrowControl<BoxModel, ArrowModel>)
            {
                ArrowControl<BoxModel, ArrowModel> control = model as ArrowControl<BoxModel, ArrowModel>;
                propertyGrid.SelectedObject = createArrowEditor(control);
                descriptionRichTextBox.ResetText();
                if (control.Model.ReferencedModel != null)
                {
                    TextualExplain explainable = control.Model.ReferencedModel as TextualExplain;
                    if (explainable != null)
                    {
                        descriptionRichTextBox.Rtf = explainable.getExplain(true);
                    }
                    else
                    {
                        descriptionRichTextBox.Rtf = "";
                    }
                }
                else
                {
                    descriptionRichTextBox.Rtf = "";
                }
                if (alsoSelectInOtherWindows)
                {
                    GUIUtils.MDIWindow.Select(control.Model.ReferencedModel);
                }
            }
            else
            {
                propertyGrid.SelectedObject = null;
            }
        }

        /// <summary>
        ///     Indicates whether the control is selected
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        internal bool isSelected(Control control)
        {
            return control == Selected;
        }

        public void RefreshAfterStep()
        {
            BoxArrowContainerPanel.Refresh();
        }
    }
}