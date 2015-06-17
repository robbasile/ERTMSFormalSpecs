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
    public partial class RequirementSetDependancyControl : ArrowControl<RequirementSet, RequirementSetDependancy>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public RequirementSetDependancyControl()
            : base()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public RequirementSetDependancyControl(IContainer container)
            : base()
        {
            container.Add(this);
        }
    }
}