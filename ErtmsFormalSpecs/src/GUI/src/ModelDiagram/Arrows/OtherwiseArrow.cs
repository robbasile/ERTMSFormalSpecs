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
    ///     An arrow
    /// </summary>
    public class OthewiseArrow : ModelArrow
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public OthewiseArrow(IGraphicalDisplay source, IGraphicalDisplay target)
            : base(source, target, "", null)
        {
        }

        /// <summary>
        ///     Sets the source box for this arrow
        /// </summary>
        /// <param name="initialBox"></param>
        public override void SetInitialBox(IGraphicalDisplay initialBox)
        {
        }

        /// <summary>
        ///     Sets the target box for this arrow
        /// </summary>
        /// <param name="targetBox"></param>
        public override void SetTargetBox(IGraphicalDisplay targetBox)
        {
        }
    }
}