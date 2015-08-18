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
using DataDictionary;

namespace GUI.BoxArrowDiagram
{
    /// <summary>
    ///     An arrow editor
    /// </summary>
    public class ArrowEditor<TEnclosing, TBoxModel, TArrowModel>
        where TEnclosing : class
        where TBoxModel : class, IGraphicalDisplay
        where TArrowModel : class, IGraphicalArrow<TBoxModel>
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
            get { return Control.TypedModel.GraphicalName; }
        }
    }
}