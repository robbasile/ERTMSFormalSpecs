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
using GUI.EditorView;
using Utils;

namespace GUI
{
    public class ExplainTextBox : EditorTextBox
    {
        /// <summary>
        ///     Sets the model for this explain text box
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(IModelElement model)
        {
            Instance = model;
            RefreshData();
        }

        /// <summary>
        ///     The explanation last time
        /// </summary>
        private string _lastExplanation = "";

        /// <summary>
        ///     Refreshes the data
        /// </summary>
        public virtual void RefreshData()
        {
            SuspendLayout();

            ITextualExplain explainable = Instance as ITextualExplain;

            if (explainable != null)
            {
                string text = TextualExplanationUtils.GetText(explainable, true);

                if (text != null)
                {
                    if (text != _lastExplanation)
                    {
                        _lastExplanation = text;
                        Text = text;
                    }
                }
                else
                {
                    _lastExplanation = "";
                    Text = "";
                }
            }
            else
            {
                _lastExplanation = "";
                Text = "";
            }

            ResumeLayout();
        }
    }
}