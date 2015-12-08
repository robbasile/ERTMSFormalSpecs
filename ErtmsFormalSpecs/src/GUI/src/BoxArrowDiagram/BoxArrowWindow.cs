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
using Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace GUI.BoxArrowDiagram
{
    public abstract partial class BoxArrowWindow<TEnclosing, TBoxModel, TArrowModel> : BaseForm
        where TEnclosing : class
        where TBoxModel : class, IGraphicalDisplay
        where TArrowModel : class, IGraphicalArrow<TBoxModel>
    {
        /// <summary>
        ///     The enclosing model
        /// </summary>
        protected virtual TEnclosing Model { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected BoxArrowWindow()
        {
            InitializeComponent();

            DockAreas = DockAreas.Document;
        }

        /// <summary>
        ///     Method used to create a panel
        /// </summary>
        /// <returns></returns>
        public abstract BoxArrowPanel<TEnclosing, TBoxModel, TArrowModel> CreatePanel();

        /// <summary>
        ///     Allows to refresh the view, when the value of a model changed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns>True if the view should be refreshed</returns>
        public override bool HandleValueChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            bool retVal = base.HandleValueChange(modelElement, changeKind);

            if (retVal)
            {
                if (modelElement == null)
                {
                    BoxArrowContainerPanel.RefreshControl();
                }
                else
                {
                    TEnclosing enclosing = EnclosingFinder<TEnclosing>.find(modelElement, true);
                    while (enclosing != null)
                    {
                        if (enclosing == Model)
                        {
                            BoxArrowContainerPanel.RefreshControl();
                            enclosing = null;
                        }
                        else
                        {
                            enclosing = EnclosingFinder<TEnclosing>.find(enclosing as IEnclosed, false);
                        }
                    }
                }
            }

            if (changeKind == Context.ChangeKind.EndOfCycle)
            {
                BoxArrowContainerPanel.Refresh();
            }

            return retVal;
        }
    }
}