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
    public class FunctionalAnalysisPanel : BoxArrowPanel<NameSpace, AccessMode>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public FunctionalAnalysisPanel()
            : base()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public FunctionalAnalysisPanel(IContainer container)
            : base()
        {
            container.Add(this);
        }

        /// <summary>
        ///     Method used to create a box
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override BoxControl<NameSpace, AccessMode> createBox(NameSpace model)
        {
            BoxControl<NameSpace, AccessMode> retVal = new FunctionalBlockControl();
            retVal.Model = model;

            return retVal;
        }

        /// <summary>
        ///     Method used to create an arrow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ArrowControl<NameSpace, AccessMode> createArrow(AccessMode model)
        {
            ArrowControl<NameSpace, AccessMode> retVal = new AccessToControl();
            retVal.Model = model;

            return retVal;
        }

        /// <summary>
        ///     The namespace displayed by this panel
        /// </summary>
        public IEnclosesNameSpaces NameSpaceContainer { get; set; }

        /// <summary>
        ///     Provides the boxes that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<NameSpace> getBoxes()
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
        public override List<AccessMode> getArrows()
        {
            return IEnclosesNameSpacesUtils.getAccesses(NameSpaceContainer.EFSSystem, NameSpaceContainer);
        }
    }
}