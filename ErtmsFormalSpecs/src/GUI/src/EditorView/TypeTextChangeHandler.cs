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

namespace GUI.EditorView
{
    /// <summary>
    ///     Sets the string value into the right property
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="value"></param>
    public class TypeTextChangeHandler : Window.HandleTextChange
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="instance"></param>
        public TypeTextChangeHandler(ModelElement instance)
            : base(instance, "Type")
        {
        }

        /// <summary>
        ///     The way text is retrieved from the instance
        /// </summary>
        /// <returns></returns>
        public override string GetText()
        {
            string retVal = "";
            ITypedElement typedElement = Instance as ITypedElement;

            if (typedElement != null)
            {
                retVal = typedElement.TypeName;
            }
            return retVal;
        }

        /// <summary>
        ///     The way text is set back in the instance
        /// </summary>
        /// <returns></returns>
        public override void SetText(string text)
        {
            ITypedElement typedElement = Instance as ITypedElement;

            if (typedElement != null)
            {
                typedElement.TypeName = text;
            }
        }
    }
}