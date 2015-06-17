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
using DataDictionary.Variables;
using GUI.BoxArrowDiagram;

namespace GUI.ModelDiagram
{
    /// <summary>
    ///     An arrow
    /// </summary>
    public class ModelArrow : IGraphicalArrow<IGraphicalDisplay>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="name"></param>
        /// <param name="model"></param>
        public ModelArrow(IGraphicalDisplay source, IGraphicalDisplay target, string name, ModelElement model)
        {
            Source = source;
            Target = target;
            GraphicalName = name;
            ReferencedModel = model;
        }

        /// <summary>
        ///     The source of the arrow
        /// </summary>
        public IGraphicalDisplay Source { get; private set; }

        /// <summary>
        ///     Sets the source box for this arrow
        /// </summary>
        /// <param name="initialBox"></param>
        public void SetInitialBox(IGraphicalDisplay initialBox)
        {
            Source = initialBox;
        }

        /// <summary>
        ///     The target of the arrow
        /// </summary>
        public IGraphicalDisplay Target { get; private set; }

        /// <summary>
        ///     Sets the target box for this arrow
        /// </summary>
        /// <param name="targetBox"></param>
        public void SetTargetBox(IGraphicalDisplay targetBox)
        {
            Target = targetBox;
        }

        /// <summary>
        ///     The name to be displayed
        /// </summary>
        public string GraphicalName { get; private set; }

        /// <summary>
        ///     The model element which is referenced by this arrow
        /// </summary>
        public ModelElement ReferencedModel { get; private set; }
    }

    /// <summary>
    ///     An arrow between the models
    /// </summary>
    public class ModelArrowControl : ArrowControl<IGraphicalDisplay, ModelArrow>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="model"></param>
        public ModelArrowControl(ModelArrow model)
        {
            Model = model;
            DEFAULT_ARROW_LENGTH = 30;

            if (Model.Source is Variable)
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