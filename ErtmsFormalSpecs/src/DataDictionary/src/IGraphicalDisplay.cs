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

using Utils;

namespace DataDictionary
{
    /// <summary>
    ///     Something that can be graphically displayed
    /// </summary>
    public interface IGraphicalDisplay : IModelElement, INamable, TextualExplain
    {
        /// <summary>
        ///     The X position
        /// </summary>
        int X { get; set; }

        /// <summary>
        ///     The Y position
        /// </summary>
        int Y { get; set; }

        /// <summary>
        ///     The width
        /// </summary>
        int Width { get; set; }

        /// <summary>
        ///     The height
        /// </summary>
        int Height { get; set; }

        /// <summary>
        ///     The Guid of the graphical display
        /// </summary>
        string Guid { get; }

        /// <summary>
        ///     The name to be displayed
        /// </summary>
        string GraphicalName { get; }

        /// <summary>
        ///     Indicates that the element is hidden
        /// </summary>
        bool Hidden { get; set; }

        /// <summary>
        ///     Indicates that the element is pinned
        /// </summary>
        bool Pinned { get; set; }
    }

    /// <summary>
    ///     Something that can be graphically displayed
    /// </summary>
    public interface IGraphicalArrow<Ending>
        where Ending : IGraphicalDisplay
    {
        /// <summary>
        ///     The source of the arrow
        /// </summary>
        Ending Source { get; }

        /// <summary>
        ///     Sets the source box for this arrow
        /// </summary>
        /// <param name="initialBox"></param>
        void SetInitialBox(IGraphicalDisplay initialBox);

        /// <summary>
        ///     The target of the arrow
        /// </summary>
        Ending Target { get; }

        /// <summary>
        ///     Sets the target box for this arrow
        /// </summary>
        /// <param name="targetBox"></param>
        void SetTargetBox(IGraphicalDisplay targetBox);

        /// <summary>
        ///     The name to be displayed
        /// </summary>
        string GraphicalName { get; }

        /// <summary>
        ///     The model element which is referenced by this arrow
        /// </summary>
        ModelElement ReferencedModel { get; }
    }
}