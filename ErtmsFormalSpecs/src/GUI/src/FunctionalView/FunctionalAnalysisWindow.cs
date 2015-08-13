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
using DataDictionary.Types.AccessMode;
using GUI.BoxArrowDiagram;

namespace GUI.FunctionalView
{
    public class FunctionalAnalysisWindow : BoxArrowWindow<IEnclosesNameSpaces, NameSpace, AccessMode>
    {
        /// <summary>
        ///     The panel used to display the state diagram
        /// </summary>
        private FunctionalAnalysisPanel FunctionalAnalysisPanel
        {
            get { return (FunctionalAnalysisPanel) BoxArrowContainerPanel; }
        }

        /// <summary>
        ///     Sets the state machine type
        /// </summary>
        public void SetNameSpaceContainer(IEnclosesNameSpaces nameSpaceContainer)
        {
            Model = nameSpaceContainer;

            FunctionalAnalysisPanel.NameSpaceContainer = Model;
            FunctionalAnalysisPanel.RefreshControl();
        }

        public override BoxArrowPanel<IEnclosesNameSpaces, NameSpace, AccessMode> CreatePanel()
        {
            return new FunctionalAnalysisPanel();
        }
    }
}