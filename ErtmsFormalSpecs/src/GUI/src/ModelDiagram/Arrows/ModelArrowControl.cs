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

using DataDictionary;
using GUI.BoxArrowDiagram;
using Utils;

namespace GUI.ModelDiagram.Arrows
{
    /// <summary>
    ///     An arrow between the models
    /// </summary>
    public class ModelArrowControl : ArrowControl<IModelElement, IGraphicalDisplay, ModelArrow>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="model"></param>
        public ModelArrowControl(ModelDiagramPanel panel, ModelArrow model)
            : base (panel, model)
        {
            DefaultArrowLength = 30;

            if (model is VariableTypeArrow)
            {
                ArrowFill = ArrowFillEnum.Fill;
                ArrowMode = ArrowModeEnum.Full;
            }
            else
            {
                ArrowFill = ArrowFillEnum.Line;
                ArrowMode = ArrowModeEnum.Half;
            }
        }
    }
}