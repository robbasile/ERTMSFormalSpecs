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
using ModelElement = DataDictionary.ModelElement;

namespace GUI.MoreInfoView
{
    public partial class Window : BaseForm
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();

            moreInfoRichTextBox.ReadOnly = true;
            moreInfoRichTextBox.Enabled = true;

            DockAreas = DockAreas.DockBottom;
        }

        /// <summary>
        ///     Indicates that the model element should be displayed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <returns></returns>
        protected override bool ShouldDisplay(IModelElement modelElement)
        {
            return true;
        }

        /// <summary>
        ///     Allows to refresh the view, when the selected model changed
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if refresh should be performed</returns>
        public override bool HandleSelectionChange(Context.SelectionContext context)
        {
            bool retVal = base.HandleSelectionChange(context);

            if (retVal)
            {
                SetMoreInfo();
            }

            return retVal;
        }

        /// <summary>
        ///     Sets the more information according to the displayed model
        /// </summary>
        private void SetMoreInfo()
        {
            ModelElement.DontRaiseError(() =>
            {
                moreInfoRichTextBox.Text = "";
                ITextualExplain textualExplain = DisplayedModel as ITextualExplain;
                if (textualExplain != null)
                {
                    moreInfoRichTextBox.Instance = DisplayedModel;
                    moreInfoRichTextBox.Text = TextualExplanationUtils.GetText(textualExplain, true);
                }
                Refresh();
            });
        }

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
                SetMoreInfo();
            }

            return retVal;
        }
    }
}