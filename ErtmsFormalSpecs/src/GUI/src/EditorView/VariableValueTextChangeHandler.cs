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
using DataDictionary.Values;
using DataDictionary.Variables;

namespace GUI.EditorView
{
    /// <summary>
    ///     Sets the string value into the right property
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="value"></param>
    public class VariableValueTextChangeHandler : Window.HandleTextChange
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="instance"></param>
        public VariableValueTextChangeHandler(ModelElement instance)
            : base(instance, "Variable value")
        {
        }

        /// <summary>
        ///     The way text is retrieved from the instance
        /// </summary>
        /// <returns></returns>
        public override string GetText()
        {
            string retVal = "";
            IVariable variable = Instance as IVariable;

            if (variable != null)
            {
                retVal = variable.Value.LiteralName;
            }
            return retVal;
        }

        /// <summary>
        ///     The way text is set back in the instance
        /// </summary>
        /// <returns></returns>
        public override void SetText(string text)
        {
            IVariable variable = Instance as IVariable;

            if (variable != null && variable.Type != null)
            {
                IValue value = variable.Type.getValue(text);
                if (value != null)
                {
                    variable.Value = value;
                }
            }
        }
    }
}