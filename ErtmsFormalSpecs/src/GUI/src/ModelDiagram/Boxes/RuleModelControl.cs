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

using System.Drawing;
using DataDictionary.Rules;

namespace GUI.ModelDiagram.Boxes
{
    /// <summary>
    ///     The boxes that represent a rule
    /// </summary>
    public class RuleModelControl : ModelControl
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="model"></param>
        public RuleModelControl(ModelDiagramPanel panel, Rule model)
            : base(panel, model)
        {
            BoxMode = BoxModeEnum.RoundedCorners;
            NormalColor = Color.LightBlue;
        }

        /// <summary>
        ///     The name of the kind of model
        /// </summary>
        public override string ModelName
        {
            get
            {
                string retVal = "Rule";

                if (ComputedPositionAndSize)
                {
                    retVal = "  Rule : " + TypedModel.GraphicalName;
                }

                return retVal;
            }
        }
    }
}