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
using DataDictionary.Specification;
using GUI.BoxArrowDiagram;

namespace GUI.RequirementSetDiagram
{
    /// <summary>
    ///     A box editor
    /// </summary>
    public class RequirementSetEditor : BoxEditor<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="control"></param>
        public RequirementSetEditor(BoxControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> control)
            : base(control)
        {
        }

        [Category("Related Requirements behaviour")]
        public bool Recursive
        {
            get { return Control.TypedModel.getRecursiveSelection(); }
            set { Control.TypedModel.setRecursiveSelection(value); }
        }

        [Category("Related Requirements behaviour")]
        public bool Default
        {
            get { return Control.TypedModel.getDefault(); }
            set { Control.TypedModel.setDefault(value); }
        }
    }
}
