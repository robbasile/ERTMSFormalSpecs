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

using System.Collections.Generic;
using System.ComponentModel;
using DataDictionary;
using DataDictionary.Types;
using DataDictionary.Types.AccessMode;
using GUI.BoxArrowDiagram;

namespace GUI.FunctionalView
{
    public class FunctionalAnalysisPanel : BoxArrowPanel<IEnclosesNameSpaces, NameSpace, AccessMode>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public FunctionalAnalysisPanel()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public FunctionalAnalysisPanel(IContainer container)
        {
            container.Add(this);
        }

        /// <summary>
        ///     Method used to create a box
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override BoxControl<IEnclosesNameSpaces, NameSpace, AccessMode> CreateBox(NameSpace model)
        {
            return new FunctionalBlockControl(this, model);
        }

        /// <summary>
        ///     Method used to create an arrow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ArrowControl<IEnclosesNameSpaces, NameSpace, AccessMode> CreateArrow(AccessMode model)
        {
            return new AccessToControl(this, model);
        }

        /// <summary>
        ///     The namespace displayed by this panel
        /// </summary>
        public IEnclosesNameSpaces NameSpaceContainer { get; set; }

        /// <summary>
        ///     Provides the boxes that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<NameSpace> GetBoxes()
        {
            List<NameSpace> retVal = new List<NameSpace>();

            foreach (NameSpace nameSpace in NameSpaceContainer.NameSpaces)
            {
                retVal.Add(nameSpace);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the arrows that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<AccessMode> GetArrows()
        {
            return IEnclosesNameSpacesUtils.getAccesses(NameSpaceContainer.EFSSystem, NameSpaceContainer);
        }

        /// <summary>
        ///     Factory for BoxEditor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override BoxEditor<IEnclosesNameSpaces, NameSpace, AccessMode> CreateBoxEditor(
            BoxControl<IEnclosesNameSpaces, NameSpace, AccessMode> control)
        {
            return new NameSpaceEditor(control);
        }

        /// <summary>
        ///     Factory for arrow editor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override ArrowEditor<IEnclosesNameSpaces, NameSpace, AccessMode> CreateArrowEditor(
            ArrowControl<IEnclosesNameSpaces, NameSpace, AccessMode> control)
        {
            return new AccessModeEditor(control);
        }
    }
}