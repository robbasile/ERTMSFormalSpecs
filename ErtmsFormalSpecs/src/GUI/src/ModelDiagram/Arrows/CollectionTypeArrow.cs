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

namespace GUI.ModelDiagram.Arrows
{
    /// <summary>
    ///     An arrow for a collection
    /// </summary>
    public class CollectionTypeArrow : ModelArrow
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="model"></param>
        public CollectionTypeArrow(Collection source, Type target, Collection model)
            : base(source, target, "of " + source.getMaxSize(), model)
        {
        }

        /// <summary>
        ///     Sets the source box for this arrow
        /// </summary>
        /// <param name="initialBox"></param>
        public override void SetInitialBox(IGraphicalDisplay initialBox)
        {
            Collection collection = initialBox as Collection;
            Type type = Target as Type;
            if (collection != null && type != null)
            {
                collection.Type = type;
                Source = initialBox;
            }
        }

        /// <summary>
        ///     Sets the target box for this arrow
        /// </summary>
        /// <param name="targetBox"></param>
        public override void SetTargetBox(IGraphicalDisplay targetBox)
        {
            Collection collection = Source as Collection;
            Type type = targetBox as Type;
            if (collection != null && type != null)
            {
                collection.Type = type;
                Target = targetBox;
            }
        }
    }
}