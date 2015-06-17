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

namespace GUI
{
    public class ExplainTextBox : EditorTextBox
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public ExplainTextBox()
            : base()
        {
        }

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
        private string LastExplanation = "";

        /// <summary>
        ///     Refreshes the data
        /// </summary>
        public virtual void RefreshData()
        {
            SuspendLayout();

            TextualExplain explainable = Instance as TextualExplain;

            if (explainable != null)
            {
                string explanation = explainable.getExplain(true);

                if (explanation != null)
                {
                    if (explanation != LastExplanation)
                    {
                        LastExplanation = explanation;
                        Rtf = explanation;
                    }
                }
                else
                {
                    LastExplanation = "";
                    Rtf = "";
                }
            }
            else
            {
                LastExplanation = "";
                Rtf = "";
            }

            ResumeLayout();
        }
    }
}