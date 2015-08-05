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
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DataDictionary;
using GUI.EditorView;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.Converters
{
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class CommentableUITypedEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        ///     Sets the string value into the right property
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        private void HandleTextChange(ModelElement instance, string value)
        {
            ICommentable commentable = instance as ICommentable;

            if (commentable != null)
            {
                commentable.Comment = value;
            }
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc =
                provider.GetService(typeof (IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            if (svc != null)
            {
                ICommentable commentable = value as ICommentable;
                if (commentable != null)
                {
                    Window form = new Window();
                    form.DockAreas = DockAreas.Float;
                    form.AutoComplete = false;
                    CommentableTextChangeHandler handler = new CommentableTextChangeHandler(commentable as ModelElement);
                    form.setChangeHandler(handler);
                    GuiUtils.MdiWindow.AddChildWindow(form, DockAreas.Float);
                }
            }

            return value;
        }
    }
}