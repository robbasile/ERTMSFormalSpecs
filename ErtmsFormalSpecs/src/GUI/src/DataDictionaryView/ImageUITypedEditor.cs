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
using DataDictionary.Types;
using GUIUtils.Images;

namespace GUI.DataDictionaryView
{
    /// <summary>
    ///     Allow to enter an expresion
    /// </summary>
    public class ImageUITypedEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            NameSpaceTreeNode.Editor editor = context.Instance as NameSpaceTreeNode.Editor;
            if (editor != null)
            {
                NameSpace nameSpace = editor.Item;
                if (nameSpace != null)
                {
                    ImagesSelector selector = new ImagesSelector(nameSpace);
                    selector.ShowDialog();
                }
            }

            return value;
        }
    }
}
