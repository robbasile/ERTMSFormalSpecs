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
using DataDictionary.Tests;

namespace GUI.EditorView
{
    /// <summary>
    ///     Sets the string value into the right property
    /// </summary>
    public class ExpressionableTextChangeHandler : Window.HandleTextChange
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="title"></param>
        public ExpressionableTextChangeHandler(ModelElement instance, string title = "Expression")
            : base(instance, title)
        {
        }

        /// <summary>
        ///     The way text is retrieved from the instance
        /// </summary>
        /// <returns></returns>
        public override string GetText()
        {
            string retVal = "";
            IExpressionable expressionable = Instance as IExpressionable;

            if (expressionable != null)
            {
                retVal = expressionable.ExpressionText;
            }
            return retVal;
        }

        /// <summary>
        ///     The way text is set back in the instance
        /// </summary>
        /// <returns></returns>
        public override void SetText(string text)
        {
            text = RemoveUselessCharacters(text);

            IExpressionable expressionable = Instance as IExpressionable;
            if (expressionable != null)
            {
                string originalText = RemoveUselessCharacters(expressionable.ExpressionText);
                if (originalText != text)
                {
                    expressionable.ExpressionText = text;
                }
            }
        }
    }


    /// <summary>
    ///     Sets the string value into the right property
    /// </summary>
    public class ConditionTextChangeHandler : Window.HandleTextChange
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="instance"></param>
        public ConditionTextChangeHandler(ModelElement instance)
            : base(instance, "Condition")
        {
        }

        /// <summary>
        ///     The way text is retrieved from the instance
        /// </summary>
        /// <returns></returns>
        public override string GetText()
        {
            string retVal = "";
            Expectation expectation = Instance as Expectation;

            if (expectation != null)
            {
                retVal = expectation.getCondition();
            }
            return retVal;
        }

        /// <summary>
        ///     The way text is set back in the instance
        /// </summary>
        /// <returns></returns>
        public override void SetText(string text)
        {
            Expectation expectation = Instance as Expectation;

            if (expectation != null)
            {
                expectation.setCondition(text);
            }
        }
    }
}