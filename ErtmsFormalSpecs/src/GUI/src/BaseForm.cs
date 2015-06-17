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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI
{
    public interface IBaseForm
    {
        /// <summary>
        ///     The property grid used to edit elements properties
        /// </summary>
        MyPropertyGrid Properties { get; }

        /// <summary>
        ///     The requirements text box used to display the associated requirements
        /// </summary>
        EditorTextBox RequirementsTextBox { get; }

        /// <summary>
        ///     The text box used to edit expression
        /// </summary>
        EditorTextBox ExpressionEditorTextBox { get; }

        /// <summary>
        ///     The main tree view of the form
        /// </summary>
        BaseTreeView TreeView { get; }

        /// <summary>
        ///     The sub tree view of the form
        /// </summary>
        BaseTreeView subTreeView { get; }

        /// <summary>
        ///     The explain text box
        /// </summary>
        ExplainTextBox ExplainTextBox { get; }

        /// <summary>
        ///     Refreshed the view, while no structural model occurred
        /// </summary>
        void Refresh();

        /// <summary>
        ///     Allows to refresh the view, according to the fact that the structure for the model could change
        /// </summary>
        void RefreshModel();

        /// <summary>
        ///     Provides the model element currently selected in this IBaseForm
        /// </summary>
        IModelElement Selected { get; }
    }

    public class BaseForm : DockContent, IBaseForm
    {
        /// <summary>
        ///     The property grid used to edit elements properties
        /// </summary>
        public virtual MyPropertyGrid Properties
        {
            get { return null; }
        }

        /// <summary>
        ///     The requirements text box used to display the associated requirements
        /// </summary>
        public virtual EditorTextBox RequirementsTextBox
        {
            get { return null; }
        }

        /// <summary>
        ///     The text box used to edit expression
        /// </summary>
        public virtual EditorTextBox ExpressionEditorTextBox
        {
            get { return null; }
        }

        /// <summary>
        ///     The main tree view of the form
        /// </summary>
        public virtual BaseTreeView TreeView
        {
            get { return null; }
        }

        /// <summary>
        ///     The sub tree view of the form
        /// </summary>
        public virtual BaseTreeView subTreeView
        {
            get { return null; }
        }

        /// <summary>
        ///     The explain text box
        /// </summary>
        public virtual ExplainTextBox ExplainTextBox
        {
            get { return null; }
        }

        /// <summary>
        ///     Allows to refresh the view, according to the fact that the structure for the model could change
        /// </summary>
        public virtual void RefreshModel()
        {
        }

        /// <summary>
        ///     Provides the model element currently selected in this IBaseForm
        /// </summary>
        public virtual IModelElement Selected
        {
            get
            {
                IModelElement retVal = null;

                if (TreeView != null && TreeView.Selected != null)
                {
                    retVal = TreeView.Selected.Model;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     The thread used to synchronize node names with their model
        /// </summary>
        private class Synchronizer : GenericSynchronizationHandler<BaseForm>
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="instance"></param>
            public Synchronizer(BaseForm instance, int cycleTime)
                : base(instance, cycleTime)
            {
            }

            /// <summary>
            ///     Synchronization
            /// </summary>
            /// <param name="instance"></param>
            public override void HandleSynchronization(BaseForm instance)
            {
                instance.Invoke((MethodInvoker) delegate { instance.SynchronizeForm(); });
            }
        }

        /// <summary>
        ///     Indicates that synchronization is required
        /// </summary>
        private Synchronizer FormSynchronizer { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public BaseForm()
            : base()
        {
            DockAreas = DockAreas.Document;
            FormSynchronizer = new Synchronizer(this, 300);
            ParentChanged += new EventHandler(BaseForm_ParentChanged);
        }

        private void BaseForm_ParentChanged(object sender, EventArgs e)
        {
            FloatWindow window = ParentForm as FloatWindow;
            if (window != null)
            {
                ParentForm.Move += new EventHandler(ParentForm_Move);
            }
        }

        private void ParentForm_Move(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                Hide();
                DockAreas = DockAreas.Document;
                DockState = DockState.Document;
                Show(GUIUtils.MDIWindow.dockPanel, DockState.Document);
            }
        }

        /// <summary>
        ///     Stop the synchronizer when the form is closed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            FormSynchronizer.Stop();
        }

        /// <summary>
        ///     Synchronizes the form with its model
        /// </summary>
        public virtual void SynchronizeForm()
        {
            if (Properties != null)
            {
                if (!Properties.ContainsFocus && !(Properties.ActiveControl is Button))
                {
                    Properties.Refresh();
                }
            }

            if (TreeView != null && TreeView.SelectedNode != null)
            {
                ((BaseTreeNode) TreeView.SelectedNode).RefreshViewAccordingToModel(this, true);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseForm
            // 
            this.ClientSize = new Size(284, 262);
            this.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte) (0)));
            this.Name = "BaseForm";
            this.ResumeLayout(false);
        }

        /// <summary>
        ///     Resizes the description area of the property grid to be as small as possible
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="height"></param>
        protected static void ResizeDescriptionArea(PropertyGrid grid, int height)
        {
            if (grid == null) throw new ArgumentNullException("grid");

            foreach (Control control in grid.Controls)
            {
                if (control.GetType().Name == "DocComment")
                {
                    FieldInfo fieldInfo = control.GetType().BaseType.GetField("userSized",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);
                    fieldInfo.SetValue(control, true);
                    control.Height = height;
                    return;
                }
            }
        }
    }

    public class FormsUtils
    {
        public static Form EnclosingForm(Control control)
        {
            while (control != null && !(control is Form))
            {
                control = control.Parent;
            }
            return control as Form;
        }

        public static IBaseForm EnclosingIBaseForm(Control control)
        {
            while (control != null && !(control is IBaseForm))
            {
                control = control.Parent;
            }
            return control as IBaseForm;
        }
    }
}