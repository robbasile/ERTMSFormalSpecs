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

using DataDictionary.Types;

namespace GUI.ModelDiagram.Boxes
{
    /// <summary>
    ///     The boxes that represent a range
    /// </summary>
    public class RangeModelControl : TypeModelControl
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="model"></param>
        public RangeModelControl(ModelDiagramPanel panel, Range model)
            : base(panel, model)
        {
        }

        /// <summary>
        ///     The name of the kind of type
        /// </summary>
        public override string ModelName
        {
            get
            {
                string retVal = "Range";

                if (ComputedPositionAndSize)
                {
                    retVal = "Range : " + TypedModel.GraphicalName;
                }

                return retVal;
            }
        }
    }
}