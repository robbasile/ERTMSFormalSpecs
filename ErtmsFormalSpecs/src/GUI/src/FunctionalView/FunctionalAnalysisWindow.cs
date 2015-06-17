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

using System.ComponentModel;
using DataDictionary;
using DataDictionary.Types;
using DataDictionary.Types.AccessMode;
using GUI.BoxArrowDiagram;

namespace GUI.FunctionalView
{
    public partial class FunctionalAnalysisWindow : BoxArrowWindow<NameSpace, AccessMode>
    {
        /// <summary>
        ///     The namespace currently displayed
        /// </summary>
        public IEnclosesNameSpaces NameSpaceContainer { get; private set; }

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public FunctionalAnalysisWindow()
            : base()
        {
        }

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
        /// <param name="stateMachine"></param>
        public void SetNameSpaceContainer(IEnclosesNameSpaces nameSpaceContainer)
        {
            NameSpaceContainer = nameSpaceContainer;

            FunctionalAnalysisPanel.NameSpaceContainer = NameSpaceContainer;
            FunctionalAnalysisPanel.RefreshControl();
        }

        public override BoxArrowPanel<NameSpace, AccessMode> createPanel()
        {
            BoxArrowPanel<NameSpace, AccessMode> retVal = new FunctionalAnalysisPanel();

            return retVal;
        }

        /// <summary>
        ///     A box editor
        /// </summary>
        protected class NameSpaceEditor : BoxEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="control"></param>
            public NameSpaceEditor(BoxControl<NameSpace, AccessMode> control)
                : base(control)
            {
            }

            [Category("Description")]
            [ReadOnly(true)]
            public override string Name
            {
                get { return control.Model.GraphicalName; }
            }
        }

        /// <summary>
        ///     Factory for BoxEditor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override BoxEditor createBoxEditor(BoxControl<NameSpace, AccessMode> control)
        {
            BoxEditor retVal = new NameSpaceEditor(control);

            return retVal;
        }

        /// <summary>
        ///     An arrow editor
        /// </summary>
        protected class AccessModeEditor : ArrowEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="control"></param>
            public AccessModeEditor(ArrowControl<NameSpace, AccessMode> control)
                : base(control)
            {
            }
        }

        /// <summary>
        ///     Factory for arrow editor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override ArrowEditor createArrowEditor(ArrowControl<NameSpace, AccessMode> control)
        {
            ArrowEditor retVal = new AccessModeEditor(control);

            return retVal;
        }
    }
}