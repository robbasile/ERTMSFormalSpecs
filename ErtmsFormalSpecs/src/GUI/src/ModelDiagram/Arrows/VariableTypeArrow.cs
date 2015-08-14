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
using DataDictionary.Types;
using DataDictionary.Variables;

namespace GUI.ModelDiagram.Arrows
{
    /// <summary>
    ///     An arrow for a variable
    /// </summary>
    public class VariableTypeArrow : ModelArrow
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="model"></param>
        public VariableTypeArrow(Variable source, Type target, Variable model)
            : base (source, target, "type", model)
        {
        }

        /// <summary>
        ///     Sets the source box for this arrow
        /// </summary>
        /// <param name="initialBox"></param>
        public override void SetInitialBox(IGraphicalDisplay initialBox)
        {
            Variable variable = initialBox as Variable;
            Type type = Target as Type;
            if (variable != null && type != null)
            {
                variable.Type = type;
                Source = initialBox;
            }
        }

        /// <summary>
        ///     Sets the target box for this arrow
        /// </summary>
        /// <param name="targetBox"></param>
        public override void SetTargetBox(IGraphicalDisplay targetBox)
        {
            Variable variable = Source as Variable;
            Type type = targetBox as Type;
            if (variable != null && type != null)
            {
                variable.Type = type;
                Target = targetBox;
            }
        }
    }
}
