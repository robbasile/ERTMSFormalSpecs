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
using DataDictionary.Tests;
using GUI.EditorView;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.Converters
{
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class ExpressionableUITypedEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc =
                provider.GetService(typeof (IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            if (svc != null)
            {
                IExpressionable expressionable = value as IExpressionable;
                if (expressionable != null)
                {
                    Window form = new Window();
                    ExpressionableTextChangeHandler handler =
                        new ExpressionableTextChangeHandler(expressionable as ModelElement);
                    form.setChangeHandler(handler);
                    GUIUtils.MDIWindow.AddChildWindow(form, DockAreas.Float);
                }
            }

            return value;
        }
    }

    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class ConditionUITypedEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc =
                provider.GetService(typeof (IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            if (svc != null)
            {
                Expectation expectation = value as Expectation;
                if (expectation != null)
                {
                    Window form = new Window();
                    ConditionTextChangeHandler handler = new ConditionTextChangeHandler(expectation);
                    form.setChangeHandler(handler);
                    GUIUtils.MDIWindow.AddChildWindow(form, DockAreas.Float);
                }
            }

            return value;
        }
    }
}